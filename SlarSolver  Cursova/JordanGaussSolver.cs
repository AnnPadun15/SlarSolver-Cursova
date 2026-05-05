using System;
using System.Text;

namespace SlarSolver
{
    public class JordanGaussSolver : ISolver
    {
        public string Name => "Метод Жордана-Гауса";

        public double[] Solve(double[,] m, StringBuilder steps)
        {
            int n = m.GetLength(0);
            double[,] a = MatrixHelper.Copy(m);

            steps.AppendLine("Метод Жордана-Гауса");
            steps.AppendLine("Початкова розширена матриця:");
            MatrixHelper.Print(a, steps);

            for (int k = 0; k < n; k++)
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

                double pivot = a[k, k];

                for (int j = 0; j <= n; j++)
                    a[k, j] /= pivot;

                steps.AppendLine($"Нормалізуємо рядок {k + 1}, ділимо всі елементи на {pivot:F3}:");
                MatrixHelper.Print(a, steps);

                for (int i = 0; i < n; i++)
                {
                    if (i == k) continue;

                    double factor = a[i, k];

                    for (int j = 0; j <= n; j++)
                        a[i, j] -= factor * a[k, j];

                    steps.AppendLine($"Обнулюємо елемент a[{i + 1},{k + 1}], множник = {factor:F3}:");
                    MatrixHelper.Print(a, steps);
                }
            }

            double[] x = new double[n];

            steps.AppendLine("Після перетворень отримано одиничну матрицю зліва.");
            steps.AppendLine("Розв’язок системи:");

            for (int i = 0; i < n; i++)
            {
                x[i] = a[i, n];
                steps.AppendLine($"x{i + 1} = {x[i]:F3}");
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