using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExtractCovid19Sp
{
    /// <summary>
    /// Download dos arquivos de Covid-19 da prefeitura de SP
    /// 
    /// https://www.prefeitura.sp.gov.br/cidade/secretarias/upload/saude/27042020_Boletim_Diario.pdf
    /// https://www.prefeitura.sp.gov.br/cidade/secretarias/upload/saude/28042020_BoletimDiario.pdf
    /// https://www.prefeitura.sp.gov.br/cidade/secretarias/upload/saude/29042020_BoletimDiario1.pdf
    /// https://www.prefeitura.sp.gov.br/cidade/secretarias/upload/saude/30042020boletim_covid-19_diario.pdf
    /// https://www.prefeitura.sp.gov.br/cidade/secretarias/upload/saude/01052020boletim_covid-19_diario%20.pdf
    /// https://www.prefeitura.sp.gov.br/cidade/secretarias/upload/saude/03052020boletim_covid-19_diario.pdf
    /// https://www.prefeitura.sp.gov.br/cidade/secretarias/upload/saude/05052020boletim_covid-19_diariov2.pdf
    /// https://www.prefeitura.sp.gov.br/cidade/secretarias/upload/saude/06052020boletim_covid-19_diariov2.pdf
    /// https://www.prefeitura.sp.gov.br/cidade/secretarias/upload/saude/07052020boletim_covid-19_diariov2.pdf
    /// https://www.prefeitura.sp.gov.br/cidade/secretarias/upload/saude/09052020boletim_covid19_diariov2.pdf
    /// https://www.prefeitura.sp.gov.br/cidade/secretarias/upload/saude/10052020boletim_covid19_diario.pdf
    /// </summary>
    public class DownloadCovidSpFiles
    {
        private readonly string dataPath;

        public DownloadCovidSpFiles(string dataPath)
        {
            this.dataPath = dataPath;
        }

        public void DownloadFiles()
        {

            var startDate = new DateTime(2020, 04, 30);

            var currentDate = startDate;

            while (currentDate <= DateTime.Now.Date)
            {
                var localfile = Path.Combine(this.dataPath, $"covidsp{currentDate.ToString("yyyy-MM-dd")}.pdf");

                if (!File.Exists(localfile))
                {
                    using (var client = new WebClient())
                    {
                        var filesToTry = new string[]{ "boletim_covid19_diariov2.pdf"
                                                      , "boletim_covid19_diario.pdf"
                                                      , "boletim_covid-19_diario.pdf"
                                                      , "boletim_covid-19_diariov2.pdf"
                                                      };
                        for (int i = 0; i < filesToTry.Length; i++)
                        {
                            var isLastFile = (i == (filesToTry.Length - 1));
                            var downloadOk = TryToDownload(currentDate, localfile, client, filesToTry[i], isLastFile);
                            if (downloadOk)
                            {
                                break;
                            }
                        }

                    }
                }


                currentDate = currentDate.AddDays(1);
            }





        }

        private static bool TryToDownload(DateTime currentDate, string localfile, WebClient client, string fileTermination, bool throwException)
        {
            try
            {
                var downloadUrl = $"https://www.prefeitura.sp.gov.br/cidade/secretarias/upload/saude/{currentDate.ToString("ddMMyyyy")}{fileTermination}";
                client.DownloadFile(downloadUrl, localfile);
                return true;
            }
            catch (Exception)
            {
                if (File.Exists(localfile))
                {
                    File.Delete(localfile);
                }

                if (throwException)
                {
                    throw;
                }

                return false;
            }

        }
    }
}
