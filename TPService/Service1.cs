using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TPService.Utilities;

namespace TPService
{
    public partial class Service1 : ServiceBase
    {
        private static int interval = Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);
        string LocalCnString = ConfigurationManager.ConnectionStrings["LocalCnString"].ConnectionString;
        string CloudCnString = ConfigurationManager.ConnectionStrings["LocalCloudString"].ConnectionString;
        string ImageLocalPath = ConfigurationManager.AppSettings["ImageLocalPath"];

        DBConnect objLocal;
        DBConnect objCloud;
        string MacId = GetSystemInfo();
        bool IsInternetConnected = false;

        public Service1()
        {
            InitializeComponent();
            objLocal = new DBConnect(LocalCnString);
            objCloud = new DBConnect(CloudCnString);
            IsInternetConnected = CheckNet();
            CaptureActiveScreen();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                ErrorLog.WriteToFile("Service started time.......");

                this.timer = new System.Timers.Timer(interval * 1000);  // 30000 milliseconds = 30 seconds
                this.timer.AutoReset = true;
                this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
                this.timer.Start();

                if (IsInternetConnected)
                {
                    objCloud.InsertLogInfo(DateTime.Now, MacId, 1);
                }
                else
                {
                    objLocal.InsertLogInfo(DateTime.Now, MacId, 1);
                }
                CaptureActiveScreen();
            }
            catch (Exception e)
            {
                ErrorLog.WriteToFile("Exception while starting service time.......");
                ErrorLog.ErrorLogging(e);
                //write log informations
            }
        }

        protected override void OnStop()
        {
            try
            {
                this.timer.Stop();
                this.timer = null;

                if (IsInternetConnected)
                {
                    objCloud.InsertLogInfo(DateTime.Now, MacId, 0);
                }
                else
                {
                    objLocal.InsertLogInfo(DateTime.Now, MacId, 0);
                }
            }
            catch (Exception e)
            {

                ErrorLog.WriteToFile("Exception while Stoping service time.......");
                ErrorLog.ErrorLogging(e);
                //write log informations
            }
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                IsInternetConnected = CheckForInternetConnection();
                CaptureActiveScreen();
            }
            catch
            {
                //write the log
            }
        }

        public async void CaptureActiveScreen()
        {
            try
            {
                if (IsInternetConnected)
                {
                    var result = UploadDataOnCloud();
                }
                ScreenCapture obj = new ScreenCapture();

                Image image = obj.CaptureScreen();
                string ImageName = DateTime.Now.ToString("yyyyMMddHHmmss"); // case sensitive  

                SaveImageLocal(ImageLocalPath, ImageName, image);

                //objLocal.InsertImage(DateTime.Now, MacId, ImageName, "0", ImageStream);

                await Task.Delay(TimeSpan.FromSeconds(3));
            }
            catch (Exception e)
            {

                ErrorLog.WriteToFile("Exception while CaptureActiveScreen.......");
                ErrorLog.ErrorLogging(e);
                //write log informations
            }
        }

        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool CheckNet()
        {
            try
            {
                int desc;
                return InternetGetConnectedState(out desc, 0);
            }
            catch (Exception e)
            {

                ErrorLog.WriteToFile("Exception while CheckNet.......");
                ErrorLog.ErrorLogging(e);
                return false;
                //write log informations
            }
        }

        public bool CheckForInternetConnection()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 3000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception e)
            {

                ErrorLog.WriteToFile("Exception while CheckForInternetConnection.......");
                ErrorLog.ErrorLogging(e);
                return false;
                //write log informations
            }
        }

        public static void OpenConnectionServer()
        {
            try
            {

                TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 80);

                server.Start();
                Console.WriteLine("Server has started on 115.124.109.110:8080.{0}Waiting for a connection...", Environment.NewLine);

                TcpClient client = server.AcceptTcpClient();

                Console.WriteLine("A client connected.");

            }
            catch (Exception e)
            {

                ErrorLog.WriteToFile("Exception while OpenConnectionServer.......");
                ErrorLog.ErrorLogging(e);
                //write log informations
            }
        }

        public static string GetSystemInfo()
        {
            string result = string.Empty;
            try
            {
                //string MachineName = System.Environment.MachineName;
                string dnsName = System.Net.Dns.GetHostName();

                var macAddr =
                  (
                      from nic in NetworkInterface.GetAllNetworkInterfaces()
                      where nic.OperationalStatus == OperationalStatus.Up
                      select nic.GetPhysicalAddress().ToString()
                  ).FirstOrDefault();

                result = macAddr;

                #region get all system level informations.
                //string[] queryItems = { "Win32_ComputerSystem", "b.Win32_DiskDrive", "c.Win32_OperatingSystem", "d.Win32_Processor", "e.Win32_ProgramGroup", "f.Win32_SystemDevices", "g.Win32_StartupCommand" };
                //ManagementObjectSearcher searcher;
                //int i = 0;
                //ArrayList arrayListInformationCollactor = new ArrayList();
                //try
                //{
                //    searcher = new ManagementObjectSearcher("SELECT * FROM " + queryItems[0]);
                //    foreach (ManagementObject mo in searcher.Get())
                //    {
                //        i++;
                //        PropertyDataCollection searcherProperties = mo.Properties;
                //        foreach (PropertyData sp in searcherProperties)
                //        {
                //            arrayListInformationCollactor.Add(sp);
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //}  
                #endregion                
            }
            catch (Exception e)
            {

                ErrorLog.WriteToFile("Exception while GetSystemInfo.......");
                ErrorLog.ErrorLogging(e);
                //write log informations
            }
            return result;
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    imageIn.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
            catch (Exception e)
            {

                ErrorLog.WriteToFile("Exception while ImageToByteArray.......");
                ErrorLog.ErrorLogging(e);
                return null;
                //write log informations
            }
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
            catch (Exception e)
            {

                ErrorLog.WriteToFile("Exception while byteArrayToImage.......");
                ErrorLog.ErrorLogging(e);
                return null;
                //write log informations
            }
        }

        public async Task<bool> UploadDataOnCloud()
        {
            try
            {
                string SelectQuery = "select * from tbltoposcreens";
                DataTable dtRows = objLocal.Select(SelectQuery);

                string DeleteQuery = "delete from tbltoposcreens";
                objLocal.Delete(DeleteQuery);

                List<string> Rows = new List<string>();
                for (int i = 0; i < dtRows.Rows.Count; i++)
                {
                    objCloud.InsertImage(Convert.ToDateTime(dtRows.Rows[i]["fldDateTime"]), Convert.ToString(dtRows.Rows[i]["fldMacID"]), Convert.ToString(dtRows.Rows[i]["fldScreenshot"]), Convert.ToString(dtRows.Rows[i]["fldScreenShotProcessedYesNo"]), (byte[])dtRows.Rows[0]["fldScreenBlob"]);
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }

                return true;
            }
            catch (Exception e)
            {

                ErrorLog.WriteToFile("Exception while byteArrayToImage.......");
                ErrorLog.ErrorLogging(e);
                return false;
                //write log informations
            }
        }

        public static bool SaveImageLocal(string ImageLocalPath, string ImageName, Image Screen)
        {
            bool result = false;
            try
            {
                if (!Directory.Exists(ImageLocalPath))
                {
                    Directory.CreateDirectory(ImageLocalPath);
                }
                Screen.Save(ImageLocalPath + ImageName + ".png", ImageFormat.Png);
            }
            catch (Exception e)
            {
                ErrorLog.WriteToFile("Exception while SaveImageLocal.......");
                ErrorLog.ErrorLogging(e);
                return false;
                //write log informations
            }
            return result;
        }
    }
}
