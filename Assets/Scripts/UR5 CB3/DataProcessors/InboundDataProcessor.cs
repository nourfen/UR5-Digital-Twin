using System;
using System.Linq;

public class InboundDataProcessor
{

    double[] angles = { 0, 0, 0, 0, 0, 0 };
    public double[] ProcessRobotInformation(byte[] buffer, int bytesRead)
    {
        //The first four bytes contain the message length, the byte array needs to be reversed because of endian
        byte[] messageStateLengthBuffer = ReverseByteArray(buffer.Take(4).ToArray());
        int messageStateLength = BitConverter.ToInt32(messageStateLengthBuffer, 0);

        //The next byte contains the message type
        byte messageType = buffer.Skip(4).Take(1).ToArray()[0];

        //MESSAGE_TYPE_ROBOT_STATE = 16 contains packages about the robot's state
        if (messageType == 16)
        {
            //The message is divided into bytes and it is important to know exactly where you are within the message in order to get the correct information
            int currentMessagePosition = 0;
            //Every increase is in bytes, the first five bytes represent the "message length" and the "message type"
            while (currentMessagePosition + 5 < messageStateLength)
            {
                //We skip the first 5 bytes targeted to the message and take 4 that contains the package size
                byte[] packetLengthBuffer = ReverseByteArray(buffer.Skip(5 + currentMessagePosition).Take(4).ToArray());
                int packetLength = BitConverter.ToInt32(packetLengthBuffer, 0);
                //We skip the first 9 bytes targeted to the message and package size and take the next 1 that contains the package type
                byte packageType = buffer.Skip(9 + currentMessagePosition).Take(1).ToArray()[0];

                //Extract robot joint values ROBOT_STATE_PACKAGE_TYPE_JOINT_DATA = 1
                if (packageType == 1)
                {
                    int j = 0;
                    while (j < 6)
                    {
                        //The package contains a lot of data that we do not require and so the "j * 41" is used to skip the data we do not need
                        //8 bytes are taken because each robot joint angle is represented by a double
                        angles[j] = BitConverter.ToDouble(
                            ReverseByteArray(buffer.Skip(10 + currentMessagePosition + (j * 41)).Take(8).ToArray()));
                        j++;
                    }
                }
                currentMessagePosition += packetLength;
            }
        }

        return angles;
    }

    private static byte[] ReverseByteArray(byte[] array)
    {
        Array.Reverse(array);
        return array;
    }

}
