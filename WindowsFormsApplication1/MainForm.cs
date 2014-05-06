// PTE 508 - Spring 2014
// Project #1
// by Tracy Lenz
// Written in C# using MS Visual Studio 2013

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

		private void MainForm_Load(object sender, EventArgs e)
		{

		}

		private void textBox4_TextChanged(object sender, EventArgs e)
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
			time_steps = Convert.ToInt32(time_frame / delta_t)+1; // time_frame divided by delta_t

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

			bool ConvToInj = cbConvertInj.Checked;
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
			//I don't think the dirac variable actually ended up being used...
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
			string seriesName = "Initial Conditions";
			chart1.Series.Add(seriesName);
			chart1.Series[seriesName].ChartType = SeriesChartType.Line;
			chart1.Series[seriesName].BorderWidth = 2;
			for (int pi = 0; pi < grid_x; pi++)
			{
				chart1.Series[0].Points.AddXY(x_array[pi], P[0, pi]);
			}

			//Output OOIP in bbls
			double ooip = porosity * So * length * width * height / Boi/ 5.6145;
			lbOOIP.Text = "OOIP = " + ooip.ToString("N0") + " STB";

			//set up place to store production, one column for each well
			double[] CumProd = new double[3];
			double resProd = 0;
			double RecoveryFactor = 0;

			//---Uncomment the definition of "timePlot" that you want to use---
			//int[] timePlot = {0,1,2,3,4,5,6,7,8,9,10,20,30,40,50,75,100,125,150,175,200,250,300,350,400,450,500, 600, 700, 800, 900, 1000};
			//int[] timePlot = { 0,  50, 100 };
			int[] timePlot = { 0, 50,100, 150,200, 250, 300, 350, 400, 450, 500 };
			
			int ts_count = 0;
			double Qtotal_t1 = 0.0f;

			//MAIN PRESSURE CALCULATIONS
			for (n = 0; n < time_steps; n++)
			{
				if (n==1)
				{
					Qtotal_t1 = QwTotal(Qw, 0);
				}
				if ((Qtotal_t1 *0.01<  QwTotal(Qw, n - 1)) || n <= 1 || ConvToInj )
				{
					//add the well terms to the b an d arrays
					for (int ii = 0; ii < 3; ii++)
					{
						if (wells[ii] == true)
						{
							//identify where well_ii is located
							int loc = Convert.ToInt32((X_loc[ii] / delta_x)) - 1;


							//Is this a constant rate well?
							if (QwConst[ii] == true)
							{
								wellTerm = 887.53 * QwRate[ii] * oilVisc * Bo_n(Pn[loc]) * delta_x / (perm * delta_y * delta_z);
								d[loc] = d[loc] - wellTerm;
								Qw[n, ii] = QwRate[ii];
							}
							else //if not constant rate, use the set Pwf to calcuate
							{
								if (n > 0 && Inj[ii] == true && -Qw[n - 1, ii] < -0.1 * Qw[0, ii])
								{
									wellTerm = 887.53 * 0.00708 / (Math.Log(re / WellRw[ii]) + Skin[ii]) * delta_x / delta_y;
									d[loc] = d[loc] - wellTerm * ConvToInjPres;
									b[loc] = b[loc] - wellTerm;
									Pwf[n, ii] = ConvToInjPres;
								}
								else
								{
									wellTerm = 887.53 * 0.00708 / (Math.Log(re / WellRw[ii]) + Skin[ii]) * delta_x / delta_y;
									d[loc] = d[loc] - wellTerm * PwfPres[ii];
									b[loc] = b[loc] - wellTerm;
									Pwf[n, ii] = PwfPres[ii];
								}
							}
						}
					}

					Pn = ThomasMethod(a, b, c, d, grid_x);

					//reset the beta and d terms
					for (int ii = 0; ii < (grid_x); ii++)
					{
						b[ii] = beta;
						b[0] = 1 + beta;
						b[grid_x - 1] = 1 + beta;
						d[ii] = -alpha * Pn[ii];
					}

					//save Pn pressure array to Pn+1 in the P[,] matix
					for (int ii = 0; ii < grid_x; ii++)
					{
						P[n + 1, ii] = Pn[ii];
					}

					//chart the new time step (if it's an important one)
					if (Array.Exists(timePlot, element => element == (n + 1) * delta_t) || (n + 1) * delta_t == time_frame)
					{
						seriesName = "Time = " + ((n + 1) * delta_t) + " days";
						chart1.Series.Add(seriesName);
						chart1.Series[seriesName].ChartType = SeriesChartType.Line;
						chart1.Series[seriesName].BorderWidth = 2;

						for (int ii = 0; ii < grid_x; ii++)
						{
							chart1.Series[seriesName].Points.AddXY(x_array[ii], P[n + 1, ii]);
						}

						for (int wellID = 0; wellID < 3; wellID++)
						{
							if (wells[wellID] == true)
							{
								chart1.ChartAreas[0].AxisX.StripLines.Add(new StripLine());
								chart1.ChartAreas[0].AxisX.StripLines[wellID].BackColor = Color.Black;
								chart1.ChartAreas[0].AxisX.StripLines[wellID].StripWidth = 20;
								chart1.ChartAreas[0].AxisX.StripLines[wellID].Interval = 10000;
								chart1.ChartAreas[0].AxisX.StripLines[wellID].IntervalOffset = X_loc[wellID];
								chart1.ChartAreas[0].AxisX.StripLines[wellID].Text = "Well " + Convert.ToString(wellID + 1);
							}
						}
						ts_count++;
					} //end of charting loop

					//update cum. production and recovery factor calcs
					for (int wellID = 0; wellID < 3; wellID++)
					{
						if (wells[wellID] == true)
						{
							//identify where well_ii is located
							int loc = Convert.ToInt32((X_loc[wellID] / delta_x)) - 1;

							if (QwConst[wellID] == true)
							{
								Pwf[n, wellID] = P[n+1,loc] - (-Qw[n, wellID]) / Jw(P[n,loc], WellRw[wellID], Skin[wellID]);
								CumProd[wellID] = CumProd[wellID] - Qw[n, wellID] * delta_t;
							}
							else
							{
								Qw[n, wellID] = -(P[n+1,loc] - Pwf[n, wellID]) * Jw(P[n,loc], WellRw[wellID], Skin[wellID]);
								CumProd[wellID] = CumProd[wellID] - Qw[n, wellID] * delta_t;
							}
						}
					}

					resProd = CumProd[0] + CumProd[1] + CumProd[2];
					RecoveryFactor = resProd / ooip;
				}
			} //end of main pressure calculations
			
			lbRF.Text = "Recovery = " + RecoveryFactor.ToString("P2");
			lbProdTotal.Text = "Production = " + resProd.ToString("N0") + " STB";
			lbProdWell1.Text = "Well1 = " + CumProd[0].ToString("N0") + " STB";
			lbProdWell2.Text = "Well2 = " + CumProd[1].ToString("N0") + " STB";
			lbProdWell3.Text = "Well3 = " + CumProd[2].ToString("N0") + " STB";

		}

		private double QwTotal(double[,] Qw, int timestep)
		{
			double _QwTotal = 0.0f;
			if (timestep<0)
			{
				_QwTotal = 0.0f;
			}
			else
			{
				for (int i = 0; i < Qw.GetLength(1); i++)
				{
					_QwTotal = (_QwTotal-Qw[timestep, i]);
				}
			}
			return _QwTotal;
		}

		private void SaveExcel(CreateExcelDoc myWB, double[,] x, int sheet)
		{
			for (int i = 0; i < x.GetLength(0); i++)
			{
				for (int j = 0; j < x.GetLength(1); j++)
				{
					myWB.addData(i + 1, j + 1, x[i, j], sheet);
				}
			}
		}

		//calculate the pressure dependent FVF
		private double Bo_n(double Pn)
		{
			double Bn = Boi*Math.Exp(-liquidComp*(Pn-Pinitial));
			return Bn;
		}
		
		//Productivity index from Peaceman's Method
		private double Jw(double Pn, double rw, double S)
		{
			double re = 0.14 * Math.Sqrt(Math.Pow(delta_x, 2) + Math.Pow(delta_y, 2));
			double Bn = Boi*Math.Exp(-liquidComp*(Pn-Pinitial));
			//double Bn = 1.25;
			double Jw_n;
			Jw_n= 0.00708 / (oilVisc * Bn) * perm * height / (Math.Log(re/rw) +S);
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

		private void button2_Click(object sender, EventArgs e)
		{
		   Form1 f1 = new Form1(Qw, delta_t); // Instantiate a Form1 object.
		   f1.Show();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			CreateExcelDoc excell_app = new CreateExcelDoc();
			SaveExcel(excell_app, P, 0);
			SaveExcel(excell_app, Qw, 1);
			SaveExcel(excell_app, Pwf, 2);
			MessageBox.Show("Export to Excel is complete","Important Message");
		}

	}
	

	class CreateExcelDoc
	{
		private Application app = null;
		public Workbook workbook = null;
		public Worksheet[] worksheet = new Worksheet[20];
		private Range workSheet_range = null;

		public CreateExcelDoc()
		{
			createDoc(5); 
		}
		public void createDoc(int sheets)
		{
			try
			{       
				app = new Application();
				app.Visible = true;
				workbook = app.Workbooks.Add();
				for (int i = 0; i < sheets+1; i++)
				{
					worksheet[i] = workbook.Worksheets.Add();
				}
				
			} 
			catch (Exception e)
			{
				Console.Write("Error: "+e);
			}
			finally
			{
			}
		}

		public void addData(int row, int col, double data, int i)
		{
			worksheet[i].Cells[row, col] = data;
		}   

	}


	class Homework9
	{
		public void HW9()
		{
			double Pinitial = 3000;
			double Cm = -5.2;
			double Sm = 1;
			double Wm = 1;
			double Nm = 1;
			double Em = 1;
			double rm = -1.2;
			double well = 887.53 * 100 * 2 * 1 / (50 * 78);
			int Nx = 5;
			int Ny = 3;
			double deltat = 10;
			int timesteps = Convert.ToInt32(500 / deltat);

			double[,] P = new double[timesteps + 1, 15];
			double[] Pn = new double[15];
			double[,] mymatrix = new double[15, 15];
			double[] RHS = new double[15];

			for (int i = 0; i < mymatrix.GetLength(0); i++)
			{
				Pn[i] = Pinitial;
				P[0, i] = Pinitial;
			}

			double[,] testmatrix = new double[3, 3] { { 4, 0, 1 }, { 0, 3, 2 }, { 1, 2, 4 } };
			double[] b = new double[3] { 2, 1, 3 };
			double[,] A1 = new double[3, 3];
			double[] b1 = new double[3];
			double[] initial = new double[3] { 0, 0, 0 };
			b.CopyTo(b1, 0);
			double[] x1 = new double[3];
			x1 = GSSolve(testmatrix, b1, 0, 10);

			mymatrix = CreateMatrix(Nx, Ny, Nm, Sm, Wm, Em, Cm);

			for (int n = 0; n < timesteps; n++)
			{
				RHS = RHS_Create(3, 2, Nx, Ny, Pn, rm, well);
				Pn = GSSolve(mymatrix, RHS, 3000, 10);

				for (int i = 0; i < mymatrix.GetLength(0); i++)
				{
					P[n + 1, i] = Pn[i];
				}
			}
			CreateExcelDoc excell_app = new CreateExcelDoc();
			SaveExcel(excell_app, P, 0);
		}

		private double[] RHS_Create(double xw, double yw, double Nx, double Ny, double[] Pn, double rm, double well)
		{
			int well_m = Convert.ToInt32((yw - 1) * Nx + xw - 1);
			int grids = Convert.ToInt32(Nx * Ny);
			double[] RHS = new double[grids];

			for (int i = 0; i < grids; i++)
			{
				RHS[i] = rm * Pn[i];
				if (i == well_m)
				{
					RHS[i] = RHS[i] + well;
				}
			}
			return RHS;
		}

		private double[,] CreateMatrix(double Nx, double Ny, double Nm, double Sm, double Wm, double Em, double Cm)
		{
			int grids = Convert.ToInt32(Nx * Ny);
			double[,] mymatrix = new double[grids, grids];
			double rem_i;
			double rem_j;

			for (int i = 0; i < mymatrix.GetLength(0); i++)
			{
				for (int j = 0; j < mymatrix.GetLength(1); j++)
				{
					rem_i = (i + 1) % Nx;
					rem_j = (j + 1) % Nx;
					mymatrix[i, j] = 0;
					if (i == j) { mymatrix[i, j] = Cm; }
					if (i == j + 1) { mymatrix[i, j] = Wm; }
					if (i == j - 1) { mymatrix[i, j] = Em; }
					if (i == j - Nx) { mymatrix[i, j] = Nm; }
					if (i - Nx == j) { mymatrix[i, j] = Sm; }
					if (rem_i == 1 && rem_j == 0) { mymatrix[i, j] = 0; }
					if (rem_i == 0 && rem_j == 1) { mymatrix[i, j] = 0; }
					if (i == j && i < Nx) { mymatrix[i, j] = mymatrix[i, j] + Sm; }
					if (i == j && i > Nx * Ny - Nx - 1) { mymatrix[i, j] = mymatrix[i, j] + Nm; }
					if (i == j && rem_i == 1) { mymatrix[i, j] = mymatrix[i, j] + Wm; }
					if (i == j && rem_i == 0) { mymatrix[i, j] = mymatrix[i, j] + Em; }
				}
			}

			return mymatrix;
		}

		private void SaveExcel(CreateExcelDoc myWB, double[,] x, int sheet)
		{



			for (int i = 0; i < x.GetLength(0); i++)
			{
				for (int j = 0; j < x.GetLength(1); j++)
				{
					myWB.addData(i + 1, j + 1, x[i, j], sheet);
				}
			}

		}
		private double[] GSSolve(double[,] matrix, double[] right, double initial, int iterations)
		{
			double[] x = new double[right.Length];
			for (int i = 0; i < right.Length; i++)
			{
				x[i] = initial;
			}
			double entry;
			double diagonal;
			double[] xOld = new double[x.Length];

			for (int k = 0; k < iterations; ++k)
			{
				x.CopyTo(xOld, 0);

				for (int i = 0; i < right.Length; ++i)
				{
					entry = right[i];
					diagonal = matrix[i, i];

					for (int j = 0; j < i; j++)
						entry -= matrix[i, j] * x[j];
					for (int j = i + 1; j < right.Length; j++)
						entry -= matrix[i, j] * xOld[j];

					x[i] = entry / diagonal;
				}
			}
			return x;
		}
	}


}