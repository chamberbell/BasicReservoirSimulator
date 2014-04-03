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
    public partial class MainForm : Form
    {

        double time_frame; // [days] this is how long the simulation will run
        double delta_t; // [days] the number of days between time steps
        int time_steps; // time_frame divided by delta_t

        double delta_x; //[ft]
        double delta_y; //[ft]
        double delta_z; //[ft]

        int grid_x;
        int grid_y;
        int grid_z;

        double length; //[ft]
        double width; //[ft]
        double height; //[ft]

        double porosity;
        double perm;
        double rockComp;
        double totalComp;
        double liquidComp;

        double Sw;
        double Pb;
        double Boi;
        double oilVisc;
        double Pinitial;
        double Pmin;
        double ConvToInjPres;

        bool? UseWell1 = null;
        bool? Well1_Inj = null;
        bool? Well1_Qw_or_Pwf = null; //1=Qw, 0=Pwf
        double Well1Pwf; 
        double Well1Qw;
        double Well1Skin;
        double Well1Rw; //[ft]
        double Well1XLoc; //[ft]
        double Well1YLoc; //[ft]

        bool? UseWell2 = null;
        bool? Well2_Inj = null;
        bool? Well2_Qw_or_Pwf = null; //1=Qw, 0=Pwf
        double Well2Pwf;
        double Well2Qw;
        double Well2Skin;
        double Well2Rw; //[ft]
        double Well2XLoc; //[ft]
        double Well2YLoc; //[ft]

        bool? UseWell3 = null;
        bool? Well3_Inj = null;
        bool? Well3_Qw_or_Pwf = null; //1=Qw, 0=Pwf
        double Well3Pwf;
        double Well3Qw;
        double Well3Skin;
        double Well3Rw; //[ft]
        double Well3XLoc; //[ft]
        double Well3YLoc; //[ft]




        public MainForm()
        {
            InitializeComponent();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        //this is what heppens what you clidk "Calculate"
        private void button1_Click(object sender, EventArgs e)
        {
            refreshData();
            MessageBox.Show("The data is refrshed! But no calculations are programmed yet", "My Application",
MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
        }
   
        private void refreshData()
        {
            //double.TryParse(tbBubblePoint.Text, out foo);
            //foo = foo + 200;
            //tbBubblePoint.Text = Convert.ToString(foo);

            chart1.Series.Clear();


            double.TryParse(tbTimeFrame.Text, out time_frame); // [days] this is how long the simulation will run
            double.TryParse(tbTimeStep.Text, out  delta_t); // [days] the number of days between time steps
            time_steps = Convert.ToInt32(time_frame / delta_t); // time_frame divided by delta_t


            double.TryParse(txLength.Text, out  length); //[ft]
            double.TryParse(txWidth.Text, out  width); //[ft]
            double.TryParse(txHeight.Text, out  height); //[ft]

            int.TryParse(txXGridBlocks.Text, out  grid_x); //[ft]
            int.TryParse(txYGridBlocks.Text, out  grid_y); //[ft]
            int.TryParse(txZGridBlocks.Text, out  grid_z); //[ft]

            delta_x = length / grid_x; //[ft]
            delta_y = width / grid_y; //[ft]
            delta_z = height / grid_z; //[ft]

            double.TryParse(tbPorosity.Text, out  porosity);
            double.TryParse(tbPerm.Text, out  perm);
            double.TryParse(tbRockComp.Text, out  rockComp);
            double.TryParse(tbTotalComp.Text, out  totalComp);
            double.TryParse(tbLiquComp.Text, out  liquidComp);

            double.TryParse(tbWaterSat.Text, out  Sw);
            double.TryParse(tbBubblePoint.Text, out  Pb);
            double.TryParse(tbInitialBo.Text, out  Boi);
            double.TryParse(tbOilVisc.Text, out  oilVisc);
            double.TryParse(txInitialP.Text, out  Pinitial);
            double.TryParse(txPresToConvert.Text, out  Pmin);
            double.TryParse(txPresToConvert.Text, out  ConvToInjPres);

            UseWell1 = cbWell1Active.Checked;
            Well1_Inj = cbWell1Injector.Checked;
            Well1_Qw_or_Pwf = rbWell1Qw.Checked; //1=Qw, 0=Pwf
            double.TryParse(tbWell1Pwf.Text, out  Well1Pwf);
            double.TryParse(tbWell1Qw.Text, out  Well1Qw);
            double.TryParse(tbWell1Skin.Text, out  Well1Skin);
            double.TryParse(tbWell1rw.Text, out  Well1Rw); //[ft]
            double.TryParse(tbWell1X.Text, out  Well1XLoc); //[ft]
            double.TryParse(tbWell1Y.Text, out  Well1YLoc); //[ft]

            UseWell2 = cbWell2Active.Checked;
            Well2_Inj = cbWell2Injector.Checked;
            Well2_Qw_or_Pwf = rbWell2Qw.Checked; //1=Qw, 0=Pwf
            double.TryParse(tbWell2Pwf.Text, out  Well2Pwf);
            double.TryParse(tbWell2Qw.Text, out  Well2Qw);
            double.TryParse(tbWell2Skin.Text, out  Well2Skin);
            double.TryParse(tbWell2rw.Text, out  Well2Rw); //[ft]
            double.TryParse(tbWell2X.Text, out  Well2XLoc); //[ft]
            double.TryParse(tbWell2Y.Text, out  Well2YLoc); //[ft]

            UseWell2 = cbWell2Active.Checked;
            Well2_Inj = cbWell2Injector.Checked;
            Well2_Qw_or_Pwf = rbWell2Qw.Checked; //1=Qw, 0=Pwf
            double.TryParse(tbWell3Pwf.Text, out  Well3Pwf);
            double.TryParse(tbWell3Qw.Text, out  Well3Qw);
            double.TryParse(tbWell3Skin.Text, out  Well3Skin);
            double.TryParse(tbWell3rw.Text, out  Well3Rw); //[ft]
            double.TryParse(tbWell3X.Text, out  Well3XLoc); //[ft]
            double.TryParse(tbWell3Y.Text, out  Well3YLoc); //[ft]

            //calculate constants
            double alpha = -158 * porosity * oilVisc * totalComp / perm * Math.Pow(delta_x, 2) / delta_t;
            double beta = 2+alpha; 

            //set up the arrays
            double[] x_array = new double[grid_x + 1];
            double[] Pn = new double[grid_x + 1];
            double[] a = new double[grid_x + 1];
            double[] b = new double[grid_x + 1];
            double[] c = new double[grid_x + 1];
            double[] d = new double[grid_x + 1];

            x_array[0] = delta_x / 2;
            Pn[0] = Pinitial;

            int i; //time step
            int n; //grid block

            //define the pressure matrix to store the P values over time and space
            double[,] P = new double [time_steps,grid_x+1]; // P[n,i]

            //set up the dirac delta well term and initialize arrays
            double[] dirac = new double[grid_x + 1];
            for (int x = 0;  x < (grid_x+1);  x++)
            {
                dirac[x] = 0;
                P[0, x] = Pinitial;
                x_array[x] = x * delta_x + delta_x / 2;
                a[x] = 1;
                b[x] = beta;
                c[x] = 1;
                d[x] = -alpha * Pinitial;
            }

            //Manage the end points and boundary conditions
            a[0] = 0;
            b[0] = 1 + beta; //no flow x=0 boundary
            b[(grid_x )] = 1 + beta; //no flow x=L boundary
            c[(grid_x )] = 0;

            //mark where the wells are in the dirac delta array
            if (UseWell1 == true)
            {
                n = Convert.ToInt32((Well1XLoc / delta_x)+1);
                dirac[n] = 1;
            }

            if (UseWell2 == true)
            {
                n = Convert.ToInt32((Well2XLoc / delta_x) + 1);
                dirac[n] = 1;
            }

            if (UseWell3 == true)
            {
                n = Convert.ToInt32((Well3XLoc / delta_x) + 1);
                dirac[n] = 1;
            } 

            //compute the well term
                //computer new Jw based on pressure at the block [B@Pi]
            double Well1Jw = Jw(Pinitial, Well1Rw, Well1Skin);

            //MAIN PRESSURE CALCULATIONS
            for (n = 0; n < time_steps+1; n++)
            {
                
            }

            //initialize the first series (t=0) on the graph
            string seriesName = "Time Step #0"; ;
            chart1.Series.Add(seriesName);
            chart1.Series[seriesName].ChartType = SeriesChartType.Line;

            //Initializes the basics of the pressure vs x vs time plot (@time = 0)
            for (int pi=0; pi < grid_x + 1; pi++ )
            {
                chart1.Series[0].Points.AddXY(x_array[pi], P[0, pi]);
            }


            //This block of code takes the P[,] matrix and plots the pressures vs x, for each time step.
            n = 1; //each n is a time_step. n+1 is the next time step.
            while(n<time_steps-1)
            {
                seriesName = "Time Step #" + n; ;
                chart1.Series.Add(seriesName);
                chart1.Series[seriesName].ChartType = SeriesChartType.Line;

                i = 0;
                while (i < grid_x + 1)
                {
                    P[n,i] = P[n - 1, i] - delta_t*1; //just a dummy equation to test the graph
                    //P[n,i];  //this is what to switch it to when the code is working

                    chart1.Series[seriesName].Points.AddXY(x_array[i], P[n, i]);
                    i++;
                }
                n++;
            }

            

        }

        //calculate the well term for the matrix
        private double wellTermQ(double Qw, double Pn, int dirac)
        {
            double well = 887.53 * dirac * Qw * oilVisc * Bo_n(Pn) * delta_x / (perm * delta_y * delta_z);
            return well;
        }

        //calculate the pressure dependent FVF
        private double Bo_n(double Pn)
        {
            double Bn = Boi*Math.Exp(-liquidComp*(Pn-Pinitial));
            return Bn;
        }

        //Productivity Index from Q
        private double Jw_Q(double Qw, double Pn)
        {
            double Jw_n = 1; //just a placeholder
            return Jw_n;
        }

        //Productivity index from Peaceman's Method
        private double Jw(double Pn, double rw, double S)
        {
            double re = 0.14 * Math.Sqrt(Math.Pow(delta_x, 2) + Math.Pow(delta_y, 2));
            double Bn = Boi*Math.Exp(-liquidComp*(Pn-Pinitial)); //need to make this based on pressure from c_liquid
            double Jw_n;
            Jw_n= 0.00708 / oilVisc / Bn * perm * height / (Math.Log(rw/re) +S);
            return Jw_n;
        }

        private void ThomasMethod(double[] a, double[] b, double[] c, double[] d, int n)
        {
            n--; 
            c[0] /= b[0];
            d[0] /= b[0];

            for (int i = 1; i < n; i++)
            {
                c[i] /= b[i] - a[i] * c[i - 1];
                d[i] = (d[i] - a[i] * d[i - 1]) / (b[i] - a[i] * c[i - 1]);
            }

            d[n] = (d[n] - a[n] * d[n - 1]) / (b[n] - a[n] * c[n - 1]);

            for (int i = n; i-- > 0; )
            {
                d[i] -= c[i] * d[i + 1];
            }
        }

        private void cbWell1Active_CheckedChanged(object sender, EventArgs e)
        {
                
                
        }

        private void rbWell1Qw_CheckedChanged(object sender, EventArgs e)
        {
            

        }

        private void rbWell1Pwf_CheckedChanged(object sender, EventArgs e)
        {


        }

        private void button2_Click(object sender, EventArgs e)
        {
            //chart1.

            
            Form1 f1 = new Form1(); // Instantiate a Form1 object.
            f1.Show();    
        }

    }
}
