using System;
using System.IO.Ports;
using UnityEngine;

public class ArduinoInputController : MonoBehaviour
{
    public string portName = "COM19";   // Ganti sesuai port Arduino kamu
    public int baudRate = 9600;

    private SerialPort serialPort;
    private ThirdPersonController tpc;

    void Start()
    {
        tpc = GetComponent<ThirdPersonController>();
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 50;
            serialPort.Open();
            Debug.Log("Serial port opened");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                while (serialPort.BytesToRead > 0)
                {
                    string data = serialPort.ReadLine().Trim();
                    // Debug.Log("Received: " + data);

                    if (data == "yellow-left" || data == "1")
                    {
                        // Kalau mau pakai input kiri, bisa panggil method khusus misal SetForceLeft(true)
                        // Tapi untuk sekarang fokus maju (forward)
                        tpc.SetForceForward(true);
                    }
                    else if (data == "green-right" || data == "2")
                    {
                        // Bisa buat flag lain kalau mau, misal side movement
                        // Tapi misal sekarang hanya stop forward movement
                        tpc.SetForceForward(false);
                    }
                    else if (data == "no-input")
                    {
                        tpc.SetForceForward(false);
                    }
                    else
                    {
                        // Kalau data tidak dikenali, matikan forward
                        tpc.SetForceForward(false);
                    }
                }
            }
            catch (TimeoutException)
            {
                // Timeout biasa kalau nggak ada data, tidak masalah
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed");
        }
    }
}
