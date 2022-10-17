using discord_rpc;
using KatyCorp.Tools;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Path = System.IO.Path;

namespace Download_Info
{
    /// <summary>
    /// Logique d'interaction pour Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {

        public RichPresence RPC;
        public string path;
        private readonly string AppName;
        /// <summary>
        /// Normal:
        /// In gigaoctet
        /// Pre size:
        /// In octet
        /// </summary>
        public float TotalSize;
        /// <summary>
        /// Normal:
        /// In octet
        /// Pre size:
        /// In gigaoctet
        /// </summary>
        public float ActualSize;

        /// <summary>
        /// In megaoctet
        /// </summary>
        private float DlSpeed;
        public bool Manual;
        public string[] ForbidentChar = new string[] { ".", "$", "é", "&", "'", "(", "è", "_", "ç", "à", ")", "=", "^", "+£", "¤", "ù", "%", "µ", "*", "!", "§", "/", ":", ";", "?", "<", ">", "°" };

        public Page1(string FolderUri, float FinalSize, float InternetSpeed, float ActualSizeMW, bool ManualMW, RichPresence RPCMW)
        {
            InitializeComponent();
            RPC = RPCMW;
            Manual = ManualMW;
            path = FolderUri;
            if (!path.EndsWith(@"\"))
                path += @"\";
            AppName = Path.GetFileName(Path.GetDirectoryName(path));

            // Displays the application being downloaded on discord
            RPC.Details = "Downloading: " + AppName;
            RPC.largeImageKey = RPCassets.GetImageKey(AppName);
            RPC.largeImageText = AppName;
            RPC.smallImageKey = "download";
            RPC.smallImageText = "Downloading...";
            Discord.UpdatePresence(RPC);

            if (!Manual)
            {
                TextSize.Text = "Final size:";
                BoxSize.Text = FinalSize.ToString();

                //TextInternetSpeed.Text = "Transfer speed";

                SpeedPanel.Visibility = Visibility.Hidden;
                TextSpeed.Visibility = Visibility.Visible;
                this.FinalSize.Visibility = Visibility.Collapsed;

                TotalSize = (long)FinalSize; // In Go

                NormalMode();
            }
            else
            {
                TextSize.Text = "Actual size:";
                BoxSize.Text = ActualSizeMW.ToString();

                SpeedPanel.Visibility = Visibility.Visible;
                TextSpeed.Visibility = Visibility.Hidden;
                ActualSize_Text.Visibility = Visibility.Collapsed;

                BoxInternetSpeed.Text = InternetSpeed.ToString();

                ActualSize = ActualSizeMW; // In Go
                DlSpeed = InternetSpeed; // In Mo/s

                PreSizeMode();
            }

        }

        private void BoxSize_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BoxSize.Text))
            {
                FinalSizeCheck.Visibility = Visibility.Hidden;
            }
            else
            {
                if (!MainWindow.ForbidentChar(BoxSize.Text))
                {
                    FinalSizeCheck.Text = "✔";
                    FinalSizeCheck.Foreground = Brushes.Green;
                    FinalSizeCheck.Visibility = Visibility.Visible;

                    try
                    {
                        if (Manual)
                        {
                            ActualSize = long.Parse(BoxSize.Text);
                        }
                        else
                        {
                            TotalSize = long.Parse(BoxSize.Text);
                        }
                    }
                    catch
                    {
                        FinalSizeCheck.Text = "❌";
                        FinalSizeCheck.Foreground = Brushes.Red;
                        FinalSizeCheck.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    FinalSizeCheck.Text = "❌";
                    FinalSizeCheck.Foreground = Brushes.Red;
                    FinalSizeCheck.Visibility = Visibility.Visible;
                }
            }
        }

