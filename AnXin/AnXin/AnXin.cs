using System;

using Xamarin.Forms;
using PCLAnXin;
using System.Diagnostics;

namespace AnXin
{
	public class App : Application
	{
		//Note: Icon is made via a website:
		//https://romannuril.github.io/AndroidAssetStudio/icons-launcher.html
		//Background color: \#ff9800

		public App ()
		{
			MainPage = new LandingPage ();
			// The root page of your application
			/*
			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Children = {
						new Label {
							XAlign = TextAlignment.Center,
							Text = "Welcome to Xamarin Forms!"
						}
					}
				}
			};
			*/

			//Init the network connectivity status. This is required by iOS. 
			bool isOnline = ConnectionHelper.Online;
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}



		#region These action will be called by Reachability for iOS and NetworkStateReceiver for Android whenever a network status changed..

		public static Action InternetConnOnlineAction {
			set { }
			get {
				return new Action (() => {
					Debug.WriteLine ("Online");
				});
			}
		}

		public static Action InternetConnOfflineAction {
			set { }
			get {
				return new Action (() => {
					Debug.WriteLine("Offline");
				});
			}
		}

		#endregion
	}
}

