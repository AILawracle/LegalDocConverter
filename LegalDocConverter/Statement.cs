using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LegalDocConverter
{
    class Statement
    {
        Dictionary<string, string> dict;

        public Statement(Dictionary<string, string> dict)
        {
            this.dict = new Dictionary<string, string>(dict);
            
        }

        /// <summary>
        /// Replace all the parts that match with the fileds of  {the Statement.
        /// </summary>
        /// <param name="output"></param>
        public void SearchAndReplace(string output)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(output, true))
            {
                string docText = null;
                Regex regexText;

                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                foreach (KeyValuePair<string, string> kvp in dict)
                {
                    regexText = new Regex(@"&lt;" + kvp.Key + @"&gt;");
                    docText = regexText.Replace(docText, kvp.Value);
                }
               

                regexText = new Regex(@"&lt;time&gt;");
                DateTime localDate = DateTime.Now;
                string time = localDate.ToString("dd-MMMM-yyyy");
                docText = regexText.Replace(docText, time);

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
        }

    }
}
