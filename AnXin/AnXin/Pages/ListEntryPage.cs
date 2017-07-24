using System;
using Xamarin.Forms;
using PCLAnXin;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AnXin
{
	public class ListEntryPage : ContentPage
	{
		ListView listView;
		StackLayout mainLayout;
		TabPage parentPage;

		public ListEntryPage (TabPage parentPage)
		{
			this.parentPage = parentPage;

			BackgroundColor = StyleHelpers.ContentPageBgColor;
			BackgroundImage = "bg.jpg";

			listView = new ListView {
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};
			listView.HasUnevenRows = true;
			listView.ItemTapped += ListView_ItemTapped;

			mainLayout = new StackLayout {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {listView}
			};

			Content = mainLayout;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			listView.ItemTemplate = new DataTemplate (typeof(ItemPassAccountListCell));
			ItemPassAccountListCell.ParentPage = this;
			PopulateData ();
		}

		public void PopulateData(){
			
			Task.Delay (250).ContinueWith (async t => {
				Device.BeginInvokeOnMainThread (async() => {
					if (t.IsCompleted){
						var result = await PassAccountManager.GetPassAccountList ();
						if (result.Item2 == PassAccountStatus.Ok && result.Item1 != null) {
							Debug.WriteLine("result.Item1: " + result.Item1.Count().ToString());
							//listView.ItemsSource = result.Item1.OrderBy (x => x.AccountName);
							//listView.ItemsSource = result.Item1.Where(x=>x.AccountName != null).OrderBy(x=>x.AccountName);

							listView.ItemsSource = result.Item1;
						} else if (result.Item2 == PassAccountStatus.Ok && result.Item1 == null) {
							listView.ItemsSource = null;
							mainLayout.Children.Clear ();

							var msgLayout = new Label {
								Text = "No record",
								TextColor = Color.Black,
								BackgroundColor = Color.White
							};
							mainLayout.Children.Add (msgLayout);
						} else if (result.Item2 == PassAccountStatus.AuthError) {
							await DisplayAlert ("Auth Error", "Authentication error", "OK");
						} else if (result.Item2 == PassAccountStatus.Offline) {
							await DisplayAlert ("OffLine", "Internet connection required", "OK");
						} else if (result.Item2 == PassAccountStatus.Error) {
							await DisplayAlert ("Error", "error", "OK");
						} else {
							await DisplayAlert ("Error", "error", "OK");
						}
					}else {
						await DisplayAlert ("Timeout", "error", "OK");
					}
				});
			});
		}


		void ListView_ItemTapped (object sender, ItemTappedEventArgs e)
		{
			var data = e.Item as TblPassAccount;
			Navigation.PushAsync (new EditEntryPage (this, data));
		}
	}
}

