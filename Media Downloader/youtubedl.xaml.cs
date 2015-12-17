using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

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

        
        //We have to get the Downloads Folder location, we do this by using the registry so we can find the Downloads folder of the user, most times, this is C:\Users\Username\Downloads.
        //But, since it can vary from user to user, we use the registry to make sure.
        public object DownloadsFolder = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads");
        public object YouTubeDLPath = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads\\youtube-dl.exe");
        public object RipMePath = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads\\ripme.jar");
      //public object FFmpegPath = "C:\\ffmpeg\\bin\\ffmpeg.exe";

        //Here we need to define all string we use for different statements, this is so we can use the same string in if/else statements.
        public string DefaultArguments;
        public string AudioArguments;
        public string VideoArguments;
        public string DownloadURL;

        //When the form loads, we have to make sure that the "Media Downloads" folder exists, we first check if it exists, if it does not exist, I will create it.
        private void youtubedl_Loaded(object sender, RoutedEventArgs e)
        {
            Directory.SetCurrentDirectory(DownloadsFolder.ToString());
            if (!Directory.Exists(DownloadsFolder.ToString()))
            {
                Directory.CreateDirectory(DownloadsFolder.ToString());
            }
        }
        //Here we remove the text of the textboxes so the user does not need to remove the text him or herself, the text will get removed when the box is select, also known as 'GotFocus'.
        private void youtubedlURLBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (youtubedlURLBox.Text == "Paste your URL's in here!")
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
            if (youtubedlURLBox.Text == "Paste your URL's in here!" || string.IsNullOrEmpty(youtubedlURLBox.Text) || !File.Exists(YouTubeDLPath.ToString()) || !File.Exists(RipMePath.ToString()) )
            {
                MessageBox.Show("You did not enter a URL, youtube-dl, RipMe or FFmpeg is missing.");
            }
            //If the URL does not contain the default text, we can continue with our script.
            else
            {

                //Reset the Downloads Folder location, if this is the 2nd time the user is downloading something in the same session, and the separate folder option was used before, the current
                //Downloadsfolder path is messed up. It would else still contain the path to the separate folder.
                object DownloadsFolder = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads");
                Directory.SetCurrentDirectory(DownloadsFolder.ToString());

                //We replace each new line in the URL text box with spaces, so we can download multiple URLs at once
                DownloadURL = youtubedlURLBox.Text.Replace(System.Environment.NewLine, " ");

                //First, we check if the RipMe checkbox is checked, if that is the case, we will use the RipMe process.
                if (RipMeBox.IsChecked.Value)
                {
                    //Here is where the RipMe process starts, since there are not a lot of arguments for RipMe, we can just start it without worrying about any arguments.
                    //So, lets create the RipMe Process and start downloading.
                    Process ripme = new Process();
                    ripme.StartInfo.CreateNoWindow = false;
                    ripme.StartInfo.UseShellExecute = false;
                    ripme.StartInfo.FileName = "java";
                    ripme.StartInfo.Arguments = " -jar \"" + DownloadsFolder.ToString() + "\\ripme.jar\" -u " + DownloadURL;
                    ripme.Start();
                }
                else
                {
                    //Here starts the youtube-dl part.
                    
                    //Set the correct arguments when the Audio Button is checked
                    string AudioArguments = "--continue --ignore-errors --no-overwrites --extract-audio --output \"%(title)s.%(ext)s\" --audio-format mp3 --audio-quality 0 --ffmpeg-location C:\\ffmpeg\\bin ";
                    if (AudioButton.IsChecked.Value) { DefaultArguments = AudioArguments; };

                    //Set the correct arguments when the Video Button is checked
                    string VideoArguments = "--continu --ignore-errors --no-overwrites --output \"%(title)s.%(ext)s\" --ffmpeg-location C:\\ffmpeg\\bin ";
                    if (VideoButton.IsChecked.Value) { DefaultArguments = VideoArguments; };

                    //Set the connection type of youtube-dl to IPv6, this option is still experimental, but I think it was a nice thing to add.
                    if (IPv6Box.IsChecked.Value) { DefaultArguments = DefaultArguments + "--force-ipv6 "; } ;

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

                    //Here is where we create the youtube-dl process, and start it.
                    Process youtubedl = new Process();
                    youtubedl.StartInfo.CreateNoWindow = true;

                    //If the livesteam checkbox is checked, we will create a new windows, so users can stop the livestream download
                    //not a nice way to do this, since it just shows the youtube-dl console. But I could not find a way to do this differently.
                    if (LivestreamBox.IsChecked.Value)
                    {
                        youtubedl.StartInfo.CreateNoWindow = false;
                    }
                    if (TwoFactorBox.IsChecked.Value)
                    {
                        youtubedl.StartInfo.CreateNoWindow = false;
                    }
                    youtubedl.StartInfo.UseShellExecute = false;
                    youtubedl.StartInfo.FileName = DownloadsFolder.ToString() + "\\youtube-dl.exe";
                    youtubedl.StartInfo.Arguments = DefaultArguments + DownloadURL;

                    //If the Separate Folder box is checked, we need to create this directory and set our current working location to there.
                    //After that we can start the download process
                    if (SeparateFolderBox.IsChecked.Value)
                    {
                        Directory.CreateDirectory(SeparateFolderTextBox.Text.ToString());
                        DownloadsFolder = DownloadsFolder + "\\" + SeparateFolderTextBox.Text.ToString();
                        Directory.SetCurrentDirectory(DownloadsFolder.ToString());
                    }
                    youtubedl.Start();
                }
            }
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
            //Reset the Downloads Folder location, if the user used the separate folder option before using this button, the DownloadsLocation would sitll point to the separatefolder.
            object DownloadsFolder = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads");
            Directory.SetCurrentDirectory(DownloadsFolder.ToString());
            File.Delete("rip.properties");
            File.Delete("history.json");
        }
    }
}