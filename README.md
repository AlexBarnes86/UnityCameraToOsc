# Unity Camera to OSC
Create virtual cameras in Unity.

* Sends low resolution version of virtual cameras (10x10) rgb over OSC.
* defaults to 300 float values on osc address /wek/inputs
* Intended for use with Wekinator for Machine Learning / Image recognition

|Control | Action|
|-|-|
| WASD | Move forward, left strafe, backward, right strafe |
| c | Create virtual camera (output appears on the HUD / Unity Canvas) |
| z | Cycle current virtual camera left through created virtual cameras |
| x | Cycle current virtual camera right through created virutal cameras |
| < | Destroy current virtual camera (and cycle left) |

OSC Values are sent automatically once a virtual camera has been created
* 3 float values per pixel
* Ordering: red, green, blue
* Range: float values between 0 and 1

Intended for use with Wekinator: http://www.wekinator.org/
See: http://opensoundcontrol.org/introduction-osc

## Getting Started

1. Download UnityHub
2. Install an appropriate version of Unity (time of writing used LTS 2018.4.20f1)
3. Clone and add this project folder in Unity Hub
4. Open
5. Navigate to Scenes in the project view
6. Drag in SampleScene
7. Delete the default scene
8. Edit the scene, recommend animating objects in front of cameras etc

### Use a different OSC host, port, or address:
In Unity navigate to TripodController and edit TripodToOSC values in the inspector

Uses Wekinator defaults: localhost:6448 /wek/inputs

Currently the program does nothing on recieved OSC events (/wek/outputs), but easy to modify

### Modifying the virtual camera resolution 
It's a hack, but ensure the MAX\_BUFFER\_SIZE can accomodate your desired resolution
The default buffer size was 1000, this was causing out of index issues
In the interest of time I simply bumped it up 10x from the default 1000 value provided in the wekinator example code 
Your mileage may vary
* Consider modifying TripodToOSC::OnTripodViewUpdate to send monochrome values if color is not needed
* Be wary of making the resolution of the virtual camera too large as the conversion to RAM is slow and may lock up if too large!

### Ideas
* Process the OSC messages with Wekinator
  * Use classification or regression algorithms on the inputs to detect virtual objects
  * Play different music based on what is in front of the virtual camera
  * Create a virtual zoo of simple scripted "animal" objects that walk in front of cameras and make sounds at it
* Edit the code to make a virtual osc camera that follows (is parented to) the player
* Cameras are currently stationary, consider giving them a rigid body/physics and animating them

### Prerequisites

Unity, (Developed on 2018.4)

## Built With

*  Visual Studio, C#

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Alexander Barnes** - *Initial work* - [ToastedBits](http://toastedbits.com/)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* [Wekinator \| Software for real-time, interactive machine learning](http://www.wekinator.org/) by Rebecca Fiebrink
* [Machine Learning for Musicians and Artists - an Online Machine Art Course at Kadenze](https://www.kadenze.com/courses/machine-learning-for-musicians-and-artists-v)
* Special thanks to the Wekinator Example Osc and UDPPacketIO
