using System;
using System.Collections.Generic;
using System.Text;

namespace B4.Mope
{
	public class BooleanPropertyChangedEventArgs : EventArgs
	{
		public bool OldValue { get; }
		public bool NewValue { get; }

		public BooleanPropertyChangedEventArgs(bool oldValue, bool newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
