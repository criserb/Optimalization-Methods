using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using org.mariuszgromada.math.mxparser;

namespace Optimalization_Methods.Menu
{
    public enum FunctionChoice
    {
        Michalewicz,
        Rosenbrock,
        Ackley,
        Rastrigin
    }

    class FireflyAlgorithm
    {
        private double trueMin = 0;
        private List<double> values;
        private TextBox textBox;
        private int numFireflies, maxEpochs, dim, seed = 0;
        private double minX, maxX;
        private List<double> points;

        public FireflyAlgorithm(TextBox textBox, int numFireflies, int maxEpochs, double minX, double maxX, List<double> values)
        {
            this.textBox = textBox;
            this.maxEpochs = maxEpochs;
            this.numFireflies = numFireflies;
            this.minX = minX;
            this.maxX = maxX;
            this.values = values;
            this.dim = values.Count;
        }

        public FireflyAlgorithm(TextBox textBox, int numFireflies, int maxEpochs, double minX, double maxX)
        {
            this.textBox = textBox;
            this.maxEpochs = maxEpochs;
            this.numFireflies = numFireflies;
            this.minX = minX;
            this.maxX = maxX;
        }

        void AddToTextBox(string s)
        {
            textBox.Text += s + '\n';
        }

        void AddToTextBoxWithoutNewLine(string s)
        {
            textBox.Text += s;
        }

        void ShowVector(double[] v, int dec, bool nl)
        {
            for (int i = 0; i < v.Length; ++i)
                AddToTextBoxWithoutNewLine(v[i].ToString("F" + dec) + " ");
            if (nl == true)
                AddToTextBox(" ");
            AddToTextBox(" ");
        }

        //public PlotInfo OwnFunction(string equation, string args)
        //{
        //    //this.dim = args.Count;
        //    Function function = new Function(equation);
        //    string f = string.Empty;
        //    for (int i = 0; i < equation.Length; i++)
        //    {
        //        if (equation[i] != ')')
        //            f += equation[i];
        //        else
        //            break;
        //    }

        //    //string argsLine = "";
        //    //for (int i = 0; i < args.Count; i++)
        //    //{
        //    //    argsLine += args[i];
        //    //    if (i + 1 < args.Count)
        //    //        argsLine += ',';
        //    //}

        //    MessageBox.Show("ArgsLine: " + args);

        //    org.mariuszgromada.math.mxparser.Expression e1 =
        //        new org.mariuszgromada.math.mxparser.Expression($"f({args.ToString()})", function);
        //    double trueMin = e1.calculate();

        //    AddToTextBox(trueMin.ToString());

        //    PlotInfo plotInfo = new PlotInfo
        //    {
        //        SeriesIn = new List<double> { 1, 2 },
        //        Error = 1,
        //        Val = 1
        //    };

        //    //ShowDetails(bestPosition, trueMin, z, error);

        //    return plotInfo;
        //}

        //public PlotInfo Demo()
        //{
        //    trueMin = Michalewicz(values.ToArray());

        //    double[] bestPosition = Solve();
        //    double z = Michalewicz(bestPosition);
        //    double error = Error(bestPosition);

        //    PlotInfo plotInfo = new PlotInfo
        //    {
        //        SeriesIn = points,
        //        Error = error,
        //        Val = z
        //    };

        //    ShowDetails(bestPosition, trueMin, z, error);

        //    return plotInfo;
        //}

        private void ShowDetails(double[] bestPosition, double trueMin, double z, double error)
        {
            textBox.Text = String.Empty;
            AddToTextBox($"True min at: {Math.Round(trueMin, 5)}");
            AddToTextBox($"Best solution found:");
            ShowVector(bestPosition, 4, false);
            AddToTextBox($"Value of function at best position: {Math.Round(z, 5)}");
            AddToTextBox($"Error at best position: {Math.Round(error, 5)}");
        }

        private delegate double FunctionDelegate(double[] val);

        public PlotInfo Start(FunctionChoice functionChoice)
        {
            FunctionDelegate myFunction = null;

            switch (functionChoice)
            {
                case FunctionChoice.Michalewicz:
                    myFunction = Michalewicz;
                    break;
                case FunctionChoice.Rosenbrock:
                    myFunction = Rosenbrock;
                    break;
                case FunctionChoice.Ackley:
                    myFunction = Ackley;
                    break;
                case FunctionChoice.Rastrigin:
                    myFunction = Rastrigin;
                    break;
                default:
                    break;
            }


            trueMin = myFunction(values.ToArray());

            double[] bestPosition = Solve(myFunction);
            double z = myFunction(bestPosition);
            double error = Error(bestPosition, myFunction);

            PlotInfo plotInfo = new PlotInfo
            {
                SeriesIn = points,
                Error = error,
                Val = z
            };

            ShowDetails(bestPosition, trueMin, z, error);

            return plotInfo;
        }

