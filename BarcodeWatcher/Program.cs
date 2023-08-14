using BarcodeWatcher.Helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Globalization;
using ZXing;

class Program
{
    static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<FileWatcherService>();
            })
            .UseWindowsService() // Use Windows service instead of console lifetime
            .ConfigureLogging((hostContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddEventLog(settings =>
                {
                    settings.SourceName = "FolderWatcher";
                    settings.LogName = "Application";
                });
            });
}

public class FileWatcherService : BackgroundService
{
    private readonly string _directoryPath;
    private readonly ILogger<FileWatcherService> _logger;
    private FileSystemWatcher _fileWatcher;
    private bool isQueued = false;
    private Queue<string> Queue = new Queue<string>();

    public FileWatcherService(ILogger<FileWatcherService> logger)
    {
       
        _logger = logger;
        // Set the directory path to monitor
        _directoryPath = "C:\\BatchesPro";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("FileWatcherService is starting.");

        _fileWatcher = new FileSystemWatcher(_directoryPath);
        _fileWatcher.IncludeSubdirectories = true;
        _fileWatcher.Created += OnFileCreated;
        _fileWatcher.EnableRaisingEvents = true;

        while (!stoppingToken.IsCancellationRequested)
        {
            // Perform background tasks here (if any)
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }

        _logger.LogInformation("FileWatcherService is stopping.");
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        Queue.Enqueue(e.FullPath);

        // Start the upload process if it's not already in progress
        if (!isQueued)
        {
            ProcessUploadQueue();
        }


    }

    private async void ProcessUploadQueue()
    {
        // Check if upload is already in progress
        if (isQueued)
        {
            return;
        }

        // Set uploading flag to true
        isQueued = true;

        while (Queue.Count > 0)
        {
            string filePath = Queue.Dequeue();



            // Upload the file
            bool uploadStatus = await CreateJson(filePath);

            // Perform actions based on upload status
            if (uploadStatus)
            {

            }
            else
            {

            }
        }

        // Set uploading flag to false
        isQueued = false;
    }
    private async Task<bool> CreateJson(string sourceFilePath)
    {
        if (!File.Exists(sourceFilePath))
        {
            return false; // File not found
        }

        // Check if the file has a .json extension
        if (Path.GetExtension(sourceFilePath).Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            return false; // Skip processing .json files
        }

        // Rest of the code to create the JSON file
        string folderName = Path.GetDirectoryName(sourceFilePath);
        string jsonFilePath = Path.Combine(folderName, $"{folderName}.json");

        int maxRetries = 3;
        int retryDelay = 1000; // 1 second delay between retries

        for (int retry = 1; retry <= maxRetries; retry++)
        {
            try
            {
                // Detect the text in zones and get the barcode text
                Result barcodeResult = BarcodeHelper.DecodeBarcode(sourceFilePath);
                if (barcodeResult != null)
                {
                    // Read the existing JSON content from the file
                    string existingJson = "";
                    if (File.Exists(jsonFilePath))
                    {
                        using (StreamReader reader = File.OpenText(jsonFilePath))
                        {
                            existingJson = await reader.ReadToEndAsync();
                        }
                    }

                    // Deserialize the existing JSON content to a list of barcodes
                    List<BarcodeInfo> barcodeList = JsonConvert.DeserializeObject<List<BarcodeInfo>>(existingJson) ?? new List<BarcodeInfo>();

                    // Add the new barcode to the list
                    BarcodeInfo barcodeInfo = new BarcodeInfo
                    {
                        FileName = Path.GetFileName(sourceFilePath),
                        Barcode = barcodeResult.Text
                    };
                    barcodeList.Add(barcodeInfo);

                    // Serialize the updated barcode list to JSON
                    string updatedJson = JsonConvert.SerializeObject(barcodeList, Formatting.Indented);

                    // Write the JSON string to the file
                    File.WriteAllText(jsonFilePath, updatedJson);

                    // JSON file updated successfully
                    return true;
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Parameter is not valid for image: {sourceFilePath}");
                _logger.LogWarning($"Parameter is not valid for image: {sourceFilePath}");

                if (retry < maxRetries)
                {
                    // Delay before retrying
                    await Task.Delay(retryDelay);
                }
                else
                {
                    _logger.LogError($"Error creating JSON file for {sourceFilePath}: {ex.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _logger.LogError($"Error creating JSON file for {sourceFilePath}: {ex.Message}");
                return false;
            }
        }

        return false;
    }
    public class BarcodeInfo
    {
        public string FileName { get; set; }
        public string Barcode { get; set; }
    }
}
