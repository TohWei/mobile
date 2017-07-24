using System;
using SQLite.Net.Attributes;

namespace PCLAnXin
{
	public class TblPassAccount : TblBase
	{
		public string AccountName {
			get;
			set;
		}

		public string LoginName {
			get;
			set;
		}

		public string AnXin {
			get;
			set;
		}

		public string Comments {
			get;
			set;
		}

		public int UserId { get; set; }


		public string EncryptedPass {get;set;}
	}
}

