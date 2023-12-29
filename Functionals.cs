using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationInterfaces
{
    class L1 : IDifferentiableFunctional
    {
        public List<Vector> points;
        public int n;
        public IVector Gradient(IFunction function)
        {
            var grad = new Vector();
            for (int i = 0; i < n; i++)
            {
                grad.Add(0);
            }
            string type = define(function.GetType().ToString());
            switch (type)
            {
                case "LineFunction":
                    for (int i = 0; i < n - 1; i++)
                    {
                        for (int j = 0; j < points.Count; j++)
                        {
                            grad[i] += function.Value(points[j]);
                            grad[i] *= points[j][i];
                        }
                    }
                    for (int j = 0; j < points.Count; j++)
                    {
                        grad[n - 1] += function.Value(points[j]);
                    }
                    break;

                case "Polynomial":
                    int pow = points.Count - 1;
                    for (int i = 0; i < n - 1; i++)
                    {
                        for (int j = 0; j < points.Count; j++)
                        {
                            grad[i] += function.Value(points[j]);
                            grad[i] *= Math.Pow(points[j][i], pow);
                        }
                        pow--;
                    }
                    for (int j = 0; j < points.Count; j++)
                    {
                        grad[n - 1] += function.Value(points[j]);
                    }
                    break;
            }
            return grad;
        }

        public double Value(IFunction function)
        {
            double sum = 0;
            foreach(var p in points) sum += Math.Abs(function.Value(p)); //STUPID NW!
            return sum;
        }
        string define(string s)
        {
            if (s.IndexOf("LineFunction") > -1)
                return "LineFunction";
            if (s.IndexOf("Polynomial") > -1)
                return "Polynomial";
            return s;
        }

    }
    class L2 : IDifferentiableFunctional, ILeastSquaresFunctional
    {
        public List<Vector> points;
        public int n;
        public IVector Gradient(IFunction function)
        {
            var grad = new Vector();
            for (int i = 0; i < n; i++)
            {
                grad.Add(0);
            }
            string type = define(function.GetType().ToString());
            double sqrt = 0;
            for (int j = 0; j < points.Count; j++)
            {
                sqrt += function.Value(points[j]);
            }
            sqrt = Math.Sqrt(sqrt);
                switch (type)
            {
                case "LineFunction":

                    for (int i = 0; i < n - 1; i++)
                    {
                        for (int j = 0; j < points.Count; j++)
                        {
                            grad[i] += function.Value(points[j]);
                            grad[i] *= points[j][i];
                        }
                        grad[i] /= sqrt;
                       // sqrt = 0;
                    }
                    for (int j = 0; j < points.Count; j++)
                    {
                        grad[n - 1] += function.Value(points[j]);
                    }
                    grad[n - 1] /= sqrt;
                    break;

                case "Polynomial":
                    int pow = points.Count - 1;
                    for (int i = 0; i < n - 1; i++)
                    {
                        for (int j = 0; j < points.Count; j++)
                        {
                            grad[i] += function.Value(points[j]);
                            grad[i] *= Math.Pow(points[j][i], pow);
                        }
                        grad[i] /= sqrt;
                        sqrt = 0;
                        pow--;
                    }
                    for (int j = 0; j < points.Count; j++)
                    {
                        grad[n - 1] += function.Value(points[j]);
                    }
                    grad[n - 1] /= sqrt;
                    break;
            }
            return grad;
        }

        public IMatrix Jacobian(IFunction function)
        {
            var Jlist = new List<List<double>>();
            for(int i =0; i < points.Count; i++)
            {
                var strlist = new List<double>();
                for(int j = 0; j < n;j++)
                {
                    double d = ParamDerivative(points[i], j, function);
                    strlist.Add(d);
                }
                Jlist.Add(strlist);
            }

            var J = new Matrix(points.Count, n) { list = Jlist};
            return J;
        }

        double ParamDerivative(Vector point, int paramnum, IFunction function)
        {
            double der = 0;
            string type = define(function.GetType().ToString());
            switch(type)
            {
                case "LineFunction":
                    der = point[paramnum];
                    break;
                case "Polynomial":
                    double pow = n - 1 - paramnum;
                    der = Math.Pow(point[paramnum], pow);
                    break;
            }
            return der;
        }

        public IVector Residual(IFunction function)
        {
            var residual = new Vector();
            foreach(var p in points)
            {
                double r = p[ n - 1 ];
                r -= function.Value(p);
               // r *= r;
                residual.Add(r);
            }
            return residual;
        }

        public double Value(IFunction function)
        {
            double sum = 0;
            foreach (var p in points)
                sum += Math.Pow(function.Value(p), 2);
            return Math.Sqrt(sum);
        }
        string define(string s)
        {
            if (s.IndexOf("LineFunction") > -1)
                return "LineFunction";
            if (s.IndexOf("Polynomial") > -1)
                return "Polynomial";
            return s;
        }
    }
    class Linf : IFunctional
    {
        public List<Vector> points;
        public double Value(IFunction function)
        {
            double val = function.Value(points[0]);
            double currval;
            foreach(var p in points)
            {
                currval = Math.Abs(function.Value(p));
                if (currval > val) val = currval;
            }
            return val;
        }
    }
    class Integral : IFunctional
    {
        public List<Vector> points;  // x =0, y = 1
        public double Value(IFunction function)
        {
            if(points.Count < 2) return 0;
            double sum = 0;
            for(int i = 1 ; i < points.Count; i++)
            {
                var p1 = new Vector() { points[i - 1][0], points[i - 1][1] };
                var p2 = new Vector() { points[i][0], points[i][1] };
                double x1 = points[i - 1][0];
                double x2 = points[i][0];
                double y1 = function.Value(p1);
                double y2 = function.Value(p2);
                // halfsumm
                double h = halfsum(y1, y2);
                sum += (x2-x1)*h;
            }
            return sum;
        }
        double halfsum(double l, double r) 
        { 
        if(l < 0 && r < 0)
            {
                l *= -1;
                r *= -1;
            }
        if(l > 0 && r < 0)
            {
                r *= -1;
                l += r;
            }
        if(l < 0 && r > 0)
            {
                l *= -1;
                r += l;
            }
            return (l + r) / 2;
        }
    }
}
