using System;
using System.Runtime.InteropServices;

namespace discord_rpc
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DiscordEventHandlers_ready();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DiscordEventHandlers_disconnected(int errorCode, string message);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DiscordEventHandlers_errored(IntPtr errorCode, IntPtr message);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DiscordEventHandlers_joinGame(string joinSecret);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DiscordEventHandlers_spectateGame(string spectateSecret);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DiscordEventHandlers_joinRequest(ref DiscordJoinRequest request);

    [StructLayout(LayoutKind.Sequential)]
    public struct DiscordEventHandlers
    {
        public DiscordEventHandlers_ready ready;
        public DiscordEventHandlers_disconnected disconnected;
        public DiscordEventHandlers_errored errored;
        public DiscordEventHandlers_joinGame joinGame;
        public DiscordEventHandlers_spectateGame spectateGame;
        public DiscordEventHandlers_joinRequest joinRequest;
    }
}