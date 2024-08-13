using System.Linq;
using UnityEngine;

public class DigitalTwin : MonoBehaviour
{
    #region Robot CAD Model Configuration
    private const int NumRobotJoints = 6;
    private ArticulationBody[] _jointArticulationBodies;
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
}