using System;
using Xamarin.Forms;

namespace PCLAnXin
{
	public static class InfoServiceHelper
	{
		private static readonly IInfoService infoService;

		static InfoServiceHelper ()
		{
			infoService = DependencyService.Get<IInfoService> ();
		}

		public static string AppVersion { 
			get {
				return infoService.AppVersionName;
			}
		}
			
		//Need to remove  slash ("/"), semicolon (";"), round brackets or any whitespace.
		public static string GetArtesianUserAgent()
		{	
			if (infoService.Manufacturer.ToLower ().Equals ("apple")) {
				return string.Format ("Test/{0} (IOS/{1}; {2})", tuneStrVal(infoService.AppVersionName), tuneStrVal(infoService.FirmwareVersion), tuneStrVal(infoService.ModelName));
			} else {
				return string.Format ("Test/{0} (Android/{1}; {2})", tuneStrVal(infoService.AppVersionName), tuneStrVal(infoService.FirmwareVersion), tuneStrVal(infoService.ModelName));
			}
		}

		public static string GetTimeZone(){
			return infoService.TimeZone;
		}

		public static string GetTimeZoneOffset(){
			return infoService.TimeZoneOffset.ToString();
		}

		private static string tuneStrVal(string txt){
			txt = txt.Replace ("/", " ").Replace (";", " ").Replace ("(", " ").Replace (")", " ");
			return txt.Trim ();
		}
	}
}

