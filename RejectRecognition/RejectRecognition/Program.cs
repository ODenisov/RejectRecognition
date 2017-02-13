using System;
using System.IO;

using Emgu.CV;
using Emgu.CV.Structure;

namespace RejectRecognition
{
    class Program
    {
        //private static async void StartListener()
        //{            
        //}
        static void Main(string[] args)
        {
            Mat CameraFeed = new Mat();
            Mat[] Masks = new Mat[10];
            int i = 0;
            System.Drawing.Size GausianSize = new System.Drawing.Size(3, 3);

            //Results
            Mat[] ResPic = new Mat[10];
            for (i = 0; i < 10; i++)
            {
                ResPic[i] = new Mat();
            }
            //

            //mask
            for (i = 0; i < 10; i++)
            {
                try
                {
                    Masks[i] = CvInvoke.Imread("capture" + i.ToString() + ".jpg");
                }
                catch
                {
                    continue;
                }
                CvInvoke.GaussianBlur(Masks[i], Masks[i], GausianSize, 1);
            }
            //

            VideoCapture[] VSource = new VideoCapture[10];
            for (i = 0; i < 10; i++)
            {
                try
                {
                    VSource[i] = new VideoCapture(i);
                }
                catch
                {
                    break;
                }
            }

            OpenSettings(VSource);

            while (true)
            {
                for (i = 0; i < 10; i++)
                {
                    VSource[i].Retrieve(ResPic[i]);
                    if (!ResPic[i].IsEmpty)
                    {
                        CvInvoke.Imshow(i.ToString(),PrepPic(Masks[i], ResPic[i]));
                        //CvInvoke.Imshow(i.ToString(), ResPic[i]);
                    }
                }

                int c = CvInvoke.WaitKey(33);

                if (c == 27)
                {
                    break;
                }
                else if (c == 13)
                {
                    for (i = 0; i < 10; i++)
                    {
                        if (!ResPic[i].IsEmpty)
                        {
                            ResPic[i].Save("capture" + i.ToString() + ".jpg");
                            Masks[i] = CvInvoke.Imread("capture" + i.ToString() + ".jpg");
                            CvInvoke.GaussianBlur(Masks[i], Masks[i], GausianSize, 1);
                        }//fi
                    }//for
                }//else fi
                else if (c == 32)
                {
                    for (i = 0; i < 10; i++)
                        if (ResPic[i].Height > 0)
                        {
                            Console.WriteLine(BuildReport(i, CountDiff(PrepPic(Masks[i], ResPic[i]))));
                        }//fi
                }//else fi
            }

            CvInvoke.DestroyAllWindows(); //конец видеопотока
        }//main

        static void SetProperties(VideoCapture source, double brigth, double FPS, double exposure,double contrast)
        {
            source.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Brightness, brigth);
            source.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, FPS);
            source.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Exposure, exposure);
            source.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Contrast, contrast);
        }//SetProperties 4 args

        static void SetProperties(VideoCapture source, double brigth, double FPS, double contrast)
        {
            source.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Brightness, brigth);
            source.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, FPS);
            source.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Contrast, contrast);
        }//SetProperties 3 args

        static string BuildReport(int CamNum, double ErrorPerc)
        {
            string Report = "id: " + CamNum.ToString() + " ; disparity: " + ErrorPerc.ToString();
            return Report;
        }//BuildReport

        static Mat PrepPic(Mat mask, Mat pic)
        {
            if (mask == null)
                return pic;

            Mat temp = new Mat();
            CvInvoke.GaussianBlur(pic, temp, new System.Drawing.Size(3, 3), 1);

            if (mask.Height == temp.Height)
            {
                CvInvoke.AbsDiff(mask.Split()[0], temp.Split()[0], temp);
                CvInvoke.Canny(temp, temp, 20, 170);
            }

            return temp;
        }//PrepPic

        static double CountDiff(Mat pic)
        {
            MCvScalar scalar = CvInvoke.Sum(pic);
            return scalar.V0 / 25500;
        }//CountDiff

        static void OpenSettings(VideoCapture[] Videos)
        {
            StreamReader sr;
            for (int cameraNum = 0; cameraNum < 10; cameraNum++)
            {
                try
                {
                    sr = new StreamReader("Settings_Camera_" + cameraNum.ToString() + ".txt");
                }catch
                {
                    continue;
                }
                sr.ReadLine();
                string Brightness = sr.ReadLine();
                string Contrast = sr.ReadLine();
                string Exposure = sr.ReadLine();
                string Fps = sr.ReadLine();
                double bright = Convert.ToDouble(Brightness.Split(' ')[1]);
                double contrast = Convert.ToDouble(Contrast.Split(' ')[1]);
                double expos = Convert.ToDouble(Exposure.Split(' ')[1]);
                double fraps = Convert.ToDouble(Fps.Split(' ')[1]);
                SetProperties(Videos[cameraNum], bright, contrast, fraps);//3 args
                sr.Close();
                sr.Dispose();
            }
        }//OpenSettings

    }
}
