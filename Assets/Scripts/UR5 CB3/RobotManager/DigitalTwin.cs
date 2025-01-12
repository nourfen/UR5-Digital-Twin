using System.Linq;
using UnityEngine;

public class DigitalTwin : MonoBehaviour
{
    #region Robot CAD Model Configuration
    private const int NumRobotJoints = 6;
    private ArticulationBody[] _jointArticulationBodies;
    private ArticulationBody _leftKnuckle;
    private ArticulationBody _leftInnerKnuckle;
    private ArticulationBody _leftInnerKnuckleFingerTip;
    private ArticulationBody _rightKnuckle;
    private ArticulationBody _rightInnerKnuckle;
    private ArticulationBody _rightInnerKnuckleFingerTip;

    public GameObject leftKnuckle;
    public GameObject rightKnuckle;
    public GameObject leftInnerKnuckle;
    public GameObject rightInnerKnuckle;
    public GameObject leftInnerKnuckleFingerTip;
    public GameObject rightInnerKnuckleFingerTip;


    public float leftKnuckleDriveTarget = 0f;
    public float rightKnuckleDriveTarget = 0f;
    public float leftInnerKnuckleDriveTarget = 0f;
    public float rightInnerKnuckleDriveTarget = 0f;
    public float leftInnerKnuckleFingerTipDriveTarget = 0f;
    public float rightInnerKnuckleFingerTipDriveTarget = 0f;
    public GameObject UR5Robot;
    #endregion

    private DataProcessor _dataProcessor;

    private void Awake()
    {
        _jointArticulationBodies = new ArticulationBody[NumRobotJoints];
        _dataProcessor = GameObject.Find("RobotManager").GetComponent<DataProcessor>();
    }

