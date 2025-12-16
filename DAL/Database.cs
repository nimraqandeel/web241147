using Microsoft.Data.Sqlite;

namespace Webapi.DAL
{
    public class Database
    {
        public static string ConnectionString = "Data Source=Data/todo.db;";
        public static SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            return conn;
        }    }
}
