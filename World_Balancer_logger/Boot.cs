using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using VRC_Mod_Tool.Wrappers;
using Newtonsoft.Json;
using System.Media;
using System.Text.Json;

namespace VRC_Mod_Tool
{
    internal class Boot
    {

        public static async Task Main(string[] args)
        {
            Console.Title = "Moderation_System_logger";

            AppConfig config = ConfigManager.LoadConfig("./config.json");

            // Check if loading the configuration was successful
            if (config != null)
            {
                // Now you can use the loaded configuration in your application
                //Console.WriteLine($"EnableBeep is set to: {config.EnableBeep}");
                //Console.WriteLine($"Volume level is set to: {config.Volume}");
                //Console.WriteLine($"RoleIdToPing is set to: {config.RoleIdToPing}");
                //Console.WriteLine($"DiscordWebhookUrl is set to: {config.DiscordWebhookUrl}");
            }
            else
            {
                // Handle the case where loading the configuration failed
                Console.WriteLine("Failed to load configuration. Exiting...");
            }

            {
                // Check if the game process is still running
                Process[] VRCProcs = Process.GetProcessesByName("VRChat");
                if (VRCProcs != null && VRCProcs.Length > 0)
                {

                    // Initialize the timer
                    await InitializeTimer();

                    // Call ScanLog only if the game is running
                    ScanLog();
                }
                else
                {
                    // Print a message if the game is not running
                    Logger.Log("VRChat process is not running. Exit the Application");
                    return; // Exit the application
                }
            }
        }

        // Create a timer to call FetchLists every 5 minutes
        private static Timer fetchListsTimer;

        // Initial setup for the timer
        public static async Task InitializeTimer()
        {
            try
            {
                // Set the interval to 20 minutes (adjust as needed)
                int interval = 20 * 60 * 1000;

                // Log a message indicating that the timer is being initialized
                Logger.LogDebug($"Setting up timer with an interval of {interval / 1000} seconds");

                // Create the timer and start it
                fetchListsTimer = new Timer(async _ =>
                {
                    // Notification before fetching data again
                    Logger.LogWarning("Fetching data again in 20 Minutes...");
                    await APIClient.FetchLists();
                }, null, 0, interval);
            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occurred during timer initialization: {ex}");
            }
        }

        private static string SanitizeUsername(string username)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string pattern = @"^[^\p{C}]+$";
            bool isValid = System.Text.RegularExpressions.Regex.IsMatch(username, pattern);
            //Logger.LogDebug($"{isValid}");

            // Display validation result
            if (isValid)
            {
                //Logger.LogSuccess("The username is valid.");
            }
            else
            {
                //Logger.LogError("The username is invalid.");
            }

            // Print each character along with its Unicode code point
            //foreach (char c in username)
            //{
            //    Logger.LogDebug($"Character: {c}, Unicode: {(int)c}");
            //}

            // Return the original or sanitized username
            // For example, if you want to remove invalid characters, you can modify this part accordingly
            return username;
        }

        private static void ScanLog()
        {
            var directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\VRChat\VRChat");
            if (directory != null && directory.Exists)
            {
                FileInfo target = null;
                foreach (var info in directory.GetFiles("output_log_*.txt", SearchOption.TopDirectoryOnly))
                {
                    if (target == null || info.LastWriteTime.CompareTo(target.LastWriteTime) >= 0) target = info;
                }

                if (target != null)
                {
                    Process[] VRCProcs = Process.GetProcessesByName("VRChat");
                    if (VRCProcs != null && VRCProcs.Length > 0)
                    {
                        Logger.Log($"Watching VRChat Process [{target.Name}]");
                        Logger.ListLog("Getting API Data");
                        Process VRChat = VRCProcs[0];

                        ReadNewLines(target.FullName);

                        while (!VRChat.HasExited)
                        {
                            ReadLog(target.FullName);
                            Thread.Sleep(1);
                        }
                    }
                }
            }
        }

