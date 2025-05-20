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
                slot INTEGER, itemId TEXT, quantity INTEGER, saveSlot INTEGER);
            CREATE TABLE IF NOT EXISTS Quest (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                questId TEXT, isCompleted INTEGER, isActive INTEGER, saveSlot INTEGER);
            CREATE TABLE IF NOT EXISTS PlayerStats (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                coins INTEGER, xp INTEGER, currentScene TEXT,
                posX REAL, posY REAL, posZ REAL, rotY REAL, saveSlot INTEGER);";

            command.ExecuteNonQuery();
        }
    }

    public void SaveInventory(List<ItemSlot> items, int saveSlot)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var command = connection.CreateCommand();

            // Clear old inventory
            command.CommandText = "DELETE FROM Inventory WHERE saveSlot = @slot;";
            command.Parameters.AddWithValue("@slot", saveSlot);
            command.ExecuteNonQuery();

            // Save new inventory
            foreach (var item in items)
            {
                command.CommandText = "INSERT INTO Inventory (slot, itemId, quantity, saveSlot) VALUES (@s, @id, @q, @slot);";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@s", item.slotIndex);
                command.Parameters.AddWithValue("@id", item.itemId);
                command.Parameters.AddWithValue("@q", item.quantity);
                command.Parameters.AddWithValue("@slot", saveSlot);
                command.ExecuteNonQuery();
            }
        }
    }

    public List<ItemSlot> LoadInventory(int saveSlot)
    {
        List<ItemSlot> inventory = new List<ItemSlot>();

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = "SELECT slot, itemId, quantity FROM Inventory WHERE saveSlot = @slot;";
            command.Parameters.AddWithValue("@slot", saveSlot);

            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var slot = new ItemSlot
                    {
                        slotIndex = reader.GetInt32(0),
                        itemId = reader.GetString(1),
                        quantity = reader.GetInt32(2)
                    };
                    inventory.Add(slot);
                }
            }
        }

        return inventory;
    }


}
