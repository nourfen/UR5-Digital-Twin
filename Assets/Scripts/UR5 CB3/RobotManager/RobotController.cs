using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    private DataProcessor _dataProcessor;
    private Vector3 _input = Vector3.zero;


    void Start()
    {
        _dataProcessor = GameObject.Find("RobotManager").GetComponent<DataProcessor>();
    }

    void Update()
    {

         GenerateUserInput();
        _dataProcessor.Input = _input;
        _input = Vector3.zero;
    }

    void GenerateUserInput()
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
