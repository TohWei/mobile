using System;

namespace PCLAnXin
{
	/// <summary>
	/// Tbl company profile.
	// Stores a company profile object serialised as a Json string (reduces difficulty
	// in combining the many complex sub objects).
	/// </summary>
	public class TblCompanyProfile : TblBase
	{
		public string ExpId { get; set; }
		public string LuxId { get; set; }
		public string CompanyDTOJson { get; set; }
		public string Username { get; set; }
	}
}

