using System;
using System.Net;
using SystemConfiguration;
using CoreFoundation;
using Xamarin.Forms;
using AnXin.iOS;
using PCLAnXin;

[assembly: Dependency(typeof(DSConnection))]
namespace AnXin.iOS {

	public class DSConnection : IConnection
	{
		public bool Online { 
			get {
				return Reachability.InternetConnectionStatus () != NetworkStatus.NotReachable;
			}
		}
	}
}