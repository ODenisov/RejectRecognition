using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace RejectRecognition
{
    class Program
    {
        static void Main(string[] args)
        {
            //тестовые переменные
            VectorOfMat CannyTest = new VectorOfMat(5);
            VectorOfMat strg1 = new VectorOfMat(3);
            VectorOfMat strg2 = new VectorOfMat(3);

            string Str1 = "Image";
            string StrRes = "Result";

            Mat test1 = new Mat();
            Mat test2 = new Mat();

            test1 = CvInvoke.Imread("test3.jpg", Emgu.CV.CvEnum.ImreadModes.AnyDepth | Emgu.CV.CvEnum.ImreadModes.AnyColor);
            test2 = CvInvoke.Imread("test4.jpg");

            Mat[] result = new Mat[10];
            for (int i = 0;i<10;i++)
            {
                result[i] = new Mat();
            }

            //конец тестовых переменных


            //тестирование видеопотока
            VideoCapture VSource = new VideoCapture();
            VSource.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Brightness, 150);
            VSource.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Contrast, 34);
            VSource.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, 15);
            VSource.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Exposure, 50);
            VSource.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Focus, 50);
            VSource.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Gamma,50);

            //VSource.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Exposure,100);


            System.Drawing.Size GausianSize = new System.Drawing.Size(3, 3);
            VSource.Start();
            while (true)
            {
                VSource.Retrieve(test1, 1);
                test2 = CvInvoke.Imread("capture.jpg");
                CvInvoke.GaussianBlur(test2, result[1], GausianSize, 6);
                CvInvoke.GaussianBlur(test1, result[0], GausianSize, 6);

                CvInvoke.AbsDiff(result[0].Split()[0], result[1].Split()[0], result[2]);
                CvInvoke.Canny(result[2], result[3],25,150);

                MCvScalar scalar = CvInvoke.Sum(result[3]);
                result[3].CopyTo(result[9]);
                CvInvoke.PutText(result[9], (scalar.V0/255).ToString(), new System.Drawing.Point(100, 100), Emgu.CV.CvEnum.FontFace.HersheyPlain, 5, new MCvScalar(255, 255, 255));


                CvInvoke.Imshow("pixels", result[9]);
                CvInvoke.Imshow("video", result[3]);


                int c = CvInvoke.WaitKey(33);
                if(c == 27)
                {
                    break;
                }
                else if(c == 13)
                {
                    test1.Save("capture.jpg");
                }
            }

            CvInvoke.DestroyAllWindows(); //конец тестирования видеопотока

            //ВСЯКОЕ КЛАССНЫЕ ФУНКЦИИ

            //CvInvoke.Compare(test1, test2, result, Emgu.CV.CvEnum.CmpType.NotEqual);

            //CvInvoke.Canny(test1, CannyTest[0], 10, 50);
            //CvInvoke.Canny(test2, CannyTest[1], 10, 50);

            //CvInvoke.CvtColor(test1, test1, Emgu.CV.CvEnum.ColorConversion.Bgr2Luv);


            //CvInvoke.Split(test1, strg1);
            //CvInvoke.Split(test2, strg2);

            //CvInvoke.Compare(strg1[0], strg2[0], result, Emgu.CV.CvEnum.CmpType.NotEqual);
            //CvInvoke.Subtract(result, strg2[0], result1);
            //CvInvoke.Subtract(result, strg1[0], result2);

            //CvInvoke.Subtract(result2, result1, result);
            //CvInvoke.Canny(result, CannyTest[2], 30, 50);

            ////ПОКАЗЫВАЕМ ОКНА

            //CvInvoke.Imshow("canny", CannyTest[2]);
            //CvInvoke.Imshow(StrRes, result);

            ////УБИРАЕМ ОКНА

            //CvInvoke.WaitKey(0);
            //CvInvoke.DestroyAllWindows();
        }
    }
}
