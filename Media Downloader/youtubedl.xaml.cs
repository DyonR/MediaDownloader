using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using System;

namespace MediaDownloader
{

    /// <summary>
    /// Interaction logic for youtubedl.xaml
    /// </summary>
    public partial class youtubedl : UserControl
    {
        public youtubedl()
        {
            InitializeComponent();
            Loaded += youtubedl_Loaded;
        }

        BackgroundWorker processWorker;

        //We have to get the Downloads Folder location, we do this by using the registry so we can find the Downloads folder of the user, most times, this is C:\Users\Username\Downloads.
        //But, since it can vary from user to user, we use the registry to make sure.
        public string DownloadsFolder = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads\\");
        public string LocalStorageFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\");
        public string YouTubeDLPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\youtube-dl.exe");
        public string RipMePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\ripme.jar");
        public string ffmpegfolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\ffmpeg");
        public string ffmpegPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + (@"\Media Downloader\ffmpeg\bin\ffmpeg.exe");

        //Here we need to define all string we use for different statements, this is so we can use the same string in if/else statements.
        public string DefaultArguments;
        public string AudioArguments;
        public string VideoArguments;
        public string DownloadURL;

        //When the form loads, we have to make sure that the "Media Downloads" folder exists, we first check if it exists, if it does not exist, I will create it.
        private void youtubedl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= youtubedl_Loaded;
            if (!Directory.Exists(DownloadsFolder))
            {
                Directory.CreateDirectory(DownloadsFolder);
            }
            if (!Directory.Exists(LocalStorageFolder))
            {
                Directory.CreateDirectory(LocalStorageFolder);
            }
            Directory.SetCurrentDirectory(DownloadsFolder);
        }

