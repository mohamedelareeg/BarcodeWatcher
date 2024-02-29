
Barcode Watcher is a tool designed to automatically detect barcodes from images
and extract the result. It utilizes a folder watcher to monitor a specified
directory for new image files, processes them to extract barcode information,
and stores the results in JSON format.

-------------------------------------------------------------------------------

Features
--------

- Automatically detects barcode images in a specified directory.
- Supports various barcode formats including QR Code, CODE_128, and CODE_39.
- Extracts barcode information and stores it in a JSON file.
- Utilizes ZXing library for barcode decoding.

-------------------------------------------------------------------------------

Usage
-----

To use Barcode Watcher:

1. Clone the repository:

   git clone https://github.com/mohamedelareeg/BarcodeWatcher.git

2. Build the solution using Visual Studio or your preferred IDE.

3. Configure the directory path to monitor by modifying the _directoryPath
   variable in FileWatcherService.cs.

4. Run the application. If using as a Windows service, ensure appropriate
   permissions are set.

5. Place images containing barcodes in the specified directory. The application
   will automatically detect and process them.

6. Extracted barcode information will be stored in JSON files within the same
   directory.

-------------------------------------------------------------------------------

Dependencies
------------

- .NET Core - .NET Core runtime and SDK.
- ZXing.Net - ZXing.Net barcode scanning library.
- Newtonsoft.Json - JSON serialization and deserialization library.

-------------------------------------------------------------------------------

Contributing
------------

Contributions are welcome! If you'd like to contribute to Barcode Watcher, feel
free to open a pull request or submit an issue on the GitHub repository.

-------------------------------------------------------------------------------

License
-------

This project is licensed under the MIT License - see the LICENSE file for
details.

-------------------------------------------------------------------------------

Acknowledgments
---------------

- ZXing.Net - A popular open-source barcode scanning library.
- Microsoft.Extensions.DependencyInjection - Dependency injection library for
  .NET Core.
- Microsoft.Extensions.Hosting - Hosting infrastructure for .NET Core
  applications.
- Newtonsoft.Json - JSON serialization and deserialization library for .NET.

