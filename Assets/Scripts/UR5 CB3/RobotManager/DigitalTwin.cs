using System.Linq;
using UnityEngine;

public class DigitalTwin : MonoBehaviour
{
    #region Robot CAD Model Configuration
    private const int NumRobotJoints = 6;
    private ArticulationBody[] _jointArticulationBodies;
    #endregion

    public GameObject UR5Robot;

    private NetworkManager _networkManager;
    // Start is called before the first frame update

    private void Awake()
    {
        _jointArticulationBodies = new ArticulationBody[NumRobotJoints];
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
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
        if (_networkManager.Angles == new double[] { 0, 0, 0, 0, 0, 0 }) return;
        MimicRealRobot(_networkManager.Angles);
    }
    
    /// <summary>
    /// Reads and parses the joint rotational values, transforms them into values suitable for articulation bodies and assigns them to the corresponding joint;
    /// </summary>
    /// <param name="robotAngles"></param>
    /// <returns></returns>
    private void MimicRealRobot(double[] robotAngles)
    {
        var result = robotAngles.Select(r => (float)r * Mathf.Rad2Deg).ToArray();
        
        for (var joint = 0; joint < _jointArticulationBodies.Length; joint++)
        {
            //Struct has to be accessed as a variable
            var joint1XDrive = _jointArticulationBodies[joint].xDrive;
            joint1XDrive.target = result[joint];
            _jointArticulationBodies[joint].xDrive = joint1XDrive;
        }
    }
}