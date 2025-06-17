using System.IO.Ports;
using UnityEngine;

public class JoystickMovement : MonoBehaviour
{
    public CharacterController characterController;
    public GameObject pauseMenu;  // Reference to the pause menu
    public GameObject minimapPanel;  // Reference to the minimap panel
    private bool isPaused = false;  // Track the pause state

    SerialPort sp = new SerialPort("COM18", 115200);  // Change COM18 to the correct port for your ESP32
    private float moveX = 0f, moveY = 0f; // Initialize joystick movement values
    private bool touchDetected = false; // Track touch sensor state
    private bool jsb = false; // Track joystick button press
    private float potentiometerValue = 0f; // Potentiometer value to control Time of Day

    private LightingManager lightingManager;  // Reference to the LightingManager to control TimeOfDay

    void Start()
    {
        try
        {
            sp.Open();  // Open the serial port
            sp.ReadTimeout = 50;  // Set timeout for reading data
            Debug.Log("Serial port opened successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }

        // Get LightingManager reference
        lightingManager = FindObjectOfType<LightingManager>();
    }

    void Update()
    {
        if (sp.IsOpen)
        {
            // Read data from the serial port and parse it
            try
            {
                if (sp.BytesToRead > 0)  // Check if data is available
                {
                    string data = sp.ReadLine();  // Read serial data

                    if (!string.IsNullOrEmpty(data))
                    {
                        // Parse joystick data for movement, touch sensor state, and potentiometer value
                        ParseJoystickData(data);

                        // Handle pause button or touch sensor state
                        if (touchDetected)
                        {
                            TogglePause();  // Pause or resume the game
                        }

                        // Handle minimap button (button 1 press)
                        if (jsb)
                        {
                            ToggleMinimap();  // Toggle the minimap panel
                        }

                        // Map potentiometer value (0-4095) to TimeOfDay (0-24)
                        float timeOfDay = (potentiometerValue / 4095f) * 24f;  // Convert potentiometer value to day-night cycle

                        // Debugging output for TimeOfDay
                        Debug.Log("Potentiometer Value: " + potentiometerValue + " | Mapped TimeOfDay: " + timeOfDay);

                        // Set the TimeOfDay only from the potentiometer, not from button presses
                        lightingManager.SetTimeOfDay(timeOfDay);  // Set the TimeOfDay in LightingManager
                    }
                }
            }
            catch (System.TimeoutException)
            {
                // Ignore timeout exception (no data)
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading serial data: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Serial port is not open.");
        }

        // Optionally, allow manual pause using the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // Parse joystick data received from the serial port
    private void ParseJoystickData(string data)
    {
        string[] dataValues = data.Split('\n'); // Split data based on newline

        foreach (string value in dataValues)
        {
            if (value.StartsWith("VRX:"))
            {
                moveX = float.Parse(value.Substring(4)); // Get X-axis value (joystick movement)
            }
            else if (value.StartsWith("VRY:"))
            {
                moveY = float.Parse(value.Substring(4)); // Get Y-axis value (joystick movement)
            }
            else if (value.StartsWith("Touch:"))
            {
                touchDetected = value.Substring(6) == "1"; // Set touch detection
            }
            else if (value.StartsWith("JS:"))
            {
                jsb = value.Substring(3) == "1"; // Set joystick button press state
            }
            else if (value.StartsWith("Pot:"))
            {
                potentiometerValue = float.Parse(value.Substring(4)); // Get potentiometer value
            }
        }
    }

    // Move the character based on joystick input (you can implement movement logic if necessary)
    private void MoveCharacter()
    {
        Vector3 move = new Vector3(moveX, 0f, moveY);  // X -> left/right, Y -> forward/backward
        move = transform.TransformDirection(move);  // Adjust movement according to character's orientation

        // Apply movement to the character using CharacterController
        characterController.Move(move * 5f * Time.deltaTime);  // Adjust the movement speed as needed
    }

    // Toggle the pause state
    private void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);  // Show or hide the pause menu
        Time.timeScale = isPaused ? 0f : 1f;  // Pause or resume the game (freeze or unfreeze time)
    }

    // Toggle the minimap panel
    private void ToggleMinimap()
    {
        minimapPanel.SetActive(!minimapPanel.activeSelf);  // Toggle the minimap visibility
    }

    private void OnApplicationQuit()
    {
        if (sp.IsOpen)
        {
            sp.Close();  // Close the serial port when the application quits
        }
    }
}
