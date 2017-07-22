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
        private string plantiff;
        private string defendant;
        private string representative;
        private string contact;
        private string phone;
        private string email;

        public Statement(string plantiff, string defendant, string representative, string contact, string phone, string email)
        {
            this.plantiff = plantiff;
            this.defendant = defendant;
            this.representative = representative;
            this.contact = contact;
            this.phone = phone;
            this.email = email;
        }

        /// <summary>
        /// Replace all the parts that match with the fileds of the Statement.
        /// </summary>
        /// <param name="output"></param>
        public void SearchAndReplace(string output)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(output, true))
            {
                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                Regex regexText = new Regex(@"&lt;plaintiff&gt;");
                docText = regexText.Replace(docText, this.plantiff);

                regexText = new Regex(@"&lt;defendant&gt;");
                docText = regexText.Replace(docText, this.defendant);

                regexText = new Regex(@"&lt;representitive&gt;");
                docText = regexText.Replace(docText, this.representative);

                regexText = new Regex(@"&lt;contact&gt;");
                docText = regexText.Replace(docText, this.contact);

                regexText = new Regex(@"&lt;email&gt;");
                docText = regexText.Replace(docText, this.email);

                regexText = new Regex(@"&lt;phone&gt;");
                docText = regexText.Replace(docText, this.phone);

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

        public override string ToString()
        {
            return "plantiff: " + plantiff + "\ndefendant: " + defendant + "\nrepresentative: " + representative + "\ncontact: "
                + contact + "\nphone: " + phone + "\nemail: " + email;
        }
    }
}
