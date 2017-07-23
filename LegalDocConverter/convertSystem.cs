using System;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace LegalDocConverter
{
    class convertSystem
    {
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
                Body body = wordprocessingDocument.MainDocumentPart.Document.Body;

                Dictionary<string, string> dict = new Dictionary<string, string>();

                string text = body.InnerText;

                string expr = @"Plaintiff(.*)Defendant";
                string plaintiff = Regex.Match(text, expr).Groups[1].Value;
                dict.Add("plaintiff", plaintiff);

                expr = @"Defendant(.*)FILING DETAILS";
                string defendant = Regex.Match(text, expr).Groups[1].Value;
                dict.Add("defendant", defendant);


                expr = @"Legal Representative([\w\s]*).*Legal representative reference";
                string representivie = Regex.Match(text, expr).Groups[1].Value;
                dict.Add("representitive", representivie);

                expr = @"Contact name and telephone(.*),.*?(\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d)Contact email";
                string contact = Regex.Match(text, expr).Groups[1].Value;
                string phone = Regex.Match(text, expr).Groups[2].Value;
                dict.Add("contact", contact);
                dict.Add("phone", phone);

                expr = @"Contact email(.*)TYPE OF CLAIM";
                string email = Regex.Match(text, expr).Groups[1].Value;
                dict.Add("email", email);

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

    }
}
