using System;
using System.Text;

namespace SlarSolver
{
    public class GaussianSolver : ISolver
    {
        public string Name => "Класичний метод Гауса";

        public double[] Solve(double[,] m, StringBuilder steps)
        {
            int n = m.GetLength(0);
            double[,] a = MatrixHelper.Copy(m);

            steps.AppendLine("Класичний метод Гауса");
            steps.AppendLine("Початкова розширена матриця:");
            MatrixHelper.Print(a, steps);

            for (int k = 0; k < n - 1; k++)
            {
                int maxRow = k;

                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(a[i, k]) > Math.Abs(a[maxRow, k]))
                        maxRow = i;
                }

                if (Math.Abs(a[maxRow, k]) < 1e-10)
                    throw new InvalidOperationException("Система не має єдиного розв’язку або має безліч розв’язків.");

                if (maxRow != k)
                {
                    SwapRows(a, k, maxRow);
                    steps.AppendLine($"Переставляємо рядки {k + 1} і {maxRow + 1}:");
                    MatrixHelper.Print(a, steps);
                }

                for (int i = k + 1; i < n; i++)
                {
                    double factor = a[i, k] / a[k, k];

                    for (int j = k; j <= n; j++)
                        a[i, j] -= factor * a[k, j];

                    steps.AppendLine($"Обнулюємо елемент a[{i + 1},{k + 1}], множник = {factor:F3}:");
                    MatrixHelper.Print(a, steps);
                }
            }

            double[] x = new double[n];

            steps.AppendLine("Зворотний хід:");

            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;

                for (int j = i + 1; j < n; j++)
                    sum += a[i, j] * x[j];

                if (Math.Abs(a[i, i]) < 1e-10)
                    throw new InvalidOperationException("Система не має єдиного розв’язку.");

                x[i] = (a[i, n] - sum) / a[i, i];

                steps.AppendLine($"x{i + 1} = ({a[i, n]:F3} - {sum:F3}) / {a[i, i]:F3} = {x[i]:F3}");
            }

            return x;
        }

        private void SwapRows(double[,] matrix, int row1, int row2)
        {
            int cols = matrix.GetLength(1);

            for (int j = 0; j < cols; j++)
            {
                double temp = matrix[row1, j];
                matrix[row1, j] = matrix[row2, j];
                matrix[row2, j] = temp;
            }
        }
    }
}