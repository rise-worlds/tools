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
		#region SVN
		public Revision(XmlNode node, string basePath)
		{
			revision = Compress.getFileRevisionSVN(node);
			path = Compress.getFileBasePathSVN(node, basePath);
			file = Compress.getFileName(node);
			size = Compress.getFileSizeSVN(node);
		}

		public void appendSVN(XmlNode node)
		{
			file += "," + Compress.getFileName(node);
			size += "," + Compress.getFileSizeSVN(node);
		}
		#endregion
		#region GIT
		public Revision(string[] attribs, string basePath)
		{
			revision = attribs[2];
			path = Compress.getFileBasePathGIT(attribs[4], basePath);
			file = Compress.getFileNameGIT(attribs[4]);
			size = attribs[3];
		}

		public void appendGIT(string file, string size)
		{
			this.file += "," + file;
			this.size += "," + size;
		}
		#endregion
	}
}
