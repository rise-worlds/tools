using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VersionMaker
{
	class Revision
	{
		public int revision;
		public string path;
		public string file;
		public string size;

		public Revision(XmlNode node, string basePath)
		{
			revision = Compress.getFileRevision(node);
			path = Compress.getFileBasePath(node, basePath);
			file = Compress.getFileName(node);
			size = Compress.getFileSize(node);
		}

		public void append(XmlNode node)
		{
			file += "," + Compress.getFileName(node);
			size += "," + Compress.getFileSize(node);
		}

		override public string ToString()
		{
			return revision + "|" + path + "|" + file + "|" + size;
		}
	}
}
