using System;
using Xamarin.Forms;
using System.Diagnostics;
using PCLAnXin;

namespace AnXin
{
	public class TabPage : TabbedPage
	{
		public TabPage ()
		{
			Debug.WriteLine ("CurrentUserId : " + PassAccountManager.CurrentUserId.ToString());
			this.Children.Add (new ListEntryPage(this){Title=AppResources.NavHome});
			this.Children.Add (new AddEntryPage(this){Title=AppResources.NavAddEntry});
			this.Children.Add (new LogoutPage(this){Title=AppResources.NavLogout, Icon="ic_lock_black.png"});
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
		}
	}
}

