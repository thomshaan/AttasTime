using System;
using System.IO.Ports;
using UnityEngine;

public class ESP32Controller : MonoBehaviour
{
    public string portName = "COM19";  // Change to your ESP32 COM port
    public int baudRate = 115200;

    public float moveSpeed = 5f;
    public float turnSpeed = 100f;

    private SerialPort serialPort;
    private float moveXPercent = 50f;  // Default center (joystick middle)
    private float moveYPercent = 50f;
    private bool joyButtonPressed = false;
    private bool button2Pressed = false;

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 50;
            serialPort.Open();
            Debug.Log("Serial port opened: " + portName);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
    }

    void Update()
    {
        ReadSerial();

        // Map Y-axis (moveYPercent) from 0-100 to -1 to 1 movement forward/backward
        float moveAmount = ((moveYPercent - 50f) / 50f) * moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveAmount);

        // Map X-axis (moveXPercent) from 0-100 to -1 to 1 rotation left/right
        float turnAmount = ((moveXPercent - 50f) / 50f) * turnSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, turnAmount);

        // Optional: Use joystick button (joyButtonPressed) and button2Pressed for other actions
        // For example, stop movement or trigger animations
    }

    void ReadSerial()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                while (serialPort.BytesToRead > 0)
                {
                    string line = serialPort.ReadLine().Trim();
                    // Debug.Log("Received: " + line);

                    if (line.StartsWith("VRX:"))
                    {
                        // Parse VRX percent value from format: "VRX: 2048 (50%)"
                        int start = line.IndexOf('(');
                        int end = line.IndexOf('%');
                        if (start > 0 && end > start)
                        {
                            string percentStr = line.Substring(start + 1, end - start - 1);
                            if (float.TryParse(percentStr, out float val))
                            {
                                moveXPercent = val;
                            }
                        }
                    }
                    else if (line.StartsWith("VRY:"))
                    {
                        // Parse VRY percent value
                        int start = line.IndexOf('(');
                        int end = line.IndexOf('%');
                        if (start > 0 && end > start)
                        {
                            string percentStr = line.Substring(start + 1, end - start - 1);
                            if (float.TryParse(percentStr, out float val))
                            {
                                moveYPercent = val;
                            }
                        }
                    }
                    else if (line.StartsWith("Joy Btn:"))
                    {
                        joyButtonPressed = line.Contains("Pressed");
                    }
                    else if (line.StartsWith("Btn2:"))
                    {
                        button2Pressed = line.Contains("Pressed");
                    }
                }
            }
            catch (TimeoutException)
            {
                // No data currently available
            }
            catch (Exception e)
            {
                Debug.LogError("Serial read error: " + e.Message);
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}
