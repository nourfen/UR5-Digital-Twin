using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class OutboundDataProcessor
{
    #region Robot TCP/IP Calculations
    public void CalculateAndSendTcpInstruction(NetworkStream networkStream, Vector3 userMovementInput, Vector3 userRotationInput, float axisAcceleration, float functionReturnTime)
    {


        Vector3 userInputInUr5Space = TransformToUr5Space(userMovementInput);
        string pose = $"speedl([{userInputInUr5Space.x},{userInputInUr5Space.y},{userInputInUr5Space.z}, {userRotationInput.x}, {userRotationInput.y}, {userRotationInput.z}], {axisAcceleration}, {functionReturnTime})";
        string instruction = $"def myProg():\n {pose}\nend\n";

        SendTcpInstruction(instruction, networkStream);
    }

    private Vector3 TransformToUr5Space(Vector3 unitySpaceVector)
    {
        return new Vector3(-unitySpaceVector.z, unitySpaceVector.x, unitySpaceVector.y);
    }


    /// <summary>
    /// Encodes the instruction and sends it to the robot through the TCP connection 
    /// </summary>
    /// <param name="instruction"></param>
    private void SendTcpInstruction(string instruction, NetworkStream networkStream)
    {
        if (networkStream != null)
        {
            byte[] encodedInstruction =
                Encoding.ASCII.GetBytes(instruction);
            networkStream.WriteAsync(encodedInstruction, 0, encodedInstruction.Length);
        }
    }
    #endregion
}
