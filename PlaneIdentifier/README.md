# Image detection sample

This is a Windows application that uses a pre-trained machine learning model, generated using the [Custom Vision](https://www.customvision.ai/) service on Azure, to detect if the given image contains a specific object: a plane.

The sample comes in two different versions:

- A UWP application
- A WPF application packaged with the [Desktop Bridge](https://docs.microsoft.com/en-us/windows/uwp/porting/desktop-to-uwp-root)

They both leverage the UWP APIs included in the **Windows.AI.MachineLearning** namespace.

## Prerequisites

- [Visual Studio 2017 Version 15.7.4 or Newer](https://developer.microsoft.com/en-us/windows/downloads)
- [Windows 10 - Build 17763 or higher](https://www.microsoft.com/en-us/software-download/windowsinsiderpreviewiso)
- [Windows SDK - Build 17763 or higher](https://www.microsoft.com/en-us/software-download/windowsinsiderpreviewSDK)

## Build the sample

1. If you download the samples ZIP, be sure to unzip the entire archive, not just the folder with the sample you want to build.
2. Start Microsoft Visual Studio 2017 and select **File > Open > Project/Solution**.
3. Starting in the folder where you unzipped the samples, go to the **PlaneIdentifer** subfolder. Double-click the Visual Studio solution file (.sln).
4. Confirm that the project is pointed to the correct SDK that you installed (e.g. 17763). You can do this by right-clicking the project in the **Solution Explorer**, selecting **Properties**, and modifying the **Windows SDK Version**.
5. Confirm that you are set for the right configuration and platform (for example: Debug, x64).
6. Build the solution (**Ctrl+Shift+B**).

## Run the sample

If you want to try the UWP application, right click on **PlaneIdentifier** project in Visual Studio and choose **Set as StartUp Project**. Then press F5 to deploy the application on your machine and launch the debugging experience.

If you want to try the WPF application, instead, right click on the **PlaneIdentifier.Package** project in Visual Studio and choose **Set as StartUp Project**. Then press F5 to deploy and run the packaged version of the application.

**Don't launch** directly the **PlaneIdentifier.Desktop** project. The WinML APIs, in fact, require the application to have an identity in order to work, so the WPF application must be packaged with the Desktop Bridge to consume them.

Regardless of the version of the application you have launched, the user experience is very basic. Just press the **Recognize** button and upload a photo from your computer. The application will tell you if the image indeed contains a plane or not.

## License

MIT. See [LICENSE file](https://github.com/Microsoft/Windows-AppConsult-Samples-UWP/blob/master/LICENSE).