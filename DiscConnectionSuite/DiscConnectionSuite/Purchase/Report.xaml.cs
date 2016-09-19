using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiscConnectionSuite.Purchase
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        private string bpd, userId;

        public Report(string newBpd, string newUserId, string totalPrice)
        {
            InitializeComponent();
            bpd = newBpd;
            userId = newUserId;
            lbTotalPrice.Content = totalPrice;
            _reportViewer.Load += ReportViewer_Load;
        }

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            DataTable dt = BusLayer.blPurchase.GetReport(bpd);
            ReportDataSource rds = new ReportDataSource("DCReport", dt);

            _reportViewer.ProcessingMode = ProcessingMode.Local;
            LocalReport rep = _reportViewer.LocalReport;
                
            System.Drawing.Printing.Margins m = new System.Drawing.Printing.Margins();
            m.Left = 0;
            m.Right = 0;
            m.Top = 0;
            m.Bottom = 0;

            System.Drawing.Printing.PageSettings p = new System.Drawing.Printing.PageSettings();
            p.Margins = m;
            _reportViewer.SetPageSettings(p);
            rep.ReportPath = "BPDReport.rdlc";
            
            rep.DataSources.Clear();

            ReportParameter p1 = new ReportParameter("BPDID", bpd);
            _reportViewer.LocalReport.SetParameters(p1);

            rep.DataSources.Add(rds);
            _reportViewer.RefreshReport();
        }

        private void btDone_Click(object sender, RoutedEventArgs e)
        {
            if(chDone.IsChecked == false) {
                ErrMsg.Content = "Please confirm that you have printed the report by checking the \"Confirm\" box.";
                return;
            }

            try {
                MainMenu mainMenu = new MainMenu(userId);
                mainMenu.Show();
                this.Close();
            }
            catch (Exception)
            {

            }
        }
    }
}
