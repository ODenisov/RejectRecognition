using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        private static VideoCapture _cameraCapture;

        private static Mat[] result = new Mat[10];
        static Mat frame = _cameraCapture.QueryFrame();
        static Mat frame2 = new Mat();
        static Mat mask = new Mat();


        public static bool showDifference = false;
        public Form1()
        {
            InitializeComponent();
            CameraMan();
        }

        private void CameraMan()
        {

            try
            {
                _cameraCapture = new VideoCapture();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
            _cameraCapture.SetCaptureProperty(CapProp.Brightness, 100);
            _cameraCapture.SetCaptureProperty(CapProp.Contrast, 30);
            _cameraCapture.SetCaptureProperty(CapProp.Fps, 15);
            _cameraCapture.SetCaptureProperty(CapProp.Exposure, 100);
            _cameraCapture.SetCaptureProperty(CapProp.Focus, 50);



            Application.Idle += Run;
        }
        void Run(object sender, EventArgs e)
        {

            for (int i = 0; i < 10; i++)
            {
                result[i] = new Mat();
            }

            result[9] = frame;
            mask = CvInvoke.Imread("capture.jpg");

            CvInvoke.GaussianBlur(frame, result[1], new Size(3, 3), 1); //filter out noises

            CvInvoke.GaussianBlur(mask, result[0], new Size(3, 3), 1);

            //CvInvoke.GaussianBlur(frame2, result[0], GausianSize, 6);

            CvInvoke.AbsDiff(result[0].Split()[0], result[1].Split()[0], result[2]);
            CvInvoke.Canny(result[2], result[3], 25, 150);
            MCvScalar scalar =  CvInvoke.Sum(result[3]);

            if (showDifference)
            { 
                cameraFeed1.Image = result[3];
            }
            else
            {
                cameraFeed1.Image = frame;
            }


        }
        private void snapshot1_Click(object sender, EventArgs e)
        {
            result[9].Save("capture.jpg");
        }

        private void snapshot2_Click(object sender, EventArgs e)
        {

        }

        private void buttonDetect_Click(object sender, EventArgs e)
        {
            showDifference = true;
        }
    }
}
