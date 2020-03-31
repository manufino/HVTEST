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
                ;

            }
        }

        private void PlotForm_Load(object sender, EventArgs e)
        {
           // myModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
            pictureBox1.Model = myModel;
        }
    }
}
