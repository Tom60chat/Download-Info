using System;
using System.Runtime.InteropServices;

namespace discord_rpc
{
    internal class RPC
    {
        internal const string DiscordDLL = "discord-rpc.dll";

        [DllImport(DiscordDLL, EntryPoint = "Discord_Initialize", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None)]
        internal static extern void Discord_Initialize(IntPtr applicationId, ref DiscordEventHandlers handlers, int autoRegister, IntPtr optionalSteamId);

        [DllImport(DiscordDLL, EntryPoint = "Discord_Shutdown", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Discord_Shutdown();

        [DllImport(DiscordDLL, EntryPoint = "Discord_RunCallbacks", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Discord_RunCallbacks();

        [DllImport(DiscordDLL, EntryPoint = "Discord_UpdatePresence", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Discord_UpdatePresence(ref DiscordRichPresence presence);

        [DllImport(DiscordDLL, EntryPoint = "Discord_ClearPresence", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Discord_ClearPresence();

        [DllImport(DiscordDLL, EntryPoint = "Discord_Respond", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Discord_Respond(IntPtr userid, int reply);
    }

}