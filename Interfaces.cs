using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationInterfaces
{
    interface IOptimizator

    {
            IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters,
                             IVector minimumParameters = default, IVector maximumParameters = default);
            
    }
    interface IParametricFunction
    {
        IFunction Bind(IVector parameters);
    }
    interface IDifferentiableFunction : IFunction
    {
        // По параметрам исходной IParametricFunction
        IVector Gradient(IVector point);
    }
    interface IFunctional
    {
        double Value(IFunction function);
    }
    interface IDifferentiableFunctional : IFunctional
    {
        IVector Gradient(IFunction function);
    }
    interface IMatrix
    {
        double this[int i, int j] { get; set; }
        public Matrix TransposedMatrix(int N, int M);

    }
    interface ILeastSquaresFunctional : IFunctional
    {
        IVector Residual(IFunction function);
        IMatrix Jacobian(IFunction function);
    }
    interface IFunction
    {
        double Value(IVector point);
    }
    interface IVector : IList<double> { }
}
