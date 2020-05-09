using System;
using System.IO;

namespace createFolders
{
    class Program
    {
        static void Main(string[] args)
        {
            for(int i = 1; i < 9; i++)
            {
                for(int j=10;j<99;j++)
                EvaluatePath(@"\\192.168.1.139\sambashare\2020030" + i.ToString()+ "\\NA00112Q"+i.ToString()+"01C0990700"+j+"_02__OK.log");
            }

            for (int i = 10; i < 29; i++)

            {
                for (int j = 10; j < 99; j++)
                    EvaluatePath(@"\\192.168.1.139\sambashare\202003" + i.ToString() + "\\NA00112Q" + i.ToString() + "01C0990700" + j + "_02__OK.log");
            }
        }

        private static String EvaluatePath(String path)
        {
           try
            {
                // Try to create the directory.
                File.Create(path);
            }
            catch (IOException ioex)
            {
                Console.WriteLine(ioex.Message);
                return "";
            }
            return path;
        }
    }
}
