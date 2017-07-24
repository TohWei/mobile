using System;
using Xamarin.Forms;
using Android.Provider;
using Android.OS;
using Java.IO;
using Java.Util;
using Java.Util.Concurrent;
using PCLAnXin.Droid;

[assembly: DependencyAttribute(typeof(InfoServiceDroid))]
namespace PCLAnXin.Droid
{
	public class InfoServiceDroid : IInfoService
	{
		public InfoServiceDroid ()
		{
		}

		#region IInfoService implementation
		public string DeviceName {
			get {
				return Build.Device;
			}
		}

		public string DeviceId {
			get {
				return Settings.Secure.GetString (Forms.Context.ContentResolver, Settings.Secure.AndroidId);
			}
		}

		public string PackageName {
			get {
				return Forms.Context.PackageName;
			}
		}

		public string AppVersionName {
			get {
				return Forms.Context.PackageManager.GetPackageInfo (Forms.Context.PackageName, 0).VersionName;
			}
		}

		public string AppVersionCode {
			get {
				return Forms.Context.PackageManager.GetPackageInfo (Forms.Context.PackageName, 0).VersionCode.ToString();
			}
		}
			
		public string FirmwareVersion { 
			get{
				return Build.VERSION.Release;
			}
		}

		public string HardwareVersion { 
			get {
				return Build.Hardware;
			}
		}

		public string Manufacturer { 
			get{
				return Build.Manufacturer;
			}
		}

		public string ModelName {
			get {
				return Build.Model;
			}
		}

		public double TimeZoneOffset
		{
			get
			{
				using (var calendar = new GregorianCalendar())
				{
					return TimeUnit.Hours.Convert(calendar.TimeZone.RawOffset, TimeUnit.Milliseconds) / 3600;
				}
			}
		}

		public string TimeZone
		{
			get { return Java.Util.TimeZone.Default.ID; }
		}

		#endregion

		/// <summary>
		/// Gets the total memory in bytes.
		/// </summary>
		private static readonly long DeviceTotalMemory = GetTotalMemory();
		public long TotalMemory { 
			get {
				return DeviceTotalMemory;
			}
		}

		private static long GetTotalMemory()
		{
			using (var reader = new RandomAccessFile("/proc/meminfo", "r")) 
			{
				var line = reader.ReadLine(); // first line --> MemTotal: xxxxxx kB
				var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				return Convert.ToInt64(split[1]) * 1024;
			}
		}
	}
}

