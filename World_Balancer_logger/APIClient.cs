using Newtonsoft.Json;
using System.Net;
using System.Text;
using VRC_Mod_Tool.Wrappers;
using static VRC_Mod_Tool.Boot;

namespace VRC_Mod_Tool
{
    public class UserResponse
    {
        public WatchingUserData[] watching { get; set; }
        public BannedUserData[] banned { get; set; }
        public BotUserData[] bot { get; set; }
        public CrasherUserData[] crasher { get; set; }
        public BannedKidsUserData[] bannedkids { get; set; }
        public BlacklistUserData[] blacklist { get; set; }
    }

    public class WatchingUserData
    {
        public string DisplayName { get; set; }
        public string UserId { get; set; }
    }

    public class BannedUserData
    {
        public string DisplayName { get; set; }
        public string UserId { get; set; }
        public string Reason { get; set; }
    }

    public class BotUserData
    {
        public string DisplayName { get; set; }
        public string UserId { get; set; }
    }

    public class CrasherUserData
    {
        public string DisplayName { get; set; }
        public string UserId { get; set; }
    }

    public class BannedKidsUserData
    {
        public string DisplayName { get; set; }
        public string UserId { get; set; }
        public string Reason { get; set; }
    }

    public class BlacklistUserData
    {
        public string DisplayName { get; set; }
        public string UserId { get; set; }
        public string Reason { get; set; }
    }
    internal class APIClient
    {
        public static string[] BotUsers;
        public static string[] CrasherUsers;
        public static string[] BannedUsers;
        public static string[] WatchingUsers;
        public static string[] Bannedkids;
        public static string[] Blacklist;

