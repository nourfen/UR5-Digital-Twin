using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Windows;

public class DataProcessor : MonoBehaviour
{
    private NetworkManager _networkManager;
    private NetworkStream _networkStream = null;
    private TcpClient _client = null;

    private InboundDataProcessor _inboundDataProcessor;
    private OutboundDataProcessor _outboundDataProcessor;
    //Data
    private readonly byte[] _messageBuffer = new byte[4096];
    private double[] _angles = { 0, 0, 0, 0, 0, 0 };
    private Vector3 _movementInput = Vector3.zero;
    private Vector3 _rorationtInput = Vector3.zero;
    public double[] Angles => _angles;

    public Vector3 MovementInput  { get => _movementInput; set => _movementInput = value; }
    public Vector3 RotationInput  { get => _rorationtInput; set => _rorationtInput = value; }
    
    //Threading
    bool _isGetRobotInformationRunning = false;
    private Thread _robotInformation;
    private bool _isHandleUserInputRunning = false;
    private Thread _inputHandler;


    [SerializeField] private float _toolSpeed = 30f;
    [SerializeField] private float _axisAcceleration = 1f;
    [SerializeField] private float _functionReturnTime = 0.1f;

    private void Start()
    {
        //Init
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        _inboundDataProcessor = new InboundDataProcessor();
        _outboundDataProcessor = new OutboundDataProcessor();

        //Register Events
        RegisterEvents();

        //Threads
        _isGetRobotInformationRunning = true;
        _isHandleUserInputRunning = true;
        _robotInformation = new Thread(GetRobotInformation);
        _inputHandler = new Thread(HandleUserInput);
        _robotInformation.Start();
        _inputHandler.Start();
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    private void OnApplicationQuit()
    {
        Cleanup();
    }

    void GetRobotInformation()
    {
        while (_isGetRobotInformationRunning)
        {
            if (_networkStream != null && _networkStream.CanRead)
            {
                try
                {
                    int bytesRead = _networkStream.Read(_messageBuffer, 0, _messageBuffer.Length);
                    if (bytesRead > 0)
                    {
                        _angles = _inboundDataProcessor.ProcessRobotInformation(_messageBuffer, bytesRead);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error reading from network stream: {ex.Message}");
                }
            }
        }
    }

    private void HandleUserInput()
    {
        while (_isHandleUserInputRunning)
        {
            if ((_movementInput != Vector3.zero || _rorationtInput != Vector3.zero) && _networkStream != null && _networkStream.CanWrite)
            {
                try
                {
                    _outboundDataProcessor.CalculateAndSendTcpInstruction(_networkStream, _movementInput * _toolSpeed, _rorationtInput * 50, _axisAcceleration, _functionReturnTime);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error reading from network stream: {ex.Message}");
                }
            }

            Thread.Sleep(10);  // Avoid busy-waiting
        }
    }

    private void Cleanup()
    {
        UnregisterEvents();
        TerminateThreads();
        TerminateNetwork();
    }

    void RegisterEvents()
    {
        _networkManager.TCPClientStatusChanged += HandleTCPClientStatusChanged;
    }

    void UnregisterEvents()
    {
        _networkManager.TCPClientStatusChanged -= HandleTCPClientStatusChanged;
    }

    void TerminateThreads()
    {
        _isGetRobotInformationRunning = false;  // Signal the thread to stop
        _isHandleUserInputRunning = false;
        if (_robotInformation != null && _robotInformation.IsAlive)
        {
            _robotInformation.Join();  // Wait for the thread to finish
        }

        if (_inputHandler != null && _inputHandler.IsAlive)
        {
            _inputHandler.Join();  // Wait for the thread to finish
        }
    }

    void TerminateNetwork()
    {
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

    private void HandleTCPClientStatusChanged(TcpClient client)
    {
        if (client != null)
        {
            _client = client;
            _networkStream = _client.GetStream();
            Debug.Log("TCP Client not null");
            
        }
        else
        {
            Debug.Log("TCP Client is null");
        }
    }
}
