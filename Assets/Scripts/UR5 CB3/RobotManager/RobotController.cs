using UnityEngine;

public class RobotController : MonoBehaviour
{
    private DataProcessor _dataProcessor;
    private Vector3 _movementInput = Vector3.zero;
    private Vector3 _rotationtInput = Vector3.zero;


    void Start()
    {
        _dataProcessor = GameObject.Find("RobotManager").GetComponent<DataProcessor>();
    }

    void Update()
    {
        bool isControlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (isControlHeld) {
            _rotationtInput = GenerateUserInput(_movementInput);
            _dataProcessor.RotationInput = _rotationtInput;
            _rotationtInput = Vector3.zero;
        } else
        {
            _movementInput = GenerateUserInput(_movementInput);
            _dataProcessor.MovementInput = _movementInput;
            _movementInput = Vector3.zero;
        }
    }

    Vector3 GenerateUserInput(Vector3 input)
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            input += Vector3.right;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            input += Vector3.left;
        }

        if (Input.GetKey(KeyCode.K))
        {
            input += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            input += Vector3.up;
        }

        if (Input.GetKey(KeyCode.L))
        {
            input += Vector3.back;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            input += Vector3.down;
        }

        return input *= Time.deltaTime;
    }
}
