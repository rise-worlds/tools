using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace VersionMaker
{
	class Compress : IDisposable
	{
		private XmlReaderSettings settings;
		private List<Revision> revList;

		public Compress()
		{
			settings = new XmlReaderSettings();
			settings.Async = true;

			revList = new List<Revision>();
		}

		~Compress()
		{
			Dispose();
			settings = null;
			revList = null;
		}

		public void Dispose()
		{
			revList.Clear();
		}

		public void parseXmlStr(string data, string basePath)
		{
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.LoadXml(data);
				XmlNode root = doc.SelectSingleNode("lists").SelectSingleNode("list");
				foreach (XmlNode node in root.ChildNodes)
				{
					if (node.Attributes["kind"].Value == "dir") continue;
					Revision r = findRev(getFileRevision(node), getFileBasePath(node, basePath));
					if (null == r)
					{
						r = new Revision(node, basePath);
						revList.Add(r);
					}
					else
					{
						r.append(node);
					}
				}
			}
			catch(XmlException exce)
			{
				//Console.WriteLine(@"parse xml error, text:#1", data);
				Console.WriteLine(exce.ToString());
			}
			doc = null;
		}

		private Revision findRev(int rev, string path)
		{
			int length = revList.Count;
			Revision r;
			for (int i = 0; i < length; i++)
			{
				r = revList[i];
				if (r.revision == rev && r.path == path)
				{
					return r;
				}
			}
			return null;
		}

		public void writeFile(string path)
		{
			using (StreamWriter writer = new StreamWriter(path, false))
			{
				foreach (Revision rev in revList)
				{
					writer.WriteLine(rev.ToString());
				}
				
				writer.Flush();
				writer.Close();
			}
		}

		public static string getFileBasePath(XmlNode node, string basePath)
		{
			string path = node["name"].InnerText;
			int indexOf = path.LastIndexOf('/');
			if (0 <= indexOf)
			{
				path = (basePath.Length > 0 ?(basePath + "/") : "") + path.Substring(0, indexOf);
			}
			else
			{
				path = basePath;
			}

			return path;
		}

		public static string getFileName(XmlNode node)
		{
			string name = node["name"].InnerText;
			name = name.Substring(name.LastIndexOf('/') + 1);

			return name;
		}

		public static int getFileRevision(XmlNode node)
		{
			return Convert.ToInt32(node["commit"].Attributes["revision"].Value);
		}

		public static string getFileSize(XmlNode node)
		{
			return node["size"].InnerText;
		}
	}
}
