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


namespace ReservoirSimulator2D
{
	
	public partial class MainForm : Form
	{
		double _timeFrame; // [days] this is how long the simulation will run
		double _deltaT; // [days] the number of days between time steps
		int _timeSteps; // time_frame divided by delta_t

		double _deltaX; //[ft]
		double _deltaY; //[ft]
		double _deltaZ; //[ft]

		int _gridX;
		int _gridY;
		int _gridZ;

		double _length; //[ft]
		double _width; //[ft]
		double _height; //[ft]

		double _porosity;
		double _perm;
		double _rockComp;
		double _totalComp;
		double _oilComp;
	    double _waterComp;


		double _sw;
		double _so;
		double _pb;
		double _boi;
		double _bwi;
		double _bgi;
		double _oilVisc;
		double _gasVisc;
		double _waterVisc;
		double _pinitial;
		double _pmin;
		double _convToInjPres;

		double _rate;
		bool?[] wells = new bool?[3]; //right now the code supports 3 wells
		bool?[] Inj = new bool?[3];
		bool?[] QwConst = new bool?[3];
		double[] QwRate = new double[3];

		double[] PwfPres = new double[3];
		double[] WellRw = new double[3];
		double[] Skin = new double[3];
		double[] X_loc = new double[3];
		double[] Y_loc = new double[3];

		//define the pressure & rate matrices to store the _P values over time and space
        double[,] _P;   // P_avg vs time vs vs grid block
        double[,] Qw;  // rate vs time vs grid block 
        double[,] Pwf; // Pwf vs time vs grid block
	    double[,] _So; // Oil Saturation vs time vs grid block


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
			RefreshData();
		}
   
