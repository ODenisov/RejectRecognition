using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;


namespace RejectRecognition
{
    class Program
    {
        static Mat CameraFeed = new Mat();
        static Mat[] Masks = new Mat[10];
        static Mat[] ResPic = new Mat[10];
        static StreamWriter sw = new StreamWriter("log"+DateTime.Now.ToString("dd_mm_yy-HH_MM")+".txt");
        //consts
        static int HEIGHT = 500;
        static int WARNINGTRH = 15;
        static int ALERTTRH = 35;


        class Service
        {
            public string state { get; set; }
        }
        class DeviceList
        {
            public string id { get; set; }
            //public string cameraID { get; set; }
            public string disparity { get; set; }
        }
        class CameraList
        {
            public string id { get; set; }
        }
        class Error
        {
            public string code { get; set; }
            public string description { get; set; }

        }
        class ResponseForSnap
        {
            public Error error { get; set; }
        }
        class ResponseForTest
        {
            public CameraList[] cameraList { get; set; }
            public Error error { get; set; }
        }
        class ResponseForCheck
        {
            public Service service { get; set; }
            public DeviceList[] rejectList { get; set; }
            public Error error { get; set; }
        }

        private static async Task Listen(string adress)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(@"http://"+adress+":35053/comp_sight/");
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

                Console.WriteLine(request.QueryString.Keys.Count.ToString());
                
                string responseString = "";
                int keyCount=request.QueryString.AllKeys.Length;
                switch (request.QueryString.GetValues("cmd")[0])
                {
                    case "make_snp":
                        responseString = FormResponseSnap(ResPic, Masks, request.QueryString.GetValues(1)[0]);
                        Console.WriteLine("make_snp: ");
                        sw.Write("make_snp"+ DateTime.Now.ToString("(HH_mm_ss)") + ": ");
                        break;
                    case "rej_check":
                        responseString = keyCount==2? FormResponseCheck(ResPic, Masks, request.QueryString.GetValues(1)[0]): FormResponseCheck(ResPic, Masks, "ALL");
                        Console.WriteLine("rej_check: ");
                        sw.Write("rej_check: ");
                        break;
                    case "self_test":
                        responseString = FormResponseTest(ResPic, Masks);
                        Console.WriteLine("self_test: ");
                        sw.Write("self_test: ");
                        break;
                    default:
                        break;

                }

                Console.WriteLine(responseString);
                sw.WriteLine(responseString);
                sw.Flush();
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Flush();
                output.Close();
            }
            Console.WriteLine("server stoped working");
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
            Task.Run(() => Listen(adress));
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
            //CvInvoke.GaussianBlur(pic, temp, new System.Drawing.Size(3, 3), -9);
            pic.CopyTo(temp);

            if (mask.Height == temp.Height)
            {
                CvInvoke.AbsDiff(mask.Split()[0], temp.Split()[0], temp);
                CvInvoke.Threshold(temp, temp, 37, 255, ThresholdType.Binary);
                CvInvoke.Canny(temp, temp, 25, 100);
            }

            return temp;
        }//PrepPic

        static string CountDiff(Mat pic)
        {
            MCvScalar scalar = CvInvoke.Sum(pic);
            double test = scalar.V0 / 25500;
            Console.WriteLine("disp = " + (test).ToString());
            sw.WriteLine("disp = " + (scalar.V0 / 25500).ToString());
            if ((scalar.V0 / 25500) < WARNINGTRH)
                return "CLEAR";
            else if ((scalar.V0 / 25500) < ALERTTRH)
                return "WARNING";
            else
                return "ALERT";
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
                        if ((!sources[i].IsEmpty) && (sources[i].Height < HEIGHT))
                        {
                            masks[i] =sources[i];
                            sources[i].Save("capture" + i.ToString() + ".jpg");
                            //CvInvoke.GaussianBlur(masks[i], masks[i], new System.Drawing.Size(3, 3), 1);
                        }
                    }
                }
                else
                {
                    i = Convert.ToInt32(id);
                    if ((!sources[i].IsEmpty) && (sources[i].Height < HEIGHT))
                    {
                        masks[i] = sources[i];
                        sources[i].Save("capture" + i.ToString() + ".jpg");
                       //CvInvoke.GaussianBlur(masks[i], masks[i], new System.Drawing.Size(3, 3), 1);
                    }
                }
            }
            catch
            {
                err.code = 1.ToString();
                err.description = "Something went wrong";
                resp.error = err;
                answer = MakeJSON(resp);
                return answer;
            }

            err.code = 0.ToString(); ;
            err.description = "";
            resp.error = err;
            answer = MakeJSON(resp);
            return answer;
        }//FormResponse

        static string FormResponseCheck(Mat[] sources, Mat[] masks,string id)
        {
            int count = sources.Length;
            string answer = @"";
            int cam = 0;
            int Id = (id == "ALL" ? 0 : Convert.ToInt32(id));            
            for (int i = 0; i < count; i++)
            {
                if ((!sources[i].IsEmpty) && (sources[i].Height < HEIGHT))
                {
                    cam++;
                }
            }
            ResponseForCheck resp = new ResponseForCheck();
            Service serv = new Service();
            DeviceList[] rej = new DeviceList[cam];
            for (int i = 0; i < cam; i++)
            {
                rej[i] = new DeviceList();
            }
            Error err = new Error();

            serv.state = "READY";
            err.code = 0.ToString();
            err.description = "";
            try
            {
                if(cam<Id)
                {
                    throw new IndexOutOfRangeException();
                }
                if (Id == 0)
                {
                    for (int i = 0; i < cam; i++)
                    {
                        rej[i].id = "front.bsm.reject" + i.ToString();
                        rej[i].disparity = CountDiff(PrepPic(masks[i], sources[i]));
                    }
                }
                else
                {
                    rej[Id-1].id = "front.bsm.reject" + Id.ToString();
                    rej[Id-1].disparity = CountDiff(PrepPic(masks[Id-1], sources[Id-1]));
                }
            }
            catch
            {
                serv.state = "ERROR";
                err.code = 1.ToString();
                err.description = "Something went wrong";
                resp.error = err;
                answer = MakeJSON(resp);
                return answer;
            }

            resp.service = serv;
            resp.rejectList = rej;
            resp.error = err;
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
            resp.cameraList = cams;
            resp.error = err;
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
