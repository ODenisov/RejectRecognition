using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;


namespace RejectRecognGUI
{
    public partial class Form1 : Form
    {
        public static VideoCapture[] _cameras = new VideoCapture[10];
        public static int current_camera = 0;
        public static Mat[] result = new Mat[10];
        public static Mat Frame = new Mat();
        public static Mat Frame1 = new Mat();
        public static Mat Mask = new Mat();


        public Form1()
        {
            InitializeComponent();
            CameraMan();
        }

        private void CameraMan()
        {
            //
            for (int i = 0; i < 10; i++)
            {
                result[i] = new Mat();
                _cameras[i] = new VideoCapture(i);
                if (_cameras[i].Height > 0)
                {
                    _cameras[i].Start();
                    comboCameras.Items.Add("camera " + (i + 1).ToString());
                }
            }
            comboCameras.SelectedIndex = 0;
            current_camera = comboCameras.SelectedIndex;
            refProps();

            Mask = CvInvoke.Imread("capture" + current_camera.ToString() + ".jpg");
            CvInvoke.GaussianBlur(Mask, Mask, new Size(3, 3), 1);
            
            Application.Idle += Run;

        }
        void Run(object sender, EventArgs e)
        {
            Frame = PrepPic(Mask, _cameras[current_camera].QueryFrame());
            cameraFeed1.Image = Frame;
        }

        static Mat PrepPic(Mat mask, Mat pic)
        {
            Mat temp = new Mat();
            CvInvoke.GaussianBlur(pic, temp, new System.Drawing.Size(3, 3), 1);

            if (mask.Height == temp.Height)
            {
                CvInvoke.AbsDiff(mask.Split()[0], temp.Split()[0], temp);
                CvInvoke.Canny(temp, temp, 25, 150);
            }

            return temp;
        }

        private void refProps()
        {
            numericBrightness.Value = Convert.ToDecimal(_cameras[current_camera].GetCaptureProperty(CapProp.Brightness));
            numericContrast.Value = Convert.ToDecimal(_cameras[current_camera].GetCaptureProperty(CapProp.Contrast));
            numericExposure.Value = Convert.ToDecimal(_cameras[current_camera].GetCaptureProperty(CapProp.Exposure));
            numericFPS.Value = Convert.ToDecimal(_cameras[current_camera].GetCaptureProperty(CapProp.Fps));
        }

        private void numericBrightness_ValueChanged(object sender, EventArgs e)
        {
            _cameras[current_camera].SetCaptureProperty(CapProp.Brightness, Convert.ToDouble(numericBrightness.Value));
        }

        private void numericFPS_ValueChanged(object sender, EventArgs e)
        {
            _cameras[current_camera].SetCaptureProperty(CapProp.Fps, Convert.ToDouble(numericFPS.Value));
        }

        private void numericContrast_ValueChanged(object sender, EventArgs e)
        {
            _cameras[current_camera].SetCaptureProperty(CapProp.Contrast, Convert.ToDouble(numericContrast.Value));
        }

        private void numericExposure_ValueChanged(object sender, EventArgs e)
        {
            _cameras[current_camera].SetCaptureProperty(CapProp.Exposure, Convert.ToDouble(numericExposure.Value));
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter("Settings_Camera_" + comboCameras.SelectedIndex.ToString()+".txt");
            string text = "";
            text += "---Camera " + comboCameras.SelectedIndex.ToString() + "---";
            text += "\nBrigtness " + _cameras[current_camera].GetCaptureProperty(CapProp.Brightness).ToString();
            text += "\nContrast " + _cameras[current_camera].GetCaptureProperty(CapProp.Contrast).ToString();
            text += "\nExposure " + _cameras[current_camera].GetCaptureProperty(CapProp.Exposure).ToString();
            text += "\nFPS " + _cameras[current_camera].GetCaptureProperty(CapProp.Fps).ToString();
            sw.WriteLine(text);
            sw.Close();
            sw.Dispose();
        }

        private void snapshot1_Click(object sender, EventArgs e)
        {
            _cameras[current_camera].QueryFrame().Save("capture"+current_camera.ToString()+".jpg");
            Mask = CvInvoke.Imread("capture" + current_camera.ToString() + ".jpg");
            CvInvoke.GaussianBlur(Mask, result[0], new Size(3, 3), 1);
        }

        private void comboCameras_SelectedIndexChanged(object sender, EventArgs e)
        {
            current_camera = comboCameras.SelectedIndex;
            refProps();
            try
            {
                Mask = CvInvoke.Imread("capture" + current_camera.ToString() + ".jpg");
                CvInvoke.GaussianBlur(Mask, Mask, new Size(3, 3), 1);
            }
            catch
            {
                _cameras[current_camera].QueryFrame().Save("capture" + current_camera.ToString() + ".jpg");
                Mask = CvInvoke.Imread("capture" + current_camera.ToString() + ".jpg");
                CvInvoke.GaussianBlur(Mask, result[0], new Size(3, 3), 1);
            }
        }
    }
}
