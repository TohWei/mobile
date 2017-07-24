using System;
using System.IO;
using Xamarin.Forms;
using SQLite.Net.Platform.XamarinAndroid;
using SQLite.Net;
using System.Diagnostics;
using SQLite.Net.Interop;
using PCLAnXin;
using AnXin.Droid;

[assembly: Dependency(typeof(DSSQLiteDroid))]
namespace AnXin.Droid
{
	public class DSSQLiteDroid : ISQLite
	{
		const string prefix = "Droid";
		private string fileName;

		public DSSQLiteDroid ()
		{
		}

		public SQLite.Net.SQLiteConnection GetConnection (string fname)
		{
			fileName = string.Format("{0}{1}",prefix, fname);
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var path = Path.Combine (documentsPath, fileName);

			///data/data/SQLiteDemo.Droid/files/ProDroid.db3
			Debug.WriteLine ("[DSSQLiteDroid.GetConnection] Got DB path: " + path);
			var platform = new SQLitePlatformAndroid ();
			//var connection = new SQLiteConnection (platform, path, true);	// store datetime as ticks to allow simple comparison

			// https://forums.xamarin.com/discussion/549/sqlite-net-and-multiple-threads
			// https://www.sqlite.org/c3ref/open.html
			var connection = new SQLiteConnection (platform, path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex , true); 

			return connection;
		}

		public string GetDBFilePath(){
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var path = Path.Combine (documentsPath, fileName);
			return path;
		}
	}
}

