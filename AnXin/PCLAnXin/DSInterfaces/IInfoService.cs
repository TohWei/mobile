using System;

namespace PCLAnXin
{
	public interface IInfoService
	{
		string DeviceName{get;}
		string DeviceId { get;}
		string PackageName { get;}
		string AppVersionName { get;}
		string AppVersionCode{ get;}
		string FirmwareVersion { get; }
		string HardwareVersion { get; }
		string Manufacturer { get; }
		string ModelName { get;}
		// Gets the total available memory for user in bytes.
		long TotalMemory { get; }

		double TimeZoneOffset{get;}
		string TimeZone{get;}
	}
}

