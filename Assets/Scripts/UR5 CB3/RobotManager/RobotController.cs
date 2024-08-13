using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    private NetworkStream _networkStream;
    private NetworkManager _networkManager;
    private Vector3 _input = Vector3.zero;
    private bool _isRunning;
    private Thread _inputHandler;

    private readonly object _inputLock = new object();


    [SerializeField] private float _toolSpeed = 15f;
    [SerializeField] private float _axisAcceleration = 1f;
    [SerializeField] private float _functionReturnTime = 0.1f;

    void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        _networkManager.NetworkStreamStatusChanged += HandleNetworkStreamStatusChanged;
        _networkStream = null;

        _isRunning = true;
        _inputHandler = new Thread(HandleUserInput);
        _inputHandler.Start();
    }

    void Update()
    {

        //Get Input from user
        lock (_inputLock)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Debug.Log("Robot Move: X +");
                _input += Vector3.right;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Debug.Log("Robot Move: X -");
                _input += Vector3.left;
            }

            if (Input.GetKey(KeyCode.K))
            {
                Debug.Log("Robot Move: Z +");
                _input += Vector3.forward;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                Debug.Log("Robot Move: Y +");
                _input += Vector3.up;
            }

            if (Input.GetKey(KeyCode.L))
            {
                Debug.Log("Robot Move: Z -");
                _input += Vector3.back;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                Debug.Log("Robot Move: Y -");
                _input += Vector3.down;
            }

            _input *= Time.deltaTime;
        }

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
        _networkManager.NetworkStreamStatusChanged -= HandleNetworkStreamStatusChanged;

        if (_inputHandler != null && _inputHandler.IsAlive)
        {
            _inputHandler.Join();  // Wait for the thread to finish
        }

        if (_networkStream != null)
        {
            _networkStream.Close();  // Properly close the network stream
            _networkStream.Dispose();
            _networkStream = null;
        }
    }

    private void HandleUserInput()
    {
        while (_isRunning)
        {
            if (_networkStream != null && _networkStream.CanWrite)
            {
                try
                {
                    Vector3 input;
                    lock (_inputLock)
                    {
                        input = _input;
                        _input = Vector3.zero;
                    }

                    if (input != Vector3.zero)
                    {
                        CalculateAndSendTcpInstruction(input * _toolSpeed);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error reading from network stream: {ex.Message}");
                }
            }

            Thread.Sleep(10);  // Avoid busy-waiting
        }
    }

    #region Robot TCP/IP Calculations
    private void CalculateAndSendTcpInstruction(Vector3 userInput)
    {


        Vector3 userInputInUr5Space = TransformToUr5Space(userInput);
        string pose = $"speedl([{userInputInUr5Space.x},{userInputInUr5Space.y},{userInputInUr5Space.z}, {0}, {0}, {0}], {_axisAcceleration}, {_functionReturnTime})";
        string instruction = $"def myProg():\n {pose}\nend\n";

        SendTcpInstruction(instruction);
    }

    private Vector3 TransformToUr5Space(Vector3 unitySpaceVector)
    {
        return new Vector3(-unitySpaceVector.z, unitySpaceVector.x, unitySpaceVector.y);
    }


    /// <summary>
    /// Encodes the instruction and sends it to the robot through the TCP connection 
    /// </summary>
    /// <param name="instruction"></param>
    private void SendTcpInstruction(string instruction)
    { 
        if (_networkStream != null)
        {
            byte[] encodedInstruction =
                Encoding.ASCII.GetBytes(instruction); 
            _networkStream.WriteAsync(encodedInstruction, 0, encodedInstruction.Length);
        }
    }
    #endregion

    private void HandleNetworkStreamStatusChanged(NetworkStream networkStream)
    {
        if (networkStream != null)
        {
            _networkStream = networkStream;
            Debug.Log("Network Stream not null");
        } else
        {
            Debug.Log("Network Stream is null");
        }
    }
}