		private void RefreshData()
		{
			chart1.Series.Clear();
			chart1.ChartAreas[0].AxisX.StripLines.Clear();

			double.TryParse(tbTimeFrame.Text, out _timeFrame); // [days] this is how long the simulation will run
			double.TryParse(tbTimeStep.Text, out  _deltaT); // [days] the number of days between time steps
			_timeSteps = Convert.ToInt32(_timeFrame / _deltaT)+1; // time_frame divided by delta_t

			double.TryParse(txLength.Text, out  _length); //[ft]
			double.TryParse(txWidth.Text, out  _width); //[ft]
			double.TryParse(txHeight.Text, out  _height); //[ft]

			int.TryParse(txXGridBlocks.Text, out  _gridX); //[ft]
			int.TryParse(txYGridBlocks.Text, out  _gridY); //[ft]
			int.TryParse(txZGridBlocks.Text, out  _gridZ); //[ft]

			_deltaX = _length / _gridX; //[ft]
			_deltaY = _width / _gridY; //[ft]
			_deltaZ = _height / _gridZ; //[ft]

			double.TryParse(tbPorosity.Text, out  _porosity);
			_porosity = _porosity / 100; //convert from % to decimal
			double.TryParse(tbPerm.Text, out  _perm);
			double.TryParse(tbRockComp.Text, out  _rockComp);
			double.TryParse(tbTotalComp.Text, out  _totalComp);
			double.TryParse(tbLiquComp.Text, out  _oilComp);
            double.TryParse(tbWaterComp.Text, out  _waterComp);

			double.TryParse(tbWaterSat.Text, out  _sw);
			_sw = _sw / 100; //convert from % to decimal
			double.TryParse(tbOilSat.Text, out  _so);
			_so = _so / 100; //convert from % to decimal
			double.TryParse(tbBubblePoint.Text, out  _pb);
			double.TryParse(tbInitialBo.Text, out  _boi);
			double.TryParse(tbOilVisc.Text, out  _oilVisc);
			double.TryParse(txInitialP.Text, out  _pinitial);
			double.TryParse(txPresToConvert.Text, out  _pmin);
			double.TryParse(txPresToConvert.Text, out  _convToInjPres);

			bool convToInj = cbConvertInj.Checked;
			wells[0] = cbWell1Active.Checked;
			Inj[0] = cbWell1Injector.Checked;
			QwConst[0] = rbWell1Qw.Checked; //1=Qw, 0=Pwf
			double.TryParse(tbWell1Pwf.Text, out  PwfPres[0]);
			double.TryParse(tbWell1Qw.Text, out  _rate);
			QwRate[0] = -_rate; //production = negative
			double.TryParse(tbWell1Skin.Text, out  Skin[0]);
			double.TryParse(tbWell1rw.Text, out  WellRw[0]); //[ft]
			double.TryParse(tbWell1X.Text, out  X_loc[0]); //[ft]
			double.TryParse(tbWell1Y.Text, out  Y_loc[0]); //[ft]

			wells[1] = cbWell2Active.Checked;
			Inj[1] = cbWell2Injector.Checked;
			QwConst[1] = rbWell2Qw.Checked; //1=Qw, 0=Pwf
			double.TryParse(tbWell2Pwf.Text, out  PwfPres[1]);
			double.TryParse(tbWell2Qw.Text, out  _rate);
			QwRate[1] = -_rate; //production = negative
			double.TryParse(tbWell2Skin.Text, out  Skin[1]);
			double.TryParse(tbWell2rw.Text, out  WellRw[1]); //[ft]
			double.TryParse(tbWell2X.Text, out  X_loc[1]); //[ft]
			double.TryParse(tbWell2Y.Text, out  Y_loc[1]); //[ft]

			wells[2] = cbWell3Active.Checked;
			Inj[2] = cbWell3Injector.Checked;
			QwConst[2] = rbWell3Qw.Checked; //1=Qw, 0=Pwf
			double.TryParse(tbWell3Pwf.Text, out  PwfPres[2]);
			double.TryParse(tbWell3Qw.Text, out  _rate);
			QwRate[2] = -_rate; //production = negative
			double.TryParse(tbWell3Skin.Text, out  Skin[2]);
			double.TryParse(tbWell3rw.Text, out  WellRw[2]); //[ft]
			double.TryParse(tbWell3X.Text, out  X_loc[2]); //[ft]
			double.TryParse(tbWell3Y.Text, out  Y_loc[2]); //[ft]

			var wellStrings = new string[] {"Ruby", "Sapphire", "Opal"};


			//calculate constants
			double alpha = 158 * _porosity * _oilVisc * _oilComp / _perm * Math.Pow(_deltaX, 2) / _deltaT;
			double beta = -2-alpha;
		    double re = 0.14 * Math.Sqrt(Math.Pow(_deltaX, 2) + Math.Pow(_deltaY, 2)); //Peaceman


		    var mBlocks = _gridX*_gridY;

            double[,] mymatrix = new double[mBlocks, mBlocks];
            double[] RHS = new double[mBlocks];
			
            //set up the arrays
			double[] x_array = new double[_gridX ];
			double[] Pn = new double[mBlocks];
			double[] a = new double[_gridX];
			double[] b = new double[_gridX];
			double[] c = new double[_gridX];
			double[] d = new double[_gridX];

		    double Bo = Bo_n(_pinitial);
            double Bw = Bw_n(_pinitial);
		    double Mo = Mo_bar(_pinitial, _pinitial, _so, _so);
		    double Mw = Mw_bar(_pinitial, _pinitial, _so, _so);
		    double delY2 = Math.Pow(_deltaY, 2);
            double delX2 = Math.Pow(_deltaX, 2);

            double Cm = -Bo / delY2 * (Mo + Mo) - Bw / delY2 * (Mw + Mw) - Bo / delX2 * (Mo + Mo) - Bw / delX2 * (Mw + Mw);
            double Sm = Bo / delY2 * Mo + Bw / delY2 * Mw;  
            double Wm = Bo / delX2 * Mo + Bw / delX2 * Mw; 
            double Nm = Bo / delY2 * Mo + Bw / delY2 * Mw; 
            double Em = Bo / delX2 * Mo + Bw / delX2 * Mw; 
            
            x_array[0] = _deltaX / 2;
			Pn[0] = _pinitial;

			int n; //grid block

			Qw = new double[_timeSteps+1, 3];
			Pwf = new double[_timeSteps+1, 3];
			_P = new double[_timeSteps+1, mBlocks];
            _So = new double[_timeSteps+1, mBlocks];

			for (int x = 0;  x < (mBlocks);  x++)
			{
				_P[0, x] = _pinitial;
			    _So[0, x] = _so;
			}

            for (int x = 0; x < (_gridX); x++)
            {
               x_array[x] = x * _deltaX + _deltaX / 2;
            }
			
			
			//Initialize settings of the graph
			chart1.ChartAreas[0].AxisX.MajorGrid.Interval = _deltaX;
			chart1.ChartAreas[0].AxisX.Title = "x, ft";
			chart1.ChartAreas[0].AxisX.Minimum = 0;
			chart1.ChartAreas[0].AxisX.Maximum = _length;
			chart1.ChartAreas[0].AxisY.Title = "P, psia";
			
			//initialize the first series (t=0) on the graph
			string seriesName = "Initial Conditions";
			chart1.Series.Add(seriesName);
			chart1.Series[seriesName].ChartType = SeriesChartType.Line;
			chart1.Series[seriesName].BorderWidth = 2;
			for (int pi = 0; pi < _gridX; pi++)
			{
				chart1.Series[0].Points.AddXY(x_array[pi], _P[0, pi]);
			}

			//Output OOIP in bbls
			double ooip = _porosity * _so * _length * _width * _height / _boi/ 5.6145;
			lbOOIP.Text = @"OOIP = " + ooip.ToString("N0") + @" STB";

			//set up place to store production, one column for each well
			double[] cumProd = new double[3];
			double resProd = 0;
			double recoveryFactor = 0;

			//---Uncomment the definition of "timePlot" that you want to use---
			int[] timePlot = {0,1,2,3,4,5,6,7,8,9,10,20,30,40,50,75,100,125,150,175,200,250,300,350,400,450,500, 600, 700, 800, 900, 1000};
			//int[] timePlot = { 0,  50, 100 };
			//int[] timePlot = { 0, 50,100, 150,200, 250, 300, 350, 400, 450, 500 };
		
			double qtotalT1 = 0.0f;

			//MAIN PRESSURE CALCULATIONS
			for (n = 0; n < _timeSteps; n++)
			{
                //update B, Sat, Krel inputs

                //build a new matrix

                //build the RHS of the equation
			    RHS = RHS_Create(c,);

                //account for what the wells are doing
				if (n==1)
				{
					qtotalT1 = QwTotal(Qw, 0);
				}
				if ((qtotalT1 *0.01<  QwTotal(Qw, n - 1)) || n <= 1 || convToInj )
				{
					//add the well terms to the b an d arrays
					for (int ii = 0; ii < 3; ii++)
					{
						if (wells[ii] == true)
						{
							//identify where well_ii is located
							int loci = Convert.ToInt32(Math.Floor((X_loc[ii] / _deltaX)));
                            int locj = Convert.ToInt32(Math.Floor((Y_loc[ii] / _deltaY)));
						    int loc = m(loci, locj);

							//Is this a constant rate well? (won't be used for project 2)
						    double wellTerm; 
						    if (QwConst[ii] == true)
						    {
						        double jwii = .2;
                                wellTerm = well(Bo, jwii);
								d[loc] = d[loc] - wellTerm;
								Qw[n, ii] = QwRate[ii];
							}
							else //if not constant rate, use the set Pwf to calcuate
							{
                                double jwo = Jwo(Pn[loc],WellRw[ii],Skin[ii],_So[n,loc]);
                                double jww = Jww(Pn[loc],WellRw[ii],Skin[ii],_So[n,loc]);
							    double bo_m = Bo_n(Pn[loc]);
							    double bw_m = Bw_n(Pn[loc]);
                                double gamma_o = well(bo_m, jwo);
                                double gamma_w = well(bw_m, jww);
								
                                if (n > 0 && Inj[ii] == true && -Qw[n - 1, ii] < -0.1 * Qw[0, ii])
                                {
									mymatrix[loc,loc] = mymatrix[loc,loc]+gamma_o+gamma_w;
									RHS[loc] = RHS[loc] - gamma_o*_convToInjPres-gamma_w*_convToInjPres;
									Pwf[n, ii] = _convToInjPres;
								}
								else
								{
									mymatrix[loc,loc] = mymatrix[loc,loc]+gamma_o+gamma_w;
									RHS[loc] = RHS[loc] - gamma_o*PwfPres[ii]-gamma_w*PwfPres[ii];
									Pwf[n, ii] = PwfPres[ii];
								}
							}
						}
					}

                    //solve the matrix
				    Pn = GSSolve(mymatrix, RHS, _pinitial, 10);
					//Pn = ThomasMethod(a, b, c, d, _gridX);

					//reset the beta and d terms
					for (int ii = 0; ii < (_gridX); ii++)
					{
						b[ii] = beta;
						b[0] = 1 + beta;
						b[_gridX - 1] = 1 + beta;
						d[ii] = -alpha * Pn[ii];
					}

					//save Pn pressure array to Pn+1 in the P[,] matix
					for (int ii = 0; ii < _gridX; ii++)
					{
						_P[n + 1, ii] = Pn[ii];
					}

					//chart the new time step (if it's an important one)
					if (Array.Exists(timePlot, element => element == (n + 1) * _deltaT) || (n + 1) * _deltaT == _timeFrame)
					{
						seriesName = "Time = " + ((n + 1) * _deltaT) + " days";
						chart1.Series.Add(seriesName);
						chart1.Series[seriesName].ChartType = SeriesChartType.Line;
						chart1.Series[seriesName].BorderWidth = 2;

						for (int ii = 0; ii < _gridX; ii++)
						{
							chart1.Series[seriesName].Points.AddXY(x_array[ii], _P[n + 1, ii]);
						}

						//add the black lines to the preview chart showing where the wells are
						for (int wellID = 0; wellID < 3; wellID++)
						{
							if (wells[wellID] == true)
							{
								chart1.ChartAreas[0].AxisX.StripLines.Add(new StripLine());
								chart1.ChartAreas[0].AxisX.StripLines[wellID].BackColor = Color.Black;
								chart1.ChartAreas[0].AxisX.StripLines[wellID].StripWidth = 10;
								chart1.ChartAreas[0].AxisX.StripLines[wellID].Interval = 10000;
								chart1.ChartAreas[0].AxisX.StripLines[wellID].IntervalOffset = X_loc[wellID];
								chart1.ChartAreas[0].AxisX.StripLines[wellID].Text = wellStrings[wellID];
							}
							if (wells[wellID] == false)
							{
								chart1.ChartAreas[0].AxisX.StripLines.Add(new StripLine());
								chart1.ChartAreas[0].AxisX.StripLines[wellID].BackColor = Color.Black;
								chart1.ChartAreas[0].AxisX.StripLines[wellID].StripWidth = 0;
								chart1.ChartAreas[0].AxisX.StripLines[wellID].Interval = 10000;
								chart1.ChartAreas[0].AxisX.StripLines[wellID].IntervalOffset = X_loc[wellID];
								//chart1.ChartAreas[0].AxisX.StripLines[wellID].Text = "Well " + Convert.ToString(wellID + 1);
							}
						}
					} //end of charting loop

					//update cum. production and recovery factor calcs
					for (int wellID = 0; wellID < 3; wellID++)
					{
						if (wells[wellID] == true)
						{
							//identify where well_ii is located
							int loc = Convert.ToInt32((X_loc[wellID] / _deltaX)) - 1;

							if (QwConst[wellID] == true)
							{
								Pwf[n, wellID] = _P[n+1,loc] - (-Qw[n, wellID]) / Jw(_P[n,loc], WellRw[wellID], Skin[wellID]);
								cumProd[wellID] = cumProd[wellID] - Qw[n, wellID] * _deltaT;
							}
							else
							{
								Qw[n, wellID] = -(_P[n+1,loc] - Pwf[n, wellID]) * Jw(_P[n,loc], WellRw[wellID], Skin[wellID]);
								cumProd[wellID] = cumProd[wellID] - Qw[n, wellID] * _deltaT;
							}
						}
					}

					resProd = cumProd[0] + cumProd[1] + cumProd[2];
					recoveryFactor = resProd / ooip;
				}
			} //end of main pressure calculations
			
			lbRF.Text = @"Recovery = " + recoveryFactor.ToString("P2");
			lbProdTotal.Text = @"Production = " + resProd.ToString("N0") + @" STB";
			lbProdWell1.Text = @"Well1 = " + cumProd[0].ToString("N0") + @" STB";
			lbProdWell2.Text = @"Well2 = " + cumProd[1].ToString("N0") + @" STB";
			lbProdWell3.Text = @"Well3 = " + cumProd[2].ToString("N0") + @" STB";

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
			double Bn = 1.25*Math.Exp(-_oilComp*(Pn-1000));
			return Bn;
		}

