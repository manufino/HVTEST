using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace HV_Test
{
    public partial class FormMain : Form
    {
        private struct TimeWindows
        {
            public double StartTime;
            public double EndTime;
            public int WindowLengh;
        }

        private struct DataGeopsy
        {
            public double Frequency;
            public double Average;
            public double Min;
            public double Max;
            public double Five;
        }

        private int numberOfWindows;

        private TimeWindows[] timeWin;
        private DataGeopsy[] data_g;
        private double[] nc;
        private double f0, amplitude, f1, sf, f0a, f0b;

        private string hvFileName;
        private string logFileName;

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog(this);
        }

        private void btnReloadFiles_Click(object sender, EventArgs e)
        {
            loadFiles();
        }

        private void btnViewGraph_Click(object sender, EventArgs e)
        {
            PlotModel myModel = new PlotModel();
            myModel.Series.Add(new FunctionSeries());

            LineSeries linePoints = new LineSeries();
            LineSeries lineMin = new LineSeries();
            LineSeries lineMax = new LineSeries();
            LineSeries lineRed = new LineSeries();
            LineSeries lineB1 = new LineSeries();
            LineSeries lineB2 = new LineSeries();
            LineSeries lineV1 = new LineSeries();

            DataPoint XYpoint = new DataPoint();

            linePoints.Color = lineMin.Color = lineMax.Color = OxyColor.FromArgb(255,0,0,0);
            lineMin.LineStyle = LineStyle.Dot;
            lineMax.LineStyle = LineStyle.Dot;
            lineMin.StrokeThickness = 1;
            lineMax.StrokeThickness = 1;
            lineRed.Color = OxyColor.FromArgb(255, 255, 0, 0);
            lineRed.StrokeThickness = 1;
            lineB1.StrokeThickness = 1;
            lineB2.StrokeThickness = 1;
            lineB1.Color = lineB2.Color = OxyColor.FromArgb(255, 0, 0, 255);
            lineB1.LineStyle = LineStyle.Dot;
            lineB2.LineStyle = LineStyle.Dot;
            lineV1.Color = OxyColor.FromArgb(255, 0, 255, 0);
            lineV1.LineStyle = LineStyle.Dot;
            lineV1.StrokeThickness = 1;

            var listMax = new List<double>();
      
            foreach (var g in data_g)
            {
                listMax.Add(g.Max);

                XYpoint = new DataPoint(g.Frequency, g.Average);
                linePoints.Points.Add(XYpoint);

                XYpoint = new DataPoint(g.Frequency, g.Min);
                lineMin.Points.Add(XYpoint);

                XYpoint = new DataPoint(g.Frequency, g.Max);
                lineMax.Points.Add(XYpoint);
            }

            double vmax = listMax.Max()+1;
            lineRed.Points.Add(new DataPoint(f0, vmax));
            lineRed.Points.Add(new DataPoint(f0, 0));

            lineB1.Points.Add(new DataPoint(f0a, vmax));
            lineB1.Points.Add(new DataPoint(f0a, 0));

            lineB2.Points.Add(new DataPoint(f0b, vmax));
            lineB2.Points.Add(new DataPoint(f0b, 0));

            lineV1.Points.Add(new DataPoint(f0/4, vmax));
            lineV1.Points.Add(new DataPoint(f0/4, 0));


            myModel.Series.Add(linePoints);
            myModel.Series.Add(lineMin);
            myModel.Series.Add(lineMax);
            myModel.Series.Add(lineRed);
            myModel.Series.Add(lineB1);
            myModel.Series.Add(lineB2);
            myModel.Series.Add(lineV1);

            LinearAxis a = new LinearAxis();
            LogarithmicAxis b = new LogarithmicAxis();

            a.Title = "Amplitude HV";
            a.Position = AxisPosition.Left;
            a.FontSize = 8;
            a.TitleFontSize = 9;
            
            b.Title = "Frequency [Hz]";
            b.Position = AxisPosition.Bottom;
            b.FontSize = 8;
            b.TitleFontSize = 9;


            myModel.Axes.Add(a);
            myModel.Axes.Add(b);

            PlotForm graph = new PlotForm(myModel);
                        
            graph.ShowDialog(this);
        }

        private void resetLabels()
        {
            string rstStr = "--- >>";
            Color rstColor = Color.Gray;
            Color backColor = Color.White;

            lblC1.Text = rstStr;
            lblC1.ForeColor = rstColor;
            lblC1.BackColor = backColor;

            lblC2.Text = rstStr;
            lblC2.ForeColor = rstColor;
            lblC2.BackColor = backColor;

            lblC3.Text = rstStr;
            lblC3.ForeColor = rstColor;
            lblC3.BackColor = backColor;

            lblC4.Text = rstStr;
            lblC4.ForeColor = rstColor;
            lblC4.BackColor = backColor;

            lblC5.Text = rstStr;
            lblC5.ForeColor = rstColor;
            lblC5.BackColor = backColor;

            lblC6.Text = rstStr;
            lblC6.ForeColor = rstColor;
            lblC6.BackColor = backColor;

            lblC7.Text = rstStr;
            lblC7.ForeColor = rstColor;
            lblC7.BackColor = backColor;

            lblC8.Text = rstStr;
            lblC8.ForeColor = rstColor;
            lblC8.BackColor = backColor;

            lblC9.Text = rstStr;
            lblC9.ForeColor = rstColor;
            lblC9.BackColor = backColor;
        }

        private void checkFileExists()
        {
            bool logFileExist = File.Exists("HV_test.log");
            bool dataFileExist = File.Exists("HV_test.hv");

            if (!dataFileExist || !logFileExist)
            {
                MessageBox.Show("HV_test.hv or HV_test.log not found ! \nPlease place this files into software directory and relaunch program.",
                    "Error File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void loadFiles()
        {
            checkFileExists();

            string[] log = File.ReadAllLines("HV_test.log");
            string[] data = File.ReadAllLines("HV_test.hv");

            numberOfWindows = Convert.ToInt32(data[1].Split(' ')[5]);
            timeWin = new TimeWindows[numberOfWindows];
            nc = new double[numberOfWindows];
            data_g = new DataGeopsy[data.Length-7];

            f0 = double.Parse(data[2].Split(Convert.ToChar('\t'))[1], CultureInfo.InvariantCulture);
            amplitude = double.Parse(data[5].Split(Convert.ToChar('\t'))[1], CultureInfo.InvariantCulture);
            f1 = double.Parse(data[4].Split(Convert.ToChar('\t'))[2], CultureInfo.InvariantCulture);
            sf = f0 - f1;

            int index_log = 0;

            foreach (string line in log)
            {
                if (line.Contains("# Start time"))
                {
                   break;
                }
                index_log++;
            }

            for (int i = (index_log + 1), j = 0; j < numberOfWindows-1; i++, j++)
            {
                timeWin[j].StartTime = double.Parse(log[i].Split(Convert.ToChar('\t'))[0], CultureInfo.InvariantCulture);
                timeWin[j].EndTime = double.Parse(log[i].Split(Convert.ToChar('\t'))[1], CultureInfo.InvariantCulture);
                timeWin[j].WindowLengh = Convert.ToInt32(log[i].Split(Convert.ToChar('\t'))[2]);
                nc[j] = timeWin[j].WindowLengh * numberOfWindows * f0;
            }

            for (int q = 7, n=0; q < data.Length; q++,n++)
            {
                data_g[n].Frequency = double.Parse(data[q].Split(Convert.ToChar('\t'))[0], CultureInfo.InvariantCulture);
                data_g[n].Average = double.Parse(data[q].Split(Convert.ToChar('\t'))[1], CultureInfo.InvariantCulture);
                data_g[n].Min = double.Parse(data[q].Split(Convert.ToChar('\t'))[2], CultureInfo.InvariantCulture);
                data_g[n].Max = double.Parse(data[q].Split(Convert.ToChar('\t'))[3], CultureInfo.InvariantCulture);
                data_g[n].Five = data_g[n].Max / data_g[n].Average;
            }
            checkCriteria();
        }

        private void checkCriteria()
        {
            bool c1 = false;
            bool c2 = false;
            bool c3 = true;
            bool c4 = false;
            bool c5 = false;
            bool c6 = false;
            bool c7 = false;
            bool c8 = false;
            bool c9 = false;

            foreach (var w in timeWin)
            {
                if (w.WindowLengh == 0) break;
                if (f0 > Convert.ToDouble(10 / w.WindowLengh))
                {
                    c1 = true;
                }
                else
                {
                    c1 = false;
                    break;
                }
            }

            foreach (var n in nc)
            {
                if (n == 0) break;
                if (n > 200)
                {
                    c2 = true;
                }
                else
                {
                    c2 = false;
                    break;
                }
            }

            if (f0 >= 0.5)
            {
                foreach (var da in data_g)
                    if (da.Frequency > 0.5 * f0 && da.Frequency < 2 * f0 && da.Five > 2)
                        c3 = false;
            }
            else
            {
                foreach (var dd in data_g)
                    if (dd.Frequency > 0.5 * f0 && dd.Frequency < 2 * f0 && dd.Five > 3)
                        c3 = false;
            }

            foreach (var d in data_g)
            {
                if (d.Frequency > f0 / 4 && d.Frequency < f0 && d.Average < amplitude / 2)
                    c4 = true;

                if (d.Frequency > f0 && d.Frequency < 4 * f0 && d.Average < amplitude / 2)
                    c5 = true;
            }

            c6 = amplitude > 2;

            f0a = f0 + 0.05 * f0;
            f0b = f0 - 0.05 * f0;

            DataGeopsy data_max_c1 = new DataGeopsy();
            DataGeopsy data_max_c2 = new DataGeopsy();
            DataGeopsy data_max_c3 = new DataGeopsy();
            DataGeopsy data_max_c4 = new DataGeopsy();
            DataGeopsy data_max_c5 = new DataGeopsy();

            for (int i = 0; i < data_g.Length; i++)
            {
                if (i == 0)
                {
                    data_max_c1 = data_g[i];
                    data_max_c2 = data_g[i];
                    data_max_c3 = data_g[i];
                    data_max_c4 = data_g[i];
                    data_max_c5 = data_g[i];
                }
                else
                {
                    if (data_max_c1.Frequency < data_g[i].Frequency)
                        data_max_c1 = data_g[i];

                    if (data_max_c2.Average < data_g[i].Average)
                        data_max_c2 = data_g[i];

                    if (data_max_c3.Min < data_g[i].Min)
                        data_max_c3 = data_g[i];

                    if (data_max_c4.Max < data_g[i].Max)
                        data_max_c4 = data_g[i];

                    if (data_max_c5.Five < data_g[i].Five)
                        data_max_c5 = data_g[i];
                }
            }

            if (data_max_c3.Frequency > f0b)
                if (data_max_c3.Frequency < f0a && data_max_c4.Frequency > f0b && data_max_c4.Frequency < f0a)
                    c7 = true;

            double ef0;

            if (f0 < 0.2)
                ef0 = 0.25 * f0;
            else if (f0 < 0.5)
                ef0 = 0.20 * f0;
            else if (f0 < 1.0)
                ef0 = 0.15 * f0;
            else if (f0 < 2.0)
                ef0 = 0.10 * f0;
            else
                ef0 = 0.05 * f0;
            if (sf < ef0)
                c8 = true;

            double tetaf0;

            if (f0 < 0.2)
                tetaf0 = 3.0;
            else if (f0 < 0.5)
                tetaf0 = 2.5;
            else if (f0 < 1.0)
                tetaf0 = 2.0;
            else if (f0 < 2.0)
                tetaf0 = 1.78;
            else
                tetaf0 = 1.58;
            if (data_max_c2.Five < tetaf0)
                c9 = true;

            lblC1.IsPass = c1;
            lblC2.IsPass = c2;
            lblC3.IsPass = c3;
            lblC4.IsPass = c4;
            lblC5.IsPass = c5;
            lblC6.IsPass = c6;
            lblC7.IsPass = c7;
            lblC8.IsPass = c8;
            lblC9.IsPass = c9;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            resetLabels();
            loadFiles();
        }
    }
}
