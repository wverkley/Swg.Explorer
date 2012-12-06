using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Wxv.Swg.Common
{
    /// <summary>
    /// A CSV String Formatting &amp; Parsing class that also provides some static methods for quick
    /// access
    /// 
    /// Example of proper formatting use :
    /// Console.WriteLine (String.Format (new CsvUtil (), "{0} converts to {1:csv}", myStr));
    /// 
    /// Example of quicky use :
    /// Console.WriteLine (CsvUtil.Format (myStr));
    /// or
    /// Console.WriteLine (CsvUtil.Format (myStrCollection));
    /// </summary>
    public class CsvUtil
    {
        public const char Seperator = ',';
        public const char Quote = '"';
        public const string DoubleQuote = "\"\"";
        public const string NewLine = "\r\n";


        // The grunt work method to convert a string to csv
        public static string Format(object value)
        {
            if (value == null) return string.Empty;

            string result = value.ToString().Replace(Quote.ToString(), DoubleQuote);
            if (result.IndexOf(Seperator) != -1
             || result.IndexOf(Quote) != -1
             || result.IndexOf(Environment.NewLine) != -1)
                result = Quote + result + Quote;
            return result;
        }

        // Convert a collection to a CSV string list
        public static string Format(ICollection collection)
        {
            StringBuilder result = new StringBuilder();
            bool first = true;
            foreach (object value in collection)
            {
                if (first)
                    first = false;
                else
                    result.Append(Seperator);
                result.Append(Format(value));
            }
            return result.ToString();
        }

        private static string ParseField(string str)
        {
            if ((str.Length >= 2)
             && (str[0] == Quote)
             && (str[str.Length - 1] == Quote))
            {
                if (str.Length == 2)
                    str = string.Empty;
                else
                    str = str.Substring(1, str.Length - 2);
            }
            str = str.Replace(DoubleQuote, Quote.ToString());
            return str;
        }

        private static bool Parse(string str, List<string> result)
        {
            if (result != null)
                result.Clear();

            int fieldStartPos = 0;
            int pos = 0;
            bool inQuote = false;
            while (pos < str.Length)
            {
                // comma seperates a field, unless we are in a quote
                if ((str[pos] == Seperator) && !inQuote)
                {
                    if (result != null)
                        result.Add(ParseField((
                            pos - fieldStartPos) > 0
                            ? str.Substring(fieldStartPos, pos - fieldStartPos)
                            : string.Empty));
                    fieldStartPos = pos + 1;
                }
                else if (str[pos] == Quote)
                    inQuote = !inQuote;

                pos++;
            }

            // we are still escaped by a quote, so we cant be complete
            if (inQuote)
                return false;

            // add the last field
            if (result != null)
                result.Add(ParseField((
                    pos - fieldStartPos) > 0
                    ? str.Substring(fieldStartPos, pos - fieldStartPos)
                    : string.Empty));

            return true;
        }

        /// <summary>
        /// Parse this string into a CSV record.  Returns null if its not complete.
        /// </summary>
        public static string[] Parse(string str)
        {
            List<string> result = new List<string>();
            if (Parse(str, result))
                return result.ToArray();
            else
                return null;
        }

        /// <summary>
        /// Does this string represent a complete CSV record?
        /// </summary>
        public static bool ParseIsComplete(string str)
        {
            return Parse(str, (List<string>) null);
        }

        /// <summary>
        /// Parses a CSV record from a TextReader.  Returns null if it doesnt find a complete line 
        /// or its at the end.
        /// </summary>
        public static string[] Parse(TextReader reader)
        {
            string line = null;
            string completeLine = null;
            bool complete = false;
            while (!complete && ((line = reader.ReadLine()) != null))
            {
                if (completeLine == null)
                    completeLine = line;
                else
                    completeLine += NewLine + line;

                complete = ParseIsComplete(completeLine);
            }

            if (!complete)
                return null;

            List<string> result = new List<string>();
            Parse(string.IsNullOrEmpty(completeLine) ? string.Empty : completeLine, result);
            return result.ToArray();
        }

        public static Array Parse(string str, Type destinationType)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(destinationType);
            if (!typeConverter.CanConvertFrom(typeof(string)))
                throw new Exception("This type cannot be parsed");

            string[] strResults = Parse(str);
            ArrayList result = new ArrayList();
            foreach (string strResult in strResults)
                result.Add(typeConverter.ConvertFromString(strResult));
            return result.ToArray(destinationType);
        }
    }
}