        private double Bw_n(double Pn)
        {
            double Bn = 1.02 * Math.Exp(-_waterComp * (Pn - 1000));
            return Bn;
        }

        private double phi_n(double Pn)
        {
            double Phi_n = 0.2 * Math.Exp(-_rockComp * (Pn - 1000));
            return Phi_n;
        }

        private double Mobil(double k, double Kr, double mu, double B)
        {
            double M = _perm * Kr / (mu * B);
            return M;
        }

        private double Mo_bar(double P1, double P2, double So1, double So2)
        {
            double P = (P1 + P2)/2;
            double So;
            if (P1 > P2)
            {
                So = So1;
            }
            else
            {
                So = So2;}
            double M = _perm * Kro(0,0.2, (1-So)) / (_oilVisc * Bo_n(P));
            return M;
        }

        private double Mw_bar(double P1, double P2, double So1, double So2)
        {
            double P = (P1 + P2)/2;
            double So;
            if (P1 > P2)
            {
                So = So1;
            }
            else
            {
                So = So2;}
            double M = _perm * Krw(0, (1-So)) / (_oilVisc * Bw_n(P));
            return M;
        }

        private double Krw(double Swc, double Sw)
        {
            double Sn = (Sw - Swc)/(1 - Swc);
            double kr;
            if (Swc < Sw)
            {
                kr = Math.Pow(Sn, 4);
            }
            else
            {
                kr = 0;
            }
            return kr;
        }