    private static void ReadLog(string path)
    {
        // Load configuration
        AppConfig config = ConfigManager.LoadConfig("./config.json");
        if (config == null)
        {
            // Handle configuration loading failure
            return;
        }

        var lines = ReadNewLines(path); // Use 'path' instead of 'Path'

            // Initialize name variable
            string name = "";

            string votename = "";

            string udonexception = "";

            string videoplayback = "";

            string udonvideoplayback = "";

            foreach (var line in lines)
            {
                if (line.Contains("User Authenticated: "))
                {
                    //Logger.Log($"Processing line: {line}");
                    string[] parts = line.Split(new[] { "User Authenticated: " }, StringSplitOptions.None);
                    string welcomeuser = parts[1];

                    Logger.LogDebug($"Welcome to {welcomeuser} Moderation logger");
                }
                if (line.Contains("Joining or Creating Room: "))
                {
                    //Logger.Log($"Processing line: {line}"); // Add this line for debug
                    string[] parts = line.Split(new[] { "Joining or Creating Room: " }, StringSplitOptions.None);

                    if (parts.Length > 1)
                    {
                        name = parts[1].Trim(); // Trim any leading or trailing whitespaces
                        Logger.LogWorld($"You have joining {name} World");

                        // Send Discord webhook for Users have joining
                        Task.Run(() => oldSendDiscordWebhook($"Users have joining {name} World", config)).Wait();
                    }
                    else
                    {
                        Logger.Log("Warning: Unable to extract room name from the line.");
                    }

                    // Continue to the next iteration of the loop after processing the room name
                    continue;
                }

                if (line.Contains("[Behaviour] Received executive message: You have been kicked from the instance"))
                {
                    //Logger.Log($"Processing line: {line}"); // Add this line for debug
                    string[] parts = line.Split(new[] { "[Behaviour] Received executive message: You have been kicked from the instance" }, StringSplitOptions.None);

                    if (parts.Length > 1)
                    {
                        Logger.LogImportantVRChat($"[Kicked] You have been kicked from the instance");

                        Task.Run(() => oldSendDiscordWebhook($"[kicked] You have been kicked from the instance", config)).Wait();
                    }
                    else
                    {
                        Logger.Log("Warning: Unable to extract Received executive message from the line.");
                    }

                    // Continue to the next iteration of the loop after processing the Received executive message
                    continue;
                }

                if (line.StartsWith(" ---> VRC.Udon.VM.UdonVMException: "))
                {
                    //Logger.Log($"Processing UdonVMException: {udonexception}");

                    // Additional logging to check the value of udonexception
                    string[] parts = line.Split(new[] { " ---> VRC.Udon.VM.UdonVMException: " }, StringSplitOptions.None);

                    if (parts.Length > 1)
                    {
                        udonexception = parts[1]; // Corrected assignment
                        Logger.LogImportant("[UdonVMException]: " + udonexception);

                        // Continue with the rest of the code
                        Task.Run(() => oldSendDiscordWebhook($"[UdonVMException]: {udonexception}", config)).Wait();
                    }
                    else
                    {
                        Logger.Log("Warning: Unable to extract UdonVMException details from the line.");
                    }

                    // Continue to the next iteration of the loop after processing the UdonVMException details
                    continue;
                }

                if (line.Contains("[Video Playback] "))
                {
                    //Logger.Log($"Processing line: {line}"); // Add this line for debug
                    string[] parts = line.Split(new[] { "[Video Playback] " }, StringSplitOptions.None);

                    if (parts.Length > 1)
                    {
                        videoplayback = parts[1].Trim(); // Trim any leading or trailing whitespaces
                        Logger.LogImportant($"[Video Playback] {videoplayback}");

                        // Send Discord webhook for Users have joining
                        Task.Run(() => oldSendDiscordWebhook($"[Video Playback] {videoplayback}", config)).Wait();
                    }
                    else
                    {
                        Logger.Log("Warning: Unable to extract Received executive message from the line.");
                    }

                    // Continue to the next iteration of the loop after processing the Received executive message
                    continue;
                }

                if (line.Contains("[USharpVideo] "))
                {
                    //Logger.Log($"Processing line: {line}"); // Add this line for debug
                    string[] parts = line.Split(new[] { "[USharpVideo] " }, StringSplitOptions.None);

                    if (parts.Length > 1)
                    {
                        udonvideoplayback = parts[1].Trim(); // Trim any leading or trailing whitespaces
                        Logger.LogImportant($"[USharpVideo] {udonvideoplayback}");

                        // Send Discord webhook for Users have joining
                        Task.Run(() => oldSendDiscordWebhook($"[USharpVideo] {udonvideoplayback}", config)).Wait();
                    }
                    else
                    {
                        Logger.Log("Warning: Unable to extract Received executive message from the line.");
                    }

                    // Continue to the next iteration of the loop after processing the Received executive message
                    continue;
                }

                if (line.Contains("[ModerationManager] "))
                {
                    //Logger.Log($"Processing line: {line}"); // Add this line for debug
                    string[] parts = line.Split(new[] { "[ModerationManager] " }, StringSplitOptions.None);

                    if (parts.Length > 1)
                    {
                        votename = parts[1].Trim(); // Trim any leading or trailing whitespaces

                        string sanitizedUsername = SanitizeUsername(votename);
                        Logger.LogImportantVRChat($"[ModerationManager] {votename}");

                        // Send Discord webhook for vote
                        Task.Run(() => SendDiscordWebhook("ModerationManager", $"{votename}", config, "16711680")).Wait();
                    }
                    else
                    {
                        Logger.Log("Warning: Unable to extract room name from the line.");
                    }

                    // Continue to the next iteration of the loop after processing the room name
                    continue;
                }

                if (line.Contains("[Behaviour] OnPlayerJoined "))
                {
                    //Logger.Log($"Processing line: {line}"); // Add this line for debug
                    string[] parts = line.Split(new[] { "[Behaviour] OnPlayerJoined " }, StringSplitOptions.None);
                    string DisplayName = parts[1];

                    string sanitizedUsername = SanitizeUsername(DisplayName);
                    Logger.LogImportant($"User {DisplayName} connected");

                    // Use apiDisplayName here
                    string apiDisplayName = SanitizeUsername(DisplayName);

                    // Use Task.Run to asynchronously call the CheckAndSendWebhook method
                    Task.Run(() => CheckAndSendWebhook(apiDisplayName, config)).Wait();

                    if (APIClient.BotUsers.Contains(apiDisplayName))
                    {
                        Logger.botLog($"Bot User {apiDisplayName} connected");
                    }
                    // Send Discord webhook for users user
                    Task.Run(() => SendDiscordWebhook("User Connection", $"User {apiDisplayName} connected", config, "65280")).Wait();
                }

                if (line.Contains("[Behaviour] OnPlayerLeft "))
                {
                    //Logger.Log($"Processing line: {line}");
                    string[] parts = line.Split(new[] { "[Behaviour] OnPlayerLeft " }, StringSplitOptions.None);
                    string DisplayName = parts[1];

                    string sanitizedUsername = SanitizeUsername(DisplayName);
                    Logger.LogImportant($"User {DisplayName} disconnected");

                    // Send Discord webhook for User disconnected
                    Task.Run(() => oldSendDiscordWebhook($"User {DisplayName} disconnected", config)).Wait();

                }
            }
        }

