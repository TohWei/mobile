/// <summary>
/// http://developer.xamarin.com/samples/monotouch/ReachabilitySample/
/// 27/04/15
/// </summary>

using System;
using System.Net;
using SystemConfiguration;
using CoreFoundation;

public enum NetworkStatus
{
    NotReachable,
    ReachableViaCarrierDataNetwork,
    ReachableViaWiFiNetwork
}
	
public static class Reachability
{
    public static string HostName = "www.google.com";

    public static bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
    {
        // Is it reachable with the current network configuration?
        bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

        // Do we need a connection to reach it?
        bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0;

        // Since the network stack will automatically try to get the WAN up,
        // probe that
        if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
            noConnectionRequired = true;

        return isReachable && noConnectionRequired;
    }

    // Is the host reachable with the current network configuration
    public static bool IsHostReachable(string host)
    {
        if (string.IsNullOrEmpty(host))
            return false;

        using (var r = new NetworkReachability(host))
        {
            NetworkReachabilityFlags flags;

            if (r.TryGetFlags(out flags))
            {
                return IsReachableWithoutRequiringConnection(flags);
            }
        }
        return false;
    }

    //
    // Raised every time there is an interesting reachable event,
    // we do not even pass the info as to what changed, and
    // we lump all three status we probe into one
    //
    public static event EventHandler ReachabilityChanged;

    static void OnChange(NetworkReachabilityFlags flags)
    {
        var h = ReachabilityChanged;
        if (h != null)
            h(null, EventArgs.Empty);
    }


	//ConnectionRequired: Reachable, but a connection must first establish.
	//In order to get the status of isOnline or offline, we need to filter the flag in this event. Because the parent of this event will be called in number of stage. 
	//The logic for online will be: must reachable, and does not need to establish anymore. Means,.. you are connecting.
	static void OnNetworkChange(NetworkReachabilityFlags flags)
	{
		if (flags == 0) {
			Console.WriteLine ("Off line.....");
			AnXin.App.InternetConnOfflineAction.Invoke ();
		} else if (flags.HasFlag(NetworkReachabilityFlags.Reachable) && !flags.HasFlag(NetworkReachabilityFlags.ConnectionRequired)) {
			Console.WriteLine ("On line.....");
			AnXin.App.InternetConnOnlineAction.Invoke ();
		}
		var h = ReachabilityChanged;
		if (h != null)
			h(null, EventArgs.Empty);
	}

    //
    // Returns true if it is possible to reach the AdHoc WiFi network
    // and optionally provides extra network reachability flags as the
    // out parameter
    //
    static NetworkReachability adHocWiFiNetworkReachability;

    public static bool IsAdHocWiFiNetworkAvailable(out NetworkReachabilityFlags flags)
    {
        if (adHocWiFiNetworkReachability == null)
        {
            adHocWiFiNetworkReachability = new NetworkReachability(new IPAddress(new byte [] { 169, 254, 0, 0 }));
            adHocWiFiNetworkReachability.SetNotification(OnChange);
            adHocWiFiNetworkReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
        }

        return adHocWiFiNetworkReachability.TryGetFlags(out flags) && IsReachableWithoutRequiringConnection(flags);
    }

    static NetworkReachability defaultRouteReachability;

    static bool IsNetworkAvailable(out NetworkReachabilityFlags flags)
    {
        if (defaultRouteReachability == null)
        {
            defaultRouteReachability = new NetworkReachability(new IPAddress(0));
            defaultRouteReachability.SetNotification(OnChange);

			//The normal Onchange event used in multiple place. 
			//The OnNetworkChange is the event for available. 
			defaultRouteReachability.SetNotification(OnNetworkChange); 
            
			defaultRouteReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
        }
        return defaultRouteReachability.TryGetFlags(out flags) && IsReachableWithoutRequiringConnection(flags);
    }

    static NetworkReachability remoteHostReachability;

    public static NetworkStatus RemoteHostStatus()
    {
        NetworkReachabilityFlags flags;
        bool reachable;

        if (remoteHostReachability == null)
        {
            remoteHostReachability = new NetworkReachability(HostName);

            // Need to probe before we queue, or we wont get any meaningful values
            // this only happens when you create NetworkReachability from a hostname
            reachable = remoteHostReachability.TryGetFlags(out flags);

            remoteHostReachability.SetNotification(OnChange);
            remoteHostReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
        }
        else
            reachable = remoteHostReachability.TryGetFlags(out flags);

        if (!reachable)
            return NetworkStatus.NotReachable;

        if (!IsReachableWithoutRequiringConnection(flags))
            return NetworkStatus.NotReachable;

        if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
            return NetworkStatus.ReachableViaCarrierDataNetwork;

        return NetworkStatus.ReachableViaWiFiNetwork;
    }

    public static NetworkStatus InternetConnectionStatus()
    {
        NetworkReachabilityFlags flags;
        bool defaultNetworkAvailable = IsNetworkAvailable(out flags);
        if (defaultNetworkAvailable && ((flags & NetworkReachabilityFlags.IsDirect) != 0))
        {
            return NetworkStatus.NotReachable;
        }
        else if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
            return NetworkStatus.ReachableViaCarrierDataNetwork;
        else if (flags == 0)
            return NetworkStatus.NotReachable;
        return NetworkStatus.ReachableViaWiFiNetwork;
    }

    public static NetworkStatus LocalWifiConnectionStatus()
    {
        NetworkReachabilityFlags flags;
        if (IsAdHocWiFiNetworkAvailable(out flags))
        {
            if ((flags & NetworkReachabilityFlags.IsDirect) != 0)
                return NetworkStatus.ReachableViaWiFiNetwork;
        }
        return NetworkStatus.NotReachable;
    }
}

