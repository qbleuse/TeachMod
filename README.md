# _TeachMod_

A project of the CobTek Lab intending to test student in their ability to discern what to do and not to do in a clinical interview.

The project will be supported on all following devices:

- Windows
- Android
- IOS
- MacOs

The application consists in an interactive panoramic video (at 360°), where circles or questions appear in front of the user to test they skills and knowledge of a clinical interview.

## Author

This work has been developed has an internship in the CobTek Lab with the help of (in alphabetical order):

<p float="middle">
	<img src="Assets/Logo/CHU_Logo.png" width="100" />
	<img src="Assets/Logo/CobTek_Logo.png" width="100" /> 
	<img src="Assets/Logo/Don_Logo.png" width="170" />
	<img src="Assets/Logo/IA_Logo.png" width="50" />
	<img src="Assets/Logo/ICP_Logo.png" width="55" />
	<img src="Assets/Logo/ISART_Logo.png" width="140" />
	<img src="Assets/Logo/UCA_Logo.png" width="75" />
</p>

Name  | Contribution
:---: | :---:
Quentin Bleuse | Developer
Philippe Robert| Internship Director and Psychiatrist
Maël Addoum| Internship Director
Alexandre Derreumaux| Technician
Rachid Guerchouche| Technician
Paul Galindo| Consultant

Also much thanks to those who acted in the videos:

- Nadia Bilger
- Julie Brousse
- Alexandre Derreumaux
- Valeria Manera
- Magalie Templier
- Marilou Serris

The videos are not available for obvious rights of privacy reasons in this repository, they can be seen through the application available at the following link:

[Not currently Up]()

___

## Table of Contents

- Techs
- Inspiration
- To Build
- To Run
- Features
- User Manual
- Additional Notes
- Licensing

___

## Techs

The project is developed under Unity (2020.3.12f1) for two reasons:
  
- Unity is the easiest software that will allow the display of a panoramic video onto a skybox.
- The 2020.3.12f1 version was the latest version with Long Term Support (LTS) at the time of making the project.

___

## Inspiration

These are the links of the site and such that I took inspiration for certain problems:

- [panoramic video display on skybox](https://learn.unity.com/tutorial/play-360-video-with-a-skybox-in-unity)
- [CrossFade between videos and levels](https://www.youtube.com/watch?v=CE9VOZivb3I&t)
- [camera controls with a gyroscope](https://gist.github.com/kormyen/a1e3c144a30fc26393f14f09989f03e1)

___

## To Build

You should be able to Build if you have the same version of Unity (2020.3) to the platform stated above.

/!\ Videos are not available in this repository so you cannot build out of the blue after cloning the repository, but you can use the system built for the application to create your "own teachMod" (I advise you to look at the User Manual if this is the case)/!\
___

## To Run

The Application should be available at this link: [Not currently Up]()

If you built it yourself, it should be available on a folder if it is on a computer device and as an app if it is on mobile device. You should be able to run it just by opening it.

___

## Features

Here are a non-exhaustive list of features present in the editor and application:

- ### _Display of panoramic video onto a SkyBox_
  
  This is mainly done with the help of Unity. I advise you to read the [User Manual]() to create a Scene that works with the Video Player.

- ### _Gyroscopic Camera for mobile, KeyBind Camera for Computer User_
  
  The two camera switch depending on the platform, it is advised to use the prefabs created for this instance. It uses a lot of Unity in-built inputs and makes them a little bit smoother (such as recalibration for the gyroscope).

- ### _A CSV serializer, enabling user to change save out of Unity_
  
  All save for the interaction of each video are saved in UTF-8 encoded csv file, that should always be saved in the Streaming Assets/ folder. You may use Excel or other spreadsheet application to modify the file even if you are not in Unity. Please refer to the [User Manual]() to know the format of the csv files.

- ### _A CSV Editor, Made to simplify the registering of information_
  
  This one contains a 360° video player to know the timestamp of your videos, and an editor to register the information of your questionnaires and interactive moments of your videos. Please refer to the [User Manual]() to understand how to use it.
  /!\ for the moment only mp4 can be played in the video player of the editor /!\
  
___

## User Manual

The User Manual is accessible in the [UserManual.md](UserManual.md) file.

___

## Additional Notes

- In the package, the post process package is included for reasons that we used to use it, that is not the case anymore. You can uninstall it if it pleases you.
- All raw Files (such as video files and csv files) should be contained in the Streaming Assets file. Otherwise, the file will not be imported during build.  

## License

See the [LICENSE.md](LICENSE.md) file for license rights and limitations (MIT).
