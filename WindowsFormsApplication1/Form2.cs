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
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using ILNumerics.Drawing.Controls;
using ILNumerics.Drawing.Driver;
using Microsoft.Office.Interop.Excel;

namespace ReservoirSimulator2D
{
    public partial class Form2 : Form
    {

        private double[,,] stuffToPlot;
        private string thisAxisName;
        private double deltaX;
        private double deltaY;
        private int _time;
        private double minVal;
        private double maxVal;

        public Form2(string axisName,double _minVal ,double _maxVal,double[,] _P, int _timeStepSelected, int _gridX, int _gridY, double _deltaX, double _deltaY)
        {
            InitializeComponent();
            labelTitle.Text = axisName+" at "+_timeStepSelected*10+" days";
            _time = _timeStepSelected;
            thisAxisName = axisName;
            minVal = _minVal;
            maxVal = _maxVal;

            int mBlocks = _P.GetLength(1);

            deltaX = _deltaX;
            deltaY = _deltaY;

            stuffToPlot = new double[_gridY, _gridX, 3];
            int[] Xloc = new int[mBlocks];
            int[] Yloc = new int[mBlocks];

            for (int i = 1; i <= _gridX; i++)
            {
                for (int j = 1; j <= _gridY; j++)
                {
                    int m = (j - 1)*_gridX + i - 1;
                    Xloc[m] = i;
                    Yloc[m] = j;
                }
            }

            for (int i = 0; i < _P.GetLength(1); i++)
            {
                int x = Xloc[i] - 1;
                int y = Yloc[i] - 1;
                stuffToPlot[y, x, 0] = _P[_timeStepSelected, i];
                stuffToPlot[y, x, 1] = (x + 1)*_deltaX;
                stuffToPlot[y, x, 2] = (y + 1)*_deltaY;
            }


        }



        private void ilPanel1_Load(object sender, EventArgs e)
        {
            
            myProgram MyMath = new myProgram();
            ILArray<float> A = MyMath.MyArray(stuffToPlot, deltaX, deltaY);

            //ILArray<float> X = MyMath.MyArray(stuffToPlot, deltaX, deltaY);

            using (ILScope.Enter())
            {
               
               
                ILArray<float> data = A;
                ILScene scene = new ILScene();

                ILPlotCube plotCube = new ILPlotCube(twoDMode: false);

                plotCube.Rotation = Matrix4.Rotation(new Vector3(1f,0.23f,1), 0.7f);

                ILSurface surface = new ILSurface(data);
                surface.Wireframe.Color = Color.FromArgb(50, Color.LightGray);
                surface.Colormap = Colormaps.Jet;
                

                var pcsm = plotCube.ScaleModes;
                pcsm.XAxisScale = AxisScale.Linear;
                pcsm.YAxisScale = AxisScale.Linear;
                pcsm.ZAxisScale = AxisScale.Linear;
                plotCube.Axes.Visible = true;
                plotCube.Axes.ZAxis.Label.Text = thisAxisName;
                //plotCube.Axes.ZAxis.Min = (float) minVal;
                //plotCube.Axes.ZAxis.Max = (float) maxVal;

                surface.DataRange = new Tuple<float, float>((float) minVal, (float) maxVal);
                surface.Wireframe.Visible = false;
                surface.Wireframe.Color = Color.FromArgb(50, Color.LightGray);
                surface.Colormap = Colormaps.Jet;
            
                //surface.UseLighting = true;

                
                //surface.Fill.Visible = false;        
                
                ILColorbar colorbar = new ILColorbar();
                colorbar.Location = new PointF(.99f, 0.4f);
                surface.Add(colorbar);
                
               
                scene.Add(plotCube);
                plotCube.Add(surface);
                
                
                
                

                // contourPlot.Add(legend);
                // legend.Configure(); // only needed in version 3.2.2.0!
                // scene.Configure();
                
                ilPanel1.Scene = scene;
                ilPanel1.Refresh();
            }
        }
    }

    class myProgram : ILNumerics.ILMath
    {

        public ILArray<float> MyArray(double[, ,] stuffToPlot, double deltaX, double deltaY)
        {
            ILArray<float> A = zeros<float>(stuffToPlot.GetLength(0),stuffToPlot.GetLength(1),3);
            for (int i = 0; i < stuffToPlot.GetLength(0); i++)
            {
                for (int j = 0; j < stuffToPlot.GetLength(1); j++)
                {
                    A[i, j,0] = (float) stuffToPlot[i, j, 0];
                    A[i, j, 1] = (float) Convert.ToDouble(j*deltaX+deltaX/2);
                    A[i, j, 2] = (float) Convert.ToDouble(i*deltaY+deltaY/2);
                }
            }
            return A;
        }

        static void myMain(string[] args)
        {
            // create a matrix A, give values explicitely
            ILArray<double> A = array<double>(
                    new double[] { 1, 1, 1, 1, 1, 2, 3, 4, 1, 3, 6, 10, 1, 4, 10, 20 }, 4, 4);
            // use a creation function for B
            ILArray<double> B = counter(4, 2);

            // use a function of the base class: ILMath.linsolve 
            ILArray<double> Result = linsolve(A, B);

            // A.ToString() gives formated output
            Console.Out.WriteLine("A: " + Environment.NewLine + A.ToString());
            Console.Out.WriteLine("B: " + Environment.NewLine + B.ToString());
            Console.Out.WriteLine("A * [Result] = B: " + Environment.NewLine
                                                       + Result.ToString());

            // check result:
            // uses norm, multiply, eps and binary operators 
            if (norm(multiply(A, Result) - B) <= eps)
            {
                Console.Out.WriteLine("Result ok");
            }
            else
            {
                Console.Out.WriteLine("Result false");
            }
            Console.ReadKey();
        }
    }
}
