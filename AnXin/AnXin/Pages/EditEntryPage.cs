using System;
using Xamarin.Forms;
using PCLAnXin;
using System.Threading.Tasks;

namespace AnXin
{
	public class EditEntryPage : ContentPage
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
		ListEntryPage parentPage;
		TblPassAccount data;

		public EditEntryPage (ListEntryPage parentPage, TblPassAccount data)
		{
			this.parentPage = parentPage;
			this.data = data;

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
				TextColor = Color.White,
				Text = AppResources.FormUpdate,
				BackgroundColor = StyleHelpers.BlueButton,
				WidthRequest = 180,
				HorizontalOptions = LayoutOptions.Center,
			};

			btnSave.Clicked += BtnUpdate_Clicked;

			mainLayout = new StackLayout {
				Padding = new Thickness(10,20,10,10),
				Spacing = 20,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {entryAccountName, entryLoginName, passwordLayout, lblPasswordLen, entryComments, btnSave}
			};

			Content = mainLayout;
		}


		async void BtnUpdate_Clicked (object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty (entryAccountName.Text) || string.IsNullOrEmpty (entryLoginName.Text) || string.IsNullOrEmpty (entryPassword.Text)) {
				await DisplayAlert ("Error", "Please complete the form", "OK");
			} else {
				var result = await PassAccountManager.UpdatePassAccount (data.ID, entryAccountName.Text, entryLoginName.Text, entryPassword.Text, entryComments.Text);
				if (result.Item2 == PassAccountStatus.Ok) {
					await DisplayAlert ("Saved", "You entry has been encrypted and updated successfully", "OK");
				} else if (result.Item2 == PassAccountStatus.AuthError) {
					await DisplayAlert ("Error", "Authentication Error", "OK");
				} else if (result.Item2 == PassAccountStatus.Offline) {
					await DisplayAlert ("Internet connection", "Offline. Internet connection is required", "OK");
				} else if (result.Item2 == PassAccountStatus.Error) {
					await DisplayAlert ("Error", "Error", "OK");
				} else {
					await DisplayAlert ("Error", "Error", "OK");
				}
			
				((ListEntryPage)parentPage).PopulateData ();
				Navigation.PopToRootAsync (true);
			}	
		}
			
		protected override async void OnAppearing ()
		{
			base.OnAppearing ();

			entryAccountName.Text = data.AccountName;
			entryLoginName.Text = data.LoginName;
			entryPassword.Text = data.AnXin;
			entryComments.Text = data.Comments;
		}
	}
}

