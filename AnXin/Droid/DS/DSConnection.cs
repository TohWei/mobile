using System;
using System.Net;
using Xamarin.Forms;
using Android.Net;
using Android.Content;
using AnXin.Droid;
using PCLAnXin;

[assembly: Dependency(typeof(DSConnection))]
namespace AnXin.Droid {

	public class DSConnection : IConnection
	{
		public static ConnectivityManager connectivityManager;

		public bool Online { 
			get {
				if (connectivityManager == null) {
					connectivityManager = (ConnectivityManager)Forms.Context.GetSystemService (Context.ConnectivityService);
				}
				var activeConnection = connectivityManager.ActiveNetworkInfo;

				return ((activeConnection != null) && activeConnection.IsConnected);
			}
		}
	}
}