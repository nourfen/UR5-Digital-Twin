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
1. Download the [Unity Application Build](https://drive.google.com/file/d/1m69EcPrcXwI4A8r6mvVohL3BDwaqM_yl/view?usp=sharing)
2. Download [URSim 3.15](https://s3-eu-west-1.amazonaws.com/ur-support-site/172183/URSim_VIRTUAL-3.15.8.106339.rar)
3. Download [VMWare Player 17.5](https://softwareupdate.vmware.com/cds/vmw-desktop/player/17.5.1/23298084/windows/core/VMware-player-17.5.1-23298084.exe.tar)
4. Extract URSim Folder
5. Install and run VMWare Player
6. Within VMWare: Player - File - Open...
7. Locate the URSim VM file and double click it
8. Configure the VM specifications as shown
	a. Memory: 4 GB
	b. Processors: 4 vCPUs
	c. Disk: 10 GB
	d. Network: Bridged (Automatic)
9. Run VM (Virtual Machine)
10. Install VMWare Tools
11. Run UXTerm and type "ifconfig" to get the IP of the machine
12. Double click on UR5 Sim and make sure the robot is running
13. Run the Unity Application
14. Type in the IP address from earlier
15. Press "Connect"

# Controls

- Right Arrow: X+
- Left Arrow: X-
- Up Arrow: Y+
- Down Arrow: Y-
- K: Z-
- L: Z+