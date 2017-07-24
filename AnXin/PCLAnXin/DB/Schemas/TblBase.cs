using System;
using SQLite.Net.Attributes;

namespace PCLAnXin
{
	/// <summary>
	/// This is a the base class for all other tables, and is not directly
	/// instanced into the db
	/// This is a static table as other static tables inherit from it
	/// This means in production we should avoid changing this table
	/// as a db version increase will not automatically clear this
	/// table. 
	/// </summary>
	public class TblBase
	{
		[PrimaryKey, AutoIncrement]
		public int ID {
			get;
			set;
		}
			
		public DateTime LastModified {
			get;
			set;
		}
	}
}

