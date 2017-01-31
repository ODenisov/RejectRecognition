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
            Mat[] Masks = new Mat[10];
            int i = 0;
            System.Drawing.Size GausianSize = new System.Drawing.Size(3, 3);

            //Reults
            Mat[] ResPic = new Mat[10];
            for ( i = 0;i<10;i++)
            {
                ResPic[i] = new Mat();
            }
            //

            //mask
            for (i = 0; i < 10; i++)
            {
                try
                {
                    Masks[i] = CvInvoke.Imread("capture"+i.ToString()+".jpg");
                }
                catch
                {
                    continue;
                }
                CvInvoke.GaussianBlur(Masks[i], Masks[i], GausianSize, 1);
            }
            //

            VideoCapture[] VSource = new VideoCapture[10];
            for (  i = 0; i < 10 ; i++)
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
            for ( i = 0; i < 10; i++)
            {
                SetProperties(VSource[i], 150, 15, 50);
                VSource[i].Start();
            }



            while (true)
            {
                for ( i = 0; i<10; i++)
                {
                    VSource[i].Retrieve(ResPic[i]);
                    if(!ResPic[i].IsEmpty)
                    CvInvoke.Imshow(i.ToString(),PrepPic(Masks[i], ResPic[i]));
                }

                int c = CvInvoke.WaitKey(33);

                if(c == 27)
                {
                    break;
                }
                else if(c == 13)
                {
                    for ( i = 0; i < 10; i++)
                    {
                        if (!ResPic[i].IsEmpty)
                        {
                            ResPic[i].Save("capture" + i.ToString() + ".jpg");
                            Masks[i] = CvInvoke.Imread("capture" + i.ToString() + ".jpg");
                            CvInvoke.GaussianBlur(Masks[i], Masks[i], GausianSize, 1);
                        }//fi
                    }//for
                }//else fi
                else if(c == 32)
                {
                    for (i = 0; i < 10; i++)
                        if (ResPic[i].Height > 0)
                        {
                            Console.WriteLine(i.ToString() + " = " + CountDiff(PrepPic(Masks[i], ResPic[i])).ToString());
                        }//fi
                }//wlse fi
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

            if (mask.Height == temp.Height)
            {
                CvInvoke.AbsDiff(mask.Split()[0], temp.Split()[0], temp);
                CvInvoke.Canny(temp, temp, 25, 150);
            }

            return temp;
        }

        static double CountDiff(Mat pic)
        {
            MCvScalar scalar = CvInvoke.Sum(pic);
            return scalar.V0 / 25500;
        }


    }
}
