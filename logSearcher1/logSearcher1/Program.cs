using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace logSearcher1
{
    class Program
    {
        public class PcData
        {
            public string pc_ip;
            public string win_login;
            public string win_password;
            public string log_path;

            public PcData(string pc_ip= "192.168.1.141", string win_login="peti", string win_password="Mk481'24")
            {
                this.pc_ip = pc_ip;
                this.win_login = win_login;
                this.win_password = win_password;
                this.log_path = this.pc_ip+"\\sambashare";
            }
        }

        private class WorkerArgs
        {
            public DateTime firstDay;
            public DateTime lastDay;
            public string serialNumber;
            public WorkerArgs(string first, string last, PcData d)
            {
                this.firstDay = DateTime.ParseExact(first, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                this.lastDay = DateTime.ParseExact(last, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                this.serialNumber = "NA00112Q2601C099070019";
            }
        }
        static void Main(string[] args)
        {
            PcData pcData = new PcData();
            
            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Number of threads: {1;2;4;7;14;28}");
            int num=Convert.ToInt32(Console.ReadLine());//Number of threads.

            string[] result=new string[num];
            DateTime firstDay = DateTime.ParseExact("2020-03-01", "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
            DateTime lastDay = DateTime.ParseExact("2020-03-28", "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
            int max = 28; //Number of days.
            
            // Login to PC            
            string strCmdText = "cmdkey /add:" + pcData.pc_ip.Trim(' ') + " /user:" + pcData.win_login.Trim(' ') + " /pass:" + pcData.win_password.Trim(' ');
            Process newProcess = System.Diagnostics.Process.Start("CMD.exe", strCmdText);
            newProcess.Kill();

            sw.Start();
            
            Thread[] szalak = new Thread[num];
 
            for (int i = 0; i < num; i++)
            {
                int j = i;
                lastDay = firstDay.AddDays((max / num) - 1);
                WorkerArgs arg1 = new WorkerArgs(firstDay.ToString("yyyy-MM-dd"), lastDay.ToString("yyyy-MM-dd"), pcData);
                szalak[j] = new Thread(delegate () {
                       result[j] = FinderThreadFn(arg1,pcData);
                    });

                 firstDay = lastDay.AddDays(1);
            }

            foreach (Thread t in szalak)
                t.Start();
            foreach (Thread t in szalak)
                t.Join();

            sw.Stop();
            for (int i = 0; i < num; i++)
            {
                Console.WriteLine("Thread:"+i+" The serial number founded in the log file name:" + result[i]);
            }
            Console.WriteLine("Time elapsed:"+sw.Elapsed);
        }

        private static string FinderThreadFn(WorkerArgs wa, PcData pd)
        {
            //Path, where the right filename founded.
            string foundedFileSource = "";
            
            // Iterate date
             DateTime actualDay = wa.firstDay;
            while (actualDay <= wa.lastDay)
            {
               
                string path = @"\\"+pd.log_path + "\\" + actualDay.ToString("yyyyMMdd");
                DirectoryInfo d = new DirectoryInfo(path);
                System.Diagnostics.Debug.WriteLine(path);
                if (Directory.Exists(path))
                {
                    FileInfo[] files = d.GetFiles("*.log");

                    foreach (var file in files)
                    {
                        if (file.Name.Contains(wa.serialNumber))
                        {
                            foundedFileSource = foundedFileSource+"\n"+ path + "\\" + file.Name; 
                        }
                    }
                }
                actualDay = actualDay.AddDays(1);
            }
            return foundedFileSource;
        }


    }
}
