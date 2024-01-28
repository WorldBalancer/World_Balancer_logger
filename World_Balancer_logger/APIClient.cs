using Newtonsoft.Json;
using System.Net;
using VRC_Mod_Tool.Wrappers;
using static VRC_Mod_Tool.Boot;

namespace VRC_Mod_Tool
{
    public class UserResponse
    {
        public List<User> Data { get; set; }
    }

    public class User
    {
        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }

        [JsonProperty("UserId")]
        public string UserId { get; set; }
    }
    internal class APIClient
    {
        public static string[] BotUsers;
        public static string[] CrasherUsers;
        public static string[] BannedUsers;
        public static string[] WatchingUsers;
        public static string[] Bannedkids;
        public static string[] Blacklist;
        public static string apiUserId;

        public static Task FetchLists()
        {
#pragma warning disable CS1058 // A previous catch clause already catches all exceptions
            try
            {
                // Replace the API response with reading from the test JSON file
                string bannedUserData = File.ReadAllText("json/banned.json");
                UserResponse bannedapiResponse = JsonConvert.DeserializeObject<UserResponse>(bannedUserData);

                if (bannedapiResponse != null && bannedapiResponse.Data != null)
                {

                    // Populate the array
                    BannedUsers = bannedapiResponse.Data.Select(user => user.DisplayName).ToArray();

                    // Log the connection for each user
                    foreach (var user in bannedapiResponse.Data)
                    {
                        string apiDisplayName = user.DisplayName;
                        //apiUserId = user.UserId; // Assign the value to apiUserId
                    }

                        // Log the total number of Banned Users
                        Logger.ListLog("Getting API Data");
                        Logger.ListLog($"{BannedUsers.Length} Banned Users");
                }

                // Replace the API response with reading from the test JSON file
                string BotUserData = File.ReadAllText("json/bot.json");
                UserResponse botapiResponse = JsonConvert.DeserializeObject<UserResponse>(BotUserData);

                if (botapiResponse != null && botapiResponse.Data != null)
                {

                    // Populate the array
                    BotUsers = botapiResponse.Data.Select(user => user.DisplayName).ToArray();

                    // Log the connection for each user
                    foreach (var user in botapiResponse.Data)
                    {
                        string apiDisplayName = user.DisplayName;
                        //apiUserId = user.UserId; // Assign the value to apiUserId
                        //Logger.LogDebug($"Bot Users {apiDisplayName} connected");
                    }

                    // Log the total number of Bot Users
                    Logger.ListLog($"{BotUsers.Length} Bot Users");
                }

                // Replace the API response with reading from the test JSON file
                string WatchingUserData = File.ReadAllText("json/Watching.json");
                UserResponse WatchingapiResponse = JsonConvert.DeserializeObject<UserResponse>(WatchingUserData);

                if (WatchingapiResponse != null && WatchingapiResponse.Data != null)
                {

                    // Populate the array
                    WatchingUsers = WatchingapiResponse.Data.Select(user => user.DisplayName).ToArray();

                    // Log the connection for each user
                    foreach (var user in WatchingapiResponse.Data)
                    {
                        string apiDisplayName = user.DisplayName;
                        //apiUserId = user.UserId; // Assign the value to apiUserId
                    }

                    // Log the total number of WatchingUsers Users
                    Logger.ListLog($"{WatchingUsers.Length} Watching Users");
                }

                // Replace the API response with reading from the test JSON file
                string CrasherUserData = File.ReadAllText("json/Crasher.json");
                UserResponse CrasherapiResponse = JsonConvert.DeserializeObject<UserResponse>(CrasherUserData);

                if (CrasherapiResponse != null && CrasherapiResponse.Data != null)
                {

                    // Populate the array
                    CrasherUsers = CrasherapiResponse.Data.Select(user => user.DisplayName).ToArray();

                    // Log the connection for each user
                    foreach (var user in CrasherapiResponse.Data)
                    {
                        string apiDisplayName = user.DisplayName;
                        apiUserId = user.UserId; // Assign the value to apiUserId
                    }

                    // Log the total number of CrasherUsers Users
                    Logger.ListLog($"{CrasherUsers.Length} Crasher Users");
                }

                // Replace the API response with reading from the test JSON file
                string BannedKidsUserData = File.ReadAllText("json/Banned_kids.json");
                UserResponse BannedKidsapiResponse = JsonConvert.DeserializeObject<UserResponse>(BannedKidsUserData);

                if (BannedKidsapiResponse != null && BannedKidsapiResponse.Data != null)
                {

                    // Populate the array
                    Bannedkids = BannedKidsapiResponse.Data.Select(user => user.DisplayName).ToArray();

                    // Log the connection for each user
                    foreach (var user in BannedKidsapiResponse.Data)
                    {
                        string apiDisplayName = user.DisplayName;
                        //apiUserId = user.UserId; // Assign the value to apiUserId
                    }

                    // Log the total number of Bannedkids Users
                    Logger.ListLog($"{Bannedkids.Length} Banned_Kids Users");
                }

                // Replace the API response with reading from the test JSON file
                string BlacklistUserData = File.ReadAllText("json/Blacklist.json");
                UserResponse BlacklistapiResponse = JsonConvert.DeserializeObject<UserResponse>(BlacklistUserData);

                if (BlacklistapiResponse != null && BlacklistapiResponse.Data != null)
                {

                    // Populate the array
                    Blacklist = BlacklistapiResponse.Data.Select(user => user.DisplayName).ToArray();

                    // Log the connection for each user
                    foreach (var user in BlacklistapiResponse.Data)
                    {
                        string apiDisplayName = user.DisplayName;
                        //apiUserId = user.UserId; // Assign the value to apiUserId
                    }

                    // Log the total number of Blacklist Users
                    Logger.ListLog($"{Blacklist.Length} Blacklist Users");
                }
            }

            catch (Exception ex)
            {
                Logger.LogError($"An error occurred: {ex}");
            }
            catch
            {
                Logger.LogError("Failed to create list from api data, this can happen if the lists are outdated, reboot the software.");
            }

            return Task.CompletedTask;
        }
    }
}
