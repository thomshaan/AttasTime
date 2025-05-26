using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;

public class DatabaseTest : MonoBehaviour
{
    private string dbPath;

    void Start()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/TestDB.db";

        TestDatabaseConnection();
    }

    void TestDatabaseConnection()
    {
        try
        {
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "CREATE TABLE IF NOT EXISTS Test (id INTEGER PRIMARY KEY, message TEXT);";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Test (message) VALUES ('Hello from SQLite!');";
                command.ExecuteNonQuery();

                command.CommandText = "SELECT * FROM Test;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string message = reader.GetString(1);
                        Debug.Log($"[SQLite Test] Row: ID = {id}, Message = {message}");
                    }
                }

                connection.Close();
            }

            Debug.Log("[SQLite Test] SUCCESS: Database is working!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[SQLite Test] ERROR: " + ex.Message);
        }
    }
}