        private double Kro(double Swc, double Sor,double Sw)
        {
            double So = 1 - Sw;
            double Sn = (Sw - Swc) / (1 - Swc);
            double kr;
            if (Sor < So || Sw<(1-Sor))
            {
                kr = Math.Pow(1-Sn, 2)*(1-Math.Pow(Sn, 2));
            }
            else
            {
                kr = 0;
            }
            return kr;
        }

        private int m(int i, int j)
        {
            var m_loc = (j - 1)*_gridX + i;
            return m_loc;
        }

        private double well(double B, double Jw)
        {
            var wellTerm = -887.53*(B*Jw)/(_deltaX*_deltaY*_deltaZ);
            return wellTerm;
        }

		//Productivity index from Peaceman's Method
		private double Jw(double Pn, double rw, double S)
		{
			double re = 0.14 * Math.Sqrt(Math.Pow(_deltaX, 2) + Math.Pow(_deltaY, 2));
			double Bn = _boi*Math.Exp(-_oilComp*(Pn-_pinitial));
			//double Bn = 1.25;
			double Jw_n;
			Jw_n= 0.00708 / (_oilVisc * Bn) * _perm * _height / (Math.Log(re/rw) +S);
			return Jw_n;
		}

        private double Jwo(double Pn, double rw, double S, double So)
        {
            double re = 0.14 * Math.Sqrt(Math.Pow(_deltaX, 2) + Math.Pow(_deltaY, 2));
            double Bon = Bo_n(Pn);
            double kro = Kro(0.2, 0, (1-So));
            double Jw_n;
            Jw_n = 0.00708 / (_oilVisc * Bon) * _perm*kro * _height / (Math.Log(re / rw) + S);
            return Jw_n;
        }

