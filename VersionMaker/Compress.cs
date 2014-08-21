using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

		private Revision findRev(string rev, string path)
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

#if SVN
		public void parseStr(string data, string basePath)
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

		public static string getFileBasePath(XmlNode node, string basePath)
		{
			string path = node["name"].InnerText;
			int indexOf = path.LastIndexOf('/');
			if (0 <= indexOf)
			{
				path = (basePath.Length > 0 ? (basePath + "/") : "") + path.Substring(0, indexOf);
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

		public static string getFileRevision(XmlNode node)
		{
			return node["commit"].Attributes["revision"].Value;
		}

		public static string getFileSize(XmlNode node)
		{
			return node["size"].InnerText;
		}
#else
		public void parseStr(string data, string basePath)
		{
			string[] nodes = data.Split('\n');
			int length = nodes.Length;
			for (int i = 0; i < length; i++)
			{
				string[] attribs = Regex.Split(nodes[i], @"\s+");
				if (attribs.Length <= 1 || attribs[1] != "blob") continue;
				Revision r = findRev(attribs[2], getFileBasePath(attribs[4], basePath));
				if (null == r)
				{
					r = new Revision(attribs, basePath);
					revList.Add(r);
				}
				else
				{
					r.append(getFileName(attribs[4]), attribs[3]);
				}
			}
		}

		public static string getFileBasePath(string path, string basePath)
		{
			int indexOf = path.LastIndexOf('/');
			if (0 <= indexOf)
			{
				path = (basePath.Length > 0 ? (basePath + "/") : "") + path.Substring(0, indexOf);
			}
			else
			{
				path = basePath;
			}

			return path;
		}

		public static string getFileName(string path)
		{
			string name = path.Substring(path.LastIndexOf('/') + 1);

			return name;
		}
#endif
	}
}
