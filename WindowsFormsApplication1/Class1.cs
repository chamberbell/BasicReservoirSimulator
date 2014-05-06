using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class GaussSeidel
    {
        public static double[] Solve (double[,] matrix, double[] right,
                              double relaxation, int iterations, 
                              double lo[], double hi[])
        {
            // Validation omitted
            var x = right;
            double delta;

            // Gauss-Seidel with Successive OverRelaxation Solver
            for (int k = 0; k < iterations; ++k) {
                for (int i = 0; i < right.Length; ++i) {
                    delta = 0.0f;

                    for (int j = 0; j < i; ++j)
                        delta += matrix [i, j] * x [j];
                    for (int j = i + 1; j < right.Length; ++j)
                        delta += matrix [i, j] * x [j];

                    delta = (right [i] - delta) / matrix [i, i];
                    x [i] += relaxation * (delta - x [i]);
            // Project the solution within the lower and higher limits
                    if (x[i]<lo[i])
                    {
                        x[i]=lo[i];
                    }
                    if (x[i]>hi[i])
                        x[i]=hi[i];
                }
            }
            return x;
        }
    }
}
