using System;
using System.IO;
using SQLite.Net;
using SQLite.Net.Platform.XamarinIOS;
using Xamarin.Forms;
using SQLite.Net.Interop;
using PCLAnXin;
using AnXin.iOS;

[assembly: Dependency(typeof(DSSQLiteIOS))]
namespace AnXin.iOS
{
	public class DSSQLiteIOS : ISQLite
	{
		const string prefix = "iOS";
		private string fileName;

		public DSSQLiteIOS ()
		{
		}
			
		public SQLiteConnection GetConnection (string fname)
		{
			fileName = string.Format("{0}{1}",prefix, fname);
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var libraryPath = Path.Combine (documentsPath, "..", "Library");
			var path = Path.Combine (libraryPath, fileName);

			var platform = new SQLitePlatformIOS ();
			//var connection = new SQLiteConnection (platform, path, true); // store datetime as ticks to allow simple comparison

			// https://forums.xamarin.com/discussion/549/sqlite-net-and-multiple-threads
			// https://www.sqlite.org/c3ref/open.html
			var connection = new SQLiteConnection (platform, path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex , true); // store datetime as ticks to allow simple comparison

			return connection;
		}


		//TODO: need to pass in version number. 
		public string GetDBFilePath(){

			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var libraryPath = Path.Combine (documentsPath, "..", "Library");

			var fileName = string.Format("{0}{1}",prefix, "ver1.db3");
			var path = Path.Combine (libraryPath, fileName);
			return path;
		}
	}
}

