using System;

namespace PCLAnXin
{
	/// <summary>
	// Database storage corresponding to the ApplicationManager
	/// This is a Static Table
	/// Not automatically cleared on db version increase,
	/// should avoid changing or have custom plan for upgrade
	/// </summary>
	public class TblAppSettings : TblBase
	{
		public string Endpoint {
			get;
			set;
		}

		public string CurrentUser {
			get;
			set;
		}

		public string LastUser {
			get;
			set;
		}

		public bool TestMode { 
			get; 
			set;
		}

		public bool FinishedTour {
			get;
			set;
		}

		public bool ForceToShowTour {
			get;
			set;
		}
	}
}
