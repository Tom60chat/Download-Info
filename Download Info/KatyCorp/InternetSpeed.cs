using System;
using System.Diagnostics;

namespace KatyCorp.Tools
{
    public class InternetSpeed
    {
        private readonly Stopwatch stopwatch;
        private TimeSpan LastElapsed;
        private long LastSizeElapsed;

        public InternetSpeed()
        {
            stopwatch = new Stopwatch();
        }

        public string GetInternetSpeed(long FileSize)
        {
            long speed = GetInternetSpeedRaw(FileSize);

            if (speed == -1)
                return string.Empty;
            else
                return ConvertData(speed);
        }

        public long GetInternetSpeedRaw(long FileSize)
        {
            if (stopwatch.IsRunning)
            {
                double TimeElapsed = stopwatch.Elapsed.TotalSeconds - LastElapsed.TotalSeconds;

                if (LastElapsed.TotalSeconds + 1 < stopwatch.Elapsed.TotalSeconds)
                {

                    long SizeElapsed = FileSize - LastSizeElapsed;
                    long DownloadSpeed = SizeElapsed / (long)TimeElapsed;
                    LastSizeElapsed = FileSize;
                    LastElapsed = stopwatch.Elapsed;

                    if (DownloadSpeed < 0)
                        return -1;

                    return DownloadSpeed;
                }
                //Console.WriteLine("Time elapsed: {0} / Size elpased: {1}", TimeElapsed, SizeElapsed);
            }
            else
            {
                stopwatch.Start();
                LastSizeElapsed = FileSize;
                LastElapsed = stopwatch.Elapsed;
            }
            return -1;
        }

        public static string ConvertData(long Octect, bool speed = true)
        {
            string Speed = "/s";
            if (!speed)
                Speed = string.Empty;

            // 1000000000 Octet/Byte = 1 Go/GB
            if (Octect >= 1000000000)
                return string.Format("{0} Go{1}", Math.Round(Octect * 0.000000001, 2), Speed);

            // 1000000 Octet/Byte = 1 Mo/MB
            else if (Octect >= 1000000)
                return string.Format("{0} Mo{1}", Math.Round(Octect * 0.000001, 2), Speed);

            // 1000 Octet/Byte = 1 Ko/KB
            else if (Octect >= 1000)
                return string.Format("{0} Ko{1}", Math.Round(Octect * 0.001, 2), Speed);

            // other
            else
                return string.Format("{0} Octet{1}", Math.Round((double)Octect, 2), Speed);
        }
    }
}
