using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TAIniEditor
{

    class IniParser
    {

        private static bool ParseCategory(string line, out string name)
        {
            name = null;
            if (line.Length < 2) { return false; }
            if (line[0] != '[') { return false; }
            if (line[line.Length - 1] != ']') { return false; }
            name = line.Substring(1, line.Length - 2);
            return true;
        }

        private static bool ParseValue(string line, out string name, out string value)
        {
            name = null;
            value = null;
            if (line.IndexOf('=') < 0) { return false; }
            string[] s = line.Split("=".ToCharArray(), 2);
            if (s.Length != 2)
            {
                return false;
            }
            name = s[0].Trim();
            value = s[1].Trim();
            if (name.Length == 0)
            {
                // TODO log this
                name = null;
                value = null;
                return false;
            }
            return true;
        }

        private static string FormatOption(Option o)
        {
            return String.Format("{0}={1}{2}", o.Name, o.Value, System.Environment.NewLine);
        }
        private static string FormatCategory(Category c)
        {
            string s = String.Format("[{0}]{1}", c.Name, System.Environment.NewLine);
            foreach (Option o in c.Options)
            {
                s += FormatOption(o);
            }
            return s;
        }

        public static string Write(RootContext root)
        {
            string s = "";
            foreach (Option o in root.Options)
            {
                s += FormatOption(o);
            }
            foreach (Category c in root.Categories)
            {
                s += FormatCategory(c);
            }
            return s;
        }

        public static RootContext ReadPath(string path)
        {
            string s = File.ReadAllText(path);
            return ReadString(s);
        }

        public static RootContext ReadString(string s) {

            string[] lines = s.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            RootContext root = new RootContext();
            Category c = null;

            foreach (string line in lines)
            {
                string thisLine = line.Trim();
                if (thisLine == String.Empty) { continue; }
                string name, value;
                if (ParseCategory(thisLine, out name))
                {
                    c = new Category() { Name = name };
                    root.Categories.Add(c);
                }
                else if (ParseValue(thisLine, out name, out value))
                {
                    Option o = new Option()
                    {
                        Name = name,
                        Value = value,
                        Category = c
                    };
                    if (c != null) { c.Options.Add(o); }
                    else { root.Options.Add(o); }
                }
                else
                {
                    // do some logging here 
                }
            }

            return root;

        }
    }



    public class Ini
    {
        
        public string Path { get; set; }
        public RootContext Root { get; set; }

        private bool loaded = false;

        public Ini(string path)
        {
            Path = path;
            Root = IniParser.ReadPath(path);
        }

        public void Save() {
            string s = IniParser.Write(Root);
            File.WriteAllText(Path, s);
        }


        public bool ParseBool(string value)
        {
            // TODO this should respect the casing convention
            // already present
            return value.ToLower() == "true";
        }
        public string FormatBool(bool value)
        {
            // TODO this should respect the casing convention
            // already present
            if (value) { return "True"; }
            else { return "False"; }
        }



    }
}
