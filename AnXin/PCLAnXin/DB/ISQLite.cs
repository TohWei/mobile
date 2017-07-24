using System;
using SQLite.Net;

namespace PCLAnXin
{
	public interface ISQLite
	{
		SQLiteConnection GetConnection(string fileName);

		string GetDBFilePath();
	}
}

