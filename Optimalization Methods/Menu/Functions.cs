using System;

namespace Optimalization_Methods.Menu
{
    public enum FunctionChoice
    {
        Michalewicz,
        Rosenbrock,
        Ackley,
        Rastrigin,
        Sphere,
        Levy,
        Griewank
    }

    public delegate double FunctionDelegate(double[] val, int dim);

    static class Functions
    {
        public static double Michalewicz(double[] xValues, int dim)
        {
            double result = 0.0;
            for (int i = 0; i < xValues.Length; ++i)
            {
                double a = Math.Sin(xValues[i]);
                double b = Math.Sin(((i + 1) * xValues[i] * xValues[i]) / Math.PI);
                double c = Math.Pow(b, 20);
                result += a * c;
            }
            return -1.0 * result;
        }

        public static double Sphere(double[] xValues, int dim)
        {
            double result = 0.0;
            for (int i = 0; i < xValues.Length; ++i)
            {
                result += Math.Pow(xValues[i], 2);
            }
            return result;
        }

        public static double Ackley(double[] xValues, int dim)
        {
            double result = 0.0, a = 0, b = 0, c, d;
            for (int i = 0; i < xValues.Length; ++i)
            {
                a += Math.Pow(xValues[i], 2);
            }
            for (int i = 0; i < xValues.Length; ++i)
            {
                b += Math.Cos(2 * Math.PI * xValues[i]);
            }
            c = -20 * Math.Exp(0.2 * Math.Sqrt((1 / dim) * a));
            d = -Math.Exp((1 / dim) * b) + 20 + Math.E;
            result = c + d;
            return result;
        }

        public static double Rastrigin(double[] xValues, int dim)
        {
            double result = 0.0;
            double A = 10.0;
            for (int i = 0; i < xValues.Length; ++i)
            {
                result += (Math.Pow(xValues[i], 2) - (A * Math.Cos(2 * Math.PI * xValues[i])));
            }
            return A * dim + result;
        }

        public static double Rosenbrock(double[] xValues, int dim)
        {
            double result = 0.0;
            for (int i = 0; i < xValues.Length - 1; ++i)
            {
                double a = (1 - xValues[i]) * (1 - xValues[i]);
                double b = 100 * Math.Pow((xValues[i + 1] - Math.Pow(xValues[i], 2)), 2);
                result += a + b;
            }
            return result;
        }

        public static double Levy(double[] xValues, int dim)
        {
            double result = 0.0;
            double w = 0.0, b = 0.0, c = 0.0;
            double wd = 1 + ((xValues[xValues.Length - 1] - 1) / 4);
            double w1 = 1 + ((xValues[0] - 1) / 4);
            for (int i = 0; i < xValues.Length - 1; ++i)
            {
                w = 1 + ((xValues[i] - 1) / 4);
                b = Math.Pow(w - 1, 2) * (1 + 10 * Math.Pow(Math.Sin(Math.PI * w + 1), 2));
                c = Math.Pow(wd - 1, 2) * (1 + Math.Pow(Math.Sin(2 * Math.PI * wd), 2));
                result += (b + c);
            }
            return Math.Pow(Math.Sin(Math.PI * w1), 2) + result;
        }

        public static double Griewank(double[] xValues, int dim)
        {
            double result = 0.0;
            double a = 0.0, b = 1.0;
            for (int i = 0; i < xValues.Length; ++i)
            {
                a += Math.Pow(xValues[i], 2) / 4000;
                b *= Math.Cos(xValues[i] / Math.Sqrt(i)) + 1;
            }
            result = a - b;
            return result;
        }

        public static double Schwefel(double[] xValues, int dim)
        {
            double a = 0.0, b = 0.0, result = 0.0;
            for (int i = 0; i < xValues.Length; ++i)
            {
                a += (xValues[i] * Math.Sin(Math.Sqrt(Math.Abs(xValues[i]))));
            }
            result = 418.9829 * dim - a;
            return result;
        }
    }
}
