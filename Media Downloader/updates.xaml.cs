using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Net;
using Ionic.Zip;

namespace MediaDownloader
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class updates : UserControl
    {
        public updates()
        {
            InitializeComponent();
            Loaded += update_Loaded;
        }

        //I don't feel like placing comments in this file yet.
        //Will do it someday later.
        //I also need to clean up this section, probably.

        WebClient WebClient = new WebClient();
        public string YouTubeDLPath = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads\\youtube-dl.exe");
        public string DownloadsFolder = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads");
        public string RipMePath = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads\\ripme.jar");
        public string ffmpegPath = "C:\\ffmpeg\\bin\\ffmpeg.exe";
        public string RTMPDumpPath = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders").GetValue("{374DE290-123F-4565-9164-39C4925E467B}") + ("\\Media Downloads\\rtmpdump.exe");
        public string LatestFFmpegVersion;
        public string CurrentFFmpegVersion;

 

        private void update_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= update_Loaded;
            if (!File.Exists(YouTubeDLPath))
            {
                CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + "youtube-dl.exe not found";
                string LatestYoutubeDLVersion = WebClient.DownloadString("https://yt-dl.org/latest/version");

                LatestYouTubeDLVersionText.Text = "Latest youtube-dl.exe version: " + LatestYoutubeDLVersion;
                UpdateYouTubeDL.IsEnabled = true;
                YouTubeDLVersionStatusText.Text = "youtube-dl.exe can not be found, please click the button below.";
            }
            else {

                Directory.SetCurrentDirectory(DownloadsFolder);

                //Here I get the current version of youtube-dl.exe, to get the version number, we have to run youtube-dl.exe --version
                Process youtubedl = new Process();
                youtubedl.StartInfo.CreateNoWindow = true;
                youtubedl.StartInfo.UseShellExecute = false;
                youtubedl.StartInfo.RedirectStandardOutput = true;
                youtubedl.StartInfo.RedirectStandardError = true;
                youtubedl.StartInfo.FileName = DownloadsFolder + "\\youtube-dl.exe";
                youtubedl.StartInfo.Arguments = " --version";
                youtubedl.Start();
                string CurrentYouTubeDLVersion = youtubedl.StandardOutput.ReadToEnd();
                CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + CurrentYouTubeDLVersion;

                //We also need to get the latest version, so we can compare the latest version with the current version
                string LatestYoutubeDLVersion = WebClient.DownloadString("https://yt-dl.org/latest/version");
                LatestYouTubeDLVersionText.Text = "Latest youtube-dl.exe version: " + LatestYoutubeDLVersion;

                int YouTubeDLUptodate = CurrentYouTubeDLVersion.CompareTo(LatestYoutubeDLVersion);

                // MessageBox.Show(YouTubeDLUptodate.ToString());
                // YouTubeDLUptodate = 1 means that the version is up to date
                // YouTubeDLUptodate = -1 means that the version is out of date
                if (YouTubeDLUptodate < 1)
                {
                    YouTubeDLVersionStatusText.Text = "Your youtube-dl.exe is out of date, please click the button below to update.";
                    UpdateYouTubeDL.IsEnabled = true;
                }
                else
                {
                    YouTubeDLVersionStatusText.Text = "youtube-dl.exe is up to date!";
                }
            }

            if (!File.Exists(RipMePath))
            {

                CurrentRipMeVersionText.Text = "Current RipMe version: " + "ripme.jar not found";
                WebClient.DownloadFile("http://www.rarchives.com/ripme.json", DownloadsFolder + "\\latestripme.txt");
                string LatestRipMeVersion = File.ReadLines("latestripme.txt").Skip(1).First();
                File.Delete("latestripme.txt");
                char[] LatestRipMeTrim = { ' ', ' ', '"', 'l', 'a', 't', 'e', 's', 't', 'V', 'e', 'r', 's', 'i', 'o', 'n', '"', ' ', ':', ' ', '"', '"', ',' };
                LatestRipMeVersion = LatestRipMeVersion.Trim(LatestRipMeTrim);
                LatestRipMeVersionText.Text = "Latest RipMe version: " + LatestRipMeVersion;
                UpdateRipMe.IsEnabled = true;
                RipMeVersionStatusText.Text = "ripme.jar can not be found, please click the button below.";
            }
            else {
                Directory.SetCurrentDirectory(DownloadsFolder);
                Process ripme = new Process();
                ripme.StartInfo.CreateNoWindow = true;
                ripme.StartInfo.UseShellExecute = false;
                ripme.StartInfo.RedirectStandardOutput = true;
                ripme.StartInfo.RedirectStandardError = true;
                ripme.StartInfo.FileName = "java";
                ripme.StartInfo.Arguments = " -jar \"" + DownloadsFolder + "\\ripme.jar\" --help";
                ripme.Start();

                //Getting the current RipMe version
                File.WriteAllText("currentripme.txt", ripme.StandardOutput.ReadToEnd());
                string CurrentRipMeVersion = File.ReadLines("currentripme.txt").Skip(2).First();
                File.Delete("currentripme.txt");
                char[] CurrentRipMeTrim = { 'I', 'n', 'i', 't', 'i', 'a', 'l', 'i', 'z', 'e', 'd', ' ', 'r', 'i', 'p', 'm', 'e', ' ', 'v' };
                CurrentRipMeVersion = CurrentRipMeVersion.Trim(CurrentRipMeTrim);
                CurrentRipMeVersionText.Text = "Current RipMe version: " + CurrentRipMeVersion;

                //Getting the latest RipMe Version
                WebClient.DownloadFile("http://www.rarchives.com/ripme.json", DownloadsFolder + "\\latestripme.txt");
                string LatestRipMeVersion = File.ReadLines("latestripme.txt").Skip(1).First();
                File.Delete("latestripme.txt");
                char[] LatestRipMeTrim = { ' ', ' ', '"', 'l', 'a', 't', 'e', 's', 't', 'V', 'e', 'r', 's', 'i', 'o', 'n', '"', ' ', ':', ' ', '"', '"', ',' };
                LatestRipMeVersion = LatestRipMeVersion.Trim(LatestRipMeTrim);
                LatestRipMeVersionText.Text = "Latest RipMe version: " + LatestRipMeVersion;

                int RipMeUptodate = CurrentRipMeVersion.CompareTo(LatestRipMeVersion);
                if (RipMeUptodate < 0)
                {
                    RipMeVersionStatusText.Text = "Your RipMe is out of date, please click the button below to update.";
                    UpdateRipMe.IsEnabled = true;
                }
                else
                {
                    RipMeVersionStatusText.Text = "RipMe is up to date!";
                }
            }
            if (!File.Exists(ffmpegPath))
            {
                CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + "FFmpeg not found";

                Process GetFFmpeg = new Process();
                GetFFmpeg.StartInfo.CreateNoWindow = true;
                GetFFmpeg.StartInfo.UseShellExecute = false;
                File.WriteAllText(DownloadsFolder + "\\Getffmpeg.ps1",
                 @"$DownloadsLocation = (Get-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders').'{374DE290-123F-4565-9164-39C4925E467B}'
$DownloadsLocation = ""$DownloadsLocation\Media Downloads""
Set-Location $DownloadsLocation
$FFmpegURL = Invoke-WebRequest ""http://ffmpeg.zeranoe.com/builds/win64/static/""
($FFmpegURL.ParsedHTML.getElementsByTagName(""div"") | Where {$_.className -eq 'container'}).innerText | Out-File ""$DownloadsLocation\LatestFFmpeg.txt""
$LatestFFmpeg = Get-Content ""$DownloadsLocation\LatestFFmpeg.txt"" | Select -Skip 2 -First 1
$LatestFFmpeg = $LatestFFmpeg.Trim(""      ffmpeg-"")
$LatestFFmpeg = $LatestFFmpeg.Substring(0, $LatestFFmpeg.IndexOf('-'))
$LatestFFmpeg | Out-File LastFFmpeg.txt");
                GetFFmpeg.StartInfo.FileName = "powershell.exe";
                GetFFmpeg.StartInfo.Arguments = " -exec bypass -File \"" + DownloadsFolder + "\\Getffmpeg.ps1\"";
                GetFFmpeg.Start();
                GetFFmpeg.WaitForExit();
                string LatestFFmpegVersion = File.ReadAllText(@"LastFFmpeg.txt");

                try
                {
                    File.Delete(DownloadsFolder + "\\LatestFFmpeg.txt");
                    File.Delete(DownloadsFolder + "\\LastFFmpeg.txt");
                    File.Delete(DownloadsFolder + "\\Getffmpeg.ps1");
                }
                catch (Exception)
                {
                    MessageBox.Show("Something happend");
                }
                LatestFFmpegVersionText.Text = "Latest FFmpeg version: " + LatestFFmpegVersion;

                UpdateFFmpeg.IsEnabled = true;
                FFmpegVersionStatusText.Text = "FFmpeg can not be found, please click the button below.";
            }
            else {
                if (File.Exists("C:\\ffmpeg\\bin\\version.txt"))
                {
                    string CurrentFFmpegVersion = File.ReadLines(@"C:\\ffmpeg\\bin\\version.txt").First();
                    CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + CurrentFFmpegVersion;
                    Process GetFFmpeg = new Process();
                    GetFFmpeg.StartInfo.CreateNoWindow = true;
                    GetFFmpeg.StartInfo.UseShellExecute = false;
                    File.WriteAllText(DownloadsFolder + "\\Getffmpeg.ps1",
                     @"$DownloadsLocation = (Get-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders').'{374DE290-123F-4565-9164-39C4925E467B}'
$DownloadsLocation = ""$DownloadsLocation\Media Downloads""
Set-Location $DownloadsLocation
$FFmpegURL = Invoke-WebRequest ""http://ffmpeg.zeranoe.com/builds/win64/static/""
($FFmpegURL.ParsedHTML.getElementsByTagName(""div"") | Where {$_.className -eq 'container'}).innerText | Out-File ""$DownloadsLocation\LatestFFmpeg.txt""
$LatestFFmpeg = Get-Content ""$DownloadsLocation\LatestFFmpeg.txt"" | Select -Skip 2 -First 1
$LatestFFmpeg = $LatestFFmpeg.Trim(""      ffmpeg-"")
$LatestFFmpeg = $LatestFFmpeg.Substring(0, $LatestFFmpeg.IndexOf('-'))
$LatestFFmpeg | Out-File LastFFmpeg.txt");
                    GetFFmpeg.StartInfo.FileName = "powershell.exe";
                    GetFFmpeg.StartInfo.Arguments = " -exec bypass -File \"" + DownloadsFolder + "\\Getffmpeg.ps1\"";
                    GetFFmpeg.Start();
                    GetFFmpeg.WaitForExit();
                    string LatestFFmpegVersion = File.ReadLines(@"LastFFmpeg.txt").First();

                    try
                    {
                        File.Delete(DownloadsFolder + "\\LatestFFmpeg.txt");
                        File.Delete(DownloadsFolder + "\\LastFFmpeg.txt");
                        File.Delete(DownloadsFolder + "\\Getffmpeg.ps1");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Something happend");
                    }
                    LatestFFmpegVersionText.Text = "Latest FFmpeg version: " + LatestFFmpegVersion;

                    int FFmpegUptodate = CurrentFFmpegVersion.CompareTo(LatestFFmpegVersion);
                    //Higher version = 1
                    //Outdated = -1 
                    //Same version = 0
                    if (FFmpegUptodate == -1)
                    {
                        FFmpegVersionStatusText.Text = "Your FFmpeg is out of date, please click the button below to update.";
                        UpdateFFmpeg.IsEnabled = true;
                    }
                    else
                    {
                        FFmpegVersionStatusText.Text = "FFmpeg is up to date!";

                    }
                }
                else
                {
                    Process GetFFmpeg = new Process();
                    GetFFmpeg.StartInfo.CreateNoWindow = true;
                    GetFFmpeg.StartInfo.UseShellExecute = false;
                    File.WriteAllText(DownloadsFolder + "\\Getffmpeg.ps1",
                     @"$DownloadsLocation = (Get-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders').'{374DE290-123F-4565-9164-39C4925E467B}'
$DownloadsLocation = ""$DownloadsLocation\Media Downloads""
Set-Location $DownloadsLocation
$FFmpegURL = Invoke-WebRequest ""http://ffmpeg.zeranoe.com/builds/win64/static/""
($FFmpegURL.ParsedHTML.getElementsByTagName(""div"") | Where {$_.className -eq 'container'}).innerText | Out-File ""$DownloadsLocation\LatestFFmpeg.txt""
$LatestFFmpeg = Get-Content ""$DownloadsLocation\LatestFFmpeg.txt"" | Select -Skip 2 -First 1
$LatestFFmpeg = $LatestFFmpeg.Trim(""      ffmpeg-"")
$LatestFFmpeg = $LatestFFmpeg.Substring(0, $LatestFFmpeg.IndexOf('-'))
$LatestFFmpeg | Out-File LastFFmpeg.txt");
                    GetFFmpeg.StartInfo.FileName = "powershell.exe";
                    GetFFmpeg.StartInfo.Arguments = " -exec bypass -File \"" + DownloadsFolder + "\\Getffmpeg.ps1\"";
                    GetFFmpeg.Start();
                    GetFFmpeg.WaitForExit();
                    string LatestFFmpegVersion = File.ReadAllText(@"LastFFmpeg.txt");

                    try
                    {
                        File.Delete(DownloadsFolder + "\\LatestFFmpeg.txt");
                        File.Delete(DownloadsFolder + "\\LastFFmpeg.txt");
                        File.Delete(DownloadsFolder + "\\Getffmpeg.ps1");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Something happend");
                    }
                    CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + "Error getting current version.";
                    LatestFFmpegVersionText.Text = "Latest FFmpeg version: " + LatestFFmpegVersion;
                    FFmpegVersionStatusText.Text = "Error getting version, please update.";
                    UpdateFFmpeg.IsEnabled = true;
                }

            }
            if (!File.Exists(RTMPDumpPath))
            {
                DownloadRTMPDump.IsEnabled = true;
            }
        }



        private void UpdateYouTubeDL_Click(object sender, RoutedEventArgs e)
        { 
            WebClient.DownloadFile("https://yt-dl.org/downloads/latest/youtube-dl.exe", DownloadsFolder + "\\youtube-dl.exe");
            Directory.SetCurrentDirectory(DownloadsFolder);
            Process youtubedl = new Process();
            youtubedl.StartInfo.CreateNoWindow = true;
            youtubedl.StartInfo.UseShellExecute = false;
            youtubedl.StartInfo.RedirectStandardOutput = true;
            youtubedl.StartInfo.RedirectStandardError = true;
            youtubedl.StartInfo.FileName = DownloadsFolder + "\\youtube-dl.exe";
            youtubedl.StartInfo.Arguments = " --version";
            youtubedl.Start();
            string CurrentYouTubeDLVersion = youtubedl.StandardOutput.ReadToEnd();
            CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + CurrentYouTubeDLVersion;
            UpdateYouTubeDL.IsEnabled = false;
            YouTubeDLVersionStatusText.Text = "Updated youtube-dl.exe";
            
        }

        private void UpdateRipMe_Click(object sender, RoutedEventArgs e)
        {
            WebClient.DownloadFile("http://rarchives.com/ripme.jar", DownloadsFolder + "\\ripme.jar");
            Directory.SetCurrentDirectory(DownloadsFolder);
            Process ripme = new Process();
            ripme.StartInfo.CreateNoWindow = true;
            ripme.StartInfo.UseShellExecute = false;
            ripme.StartInfo.RedirectStandardOutput = true;
            ripme.StartInfo.RedirectStandardError = true;
            ripme.StartInfo.FileName = "java";
            ripme.StartInfo.Arguments = " -jar \"" + DownloadsFolder + "\\ripme.jar\" --help";
            ripme.Start();
            File.WriteAllText("currentripme.txt", ripme.StandardOutput.ReadToEnd());
            string CurrentRipMeVersion = File.ReadLines("currentripme.txt").Skip(2).First();
            File.Delete("currentripme.txt");
            char[] CurrentRipMeTrim = { 'I', 'n', 'i', 't', 'i', 'a', 'l', 'i', 'z', 'e', 'd', ' ', 'r', 'i', 'p', 'm', 'e', ' ', 'v' };
            CurrentRipMeVersion = CurrentRipMeVersion.Trim(CurrentRipMeTrim);
            CurrentRipMeVersionText.Text = "Current RipMe version: " + CurrentRipMeVersion;
            UpdateRipMe.IsEnabled = false;
            RipMeVersionStatusText.Text = "Updated RipMe";
        }

        private void UpdateFFmpeg_Click(object sender, RoutedEventArgs e)
        {
            try {
            Directory.Delete("C:\\ffmpeg", true);
            }
            catch (Exception)
            {
                MessageBox.Show(@"Can not delete C:\\ffmpeg.
You might still be downloading something.
FFmpeg is still in use by another process.
Updating might fail.");
            }
            WebClient.DownloadFile("http://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.7z", DownloadsFolder + "\\ffmpeg.7z");
            WebClient.DownloadFile("http://www.7-zip.org/a/7za920.zip", DownloadsFolder + "\\7za.zip");
            ZipFile zip = ZipFile.Read(DownloadsFolder + "\\7za.zip");
            ZipEntry SevenZip = zip["7za.exe"];
            SevenZip.Extract(DownloadsFolder);
            zip.Dispose();
            Process SevenZipExtract = new Process();
            SevenZipExtract.StartInfo.CreateNoWindow = true;
            SevenZipExtract.StartInfo.UseShellExecute = false;
            SevenZipExtract.StartInfo.FileName = "7za.exe";
            SevenZipExtract.StartInfo.Arguments = " x -o\"C:\\ffmpeg\" ffmpeg.7z";
            SevenZipExtract.Start();
            SevenZipExtract.WaitForExit();
            File.Delete(DownloadsFolder + "\\7za.exe");

            Process UpdateFFmpegProcess = new Process();
            UpdateFFmpegProcess.StartInfo.CreateNoWindow = true;
            UpdateFFmpegProcess.StartInfo.UseShellExecute = false;
            File.WriteAllText(DownloadsFolder + "\\UpdateFFmpeg.ps1",
                @"Move-Item C:\ffmpeg\ffmpeg-*\* C:\ffmpeg
            Remove-Item -Force -Confirm:$false -Recurse C:\ffmpeg\ffmpeg-*");
            UpdateFFmpegProcess.StartInfo.FileName = "powershell.exe";
            UpdateFFmpegProcess.StartInfo.Arguments = " -exec bypass -File \"" + DownloadsFolder + "\\UpdateFFmpeg.ps1\"";
            UpdateFFmpegProcess.Start();
            UpdateFFmpegProcess.WaitForExit();

            Process GetFFmpeg = new Process();
            GetFFmpeg.StartInfo.CreateNoWindow = true;
            GetFFmpeg.StartInfo.UseShellExecute = false;
            File.WriteAllText(DownloadsFolder + "\\GetFFmpeg.ps1",
             @"$DownloadsLocation = (Get-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders').'{374DE290-123F-4565-9164-39C4925E467B}'
$DownloadsLocation = ""$DownloadsLocation\Media Downloads""
Set-Location $DownloadsLocation
$FFmpegURL = Invoke-WebRequest ""http://ffmpeg.zeranoe.com/builds/win64/static/""
($FFmpegURL.ParsedHTML.getElementsByTagName(""div"") | Where {$_.className -eq 'container'}).innerText | Out-File ""$DownloadsLocation\LatestFFmpeg.txt""
$LatestFFmpeg = Get-Content ""$DownloadsLocation\LatestFFmpeg.txt"" | Select -Skip 2 -First 1
$LatestFFmpeg = $LatestFFmpeg.Trim(""      ffmpeg-"")
$LatestFFmpeg = $LatestFFmpeg.Substring(0, $LatestFFmpeg.IndexOf('-'))
$LatestFFmpeg | Out-File LastFFmpeg.txt");
            GetFFmpeg.StartInfo.FileName = "powershell.exe";
            GetFFmpeg.StartInfo.Arguments = " -exec bypass -File \"" + DownloadsFolder + "\\GetFFmpeg.ps1\"";
            GetFFmpeg.Start();
            GetFFmpeg.WaitForExit();
            string LatestFFmpegVersion = File.ReadLines(@"LastFFmpeg.txt").First();
            File.WriteAllText(@"C:\\ffmpeg\\bin\\version.txt", LatestFFmpegVersion);
            string CurrentFFmpegVersion = File.ReadLines(@"C:\\ffmpeg\\bin\\version.txt").First();
            CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + CurrentFFmpegVersion;
            UpdateFFmpeg.IsEnabled = false;
            FFmpegVersionStatusText.Text = "Updated FFmpeg.";
            File.Delete(DownloadsFolder + "\\GetFFmpeg.ps1");
            File.Delete(DownloadsFolder + "\\UpdateFFmpeg.ps1");
            File.Delete(DownloadsFolder + "\\LastFFmpeg.txt");
            File.Delete(DownloadsFolder + "\\LatestFFmpeg.txt");
            File.Delete(DownloadsFolder + "\\7za.zip");
            File.Delete(DownloadsFolder + "\\ffmpeg.7z");
        }

        private void DownloadRTMPDump_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(DownloadsFolder + "\\rtmpdump.exe");
            WebClient.DownloadFile("https://rtmpdump.mplayerhq.hu/download/rtmpdump-2.4-git-010913-windows.zip", DownloadsFolder + "\\rtmpdump.zip");
            ZipFile zip = ZipFile.Read(DownloadsFolder + "\\rtmpdump.zip");
            ZipEntry rtmpdumpZip = zip["rtmpdump.exe"];
            rtmpdumpZip.Extract(DownloadsFolder);
            zip.Dispose();
            File.Delete(DownloadsFolder + "\\rtmpdump.zip");
            DownloadRTMPDump.IsEnabled = false;
        }
    }
}
