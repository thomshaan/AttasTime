using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    private string dbPath;

    void Awake()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/GameSave.db";
        CreateTables();
    }

    void CreateTables()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Inventory (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                itemId TEXT, quantity INTEGER, saveSlot INTEGER);
            CREATE TABLE IF NOT EXISTS PlayerStats (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                coins INTEGER, xp INTEGER, currentScene TEXT,
                posX REAL, posY REAL, posZ REAL, rotY REAL, saveSlot INTEGER);
            CREATE TABLE IF NOT EXISTS Quest (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                questId TEXT, questState TEXT, saveSlot INTEGER);
            CREATE TABLE IF NOT EXISTS GameTime (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                timeOfDay REAL, saveSlot INTEGER);";

            command.ExecuteNonQuery();
        }
    }

    // ------------------- Inventory -------------------
    public void SaveInventory(List<SimpleItemSlot> items, int saveSlot)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = "DELETE FROM Inventory WHERE saveSlot = @slot;";
            command.Parameters.AddWithValue("@slot", saveSlot);
            command.ExecuteNonQuery();

            foreach (var item in items)
            {
                command.CommandText = "INSERT INTO Inventory (itemId, quantity, saveSlot) VALUES (@id, @q, @slot);";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@id", item.itemId);
                command.Parameters.AddWithValue("@q", item.quantity);
                command.Parameters.AddWithValue("@slot", saveSlot);
                command.ExecuteNonQuery();
            }
        }
    }

    public List<SimpleItemSlot> LoadInventory(int saveSlot)
    {
        List<SimpleItemSlot> items = new List<SimpleItemSlot>();
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT itemId, quantity FROM Inventory WHERE saveSlot = @slot;";
            command.Parameters.AddWithValue("@slot", saveSlot);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(new SimpleItemSlot
                    {
                        itemId = reader.GetString(0),
                        quantity = reader.GetInt32(1)
                    });
                }
            }
        }
        return items;
    }

    // ------------------- Player Stats -------------------
    public void SavePlayerStats(int coins, int xp, string sceneName, Vector3 pos, float rotY, int saveSlot)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var cmd = connection.CreateCommand();

            cmd.CommandText = "DELETE FROM PlayerStats WHERE saveSlot = @slot;";
            cmd.Parameters.AddWithValue("@slot", saveSlot);
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
                INSERT INTO PlayerStats (coins, xp, currentScene, posX, posY, posZ, rotY, saveSlot)
                VALUES (@coins, @xp, @scene, @x, @y, @z, @rotY, @slot);";
            cmd.Parameters.AddWithValue("@coins", coins);
            cmd.Parameters.AddWithValue("@xp", xp);
            cmd.Parameters.AddWithValue("@scene", sceneName);
            cmd.Parameters.AddWithValue("@x", pos.x);
            cmd.Parameters.AddWithValue("@y", pos.y);
            cmd.Parameters.AddWithValue("@z", pos.z);
            cmd.Parameters.AddWithValue("@rotY", rotY);
            cmd.ExecuteNonQuery();
        }
    }

    public (int coins, int xp, string scene, Vector3 position, float rotY) LoadPlayerStats(int saveSlot)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var cmd = connection.CreateCommand();

            cmd.CommandText = "SELECT coins, xp, currentScene, posX, posY, posZ, rotY FROM PlayerStats WHERE saveSlot = @slot;";
            cmd.Parameters.AddWithValue("@slot", saveSlot);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return (
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetString(2),
                        new Vector3(reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5)),
                        reader.GetFloat(6)
                    );
                }
            }
        }
        return (0, 0, "WorldMain", Vector3.zero, 0f);
    }

    // ------------------- Quest -------------------
    public void SaveQuests(string questId, string questState, int saveSlot)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var cmd = connection.CreateCommand();

            cmd.CommandText = "DELETE FROM Quest WHERE saveSlot = @slot;";
            cmd.Parameters.AddWithValue("@slot", saveSlot);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO Quest (questId, questState, saveSlot) VALUES (@id, @state, @slot);";
            cmd.Parameters.AddWithValue("@id", questId);
            cmd.Parameters.AddWithValue("@state", questState);
            cmd.ExecuteNonQuery();
        }
    }

    public (string questId, string questState) LoadQuests(int saveSlot)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT questId, questState FROM Quest WHERE saveSlot = @slot;";
            cmd.Parameters.AddWithValue("@slot", saveSlot);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return (reader.GetString(0), reader.GetString(1));
                }
            }
        }

        return ("", "NotStarted");
    }

    // ------------------- Game Time -------------------
    public void SaveGameTime(float timeOfDay, int slot)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var cmd = connection.CreateCommand();

            cmd.CommandText = "DELETE FROM GameTime WHERE saveSlot = @slot;";
            cmd.Parameters.AddWithValue("@slot", slot);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO GameTime (timeOfDay, saveSlot) VALUES (@t, @slot);";
            cmd.Parameters.AddWithValue("@t", timeOfDay);
            cmd.ExecuteNonQuery();
        }
    }

    public float LoadGameTime(int slot)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT timeOfDay FROM GameTime WHERE saveSlot = @slot;";
            cmd.Parameters.AddWithValue("@slot", slot);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                    return reader.GetFloat(0);
            }
        }

        return 6f; // default morning
    }
}
