using System;
using Xamarin.Forms;
using PCLAnXin;
using System.Diagnostics;

namespace AnXin
{
	public class LandingPage : ContentPage
	{
		Frame masterPasswordFrame, newCustomerFrame;
		StackLayout mainLayout, newUserSectionLayout, notesHolder;
		Label lblEmailaddress;
		private object syncLock = new object();
		bool isInCall = false;

		public LandingPage ()
		{
			BackgroundColor = StyleHelpers.ContentPageBgColor;
			BackgroundImage = "bg.jpg";


			Entry entryMasterPassword = new Entry {
				IsPassword = true,
				TextColor = Color.Gray,
				WidthRequest = 200,
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = StyleHelpers.ContentPageBgColor
			};

			entryMasterPassword.Focused += (sender, e) => {
				if (newCustomerFrame.IsVisible){
					newCustomerFrame.IsVisible = false;
					notesHolder.IsVisible = true;
				}
			};

			Label lblMasterPassword = new Label {
				Text = AppResources.MasterPassword,
				TextColor = Color.Black,
				XAlign = TextAlignment.Center,
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
			};

			Button btn = new Button {
				BackgroundColor = StyleHelpers.BlueButton,
				Text = AppResources.Start,
				TextColor = Color.White,
				WidthRequest = 200,
				HorizontalOptions = LayoutOptions.Center
			};



			btn.Clicked += async(sender, e) => {
				lock(syncLock)
				{
					if(isInCall)
						return;
					isInCall = true;
				}
				try{
					if (entryMasterPassword.Text!= null){
						int userId = await PassAccountManager.Validate(entryMasterPassword.Text);
						if (userId > 0){
							Navigation.PushModalAsync(new NavigationPage(new TabPage()) );
						}else{
							DisplayAlert("Error", "Invalid master password","OK");
						}
					}
				}finally{
					lock(syncLock)
					{
						isInCall = false;
					}
				}
			};

			masterPasswordFrame = new Frame {
				BackgroundColor = Color.White,
				OutlineColor = Color.Gray,
				HasShadow = false,
				Padding = 10,
				Opacity = 0.8,
				Content = new StackLayout{
					Children = {lblMasterPassword, entryMasterPassword, btn}
				}
			};

			Label lblNewUserLink = new Label {
				Text = AppResources.NewUser,
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				HeightRequest = 50,
			};
			lblNewUserLink.GestureRecognizers.Add (
				new TapGestureRecognizer (sender => {
					lblNewUserLink.Opacity = 0.6;
					lblNewUserLink.FadeTo(1);

					newCustomerFrame.IsVisible = true;
					notesHolder.IsVisible = false;
				})
			);


				
			mainLayout = new StackLayout {
				Padding = new Thickness(10,30,10,5),
				Children = {masterPasswordFrame, lblNewUserLink, FeatruesSection(), NewUserSection()}
			};

			Content = mainLayout;
		}

		private StackLayout FeatruesSection(){
			Label lblNote = new Label {
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				TextColor = Color.White,
				XAlign = TextAlignment.Start,
			};
			lblNote.Text += AppResources.featureNote1 + "\n\n";
			lblNote.Text += AppResources.featureNote2 + "\n\n";
			lblNote.Text += AppResources.featureNote3 + "\n\n\n";
			lblNote.Text += AppResources.featureNote4 + "\n\n\n";
			lblNote.Text += AppResources.featureNote5 + "\n\n";

			notesHolder = new StackLayout {
				Padding = 20,
				Opacity = 0.4,
				BackgroundColor = Color.Black,
				Children = {lblNote}
			};
			return notesHolder;
		}

		private Frame NewUserSection(){
			Label lblEnterMasterPassword = new Label {
				Text = AppResources.CreateMasterPassword,
				TextColor = Color.Black,
				XAlign = TextAlignment.Center,
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
			};

			Entry entryNewMasterPassword = new Entry {
				Placeholder = AppResources.MaxChars,
				TextColor = Color.Gray,
				WidthRequest = 200,
				BackgroundColor = StyleHelpers.ContentPageBgColor
			};

			lblEmailaddress = new Label {
				Text = AppResources.SendPasswordToYourEmail,
				TextColor = Color.Black,
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
				XAlign = TextAlignment.Center
			};

			Entry entryEmailAddress = new Entry {
				Placeholder = AppResources.YourEmailAddress,
				TextColor = Color.Gray,
				WidthRequest = 200,
				BackgroundColor = StyleHelpers.ContentPageBgColor
			};

			Button btnAddNewMasterPassword = new Button {
				Text = AppResources.FormSave,
				TextColor = Color.White,
				BackgroundColor = StyleHelpers.BlueButton,
				WidthRequest = 200,
				HorizontalOptions = LayoutOptions.Center
			};

			btnAddNewMasterPassword.Clicked += async(sender, e) => {
				
				lock(syncLock)
				{
					if(isInCall)
						return;
					isInCall = true;
				}
				try{
					if (entryNewMasterPassword.Text != null && entryEmailAddress.Text != null)
					{
						if (!ConnectionHelper.Online)
						{
							DisplayAlert(AppResources.NoInternetConnection, "Please enable your internet connection for us to send the email","OK");
							return;
						}

						var res = await PassAccountManager.CreateUser(entryNewMasterPassword.Text);
						if (res){
							var masterPass = entryNewMasterPassword.Text.Trim();
							var receiverEmail = entryEmailAddress.Text.Trim();

							DisplayAlert(AppResources.MasterPasswordSet, AppResources.MasterPasswordSetDesc + " " + receiverEmail+".","OK");

							var mailKitHelper = DependencyService.Get<IMailKitHelper> ();
							mailKitHelper.SendYahooEmail(receiverEmail, masterPass);
							
							newCustomerFrame.IsVisible = false;
							entryNewMasterPassword.Text = "";
						}else{
							DisplayAlert("Oops", "Unable to create master password. ","OK");
						}
					}
				}finally{
					lock(syncLock)
					{
						isInCall = false;
					}
				}
			};

			newUserSectionLayout = new StackLayout {
				Padding = 10,
				Children = {lblEnterMasterPassword, entryNewMasterPassword, lblEmailaddress, entryEmailAddress, btnAddNewMasterPassword}
			};

			newCustomerFrame = new Frame {
				BackgroundColor = Color.White,
				OutlineColor = Color.Gray,
				HasShadow = false,
				Opacity = 0.8,
				Padding = 10,
				Content = new StackLayout{
					Children = {newUserSectionLayout}
				}
			};
			newCustomerFrame.IsVisible = false;

			return newCustomerFrame;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
		}
	}
}

