using System;
using Xamarin.Forms;
using System.Diagnostics;
using PCLAnXin;

namespace AnXin
{
	public class ItemPassAccountListCell : ViewCell
	{
		public static Page ParentPage { get; set; }
		Label lblAccountName;
		Label lblLoginName;
		Label lblEncryptedPass;
		public Image imgIcon;

		StackLayout textHolderLayout, imageHolder;

		public ItemPassAccountListCell()
		{
			imgIcon = new Image{
				Source = FileImageSource.FromFile ("ic_lock_black.png"),
				BackgroundColor = Color.Transparent,
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			imageHolder = new StackLayout {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = 60,
				HeightRequest = 60,
				BackgroundColor = Color.Transparent,
				Children = {imgIcon}
			};

			lblAccountName = new Label{
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.Black
			};
			lblLoginName = new Label{
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				TextColor = Color.Black
			};

			lblEncryptedPass = new Label {
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				TextColor = Color.Black
			};


			textHolderLayout = new StackLayout () {
				Padding = 5,
				Spacing = 1,
				BackgroundColor = Color.Transparent,
				Children = {
					lblAccountName,
					lblLoginName,
					lblEncryptedPass,
				},
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.End,
				IsClippedToBounds = true
			};	

			Grid grid = new Grid {
				Padding = 5,
				RowDefinitions = {new RowDefinition{Height = 80}},
				ColumnDefinitions = {
					new ColumnDefinition{Width = new GridLength(2, GridUnitType.Star)},
					new ColumnDefinition{Width = new GridLength(8, GridUnitType.Star)},
				},
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				Opacity = 0.6
			};

			grid.Children.Add (imageHolder, 0, 0);
			grid.Children.Add (textHolderLayout, 1, 0);

			View = grid;


			//------ Creating Contact Action 1 Start --------//
			var delAction = new MenuItem { Text = AppResources.FormDelete, IsDestructive = true }; //Red background.
			delAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			delAction.Clicked += async (sender, e) => {
				var mi = ((MenuItem)sender);
				Debug.WriteLine ("More Context Action clicked: " + mi.CommandParameter);
				var selectedRec = mi.CommandParameter as TblPassAccount;
				var confirmToDel = await ParentPage.DisplayAlert(AppResources.FormDelete,"Please confirm to delete " + selectedRec.AccountName , "OK", "Cancel");
				if (confirmToDel){
					PassAccountManager.DelPassAccount(selectedRec.ID);
					((ListEntryPage)ParentPage).PopulateData();
				}

			};
			ContextActions.Add (delAction);
			//------ Creating Contact Action 1 Start --------//
		}


		protected override void OnBindingContextChanged ()
		{
			base.OnBindingContextChanged ();

			var accountPassData = this.BindingContext as TblPassAccount;

			if (accountPassData != null) {
				lblAccountName.Text = accountPassData.AccountName;
				lblLoginName.Text = accountPassData.LoginName;
				lblEncryptedPass.Text = accountPassData.EncryptedPass;
			}
		}
	}
}

