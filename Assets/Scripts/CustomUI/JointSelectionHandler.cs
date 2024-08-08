using CustomUI;
using TMPro;
using UnityEngine;

public class JointSelectionHandler : MonoBehaviour
{
    #region Events
    public delegate void JointIndexReady(int jointIndex);
    public event JointIndexReady OnJointIndexReady;
    #endregion
    
    #region Private Variables
    private int _currentJointIndex;
    private readonly string[] _ur5LinkNames =
        {"Shoulder Link", "Upper Arm Link", "Forearm Link", "Wrist Link 1", "Wrist Link 2", "Wrist Link 3"};

    private GameObject _jointNameTitle;
    private TextMeshPro _jointName;

    private GameObject _nextButton;
    private GameObject _previousButton;

    private ButtonStateHandler _nextButtonStateHandler;
    private ButtonStateHandler _previousButtonStateHandler;
    #endregion
    
    void Awake()
    {
        _nextButtonStateHandler = new ButtonStateHandler();
        _previousButtonStateHandler = new ButtonStateHandler();
    }
    void Start()
    {
        GetGameObjects();
        Initialize();
        UpdateButtonState();
    }

    #region Helper Functions
    public void NextJoint()
    {
        _currentJointIndex++;
        OnJointIndexReady?.Invoke(_currentJointIndex);
        UpdateButtonState();
        UpdateJointName();
    }

    public void PreviousJoint()
    {
        _currentJointIndex--;
        OnJointIndexReady?.Invoke(_currentJointIndex);
        UpdateButtonState();
        UpdateJointName();
    }

    private void UpdateJointName()
    {
        _jointName.text = _ur5LinkNames[_currentJointIndex];
    }

    private void UpdateButtonState()
    {
        switch (_currentJointIndex)
        {
            case 5:
                _nextButtonStateHandler.SetState(false);
                break;
            case 0:
                _previousButtonStateHandler.SetState(false);
                break;
            case > 0 and < 5:
                _nextButtonStateHandler.SetState(true);
                _previousButtonStateHandler.SetState(true);
                break;
        }
    }
    
    private void GetGameObjects()
    {
        _previousButton = this.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject;
        _nextButton = this.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject;
        _jointNameTitle = this.transform.GetChild(1).gameObject;
    }
    
    private void Initialize()
    {
        _nextButtonStateHandler.Initialize(_nextButton);
        _previousButtonStateHandler.Initialize(_previousButton);
        
        _jointName = _jointNameTitle.GetComponent<TextMeshPro>();
        _jointName.text = _ur5LinkNames[_currentJointIndex];
    }
    #endregion
    
}
