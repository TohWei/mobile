using System;
using Xamarin.Forms;

namespace AnXin
{
	public class LogoutPage : ContentPage
	{

		Button btnLogout;

		public LogoutPage (TabPage parentPage)
		{
			BackgroundColor = Color.White;
			BackgroundImage = "bg.jpg";

			btnLogout = new Button {
				BackgroundColor = StyleHelpers.BlueButton,
				Text = AppResources.NavLogout,
				TextColor = Color.White
			};

			btnLogout.Clicked += (sender, e) => {
				Application.Current.MainPage = new LandingPage();
			};


			Label lblNote = new Label {
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				TextColor = Color.White,
				XAlign = TextAlignment.Start,
			};
			lblNote.Text += AppResources.featureNote1 + "\n\n";
			//lblNote.Text += "Main feature:\n\n";
			//lblNote.Text += "No integration with any 3rd parties services. \n";
			//lblNote.Text += "Simple model, and does not required any setup. \n\n";
			//lblNote.Text += "How it works?\n\n";
			//lblNote.Text += "Encrypt/decrypt your data with 256 bits AES Key. ";
			lblNote.Text += AppResources.featureNote2 + "\n\n";
			lblNote.Text += AppResources.featureNote3 + "\n\n\n";
			lblNote.Text += AppResources.featureNote4 + "\n\n\n";
			lblNote.Text += AppResources.featureNote5 + "\n\n";

			var notesHolder = new StackLayout {
				Padding = 20,
				Children = {lblNote}
			};

			Content = new StackLayout {
				Padding = 20,
				Children = {btnLogout, notesHolder}
			};
		}
	}
}

