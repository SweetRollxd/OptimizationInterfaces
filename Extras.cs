namespace OptimizationInterfaces
{

    public class Vector : List<double>, IVector
    {

    }

    public class Matrix : IMatrix
    {
        public List<List<double>> list;
        public Matrix(int N, int M)
        {
            list = new List<List<double>>();
            for (int i = 0; i < N; i++)
            {
                list.Add(new List<double>());
                for (int j = 0; j < M; j++)
                    list.Last().Add(0);
            }
        }

        public double this[int i, int j] { get => list[i][j]; set => list[i][j] = value; }

        public Matrix TransposedMatrix(int N, int M)
        {
            var translist = new List<List<double>>();
            for (int i = 0; i < M; i++)
            {
                var strlist = new List<double>();
                for (int j = 0; j < N; j++)
                    strlist.Add(list[j][i]);
                translist.Add(strlist);
            }
            var mat = new Matrix(M, N) {list = translist };
            return mat;
        }
    }

    public class GaussNewtonVector
    {
        double[] b;
        double[,] A;
        public int M;
        public int N;
        public Vector r;
        public Matrix Jt;
        Matrix J;

        private void PrepareA()
        {
            J = Jt.TransposedMatrix(M, N);
            A = new double[M, M];
            for (int i = 0; i < M; i++)
                for (int j = 0; j < M; j++)
                    for (int k = 0; k < N; k++)
                        A[i,j] += Jt[i, k] * J[k, j];
        }

        private void Prepareb()
        {
            b = new double[M];
            for (int i = 0; i < M; i++)
                for (int j = 0; j < N; j++)
                b[i] += Jt[i, j]*r[j];
        }
        public double[] GiveVector()
        {
            PrepareA();
            Prepareb();
            double[] ans = new double[b.Length];
            solveSLAE(ans);
            return ans;
        }
        
        private void solveSLAE(double[] ans)
        {
            int nSLAE = b.Length;
            if (ans.Length != nSLAE)
                throw new Exception("Size of the input array is not compatable with size of SLAE");




            for (int i = 0; i < nSLAE; i++)
            {
                double del = A[i, i];
                double absDel = Math.Abs(del);
                int iSwap = i;


                for (int j = i + 1; j < nSLAE; j++) // ищем максимальный элемент по столбцу
                {
                    if (absDel < Math.Abs(A[j, i]))
                    {
                        del = A[j, i];
                        absDel = Math.Abs(del);
                        iSwap = j;
                    }
                }

                if (iSwap != i)
                {
                    double buf;
                    for (int j = i; j < nSLAE; j++)
                    {
                        buf = A[i, j];
                        A[i, j] = A[iSwap, j];
                        A[iSwap, j] = buf;
                    }
                    buf = b[i];
                    b[i] = b[iSwap];
                    b[iSwap] = buf;
                }

                for (int j = i; j < nSLAE; j++)
                    A[i, j] /= del;

                b[i] /= del;

                for (int j = i + 1; j < nSLAE; j++)
                {
                    if (A[j, i] == 0) continue;

                    double el = A[j, i];
                    for (int k = i; k < nSLAE; k++)
                    {
                        A[j, k] -= A[i, k] * el;
                    }

                    b[j] -= b[i] * el;
                }
            }

            for (int i = nSLAE - 1; i > -1; i--)
            {
                for (int j = i + 1; j < nSLAE; j++)
                    b[i] -= ans[j] * A[i, j];
                ans[i] = b[i];
            }
        }
    }
}
