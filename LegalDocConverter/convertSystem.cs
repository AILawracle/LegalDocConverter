using System;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LegalDocConverter
{
    class convertSystem
    {


       private Dictionary<string, string> dict = new Dictionary<string, string>();

        /// <summary>
        /// The main function for convertSystem
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="template"></param>
        public void convert(string input, string output, string template)
        {
            Statement stm = getInfo(input);
            copyDoc(template, output);
            stm.SearchAndReplace(output);
        }

        /// <summary>
        /// Retrive the content of a specified document 
        /// to generate a Statement object.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private Statement getInfo(string filepath)
        {
            using (WordprocessingDocument wordprocessingDocument =
     WordprocessingDocument.Open(filepath, true))
            {

                // Assign a reference to the existing document body.
                OpenXmlElement body = wordprocessingDocument.MainDocumentPart.Document.Body;

                string text = ReadWordDocument(body);

                matchInfo("Plaintiff", text);
                matchInfo("Defendant", text);
                matchInfo("email", text);

                string expr = "Legal Representative" + Environment.NewLine + Environment.NewLine + @"([\w\s]*).*" + Environment.NewLine;
                string value = Regex.Match(text, expr, RegexOptions.Multiline).Groups[1].Value;
                dict.Add("representitive", value);

                expr = "Contact name and telephone" + Environment.NewLine + Environment.NewLine + @"(.*),.*?(\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d)" + Environment.NewLine;
                value = Regex.Match(text, expr, RegexOptions.Multiline).Groups[1].Value;
                dict.Add("Contact", value);
                value = Regex.Match(text, expr, RegexOptions.Multiline).Groups[2].Value;
                dict.Add("phone", value);

                DateTime localDate = DateTime.Now;
                string time = localDate.ToString("dd-MMMM-yyyy");
                dict.Add("time", time);

                return (new Statement(dict));
            }

        }


        /// <summary>
        /// Make a copy of the template to dest
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        private void copyDoc(string source, string dest)
        {
            using (var mainDoc = WordprocessingDocument.Open(source, false))
            using (var resultDoc = WordprocessingDocument.Create(dest,
              WordprocessingDocumentType.Document))
            {
                // copy parts from source document to dest document
                foreach (var part in mainDoc.Parts)
                    resultDoc.AddPart(part.OpenXmlPart, part.RelationshipId);
            }
        }

        /// <summary> 
        ///  Read Plain Text in all XmlElements of word document 
        /// </summary> 
        /// <param name="element">XmlElement in document</param> 
        /// <returns>Plain Text in XmlElement</returns> 
        public string GetPlainText(OpenXmlElement body)
        {
            StringBuilder PlainTextInWord = new StringBuilder();
            foreach (OpenXmlElement section in body.Elements())
            {
                switch (section.LocalName)
                {
                    // Text 
                    case "t":
                        PlainTextInWord.Append(section.InnerText);
                        break;


                    case "cr":                          // Carriage return 
                    case "br":                          // Page break 
                        PlainTextInWord.Append(Environment.NewLine);
                        break;


                    // Tab 
                    case "tab":
                        PlainTextInWord.Append("\t");
                        break;


                    // Paragraph 
                    case "p":
                        PlainTextInWord.Append(GetPlainText(section));
                        PlainTextInWord.AppendLine(Environment.NewLine);
                        break;


                    default:
                        PlainTextInWord.Append(GetPlainText(section));
                        break;
                }
            }


            return PlainTextInWord.ToString();
        }

        /// <summary> 
        ///  Read Word Document 
        /// </summary> 
        /// <returns>Plain Text in document </returns> 
        public string ReadWordDocument(OpenXmlElement body)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GetPlainText(body));
            return sb.ToString();
        }


        private void matchInfo(string element, string text)
        {
            string expr = element + Environment.NewLine + Environment.NewLine + "(.*)" + Environment.NewLine;
            string value = Regex.Match(text, expr,RegexOptions.Multiline).Groups[1].Value;
            dict.Add(element, value);
        }
    }
}
