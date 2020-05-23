using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ExtractCovid19Sp
{
    public partial class frmExportCovidToCsv : Form
    {
        public frmExportCovidToCsv()
        {
            InitializeComponent();
        }

        private void cmdExportCsv_Click(object sender, EventArgs e)
        {
            try
            {
                var covidDataSpFile = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\data\\covidsp.csv"));
                var dataPath = Path.GetDirectoryName(covidDataSpFile);
                var downloadFiles = new DownloadCovidSpFiles(dataPath);
                downloadFiles.DownloadFiles(dtpEndDate.Value);

                var dataCovidExport = new DataCovidExport(covidDataSpFile);
                dataCovidExport.ExportPdfToCsv();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }


        }

        private void frmExportCovidToCsv_Load(object sender, EventArgs e)
        {
            dtpEndDate.Value = DateTime.Now.Date;
        }
    }
}
