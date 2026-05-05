using System;
using System.Text;

namespace SlarSolver
{
    public class MatrixSolver : ISolver
    {
        public string Name => "Матричний метод";

        public double[] Solve(double[,] m, StringBuilder steps)
        {
            int n = m.GetLength(0);

            double[,] A = new double[n, n];
            double[] b = new double[n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    A[i, j] = m[i, j];

                b[i] = m[i, n];
            }

            steps.AppendLine("Матричний метод");
            steps.AppendLine("Формула методу: X = A^(-1) · B");
            steps.AppendLine();

            steps.AppendLine("Матриця коефіцієнтів A:");
            PrintSquareMatrix(A, steps);

            steps.AppendLine("Вектор вільних членів B:");
            for (int i = 0; i < n; i++)
                steps.AppendLine($"b{i + 1} = {b[i]:F3}");
            steps.AppendLine();

            double[,] inv = Inverse(A, steps);

            steps.AppendLine("Обернена матриця A^(-1):");
            PrintSquareMatrix(inv, steps);

            double[] x = new double[n];

            steps.AppendLine("Множення A^(-1) на B:");

            for (int i = 0; i < n; i++)
            {
                double sum = 0;

                for (int j = 0; j < n; j++)
                {
                    sum += inv[i, j] * b[j];
                    steps.AppendLine($"x{i + 1}: додаємо {inv[i, j]:F3} * {b[j]:F3}");
                }

                x[i] = sum;
                steps.AppendLine($"x{i + 1} = {x[i]:F3}");
                steps.AppendLine();
            }

            return x;
        }

        private double[,] Inverse(double[,] A, StringBuilder steps)
        {
            int n = A.GetLength(0);
            double[,] aug = new double[n, 2 * n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    aug[i, j] = A[i, j];

                aug[i, i + n] = 1;
            }

            steps.AppendLine("Формуємо розширену матрицю [A | E]:");
            PrintMatrix(aug, steps);

            for (int k = 0; k < n; k++)
            {
                int maxRow = k;

                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(aug[i, k]) > Math.Abs(aug[maxRow, k]))
                        maxRow = i;
                }

                if (Math.Abs(aug[maxRow, k]) < 1e-10)
                    throw new InvalidOperationException("Матриця є виродженою. Обернена матриця не існує.");

                if (maxRow != k)
                {
                    SwapRows(aug, k, maxRow);
                    steps.AppendLine($"Переставляємо рядки {k + 1} і {maxRow + 1}:");
                    PrintMatrix(aug, steps);
                }

                double pivot = aug[k, k];

                for (int j = 0; j < 2 * n; j++)
                    aug[k, j] /= pivot;

                steps.AppendLine($"Нормалізуємо рядок {k + 1}, ділимо на {pivot:F3}:");
                PrintMatrix(aug, steps);

                for (int i = 0; i < n; i++)
                {
                    if (i == k) continue;

                    double factor = aug[i, k];

                    for (int j = 0; j < 2 * n; j++)
                        aug[i, j] -= factor * aug[k, j];

                    steps.AppendLine($"Обнулюємо елемент у рядку {i + 1}, стовпці {k + 1}, множник = {factor:F3}:");
                    PrintMatrix(aug, steps);
                }
            }

            double[,] inv = new double[n, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    inv[i, j] = aug[i, j + n];

            return inv;
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

        private void PrintMatrix(double[,] matrix, StringBuilder steps)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    steps.Append($"{matrix[i, j],12:F4}");

                steps.AppendLine();
            }

            steps.AppendLine();
        }

        private void PrintSquareMatrix(double[,] matrix, StringBuilder steps)
        {
            int n = matrix.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    steps.Append($"{matrix[i, j],12:F4}");

                steps.AppendLine();
            }

            steps.AppendLine();
        }
    }
}