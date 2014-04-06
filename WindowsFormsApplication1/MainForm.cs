﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection; // For Missing.Value and BindingFlags
using System.Runtime.InteropServices; // For COMException
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;


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
        double So;
        double Pb;
        double Boi;
        double oilVisc;
        double Pinitial;
        double Pmin;
        double ConvToInjPres;

        double rate;
        bool?[] wells = new bool?[3]; //right now the code supports 3 wells
        bool?[] Inj = new bool?[3];
        bool?[] QwConst = new bool?[3];
        double[] QwRate = new double[3];

        double[] PwfPres = new double[3];
        double[] WellRw = new double[3];
        double[] Skin = new double[3];
        double[] X_loc = new double[3];
        double[] Y_loc = new double[3];

        //define the pressure & rate matrices to store the P values over time and space
        double[,] P;   // P_avg vs time vs x_loc
        double[,] Qw;  // rate vs time vs x loc
        double[,] Pwf; // Pwf vs time vs x_loc


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
        }
   
        private void refreshData()
        {
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisX.StripLines.Clear();



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
            porosity = porosity / 100; //convert from % to decimal
            double.TryParse(tbPerm.Text, out  perm);
            double.TryParse(tbRockComp.Text, out  rockComp);
            double.TryParse(tbTotalComp.Text, out  totalComp);
            double.TryParse(tbLiquComp.Text, out  liquidComp);

            double.TryParse(tbWaterSat.Text, out  Sw);
            Sw = Sw / 100; //convert from % to decimal
            double.TryParse(tbOilSat.Text, out  So);
            So = So / 100; //convert from % to decimal
            double.TryParse(tbBubblePoint.Text, out  Pb);
            double.TryParse(tbInitialBo.Text, out  Boi);
            double.TryParse(tbOilVisc.Text, out  oilVisc);
            double.TryParse(txInitialP.Text, out  Pinitial);
            double.TryParse(txPresToConvert.Text, out  Pmin);
            double.TryParse(txPresToConvert.Text, out  ConvToInjPres);

            wells[0] = cbWell1Active.Checked;
            Inj[0] = cbWell1Injector.Checked;
            QwConst[0] = rbWell1Qw.Checked; //1=Qw, 0=Pwf
            double.TryParse(tbWell1Pwf.Text, out  PwfPres[0]);
            double.TryParse(tbWell1Qw.Text, out  rate);
            QwRate[0] = -rate; //production = negative
            double.TryParse(tbWell1Skin.Text, out  Skin[0]);
            double.TryParse(tbWell1rw.Text, out  WellRw[0]); //[ft]
            double.TryParse(tbWell1X.Text, out  X_loc[0]); //[ft]
            double.TryParse(tbWell1Y.Text, out  Y_loc[0]); //[ft]

            wells[1] = cbWell2Active.Checked;
            Inj[1] = cbWell2Injector.Checked;
            QwConst[1] = rbWell2Qw.Checked; //1=Qw, 0=Pwf
            double.TryParse(tbWell2Pwf.Text, out  PwfPres[1]);
            double.TryParse(tbWell2Qw.Text, out  rate);
            QwRate[1] = -rate; //production = negative
            double.TryParse(tbWell2Skin.Text, out  Skin[1]);
            double.TryParse(tbWell2rw.Text, out  WellRw[1]); //[ft]
            double.TryParse(tbWell2X.Text, out  X_loc[1]); //[ft]
            double.TryParse(tbWell2Y.Text, out  Y_loc[1]); //[ft]

            wells[2] = cbWell3Active.Checked;
            Inj[2] = cbWell3Injector.Checked;
            QwConst[2] = rbWell3Qw.Checked; //1=Qw, 0=Pwf
            double.TryParse(tbWell3Pwf.Text, out  PwfPres[2]);
            double.TryParse(tbWell3Qw.Text, out  rate);
            QwRate[2] = -rate; //production = negative
            double.TryParse(tbWell3Skin.Text, out  Skin[2]);
            double.TryParse(tbWell3rw.Text, out  WellRw[2]); //[ft]
            double.TryParse(tbWell3X.Text, out  X_loc[2]); //[ft]
            double.TryParse(tbWell3Y.Text, out  Y_loc[2]); //[ft]

            //calculate constants
            double alpha = 158 * porosity * oilVisc * liquidComp / perm * Math.Pow(delta_x, 2) / delta_t;
            double beta = -2-alpha;
            double wellTerm; //will calculate later in program
            double re = 0.14 * Math.Sqrt(Math.Pow(delta_x, 2) + Math.Pow(delta_y, 2)); //Peaceman

            //set up the arrays
            double[] x_array = new double[grid_x ];
            double[] Pn = new double[grid_x];
            double[] a = new double[grid_x];
            double[] b = new double[grid_x];
            double[] c = new double[grid_x];
            double[] d = new double[grid_x];
          

            x_array[0] = delta_x / 2;
            Pn[0] = Pinitial;

            int i; //time step
            int n; //grid block

            Qw = new double[time_steps+1, 3];
            Pwf = new double[time_steps+1, 3];
            P = new double[time_steps+1, grid_x];

            //set up the dirac delta well term and initialize arrays
            double[] dirac = new double[grid_x ];
            for (int x = 0;  x < (grid_x);  x++)
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
            b[(grid_x-1 )] = 1 + beta; //no flow x=L boundary
            c[(grid_x-1 )] = 0;
            
            //mark where the wells are in the dirac delta array
            for (int ii = 0; ii < 3; ii++)
            {
                if (wells[ii] == true)
                {
                    n = Convert.ToInt32((X_loc[ii] / delta_x)+1);
                    dirac[n] = 1;
                }
            }
            
            //Initialize settings of the graph
            chart1.ChartAreas[0].AxisX.MajorGrid.Interval = delta_x;
            chart1.ChartAreas[0].AxisX.Title = "x, ft";
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = length;
            chart1.ChartAreas[0].AxisY.Title = "P, psia";
            
            //initialize the first series (t=0) on the graph
            string seriesName = "Time Step #0"; ;
            chart1.Series.Add(seriesName);
            chart1.Series[seriesName].ChartType = SeriesChartType.Line;
            for (int pi = 0; pi < grid_x; pi++)
            {
                chart1.Series[0].Points.AddXY(x_array[pi], P[0, pi]);
            }

            //Output OOIP in bbls
            double ooip = porosity * So * length * width * height / 5.6145;
            lbOOIP.Text = "OOIP = " + ooip.ToString("N0") + " bbls";

            //set up place to store production, one column for each well
            double[] CumProd = new double[3];
            double resProd = 0;
            double RecoveryFactor = 0;

            //MAIN PRESSURE CALCULATIONS
            for (n = 0; n < time_steps; n++)
            {
                for (int ii = 0; ii < grid_x; ii++) //set the Thomas Method arrays
                {
                    Pn[ii] = P[n, ii];
                    //Qw[n, ii] = 0;
                    //Pwf[n, ii] = 0;
                }

                //add the well terms to the b an d arrays
                for (int ii = 0; ii < 3; ii++)
                {
                    if (wells[ii] == true)
                    {
                        int loc = Convert.ToInt32((X_loc[ii] / delta_x))-1;
                        if (QwConst[ii]==true)
                        {
                            wellTerm = 887.53 * QwRate[ii] * oilVisc * Bo_n(Pn[loc]) * delta_x / (perm * delta_y * delta_z);
                            d[loc] = d[loc] - wellTerm;
                            Qw[n, ii] = QwRate[ii];
                            if (n>0)
	                        {
                                Pwf[ n-1,ii] =Pn[loc]-(-QwRate[ii])/Jw(Pn[loc],WellRw[ii],Skin[ii]);
                                CumProd[ii] = CumProd[ii] - QwRate[ii] * delta_t;
	                        }
                                
                        }
                        else
                        {
                            wellTerm = 887.53 * 0.00708  / (Math.Log(re / WellRw[ii]) + Skin[ii]) * delta_x / delta_y;
                            d[loc] = d[loc] - wellTerm*PwfPres[ii];
                            b[loc] = b[loc] - wellTerm;
                            Pwf[n,ii]=PwfPres[ii];
                            if (n > 0)
                            {
                                Qw[n - 1,ii] = -(Pn[loc]-PwfPres[ii])*Jw(Pn[loc], WellRw[ii], Skin[ii]);
                                CumProd[ii] = CumProd[ii] - Qw[n-1,ii] * delta_t;
                            }
                            else
                            {
                                CumProd[ii] = -(Pinitial - PwfPres[ii]) * Jw(Pinitial, WellRw[ii], Skin[ii]);
                            }
                        }
                    }
                }

                Pn = ThomasMethod(a, b, c, d, grid_x);

                //reset the beta and d terms
                for (int ii = 0; ii < (grid_x ); ii++)
                {
                    b[ii] = beta;
                    b[0] = 1 + beta;
                    b[grid_x-1] = 1+beta;
                    d[ii] = -alpha * Pn[ii];
                }

                //save Pn pressure array to Pn+1 in the P[,] matix
                for (int ii= 0; ii < grid_x; ii++) 
                {
                    P[n + 1, ii] = Pn[ii];
                }

                //chart the new time step
                seriesName = "Time Step #" + n+1; ;
                chart1.Series.Add(seriesName);
                chart1.Series[seriesName].ChartType = SeriesChartType.Line;


                for (int ii = 0; ii < grid_x; ii++ )
                {
                    chart1.Series[seriesName].Points.AddXY(x_array[ii], P[n + 1, ii]);
                }

                for (int wellID = 0; wellID < 3; wellID++)
                {
                    if (wells[wellID] == true)
                    {
                        chart1.ChartAreas[0].AxisX.StripLines.Add(new StripLine());
                        chart1.ChartAreas[0].AxisX.StripLines[wellID].BackColor = Color.Black;
                        chart1.ChartAreas[0].AxisX.StripLines[wellID].StripWidth = 40;
                        chart1.ChartAreas[0].AxisX.StripLines[wellID].Interval = 10000;
                        chart1.ChartAreas[0].AxisX.StripLines[wellID].IntervalOffset = X_loc[wellID];
                        chart1.ChartAreas[0].AxisX.StripLines[wellID].Text = "Well " + Convert.ToString(wellID + 1);

                        //while we're here, go ahead and update cumulative production
                        
                    }

                    resProd = CumProd[0] + CumProd[1] + CumProd[2];
                    RecoveryFactor = resProd / ooip;
                }
            }

            lbRF.Text = "Recovery = " + RecoveryFactor.ToString("P2");
            lbProdTotal.Text = "Production = " + resProd.ToString("N0") + " STB";
            lbProdWell1.Text = "Well1 = " + CumProd[0].ToString("N0") + " STB";
            lbProdWell2.Text = "Well2 = " + CumProd[1].ToString("N0") + " STB";
            lbProdWell3.Text = "Well3 = " + CumProd[2].ToString("N0") + " STB";

            /*
            //initialize the first series (t=0) on the graph
            string seriesName = "Time Step #0"; ;
            chart1.Series.Add(seriesName);
            chart1.Series[seriesName].ChartType = SeriesChartType.Line;

            //Initializes the basics of the pressure vs x vs time plot (@time = 0)
            for (int pi=0; pi < grid_x ; pi++ )
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
                while (i < grid_x )
                {
                    chart1.Series[seriesName].Points.AddXY(x_array[i], P[n, i]);
                    i++;
                }
                n++;
            }

            */

        }

        private void SaveExcel()
        {

            CreateExcelDoc excell_app = new CreateExcelDoc();

            for (int i = 0; i < P.GetLength(0); i++)
            {
                for (int j = 0; j < P.GetLength(1); j++)
                {
                    excell_app.addData(i+1, j+1, P[i, j]);
                }
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
            Jw_n= 0.00708 / oilVisc / Bn * perm * height / (Math.Log(re/rw) +S);
            return Jw_n;
        }

        private double[] ThomasMethod(double[] a, double[] b, double[] c, double[] d, int n)
        {
            
            double[] P = new double[n];
            double[] w = new double[n];
            double[] g = new double[n];
       
            w[0] = c[0] / b[0];
            g[0] = d[0] / b[0];

            for (int i = 1; i < n; i++)
            {
                w[i] = c[i] / (b[i] - a[i]*w[i-1]);
            }


            for (int i = 1; i < n; i++)
            {
                g[i] = (d[i] - a[i] * g[i - 1]) / (b[i] - a[i] * w[i - 1]);
            }

            P[n - 1] = g[n - 1];

            for (int i = n - 2; i >= 0; i--)
			{
			    P[i] = g[i] - w[i]*P[i+1];
			}

            return P;
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
           Form1 f1 = new Form1(Qw, delta_t); // Instantiate a Form1 object.
           f1.Show();
        }

        private void tbTimeStep_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveExcel();
        }

    }




    class CreateExcelDoc
    {
        
        private Application app = null;
        private Workbook workbook = null;
        private Worksheet worksheet = null;
        private Range workSheet_range = null;
        public CreateExcelDoc()
        {
            createDoc(); 
        }
        public void createDoc()
        {
            try
            {       
                app = new Application();
                app.Visible = true;
                workbook = app.Workbooks.Add(1);
                worksheet = (Worksheet)workbook.Sheets[1];
            }
            catch (Exception e)
            {
                Console.Write("Error");
            }
            finally
            {
            }
        }

public void createHeaders(int row, int col, string htext, string cell1, string cell2, int mergeColumns,string b, bool font,int size,string fcolor)
        {
            worksheet.Cells[row, col] = htext;
            workSheet_range = worksheet.get_Range(cell1, cell2);
            workSheet_range.Merge(mergeColumns);
            switch(b)
            {
                case "YELLOW":
                workSheet_range.Interior.Color = System.Drawing.Color.Yellow.ToArgb();
                break;
                case "GRAY":
                    workSheet_range.Interior.Color = System.Drawing.Color.Gray.ToArgb();
                break;
                case "GAINSBORO":
                    workSheet_range.Interior.Color = 
			System.Drawing.Color.Gainsboro.ToArgb();
                    break;
                case "Turquoise":
                    workSheet_range.Interior.Color = 
			System.Drawing.Color.Turquoise.ToArgb();
                    break;
                case "PeachPuff":
                    workSheet_range.Interior.Color = 
			System.Drawing.Color.PeachPuff.ToArgb();
                    break;
                default:
                  //  workSheet_range.Interior.Color = System.Drawing.Color..ToArgb();
                    break;
            }
         
            workSheet_range.Borders.Color = System.Drawing.Color.Black.ToArgb();
            workSheet_range.Font.Bold = font;
            workSheet_range.ColumnWidth = size;
            if (fcolor.Equals(""))
            {
                workSheet_range.Font.Color = System.Drawing.Color.White.ToArgb();
            }
            else {
                workSheet_range.Font.Color = System.Drawing.Color.Black.ToArgb();
            }
        }

        public void addData(int row, int col, double data)
        {
            worksheet.Cells[row, col] = data;
        }   

    }

}