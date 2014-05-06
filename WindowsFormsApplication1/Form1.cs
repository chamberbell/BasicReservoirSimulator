using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        private double[,] Qw_vs_Time;


        public Form1(double[,] Qw, double del_t)
        {
            InitializeComponent();
            chart1.Series.Clear();

            this.Qw_vs_Time = Qw;
            string seriesName;
            int time_steps = Qw_vs_Time.GetLength(0);
            int wells = Qw_vs_Time.GetLength(1);

            //Initialize settings of the graph
            chart1.ChartAreas[0].AxisX.Title = "time, days";
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            //chart1.ChartAreas[0].AxisX.Maximum = length;
            chart1.ChartAreas[0].AxisY.Title = "Rate, stb/day";
            
            

            for (int i = 0; i < wells ; i++)
            {
                seriesName = "Well" + (i+1); ;
                chart1.Series.Add(seriesName);
                chart1.Series[seriesName].ChartType = SeriesChartType.Line;
                chart1.Series[seriesName].BorderWidth = 2;

                for (int n = 0; n < time_steps; n++)
                {
                    chart1.Series[seriesName].Points.AddXY((n) * del_t, -Qw_vs_Time[n, i]);
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {
        }
    }
}
