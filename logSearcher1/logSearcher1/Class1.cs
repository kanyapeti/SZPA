//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Data.SqlClient;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System;
//using System.IO;
//using System.Diagnostics;
//using System.Net;
//using System.Collections.Concurrent;
//using System.Threading;

//namespace Serilnumber_searcher
//{

//    public partial class Form1 : Form
//    {
//        private BackgroundWorker FindMainWorker = new BackgroundWorker();


//        public Form1()
//        {
//            InitializeComponent();

//            FindMainWorker.DoWork += FindMainWorker_DoWork;
//            FindMainWorker.RunWorkerCompleted += FindMainWorker_RunWorkerCompleted;

//            dateTimePicker2.Value = DateTime.Now;
//            dateTimePicker1.Value = dateTimePicker2.Value.AddDays(-14);
//        }

//        private void FindMainWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        {
//            if (FoundItems.Count == 0)
//            {
//                listBox1.Items.Add("No item found");
//                return;
//            }

//            FoundItem fi;
//            while (FoundItems.TryDequeue(out fi))
//            {
//                Links.Items.Add(fi.fileName);
//            }
//        }

//        private void FindMainWorker_DoWork(object sender, DoWorkEventArgs e)
//        {
//            List<Thread> threads = new List<Thread>();
//            MainArgs ma = (MainArgs)e.Argument;


//            foreach (PcData data in ma.datas)
//            {


//                threads.Add(new Thread(() => FindThreadFn(new WorkerArgs(ma.firstDay,
//                                                                         ma.lastDay,
//                                                                         ma.firstDay,
//                                                                         ma.serialNumber,
//                                                                         data))));
//            }

//            foreach (Thread t in threads)
//            {
//                t.Start();
//            }

//            foreach (Thread t in threads)
//            {
//                t.Join();
//            }
//        }

//        private void button1_Click(object sender, EventArgs e)
//        {
//            List<PcData> datas = LoadProduct();
//            string SN = " ";
//            SN = textBox1.Text;
//            SN = SN.Trim(' ');
//            Links.Items.Clear();
//            listBox1.Items.Clear();

//            foreach (PcData data in datas)
//            {
//                System.Diagnostics.Debug.WriteLine(data.mes_id);
//            }

//            var firstDate = dateTimePicker1.Value;
//            var lastDate = dateTimePicker2.Value;
//            if (firstDate > lastDate)
//            {

//                MessageBox.Show("Wrong date.");
//                return;
//            }

//            if (SN == " " || SN.Contains("NA") == false) return;
//            try
//            {
//                FindMainWorker.RunWorkerAsync(new MainArgs(firstDate, lastDate, SN, datas));
//            }
//            catch
//            {
//                return;
//            }
//        }
//        public static List<PcData> LoadProduct()
//        {
//            List<PcData> datas = new List<PcData>();
//            string Query = "SELECT DISTINCT([pc_ip]),[win_login],[win_password],[log_path] FROM [MIB2+HIGH].[dbo].[kp_flash] ";
//            SqlConnection Con = new SqlConnection("user id= mib2high_read; password= V86o291z0P; server=172.24.222.164\\mmd; database=MIB2+HIGH; connection timeout=8");
//            try
//            {
//                Con.Open();
//            }
//            catch
//            {
//                return null;
//            }
//            SqlCommand Cmd = new SqlCommand(Query, Con);
//            //Cmd.Parameters.AddWithValue("@TEMPSN", NACode);
//            SqlDataReader Rd = Cmd.ExecuteReader();

//            while (Rd.Read())
//            {
//                PcData data = new PcData();
//                if (!Rd.IsDBNull(0)) data.pc_ip = (string)Rd["pc_ip"];
//                if (!Rd.IsDBNull(1)) data.win_login = (string)Rd["win_login"];
//                if (!Rd.IsDBNull(2)) data.win_password = (string)Rd["win_password"];
//                if (!Rd.IsDBNull(3)) data.log_path = (string)Rd["log_path"];
//                datas.Add(data);
//            }
//            Con.Close();
//            return datas;
//        }