        private void BoxInternetSpeed_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BoxInternetSpeed.Text))
            {
                InternetSpeedCheck.Visibility = Visibility.Hidden;
            }
            else
            {
                bool ForbidentCharDetect = false;
                for (int i = 0; i < ForbidentChar.Length; i++)
                {
                    if (BoxInternetSpeed.Text.Contains(ForbidentChar[i]))
                        ForbidentCharDetect = true;
                }
                if (!ForbidentCharDetect && !Regex.IsMatch(BoxInternetSpeed.Text, "[A-Z]") && !Regex.IsMatch(BoxInternetSpeed.Text, "[a-z]"))
                {
                    InternetSpeedCheck.Text = "✔";
                    InternetSpeedCheck.Foreground = Brushes.Green;
                    InternetSpeedCheck.Visibility = Visibility.Visible;

                    try
                    {
                        DlSpeed = float.Parse(BoxInternetSpeed.Text);
                    }
                    catch
                    {
                        InternetSpeedCheck.Text = "❌";
                        InternetSpeedCheck.Foreground = Brushes.Red;
                        InternetSpeedCheck.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    InternetSpeedCheck.Text = "❌";
                    InternetSpeedCheck.Foreground = Brushes.Red;
                    InternetSpeedCheck.Visibility = Visibility.Visible;
                }
            }
        }

        private async void NormalMode()
        {
            DirectoryInfo di = new DirectoryInfo(path);
            InternetSpeed internetSpeed = new InternetSpeed();
            string OldSpeed = string.Empty;

            // Displays the application being downloaded
            TextDownloading.Text = "Downloading: " + AppName;
            TextSpeed.Text = "Calculating";

            while (true)
            {
                try
                {
                    float OldActualSize = ActualSize;
                    long OldTimeLeft = -1;
                    ActualSize = AppSize.DirSize(di);
                    float Poucentage = 0;
                    long TimeLeft = -1;

                    // Avoid System.DivideByZeroException exception.
                    if (ActualSize != 0 && TotalSize != 0)
                        Poucentage = ActualSize * 100 / (TotalSize * 1000000000);
                    Poucentage = (float)Math.Round(Poucentage, 2);

                    ActualSize_Text.Text = InternetSpeed.ConvertData((long)ActualSize, false) + " / ";
                    ProgressBar1.Value = Poucentage;
                    TextProgress.Text = Poucentage.ToString() + "%";

                    if (OldActualSize != ActualSize)
                    {
                        try
                        {
                            long speed = internetSpeed.GetInternetSpeedRaw((long)ActualSize);
                            string Speed = InternetSpeed.ConvertData(speed);
                            if (Speed != string.Empty)
                            {
                                OldSpeed = Speed;
                                TextSpeed.Text = '~' + Speed;
                            }
                            else
                            {
                                Speed = OldSpeed;
                            }

                            /*// Calcul time left
                            if (ActualSize != 0 && TotalSize != 0)
                            {
                                TimeLeft = (long)(TotalSize - (ActualSize * 1000000000)) / speed;
                                TimeLeft = (long)(Math.Round((Decimal)TimeLeft, 0, MidpointRounding.AwayFromZero));
                            }*/

                            // Show info
                            Console.WriteLine("Downloading {0}: {1}% | ~{2}| {3}", AppName, Poucentage, Speed, TimeLeft);
                            RPC.State = string.Format("{0}% | ~{1}", Poucentage, Speed);
                            /*if (OldTimeLeft != TimeLeft && TimeLeft != -1)
                            {
                                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                RPC.endTimestamp = Timestamp + TimeLeft;
                                OldTimeLeft = TimeLeft;
                            }*/

                            Discord.UpdatePresence(RPC);
                        }
                        catch
                        {
                            FinalSizeCheck.Text = "❌";
                            FinalSizeCheck.Foreground = Brushes.Red;
                            FinalSizeCheck.Visibility = Visibility.Visible;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    TextDownloading.Text = "Error: " + e.Message;
                    RPC.Details = "Error: " + e.Message;
                    Discord.UpdatePresence(RPC);
                }

                await Task.Delay(1000);
            }
        }

        private async void PreSizeMode()
        {
            DirectoryInfo di = new DirectoryInfo(path);
            float OldPoucentage = 0;
            long OldTimeLeft = -1;

            // Displays the application being downloaded
            TextDownloading.Text = "Downloading: " + AppName;
            Discord.UpdatePresence(RPC);

            while (true)
            {
                try
                {
                    long Speed = (long)(DlSpeed * 1000000); // 1 Mb to Byte
                    float Poucentage = 0;
                    TotalSize = AppSize.DirSize(di);

                    // Avoid System.DivideByZeroException exception.
                    if (ActualSize != 0 && TotalSize != 0)
                        Poucentage = (ActualSize * 1000000000) * 100 / TotalSize;
                    Poucentage = (float)Math.Round(Poucentage, 2);

                    if (OldPoucentage < 100 && !BoxSize.IsFocused)
                    {
                        try
                        {
                            // Calcul time left
                            long TimeLeft = -1;
                            if (ActualSize != 0 && TotalSize != 0)
                                TimeLeft = (long)(TotalSize - (ActualSize * 1000000000)) / Speed;

                            TimeLeft = (long)(Math.Round((Decimal)TimeLeft, 0, MidpointRounding.AwayFromZero));

                            // Show the info
                            Console.WriteLine("Downloading {0}: ~{1}% | ~{2} | {3}", AppName, Poucentage, InternetSpeed.ConvertData(Speed, true), TimeLeft);
                            RPC.State = string.Format("~{0}% | ~{1}", Poucentage, InternetSpeed.ConvertData(Speed, true));

                            if (OldTimeLeft != TimeLeft)
                            {
                                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                RPC.endTimestamp = Timestamp + TimeLeft;
                                OldTimeLeft = TimeLeft;
                            }
                        }
                        catch
                        {
                            FinalSizeCheck.Text = "❌";
                            FinalSizeCheck.Foreground = Brushes.Red;
                            FinalSizeCheck.Visibility = Visibility.Visible;
                        }

                        if (OldPoucentage != Poucentage)
                            Discord.UpdatePresence(RPC);

                        FinalSize.Text = " / " + InternetSpeed.ConvertData((long)TotalSize, false);
                        ProgressBar1.Value = Poucentage;
                        TextProgress.Text = "~" + Poucentage.ToString() + "%";
                        ActualSize += DlSpeed * 0.001f; // Mo/s to Go/s
                        BoxSize.Text = ActualSize.ToString();
                    }

                    OldPoucentage = Poucentage;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    TextDownloading.Text = "Error: " + e.Message;
                    RPC.Details = "Error: " + e.Message;
                    Discord.UpdatePresence(RPC);
                }

                await Task.Delay(1000);
            }
        }
    }
}
