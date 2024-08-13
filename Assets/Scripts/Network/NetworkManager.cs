using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using UnityEngine;

public class NetworkManager : MonoBehaviour 
{
    #region Network TCP Configuration
    private const int PORT = 30001;
    private static TcpClient _client;
    #endregion

    public TextMeshProUGUI DebugArea;
    public GameObject UserInterfaceCanvas;

    //Events
    public delegate void OnTCPClientStatusChanged(TcpClient client);
    public event OnTCPClientStatusChanged TCPClientStatusChanged;

    private void OnDestroy()
    {
        Cleanup();
    }

    private void OnApplicationQuit()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        if (_client != null)
        {
            _client.Close();  // Properly close the TCP client
            _client.Dispose();
            _client = null;
        }
    }

    public async void Connect(TextMeshProUGUI machineIpAddress)
    {
        try
        {
            //Cleaning up the IP
            string ip = machineIpAddress.GetParsedText();
            string cleanedIp = Regex.Replace(ip, @"[^0-9\.]", "");
            var ipAddress = IPAddress.Parse(cleanedIp);

            //Connecting
            _client = new TcpClient(AddressFamily.InterNetwork);
            await _client.ConnectAsync(ipAddress, PORT);

            //Triggering the Event
            TCPClientStatusChanged.Invoke(_client);

            //Disable the UI
            UserInterfaceCanvas.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            //Parsing Error
            DebugArea.color = Color.red;
            DebugArea.text = $"Error: {ex.Message}";
        }
    }
}
//192.168.178.87
