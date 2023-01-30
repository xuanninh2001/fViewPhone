using Leaf.xNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Octopus_FB
{
    public static class Extension
    {
 
		public static Random rnd = new Random();

		public static int RanNumber(double minNumber, double maxNumber)
		{
			double value = 0;
			try
			{
				value = rnd.NextDouble() * (maxNumber - minNumber) + minNumber;
			}
			catch
			{
			}
			return Convert.ToInt32(value);
		}

		  
	 
 		public static string RunCMD_Result(string cmdText)
		{
			string result = "";
			try
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo
				{
					FileName =  "adb.exe",
					Arguments = cmdText,
					CreateNoWindow = true,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
				};
				process.Start();
				StreamReader standardOutput = process.StandardOutput;
				result = standardOutput.ReadToEnd();
				process.WaitForExit();
			}
			catch
			{
				result = "";
			}

			return result;
		}
		public static DialogResult NotifyMsg(string text, int type = 0, string title = "Thông báo")
		{
			return MessageBox.Show(text, title, type == 0 ? MessageBoxButtons.OK : MessageBoxButtons.YesNo, MessageBoxIcon.Information);
		}
		 


	}
}
