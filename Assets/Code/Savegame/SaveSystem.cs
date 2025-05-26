using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections.Generic;

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
        using var connection = new SqliteConnection(dbPath);
        connection.Open();
        using var command = connection.CreateCommand();

        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Inventory (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                itemId TEXT,
                quantity INTEGER,
                saveSlot INTEGER);

            CREATE TABLE IF NOT EXISTS PlayerStats (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                coins INTEGER,
                xp INTEGER,
                currentScene TEXT,
                posX REAL,
                posY REAL,
                posZ REAL,
                rotY REAL,
                saveSlot INTEGER UNIQUE,
                lastPlayed TEXT);

            CREATE TABLE IF NOT EXISTS Quest (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                questId TEXT,
                questState TEXT,
                saveSlot INTEGER);

            CREATE TABLE IF NOT EXISTS GameTime (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                timeOfDay REAL,
                saveSlot INTEGER);";

        command.ExecuteNonQuery();
    }

    // --------------- Inventory ---------------
    public static void SaveInventory(List<SimpleItemSlot> items, int saveSlot)
    {
        using var connection = new SqliteConnection(dbPath);
        connection.Open();

        // Clear previous save slot data
        using (var deleteCmd = connection.CreateCommand())
        {
            deleteCmd.CommandText = "DELETE FROM Inventory WHERE saveSlot = @slot;";
            deleteCmd.Parameters.AddWithValue("@slot", saveSlot);
            deleteCmd.ExecuteNonQuery();
        }

        // Insert each item
        foreach (var item in items)
        {
            using var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = "INSERT INTO Inventory (itemId, quantity, saveSlot) VALUES (@id, @q, @slot);";
            insertCmd.Parameters.AddWithValue("@id", item.itemId);
            insertCmd.Parameters.AddWithValue("@q", item.quantity);
            insertCmd.Parameters.AddWithValue("@slot", saveSlot);
            insertCmd.ExecuteNonQuery();
        }
    }

    public static List<SimpleItemSlot> LoadInventory(int saveSlot)
    {
        var items = new List<SimpleItemSlot>();

        using var connection = new SqliteConnection(dbPath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT itemId, quantity FROM Inventory WHERE saveSlot = @slot;";
        command.Parameters.AddWithValue("@slot", saveSlot);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new SimpleItemSlot
            {
                itemId = reader.GetString(0),
                quantity = reader.GetInt32(1)
            });
        }

        return items;
    }

    // --------------- Player Stats ---------------
    public static void SavePlayerStats(int coins, int xp, string sceneName, Vector3 position, float rotationY, int saveSlot)
{
    using var connection = new SqliteConnection(dbPath);
    connection.Open();

    // Check if saveSlot exists
    using (var checkCmd = connection.CreateCommand())
    {
        checkCmd.CommandText = "SELECT COUNT(*) FROM PlayerStats WHERE saveSlot = @slot;";
        checkCmd.Parameters.AddWithValue("@slot", saveSlot);
        long count = (long)checkCmd.ExecuteScalar();

        using var command = connection.CreateCommand();

        if (count > 0)
        {
            // UPDATE existing record
            command.CommandText = @"
                UPDATE PlayerStats SET 
                    coins = @coins,
                    xp = @xp,
                    currentScene = @scene,
                    posX = @x,
                    posY = @y,
                    posZ = @z,
                    rotY = @rotY,
                    lastPlayed = @lastPlayed
                WHERE saveSlot = @slot;";
        }
        else
        {
            // INSERT new record
            command.CommandText = @"
                INSERT INTO PlayerStats (coins, xp, currentScene, posX, posY, posZ, rotY, saveSlot, lastPlayed)
                VALUES (@coins, @xp, @scene, @x, @y, @z, @rotY, @slot, @lastPlayed);";
        }

        command.Parameters.AddWithValue("@coins", coins);
        command.Parameters.AddWithValue("@xp", xp);
        command.Parameters.AddWithValue("@scene", sceneName);
        command.Parameters.AddWithValue("@x", position.x);
        command.Parameters.AddWithValue("@y", position.y);
        command.Parameters.AddWithValue("@z", position.z);
        command.Parameters.AddWithValue("@rotY", rotationY);
        command.Parameters.AddWithValue("@slot", saveSlot);
        command.Parameters.AddWithValue("@lastPlayed", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        command.ExecuteNonQuery();
    }
}


    public static PlayerSaveData LoadPlayerStats(int saveSlot)
    {
        using var connection = new SqliteConnection(dbPath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT coins, xp, currentScene, posX, posY, posZ, rotY, lastPlayed FROM PlayerStats WHERE saveSlot = @slot;";
        command.Parameters.AddWithValue("@slot", saveSlot);

        using var reader = command.ExecuteReader();
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

    // --------------- Quest ---------------
    public static void SaveQuests(string questId, string questState, int saveSlot)
    {
        using var connection = new SqliteConnection(dbPath);
        connection.Open();

        using var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM Quest WHERE saveSlot = @slot;";
        deleteCmd.Parameters.AddWithValue("@slot", saveSlot);
        deleteCmd.ExecuteNonQuery();

        using var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = "INSERT INTO Quest (questId, questState, saveSlot) VALUES (@id, @state, @slot);";
        insertCmd.Parameters.AddWithValue("@id", questId);
        insertCmd.Parameters.AddWithValue("@state", questState);
        insertCmd.Parameters.AddWithValue("@slot", saveSlot);
        insertCmd.ExecuteNonQuery();
    }

    public static (string questId, string questState) LoadQuests(int saveSlot)
    {
        using var connection = new SqliteConnection(dbPath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT questId, questState FROM Quest WHERE saveSlot = @slot;";
        command.Parameters.AddWithValue("@slot", saveSlot);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return (reader.GetString(0), reader.GetString(1));
        }

        return ("", "NotStarted");
    }

    // --------------- Game Time ---------------
    public static void SaveGameTime(float timeOfDay, int saveSlot)
    {
        using var connection = new SqliteConnection(dbPath);
        connection.Open();

        using var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM GameTime WHERE saveSlot = @slot;";
        deleteCmd.Parameters.AddWithValue("@slot", saveSlot);
        deleteCmd.ExecuteNonQuery();

        using var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = "INSERT INTO GameTime (timeOfDay, saveSlot) VALUES (@t, @slot);";
        insertCmd.Parameters.AddWithValue("@t", timeOfDay);
        insertCmd.Parameters.AddWithValue("@slot", saveSlot);
        insertCmd.ExecuteNonQuery();
    }

    public static float LoadGameTime(int saveSlot)
    {
        using var connection = new SqliteConnection(dbPath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT timeOfDay FROM GameTime WHERE saveSlot = @slot;";
        command.Parameters.AddWithValue("@slot", saveSlot);

        using var reader = command.ExecuteReader();
        if (reader.Read())
            return reader.GetFloat(0);

        return 6f; // default time (6 AM)
    }

    // --------------- Utility ---------------
    public static void ClearInventory(int saveSlot)
    {
        using var connection = new SqliteConnection(dbPath);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Inventory WHERE saveSlot = @slot;";
        cmd.Parameters.AddWithValue("@slot", saveSlot);
        cmd.ExecuteNonQuery();
    }

    public static bool TryLoadPlayerStats(int slot, out PlayerSaveData data)
    {
        data = LoadPlayerStats(slot);

        bool isEmpty = data.scene == "WorldMain" && data.position == Vector3.zero && data.coins == 0 && data.xp == 0;
        return !isEmpty;
    }
}
