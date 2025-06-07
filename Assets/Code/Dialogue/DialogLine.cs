using UnityEngine;


[System.Serializable]
public class DialogLine
{
    public string speakerName;     // Nama karakter yang bicara
    [TextArea] public string text;  // Kalimat dialog
    public bool isChoice;          // Jika true, muncul pilihan Yes/No
    public bool triggersQuest;     // Bisa dipakai untuk trigger quest
    public bool triggersShop;      // Untuk trigger mode jual-beli
}