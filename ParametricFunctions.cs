using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationInterfaces
{
    class LineFunction : IParametricFunction
    {
        class InternalLineFunction : IFunction
        {
            public IVector param;
            public double Value(IVector point)
            {
                double f = 0;
                for(int i = 0; i < point.Count - 1; i++)
                {
                    f += point[i]*param[i];
                }
                f += param[point.Count - 1];
                return f;
            }
        }
        public IFunction Bind(IVector parameters) => new InternalLineFunction() { param = parameters};

    }

    class Polynomial : IParametricFunction
    {
        class PolynomialFunction : IFunction
        {
            public IVector param;
            public double Value(IVector point)
            {
                double f = 0;
                int pow = point.Count - 1;
                for(int i = 0; i < point.Count - 1; i++)
                {
                    f += Math.Pow(point[i], pow) * param[i];
                }
                f += param[point.Count -1];
                return f;
            }
        }
        public IFunction Bind(IVector parameters)
        {
            return new PolynomialFunction() {param = parameters };
        }
    }
    

}
