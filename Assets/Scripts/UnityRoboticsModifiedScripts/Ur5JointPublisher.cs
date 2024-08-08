using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class Ur5JointPublisher : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    GameObject m_Ur5Robot;

    const int k_NumRobotJoints = 6;

    public static readonly string[] Ur5LinkNames =
        {"world/base_link/shoulder_link", "/upper_arm_link", "/forearm_link", "/wrist_1_link", "/wrist_2_link", "/wrist_3_link"};

    UrdfJointRevolute[] m_JointArticulationBodies;
    void Start()
    {
        m_JointArticulationBodies = new UrdfJointRevolute[k_NumRobotJoints];

        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            linkName += Ur5LinkNames[i];
            m_JointArticulationBodies[i] = m_Ur5Robot.transform.Find(linkName).GetComponent<UrdfJointRevolute>();
            Debug.Log("JOINT NAME " + i + " " + m_JointArticulationBodies[i].name);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