        double[] Solve(FunctionDelegate function)
        {
            points = new List<double>();
            Random rnd = new Random(seed);
            double B0 = 1.0;
            double g = 1.0;
            double a = 0.20;
            // int displayInterval = maxEpochs / 10;
            double bestError = double.MaxValue;
            double[] bestPosition = new double[dim]; // Best ever
            Firefly[] swarm = new Firefly[numFireflies]; // All null

            for (int i = 0; i < numFireflies; ++i)
            {
                swarm[i] = new Firefly(dim);
                for (int k = 0; k < dim; ++k) // Random position
                    swarm[i].position[k] = (maxX - minX) * rnd.NextDouble() + minX;
                swarm[i].error = Error(swarm[i].position, function);
                swarm[i].intensity = 1 / (swarm[i].error + 1);
                if (swarm[i].error < bestError)
                {
                    bestError = swarm[i].error;
                    for (int k = 0; k < dim; ++k)
                        bestPosition[k] = swarm[i].position[k];
                }
            } // For each firefly

            int epoch = 0;
            while (epoch < maxEpochs)
            {
                //if (epoch % displayInterval == 0 && epoch < maxEpochs)
                //{
                //    string sEpoch = epoch.ToString().PadLeft(6);
                //    AddToTextBoxWithoutNewLine("epoch = " + sEpoch);
                //    AddToTextBox(" error = " + bestError.ToString("F14"));
                //}
                for (int i = 0; i < numFireflies; ++i) // Each firefly
                {
                    for (int j = 0; j < numFireflies; ++j) // Others
                    {
                        if (swarm[i].intensity < swarm[j].intensity)
                        {
                            // Move firefly(i) toward firefly(j)
                            double r = Distance(swarm[i].position, swarm[j].position);
                            double beta = B0 * Math.Exp(-g * r * r);

                            for (int k = 0; k < dim; ++k)
                            {
                                swarm[i].position[k] += beta *
                                  (swarm[j].position[k] - swarm[i].position[k]);
                                swarm[i].position[k] += a * (rnd.NextDouble() - 0.5);
                                if (swarm[i].position[k] < minX)
                                    swarm[i].position[k] = (maxX - minX) * rnd.NextDouble() + minX;
                                if (swarm[i].position[k] > maxX)
                                    swarm[i].position[k] = (maxX - minX) * rnd.NextDouble() + minX;
                            }

                            swarm[i].error = Error(swarm[i].position, function);
                            swarm[i].intensity = 1 / (swarm[i].error + 1);
                        } // If firefly(i) < firefly(j)
                    } // j
                } // i each firefly
                Array.Sort(swarm); // low error to high
                if (swarm[0].error < bestError)
                {
                    bestError = swarm[0].error;
                    for (int k = 0; k < dim; ++k)
                        bestPosition[k] = swarm[0].position[k];
                }
                points.Add(function(bestPosition));
                ++epoch;
            } // While
            return bestPosition;
        } // Solve



        double Distance(double[] posA, double[] posB)
        {
            double ssd = 0.0; // sum squared diffrences
            for (int i = 0; i < posA.Length; ++i)
                ssd += (posA[i] - posB[i]) * (posA[i] - posB[i]);
            return Math.Sqrt(ssd);
        }


        public double Michalewicz(double[] xValues)
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

        public double Ackley(double[] xValues)
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

        public double Rastrigin(double[] xValues)
        {
            double result = 0.0;
            for (int i = 0; i < xValues.Length; ++i)
            {
                result += (Math.Pow( xValues[i],2) - Math.Cos(18*Math.PI*xValues[i]));
            }return result;
        }

        public double Rosenbrock(double[] xValues)
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



        double Error(double[] xValues, FunctionDelegate function)
        {
            int dim = xValues.Length;
            //double trueMin = 0.0;
            //if (dim == 2)
            //    trueMin = -1.8013; // Approx.
            //else if (dim == 5)
            //    trueMin = -4.687658; // Approx.
            double calculated = function(xValues);
            return (trueMin - calculated) * (trueMin - calculated);
        }

    }

    public class PlotInfo
    {
        public List<double> SeriesIn { get; set; }
        public double Error { get; set; }
        public double Val { get; set; }
        public PlotInfo() { }
        public PlotInfo(List<double> seriesIn, double error, double val)
        {
            SeriesIn = seriesIn;
            Error = error;
            Val = val;
        }
    }

    public class Firefly : IComparable<Firefly>
    {
        public double[] position;
        public double error;
        public double intensity;

        public Firefly(int dim)
        {
            this.position = new double[dim];
            this.error = 0.0;
            this.intensity = 0.0;
        }

        public int CompareTo(Firefly other)
        {
            if (this.error < other.error) return -1;
            else if (this.error > other.error) return +1;
            else return 0;
        }
    } // Class Firefly
} // Algorithm



