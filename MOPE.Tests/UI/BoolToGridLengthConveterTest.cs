using B4.Mope.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Xunit;

namespace MOPE.Tests.UI
{
	public class BoolToGridLengthConverterTest
	{
		[Fact]
		public void TestFalseEqualsStar()
		{
			var converter = new BoolToGridLengthConverter();
			var result = (GridLength)converter.Convert(false, typeof(GridLength), null, null);
			Assert.True(result.IsStar);
			Assert.Equal(1, result.Value);
		}

		[Fact]
		public void TestFalseEqualsAuto()
		{
			var converter = new BoolToGridLengthConverter();
			var result = (GridLength)converter.Convert(true, typeof(GridLength), null, null);
			Assert.True(result.IsAuto);
		}
	}
}
