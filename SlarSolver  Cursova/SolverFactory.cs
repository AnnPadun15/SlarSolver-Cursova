namespace SlarSolver
{
    public static class SolverFactory
    {
        public static ISolver GetSolver(int index)
        {
            switch (index)
            {
                case 0: return new GaussianSolver();
                case 1: return new JordanGaussSolver();
                case 2: return new MatrixSolver();
                default: return new GaussianSolver();
            }
        }
    }
}