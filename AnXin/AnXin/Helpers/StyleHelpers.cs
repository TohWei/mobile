using System;
using Xamarin.Forms;

namespace AnXin
{
	public static class StyleHelpers
	{
		public static Color ContentPageBgColor = Color.FromHex("#e3e3e3");
		public static Color BlueButton = Color.FromHex("#2B79E5");

		public static BoxView HorizontalLine = new BoxView{
			HeightRequest = 1,
			BackgroundColor = Color.Gray
		};
	}
}

