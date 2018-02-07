using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using FluentFTP;
using Microsoft.Extensions.CommandLineUtils;
using System.Collections.Generic;

namespace AppFtpUpload
{
    class Program
    {
        private static ManualResetEvent m_reset = new ManualResetEvent(false);
        private static string file;
        private static int port;
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string ip = String.Empty;
#region CommandLineArgs
            var app = new CommandLineApplication();
            app.Name = "AppFtpUpload";
            app.Description = ".NET Core console app with file ftp upload.";

            app.HelpOption("-?|-h|--help");
            var pathOption = app.Option("-f|--file <PATH>", "file option value", CommandOptionType.SingleValue);
            var ipOption = app.Option("-ip <IP>", "cur lan ip", CommandOptionType.SingleValue);
            var portOption = app.Option("-port <PORT>", "ftp server port", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (pathOption.HasValue())
                {
                    file = pathOption.Value();
                }
                else
                {
                    app.ShowHint();
                    return -1;
                }
                if (portOption.HasValue())
                {
                    port = Convert.ToInt(portOption.Value());
                }
                else
                {
                    app.ShowHint();
                    return -1;
                }
                if (ipOption.HasValue())
                {
                    ip = ipOption.Value();
                }
                else
                {
                    IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                    if (addressList.Length > 1)
                        ip = addressList[1].ToString();
                    else if (addressList.Length == 1)
                        ip = addressList[0].ToString();
                    else
                    {
                        app.ShowHint();
                        return -1;
                    }
                }

                return 0;
            });

            if (0 != app.Execute(args))
                return;
#endregion
            //file = @"D:\SkyEntertain\bin\release\android\测试服-02.05.apk";

            ip = ip.Substring(0, ip.LastIndexOf('.') + 1);
            for (int i = 1; i < 255; i++)
            {
                string curIp = string.Format(@"{0}{1}", ip, i);
                ThreadPool.QueueUserWorkItem(new WaitCallback(Upload), curIp);
            }
            int maxWorkerThreads, workerThreads;
            int portThreads;
            while (true)
            {
                /*
                 GetAvailableThreads()：检索由 GetMaxThreads 返回的线程池线程的最大数目和当前活动数目之间的差值。
                 而GetMaxThreads 检索可以同时处于活动状态的线程池请求的数目。
                 通过最大数目减可用数目就可以得到当前活动线程的数目，如果为零，那就说明没有活动线程，说明所有线程运行完毕。
                 */
                ThreadPool.GetMaxThreads(out maxWorkerThreads, out portThreads);
                ThreadPool.GetAvailableThreads(out workerThreads, out portThreads);
                if (maxWorkerThreads - workerThreads == 0)
                {
                    Console.WriteLine("传输全部完成!");
                    break;
                }
            }
        }

        static void Upload(object ip)
        {
            using (FtpClient client = new FtpClient((string)ip, port, "anonymous", "rise.worlds@outlook.com"))
            {
                try
                {
                    client.Encoding = Encoding.GetEncoding("gb2312");
                    client.SocketKeepAlive = true;
                    client.ConnectTimeout = 3000;
                    client.Connect();

                    string filename = file.Substring(file.LastIndexOf('\\') + 1);
                    string path = string.Format(@"/Download/{0}", filename);
                    Console.WriteLine(@"正在上传到{0}", ip);

                    var progress = new Progress<double>();
                    progress.ProgressChanged += Progress_ProgressChanged;
                    client.UploadFile(file, path, FtpExists.Overwrite, false, FtpVerify.None, progress);
                    Console.WriteLine(@"上传到{0}完成", ip);
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }

        private static void Progress_ProgressChanged(object sender, double e)
        {
        }
    }
}
