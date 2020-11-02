using B4.Mope.Packaging;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace B4.Mope
{
	public class WebHost : IDisposable
	{
		HttpListener HttpListener { get; } = new HttpListener();
		public int Port { get; }
		public Data Data { get; }
		private bool m_stopped;
		//Thread m_listeningThread;

		public string GetUrl(string relativePath)
		{
			return $"http://127.0.0.1:{Port}/{relativePath}";
		}

		private bool m_isDisposed;
		private bool m_paused;

		public WebHost(Data data, int? port = null)
		{
			if (port != null)
			{
				Port = port.Value;
			}
			else
			{
				// randomly generate a port number between 49152–65535
				var rand = new Random();
				Port = rand.Next(49152, 65535);
			}
			HttpListener.Prefixes.Add($"http://127.0.0.1:{Port}/");
			Data = data ?? throw new ArgumentNullException(nameof(data));
		}

		public void ListenOnThread()
		{
			//m_listeningThread = new Thread(Listen);
			//m_listeningThread.IsBackground = true;
			//m_listeningThread.Start();
			Listen();
		}

		private static string GetContentType(string url)
		{
			string ext = Path.GetExtension(url);
			switch (ext)
			{
				case ".js":
					return "application/javascript";
				case ".css":
					return "text/css";
				case ".html":
					return "text/html";
				case ".htm":
					return "text/html";
				case ".ttf":
					return "application/x-font-ttf";
				default:
					return string.Empty;
			}
		}

		private void Listen()
		{
			HttpListener.Start();

			var asyncResult = HttpListener.BeginGetContext(new AsyncCallback(ListenerCallback), this);

		}

		private void HandleRequest(HttpListenerContext context)
		{
			if (m_paused)
			{
				context.Response.StatusCode = 500;
				return;
			}

			try
			{
				var url = context.Request.Url;
				if (url.LocalPath.StartsWith("/part/"))
				{
					SetResponseToPart(context, url);
				}
				else if (url.LocalPath.StartsWith("/post/"))
				{
					ReadPartFromRequest(context, url);
				}
				else if (url.LocalPath.StartsWith("/dirty/"))
				{
					HandleDirtyRequest(context, url);
				}
				else
				{
					using (var resourceStream = System.Windows.Application.GetResourceStream(new Uri($"pack://application:,,,/MOPE;component/{url.LocalPath}")).Stream)
					{
						//context.Response.ContentLength64 = resourceStream.Length;
						context.Response.ContentType = GetContentType(url.LocalPath);
						resourceStream.CopyTo(context.Response.OutputStream);
						context.Response.OutputStream.Close();
					}
				}
			}
			catch (IOException)
			{
				// error
				context.Response.StatusCode = 404;
			}
			catch (InvalidOperationException exc)
			{
				// if app is being shutdown stop
				if (exc.HResult == -2146233079)
				{
					m_stopped = true;
				}
				else
				{
					throw;
				}
			}
		}

		private void HandleDirtyRequest(HttpListenerContext context, Uri url)
		{
			var partUri = url.LocalPath.Substring(7);
			var partModel = Data.PartModels[partUri];

			partModel.SetDirty(true);
			context.Response.StatusCode = 200;
			context.Response.OutputStream.Close();

			Data.IsPackageDirty = true;
		}

		private static void ListenerCallback(IAsyncResult result)
		{
			try
			{
				var webHost = (WebHost)result.AsyncState;
				var context = webHost.HttpListener.EndGetContext(result);
				webHost.HandleRequest(context);

				if (!webHost.m_stopped)
				{
					var asyncResult = webHost.HttpListener.BeginGetContext(new AsyncCallback(ListenerCallback), webHost);
				}
				else
				{
					webHost.HttpListener.Stop();
				}
			}
			catch (HttpListenerException)
			{
				if (Debugger.IsAttached)
					Debugger.Break();
			}
		}

		private void ReadPartFromRequest(HttpListenerContext context, Uri url)
		{
			var partUri = url.LocalPath.Substring(6);

			var partModel = Data.PartModels[partUri];
			var file = partModel.Part.GetFileInfo();
			// okay so I'm obviously doing something dumb, but just writing to the filestream results in weird behavior,
			// so delete and then recreate the file
			file.Delete();
			using (var stream = new FileStream(file.FullName, FileMode.Create))
			//using (var writer = new StreamWriter(stream))
			//{
			//	using (var reader = new StreamReader(context.Request.InputStream))
			//	{
			//		var contents = reader.ReadToEnd();
			//		writer.Write(contents);
					context.Request.InputStream.CopyTo(stream);
			//	}
			//}

			Data.IsPackageDirty = true;
			partModel.SetDirty(false);
			context.Response.StatusCode = 200;
			context.Response.OutputStream.Close();
		}

		private void SetResponseToPart(HttpListenerContext context, Uri url)
		{
			var partUri = url.LocalPath.Substring(6);
			var part = Data.Package.Parts[partUri];

			context.Response.ContentType = part.ContentType;

			using (var partStream = part.GetFileInfo().OpenRead())
			{
				if (Data.Settings.EditorFormatXmlOnLoad && ContentTypes.IsXmlType(part.ContentType))
				{
					var writerSettings = new XmlWriterSettings()
					{
						Indent = true,
					};

					using (var textReader = new StreamReader(partStream))
					using (var xmlWriter = XmlWriter.Create(context.Response.OutputStream, writerSettings))
					{
						var elt = XElement.Parse(textReader.ReadToEnd());
						elt.Save(xmlWriter);
					}
				}
				else
				{
					partStream.CopyTo(context.Response.OutputStream);
				}

				context.Response.StatusCode = 200;
				context.Response.OutputStream.Close();
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!m_isDisposed)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
					HttpListener.Stop();
					((IDisposable)HttpListener).Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				m_isDisposed = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~WebHost()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			m_stopped = true;
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void Pause()
		{
			m_paused = true;
		}

		public void Resume()
		{
			m_paused = false;
		}

		public void Stop()
		{
			m_stopped = true;
		}
	}
}
