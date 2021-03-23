using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B4.Mope.Utility
{
	public class WaitOnPartsNotDirty
	{
		private List<PartModel> m_dirtyParts = new List<PartModel>();

		public delegate void AllPartsNotDirtyEventHandler(object sender, EventArgs e);
		public event AllPartsNotDirtyEventHandler AllPartsNotDirty;

		public bool HasDirtyParts => m_dirtyParts.Count > 0;

		public WaitOnPartsNotDirty()
		{

		}

		public void AddPart(PartModel part)
		{
			if (!part.IsDirty)
				return;

			m_dirtyParts.Add(part);
			part.DirtyChanged += Part_DirtyChanged;
		}

		private void Part_DirtyChanged(object sender, EventArgs e)
		{
			var partModel = (PartModel)sender;
			if (!m_dirtyParts.Contains(partModel))
				return;

			m_dirtyParts.Remove(partModel);
			partModel.DirtyChanged -= Part_DirtyChanged;

			if (m_dirtyParts.Count == 0)
			{
				AllPartsNotDirty?.Invoke(this, new EventArgs());
			}
		}
	}
}