        private static async Task CheckAndSendWebhook(string apiDisplayName, AppConfig config)
        {
            try
            {

                if (APIClient.BannedUsers.Contains(apiDisplayName))
                {
                    Logger.BannedLog($"Banned User {apiDisplayName} connected");

                    // Play beep sound if enabled in the configuration
                    if (config.EnableBeep == true)
                    {
                        using (SoundPlayer player = new SoundPlayer("beep-01a.wav"))
                        //beep-01a.wav
                        {
                            AdjustVolume(config.Volume); // Adjust the volume

                            // Play the sound
                            player.PlaySync();
                        }
                    }

                    // Send Discord webhook for banned user
                    await SendDiscordWebhookWithRolePing("User Connection", $"Banned User {apiDisplayName} connected", config, "16711680"); // red
                }

                if (APIClient.CrasherUsers.Contains(apiDisplayName))
                {
                    Logger.CrasherLog($"Crasher User {apiDisplayName} connected");

                    // Play beep sound if enabled in the configuration
                    if (config.EnableBeep == true)
                    {
                        using (SoundPlayer player = new SoundPlayer("beep-01a.wav"))
                        {
                            AdjustVolume(config.Volume); // Adjust the volume

                            // Play the sound
                            player.PlaySync();
                        }
                    }

                    // Send Discord webhook for crasher user
                    await SendDiscordWebhookWithRolePing("User Connection", $"Crasher User {apiDisplayName} connected", config, "FFFF00"); // Yellow
                }

                if (APIClient.WatchingUsers.Contains(apiDisplayName))
                {
                    Logger.CrasherLog($"Watching User {apiDisplayName} connected");

                    // Play beep sound if enabled in the configuration
                    if (config.EnableBeep == true)
                    {
                        using (SoundPlayer player = new SoundPlayer("beep-01a.wav"))
                        {
                            AdjustVolume(config.Volume); // Adjust the volume

                            // Play the sound
                            player.PlaySync();
                        }
                    }

                    // Send Discord webhook for watching user
                    await SendDiscordWebhook("User Connection", $"Watching User {apiDisplayName} connected", config, "0000FF"); // Blue

                }

                if (APIClient.Bannedkids.Contains(apiDisplayName))
                {
                    Logger.BannedLog($"Banned_Kids User {apiDisplayName} connected");

                    // Play beep sound if enabled in the configuration
                    if (config.EnableBeep == true)
                    {
                        using (SoundPlayer player = new SoundPlayer("beep-01a.wav"))
                        //beep-01a.wav
                        {
                            AdjustVolume(config.Volume); // Adjust the volume

                            // Play the sound
                            player.PlaySync();
                        }
                    }

                    // Send Discord webhook for Banned_Kids user
                    await SendDiscordWebhookWithRolePing("User Connection", $"Banned_Kids User {apiDisplayName} connected", config, "16711680"); // red
                }

                if (APIClient.Blacklist.Contains(apiDisplayName))
                {
                    Logger.BannedLog($"Blacklist User {apiDisplayName} connected");

                    // Play beep sound if enabled in the configuration
                    if (config.EnableBeep == true)
                    {
                        using (SoundPlayer player = new SoundPlayer("beep-01a.wav"))
                        //beep-01a.wav
                        {
                            AdjustVolume(config.Volume); // Adjust the volume

                            // Play the sound
                            player.PlaySync();
                        }
                    }

                    // Send Discord webhook for Blacklist user
                    await SendDiscordWebhookWithRolePing("User Connection", $"Blacklist User {apiDisplayName} connected", config, "16711680"); // red
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in CheckAndSendDiscordWebhook: {ex.Message}");
            }
        }

        // Method to send Discord webhook
        private static async Task SendDiscordWebhook(string title1, string message, AppConfig config, string color = "")
        {
            try
            {
                string webhookUrl = config.DiscordWebhookUrl; // Use the webhook URL from the config

                using (HttpClient client = new HttpClient())
                {
                    var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

                    var embed = new
                    {
                        title = title1,
                        description = message,
                        color = color,
                        timestamp = timestamp
                    };

                    var payload = new
                    {
                        embeds = new[] { embed }
                    };

                    var json = JsonConvert.SerializeObject(payload);


                    //Logger.Log($"Sending Discord webhook. Payload: {json}"); // Add this line for debugging

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    //var response = await client.PostAsync(webhookUrl, content);
                    //var responseContent = await response.Content.ReadAsStringAsync();
                    //Logger.Log($"Discord webhook response: {responseContent}");

                    await client.PostAsync(webhookUrl, content);

                    //Logger.Log("Discord webhook sent successfully."); // Add this line for debugging
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error sending Discord webhook: {ex.Message}");
            }
        }
        private static async Task SendDiscordWebhookWithRolePing(string title1, string message, AppConfig config, string color = "")
        {
            try
            {
                string webhookUrl = config.DiscordWebhookUrl; // Use the webhook URL from the config
                string roleIdToPing = config.RoleIdToPing; // Use the role ID from the config

                using (HttpClient client = new HttpClient())
                {
                    var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

                    var embed = new
                    {
                        title = title1,
                        description = message,
                        color = color,
                        timestamp = timestamp
                    };

                    var payload = new
                    {
                        content = $"<@&{roleIdToPing}>", // Mention the role outside of the embed
                        embeds = new[] { embed }
                    };

                    var json = JsonConvert.SerializeObject(payload);

                    //Logger.Log($"Sending Discord webhook with role ping. Payload: {json}");

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    await client.PostAsync(webhookUrl, content);

                    //Logger.Log("Discord webhook sent successfully with role ping.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error sending Discord webhook: {ex.Message}");
            }
        }

        private static async Task oldSendDiscordWebhook(string message, AppConfig config)
        {
            string webhookUrl = config.DiscordWebhookUrl;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var payload = new
                    {
                        content = message
                    };

                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(webhookUrl, content);

                    //Logger.Log($"Sending Discord webhook. Payload: {json}"); // Add this line for debugging

                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error sending Discord webhook: {ex.Message}");
                }
            }
        }

        private static void AdjustVolume(int volume)
        {
            try
            {
                var player = new SoundPlayer();
                player.Play(); // This will play the default system beep sound

            }
            catch (Exception ex)
            {
                Logger.LogError($"Error adjusting volume: {ex.Message}");
            }
        }

        private static readonly object readLock = new object();
        private static readonly StringBuilder stringBuilder = new StringBuilder();

        private static List<string> ReadNewLines(string filePath)
        {
            List<string> lines = new List<string>();

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream))
                {
                    lock (readLock)
                    {
                        reader.BaseStream.Seek(LastReadOffset, SeekOrigin.Begin);

                        int bufferSize = 8192; // Adjust the buffer size as needed
                        byte[] buffer = new byte[bufferSize];
                        int bytesRead;

                        while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            stringBuilder.Append(chunk);

                            int index;
                            while ((index = stringBuilder.ToString().IndexOf('\n')) >= 0)
                            {
                                lines.Add(stringBuilder.ToString(0, index));
                                stringBuilder.Remove(0, index + 1);
                            }
                        }

                        LastReadOffset = reader.BaseStream.Position;
                    }
                }
            }
            catch (IOException ex)
            {
                Logger.LogError(ex);
            }

            return lines;
        }

        private static long LastReadOffset = 0;

        public class ConfigManager
        {
            public static AppConfig LoadConfig(string configFilePath)
            {
                try
                {
                    if (System.IO.File.Exists(configFilePath))
                    {
                        string json = System.IO.File.ReadAllText(configFilePath);
                        return JsonConvert.DeserializeObject<AppConfig>(json);
                    }
                    else
                    {
                        // Default configuration if the file doesn't exist
                        AppConfig defaultConfig = new AppConfig
                        {
                            EnableBeep = true,
                            Volume = 40, // Set a default volume level (adjust as needed)
                            RoleIdToPing = "00000000000000"
                        };

                        // Prompt the user to set the EnableBeep value
                        Console.WriteLine("Do you want to enable beep sound? (yes/no):");
                        string userInput = Console.ReadLine().ToLower();

                        if (userInput == "yes")
                        {
                            defaultConfig.EnableBeep = true;
                        }
                        else
                        {
                            defaultConfig.EnableBeep = false;
                        }

                        Console.WriteLine("Enter the volume level (0-100):");
                        int volumeInput;
                        if (int.TryParse(Console.ReadLine(), out volumeInput) && volumeInput >= 0 && volumeInput <= 100)
                        {
                            defaultConfig.Volume = volumeInput;
                        }
                        else
                        {
                            Console.WriteLine("Invalid volume level. Using the default volume.");
                        }

                        // Prompt the user for the Discord webhook URL
                        Console.WriteLine("Enter the Discord webhook URL:");
                        string webhookUrl = Console.ReadLine();

                        if (string.IsNullOrEmpty(webhookUrl))
                        {
                            // Use a default webhook URL if the user doesn't provide one
                            defaultConfig.DiscordWebhookUrl = "";
                        }
                        else
                        {
                            defaultConfig.DiscordWebhookUrl = webhookUrl;
                        }

                        Console.WriteLine("Enter the Role ID to ping:");
                        defaultConfig.RoleIdToPing = Console.ReadLine();

                        SaveConfig(configFilePath, defaultConfig);

                        Console.WriteLine("Configuration file created. Please edit the file to customize other settings.");
                        Console.WriteLine($"Config file path: {configFilePath}");

                        Console.Clear();

                        return defaultConfig;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading configuration: {ex.Message}");
                    return null;
                }
            }

            public static void SaveConfig(string configFilePath, AppConfig config)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                    System.IO.File.WriteAllText(configFilePath, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving configuration: {ex.Message}");
                }
            }
        }

        public class AppConfig
        {
            public bool EnableBeep { get; set; }
            public int Volume { get; set; }
            public string RoleIdToPing { get; set; }
            public string DiscordWebhookUrl { get; set; }
        }
    }
}
