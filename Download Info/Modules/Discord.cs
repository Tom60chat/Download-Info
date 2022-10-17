using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace discord_rpc
{
    public class Discord
    {
        private static DiscordEventHandlers handlers;
        public static void Initialize(string ApplicationID, DiscordEventHandlers handlers, int autoRegister, string SteamID)
        {
            IntPtr AppID = ConvertString(ApplicationID);
            IntPtr Steam = ConvertString(SteamID);
            if (Environment.Is64BitOperatingSystem)
                RPC_x64.Discord_Initialize(AppID, ref handlers, autoRegister, Steam);
            else
                RPC.Discord_Initialize(AppID, ref handlers, autoRegister, Steam);
            Discord.handlers = handlers;
            FreePointers(AppID, Steam);
        }
        public static Task InitializeAsync(string ApplicationID, DiscordEventHandlers handlers, int autoRegister, string SteamID)
        {
            Initialize(ApplicationID, handlers, autoRegister, SteamID);
            return Task.FromResult(0);
        }
        public static void Shutdown()
        {
            if (Environment.Is64BitOperatingSystem)
                RPC_x64.Discord_Shutdown();
            else
                RPC.Discord_Shutdown();
        }
        public static Task ShutdownAsync()
        {
            Shutdown();
            return Task.FromResult(0);
        }
        public static void RunCallbacks()
        {
            if (Environment.Is64BitOperatingSystem)
                RPC_x64.Discord_RunCallbacks();
            else
                RPC.Discord_RunCallbacks();
        }
        public static Task RunCallbacksAsync()
        {
            RunCallbacks();
            return Task.FromResult(0);
        }
        public static Task UpdatePresenceAsync(RichPresence presence)
        {
            UpdatePresence(presence);
            return Task.FromResult(0);
        }
        public static void UpdatePresence(RichPresence presence)
        {
            DiscordRichPresence rich = new DiscordRichPresence { };

            rich.state = ConvertString(presence.State);
            rich.details = ConvertString(presence.Details);

            rich.startTimestamp = presence.startTimestamp;
            rich.endTimestamp = presence.endTimestamp;

            rich.largeImageKey = ConvertString(presence.largeImageKey);
            rich.largeImageText = ConvertString(presence.largeImageText);
            rich.smallImageKey = ConvertString(presence.smallImageKey);
            rich.smallImageText = ConvertString(presence.smallImageText);

            rich.partySize = presence.partySize;
            rich.partyMax = presence.partyMax;

            rich.matchSecret = ConvertString(presence.matchSecret);
            rich.joinSecret = ConvertString(presence.joinSecret);
            rich.spectateSecret = ConvertString(presence.spectateSecret);
            rich.instance = presence.instance;

            if (Environment.Is64BitOperatingSystem)
                RPC_x64.Discord_UpdatePresence(ref rich);
            else
                RPC.Discord_UpdatePresence(ref rich);

            FreePointers(rich.state, rich.details, rich.largeImageKey, rich.largeImageText, rich.smallImageKey, rich.smallImageText, rich.matchSecret, rich.joinSecret, rich.spectateSecret);
        }
        public static void ClearPresence()
        {
            if (Environment.Is64BitOperatingSystem)
                RPC_x64.Discord_ClearPresence();
            else
                RPC.Discord_ClearPresence();
        }
        public static Task ClearPresenceAsync()
        {
            ClearPresence();
            return Task.FromResult(0);
        }
        public static void Respond(string userid, int reply)
        {
            IntPtr uid = ConvertString(userid);
            if (Environment.Is64BitOperatingSystem)
                RPC_x64.Discord_Respond(uid, reply);
            else
                RPC.Discord_Respond(uid, reply);
            FreePointers(uid);
        }
        public static Task RespondAsync(string userid, int reply)
        {
            Respond(userid, reply);
            return Task.FromResult(0);
        }
        private static IntPtr ConvertString(string str)
        {
            if (str != null)
            {
                byte[] retArray = Encoding.UTF8.GetBytes(str);
                IntPtr retPtr = Marshal.AllocHGlobal(retArray.Length + 1);
                Marshal.Copy(retArray, 0, retPtr, retArray.Length);
                Marshal.WriteByte(retPtr, retArray.Length, 0);
                return retPtr;
            }
            return IntPtr.Zero;
        }
        private static void FreePointers(params IntPtr[] Pointers)
        {
            foreach (IntPtr Pointer in Pointers)
            {
                Marshal.FreeHGlobal(Pointer);
            }
        }
    }
}