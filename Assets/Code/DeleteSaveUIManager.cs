using UnityEngine;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;

public class DeleteSaveUIManager : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu"; // Ganti sesuai nama scene menu utama kamu

    private string dbPath => "URI=file:" + UnityEngine.Application.persistentDataPath + "/GameSave.db";

    public void OnDeleteSaveClicked()
    {
        using var connection = new SqliteConnection(dbPath);
        connection.Open();

        using (var command = connection.CreateCommand())
        {
            // Hapus semua data dari tabel utama
            command.CommandText = @"
                DELETE FROM Inventory;
                DELETE FROM PlayerStats;
                DELETE FROM Quest;
                DELETE FROM GameTime;
            ";
            command.ExecuteNonQuery();
        }

        Debug.Log("[DeleteSaveUIManager] Semua data save di database berhasil dihapus.");

        // Reload scene atau kembali ke main menu
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
