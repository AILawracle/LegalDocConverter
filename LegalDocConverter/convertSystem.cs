using System;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;

namespace LegalDocConverter
{
    class convertSystem
    {
        /// <summary>
        /// 
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
                string text = body.InnerText;


                string expr = @"Plaintiff(.*)Defendant";
                string plaintiff = Regex.Match(text, expr).Groups[1].Value;

                expr = @"Defendant(.*)FILING DETAILS";
                string defendant = Regex.Match(text, expr).Groups[1].Value;


                expr = @"Legal Representative([\w\s]*).*Legal representative reference";
                string representivie = Regex.Match(text, expr).Groups[1].Value;

                expr = @"Contact name and telephone(.*),.*?(\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d)Contact email";
                string contact = Regex.Match(text, expr).Groups[1].Value;
                string phone = Regex.Match(text, expr).Groups[2].Value;

                expr = @"Contact email(.*)TYPE OF CLAIM";
                string email = Regex.Match(text, expr).Groups[1].Value;

                return (new Statement(plaintiff, defendant, representivie, contact, phone, email));
            }

        }


        /// <summary>
        /// make a copy of the template to dest
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
