using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Globalization;
using System.Web;

namespace ExtractCovid19Sp
{
    /// <summary>
    /// Download dos arquivos de Covid-19 da prefeitura de SP
    /// </summary>
    public class DownloadCovidSpFiles
    {
        private readonly string dataPath;

        public DownloadCovidSpFiles(string dataPath)
        {
            this.dataPath = dataPath;
        }


        public void DownloadFiles(DateTime lastDate)
        {

            var startDate = new DateTime(2020, 04, 30);

            var currentDate = startDate;

            while (currentDate <= lastDate)
            {
                var localfile = Path.Combine(this.dataPath, $"covidsp{currentDate.ToString("yyyy-MM-dd")}.pdf");

                if (!File.Exists(localfile))
                {
                    using (var client = new WebClient())
                    {

                        var dailyPage = FindUrlOfDailyPage(currentDate);

                        var pdfFileUrl = FindPdfPage(dailyPage);

                        client.DownloadFile(pdfFileUrl, localfile);

                    }
                }


                currentDate = currentDate.AddDays(1);
            }

        }


        private string FindPdfPage(string urlOfDailyPage)
        {
            //var path = @"BoletinsDay.html";
            //var doc = new HtmlDocument();
            //doc.Load(path);

            if (urlOfDailyPage.ToLower().EndsWith(".pdf"))
            {
                return urlOfDailyPage;
            }

            HtmlWeb html = new HtmlWeb();
            var doc = html.Load(urlOfDailyPage);

            var nodes = doc.DocumentNode.SelectNodes("//a");

            var linkNodes = nodes.Where(n => IsFoundPdfLink(n.InnerText)).ToArray();

            var url = "";

            if (linkNodes.Length > 0)
            {
                url = linkNodes[0].Attributes["href"].Value;
            }

            return url;

        }

        private bool IsFoundPdfLink(string innerText)
        {
            innerText = innerText.ToLower();

            if (!innerText.Contains("acesse"))
            {
                return false;
            }

            if (!innerText.Contains("boletim"))
            {
                return false;
            }

            return true;
        }




        private string FindUrlOfDailyPage(DateTime fileDate)
        {
            //var path = @"Boletins.html";
            //var doc = new HtmlDocument();

            var urlMaiin = "https://www.prefeitura.sp.gov.br/cidade/secretarias/saude/vigilancia_em_saude/doencas_e_agravos/coronavirus/index.php?p=295572";
            HtmlWeb html = new HtmlWeb();
            var doc = html.Load(urlMaiin);

            var nodes = doc.DocumentNode.SelectNodes("//a");

            var nodesInnerText = nodes.Select(x => x.InnerText).ToArray();

            var linkNodes = nodes.Where(n => IsFoundDateLink(fileDate, n.InnerText)).ToArray();

            var url = "";

            if (linkNodes.Length > 0)
            {
                url = linkNodes[0].Attributes["href"].Value;
            }

            return url;

        }

        private bool IsFoundDateLink(DateTime documentDate, string innerText)
        {
            innerText = System.Net.WebUtility.HtmlDecode(innerText);

            innerText = innerText.ToLower();

            if (!innerText.Contains("boletim"))
            {
                return false;
            }

            if (documentDate.Day == 1)
            {
                if (!innerText.Contains("1 de"))
                {
                    if (!innerText.Contains("1º de"))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!innerText.Contains($"{documentDate.Day} de"))
                {
                    return false;
                }

            }


            if (!innerText.Contains(documentDate.Day.ToString()))
            {
                return false;
            }

            if (!innerText.Contains(documentDate.Year.ToString()))
            {
                return false;
            }

            string fullMonthName = documentDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("pt-BR")).ToLower();

            if (!innerText.Contains(fullMonthName))
            {
                return false;
            }

            return true;
        }




    }
}
