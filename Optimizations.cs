namespace OptimizationInterfaces
{
    class MinimizerGradientDescent : IOptimizator
    {
        public List<Vector> points;
        double threshold = 0.001;
        double rate = 0.0001;
        double difference = 1;
        public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters,
            IVector minimumParameters = null, IVector maximumParameters = null)
        {
            var prevparam = new Vector();
            var param = new Vector();
            foreach (var p in initialParameters) param.Add(p);
            foreach (var p in initialParameters) prevparam.Add(p);

            var obj = CreateFunctional(points, param.Count, objective);

            while (difference > threshold)
            {
                var fun = function.Bind(param);
                var diff = obj.Gradient(fun);
                for (int i = 0; i < param.Count; i++)
                {
                    param[i] -= rate*diff[i];
                }
                difference = 0;
                for(int i = 0; i < param.Count;i++) { difference += Math.Abs(Math.Abs(prevparam[i]) - Math.Abs(param[i])); }

                prevparam = param;

            }
            return param;
        }

        IDifferentiableFunctional CreateFunctional(List<Vector> points, int n, IFunctional objective)
        {
            string type = define(objective.GetType().ToString());
            switch (type)
            {
                case "L1":
                    return new L1 { n = n, points = points };
                    break;
                case "L2":
                    return new L2 { n = n, points = points };
                    break;
                default:
                    throw new UnsupportedFuncationalException(String.Format("Функционал {0} не поддерживается методом градиентного спуска", type));
            }
            return null;
        }
        string define(string s)
        {
            if (s.IndexOf("L1") > -1)
                return "L1";
            if (s.IndexOf("L2") > -1)
                return "L2";
            if (s.IndexOf("Linf") > -1)
                return "Linf";
            if (s.IndexOf("Integral") > -1)
                return "Integral";
            return s;
        }
    }

    class MinimizerGaussNewton : IOptimizator
    {
        // Do with respect to a & b, like I did for Gradient method
        public List<Vector> points;
        public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector minimumParameters = null, IVector maximumParameters = null)
        {
            double difference = 1;
            double threshold = 0.001;
            

            var l = new List<int>();
            l.Add(0);
            var prevparam = new Vector();
            var param = new Vector();
            foreach (var p in initialParameters) param.Add(p);
            foreach (var p in initialParameters) prevparam.Add(p);
            var obj = new L2 { points = points, n = param.Count };
            
            while(difference > threshold)
            {
                var fun = function.Bind(param);
                var residual = obj.Residual(fun);
                var res = new Vector();
                for (int i = 0; i < residual.Count; i++) { res.Add(residual[i]); }
                var J = obj.Jacobian(fun);
                var Jt = J.TransposedMatrix(points.Count, param.Count);
                var GNvect = new GaussNewtonVector() { Jt = Jt, M = param.Count, N = points.Count, r = res };
                var Jres = GNvect.GiveVector();

                for (int i = 0; i < param.Count; i++)
                    param[i] -= Jres[i];

                difference = 0;
                for (int i = 0; i < param.Count; i++) { difference += Math.Abs(Math.Abs(prevparam[i]) - Math.Abs(param[i])); }
                prevparam = param;
            }

            return param;
        }
    }

    class MinimizerMonteCarlo : IOptimizator
    {
        public int MaxIter = 100000;
        public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector minimumParameters = null, IVector maximumParameters = null)
        {
            var param = new Vector();
            var minparam = new Vector();
            foreach (var p in initialParameters) param.Add(p);
            foreach (var p in initialParameters) minparam.Add(p);
            var fun = function.Bind(param);
            var currentmin = objective.Value(fun);
            var rand = new Random(0);
            for (int i = 0; i < MaxIter; i++)
            {
                for (int j = 0; j < param.Count; j++) param[j] = rand.NextDouble();
                var f = objective.Value(function.Bind(param));
                if (f < currentmin)
                {
                    currentmin = f;
                    for (int j = 0; j < param.Count; j++) minparam[j] = param[j];
                }
            }
            return minparam;
        }
    }
}


 