        public static Task FetchLists()
        {
            try
            {
                // Replace the API response with reading from the test JSON file
                string bannedUserData = File.ReadAllText("json/banned.json");
                UserResponse bannedapiResponse = JsonConvert.DeserializeObject<UserResponse>(bannedUserData);

                if (bannedapiResponse != null && bannedapiResponse.banned != null)
                {
                    // Log the number of users in the API response
                    //Logger.LogDebug($"[Debug]: Number of Banned Users in API response: {bannedapiResponse.banned.Length}");

                    // Populate the array
                    BannedUsers = bannedapiResponse.banned.Select(user => user.DisplayName).ToArray();

                    // Log the connection for each user
                    foreach (var user in bannedapiResponse.banned)
                    {
                        string apiDisplayName = user.DisplayName;
                    }

                        // Log the total number of Banned Users
                        Logger.ListLog($"{BannedUsers.Length} Banned Users");
                }
                else
                {
                    Logger.LogError("Banned API response or Banned array is null. Check the API response structure.");
                }

                // Replace the API response with reading from the test JSON file
                string BotUserData = File.ReadAllText("json/bot.json");
                UserResponse botapiResponse = JsonConvert.DeserializeObject<UserResponse>(BotUserData);

                if (botapiResponse != null && botapiResponse.bot != null)
                {
                    // Log the number of users in the API response
                    //Logger.LogDebug($"[Debug]: Number of Bot Users in API response: {botapiResponse.bot.Length}");

                    // Populate the array
                    BotUsers = botapiResponse.bot.Select(user => user.DisplayName).ToArray();

                    // Log the connection for each user
                    foreach (var user in botapiResponse.bot)
                    {
                        string apiDisplayName = user.DisplayName;
                    }

                    // Log the total number of Bot Users
                    Logger.ListLog($"{BotUsers.Length} Bot Users");
                }
                else
                {
                    Logger.LogError("Bot API response or bot array is null. Check the API response structure.");
                }

                // Replace the API response with reading from the test JSON file
                string WatchingUserData = File.ReadAllText("json/Watching.json");
                UserResponse WatchingapiResponse = JsonConvert.DeserializeObject<UserResponse>(WatchingUserData);

                if (WatchingapiResponse != null && WatchingapiResponse.watching != null)
                {
                    // Log the number of users in the API response
                    //Logger.LogDebug($"[Debug]: Number of Watching Users in API response: {WatchingapiResponse.watching.Length}");

                    // Populate the array
                    WatchingUsers = WatchingapiResponse.watching.Select(user => user.DisplayName).ToArray();

                    // Log the connection for each user
                    foreach (var user in WatchingapiResponse.watching)
                    {
                        string apiDisplayName = user.DisplayName;
                    }

                    // Log the total number of WatchingUsers Users
                    Logger.ListLog($"{WatchingUsers.Length} Watching Users");
                }
                else
                {
                    Logger.LogError("Watching API response or Watching array is null. Check the API response structure.");
                }

                // Replace the API response with reading from the test JSON file
                string CrasherUserData = File.ReadAllText("json/Crasher.json");
                UserResponse CrasherapiResponse = JsonConvert.DeserializeObject<UserResponse>(CrasherUserData);

                if (CrasherapiResponse != null && CrasherapiResponse.crasher != null)
                {
                    // Log the number of users in the API response
                    //Logger.LogDebug($"[Debug]: Number of Crasher Users in API response: {CrasherapiResponse.crasher.Length}");

                    // Populate the array
                    CrasherUsers = CrasherapiResponse.crasher.Select(user => user.DisplayName).ToArray();

                    // Log the connection for each user
                    foreach (var user in CrasherapiResponse.crasher)
                    {
                        string apiDisplayName = user.DisplayName;
                    }

                    // Log the total number of CrasherUsers Users
                    Logger.ListLog($"{CrasherUsers.Length} Crasher Users");
                }
                else
                {
                    Logger.LogError("Crasher API response or Crasher array is null. Check the API response structure.");
                }

                // Replace the API response with reading from the test JSON file
                string BannedKidsUserData = File.ReadAllText("json/Banned_kids.json");
                UserResponse BannedKidsapiResponse = JsonConvert.DeserializeObject<UserResponse>(BannedKidsUserData);

                if (BannedKidsapiResponse != null && BannedKidsapiResponse.bannedkids != null)
                {
                    // Log the number of users in the API response
                    //Logger.LogDebug($"[Debug]: Number of Banned Kids Users in API response: {BannedKidsapiResponse.bannedkids.Length}");

                    // Populate the array
                    Bannedkids = BannedKidsapiResponse.bannedkids.Select(user => user.DisplayName).ToArray();

                    // Log the connection for each user
                    foreach (var user in BannedKidsapiResponse.bannedkids)
                    {
                        string apiDisplayName = user.DisplayName;
                    }

                    // Log the total number of Bannedkids Users
                    Logger.ListLog($"{Bannedkids.Length} Banned_Kids Users");
                }
                else
                {
                    Logger.LogError("Banned Kids API response or Banned Kids array is null. Check the API response structure.");
                }


                // Replace the API response with reading from the test JSON file
                string BlacklistUserData = File.ReadAllText("json/Blacklist.json");
                UserResponse BlacklistapiResponse = JsonConvert.DeserializeObject<UserResponse>(BlacklistUserData);

                if (BlacklistapiResponse != null && BlacklistapiResponse.blacklist != null)
                {
                    // Log the number of users in the API response
                    //Logger.LogDebug($"[Debug]: Number of Blacklist Users in API response: {BlacklistapiResponse.blacklist.Length}");

                    // Populate the array
                    Blacklist = BlacklistapiResponse.blacklist.Select(user => user.DisplayName).ToArray();

                    // Log the connection for each user
                    foreach (var user in BlacklistapiResponse.blacklist)
                    {
                        string apiDisplayName = user.DisplayName;
                    }

                    // Log the total number of Blacklist Users
                    Logger.ListLog($"{Blacklist.Length} Blacklist Users");
                }
                else
                {
                    Logger.LogError("Blacklist API response or Blacklist array is null. Check the API response structure.");
                }
            }

            catch
            {
                Logger.LogError("Failed to create list from api data, this can happen if the lists are outdated, reboot the software.");
            }

            return Task.CompletedTask;
        }
    }
}
