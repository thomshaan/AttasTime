using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestorePlayerPosition : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("PrevPosX"))
        {
            Vector3 savedPos = new Vector3(
                PlayerPrefs.GetFloat("PrevPosX"),
                PlayerPrefs.GetFloat("PrevPosY"),
                PlayerPrefs.GetFloat("PrevPosZ")
            );

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = savedPos;
            }

            // Hapus data posisi agar tidak digunakan ulang
            PlayerPrefs.DeleteKey("PrevPosX");
            PlayerPrefs.DeleteKey("PrevPosY");
            PlayerPrefs.DeleteKey("PrevPosZ");
        }
    }
}