        //Here we remove the text of the textboxes so the user does not need to remove the text him or herself, the text will get removed when the box is select, also known as 'GotFocus'.
        private void youtubedlURLBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (youtubedlURLBox.Text == "Paste your URL's in here!" || youtubedlURLBox.Text.Contains("Finished!"))
                youtubedlURLBox.Text = null;
        }
        private void SeparateFolderTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SeparateFolderTextBox.Text == "Enter the folder name here")
                SeparateFolderTextBox.Text = null;
        }
        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "Username")
                UsernameTextBox.Text = null;
        }
        private void PasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBox.Password == "Password")
                PasswordTextBox.Password = null;
        }
        private void VideoPasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (VideoPasswordTextBox.Password == "Password")
                VideoPasswordTextBox.Password = null;
        }


        //Let's start the fun, the magic download button.
        private void StartyouTubedlButton_Click(object sender, RoutedEventArgs e)
        {
            //If the URL box contains the default text or is empty, we will give the user a error message after the button has been pressed.
            if (!youtubedlURLBox.Text.Contains(".") || !File.Exists(YouTubeDLPath) || !File.Exists(RipMePath) || !File.Exists(ffmpegPath))
            {
                MessageBox.Show("You did not enter a URL, youtube-dl, RipMe or FFmpeg is missing.");
            }
            //If the URL does not contain the default text, we can continue with our script.
            else
            {
                processWorker = new BackgroundWorker();
                processWorker.WorkerReportsProgress = true;
                //We replace each new line in the URL text box with spaces, so we can download multiple URLs at once
                DownloadURL = null;
                DownloadURL = youtubedlURLBox.Text.Replace(System.Environment.NewLine, " ");
                youtubedlURLBox.Text = null;

                //First, we check if the RipMe checkbox is checked, if that is the case, we will use the RipMe process.
                if (RipMeBox.IsChecked.Value)
                {
                    processWorker.DoWork += (obj, ea) => ripMe_Process();
                    processWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ripMe_Process_Complete);
                    processWorker.RunWorkerAsync();
                }
                else
                {
                    processWorker.DoWork += (obj, ea) => youtubeDL_Process();
                    processWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(youtubeDL_Process_Complete);
                    processWorker.RunWorkerAsync();
                }
            }
        }

        public void ripMe_Process()
        {
            string DownloadsFolder = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads\\");
            Directory.SetCurrentDirectory(DownloadsFolder);
            Process ripme = new Process();
            this.Dispatcher.Invoke((Action)(() => {
                killButton.IsEnabled = true;
                StartyouTubedlButton.IsEnabled = false;
                if (LivestreamBox.IsChecked.Value)
                {
                    ripme.StartInfo.CreateNoWindow = false;
                }
                else
                {
                    ripme.StartInfo.CreateNoWindow = true;
                }
            }));
            ripme.StartInfo.UseShellExecute = false;
            ripme.StartInfo.FileName = "java";
            ripme.StartInfo.Arguments = " -jar \"" + RipMePath + "\" -u " + DownloadURL;
            ripme.StartInfo.RedirectStandardOutput = true;
            ripme.OutputDataReceived += new DataReceivedEventHandler(ripme_OutputDataReceived);
            ripme.Start();
            ripme.BeginOutputReadLine();
            ripme.WaitForExit();
        }
        public void ripme_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>{
                if(e.Data != null)
                {
                    youtubedlURLBox.Text += e.Data.ToString() + System.Environment.NewLine + System.Environment.NewLine;
                    youtubedlURLBox.ScrollToEnd();
                }
            }));
        }

        private void ripMe_Process_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            youtubedlURLBox.Text += System.Environment.NewLine + "Finished!";
            killButton.IsEnabled = false;
            StartyouTubedlButton.IsEnabled = true;
            processWorker = null;
        }

        public void youtubeDL_Process()
        {
            string DownloadsFolder = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads\\");
            Directory.SetCurrentDirectory(DownloadsFolder);
            this.Dispatcher.Invoke((Action)(() => {
                killButton.IsEnabled = true;
                StartyouTubedlButton.IsEnabled = false;
                string AudioArguments = "--continue --ignore-errors --no-overwrites --extract-audio --output \"%(title)s.%(ext)s\" --audio-format mp3 --audio-quality 0 --ffmpeg-location \"" + ffmpegPath + "\" ";
                if (AudioButton.IsChecked.Value) { DefaultArguments = AudioArguments; };

                //Set the correct arguments when the Video Button is checked
                string VideoArguments = "--continu --ignore-errors --no-overwrites --output \"%(title)s.%(ext)s\" --ffmpeg-location \"" + ffmpegPath + "\" ";
                if (VideoButton.IsChecked.Value) { DefaultArguments = VideoArguments; };

                //Set the connection type of youtube-dl to IPv6, this option is still experimental, but I think it was a nice thing to add.
                if (IPv6Box.IsChecked.Value) { DefaultArguments = DefaultArguments + "--force-ipv6 "; };

                //If the limit speed textbox is not empty, we have to add the download limit to the arguments.
                if (!string.IsNullOrEmpty(LimitSpeedBox.Text))
                {
                    DefaultArguments = DefaultArguments + "--rate-limit " + LimitSpeedBox.Text + " ";
                }

                //If the Login box is checked, we need to add the login credentials to youtube-dl.
                if (LoginBox.IsChecked.Value)
                {
                    DefaultArguments = DefaultArguments + "--username " + UsernameTextBox.Text + " --password " + PasswordTextBox.Password + " ";
                }

                //If the video password box is checked, we to pass the video password to youtube-dl.
                if (VideoPasswordBox.IsChecked.Value)
                {
                    DefaultArguments = DefaultArguments + "--video-password " + VideoPasswordTextBox.Password + " ";
                }
            }));
                //Here is where we create the youtube-dl process, and start it.
                //If the livesteam checkbox is checked, we will create a new windows, so users can stop the livestream download
                //not a nice way to do this, since it just shows the youtube-dl console. But I could not find a way to do this differently.
            Process youtubedl = new Process();
            this.Dispatcher.Invoke((Action)(() => {
                if (LivestreamBox.IsChecked.Value)
                {
                    youtubedl.StartInfo.CreateNoWindow = false;
                }
                else {
                    youtubedl.StartInfo.CreateNoWindow = true;
                }
            }));
            youtubedl.StartInfo.UseShellExecute = false;
            youtubedl.StartInfo.FileName = YouTubeDLPath;
            youtubedl.StartInfo.Arguments = DefaultArguments + DownloadURL;

            //If the Separate Folder box is checked, we need to create this directory and set our current working location to there.
            //After that we can start the download process
            youtubedl.StartInfo.RedirectStandardOutput = true;
            youtubedl.OutputDataReceived += new DataReceivedEventHandler(youtubeDL_Process_OutputDataReceived);
            this.Dispatcher.Invoke((Action)(() => {
                if (SeparateFolderBox.IsChecked.Value)
            {
                Directory.CreateDirectory(SeparateFolderTextBox.Text);
                DownloadsFolder = DownloadsFolder + SeparateFolderTextBox.Text;
                Directory.SetCurrentDirectory(DownloadsFolder);
            }
            }));
            youtubedl.Start();
            youtubedl.BeginOutputReadLine();
            youtubedl.WaitForExit();
        }

        public void youtubeDL_Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() => {
                if (e.Data != null)
                {
                    youtubedlURLBox.Text = e.Data.ToString();
                }
            }));
        }

        private void youtubeDL_Process_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            youtubedlURLBox.Text += System.Environment.NewLine + "Finished!";
            killButton.IsEnabled = false;
            StartyouTubedlButton.IsEnabled = true;
            processWorker = null;
        }

        //Down here, we just enable of disable some boxes, nothing too exicting.
        private void SeparateFolderBox_Checked(object sender, RoutedEventArgs e)
        {
            SeparateFolderTextBox.IsEnabled = true;
        }

        private void SeparateFolderBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SeparateFolderTextBox.IsEnabled = false;
        }

        private void LoginBox_Checked(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.IsEnabled = true;
            PasswordTextBox.IsEnabled = true;
            TwoFactorBox.IsEnabled = true;
        }

        private void LoginBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.IsEnabled = false;
            PasswordTextBox.IsEnabled = false;
            TwoFactorBox.IsChecked = false;
            TwoFactorBox.IsEnabled = false;
        }
        private void RipMeBox_Checked(object sender, RoutedEventArgs e)
        {
            AudioButton.IsEnabled = false;
            AudioButton.IsChecked = false;
            VideoButton.IsEnabled = false;
            VideoButton.IsChecked = false;
            SeparateFolderBox.IsEnabled = false;
            SeparateFolderBox.IsChecked = false;
            SeparateFolderTextBox.Text = "Enter the folder name here";
            LimitSpeedBox.IsEnabled = false;
            LimitSpeedBox.Text = null;
            LivestreamBox.IsEnabled = false;
            LivestreamBox.IsChecked = false;
            LoginBox.IsEnabled = false;
            LoginBox.IsChecked = false;
            UsernameTextBox.Text = "Username";
            PasswordTextBox.Password = "Password";
            VideoPasswordBox.IsEnabled = false;
            VideoPasswordBox.IsChecked = false;
            VideoPasswordTextBox.Password = "Password";
            IPv6Box.IsEnabled = false;
            IPv6Box.IsChecked = false;
        }
        private void RipMeBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AudioButton.IsEnabled = true;
            AudioButton.IsChecked = true;
            VideoButton.IsEnabled = true;
            SeparateFolderBox.IsEnabled = true;
            LimitSpeedBox.IsEnabled = true;
            LivestreamBox.IsEnabled = true;
            LoginBox.IsEnabled = true;
            UsernameTextBox.Text = "Username";
            PasswordTextBox.Password = "Password";
            VideoPasswordBox.IsEnabled = true;
            VideoPasswordTextBox.Password = "Password";
            IPv6Box.IsEnabled = true;
        }

        private void VideoPasswordBox_Checked(object sender, RoutedEventArgs e)
        {
            VideoPasswordTextBox.IsEnabled = true;
        }

        private void VideoPasswordBox_Unchecked(object sender, RoutedEventArgs e)
        {
            VideoPasswordTextBox.IsEnabled = false;
        }

        private void removeHistory_Click(object sender, RoutedEventArgs e)
        {
            try {
                File.Delete(DownloadsFolder + "rip.properties");
            }
            catch
            {
                MessageBox.Show("Can't delete " + DownloadsFolder + "rip.properties");
            }
            try {
                File.Delete(DownloadsFolder + "history.json");
            }
            catch
            {
                MessageBox.Show("Can't delete " + DownloadsFolder + "history.json");
            }
            try
            {
                File.Delete(DownloadsFolder + "ripme.log");
            }
            catch
            {
                MessageBox.Show("Can't delete " + DownloadsFolder + "ripme.log");
            }
        }

        private void killButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult WarningResult = MessageBox.Show(@"This will kill all the current youtube-dl, FFmpeg, Java en RTMPDump processes.
If you have a Java process running, this will also get force closed.
Do you want to continue and kill the processes?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (WarningResult == MessageBoxResult.Yes) {
                Process[] prs = Process.GetProcesses();
                foreach (Process pr in prs)
                {
                    if (pr.ProcessName == "youtube-dl")
                    {
                        pr.Kill();
                    }
                    if (pr.ProcessName == "ffmpeg")
                    {
                        pr.Kill();
                    }
                    if (pr.ProcessName == "java")
                    {
                        pr.Kill();
                    }
                    if (pr.ProcessName == "rtmpdump")
                    {
                        pr.Kill();
                    }
                }
            }
        }
    }
}