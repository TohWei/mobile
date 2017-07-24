using System;
using UIKit;
using Foundation;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Xamarin.Forms;
using PCLAnXin;
using AnXin.iOS;

[assembly: DependencyAttribute(typeof(InfoServiceIOS))]
namespace AnXin.iOS
{
	public class InfoServiceIOS : IInfoService
	{
		public InfoServiceIOS ()
		{
		}

		#region IInfoService implementation

		public string DeviceName{
			get {
				return UIDevice.CurrentDevice.Name.ToString ();
			}
		}

		public string DeviceId {
			get {
				return UIDevice.CurrentDevice.IdentifierForVendor.AsString();
			}
		}

		public string PackageName {
			get {
				return NSBundle.MainBundle.InfoDictionary ["CFBundleDisplayName"].ToString ();
			}
		}

		//Same as app bundle version.
		public string AppVersionName {
			get {
				return NSBundle.MainBundle.InfoDictionary ["CFBundleVersion"].ToString ();
			}
		}

		//Same as build version.
		public string AppVersionCode {
			get {
				return NSBundle.MainBundle.InfoDictionary ["CFBundleShortVersionString"].ToString ();
			}
		}

		public double DeviceScreenWidth {
			get {
				throw new NotImplementedException ();
			}
		}

		public double DeviceScreenHeight {
			get {
				throw new NotImplementedException ();
			}
		}

		public string FirmwareVersion {
			get {
				return UIDevice.CurrentDevice.SystemVersion;
			}
		}

		public string HardwareVersion {
			get {
				var hardwareVersion = GetSystemProperty("hw.machine");
				return hardwareVersion;
			}
		}

		public string Manufacturer {
			get
			{
				return "Apple";
			}
		}

		public string ModelName {
			get {
				return UIDevice.CurrentDevice.Model;
			}
		}


		//The totalMemory for iOS is different from Android.
		//This totalMemory = total space available in special folder that can be used by user. 
		ulong capacity, available;
		public long TotalMemory {
			get {
				GetCapacity (out capacity, out available);
				return (long)capacity;
			}
		}

		public double TimeZoneOffset {
			get { return NSTimeZone.LocalTimeZone.GetSecondsFromGMT / 3600.0; }
		}

		public string TimeZone {
			get { return NSTimeZone.LocalTimeZone.Name; }
		}
		#endregion

		private static void GetCapacity(out ulong capacity, out ulong available){
			var space = NSFileManager.DefaultManager.GetFileSystemAttributes (Environment.GetFolderPath (Environment.SpecialFolder.Personal));
			capacity = space.Size;
			available = space.FreeSize;
		}

		public static string GetSystemProperty(string property)
		{
			var pLen = Marshal.AllocHGlobal(sizeof(int));
			sysctlbyname(property, IntPtr.Zero, pLen, IntPtr.Zero, 0);
			var length = Marshal.ReadInt32(pLen);
			var pStr = Marshal.AllocHGlobal(length);
			sysctlbyname(property, pStr, pLen, IntPtr.Zero, 0);
			return Marshal.PtrToStringAnsi(pStr);
		}

		[DllImport(Constants.SystemLibrary)]
		internal static extern int sysctlbyname(
			[MarshalAs(UnmanagedType.LPStr)] string property,
			IntPtr output,
			IntPtr oldLen,
			IntPtr newp,
			uint newlen);
	}
}

