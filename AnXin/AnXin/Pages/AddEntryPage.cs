using System;
using Xamarin.Forms;
using PCLAnXin;
using System.Threading.Tasks;

namespace AnXin
{
	public class AddEntryPage : ContentPage
	{
		StackLayout mainLayout;

		Entry entryAccountName;
		Entry entryLoginName;
		Entry entryPassword; 
		Entry entryComments;
		Image imgSeePassword;
		StackLayout passwordLayout;
		Label lblPasswordLen;
		Button btnSave;
		TabPage parentPage;

		public AddEntryPage (TabPage parentPage)
		{
			this.parentPage = parentPage;

			BackgroundColor = StyleHelpers.ContentPageBgColor;
			BackgroundImage = "bg.jpg";

			entryAccountName = new Entry {
				TextColor = Color.White,
				Placeholder = AppResources.FormAccountName,
			};

			entryLoginName = new Entry {
				TextColor = Color.White,
				Placeholder = AppResources.FormUserName,
			};
				
			entryPassword = new Entry {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				TextColor = Color.White,
				Placeholder = AppResources.FormPassword,
				IsPassword = true,
			};
			entryPassword.TextChanged += (sender, e) => {
				lblPasswordLen.Text = e.NewTextValue.Length.ToString() + AppResources.characters;
				if (e.NewTextValue.Length > 30){
					DisplayAlert ("Oops", AppResources.MaxChars, "OK");
					entryPassword.Text = e.NewTextValue.Substring(0,30);
				}
			};

			imgSeePassword = new Image ();
			imgSeePassword.BackgroundColor = Color.White;
			imgSeePassword.Source = ImageSource.FromFile ("ic_visibility_black.png");
			imgSeePassword.GestureRecognizers.Add (
				new TapGestureRecognizer (sender => {
					imgSeePassword.Opacity = 0.6;
					imgSeePassword.FadeTo(1);

					if (entryPassword.IsPassword){
						entryPassword.IsPassword = false;
						imgSeePassword.Source = ImageSource.FromFile ("ic_visibility_off_black.png");
					}else{
						entryPassword.IsPassword = true;
						imgSeePassword.Source = ImageSource.FromFile ("ic_visibility_black.png");
					}
				})
			);

			passwordLayout = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Children  ={entryPassword, imgSeePassword}
			};


			lblPasswordLen = new Label {
				Text = "0" + AppResources.characters,
			};


			entryComments = new Entry {
				Placeholder = AppResources.notes,
				TextColor = Color.White
			};

			btnSave = new Button {
				Text = AppResources.FormSave,
				BackgroundColor = StyleHelpers.BlueButton,
				WidthRequest = 180,
				HorizontalOptions = LayoutOptions.Center,
				TextColor = Color.White
			};
					

			btnSave.Clicked += BtnSave_Clicked;

			mainLayout = new StackLayout {
				Padding = new Thickness(10,20,10,10),
				Spacing = 20,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {entryAccountName, entryLoginName, passwordLayout, lblPasswordLen, entryComments, btnSave}
			};

			Content = mainLayout;
		}


		async void BtnSave_Clicked (object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty (entryAccountName.Text) || string.IsNullOrEmpty (entryLoginName.Text) || string.IsNullOrEmpty (entryPassword.Text)) {
				await DisplayAlert ("Error", "Please complete the form", "OK");
			} else {
				var result = await PassAccountManager.StorePassAccount ( entryAccountName.Text, entryLoginName.Text, entryPassword.Text, entryComments.Text);
				if (result.Item2 == PassAccountStatus.Ok) {
					await DisplayAlert ("Saved", "Your entry has been encrypted and saved successfully", "OK");
				} else if (result.Item2 == PassAccountStatus.AuthError) {
					await DisplayAlert ("Error", "Authentication Error", "OK");
				} else if (result.Item2 == PassAccountStatus.Offline) {
					await DisplayAlert ("Internet connection", "Offline. Internet connection is required", "OK");
				} else if (result.Item2 == PassAccountStatus.Error) {
					await DisplayAlert ("Error", "Error", "OK");
				} else {
					await DisplayAlert ("Error", "Error", "OK");
				}
				//Redirect to first tab.
				Navigation.PopToRootAsync();
				Application.Current.MainPage = new NavigationPage(new TabPage());
				//Issue: This will create multiple tab. 
				//parentPage.CurrentPage = parentPage.Children [0];
			}	
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			entryAccountName.Focus ();
		}
	}
}

