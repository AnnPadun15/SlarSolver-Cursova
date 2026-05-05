using System.Text;

namespace SlarSolver
{
    public interface ISolver
    {
        string Name { get; }
        double[] Solve(double[,] matrix, StringBuilder steps);
    }
}