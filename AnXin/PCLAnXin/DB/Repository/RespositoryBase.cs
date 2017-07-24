using System;

namespace PCLAnXin
{
	public class RepositoryBase
	{
		//DatabaseWrapper sqlWrapper = null;
		public DatabaseWrapper DBWrapper {
			get;
			set;
		}

		protected static RepositoryBase Local;

		static RepositoryBase ()
		{
			Local = new RepositoryBase();
		}

		protected RepositoryBase(){
			//TODO Pull the app version. Name the data file same as the app version. 
			string dbFileName = "ver1.db3";
			DBWrapper = new DatabaseWrapper (dbFileName);
		}

		public static string GetDBPath(){
			if (Local != null) {
				return Local.DBWrapper.GetDBFilePath ();
			} else {
				return null;
			}
		}
	}
}

