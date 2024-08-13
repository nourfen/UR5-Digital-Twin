using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using UnityEngine;

public class NetworkManager : MonoBehaviour 
{
    #region Network TCP Configuration
    private const int PORT = 30001;
    private static TcpClient? _client;
    private NetworkStream _networkStream;
    public NetworkStream NetworkStream => _networkStream;
    #endregion

    public TextMeshProUGUI DebugArea;
    public GameObject UserInterfaceCanvas;

    //Events
    public delegate void OnNetworkStreamStatusChanged(NetworkStream stream);
    public event OnNetworkStreamStatusChanged NetworkStreamStatusChanged;
    //Data
    private readonly byte[] _messageBuffer = new byte[4096];
    private double[] _angles = {0, 0, 0, 0, 0, 0};
    public double[] Angles => _angles;
    //Threading
    bool _isRunning = false;
    private Thread _robotInformation;

    private void Start()
    {
        _isRunning = true;
        _robotInformation = new Thread(GetRobotInformation);
        _robotInformation.Start();
    }

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
        _isRunning = false;  // Signal the thread to stop

        if (_robotInformation != null && _robotInformation.IsAlive)
        {
            _robotInformation.Join();  // Wait for the thread to finish
        }

        if (_networkStream != null)
        {
            _networkStream.Close();  // Properly close the network stream
            _networkStream.Dispose();
            _networkStream = null;
        }

        if (_client != null)
        {
            _client.Close();  // Properly close the TCP client
            _client.Dispose();
            _client = null;
        }
    }

    void GetRobotInformation()
    {
        while (_isRunning)
        {
            if (_networkStream != null && _networkStream.CanRead)
            {
                try
                {
                    int bytesRead = _networkStream.Read(_messageBuffer, 0, _messageBuffer.Length);
                    if (bytesRead > 0)
                    {
                        ProcessRobotInformation(_messageBuffer, bytesRead);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error reading from network stream: {ex.Message}");
                }
            }
        }
    }

    void ProcessRobotInformation(byte[] buffer, int bytesRead)
    {
        //The first four bytes contain the message length, the byte array needs to be reversed because of endian
        byte[] messageStateLengthBuffer = ReverseByteArray(buffer.Take(4).ToArray());
        int messageStateLength = BitConverter.ToInt32(messageStateLengthBuffer, 0);

        //The next byte contains the message type
        byte messageType = buffer.Skip(4).Take(1).ToArray()[0];

        //MESSAGE_TYPE_ROBOT_STATE = 16 contains packages about the robot's state
        if (messageType == 16)
        {
            //The message is divided into bytes and it is important to know exactly where you are within the message in order to get the correct information
            int currentMessagePosition = 0;
            //Every increase is in bytes, the first five bytes represent the "message length" and the "message type"
            while (currentMessagePosition + 5 < messageStateLength)
            {
                //We skip the first 5 bytes targeted to the message and take 4 that contains the package size
                byte[] packetLengthBuffer = ReverseByteArray(buffer.Skip(5 + currentMessagePosition).Take(4).ToArray());
                int packetLength = BitConverter.ToInt32(packetLengthBuffer, 0);
                //We skip the first 9 bytes targeted to the message and package size and take the next 1 that contains the package type
                byte packageType = buffer.Skip(9 + currentMessagePosition).Take(1).ToArray()[0];

                //Extract robot joint values ROBOT_STATE_PACKAGE_TYPE_JOINT_DATA = 1
                if (packageType == 1)
                {
                    int j = 0;
                    while (j < 6)
                    {
                        //The package contains a lot of data that we do not require and so the "j * 41" is used to skip the data we do not need
                        //8 bytes are taken because each robot joint angle is represented by a double
                        _angles[j] = BitConverter.ToDouble(
                            ReverseByteArray(buffer.Skip(10 + currentMessagePosition + (j * 41)).Take(8).ToArray()));
                        j++;
                    }
                }
                currentMessagePosition += packetLength;
            }
        }
    }

    private static byte[] ReverseByteArray(byte[] array)
    {
        Array.Reverse(array);
        return array;
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

            //Fetching the stream
            _networkStream = _client.GetStream();

            //Disable the UI
            UserInterfaceCanvas.SetActive(false);

            //Triggering the Event
            NetworkStreamStatusChanged.Invoke(_networkStream);
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
