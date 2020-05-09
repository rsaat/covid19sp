using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

namespace ExtractCovid19Sp
{
    public class PdfTextExtraction
    {
        private string _pdfFile;
        public PdfTextExtraction(string pdfFile)
        {
            _pdfFile = pdfFile;
        }

        public string ExtractText()
        {

            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(_pdfFile);
            
            var loadedPages = loadedDocument.Pages;
            var extractedText = string.Empty;

            foreach (PdfLoadedPage loadedPage in loadedPages)
            {
                extractedText += loadedPage.ExtractText(IsLayout: true);
            }

            loadedDocument.Close(true);

            return extractedText;
        }


    }
}
