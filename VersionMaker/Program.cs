using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VersionMaker
{
	class Program
	{
		static void Main(string[] args)
		{
			string path;
			if (0 == args.Length) path = "config.txt";
			else path = args[0];

			Console.WriteLine(@"start make revision.");
			using (StreamReader reader = new StreamReader(path))
			{
				string name = reader.ReadLine();
				int count = Convert.ToInt32(reader.ReadLine());
				using (Compress compress = new Compress())
				{
					for (int i = 0; i < count; i++)
					{
						string line = reader.ReadLine();
						string[] paths = line.Split(',');
						compress.parseXmlStr(run(paths[0]), paths.Length <= 1 ? "" : paths[1]);
					}
					compress.writeFile(name);
				}
				reader.Close();
			}

			Console.WriteLine(@"makeeeeee success.");
			Console.ReadKey(true);
		}

		static string run(string path)
		{
			using (Process process = new System.Diagnostics.Process())
			{
				process.StartInfo.FileName = "svn";
				process.StartInfo.Arguments = "list -r HEAD -R --xml";
				process.StartInfo.WorkingDirectory = path;
				// 必须禁用操作系统外壳程序  
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;

				process.Start();

				string output = process.StandardOutput.ReadToEnd();

				//if (String.IsNullOrEmpty(output) == false)
				//	Console.Out.Write(output + "\r\n");

				process.WaitForExit();
				process.Close();

				return output;
			}
		}
	}
}
