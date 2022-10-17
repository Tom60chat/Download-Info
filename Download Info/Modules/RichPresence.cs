namespace discord_rpc
{
    /// <summary>
    /// Wrapper around string->IntPtr
    /// C# String = UTF-16, C String = UTF-8
    /// </summary>
    public class RichPresence
    {
        public string State;
        public string Details;
        public long startTimestamp;
        public long endTimestamp;
        public string largeImageKey;
        public string largeImageText;
        public string smallImageKey;
        public string smallImageText;
        public int partySize;
        public int partyMax;
        public string matchSecret;
        public string joinSecret;
        public string spectateSecret;
        public byte instance;
    }
}