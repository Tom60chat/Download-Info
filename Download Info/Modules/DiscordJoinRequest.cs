using System.Runtime.InteropServices;

namespace discord_rpc
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DiscordJoinRequest
    {
        public string userId;
        public string username;
        public string discriminator;
        public string avatar;
    }
}
