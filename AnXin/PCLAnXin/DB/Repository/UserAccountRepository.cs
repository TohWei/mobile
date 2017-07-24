using System;
using System.Collections.Generic;
using System.Linq;

namespace PCLAnXin
{
	public class UserAccountRepository : RepositoryBase
	{
		//Insert data.
		public static int StoreMasaterPass(string masterPass)
		{
			int scopeId = -1;

			//If same amount and subject, then delete it. 
			Local.DBWrapper.ExecuteNonQuery ("DELETE FROM TblUser WHERE MasterPass = ?", new object[] {
				masterPass
			});
			var row = new TblUser () 
			{ 
				MasterPass = masterPass,
			};
			scopeId = Local.DBWrapper.InsertNewRow<TblUser> (row);
			return scopeId;
		}

		//Retrieve single record.
		public static TblUser GetMasaterPass(string masterPass) 
		{
			var dataRows = Local.DBWrapper.GetComplex<TblUser> ("SELECT * FROM TblUser WHERE MasterPass = ?", 
				new List<string> {"TblUser"}, new object[] 
				{ 
					masterPass
				});

			if (dataRows != null && dataRows.Any()) 
			{
				var row = dataRows.First ();
				return row;
			}
			return null;
		}
	}
}

