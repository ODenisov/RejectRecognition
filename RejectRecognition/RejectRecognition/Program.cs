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
            VectorOfMat CannyTest = new VectorOfMat(5);
            VectorOfMat strg1 = new VectorOfMat(3);
            VectorOfMat strg2 = new VectorOfMat(3);

            string Str1 = "Image";
            string StrRes = "Result";

            Mat test1 = CvInvoke.Imread("test3.jpg", Emgu.CV.CvEnum.ImreadModes.AnyDepth | Emgu.CV.CvEnum.ImreadModes.AnyColor);
            Mat test2 = CvInvoke.Imread("test4.jpg");

            Mat result = new Mat();

            // CvInvoke.Compare(test1, test2, result,Emgu.CV.CvEnum.CmpType.NotEqual);

            CvInvoke.Canny(test1, CannyTest[0] , 10, 50);
            CvInvoke.Canny(test2, CannyTest[1], 10, 50);

            CvInvoke.Split(test1, strg1);
            CvInvoke.Split(test2, strg2);

            CvInvoke.Compare(strg1[0], strg2[0], result,Emgu.CV.CvEnum.CmpType.NotEqual);
            CvInvoke.Subtract(result, strg2[0], result);

            //CvInvoke.Subtract(result, strg1[0], result);
            CvInvoke.Canny(result, CannyTest[2], 50, 150);

            //CvInvoke.Imshow(Str1, test3);
            //CvInvoke.Imshow("test2", test4);
            CvInvoke.Imshow("canny", CannyTest[2]);
            CvInvoke.Imshow(StrRes, result);

            CvInvoke.WaitKey(0);  
            CvInvoke.DestroyAllWindows();
        }
    }
}
