using System;
using System.IO;
using System.Text;

namespace SlarSolver
{
    public static class MatrixHelper
    {
        public static double[,] Copy(double[,] source)
        {
            int rows = source.GetLength(0);
            int cols = source.GetLength(1);

            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = source[i, j];

            return result;
        }

        public static void Print(double[,] matrix, StringBuilder sb)
        {
            using (StringWriter writer = new StringWriter())
            {
                PrintMatrix(matrix, writer, false);
                sb.Append(writer.ToString());
            }
        }

        public static void PrintMatrix(double[,] matrix, TextWriter writer, bool showVariables = false)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1) - 1;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (showVariables)
                    {
                        string coeff = $"{matrix[i, j],8:F2}";
                        string sign = j < cols - 1 ? "+" : "";
                        writer.Write($"{coeff}x{j + 1} {sign} ");
                    }
                    else
                    {
                        writer.Write($"{matrix[i, j],10:F3}");
                    }
                }

                if (showVariables)
                {
                    writer.Write($"= {matrix[i, cols],8:F2}");
                }
                else
                {
                    writer.Write($" | {matrix[i, cols],10:F3}");
                }

                writer.WriteLine();
            }

            writer.WriteLine();
        }
    }
}