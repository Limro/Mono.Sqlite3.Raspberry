using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using Mono.Data.Sqlite;

[assembly: OwinStartup(typeof(RaspberryPi.Startup))]
namespace RaspberryPi
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseFileServer(enableDirectoryBrowsing: true);
			//app.UseWelcomePage(new Microsoft.Owin.Diagnostics.WelcomePageOptions()
			//{
			//	Path = new PathString("/welcome")
			//});

			app.Run(context =>
			{
				context.Response.ContentType = "text/html";

				//string output = string.Format(
				//		"I'm running on {0} nFrom assembly {1}",
				//		Environment.OSVersion,
				//		System.Reflection.Assembly.GetEntryAssembly().FullName
				//		);

				return context.Response.WriteAsync(LoadPage());
			});

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

		string LoadPage()
		{
			//var css = File.ReadAllText("css\\style.css");
			var index = File.ReadAllText("index.html");
			//var split = "<!--Split here-->";

			//if(index.Contains(index))
			//{
			//	var start = index.IndexOf(split, 0) + split.Length;
			//	var part1 = index.Substring(0, start);
			//	var part2 = index.Substring(start, index.Length-start);

			//	index = part1 + "<style> " + css + "</style>" + part2;
			//}
			
			return index;
		}
	}
}

