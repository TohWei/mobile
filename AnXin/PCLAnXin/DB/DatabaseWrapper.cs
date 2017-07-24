using System;
using SQLite.Net;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using SQLite;

namespace PCLAnXin
{
	//That's how its supposed to work:
	//CREATE TABLE IF NOT EXIST will create the table if it doesn't exist, 
	//or ignore the command if it does. 
	//If you want it to delete the old table, use DELETE TABLE IF EXISTS before CREATE TABLE. 
	//If you want to change the schema, use ALTER TABLE, not CREATE TABLE.
	public class DatabaseWrapper
	{
		private SQLiteConnection conn;

		public DatabaseWrapper (string fileName)
		{
			if (conn == null) {
				conn = DependencyService.Get<ISQLite> ().GetConnection (fileName);
				CleanDB ();	// not sure if this best place for this
			}
		}		

		public void CleanDB() {

			string versionTableName = typeof(TblVersion).Name;
			string appVersion = InfoServiceHelper.AppVersion;

			bool tableExists = false;
			try {
				string cmdText = string.Format("Select count(name) from sqlite_master where type ='table' and name = '{0}'", versionTableName);
				var cmd = conn.CreateCommand (cmdText, 1);
				tableExists = cmd.ExecuteScalar<int> () != 0;

			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine (e.Message);
			}

			if(tableExists) {
				var cmd = conn.CreateCommand (string.Format("SELECT * FROM {0}", versionTableName));
				var row = cmd.ExecuteDeferredQuery<TblVersion> ().FirstOrDefault();
				string dbVersion = row == null ? null : row.Version;

				if (dbVersion != appVersion) {
					return;  // Database up to date
				}

				// Create static tables like user data
				CreateStaticTables();
				// On update always drop all cached data tables
				DropCachedTables();
				//StorageHelper.DeleteAllAvatars ();	// TODO: consider whether this should be moved outside of the database setup
			} else {
				// Create static tables like user data
				CreateStaticTables();

				// Init static tables
				//conn.Insert(new TblAppSettings () { Endpoint = ApplicationManager.DefaultEndpoint });
				conn.Insert(new TblAppSettings () { 
					//Endpoint = ApplicationManager.DefaultEndpoint,
					//FinishedTour = false, //New installation show tour.
				});
			} 
			// create tables
			CreateCachedTables();

			// Update the databases version no to latest.
			conn.DeleteAll<TblVersion>();
			conn.Insert(new TblVersion() { Version = appVersion });
		}

		public void CreateStaticTables() {
			conn.CreateTable<TblUser>();
			conn.CreateTable<TblAppSettings> ();
			conn.CreateTable<TblVersion> ();
			conn.CreateTable<TblPassAccount> ();
		}

		// TODO: Convert to type list
		public void DropCachedTables() {
			conn.DropTable<TblCompanyProfile> ();
		}

		public void CreateCachedTables() {
			conn.CreateTable<TblCompanyProfile> ();
		}

		/// <summary>
		/// Creates Table if doesn't exists, else it will ignore it. 
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void CreateTable<T>()
		{
			conn.CreateTable<T>();
		}

		/// <summary>
		/// Get Result by passing in an instance of table object. 
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public IEnumerable<T> Get<T>() where T: class, new()
		{	try{
				if (IsTableExists<T>()) {
					return (from b in conn.Table<T> ()select b).ToList ();
				}else{
					return null;
				}
			}catch(SQLiteException exp){
				return null;
			}
		}

		/// <summary>
		/// Get the result with a specified query and args.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="args">Arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public IEnumerable<T> Get<T>(string query, params object[] args)
		{
			try{
				SQLiteCommand cmd = conn.CreateCommand (query, args);
				return cmd.ExecuteDeferredQuery<T> ();
			}catch(Exception exp){
				return null;
			}
		}

		/// <summary>
		/// A method that allow you to perform complex query, such as inner join. 
		/// </summary>
		/// <returns>The complex.</returns>
		/// <param name="query">Query.</param>
		/// <param name="tableList">Place all the tables used in your query for validation</param>
		/// <param name="args">Arguments.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public IEnumerable<T> GetComplex<T>(string query, List<string> tableList, params object[] args)
		{
			if (tableList == null) {
				throw new Exception ("table list cannot be null");
			}

			if (IsTableExists(tableList)) {
				try {
					SQLiteCommand cmd = conn.CreateCommand (query, args);
					return cmd.ExecuteDeferredQuery<T> ();
				} catch (Exception exp) {
					return null;
				}
			} else {
				return null;
			}
		}

