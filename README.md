# UR5 Digital Twin
A digital twin of the UR5 robot that replicates its movements in real-time, enabling users to both monitor and control the robot remotely.

# Motivation
I contributed to this project as part of the Horizon EU-funded initiative, IntellIoT. This use case was developed in collaboration with Siemens and several other companies and institutions. The application enables real-time remote control with a Stylus of a UR5 robot within an Augmented Reality (AR) environment using Microsoft's Hololens 2 AR glasses, where users can view the robot's camera streams directly within their space. 

When I started this project, I was completely new to the field of Robotics and figuring out how to set this up took me a long time but was a fun learning journey. That's why I developed a simplified desktop version of the application without the camera streams, including links that allow anyone with a Windows machine to control a simulated UR5 robot. 

My motivation for this project is to assist anyone trying to integrate Robotics within Unity and help them with the learning curve that comes with it. That is why I will include a thourough documentation within this readme. 

If you still require any help getting this project to work, please contact me on my socials:
+ [LinkedIn](https://www.linkedin.com/in/nour-fendri/)
+ [Email](nourfendri@hotmail.com)

# Project Description
This project contains a 3D environment with a UR5 robot model. It allows the user to monitor and control the UR5 robot accurately in real-time by providing the IP address of the robot's controller.

The robot is a Unified Robotics Description Format (URDF) model that was created using the ROS framework within a Ubuntu environment. I added the model to Unity using the [URDF-Importer Package](https://github.com/Unity-Technologies/URDF-Importer). I then used [Unity Robotics Hub Package](https://github.com/Unity-Technologies/Unity-Robotics-Hub) and attached the necessary scripts to control the robots movement by assigning values to the different robot joints. Once this was setup, I linked the digital and real robots using a bilateral [TCP/IP interface](https://s3-eu-west-1.amazonaws.com/ur-support-site/16496/ClientInterfaces_Primary.pdf). I extract the information I need (robot angles) from the updates received from the connection and apply them to the digital robot. The same interface is also used to send commands to the robot in the form of program snippets written using URScript programming language.

# Features
+ URDF file of the UR5 model with a Robotiq gripper.
+ Real-time monitoring of the real robot's movement in simulated environment.
+ Capabililty to control the real robot from the application through digital one using a keyboard. (Controls below)


# How To Replicate (Windows Device Only)
1. Download and extract the [Application Build](https://drive.google.com/file/d/1m69EcPrcXwI4A8r6mvVohL3BDwaqM_yl/view?usp=sharing).
2. Download and extract [URSim 3.15](https://s3-eu-west-1.amazonaws.com/ur-support-site/172183/URSim_VIRTUAL-3.15.8.106339.rar) VMWare Image.
3. Download and install [VMWare Player 17.5](https://softwareupdate.vmware.com/cds/vmw-desktop/player/17.5.1/23298084/windows/core/VMware-player-17.5.1-23298084.exe.tar)
4. Run VMWare Player
![Alt text](Documentation/Images/VMWare_Workstation.png)
5. Add URSIM's image (.vmx) to VMWare
![Alt text](Documentation/Images/Open.png)
![Alt text](Documentation/Images/URSim_vmx.png)
6. Click on **Edit virtual machine settings**
![Alt text](Documentation/Images/Edit_VM_Settings.png)
7. Configure the VM specifications as shown
- Memory: 4 GB
![Alt text](Documentation/Images/Memory.png)
- Processors: 4 vCPUs
![Alt text](Documentation/Images/Processors.png)
- Disk: 10 GB
- Network: Bridged (Automatic)
![Alt text](Documentation/Images/Network.png)
8. Click on **Play virtual machine**
![Alt text](Documentation/Images/Play.png)
9. Install VMWare Tools (A pop-up will appear when you run it for the first time)
![Alt text](Documentation/Images/VMW_Tools.png) 
10. Run UXTerm 
![Alt text](Documentation/Images/UXTerm.png) 
11. Type **ifconfig** to get the IP of the machine
![Alt text](Documentation/Images/ifconfig.png) 
12. Run **UR5 Sim** and make sure the robot is running
![Alt text](Documentation/Images/Initialise.png) 
13. Within the UR5 Sim home page, click on **Program Robot**
![Alt text](Documentation/Images/Program_Robot.png) 
14. Select the **Move** tab
![Alt text](Documentation/Images/Move.png) 
15. Run the Unity Application
![Alt text](Documentation/Images/Unity_Application.png) 
16. Type in the IP address from earlier
![Alt text](Documentation/Images/IP.png) 
17. Press "Connect"
18. Use the robot interface from the move tab with UR5 Sim to see the Digital Robot mimic its movement. Or, use the controls mentioned below to move the UR5 robot using the digital one.


# Controls

- Right Arrow: X+
- Left Arrow: X-
- Up Arrow: Y+
- Down Arrow: Y-
- K: Z-
- L: Z+
