﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace ExtractCovid19Sp
{
    public class DataCovidExport
    {
        private List<DataCovidSp> listDataCovidSp;
        private FileHelperEngine<DataCovidSp> engine;
        private readonly string filename;

        public DataCovidExport(string filename)
        {
            this.filename = filename;
            this.engine = new FileHelperEngine<DataCovidSp>();
            this.engine.HeaderText = engine.GetFileHeader();

            LoadCsvFile(this.filename);


        }

        public void ExportPdfToCsv()
        {

            var dataOath = Path.GetDirectoryName(this.filename);

            var files = Directory.GetFiles(dataOath, "*.pdf");


            foreach (var file in files)
            {

                if (listDataCovidSp.Any(d => file.Contains($"covidsp{d.Date.ToString("yyyy-MM-dd")}")))
                {
                    continue;
                }

                var pdfExtract = new PdfTextExtraction(file);

                var textCovidSP = pdfExtract.ExtractText();

                var dataCovidSp = new DataCovidSp(textCovidSP);

                Append(this.filename, dataCovidSp);
            }

        }

        private void Append(string filename, DataCovidSp datacovid)
        {
            if (listDataCovidSp.Any(d => d.Date == datacovid.Date))
            {
                return;
            }

            listDataCovidSp.Add(datacovid);

            var valuesByDate = listDataCovidSp.OrderBy(d => d.Date).ToArray();

            for (int i = 13; i < valuesByDate.Length; i++)
            {
                var recovered = valuesByDate[i - 13].ConfirmedCases - valuesByDate[i].DeathsSivep;
                recovered = (recovered < valuesByDate[i - 1].Recovered) ? valuesByDate[i - 1].Recovered : recovered;
                valuesByDate[i].Recovered = (recovered >= 0) ? recovered : 0;
            }

            engine.WriteFile(filename, valuesByDate);

        }

        private void LoadCsvFile(string filename)
        {

            this.listDataCovidSp = new List<DataCovidSp>();

            if (File.Exists(filename))
            {
                var records = engine.ReadFile(filename);

                foreach (var r in records)
                {
                    if (string.IsNullOrEmpty(r.Location))
                    {
                        r.Location = "sao paulo";
                    }
                }

                listDataCovidSp.AddRange(records);
            }


        }
    }
}