        private double Jww(double Pn, double rw, double S, double So)
        {
            double re = 0.14 * Math.Sqrt(Math.Pow(_deltaX, 2) + Math.Pow(_deltaY, 2));
            double Bwn = Bw_n(Pn);
            double krw = Krw(0.2, (1 - So));
            double Jw_n;
            Jw_n = 0.00708 / (_oilVisc * Bwn) * _perm * krw * _height / (Math.Log(re / rw) + S);
            return Jw_n;
        }

	    private double Ce(double So)
	    {
	        double Ce_n = _rockComp + So*_oilComp + (1 - So)*_waterComp;
	        return Ce_n;
	    }

	    private double alphaCalc(double P,double So)
	    {
	        double phi = phi_n(P);
	        double C = Ce(So);
	        double alpha_n = 158*phi*C/_deltaT;
	        return alpha_n;
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

		private double[] RHS_Create(double xw, double yw, double Nx, double Ny, double[] Pn, double rm, double well)
		{
			int well_m = Convert.ToInt32((yw - 1)*Nx + xw - 1);
			int grids = Convert.ToInt32(Nx*Ny);
			double[] RHS = new double[grids];

			for (int i = 0; i < grids; i++)
			{
				RHS[i] = rm*Pn[i];
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

		private double[] GSSolve(double[,] matrix, double[] RHS, double initial, int iterations)
		{
			double[] x = new double[RHS.Length];
			for (int i = 0; i < RHS.Length; i++)
			{
				x[i] = initial;
			}
			double[] xOld = new double[x.Length];

			for (int k = 0; k < iterations; ++k)
			{
				x.CopyTo(xOld, 0);

				for (int i = 0; i < RHS.Length; ++i)
				{
					double entry = RHS[i];
					double diagonal = matrix[i, i];

					for (int j = 0; j < i; j++)
						entry -= matrix[i, j] * x[j];
					for (int j = i + 1; j < RHS.Length; j++)
						entry -= matrix[i, j] * xOld[j];

					x[i] = entry / diagonal;
				}
			}
			return x;
		}

		private void button2_Click(object sender, EventArgs e)
		{
		   Form1 f1 = new Form1(Qw, _deltaT); // Instantiate a Form1 object.
		   f1.Show();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			CreateExcelDoc excellApp = new CreateExcelDoc();
			SaveExcel(excellApp, _P, 0);
			SaveExcel(excellApp, Qw, 1);
			SaveExcel(excellApp, Pwf, 2);
			MessageBox.Show(@"Export to Excel is complete",@"Important Message");
		}

		private void label43_Click(object sender, EventArgs e)
		{

		}

		private void tbWaterSat_TextChanged(object sender, EventArgs e)
		{

		}

		private void label42_Click(object sender, EventArgs e)
		{

		}

		private void label49_Click(object sender, EventArgs e)
		{

		}

		private void label51_Click(object sender, EventArgs e)
		{

		}

		private void groupBox4_Enter(object sender, EventArgs e)
		{

		}

		private void cbWell3Active_CheckedChanged(object sender, EventArgs e)
		{

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
			const double deltat = 10;
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
			CreateExcelDoc excellApp = new CreateExcelDoc();
			SaveExcel(excellApp, P, 0);
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