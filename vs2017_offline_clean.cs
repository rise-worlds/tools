using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        string path = Directory.GetCurrentDirectory();
        if (path.EndsWith("\\") == false) path += "\\";
        string catalog = path + "catalog.json";
        if (File.Exists(catalog) == false)
        {
            Console.WriteLine("Please copy this program to the VS2017 offline folder.");
            return;
        }
        Console.WriteLine(path);
        string old = Directory.CreateDirectory(path + "- old -").FullName;
        var o = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(catalog));

        var folders = Directory.GetDirectories(path);
        var packages = o["packages"];
        foreach (var folder in folders)
        {
            var foldername = Path.GetFileName(folder);
            var splitstring = foldername.Split(',');
            bool found = false;
            if (splitstring.Length > 1)
            {
                foreach (var package in packages)
                {
                    if (package["id"].ToString() == splitstring[0] && 
                        package["version"] == splitstring[1].Split('=')[1].Trim())
                    {
                        found = true;
                        break;
                    }
                }
            }
            else
            {
                found = true;
                Console.WriteLine("not found in packages : " + foldername);
            }

            if (found == false)
            {
                // move directory
                Console.WriteLine("moving folder : " + foldername);
                Directory.Move(folder, old + "\\" + foldername);
            }
        }
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}