using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace VersionMaker
{
	class Revision
	{
		public string revision;
		public string path;
		public string file;
		public string size;

		override public string ToString()
		{
			return revision + "|" + path + "|" + file + "|" + size;
		}

#if SVN
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
#else
		public Revision(string[] attribs, string basePath)
		{
			revision = attribs[2];
			path = Compress.getFileBasePath(attribs[4], basePath);
			file = Compress.getFileName(attribs[4]);
			size = attribs[3];
		}

		public void append(string file, string size)
		{
			this.file += "," + file;
			this.size += "," + size;
		}
#endif
	}
}