		/// <summary>
		/// Get result with a specified id.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T Get<T> (int id)
		{
			//Same as
			//var result = conn.Table<T> ().FirstOrDefault ();
			//return conn.Table<T>().FirstOrDefault (t => t.ID == id);

			Type t = typeof(T);
			string query = string.Format("select * from \"{0}\" where ID = ?", t.Name);
			return Get<T> (query, id).FirstOrDefault();
		}


		/// <summary>
		/// Delete a row of record
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void DeleteData<T>(int id){
			if (IsTableExists<T> ()) {
				conn.Delete<T> (id);
			}
		}

		/// <summary>
		/// Deletes all data.
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void DeleteAllData<T>(){
			if (IsTableExists<T> ()) {
				conn.DeleteAll<T> ();
			}
		}

		#region Update / Insert *must update last modified*
		/// <summary>
		/// Inserts the new row. Return the scope ID.
		/// </summary>
		/// <returns>The new row.</returns>
		/// <param name="tbl">Tbl.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public int InsertNewRow<T>(T tbl) where T : TblBase {
			conn.CreateTable<T>();
			tbl.LastModified = DateTime.UtcNow;
			var affectedRow = conn.Insert (tbl);
			if (affectedRow >= 0) {
				SQLiteCommand cmd = conn.CreateCommand ("SELECT last_insert_rowid()", 1);
				return cmd.ExecuteScalar<int> ();
			} else {
				return -1;
			}
		}

		public int UpdateRow<T>(T tbl) where T : TblBase {
			tbl.LastModified = DateTime.UtcNow;
			return conn.Update (tbl);
		}


		/// <summary>
		/// Execute the specified query and args.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="args">Arguments.</param>
		public int ExecuteNonQuery(string query, object[] args){
			SQLiteCommand cmd = conn.CreateCommand (query, args);
			int r = cmd.ExecuteNonQuery ();
			return r;
		}


		/// <summary>
		/// Update query with a specific id.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="id">Identifier.</param>
		public int Update(string query, object[] args){	
			SQLiteCommand cmd = conn.CreateCommand (query, args);
			int r = cmd.ExecuteNonQuery ();
			return r;
		}

		public void DropTable<T>(){
			conn.DropTable<T> ();
		}
	 	#endregion

		/// <summary>
		/// Determines whether this instance is table exists in database.
		/// </summary>
		/// <returns><c>true</c> if this instance is table exists the specified tableList; otherwise, <c>false</c>.</returns>
		/// <param name="tableList">Table list.</param>
		public bool IsTableExists(List<string> tableList)
		{
			var formatTableList = from x in tableList
			                      select string.Format ("'{0}'", x);
			var p = string.Join (",", formatTableList);

			string cmdText = string.Format("Select count(name) from sqlite_master where type ='table' and name in ({0})", p);
			var cmd = conn.CreateCommand (cmdText, 1);
			bool status = cmd.ExecuteScalar<int> () ==  tableList.Count;
			return status;
		}
			

		public bool IsTableExists<T>()
		{
			//GetTableInfo (typeof(T).Name);

			const string cmdText = "Select name from sqlite_master where type ='table' and name=?";
			var cmd = conn.CreateCommand (cmdText, typeof(T).Name);
			return cmd.ExecuteScalar<string> () != null;
		}

		public void GetTableInfo(string tableName){
			Debug.WriteLine("tableName: " + tableName);
			var query = "pragma table_info(\"" + tableName + "\")";			
			var tblInfo = conn.Query<SQLite.Net.SQLiteConnection.ColumnInfo> (query);

			foreach (var info in tblInfo) {
				System.Diagnostics.Debug.WriteLine (info.Name);
			}
			//return conn.Query<SQLite.Net.SQLiteConnection.ColumnInfo> (query);
		}

		/// <summary>
		/// Close DB COnn.
		/// </summary>
		public void CloseConn(){
			if (conn != null) {
				try {
					conn.Close();
				}finally{
				}
			}
		}

		public string GetDBFilePath(){
			return DependencyService.Get<ISQLite> ().GetDBFilePath ();
		}
	}
}

