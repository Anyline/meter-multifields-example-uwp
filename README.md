# meter-multifields-example-uwp
How to use the Multifields Meter scanner - UWP Example App 

## File summary

* `UWP_MultiFieldsMeter` - Example app source code
* `Anyline.dll` - Precompiled Anyline Core
* `Anyline.winmd` - Metadata for the Anyline Core
* `AnylineSDK.dll` - Precompiled library for the Anyline SDK
* `README.md` - This readme.


## API Reference

The API reference for the Anyline SDK for Windows UWP can be found here: https://documentation.anyline.com/api/windows/index.html


## Requirements

- Windows 10 + Visual Studio 2015 with the Windows 10 SDK
- An Windows 10 x86 device
- An integrated or external camera/webcam (recommended: 720p and focus capability)


## Quick Start

### 0. Clone or Download

Clone or download this repository. 
Open the `UWP_MultiFieldsMeter.sln` file on Visual Studio and run the project on a x86 device.

Click `Scan` > Scan the Barcode and the start scanning the meter.

Once a scan process starts, it runs continuously collecting results. It will stop and return a result once one of the following conditions are met:

1) All changing meter values were read (one full circle with the first counter number appearing again)
2) The scanning process has been running for 20 seconds without returning a result
3) The user presses a “Stop scan” button


### 1. Integrate Anyline in your UWP App

Add `AnylineSDK.dll` and `Anyline.winmd` as reference to the project.
Make sure that `Anyline.dll` is in the same directory as the winmd file.

### 2. Configure the Project

Make sure to set The capabilities "Webcam", "Microphone" and "Internet (Client)" in Package.appxmanifest. Set the project build configuration to "x86". x64 and ARM are not supported.

### 3. License & Package Name

Make sure the Package Name in Package.appxmanifest matches the license.

To generate a license key for windows, you'll need the Package Name located under `Packaging` in the `Package.appxmanifest` file of your example project.

To claim a trial license, go to: [Anyline SDK Register Form]( https://anyline.com/free-demos/ ) or get in contact with our sales representative.



