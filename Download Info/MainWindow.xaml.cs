using discord_rpc;
using KatyLauncher.API.Tools;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Download_Info
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool DiscordActive = false;
        private const string AppId = "com.KatyCorp.DownloadInfo";

        public MainWindow()
        {
            InitializeComponent();

            Start();
        }

        public async void Start()
        {
            MainGrid.Visibility = Visibility.Hidden;
            DiscordCheckGrid.Visibility = Visibility.Visible;

            PreSize_mode.Visibility = Visibility.Hidden;

            Label.Text = "Checking for updates.";
            await KatyLauncherAPIHelper.Check(AppId);

            CheckDiscord();
        }

        #region Discord

        private void CheckDiscord()
        {
            Retry.Visibility = Visibility.Hidden;
            Process[] discordapp = Process.GetProcessesByName("Discord");
            //int i = discordapp.Length;

            if (discordapp.Length > 0)
            {
                Console.WriteLine("Discord process is found. Starting Discord-RPC module...");
                Label.Text = "Discord process is found. Starting Discord-RPC module...";

                RPCStart(Label);
            }
            else
            {
                Console.WriteLine("Discord process is not found.\nFirstly start Discord before starting Download Info.\nRetry or continue without Discord");

                Label.Text = "Discord process is not found.\nFirstly start Discord before starting Download Info.\nRetry or continue without Discord";
                Retry.Visibility = Visibility.Visible;

                return;
            }
        }

        private static bool ready = false;
        private static readonly RichPresence RPC = new RichPresence { Details = "Details" };

        public static TextBlock LabelPS;

        public static void RPCStart(TextBlock Label)
        {
            LabelPS = Label;
            //Run();
            Task.Run(Callbacks);
            Run(Label).GetAwaiter().GetResult();
        }

        private static async Task Run(TextBlock Label)
        {
            Console.WriteLine("Discord-RPC Module Started!");
            Label.Text = ("Discord-RPC Module Started!");
            //string str = String.Empty;
            //do
            //{
            if (ready)
            {
                //some code :^)
            }
            else
            {
                Console.WriteLine("Connecting...");
                Label.Text = "Connecting...";
                //await Task.Delay(2000);
                DiscordEventHandlers handlers = new DiscordEventHandlers { };
                handlers.ready = Ready;
                handlers.disconnected = Disconnected;
                await Discord.InitializeAsync("492421401433604107", handlers, 1, "");
            }
            //await Task.Delay(100);
            //} while (str.ToLower() != "c");
            //await discord_rpc.Discord.ShutdownAsync();
        }

        private static async Task Callbacks()
        {
            while (true)
            {
                Discord.RunCallbacks();
                await Task.Delay(100);
            }
        }

        private static void Disconnected(int errorCode, string Message)
        {
            Discord.ClearPresence();
            ready = false;
            Console.WriteLine("Disconnected! {0}", Message);
        }

        private static void Ready()
        {
            try
            {
                Console.WriteLine("Connected!");
                //LabelPS.Text = "Connected!";
                ready = true;

                RPC.largeImageKey = "downloadrp";
                RPC.Details = "Starting...";

                Discord.UpdatePresence(RPC);
                //MainWindow foo = new MainWindow();
                //foo.Main();

                //DiscordLoading.Content = DL;

                Application.Current.Dispatcher.Invoke(() => (Application.Current.MainWindow as MainWindow)?.DiscordReady());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DiscordReady()
        {
            Console.WriteLine("Discord done");
            DiscordActive = true;
            MainGrid.Visibility = Visibility.Visible;
            DiscordCheckGrid.Visibility = Visibility.Hidden;
            RPC.Details = "Configuring app";

            Discord.UpdatePresence(RPC);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Discord skip");
            DiscordActive = false;
            MainGrid.Visibility = Visibility.Visible;
            DiscordCheckGrid.Visibility = Visibility.Hidden;
        }

        private void Retry_Click(object sender, RoutedEventArgs e) => CheckDiscord();

        #endregion

        public static string GetTimestamp(DateTime value) => value.ToString("yyyyMMddHHmmssffff");

        public static bool ForbidentChar(string Text)
        {
            int count = 0;
            foreach (char c in Text)
            {
                if (c == ',') count++;
            }

            float.TryParse(Text, out float num);

            return Regex.IsMatch(Text, @"[A-Z]|[^\w\d ,]|[a-z]|[ ]") || count > 1 || num == 0;
        }

        public string FolderUri_placeholder;
        public bool FolderUri_placeholderToGet = true;

        private void FolderUri_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FolderUri.Text))
            {
                FolderUri.Text = FolderUri_placeholder;
                FolderUriCheck.Visibility = Visibility.Hidden;
            }
            else
            {
                if (Directory.Exists(FolderUri.Text))
                {
                    FolderUriCheck.Text = "✔";
                    FolderUriCheck.Foreground = Brushes.Green;
                    FolderUriCheck.Visibility = Visibility.Visible;
                }
                else
                {
                    FolderUriCheck.Text = "❌";
                    FolderUriCheck.Foreground = Brushes.Red;
                    FolderUriCheck.Visibility = Visibility.Visible;
                }
            }
            ButtonCheck();
        }

        private void FolderUri_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderUriCheck.Visibility = Visibility.Hidden;
            if (FolderUri_placeholderToGet)
            {
                FolderUri_placeholder = FolderUri.Text;
                FolderUri_placeholderToGet = false;
            }
            if (FolderUri_placeholder == FolderUri.Text)
                FolderUri.Text = "";
        }

        private void FolderUri_Browse_Button_Click(object sender, RoutedEventArgs e)
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                FolderUri_GotFocus(sender, e);
                using (var dialog = new CommonOpenFileDialog())
                {
                    dialog.Title = "Select a install folder";
                    dialog.IsFolderPicker = true;
                    if (Directory.Exists(FolderUri.Text))
                        dialog.InitialDirectory = FolderUri.Text;
                    /*else
                        dialog.InitialDirectory = @"C:\Program Files";*/

                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                        FolderUri.Text = Directory.Exists(dialog.FileName) ? dialog.FileName : Path.GetDirectoryName(dialog.FileName);
                }
                FolderUri_GotFocus(sender, e);
                FolderUri_LostFocus(sender, e);
            }
            else
            {
                /* using (var dialog = new System.Windows.Forms.FolderBrowserDialog())  // this is old
                {
                    dialog.SelectedPath = FolderUri.Text;
                    dialog.Description = "Select a install folder";
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    FolderUri.Text = dialog.SelectedPath;
                } */
                MessageBox.Show("Your operating system is not supported, please write the path manually.");
            }
        }


        public string FinalSize_placeholder;
        public bool FinalSize_placeholderToGet = true;

        private void FinalSize_GotFocus(object sender, RoutedEventArgs e)
        {
            FinalSizeCheck.Visibility = Visibility.Hidden;
            if (FinalSize_placeholderToGet)
            {
                FinalSize_placeholder = FinalSize.Text;
                FinalSize_placeholderToGet = false;
            }
            if (FinalSize_placeholder == FinalSize.Text)
                FinalSize.Text = "";
        }

        private void FinalSize_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FinalSize.Text))
            {
                FinalSize.Text = FinalSize_placeholder;
                FinalSizeCheck.Visibility = Visibility.Hidden;
            }
            else
            {
                if (!ForbidentChar(FinalSize.Text))
                {
                    FinalSizeCheck.Text = "✔";
                    FinalSizeCheck.Foreground = Brushes.Green;
                    FinalSizeCheck.Visibility = Visibility.Visible;
                }
                else
                {
                    FinalSizeCheck.Text = "❌";
                    FinalSizeCheck.Foreground = Brushes.Red;
                    FinalSizeCheck.Visibility = Visibility.Visible;
                }
            }
            ButtonCheck();
        }


        public string InternetSpeed_placeholder;
        public bool InternetSpeed_placeholderToGet = true;

        private void InternetSpeed_GotFocus(object sender, RoutedEventArgs e)
        {
            InternetSpeedCheck.Visibility = Visibility.Hidden;
            if (InternetSpeed_placeholderToGet)
            {
                InternetSpeed_placeholder = InternetSpeed.Text;
                InternetSpeed_placeholderToGet = false;
            }
            if (InternetSpeed_placeholder == InternetSpeed.Text)
                InternetSpeed.Text = "";
        }

        private void InternetSpeed_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InternetSpeed.Text))
            {
                InternetSpeed.Text = InternetSpeed_placeholder;
                InternetSpeedCheck.Visibility = Visibility.Hidden;
            }
            else
            {
                if (!ForbidentChar(InternetSpeed.Text))
                {
                    InternetSpeedCheck.Text = "✔";
                    InternetSpeedCheck.Foreground = Brushes.Green;
                    InternetSpeedCheck.Visibility = Visibility.Visible;

                    if (float.Parse(InternetSpeed.Text) > 80)
                    {
                        InternetSpeedComment.Visibility = Visibility.Visible;
                        InternetSpeedComment.Text = "To Fast( ͡° ͜ʖ ͡°)";
                    }
                    else if (float.Parse(InternetSpeed.Text) < 0.200)
                    {
                        InternetSpeedComment.Visibility = Visibility.Visible;
                        InternetSpeedComment.Text = "To SLOW 🐌";
                    }
                    else
                    {
                        InternetSpeedComment.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    InternetSpeedCheck.Text = "❌";
                    InternetSpeedCheck.Foreground = Brushes.Red;
                    InternetSpeedCheck.Visibility = Visibility.Visible;
                }

            }
            ButtonCheck();
        }

        public string ActualSize_placeholder;
        public bool ActualSize_placeholderToGet = true;

        private void ActualSize_GotFocus(object sender, RoutedEventArgs e)
        {
            ActualSizeCheck.Visibility = Visibility.Hidden;
            if (ActualSize_placeholderToGet)
            {
                ActualSize_placeholder = ActualSize.Text;
                ActualSize_placeholderToGet = false;
            }
            if (ActualSize_placeholder == ActualSize.Text)
                ActualSize.Text = "";
        }

        private void ActualSize_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ActualSize.Text))
            {
                ActualSize.Text = ActualSize_placeholder;
                ActualSizeCheck.Visibility = Visibility.Hidden;
            }
            else
            {
                if (!ForbidentChar(ActualSize.Text))
                {
                    ActualSizeCheck.Text = "✔";
                    ActualSizeCheck.Foreground = Brushes.Green;
                    ActualSizeCheck.Visibility = Visibility.Visible;
                }
                else
                {
                    ActualSizeCheck.Text = "❌";
                    ActualSizeCheck.Foreground = Brushes.Red;
                    ActualSizeCheck.Visibility = Visibility.Visible;
                }
            }
            ButtonCheck();
        }

        private void ButtonCheck()
        {
            if (!ManualToggleButton.IsChecked.Value)
                OkButton.IsEnabled =
                    (FolderUriCheck.Text == "✔" && FinalSizeCheck.Text == "✔") &&
                    (FolderUriCheck.Visibility == Visibility.Visible &
                    FinalSizeCheck.Visibility == Visibility.Visible);
            else
                OkButton.IsEnabled =
                    (FolderUriCheck.Text == "✔" && ActualSizeCheck.Text == "✔" && InternetSpeedCheck.Text == "✔") &&
                    (FolderUriCheck.Visibility == Visibility.Visible && ActualSizeCheck.Visibility == Visibility.Visible &&
                    InternetSpeedCheck.Visibility == Visibility.Visible);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e) =>
            Content = new Page1(FolderUri.Text, float.Parse(FinalSize.Text), float.Parse(InternetSpeed.Text), float.Parse(ActualSize.Text), ManualToggleButton.IsChecked.Value, RPC);

        private void ManualToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (ManualToggleButton.IsChecked.Value) // Pre Size Mode
            {
                Current_mode_Text.Text = "Pre size mode:";

                Normal_mode.Visibility = Visibility.Collapsed;

                PreSize_mode.Visibility = Visibility.Visible;

                if (InternetSpeedCheck.Text != "")
                {
                    InternetSpeedCheck.Visibility = Visibility.Visible;
                }
                if (ActualSizeCheck.Text != "")
                {
                    ActualSizeCheck.Visibility = Visibility.Visible;
                }
            }
            else // Normal Mode
            {
                Current_mode_Text.Text = "Normal mode:";

                Normal_mode.Visibility = Visibility.Visible;

                PreSize_mode.Visibility = Visibility.Collapsed;

                if (FinalSizeCheck.Text != "")
                {
                    FinalSizeCheck.Visibility = Visibility.Visible;
                }
            }
            ButtonCheck();
        }

        private void InfoPreSize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("You have two modes, one for each type of launcher:\n\n- 'Normal mode' for normal download.\n   (Origin, Web browser, Epic Games, Blizzard, ...)\n- 'Pre size mode' if the folder size is equal to the game size\n   (Steam, Uplay, ...)", "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void InfoPreSize_MouseEnter(object sender, MouseEventArgs e)
        {
            InfoPreSize.Foreground = Brushes.Blue;
        }

        private void InfoPreSize_MouseLeave(object sender, MouseEventArgs e)
        {
            InfoPreSize.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }

        private void CopyRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("This Software was created by OLIVIER Tom (Tom60chat)\n \nClick Yes to see my Youtube Channel\nor\nClick No to make a donation to me :)\n \nClick cancel for return\nYes the method with the buttons is ridiculous", "CopyRight", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
                Process.Start("https://www.youtube.com/channel/UCFmyqR2GRUErhFiuiFFtnKQ");
            if (result == MessageBoxResult.No)
                Process.Start("https://www.paypal.me/Tom60chat");
        }

        private void CopyRight_MouseEnter(object sender, MouseEventArgs e)
        {
            CopyRight.Foreground = Brushes.Blue;
        }

        private void CopyRight_MouseLeave(object sender, MouseEventArgs e)
        {
            CopyRight.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }
    }
}
