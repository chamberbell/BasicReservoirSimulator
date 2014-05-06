using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservoirSimulator2D
{
    /*
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
    */
}
