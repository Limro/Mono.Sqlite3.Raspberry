using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using Mono.Data.Sqlite;
using Nancy;


[assembly: OwinStartup(typeof(RaspberryPi.Startup))]
namespace RaspberryPi
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseFileServer(enableDirectoryBrowsing: true);

			app.UseNancy();
		}
	}

	public class HomeModule : NancyModule
	{
		public HomeModule() : base("/api")
		{
			Get["/",true] = GetPage;
		}
		public async Task<object> GetPage(dynamic a, System.Threading.CancellationToken b)
		{
			AccessDatabase();
			return null;
		}

		public void AccessDatabase()
		{
			string db = "MyDatabase.sqlite";

			//Varify file exists
			var dir = Directory.GetCurrentDirectory();
			var path = dir + "\\" + db;
			bool exist = true;
			if (!File.Exists(path))
			{
				SqliteConnection.CreateFile(db);
				exist = false;
			}

			var dbcon = new SqliteConnection("Data Source=" + db + ";Version=3");
			dbcon.Open();

			if (!exist)
				InsertTable(dbcon);

			//Insert data
			//InsertData(dbcon);

			//Retrieve data
			//GetData(dbcon);

			dbcon.Clone();
		}


		void InsertTable(SqliteConnection dbcon)
		{
			var sqlString = "CREATE TABLE highscores (name VARCHAR(20), score INT)";
			var comd = new SqliteCommand(sqlString, dbcon);
			var modified = comd.ExecuteNonQuery(); //Rows modified
		}

		void InsertData(SqliteConnection dbcon)
		{
			var sql = "INSERT INTO highscores (name, score) VALUES ('Me',3000)";
			var cmd = new SqliteCommand(sql, dbcon);
			cmd.ExecuteNonQuery();

			sql = "INSERT INTO highscores (name, score) VALUES ('Myself',6000)";
			cmd = new SqliteCommand(sql, dbcon);
			cmd.ExecuteNonQuery();

			sql = "INSERT INTO highscores (name, score) VALUES ('And I',9001)";
			cmd = new SqliteCommand(sql, dbcon);
			cmd.ExecuteNonQuery();
		}

		void GetData(SqliteConnection dbcon)
		{
			var retrieve = "SELECT * FROM highscores ORDER BY score DESC";
			var cmdRet = new SqliteCommand(retrieve, dbcon);
			var reader = cmdRet.ExecuteReader(); //Get data

			while (reader.Read())
				Console.WriteLine("Name: " + reader["name"] + "\tScore: " + reader["score"]);
		}
	}
}

