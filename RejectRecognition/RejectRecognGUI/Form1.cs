using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;



namespace RejectRecognGUI
{
    public partial class Form1 : Form
    {
        private Image<Bgr, byte> panno;

        static int polzunValue = 1;

        public static VideoCapture[] _cameras = new VideoCapture[10];
        public static int current_camera = 0;
        public static Mat[] result = new Mat[10];
        public static Mat Frame = new Mat();
        public static Mat Frame1 = new Mat();
        public static Mat Mask = new Mat();
        //
        private Point RectStartPoint;
        private Rectangle Rect = new Rectangle();
        private Rectangle RealImageRect = new Rectangle();
        private Brush selectionBrush = new SolidBrush(Color.FromArgb(128, 64, 64, 64));
        private int thickness = 3;
        private bool mouseDown = false;

        public Form1()
        {
            InitializeComponent();
            cameraFeed1.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            CameraMan();
        }

        static Mat changeColor(Mat source,double light)
        {
            Image<Hls, byte> temp = source.ToImage<Hls, byte>(); 
            //performance boost with no usage of C# in loop
            //byte[,,] data = temp.Data;
            Image<Gray, byte> lum = temp[1];
            lum._EqualizeHist();
            temp[1] = lum;



            //for (int i = 0; i < temp.Height; i++)
            //{

            //    for (int j = 0; j < temp.Width; j++)
            //    {
            //        data[i, j, 0] = Convert.ToByte(light);
            //    }
            //}

            //temp.Data = data;

            return temp.Mat;
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
            _cameras[0].SetCaptureProperty(CapProp.Backlight, 1);
            Mask = CvInvoke.Imread("capture" + current_camera.ToString() + ".jpg");
            CvInvoke.GaussianBlur(Mask, Mask, new Size(3, 3), 1);
            
            Application.Idle += Run;

        }

        void Run(object sender, EventArgs e)
        {
            Frame = PrepPic(Mask, _cameras[current_camera].QueryFrame());
            //Frame =   _cameras[current_camera].QueryFrame();
            panno = Frame.ToImage<Bgr, byte>();
            panno.Draw(RealImageRect, new Bgr(Color.Red), thickness);
            cameraFeed1.Image = panno;
        }
        //В GUI PrepPic используется только программистом в целях проверки фильтров
        static Mat PrepPic(Mat mask, Mat pic)
        {
            Mat temp = new Mat();
            CvInvoke.GaussianBlur(pic, temp, new System.Drawing.Size(3, 3), 1);

            if (mask.Height == temp.Height)
            {
                //canny
                //CvInvoke.AbsDiff(mask.Split()[0], temp.Split()[0], temp);
                //CvInvoke.Threshold(temp, temp, 35, 255, ThresholdType.Binary);
                //CvInvoke.Canny(temp, temp, 25, 150);

                //lum equalization
                //CvInvoke.CvtColor(temp, temp, Emgu.CV.CvEnum.ColorConversion.Bgr2Hls);
                //Mat toto = new Mat();
                //temp.CopyTo(toto);
                //toto = changeColor(toto, polzunValue);
                //CvInvoke.CvtColor(toto, temp, Emgu.CV.CvEnum.ColorConversion.Hls2Bgr);

                Image<Gray, byte> maskGray = mask.ToImage<Gray, byte>();
                Image<Gray, float> maskSobel = maskGray.Sobel(1, 0, 3).Add(maskGray.Sobel(0, 1, 3)).AbsDiff(new Gray(0.0));

                Image<Gray, byte> sourceGray = temp.ToImage<Gray, byte>();
                Image<Gray, float> sourceSobel = sourceGray.Sobel(1, 0, 3).Add(sourceGray.Sobel(0, 1, 3)).AbsDiff(new Gray(0.0));

                CvInvoke.Subtract(sourceSobel, maskSobel, sourceSobel);
                
                temp = sourceSobel.Mat;
                CvInvoke.Threshold(temp, temp, polzunValue, 255, ThresholdType.Binary);

                //Image<Gray, byte> gray = temp.ToImage<Gray, byte>();

                //Image <Gray, float> sobel = gray.Sobel(0, 1, 3).Add(gray.Sobel(1, 0, 3)).AbsDiff(new Gray(0.0));
                //temp = sobel.Mat;

            }

            return temp;
        }

