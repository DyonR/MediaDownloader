using System;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Net;
using Ionic.Zip;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace MediaDownloader
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class updates
    {
        public updates()
        {
            InitializeComponent();
            Loaded += update_Loaded;
        }

        public string LatestYoutubeDLVersion;
        public string CurrentYouTubeDLVersion;
        public string LatestFFmpegVersion;
        public string CurrentFFmpegVersion;
        public string LatestRipMeVersion;
        public string CurrentRipMeVersion;

        public bool checkforUpdates = false;
        public bool youtubedlUpdating = false;
        public bool ripmeUpdating = false;
        public bool ffmpegUpdating = false;
        public bool rtmpdumpUpdating = false;

        BackgroundWorker youtubedlWorker;
        BackgroundWorker youtubedlUpdateWorker;
        BackgroundWorker ripmeWorker;
        BackgroundWorker ripmeUpdateWorker;
        BackgroundWorker ffmpegWorker;
        BackgroundWorker ffmpegUpdateWorker;
        BackgroundWorker RTMPDumpWorker;
        BackgroundWorker RTMPDumpUpdateWorker;

        WebClient Clientyoutubedl = new WebClient();
        WebClient ClientRipMe = new WebClient();
        WebClient ClientFFmpeg = new WebClient();
        WebClient ClientRTMPDump = new WebClient();

        List<string> downloadLinks = new List<string>();

        private void update_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= update_Loaded;

            youtubedlWorker = new BackgroundWorker {WorkerReportsProgress = true};
            youtubedlWorker.DoWork += (obj, ea) => youtubedlGetCurrentVersion_Process();
            youtubedlWorker.RunWorkerAsync();

            ffmpegWorker = new BackgroundWorker {WorkerReportsProgress = true};
            ffmpegWorker.DoWork += (obj, ea) => ffmpegGetCurrentVersion_Process();
            ffmpegWorker.RunWorkerAsync();

            ripmeWorker = new BackgroundWorker {WorkerReportsProgress = true};
            ripmeWorker.DoWork += (obj, ea) => RipMeGetCurrentVersion_Process();
            ripmeWorker.RunWorkerAsync();

            RTMPDumpWorker = new BackgroundWorker {WorkerReportsProgress = true};
            RTMPDumpWorker.DoWork += (obj, ea) => RTMPDumpCompareVersion_Process();
            RTMPDumpWorker.RunWorkerAsync();
        }
        private void UpdateYouTubeDL_Click(object sender, RoutedEventArgs e)
        {
            if (checkforUpdates == false)
            {
                youtubedlUpdateWorker = new BackgroundWorker {WorkerReportsProgress = true};
                youtubedlUpdateWorker.DoWork += (obj, ea) => youtubedlInstallLastestVersion_Process();
                youtubedlUpdateWorker.RunWorkerAsync(youtubedlUpdating = true);
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRipMe_Click(object sender, RoutedEventArgs e)
        {
            if (checkforUpdates == false)
            {
                ripmeUpdateWorker = new BackgroundWorker {WorkerReportsProgress = true};
                ripmeUpdateWorker.DoWork += (obj, ea) => RipMeInstallLastestVersion_Process();
                ripmeUpdateWorker.RunWorkerAsync(ripmeUpdating = true);
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateFFmpeg_Click(object sender, RoutedEventArgs e)
        {
            if (checkforUpdates == false)
            {
                ffmpegUpdateWorker = new BackgroundWorker {WorkerReportsProgress = true};
                ffmpegUpdateWorker.DoWork += (obj, ea) => ffmpegInstallLastestVersion_Process();
                ffmpegUpdateWorker.RunWorkerAsync(ffmpegUpdating = true);
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DownloadRTMPDump_Click(object sender, RoutedEventArgs e)
        {
            if (checkforUpdates == false)
            {
                RTMPDumpUpdateWorker = new BackgroundWorker();
                RTMPDumpUpdateWorker.WorkerReportsProgress = true;
                RTMPDumpUpdateWorker.DoWork += (obj, ea) => RTMPDumpInstallLastestVersion_Process();
                RTMPDumpUpdateWorker.RunWorkerAsync(rtmpdumpUpdating = true);
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void youtubedlGetCurrentVersion_Process()
        {
            if (File.Exists(youtubedl.YouTubeDLPath))
            {
                //Here I get the current version of youtube-dl.exe, to get the version number, we have to run youtube-dl.exe --version
                var youtubedl = new Process
                {
                    StartInfo =
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        FileName = MediaDownloader.youtubedl.YouTubeDLPath,
                        Arguments = " --version"
                    }
                };
                youtubedl.Start();
                CurrentYouTubeDLVersion = youtubedl.StandardOutput.ReadToEnd();
                Dispatcher.Invoke(() =>
                {
                    CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: " + CurrentYouTubeDLVersion;
                    YouTubeDLVersionStatusText.Text = null;
                    UpdateYouTubeDL.IsEnabled = false;
                });

            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    CurrentYouTubeDLVersionText.Text = "Current youtube-dl.exe version: youtube-dl.exe not found.";
                    YouTubeDLVersionStatusText.Text = "youtube-dl.exe can not be found, please click the button below.";
                    UpdateYouTubeDL.Content = "Update youtube-dl";
                    UpdateYouTubeDL.IsEnabled = true;
                });
            }
        }
        private void youtubedlGetLatestVersion_Process()
        {
            LatestYoutubeDLVersion = Clientyoutubedl.DownloadString("https://yt-dl.org/latest/version");
            Dispatcher.Invoke(() =>
            {
                LatestYouTubeDLVersionText.Text = "Latest youtube-dl.exe version: " + LatestYoutubeDLVersion;
            });
        }
        private void youtubedlCompareVersion_Process()
        {
            youtubedlGetCurrentVersion_Process();
            youtubedlGetLatestVersion_Process();
            if (File.Exists(youtubedl.YouTubeDLPath))
            {
                var YouTubeDLUptodate = CurrentYouTubeDLVersion.CompareTo(LatestYoutubeDLVersion);
                if (YouTubeDLUptodate < 1)
                {
                    Dispatcher.Invoke(() =>
                    {
                        YouTubeDLVersionStatusText.Text = "Your youtube-dl.exe is out of date, please click the button below to update.";
                        UpdateYouTubeDL.Content = "Update youtube-dl";
                        UpdateYouTubeDL.IsEnabled = true;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        YouTubeDLVersionStatusText.Text = "youtube-dl.exe is up to date!";
                        UpdateYouTubeDL.IsEnabled = false;
                    });
                }
            }
        }
        private void youtubedlInstallLastestVersion_Process()
        {
            Dispatcher.Invoke(() =>
            {
                YouTubeDLVersionStatusText.Text = null;
                UpdateYouTubeDL.Content = "Getting latest version...";
                UpdateYouTubeDL.IsEnabled = false;
            });
            youtubedlGetLatestVersion_Process();
            Dispatcher.Invoke(() =>
            {
                UpdateYouTubeDL.Content = "Downloading youtube-dl...";
            });
            Clientyoutubedl.DownloadFile("https://yt-dl.org/downloads/latest/youtube-dl.exe", youtubedl.YouTubeDLPath);
            Dispatcher.Invoke(() =>
            {
                UpdateYouTubeDL.Content = "Getting current version...";
            });
            youtubedlGetCurrentVersion_Process();
            Dispatcher.Invoke(() =>
            {
                UpdateYouTubeDL.Content = "Update finished";
            });
            youtubedlUpdating = false;
        }
        private void RipMeGetCurrentVersion_Process()
        {
            if (File.Exists(youtubedl.RipMePath))
            {
                Directory.SetCurrentDirectory(youtubedl.LocalStorageFolder);
                var ripme = new Process
                {
                    StartInfo =
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        FileName = "java",
                        Arguments = " -jar \"" + youtubedl.RipMePath + "\" --help"
                    }
                };
                ripme.Start();

                //Getting the current RipMe version
                CurrentRipMeVersion = ripme.StandardOutput.ReadToEnd()
                    .Split(Environment.NewLine.ToCharArray())[4]
                    .Replace("Initialized ripme v", "");

                Dispatcher.Invoke(() =>
                {
                    CurrentRipMeVersionText.Text = "Current RipMe version: " + CurrentRipMeVersion;
                    RipMeVersionStatusText.Text = null;
                    UpdateRipMe.IsEnabled = false;
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    CurrentRipMeVersionText.Text = "Current RipMe version: ripme.jar not found";
                    RipMeVersionStatusText.Text = "ripme.jar can not be found, please click the button below.";
                    UpdateRipMe.Content = "Update RipMe";
                    UpdateRipMe.IsEnabled = true;
                });
            }
        }
        private void RipMeGetLatestVersion_Process()
        {
            var LatestRipMeJson = JObject.Parse(ClientRipMe.DownloadString("https://raw.githubusercontent.com/4pr0n/ripme/master/ripme.json"));
            LatestRipMeVersion = (string)LatestRipMeJson["latestVersion"];
            Dispatcher.Invoke(() =>
           {
               LatestRipMeVersionText.Text = "Latest RipMe version: " + LatestRipMeVersion;
           });
        }
        private void RipMeCompareVersion_Process()
        {
            RipMeGetCurrentVersion_Process();
            RipMeGetLatestVersion_Process();
            if (File.Exists(youtubedl.RipMePath))
            {
                //Higher version than newest = 1
                //Outdated = -1
                //Same version = 0
                var RipMeUptodate = CurrentRipMeVersion.CompareTo(LatestRipMeVersion);
                if (RipMeUptodate < 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RipMeVersionStatusText.Text = "Your RipMe is out of date, please click the button below to update.";
                        UpdateRipMe.Content = "Update RipMe";
                        UpdateRipMe.IsEnabled = true;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        RipMeVersionStatusText.Text = "RipMe is up to date!";
                        UpdateRipMe.IsEnabled = false;
                    });
                }
            }
        }
        private void RipMeInstallLastestVersion_Process()
        {
            Dispatcher.Invoke(() =>
            {
                RipMeVersionStatusText.Text = null;
                UpdateRipMe.Content = "Getting latest version...";
                UpdateRipMe.IsEnabled = false;
            });
            RipMeGetLatestVersion_Process();
            Dispatcher.Invoke(() =>
            {
                UpdateRipMe.Content = "Downloading RipMe...";
            });
            ClientRipMe.DownloadFile("https://github.com/4pr0n/ripme/releases/download/1.4.1/ripme.jar", youtubedl.RipMePath);
            Dispatcher.Invoke(() =>
            {
                UpdateRipMe.Content = "Getting current version...";
            });
            RipMeGetCurrentVersion_Process();
            Dispatcher.Invoke(() =>
            {
                UpdateRipMe.Content = "Update finished";
            });
            ripmeUpdating = false;
        }
        private void ffmpegGetCurrentVersion_Process()
        {
            if (File.Exists(youtubedl.ffmpegfolderPath + "\\bin\\version.txt"))
            {
                CurrentFFmpegVersion = File.ReadLines(youtubedl.ffmpegfolderPath + "\\bin\\version.txt").First();
                Dispatcher.Invoke(() =>
                {
                    CurrentFFmpegVersionText.Text = "Current FFmpeg version: " + CurrentFFmpegVersion;
                    FFmpegVersionStatusText.Text = null;
                    UpdateFFmpeg.IsEnabled = false;
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    CurrentFFmpegVersionText.Text = "Current FFmpeg version: FFmpeg not found.";
                    FFmpegVersionStatusText.Text = "ffmpeg can not be found, please click the button below.";
                    UpdateFFmpeg.Content = "Update FFmpeg";
                    UpdateFFmpeg.IsEnabled = true;
                });
            }
        }
        private void ffmpegGetLatestVersion_Process()
        {
            try
            {
                var ffmpegHtmlDocument = new HtmlDocument();
                ffmpegHtmlDocument.LoadHtml(ClientFFmpeg.DownloadString("http://ffmpeg.zeranoe.com/builds/win64/static/"));

                foreach (var metaTag in ffmpegHtmlDocument.DocumentNode.SelectNodes("//a[@href]"))
                {
                    var hrefValue = metaTag.GetAttributeValue("href", string.Empty);
                    downloadLinks.Add(hrefValue);
                }
                LatestFFmpegVersion = downloadLinks[7];
                LatestFFmpegVersion = LatestFFmpegVersion.Trim("ffmpeg-".ToCharArray());
                LatestFFmpegVersion = LatestFFmpegVersion.Substring(0, LatestFFmpegVersion.IndexOf('-'));
                Dispatcher.Invoke(() =>
                {
                    LatestFFmpegVersionText.Text = "Latest FFmpeg version: " + LatestFFmpegVersion;
                });
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to get latetst FFmpeg version");
            }
        }
        private void ffmpegCompareVersion_Process()
        {
            ffmpegGetCurrentVersion_Process();
            ffmpegGetLatestVersion_Process();
            if (File.Exists(youtubedl.ffmpegfolderPath + "\\bin\\version.txt"))
            {
                var FFmpegUptodate = CurrentFFmpegVersion.CompareTo(LatestFFmpegVersion);
                //Higher version = 1
                //Outdated = -1 
                //Same version = 0
                if (FFmpegUptodate == -1)
                {
                    Dispatcher.Invoke(() =>
                    {
                        FFmpegVersionStatusText.Text = "Your FFmpeg is out of date, please click the button below to update.";
                        UpdateFFmpeg.Content = "Update FFmpeg";
                        UpdateFFmpeg.IsEnabled = true;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        FFmpegVersionStatusText.Text = "FFmpeg is up to date!";
                        UpdateFFmpeg.IsEnabled = false;
                    });
                }
            }
        }
        private void ffmpegInstallLastestVersion_Process()
        {
            Dispatcher.Invoke(() =>
            {
                FFmpegVersionStatusText.Text = null;
                UpdateFFmpeg.Content = "Getting latest version...";
                UpdateFFmpeg.IsEnabled = false;
            });
            ffmpegGetLatestVersion_Process();
            try
            {
                Directory.Delete(youtubedl.ffmpegfolderPath, true);
            }
            catch (Exception)
            {
                if (Directory.Exists(youtubedl.ffmpegfolderPath))
                {
                    var errorMessage = new StringBuilder();
                    errorMessage.AppendLine("Can not delete " + youtubedl.ffmpegfolderPath + ".");
                    errorMessage.AppendLine("If you never installed FFmpeg, you can ignore this.");
                    errorMessage.AppendLine("Updating might fail.");
                    MessageBox.Show(errorMessage.ToString());
                }
            }
            Dispatcher.Invoke(() =>
            {
                UpdateFFmpeg.Content = "Downloading FFmpeg...";
            });
            ClientFFmpeg.DownloadFile("http://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.zip", youtubedl.LocalStorageFolder + "\\ffmpeg.zip");
            var zip = ZipFile.Read(youtubedl.LocalStorageFolder + "\\ffmpeg.zip");
            foreach (var file in zip)
            {
                file.Extract(youtubedl.LocalStorageFolder);
            }
            zip.Dispose();
            Dispatcher.Invoke(() =>
            {
                UpdateFFmpeg.Content = "Moving files...";
            });
            Directory.Move(youtubedl.LocalStorageFolder + "\\ffmpeg-latest-win64-static", youtubedl.LocalStorageFolder + "\\ffmpeg");

            try { File.Delete(youtubedl.LocalStorageFolder + "\\ffmpeg.zip"); }
            catch { MessageBox.Show("Unable to delete: " + youtubedl.LocalStorageFolder + "\\ffmpeg.zip"); }


            File.WriteAllText(youtubedl.ffmpegfolderPath + "\\bin\\version.txt", LatestFFmpegVersion);
            Dispatcher.Invoke(() =>
            {
                UpdateFFmpeg.Content = "Getting current version...";
            });
            ffmpegGetCurrentVersion_Process();
            Dispatcher.Invoke(() =>
            {
                UpdateFFmpeg.Content = "Update finished";
            });
            ffmpegUpdating = false;
        }
        private void RTMPDumpInstallLastestVersion_Process()
        {
            Dispatcher.Invoke(() =>
            {
                DownloadRTMPDump.Content = "Downloading RTMPdump..";
                DownloadRTMPDump.IsEnabled = false;
            });
            ClientRTMPDump.DownloadFile("https://rtmpdump.mplayerhq.hu/download/rtmpdump-2.4-git-010913-windows.zip", youtubedl.LocalStorageFolder + "\\rtmpdump.zip");
            var zip = ZipFile.Read(youtubedl.LocalStorageFolder + "\\rtmpdump.zip");
            var rtmpdumpZip = zip["rtmpdump.exe"];
            rtmpdumpZip.Extract(youtubedl.LocalStorageFolder);
            zip.Dispose();
            File.Delete(youtubedl.LocalStorageFolder + "\\rtmpdump.zip");
            youtubedlGetCurrentVersion_Process();
            Dispatcher.Invoke(() =>
            {
                DownloadRTMPDump.Content = "Update finished";
            });
            rtmpdumpUpdating = false;
        }
        private void RTMPDumpCompareVersion_Process()
        {
            if (!File.Exists(youtubedl.RTMPDumpPath))
            {
                Dispatcher.Invoke(() =>
                {
                    DownloadRTMPDump.Content = "Download RTMPDump";
                    DownloadRTMPDump.IsEnabled = true;
                });
            }
        }
        private void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            if (youtubedlUpdating == false && ripmeUpdating == false && ffmpegUpdating == false && rtmpdumpUpdating == false)
            {
                checkforUpdates = true;
                Dispatcher.Invoke(() =>
                {
                    CheckForUpdates.IsEnabled = false;
                });
                youtubedlUpdateWorker = new BackgroundWorker {WorkerReportsProgress = true};
                youtubedlUpdateWorker.DoWork += (obj, ea) => youtubedlCompareVersion_Process();
                youtubedlUpdateWorker.RunWorkerCompleted += youtubedlUpdateWorker_Process_Complete;
                youtubedlUpdateWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Please don't install and check for updates at the same time!", "Check and Install error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void youtubedlUpdateWorker_Process_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            ripmeUpdateWorker = new BackgroundWorker {WorkerReportsProgress = true};
            ripmeUpdateWorker.DoWork += (obj, ea) => RipMeCompareVersion_Process();
            ripmeUpdateWorker.RunWorkerCompleted += ripmeUpdateWorker_Process_Complete;
            ripmeUpdateWorker.RunWorkerAsync();
        }
        private void ripmeUpdateWorker_Process_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            ffmpegUpdateWorker = new BackgroundWorker {WorkerReportsProgress = true};
            ffmpegUpdateWorker.DoWork += (obj, ea) => ffmpegCompareVersion_Process();
            ffmpegUpdateWorker.RunWorkerCompleted += ffmpegUpdateWorker_Process_Complete;
            ffmpegUpdateWorker.RunWorkerAsync();
        }

        private void ffmpegUpdateWorker_Process_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            RTMPDumpUpdateWorker = new BackgroundWorker {WorkerReportsProgress = true};
            RTMPDumpUpdateWorker.DoWork += (obj, ea) => RTMPDumpCompareVersion_Process();
            RTMPDumpUpdateWorker.RunWorkerCompleted += RTMPDumpUpdateWorker_Process_Complete;
            RTMPDumpUpdateWorker.RunWorkerAsync();
        }

        private void RTMPDumpUpdateWorker_Process_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            CheckForUpdates.IsEnabled = true;
            checkforUpdates = false;
        }
    }
}
