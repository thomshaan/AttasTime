using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ESP32CharacterController : MonoBehaviour
{
    public string esp32IP = "http://YOUR_ESP32_IP/move"; // Change to your ESP32's IP address
    public float moveSpeed = 5f;
    private bool isMoving = false;

    void Update()
    {
        StartCoroutine(CheckButtonState());
        
        if (isMoving)
        {
            MoveCharacter();
        }
    }

    IEnumerator CheckButtonState()
    {
        UnityWebRequest www = UnityWebRequest.Get(esp32IP);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // If the button is pressed on ESP32
            isMoving = www.downloadHandler.text == "Move";
        }
    }

    void MoveCharacter()
    {
        // Simple movement logic (move forward)
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
