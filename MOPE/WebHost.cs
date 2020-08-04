using B4.Mope.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Resources;
using System.Text;
using System.Threading;
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



		private static void ListenerCallback(IAsyncResult result)
		{
			var webHost = (WebHost)result.AsyncState;
			var context = webHost.HttpListener.EndGetContext(result);

			try
			{
				var url = context.Request.Url;
				if (url.LocalPath.StartsWith("/part/"))
				{
					var partUri = url.LocalPath.Substring(6);
					var part = webHost.Data.Package.Parts[partUri];
					using (var partStream = part.GetFileInfo().OpenRead())
					{
						if (part.IsXml())
						{
							context.Response.ContentType = "application/xml";
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
							context.Response.ContentType = part.ContentType;
							partStream.CopyTo(context.Response.OutputStream);
						}

						context.Response.OutputStream.Close();
					}
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

			if (!webHost.m_stopped)
			{
				var asyncResult = webHost.HttpListener.BeginGetContext(new AsyncCallback(ListenerCallback), webHost);
			}
			else
			{
				webHost.HttpListener.Stop();
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

		public void Stop()
		{
			m_stopped = true;
		}
	}
}
