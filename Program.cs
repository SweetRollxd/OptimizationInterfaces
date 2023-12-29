using System.Runtime.CompilerServices;
using OptimizationInterfaces;

namespace interfaces
{
    class Program
    {
        static void Main(string[] args)
        {
            var gradient_optimizer = new MinimizerGradientDescent ();
            var montecarlo_optimizer = new MinimizerMonteCarlo();
            var gauss_optimizer = new MinimizerGaussNewton();

            List<string> filenames = new List<string>() { "l1_line", "l2_line", "linf_line", "integral_line",
                                                          "l1_polynom", "l2_polynom", "linf_polynom", "integral_polynom" };
            foreach (var filename in filenames) {
                string filepath = "inputs/" + filename + ".txt";
                Console.WriteLine("Загрузка файла {0}:", filepath);
                List<Vector> points = new();
                var initial = new Vector();
                try
                {
                    using (StreamReader sr = new StreamReader(filepath))
                    {
                        int n = int.Parse(sr.ReadLine());
                        for (int i = 0; i < n; i++)
                        {
                            var str = sr.ReadLine().Split();
                            var v = new Vector();
                            for(int j = 0; j < str.Length; j++)
                            {
                                v.Add(Convert.ToDouble(str[j]));
                            }
                        points.Add(v);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка при чтении файла:");
                    Console.WriteLine(e.Message);
                }

                for (int i = 0;i < points[0].Count; i++) { initial.Add(1);}
                var tmp = filename.Split("_");
                string functional_type = tmp[0];
                string function_type = tmp[1];
                var functional = GetFunctional(functional_type, points);
                var fun = GetFunction(function_type);
                try {
                    gradient_optimizer.points = points;
                    gauss_optimizer.points = points;
                    
                    var res = montecarlo_optimizer.Minimize(functional, fun, initial);
                    
                    Console.WriteLine("Оптимизация методом Монте-Карло\n" + PrettyResult(res));
                    res = gradient_optimizer.Minimize(functional, fun, initial);
                    Console.WriteLine("Оптимизация методом градиентного спуска\n" + PrettyResult(res));

                    res = gauss_optimizer.Minimize(functional, fun, initial);
                    Console.WriteLine("Оптимизация методом Гаусса-Ньютона\n" + PrettyResult(res) + "\n");
                }
                catch (UnsupportedFuncationalException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }
        static public string PrettyResult(IVector v)
        {
            string res = "";
            char paramname = 'a';
            for (int i = 0; i < v.Count; i++)
            {
                res += paramname + " = " + Convert.ToString(v[i]) + ' ';
                paramname++;
            }
            return res;
        }

        static public IFunctional GetFunctional(string alias, List<Vector> points){
            switch (alias)
            {
                case "l1":
                    return new L1() { points = points };
                    break;
                case "l2":
                    return new L2() { points = points };
                    break;
                case "linf":
                    return new L1() { points = points };
                    break;
                case "integral":
                    return new L1() { points = points };
                    break;
                default:
                    return default;
            }
        }

        static public IParametricFunction GetFunction(string alias){
            switch (alias)
            {
                case "line":
                    return new LineFunction();
                    break;
                case "polynom":
                    return new Polynomial();
                    break;
                default:
                    return default;
            }
        }
    }
}