//        public delegate void FoundLogHandler(PcData data, string fileName);
//        public static event FoundLogHandler OnLogFound;

//        private static ConcurrentQueue<string> foundFiles;

//        private static List<BackgroundWorker> WorkerList = new List<BackgroundWorker>();

//        private class MainArgs
//        {
//            public DateTime firstDay;
//            public DateTime lastDay;
//            public string serialNumber;
//            public List<PcData> datas;
//            public MainArgs(DateTime fd, DateTime ld, string sn, List<PcData> d)
//            {
//                firstDay = fd;
//                lastDay = ld;
//                serialNumber = sn;
//                datas = d;
//            }
//        }

//        private class WorkerArgs
//        {
//            public DateTime firstDay;
//            public DateTime lastDay;
//            public DateTime actualDay;
//            public string serialNumber;
//            public PcData data;
//            public WorkerArgs(DateTime fd, DateTime ld, DateTime ad, string sn, PcData d)
//            {
//                firstDay = fd;
//                lastDay = ld;
//                actualDay = ad;
//                serialNumber = sn;
//                data = d;
//            }
//        }

//        private List<PcData> foundDatas = new List<PcData>();

//        private ConcurrentQueue<FoundItem> FoundItems = new ConcurrentQueue<FoundItem>();

//        private class FoundItem
//        {
//            public PcData data;
//            public string fileName;
//            public FoundItem(PcData p, string f)
//            {
//                data = p;
//                fileName = f;
//            }
//        }


//        private void FindThreadFn(WorkerArgs wa)
//        {
//            // Login to PC            
//            NetworkCredential theNetworkCredential = new NetworkCredential(wa.data.win_login.Trim(' '), wa.data.win_password.Trim(' '));
//            string strCmdText = "cmdkey /add:" + wa.data.pc_ip.Trim(' ') + " /user:" + wa.data.win_login.Trim(' ') + " /pass:" + wa.data.win_password.Trim(' ');
//            Process newProcess = System.Diagnostics.Process.Start("CMD.exe", strCmdText);
//            newProcess.Kill();

//            // Iterate date
//            wa.actualDay = wa.firstDay;
//            while (wa.actualDay.ToString() != wa.lastDay.ToString())
//            {
//                string path = wa.data.log_path;
//                string[] separated = path.Split(' ');
//                path = separated[0] + wa.actualDay.ToString("yyyyMMdd");
//                DirectoryInfo d = new DirectoryInfo(path);
//                System.Diagnostics.Debug.WriteLine(path);
//                if (Directory.Exists(path))
//                {
//                    FileInfo[] files = d.GetFiles("*.log");

//                    foreach (var file in files)
//                    {
//                        if (file.Name.Contains(wa.serialNumber))
//                        {
//                            PcData foundData = new PcData();
//                            foundData = wa.data;
//                            string foundFile = path + "\\" + file.Name;
//                            FoundItems.Enqueue(new FoundItem(wa.data, foundFile));
//                        }
//                    }
//                }
//                wa.actualDay = wa.actualDay.AddDays(1);
//            }
//        }


//        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
//        {

//        }

//        private void Links_SelectedValueChanged(object sender, EventArgs e)
//        {

//        }

//        private void Links_MouseDoubleClick(object sender, MouseEventArgs e)
//        {
//            if (Links.SelectedItem != null)
//            {

//                string path = Links.SelectedItem.ToString();

//                path.Trim();

//                //File.Open(@"\\10.0.104.16\usr\RCC2_Flashing_System\Log\20190907\NA00112Q01C099070073_02__OK_20190907003429.log", FileMode.Open);
//                Process.Start(path);
//            }
//        }

//        private void Nothing(object sender, EventArgs e)
//        {

//        }
//    }

//}