        private void refProps()
        {
            numericBrightness.Value = Convert.ToDecimal(_cameras[current_camera].GetCaptureProperty(CapProp.Brightness));
            numericContrast.Value = Convert.ToDecimal(_cameras[current_camera].GetCaptureProperty(CapProp.Contrast));
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


        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (RealImageRect.X + RealImageRect.Width > 640)
            {
                RealImageRect.Width -= (RealImageRect.X + RealImageRect.Width - 640);
            }
            if (RealImageRect.Y + RealImageRect.Height > 480)
            {
                RealImageRect.Height -= (RealImageRect.Y + RealImageRect.Height - 480);
            }
            StreamWriter sw = new StreamWriter("Settings_Camera_" + comboCameras.SelectedIndex.ToString()+".txt");
            string text = "";
            text += "---Camera " + comboCameras.SelectedIndex.ToString() + "---";
            text += "\nBrigtness " + _cameras[current_camera].GetCaptureProperty(CapProp.Brightness).ToString();
            text += "\nContrast " + _cameras[current_camera].GetCaptureProperty(CapProp.Contrast).ToString();
            text += "\nExposure " + _cameras[current_camera].GetCaptureProperty(CapProp.Exposure).ToString();
            text += "\nFPS " + _cameras[current_camera].GetCaptureProperty(CapProp.Fps).ToString();
            text += "\nX " + RealImageRect.X.ToString() + 
                    "\nY " + RealImageRect.Y.ToString() + 
                    "\nHeight "+ RealImageRect.Height.ToString() +
                    "\nWidth " + RealImageRect.Width.ToString();

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
                //Yes,thats the quikiest way to nulifiy Rectangle 
                RealImageRect.X = 
                    RealImageRect.Y = 
                    RealImageRect.Height = 
                    RealImageRect.Width = 0;
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

        private void cameraFeed1_MouseDown(object sender, MouseEventArgs e)
        {
            RectStartPoint = e.Location;
            Invalidate();
        }

        private void cameraFeed1_MouseMove(object sender, MouseEventArgs e)
        {
            #region SETS COORDINATES AT INPUT IMAGE BOX
            int X0, Y0;
            if (mouseDown)
            {
                Utilities.ConvertCoordinates(cameraFeed1, out X0, out Y0, e.X, e.Y);
                labelPostionXY.Text = "Last Position: X:" + X0 + "  Y:" + Y0;
            }

            //Coordinates at input picture box
            if (e.Button != MouseButtons.Left)
                return;
            Point tempEndPoint = e.Location;
            Rect.Location = new Point(
                Math.Min(RectStartPoint.X, tempEndPoint.X),
                Math.Min(RectStartPoint.Y, tempEndPoint.Y));
            Rect.Size = new Size(
                Math.Abs(RectStartPoint.X - tempEndPoint.X),
                Math.Abs(RectStartPoint.Y - tempEndPoint.Y));
            #endregion

            #region SETS COORDINATES AT REAL IMAGE
            //Coordinates at real image - Create ROI
            Utilities.ConvertCoordinates(cameraFeed1, out X0, out Y0, RectStartPoint.X, RectStartPoint.Y);
            int X1, Y1;
            Utilities.ConvertCoordinates(cameraFeed1, out X1, out Y1, tempEndPoint.X, tempEndPoint.Y);
            RealImageRect.Location = new Point(
                Math.Min(X0, X1),
                Math.Min(Y0, Y1));
            RealImageRect.Size = new Size(
                Math.Abs(X0 - X1),
                Math.Abs(Y0 - Y1));

            //panno = Frame.ToImage<Bgr,byte>();
            //panno.Draw(RealImageRect, new Bgr(Color.Red), thickness);
            #endregion

            ((PictureBox)sender).Invalidate();
        }

        private void pictureBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Draw the rectangle...
            if (cameraFeed1.Image != null)
            {
                if (Rect != null && Rect.Width > 0 && Rect.Height > 0)
                {
                    //Seleciona a ROI
                    e.Graphics.SetClip(Rect, System.Drawing.Drawing2D.CombineMode.Exclude);
                    e.Graphics.FillRectangle(selectionBrush, new Rectangle(0, 0, ((PictureBox)sender).Width, ((PictureBox)sender).Height));
                    //e.Graphics.FillRectangle(selectionBrush, Rect);
                }
            }
        }

        private void cameraFeed1_MouseUp(object sender, MouseEventArgs e)
        {
            if (RealImageRect.Width > 0 && RealImageRect.Height > 0)
            {
                panno.ROI = RealImageRect;
                cameraFeed1.Image = panno;
            }
        }

        private void polzun_ValueChanged(object sender, EventArgs e)
        {
            polzunValue = polzun.Value;
            polzunLabel.Text = polzun.Value.ToString();
        }
    }
}
