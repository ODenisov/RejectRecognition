using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading;

using Newtonsoft.Json;

using Emgu.CV;
using Emgu.CV.Structure;

namespace RejectRecognition
{
    class Program
    {
        HttpListener server;
        bool flag = true;
        static Mat CameraFeed = new Mat();
        static Mat[] Masks = new Mat[10];
        static Mat[] ResPic = new Mat[10];


        class RequestForGET
        {
            public Service service { get; set; }
            public RejectList[] rejectList { get; set; }
            public Error error { get; set; }
        }
        class Service
        {
            public string state { get; set; }
        }
        class RejectList
        {
            public string ID { get; set; }
            //public string cameraID { get; set; }
            public string disparity { get; set; }
        }
        class Error
        {
            public int code { get; set; }
            public string description { get; set; }

        }

        static string FormResponse(Mat[] sources,Mat[] masks, string type)
        {
            int count = sources.Length;
            string answer = @"";
            switch (type)
            {
                case "POST":
                    sources[count].Save("capture" + count.ToString() + ".jpg");
                    masks[count] = CvInvoke.Imread("capture" + count.ToString() + ".jpg");
                    CvInvoke.GaussianBlur(masks[count], masks[count],new System.Drawing.Size(3, 3), 1);
                    break;
                case "GET":
                    RequestForGET resp = new RequestForGET();
                    Service serv = new Service();
                    RejectList[] rej = new RejectList[count];
                    Error err = new Error();

                    serv.state = "READY";
                    err.code = 0;
                    err.description = "";

                    for (int i = 0; i < count; i++)
                    {
                        if (!sources[i].IsEmpty)
                        {
                            rej[i].ID = i.ToString();
                            rej[i].disparity = CountDiff(PrepPic(masks[i], sources[i])).ToString();
                        }
                    }

                    resp.service = serv;resp.rejectList = rej;resp.error = err;
                    answer = makeJSON(resp);
                    break;
                default:
                    break;
            }


            return answer;
        }
        static void Main(string[] args)
        {
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
                    VSource[i].Retrieve(ResPic[i]);
                    if (!ResPic[i].IsEmpty)
                    {
                        //CvInvoke.Imshow(i.ToString(),PrepPic(Masks[i], ResPic[i]));
                        CvInvoke.Imshow(i.ToString(), ResPic[i]);
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
            CvInvoke.GaussianBlur(pic, temp, new System.Drawing.Size(3, 3), -9);

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
                } catch
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
        
        //prefix - {String} - URI kinda like @"http://192.168.2.10:6000"
        private void StartServer(string prefix)
        {
            server = new HttpListener();

            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException("prefix");
            server.Prefixes.Add(prefix);

            //запускаем север
            server.Start();
            string responseString = "";
            //Listaning
            while (server.IsListening)
            {
                //ожидаем входящие запросы
                HttpListenerContext context = server.GetContext();
                HttpListenerRequest request = context.Request;

                //запрос получен методом POST
                if (request.HttpMethod == "GET")
                {
                    responseString = FormResponse(ResPic, Masks, request.HttpMethod);
                }

              
             
                HttpListenerResponse response = context.Response;
                response.ContentType = "text/html; charset=UTF-8";

                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
            }
        }
        static string makeJSON(RequestForGET JSONobj)
        {
            return JsonConvert.SerializeObject(JSONobj);
        }

    }
}
