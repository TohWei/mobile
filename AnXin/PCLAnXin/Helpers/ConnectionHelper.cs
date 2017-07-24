using System;
using Xamarin.Forms;

namespace PCLAnXin
{
	public class ConnectionHelper
	{
		private static IConnection connection;

		public static bool Online { 
			get {
				if (connection == null) {
					connection = DependencyService.Get<IConnection>();
				}	
				return connection.Online;
			}
		}
	}
}

