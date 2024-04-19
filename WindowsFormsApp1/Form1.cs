using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.Win32;

namespace winSecure
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private bool isCameraAvailable = false;
        private bool isImageCaptured = false;

        public Form1()
        {
            InitializeComponent();
            StartMonitoring();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
         //
        }

        private void StartMonitoring()
        {
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count > 0)
            {
                isCameraAvailable = true;
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += videoSource_NewFrame;
                videoSource.Start();
            }
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock && isCameraAvailable && !isImageCaptured)
            {
                CaptureAndSaveImage();
            }
        }

        private void CaptureAndSaveImage()
        {
            try
            {
                isImageCaptured = true;

                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "Screenshots");
                Directory.CreateDirectory(folderPath);

                string randomFileName = Path.GetRandomFileName();
                string fileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{randomFileName}.jpg";
                string filePath = Path.Combine(folderPath, fileName);

                pictureBox1.Image.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                isImageCaptured = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while capturing and saving the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                isImageCaptured = false;
            }
        }

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }
    }
}
