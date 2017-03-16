using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Emgu.CV;
using Emgu.CV.Structure;


namespace RejectRecognition
{
    class Program
    {
        static Mat CameraFeed = new Mat();
        static Mat[] Masks = new Mat[10];
        static Mat[] ResPic = new Mat[10];

        class Service
        {
            public string State { get; set; }
        }
        class RejectList
        {
            public string ID { get; set; }
            //public string cameraID { get; set; }
            public string Disparity { get; set; }
        }
        class CameraList
        {
            public string ID { get; set; }
        }
        class Error
        {
            public int Code { get; set; }
            public string Description { get; set; }

        }
        class ResponseForSnap
        {
            public Error Error { get; set; }
        }
        class ResponseForTest
        {
            public CameraList[] CameraList { get; set; }
            public Error Error { get; set; }
        }
        class ResponseForCheck
        {
            public Service Service { get; set; }
            public RejectList[] RejectList { get; set; }
            public Error Error { get; set; }
        }

        private static async Task Listen(string adress)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(@"http://"+adress+":35053/");
            Console.WriteLine(listener.Prefixes.ElementAt<string>(0));
            try
            {
                listener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                string responseString = "";
                switch (request.QueryString.GetValues("comm")[0])
                {
                    case "make_snp":
                        Console.WriteLine(request.QueryString.GetValues(1)[0]);
                        responseString = FormResponseSnap(ResPic, Masks, request.QueryString.GetValues(1)[0]);
                        break;
                    case "rj_check":
                        responseString = FormResponseCheck(ResPic, Masks);
                        break;
                    case "self_test":
                        responseString = FormResponseTest(ResPic, Masks);
                        break;
                    default:
                        break;

                }

                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }//Listen
                
