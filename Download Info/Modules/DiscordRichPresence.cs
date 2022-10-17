using System;
using System.Runtime.InteropServices;

namespace discord_rpc
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DiscordRichPresence
    {
        internal IntPtr state;
        internal IntPtr details;
        internal long startTimestamp;
        internal long endTimestamp;
        internal IntPtr largeImageKey;
        internal IntPtr largeImageText;
        internal IntPtr smallImageKey;
        internal IntPtr smallImageText;
        internal IntPtr partyId;
        internal int partySize;
        internal int partyMax;
        internal IntPtr matchSecret;
        internal IntPtr joinSecret;
        internal IntPtr spectateSecret;
        internal byte instance;
    }
}
