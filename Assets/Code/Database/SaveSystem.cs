using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;

// Data structure for saving player metadata
public struct PlayerSaveData
{
    public int coins;
    public int xp;
    public string scene;
    public Vector3 position;
    public float rotY;
    public string lastPlayed;
}

public class SaveSystem : MonoBehaviour
{
    private static string dbPath => "URI=file:" + Application.persistentDataPath + "/GameSave.db";

    void Awake()
    {
        CreateTables();
    }

    private void CreateTables()
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
                posX REAL, posY REAL, posZ REAL, rotY REAL,
                saveSlot INTEGER, lastPlayed TEXT);
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
    public static void SaveInventory(List<SimpleItemSlot> items, int saveSlot)
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

    public static List<SimpleItemSlot> LoadInventory(int saveSlot)
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
    public static void SavePlayerStats(int coins, int xp, string sceneName, Vector3 position, float rotationY, int saveSlot)
    {
        using var conn = new SqliteConnection(dbPath);
        conn.Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
        INSERT OR REPLACE INTO PlayerStats 
        (coins, xp, currentScene, posX, posY, posZ, rotY, saveSlot, lastPlayed)
        VALUES 
        (@coins, @xp, @scene, @x, @y, @z, @rotY, @slot, @lastPlayed);";

        cmd.Parameters.AddWithValue("@coins", coins);
        cmd.Parameters.AddWithValue("@xp", xp);
        cmd.Parameters.AddWithValue("@scene", sceneName);
        cmd.Parameters.AddWithValue("@x", position.x);
        cmd.Parameters.AddWithValue("@y", position.y);
        cmd.Parameters.AddWithValue("@z", position.z);
        cmd.Parameters.AddWithValue("@rotY", rotationY);
        cmd.Parameters.AddWithValue("@slot", saveSlot);
        cmd.Parameters.AddWithValue("@lastPlayed", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
    }

    public static PlayerSaveData LoadPlayerStats(int saveSlot)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var cmd = connection.CreateCommand();

            cmd.CommandText = "SELECT coins, xp, currentScene, posX, posY, posZ, rotY, lastPlayed FROM PlayerStats WHERE saveSlot = @slot;";
            cmd.Parameters.AddWithValue("@slot", saveSlot);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new PlayerSaveData
                    {
                        coins = reader.GetInt32(0),
                        xp = reader.GetInt32(1),
                        scene = reader.GetString(2),
                        position = new Vector3(reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5)),
                        rotY = reader.GetFloat(6),
                        lastPlayed = reader.IsDBNull(7) ? "Never" : reader.GetString(7)
                    };
                }
            }
        }

        return new PlayerSaveData
        {
            coins = 0,
            xp = 0,
            scene = "WorldMain",
            position = Vector3.zero,
            rotY = 0f,
            lastPlayed = "Never"
        };
    }

    // ------------------- Quest -------------------
    public static void SaveQuests(string questId, string questState, int saveSlot)
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

    public static (string questId, string questState) LoadQuests(int saveSlot)
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
    public static void SaveGameTime(float timeOfDay, int saveSlot)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var cmd = connection.CreateCommand();

            cmd.CommandText = "DELETE FROM GameTime WHERE saveSlot = @slot;";
            cmd.Parameters.AddWithValue("@slot", saveSlot);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO GameTime (timeOfDay, saveSlot) VALUES (@t, @slot);";
            cmd.Parameters.AddWithValue("@t", timeOfDay);
            cmd.Parameters.AddWithValue("@slot", saveSlot);
            cmd.ExecuteNonQuery();
        }
    }

    public static float LoadGameTime(int slot)
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

        return 6f; // default fallback time
    }

    public static void ClearInventory(int slot)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Inventory WHERE saveSlot = @slot;";
            cmd.Parameters.AddWithValue("@slot", slot);
            cmd.ExecuteNonQuery();
        }
    }
}
