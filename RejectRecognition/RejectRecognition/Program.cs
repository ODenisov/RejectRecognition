using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace RejectRecognition
{
    class Program
    {
        static void Main(string[] args)
        {
            Mat CameraFeed = new Mat();
            Mat Mask = new Mat();
            System.Drawing.Size GausianSize = new System.Drawing.Size(3, 3);

            //Reults
            Mat[] ResPic = new Mat[10];
            for (int i = 0;i<10;i++)
            {
                ResPic[i] = new Mat();
            }
            //
            //mask
            Mask = CvInvoke.Imread("capture.jpg");
            CvInvoke.GaussianBlur(Mask, Mask, GausianSize, 1);
            //

            VideoCapture[] VSource = new VideoCapture[10];
            for ( int i = 0; i < 10 ; i++)
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
            for (int i = 0; i < 10; i++)
            {
                SetProperties(VSource[i], 150, 15, 50);
                VSource[i].Start();
            }



            while (true)
            {
                for (int i = 0; i<10; i++)
                {
                    VSource[i].Retrieve(ResPic[i]);
                    if(ResPic[i].Height>0)
                    CvInvoke.Imshow(i.ToString(), ResPic[i]);
                }
                //ResPic[3] = PrepPic(Mask, CameraFeed);

                //MCvScalar scalar = CvInvoke.Sum(ResPic[3]);
                //ResPic[3].CopyTo(ResPic[9]);
                //ResPic[9].SetTo(new MCvScalar(0, 0, 0));
                //CvInvoke.PutText(ResPic[9], (scalar.V0 / 255).ToString(), new System.Drawing.Point(100, 100), Emgu.CV.CvEnum.FontFace.HersheyPlain, 5, new MCvScalar(255, 255, 255));


                //CvInvoke.Imshow("pixels", ResPic[9]);
                //CvInvoke.Imshow("video", ResPic[3]);



                int c = CvInvoke.WaitKey(33);
                if(c == 27)
                {
                    break;
                }
                else if(c == 13)
                {
                    CameraFeed.Save("capture.jpg");
                    Mask = CvInvoke.Imread("capture.jpg");
                    CvInvoke.GaussianBlur(Mask, Mask, GausianSize, 1);
                }
            }

            CvInvoke.DestroyAllWindows(); //конец видеопотока
        }//main

        static void SetProperties(VideoCapture source,double brigth, double FPS, double exposure)
        {
            source.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Brightness, brigth);
            source.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, FPS);
            source.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Exposure, exposure);
        }//SetProperties

        static string BuildReport(int CamNum, int ErrorPerc, string Recomendation)
        {
            string Report = CamNum.ToString() + "-" + ErrorPerc.ToString() + "-" + Recomendation;
            return Report;
        }//BuildReport

        static Mat PrepPic(Mat mask, Mat pic)
        {
            Mat temp = new Mat();
            CvInvoke.GaussianBlur(pic, temp, new System.Drawing.Size(3,3), 1);

            CvInvoke.AbsDiff(mask.Split()[0], temp.Split()[0], temp);
            CvInvoke.Canny(temp, temp, 25, 150);

            return temp;
        }

        //static Mat CountDiff(Mat pic)
        //{
        //    MCvScalar scalar = CvInvoke.Sum(pic); 
        //}


    }
}
