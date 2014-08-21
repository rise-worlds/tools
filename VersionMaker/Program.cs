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
						string data = string.Empty;
						if (paths[0] == "svn")
						{
							data = runSVN(paths[1]);
							compress.parseStrSVN(data, paths.Length <= 2 ? "" : paths[2]);
						}
						else if (paths[0] == "git")
						{
							data = runGIT(paths[1]);
							compress.parseStrGIT(data, paths.Length <= 2 ? "" : paths[2]);
						}
						else
						{
							Console.WriteLine("Wrong config format.");
						}
					}
					compress.writeFile(name);
				}
				reader.Close();
			}

			Console.WriteLine(@"makeeeeee success.");
			Console.ReadKey(true);
		}

		static string runSVN(string path)
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
		static string runGIT(string path)
		{
			using (Process process = new System.Diagnostics.Process())
			{
				process.StartInfo.FileName = "git";
				process.StartInfo.Arguments = "ls-tree -l -r --abbrev HEAD";

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
