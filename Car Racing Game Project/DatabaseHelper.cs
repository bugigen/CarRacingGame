using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace Car_Racing_Game_MOO_ICT
{
    public class DatabaseHelper
    {
        private readonly string dbPath;
        private readonly string connectionString;

        public DatabaseHelper()
        {
            dbPath = Path.Combine(Application.StartupPath, "scores.db");
            connectionString = $"Data Source={dbPath};Version=3;";
        }

        public void InitializeDatabase()
        {
            // создаём файл, если нет
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            using (var conn = new SQLiteConnection(connectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Scores(
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Score INTEGER NOT NULL,
                        Date TEXT NOT NULL
                    );";
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertScore(string name, int score)
        {
            if (string.IsNullOrWhiteSpace(name)) name = "Anonymous";
            if (name.Length > 50) name = name.Substring(0, 50);

            string dateString = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

            using (var conn = new SQLiteConnection(connectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "INSERT INTO Scores(Name, Score, Date) VALUES(@name, @score, @date);";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@score", score);
                cmd.Parameters.AddWithValue("@date", dateString); // ISO time
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetTopScores(int limit = 10)
        {
            var dt = new DataTable();
            using (var conn = new SQLiteConnection(connectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "SELECT Name, Score, Date FROM Scores ORDER BY Score DESC, Date ASC LIMIT @limit;";
                cmd.Parameters.AddWithValue("@limit", limit);
                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public void ClearScores()
        {
            using (var conn = new SQLiteConnection(connectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = "DELETE FROM Scores;";

                cmd.ExecuteNonQuery();
            }
        }
    }
}