        static void Main(string[] args)
        {
            Console.WriteLine("IP adress: ");
            string adress = Console.ReadLine();
            int i = 0;
            System.Drawing.Size GausianSize = new System.Drawing.Size(3, 3);
            //Results
            //ResPic = new Mat[10];
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
                    //VSource[i].SetCaptureProperty(Emgu.CV.CvEnum.CapProp.AutoExposure, 1);
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
                    ResPic[i] = new Mat();
                    VSource[i].Retrieve(ResPic[i]);
                    if ((!ResPic[i].IsEmpty) && (ResPic[i].Height < 500))
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
                        if ((!ResPic[i].IsEmpty)&&(ResPic[i].Height<500))
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

        static void OpenSettings(VideoCapture[] Videos)
        {
            StreamReader sr;
            for (int cameraNum = 0; cameraNum < 10; cameraNum++)
            {
                try
                {
                    sr = new StreamReader("Settings_Camera_" + cameraNum.ToString() + ".txt");
                }
                catch
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
                double fraps = Convert.ToDouble(Fps.Split(' ')[1]) == 0 ? 60 : Convert.ToDouble(Fps.Split(' ')[1]);//если фпс не меняли, ставим 60
                SetProperties(Videos[cameraNum], bright, fraps, contrast);//3 args
                sr.Close();
                sr.Dispose();
            }
        }//OpenSettings

        static void SetProperties(VideoCapture source, double brigth, double FPS, double exposure, double contrast)
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

        static Mat PrepPic(Mat mask, Mat pic)
        {
            if (mask == null)
                return pic;

            Mat temp = new Mat();
            CvInvoke.GaussianBlur(pic, temp, new System.Drawing.Size(3, 3), -9);

            if (mask.Height == temp.Height)
            {
                CvInvoke.AbsDiff(mask.Split()[0], temp.Split()[0], temp);
                CvInvoke.Canny(temp, temp, 10, 250);
            }

            return temp;
        }//PrepPic

        static double CountDiff(Mat pic)
        {
            MCvScalar scalar = CvInvoke.Sum(pic);
            return scalar.V0 / 25500;
        }//CountDiff

        static string BuildReport(int CamNum, double ErrorPerc)
        {
            string Report = "id: " + CamNum.ToString() + " ; disparity: " + ErrorPerc.ToString();
            return Report;
        }//BuildReport

        static string FormResponseSnap(Mat[] sources, Mat[] masks, string id)
        {
            int count = sources.Length;
            int i;
            string answer="";

            ResponseForSnap resp = new ResponseForSnap();
            Error err = new Error();
            try
            {
                if (id == "ALL")
                {
                    for (i = 0; i < count; i++)
                    {
                        if ((!sources[i].IsEmpty) && (sources[i].Height < 500))
                        {
                            sources[i].Save("capture" + i.ToString() + ".jpg");
                            masks[i] = CvInvoke.Imread("capture" + i.ToString() + ".jpg");
                            CvInvoke.GaussianBlur(masks[i], masks[i], new System.Drawing.Size(3, 3), 1);
                        }
                    }
                }
                else
                {
                    i = Convert.ToInt32(id);
                    if ((!sources[i].IsEmpty) && (sources[i].Height < 500))
                    {
                        sources[i].Save("capture" + i.ToString() + ".jpg");
                        masks[i] = CvInvoke.Imread("capture" + i.ToString() + ".jpg");
                        CvInvoke.GaussianBlur(masks[i], masks[i], new System.Drawing.Size(3, 3), 1);
                    }
                }
            }
            catch
            {
                err.Code = 1;
                err.Description = "Something went wrong";
                resp.Error = err;
                answer = MakeJSON(resp);
                return answer;
            }

            err.Code = 0;
            err.Description = "";
            resp.Error = err;
            answer = MakeJSON(resp);

            return answer;
        }//FormResponse

        static string FormResponseCheck(Mat[] sources, Mat[] masks)
        {
            int count = sources.Length;
            string answer = @"";
            int cam = 0;
            for (int i = 0; i < count; i++)
            {
                if ((!sources[i].IsEmpty) && (sources[i].Height < 500))
                {
                    cam++;
                }
            }
            ResponseForCheck resp = new ResponseForCheck();
            Service serv = new Service();
            RejectList[] rej = new RejectList[count];
            for (int i = 0; i < cam; i++)
            {
                rej[i] = new RejectList();
            }
            Error err = new Error();

            serv.State = "READY";
            err.Code = 0;
            err.Description = "";
            try
            {
                for (int i = 0; i < cam; i++)
                {                
                    rej[i].ID = i.ToString();
                    rej[i].Disparity = CountDiff(PrepPic(masks[i], sources[i])).ToString();             
                }
            }
            catch
            {
                serv.State = "ERROR";
                err.Code = 1;
                err.Description = "Something went wrong";
                resp.Error = err;
                answer = MakeJSON(resp);
                return answer;
            }

            resp.Service = serv;
            resp.RejectList = rej;
            resp.Error = err;
            answer = MakeJSON(resp);

            return answer;
        }//FormResponse

        ///TODO:проверять ошибки и составить таблицу ошибок 
        static string FormResponseTest(Mat[] sources, Mat[] masks)
        {
            int count = sources.Length;
            int cam = 0;
            string answer = @"";
            ResponseForTest resp = new ResponseForTest();
            Error err = new Error();

            for (int i = 0; i < count; i++)
            {
                if ((!sources[i].IsEmpty)&&(sources[i].Height<500))
                {
                    cam++;
                }
            }
            CameraList[] cams = new CameraList[cam];
            resp.CameraList = cams;
            resp.Error = err;
            answer = MakeJSON(resp);
            return answer;
        }//FormResponse 


        static string MakeJSON(ResponseForCheck JSONobj)
        {
            return JsonConvert.SerializeObject(JSONobj);
        }//makeJson

        static string MakeJSON(ResponseForSnap JSONobj)
        {
            return JsonConvert.SerializeObject(JSONobj);
        }//makeJson

        static string MakeJSON(ResponseForTest JSONobj)
        {
            return JsonConvert.SerializeObject(JSONobj);
        }//makeJson

    }
}
