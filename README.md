# UR5 Digital Twin
A robot digital twin that allows the mimicing and control of Universal Robots' UR5 robot.

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
