using System;

namespace PCLAnXin
{
	/// <summary>
	/// For values which vary by user id
	/// This is a Static Table
	/// Not automatically cleared on db version increase,
	/// should avoid changing or have custom plan for upgrade
	/// </summary>
	public class TblUser : TblBase
	{
		public string MasterPass { get; set;}
	}
}

