using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace TAIniEditor
{

    class IniSpecOption
    {
        string Name;
        string Category;
        OptionType Type;
    }

    /// <summary>
    /// Specification of allowed options for an ini
    /// </summary>
    class IniSpec
    {

        private string Path;
        XPathDocument docNav;
        XPathNavigator nav;

        public IniSpec(string path)
        {
            Path = path;
            docNav = new XPathDocument(path);
            nav = docNav.CreateNavigator();
        }

        private static string QueryStringFor(Option o) {
            string queryFmt = "/ini/category[@name=\"{0}\"]/option[@name=\"{1}\"]";
            return String.Format(queryFmt, o.Category.Name, o.Name);
        }

        public string DescriptionFor(Option o) {
            string query = "string(" + QueryStringFor(o) + "/description/text())";
            string result = (string)nav.Evaluate(query);
            return result;
        }
        /// <summary>
        /// Because these might be floats or integers, we'll just return the
        /// string verbatim and let the caller figure out what they want to do
        /// with it. Easier than messing with generic types.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public bool MinMaxFor(Option o, out double min, out double max)
        {
            min = 0;
            max = 0;
            string qs = QueryStringFor(o);
            string q1 = "string(" + qs + "/min/text())";
            string q2 = "string(" + qs + "/max/text())";

            string minS = (string)nav.Evaluate(q1);
            string maxS = (string)nav.Evaluate(q2);
            minS.Trim();
            maxS.Trim();
            if (minS == String.Empty || maxS == String.Empty)
            {
                return false;
            }
            min = System.Convert.ToDouble(minS);
            max = System.Convert.ToDouble(maxS);
            return true;
        }

        public bool IntervalFor(Option o, out double interval)
        {
            double x1, x2;
            interval = 0;
            if (!MinMaxFor(o, out x1, out x2)) { return false; }
            string intervalS = (string)nav.Evaluate("string(" + QueryStringFor(o) + "/interval/text())");
            if (intervalS.Trim() == String.Empty) { return false; }

            try
            {
                interval = System.Convert.ToDouble(intervalS.Trim());
            }
            catch (Exception e)
            {
                return false;
            }
            return true;

        }


        public bool ValidateFor(Option o, string value, out string message)
        {
            message = null;
            // don't bother validating this at the moment, but in future we'll have to check
            // it's enable-able
            if (o.Type == OptionType.Bool) { return true; }
            else if (o.Type == OptionType.Int || o.Type == OptionType.Float)
            {
                double min, max;
                if (MinMaxFor(o, out min, out max))
                {
                    double dblVal;
                    try
                    {
                         dblVal = System.Convert.ToDouble(value);
                    }
                    catch (Exception e)
                    {
                        message = String.Format("Unhandled exception");
                        return false;
                    }

                    if (dblVal >= min && dblVal <= max)
                    {
                        return true;
                    }
                    else {
                        message = String.Format("Must be between {0} and {1}", min, max);
                        return false;
                    }
                }
            }
            return true;
        }
     
    }
}
