using ASSVS.EDR;
using ASSVS.EDR.DistanceAndWrinkleBased;
using ASSVS.EDR.DistancedBased;
using ASSVS.EDR.HybridMethodology;
using ASSVS.EDR.OpticalFlowBased;
using ASSVS.EDR.ReducedDimentions;
using ASSVS.FD;
using CVML.Logger;
using DirectShowLib;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using CVML.Constants;
using CVML.MLL;

namespace ASSVS.UI.Desktop.EDR
{

    public partial class Form2 : Form
    {
        VideoCapture capt = null;
        double FrameRate = 0;
        double TotalFrames = 0;
        // int i = 0;
        // int j = 0;
        int FrameCount;
        private bool captureInProgress;
        public Form2()
        {
            InitializeComponent();
            CvmlUtility.getInstance().setDllSearchDirectory((string)ConstantsLoader.getInstance().getValue(EnumConstant.libsPath));

            EDRIPMain.getInstance().loadPredictor();
            DsDevice[] captureDevices;

            // Get the set of directshow devices that are video inputs.
            captureDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            cameraList.DisplayMember = "Text";
            cameraList.ValueMember = "Value";
            for (int idx = 0; idx < captureDevices.Length; idx++)
            {
                cameraList.Items.Add(new { Text = idx + ":" + captureDevices.ElementAt(idx).Name, Value = idx });// Do something with the device here...
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (Video.Checked)
            {
                capt = new VideoCapture(openFileDialog1.FileName);
                TotalFrames = capt.GetCaptureProperty(CapProp.FrameCount);
                FrameRate = capt.GetCaptureProperty(CapProp.Fps);
                trackBar1.Maximum = (int)TotalFrames;

            }





        }
        int i = 0;
        private void ProcessFrame(object sender, EventArgs arg)
        {


            #region Face Detection Region 
            if (captureInProgress)
            {
                //FileInfo file = new FileInfo("‪E:\\training2\\1.jpg");
                // Mat ImageMat = new Mat(file.FullName);


                Mat ImageMat = capt.QueryFrame();
                imageList.Images.Add(ImageMat.ToImage<Bgr, byte>().ToBitmap());
                for (int j = 0; j < imageList.Images.Count; j++)
                {
                    ListViewItem item = new ListViewItem();
                    item.ImageIndex = j;
                    imageListView.Items.Add(item);
                }
                string emotions = "";
                if (Methodology.SelectedIndex == 1)
                    emotions = EDRIPMainDistanceAndWrinkledBased.getInstance().DetectEmotionsDistanceAndWrinkledBased(new Mat[] { ImageMat }, FDIPMAin.DetectFace(ImageMat)).outputResult;
                else if (Methodology.SelectedIndex == 0)
                {
                    emotions = EDRIPMainDistanceBased.getInstance().DetectEmotionsDistanceBased(new Mat[] { ImageMat }).outputResult;
                }
                if (emotions != null)
                {
                    List<double> dfs = distributionsEmotions.dists;

                    foreach (var series in chart2.Series)
                    {
                        series.Points.Clear();
                    }
                    chart2.Series["Conf"].Points.AddXY("anger", dfs[0]);
                    chart2.Series["Conf"].Points.AddXY("smile", dfs[1]);
                    chart2.Series["Conf"].Points.AddXY("sad", dfs[2]);
                    chart2.Series["Conf"].Points.AddXY("surprise", dfs[3]);
                    chart2.Series["Conf"].Points.AddXY("neutral", dfs[4]);
                    chart2.Series["Conf"].Points.AddXY("fear", dfs[5]);
                    chart2.Series["Conf"].Points.AddXY("disgust", dfs[6]);
                }


                List<Rectangle> faces = new List<Rectangle>();
                faces = FDIPMAin.DetectFace(ImageMat);
                Mat shortImage = faces.Count > 0 ? new Mat(ImageMat, faces[0]) : ImageMat;
                CvInvoke.Resize(shortImage, shortImage, new Size(320, 240), 0, 0, Inter.Linear);
                shortImage = ImageMat;
                //if(faces.Count>0&&i%3==0)
                //{

                //    ImageMat.Save("E:\\dataset\\smile\\" + i/3 + ".jpg");
                //}
                //CvInvoke.Imshow("Orignal Face", shortImage);
                faces = new List<Rectangle>();
                faces.Add(new Rectangle(0, 0, shortImage.Width, shortImage.Height));
                Image<Bgr, byte> I2 = shortImage.ToImage<Bgr, byte>();
                GC.Collect();
                Logger log = new Logger();
                log.LogIntoFile(Log.logType.INFO, "message to be written");
                //I2.Draw(emotions, new Point(10, 30), FontFace.HersheyPlain, 2.0, new Bgr(10, 0, 255));
                imageBox1.Image = I2;
                i++;
            }

            #endregion

            else
                imageBox1.Image = capt.QueryFrame();
            if (FrameCount <= TotalFrames && Video.Checked)
            {
                trackBar1.Value = FrameCount;
                FrameCount++;
            }
            else if (Video.Checked)
            {
                FrameCount = 0;
                trackBar1.Value = FrameCount;
                Play.Text = "Play";
                Application.Idle -= ProcessFrame;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            // imageBox1.Image = capt.QueryFrame(); ;// img;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            timer1.Enabled = true;
        }
        private void Camera_CheckedChanged(object sender, EventArgs e)
        {
            if (Video.Checked)
            {
                capt = new VideoCapture(openFileDialog1.FileName);
                cameraList.Enabled = false;
                videoButton.Enabled = true;
            }
            else
            {
                videoButton.Enabled = false;
                cameraList.Enabled = true;
            }

            object sender1 = new object();
            EventArgs arg1 = new EventArgs();
            ProcessFrame(sender1, arg1);

            FrameCount = 0;
            trackBar1.Value = FrameCount;
            Play.Text = "Play";
            captureInProgress = false;
            Application.Idle -= ProcessFrame;

        }

        private void Video_CheckedChanged(object sender, EventArgs e)
        {

            if (Video.Checked)
            {
                capt = new VideoCapture(openFileDialog1.FileName);
                cameraList.Enabled = false;
                videoButton.Enabled = true;
            }
            else
            {
                videoButton.Enabled = false;
                cameraList.Enabled = true;
            }


            object sender1 = new object();
            EventArgs arg1 = new EventArgs();
            ProcessFrame(sender1, arg1);

            FrameCount = 0;
            trackBar1.Value = FrameCount;
            Play.Text = "Play";
            captureInProgress = false;
            Application.Idle -= ProcessFrame;

        }

        private void Play_Click(object sender, EventArgs e)
        {


            //i = 0;
            //capt.Start(); else
            if (!Video.Checked)
            {
                if (capt == null)
                    capt = new VideoCapture(0);
            }
            if (captureInProgress)
            {
                GC.Collect();
                Application.Idle -= ProcessFrame;
                Play.Text = "Play";
            }
            else
            {
                GC.Collect();
                Application.Idle += ProcessFrame;
                Play.Text = "Pause";
            }
            captureInProgress = !captureInProgress;


        }

        private void Stop_Click(object sender, EventArgs e)
        {
            if (Video.Checked)
                capt = new VideoCapture(openFileDialog1.FileName);
            object sender1 = new object();
            EventArgs arg1 = new EventArgs();
            ProcessFrame(sender1, arg1);

            FrameCount = 0;
            trackBar1.Value = FrameCount;
            Play.Text = "Play";
            captureInProgress = !captureInProgress;
            Application.Idle -= ProcessFrame;
            //capt.Stop();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void trainSystem_FileOk(object sender, CancelEventArgs e)
        {

        }
        private void drawPoints(Image<Bgr, byte> imgProc, List<Point> listOfPoints)
        {
            foreach (Point p in listOfPoints)
            {
                imgProc.Draw(new CircleF(p, 2), new Bgr(0, 0, 0), 2);
                imgProc.Draw(new CircleF(p, 2), new Bgr(255, 255, 255), -1);
            }
        }
        private void train_Click(object sender, EventArgs e)
        {
            //string line;
            //DirectoryInfo dir = new DirectoryInfo("E:/libbs");
            //dir.Create();
            //StreamReader file = new StreamReader("SystemLibPath.txt");
            //while ((line = file.ReadLine()) != null)
            //{
            //    Console.WriteLine(line);
            //    FileInfo f = new FileInfo(line);

            //    File.Copy(line, dir.FullName + "/" + f.Name);

            //}
            EDRIPMain.getInstance().loadPredictor();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            string[] files = { };
            if (result == System.Windows.Forms.DialogResult.OK)
                files = Directory.GetFiles(fbd.SelectedPath, "*", SearchOption.AllDirectories);

            if (Methodology.SelectedIndex == 1)
                EDRIPMainDistanceAndWrinkledBased.getInstance().TrainSystemForEmotionDistanceAndWrinkledBased(files);

            else if (Methodology.SelectedIndex == 0)
            {
                EDRIPMainDistanceBased.getInstance().TrainSystemForEmotionDistancedBased(files);
            }
            else if (Methodology.SelectedIndex == 2)
            {
                EDRIPMainReducedDimensionsBased.getInstance().TrainSystemForEmotionReducedDimentionsBased(files);
            }
            else if (Methodology.SelectedIndex == 3)
            {
                OpticalflowViewTrain(files);
            }
            else if (Methodology.SelectedIndex == 4)
            {
                EDRIPHybridMethodologyBasedMain.getInstance().TrainSystemForEmotionHybrid(files);
            }

        }

        private void Methodology_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpticalFlow.Visible = false;
            if (Methodology.SelectedIndex == 2 || Methodology.SelectedIndex == 3 || Methodology.SelectedIndex == 1)
            {
                Play.Enabled = false;
                Stop.Enabled = false;
                Upload.Enabled = true;
                if (Methodology.SelectedIndex == 1)
                {
                    Play.Enabled = true;
                }
            }
            else
            {
                Play.Enabled = true;
                Stop.Enabled = true;
                Upload.Enabled = false;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public class pointxy
        {
            public long x;
            public long y;
        }


        private void OpticalflowViewTrain(string[] files)
        {

            EDRIPOpticalFlowMain.getInstance().trainEmotion(files);
        }
        private void OpticalflowView(OpenFileDialog openFileDialog1)
        {
            OpticalFlow.Text = "Testing.. Please Wait";
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";


            //selectedFilePath = result.ToString();

            int total = 0;
            foreach (String file in openFileDialog1.FileNames)
            {
                total++;
            }

            if (total < 14)
            {
                MessageBox.Show("Please select 14 images.");
            }

            Mat[] inputArray = new Mat[14];
            int j = 0;

            foreach (String file in openFileDialog1.FileNames)
            {
                // Create a PictureBox.
                try
                {
                    inputArray[j] = new Mat(file, ImreadModes.Color);
                    j++;
                }
                catch (SecurityException ex)
                {
                    // The user lacks appropriate permissions to read files, discover paths, etc.
                    MessageBox.Show("Security error. Please contact your administrator for details.\n\n" +
                        "Error message: " + ex.Message + "\n\n" +
                        "Details (send to Support):\n\n" + ex.StackTrace
                    );
                }
                catch (Exception ex)
                {
                    // Could not load the image - probably related to Windows file system permissions.
                    MessageBox.Show("Cannot display the image: " + file.Substring(file.LastIndexOf('\\'))
                        + ". You may not have permission to read the file, or " +
                        "it may be corrupt.\n\nReported error: " + ex.Message);
                }
            }
            string emotion = EDRIPOpticalFlowMain.getInstance().predictEmotionGeneralized(inputArray);

            OpticalFlow.Text = "Predicted emotion is " + emotion;

        }
        private void Upload_Click(object sender, EventArgs e)
        {

            Stream fileStream = null;
            UploadImage.Multiselect = true;
            if ((UploadImage.ShowDialog() == DialogResult.OK) && (fileStream = UploadImage.OpenFile()) != null)
            {

                string fileName = UploadImage.FileName;
                FileInfo file = new FileInfo(UploadImage.FileName);
                Mat ImageMat = new Mat(file.FullName);

                string emotions = "";
                EmotionBusinessModel mod = new EmotionBusinessModel();
                if (Methodology.SelectedIndex == 1)
                    mod = EDRIPMainDistanceAndWrinkledBased.getInstance().DetectEmotionsDistanceAndWrinkledBased(new Mat[] { ImageMat }, FDIPMAin.DetectFace(ImageMat));

                else if (Methodology.SelectedIndex == 2)
                {
                    mod = EDRIPMainReducedDimensionsBased.getInstance().DetectEmotionsReducedDimentionsBased(new Mat[] { ImageMat });
                }
                //   emotions = EDRIPMainReducedDimensionsBased.getInstance().DetectEmotionsReducedDimentionsBased(new Mat[] { ImageMat }).outputResult;
                else if (Methodology.SelectedIndex == 3)
                {
                    OpticalflowView(UploadImage);
                    OpticalFlow.Visible = true;
                }
                else if (Methodology.SelectedIndex == 4)
                {
                    EDRIPHybridMethodologyBasedMain.getInstance().testSystemForEmotionHybrid(UploadImage.FileNames);
                }
                emotions = mod.outputMessage;
                CvInvoke.Resize(ImageMat, ImageMat, new Size(320, 240), 0, 0, Inter.Linear);
                Image<Bgr, byte> I2 = mod.images[0].image.ToImage<Bgr, byte>();
                GC.Collect();
                Logger log = new Logger();
                log.LogIntoFile(Log.logType.INFO, "message to be written");

                //  I2.Draw(emotions, new Point(10, 30), FontFace.HersheyPlain, 2.0, new Bgr(10, 0, 255));
                imageBox1.Image = I2;
            }
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("HHmmss");
        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            treeView1.Nodes[0].Nodes.Add("Surprise  ");
            //treeView1.Nodes[0].Nodes[0].Nodes.Add();
            treeView1.Nodes[0].Nodes.Add("Smile ");
            treeView1.Nodes[0].Nodes.Add("Anger ");
            treeView1.Nodes[0].Nodes.Add("Fear  ");
            treeView1.Nodes[0].Nodes.Add("Disgust   ");
            treeView1.Nodes[0].Nodes.Add("Sad   ");
            treeView1.Nodes[0].Nodes.Add("Neutral   ");

            String timeStamp = GetTimestamp(DateTime.Now);

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void imageListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