    void Start()
    {
        //Loading the six joints of the UR5Robot within.
        var linkName = string.Empty;
        for (var i = 0; i < NumRobotJoints; i++)
        {
            linkName += SourceDestinationPublisher.Ur5LinkNames[i];
            _jointArticulationBodies[i] = UR5Robot.transform.Find(linkName).GetComponent<ArticulationBody>();
        }

        //var leftInnerKnuckleLink = linkName + "/tool0/robotiq_coupler/robotiq_85_base_link/robotiq_85_left_inner_knuckle_link";
        //var leftKnuckleLink = linkName + "/tool0/robotiq_coupler/robotiq_85_base_link/robotiq_85_left_knuckle_link";
        ////var leftInnerKnuckleLinkFingerTip = linkName + "/tool0/robotiq_coupler/robotiq_85_base_link/robotiq_85_left_inner_knuckle_link/robotiq_85_left_finger_tip_link";

        //var rightInnerKnuckleLink = linkName + "/tool0/robotiq_coupler/robotiq_85_base_link/robotiq_85_right_inner_knuckle_link";
        //var rightKnuckleLink = linkName + "/tool0/robotiq_coupler/robotiq_85_base_link/robotiq_85_right_knuckle_link";
        ////var rightInnerKnuckleLinkFingerTip = "/tool0/robotiq_coupler/robotiq_85_base_link/robotiq_85_right_inner_knuckle_link/robotiq_85_right_finger_tip_link";

        //var rightInnerKnuckleTransform = UR5Robot.transform.Find(rightKnuckleLink);
        _rightKnuckle = rightKnuckle.GetComponent<ArticulationBody>();
        _rightInnerKnuckle = rightInnerKnuckle.GetComponent<ArticulationBody>();
        _rightInnerKnuckleFingerTip = rightInnerKnuckleFingerTip.GetComponent<ArticulationBody>();

        _leftKnuckle = leftKnuckle.GetComponent<ArticulationBody>();
        _leftInnerKnuckle = leftInnerKnuckle.GetComponent<ArticulationBody>();
        _leftInnerKnuckleFingerTip = leftInnerKnuckleFingerTip.GetComponent<ArticulationBody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.O)) { 
            OpenGripper();
        }

        if (Input.GetKey(KeyCode.P))
        {
            CloseGripper();
        }

        if (_dataProcessor.Angles == new double[] { 0, 0, 0, 0, 0, 0 }) return;
        MimicRealRobot(_dataProcessor.Angles);
    }

    /// <summary>
    /// Reads and parses the joint rotational values, transforms them into values suitable for articulation bodies and assigns them to the corresponding joint;
    /// </summary>
    /// <param name="robotAngles"></param>
    /// <returns></returns>
    private void MimicRealRobot(double[] robotAnglesInRadian)
    {
        var robotAnglesInDegrees = robotAnglesInRadian.Select(r => (float)r * Mathf.Rad2Deg).ToArray();

        for (var joint = 0; joint < _jointArticulationBodies.Length; joint++)
        {
            //Struct has to be accessed as a variable
            var joint1XDrive = _jointArticulationBodies[joint].xDrive;
            joint1XDrive.target = robotAnglesInDegrees[joint];
            _jointArticulationBodies[joint].xDrive = joint1XDrive;
        }
    }

    void OpenGripper()
    {
        var leftKnuckleDrive = _leftKnuckle.xDrive;
        var rightKnuckleDrive = _rightKnuckle.xDrive;
        var leftInnerKnuckleDrive = _leftInnerKnuckleFingerTip.xDrive;
        var leftInnerKnuckleFingerTipDrive = _leftInnerKnuckle.xDrive;
        var rightInnerKnuckleDrive = _rightInnerKnuckle.xDrive;
        var rightInnerKnuckleFingerTipDrive = _rightInnerKnuckleFingerTip.xDrive;

        leftInnerKnuckleDrive.target = 0f;
        leftKnuckleDrive.target = 0f;
        rightInnerKnuckleDrive.target = 0f;
        rightKnuckleDrive.target = 0f;
        leftInnerKnuckleFingerTipDrive.target = 0f;
        rightInnerKnuckleFingerTipDrive.target = 0f;

        _leftKnuckle.xDrive = leftKnuckleDrive;
        _leftInnerKnuckle.xDrive = leftInnerKnuckleDrive;
        _rightKnuckle.xDrive = rightKnuckleDrive;
        _rightInnerKnuckle.xDrive = rightInnerKnuckleDrive;
        _rightInnerKnuckleFingerTip.xDrive = rightInnerKnuckleFingerTipDrive;
        _leftInnerKnuckleFingerTip.xDrive = leftInnerKnuckleFingerTipDrive;
    }

    void CloseGripper()
    {
        var leftKnuckleDrive = _leftKnuckle.xDrive;
        var rightKnuckleDrive = _rightKnuckle.xDrive;
        var leftInnerKnuckleDrive = _leftInnerKnuckle.xDrive;
        var rightInnerKnuckleDrive = _rightInnerKnuckle.xDrive;
        var leftInnerKnuckleFingerTipDrive = _leftInnerKnuckle.xDrive;
        var rightInnerKnuckleFingerTipDrive = _rightInnerKnuckleFingerTip.xDrive;

        leftKnuckleDrive.target = leftKnuckleDriveTarget;
        leftInnerKnuckleDrive.target = leftInnerKnuckleDriveTarget;
        rightKnuckleDrive.target = rightKnuckleDriveTarget;
        rightInnerKnuckleDrive.target = rightInnerKnuckleDriveTarget;
        leftInnerKnuckleFingerTipDrive.target = leftInnerKnuckleFingerTipDriveTarget;
        rightInnerKnuckleFingerTipDrive.target = rightInnerKnuckleFingerTipDriveTarget;


        _leftKnuckle.xDrive = leftKnuckleDrive;
        _leftInnerKnuckle.xDrive = leftInnerKnuckleDrive;
        _rightKnuckle.xDrive = rightKnuckleDrive;
        _rightInnerKnuckle.xDrive = rightInnerKnuckleDrive;
        _rightInnerKnuckleFingerTip.xDrive = leftInnerKnuckleFingerTipDrive;
        _leftInnerKnuckleFingerTip.xDrive = leftInnerKnuckleFingerTipDrive;
    }
}