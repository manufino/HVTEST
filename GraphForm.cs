using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace HV_Test
{
    public partial class PlotForm : Form
    {
        private PlotModel myModel;

        public PlotForm(PlotModel model)
        {
            InitializeComponent();

            myModel = model;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSaveGraph_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                PngExporter.Export(myModel, saveFileDialog.FileName, 800, 600, OxyColors.White);
            }
        }

        private void PlotForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Model = myModel;
        }
    }
}
