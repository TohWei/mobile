using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace PCLAnXin
{
	public class PassAccountRepository : RepositoryBase
	{
		//Insert data.
		public static int StorePassAccount(int userId, string accountName, string loginName, string anXin, string comments)
		{
			int scopeId = -1;

			//If same amount and subject, then delete it. 
			Local.DBWrapper.ExecuteNonQuery ("DELETE FROM TblPassAccount WHERE accountName = ? AND UserId = ?", new object[] {
				accountName,
				userId
			});
			var row = new TblPassAccount () 
			{ 
				AccountName = accountName,
				LoginName = loginName,
				AnXin = anXin,
				Comments = comments,
				UserId = userId
			};
			scopeId = Local.DBWrapper.InsertNewRow<TblPassAccount> (row);
			return scopeId;
		}

		//Retrieve single record.
		public static TblPassAccount GetPassAccountById(string Id, string username) 
		{
			var dataRows = Local.DBWrapper.GetComplex<TblPassAccount> ("SELECT * FROM TblPassAccount WHERE Id = ? AND Username = ?", 
				new List<string> {"TblPassAccount"}, new object[] 
				{ 
					Id, username 
				});
			
			if (dataRows != null && dataRows.Any()) 
			{
				var row = dataRows.First ();
				return row;
			}
			return null;
		}

		//Retrieve a set of records.
		public static List<TblPassAccount> GetPassAccountList(int userId) {
			
			IEnumerable<TblPassAccount> rows = Local.DBWrapper.Get<TblPassAccount> ("SELECT * FROM TblPassAccount WHERE UserId = ?", new object[] { userId});
			try {
				if (rows != null && rows.Any()) 
				{
					return rows.ToList();
				}
			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine ("[PassAccountRepository.GetPassAccountList] Error: " + e.Message);
			}
			return null;
		}

		//Delete data.
		public static int DelPassAccount(int userId, int id){
			int affected = -1;

			//If same ID and username, then delete it. 
			affected = Local.DBWrapper.ExecuteNonQuery ("DELETE FROM TblPassAccount WHERE ID = ? AND UserId = ?", new object[] {
				id,
				userId
			});

			return affected;
		}

		public static int UpdatePassAccount(int Id, int userId, string accountName, string loginName, string anXin, string comments)
		{
			int affected = -1;

			var expensesRow = new TblPassAccount () 
			{ 
				ID = Id,
				AccountName = accountName,
				LoginName = loginName,
				AnXin = anXin,
				Comments = comments,
				UserId = userId
			};
			affected = Local.DBWrapper.UpdateRow<TblPassAccount> (expensesRow);

			return affected;
		}
	}
}

