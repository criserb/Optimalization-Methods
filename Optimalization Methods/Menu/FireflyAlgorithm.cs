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

        public PlotInfo OwnFunction(string equation, string args)
        {
            //this.dim = args.Count;
            Function function = new Function(equation);
            string f = string.Empty;
            for (int i = 0; i < equation.Length; i++)
            {
                if (equation[i] != ')')
                    f += equation[i];
                else
                    break;
            }

            //string argsLine = "";
            //for (int i = 0; i < args.Count; i++)
            //{
            //    argsLine += args[i];
            //    if (i + 1 < args.Count)
            //        argsLine += ',';
            //}

            MessageBox.Show("ArgsLine: " + args);

            org.mariuszgromada.math.mxparser.Expression e1 =
                new org.mariuszgromada.math.mxparser.Expression($"f({args.ToString()})", function);
            double trueMin = e1.calculate();

            AddToTextBox(trueMin.ToString());

            PlotInfo plotInfo = new PlotInfo
            {
                SeriesIn = new List<double> { 1, 2 },
                Error = 1,
                Val = 1
            };

            //ShowDetails(bestPosition, trueMin, z, error);

            return plotInfo;
        }

        public PlotInfo Demo()
        {
            trueMin = Michalewicz(values.ToArray());

            double[] bestPosition = Solve();
            double z = Michalewicz(bestPosition);
            double error = Error(bestPosition);

            PlotInfo plotInfo = new PlotInfo
            {
                SeriesIn = points,
                Error = error,
                Val = z
            };

            ShowDetails(bestPosition, trueMin, z, error);

            return plotInfo;
        }

        private void ShowDetails(double[] bestPosition, double trueMin, double z, double error)
        {
            textBox.Text = String.Empty;
            AddToTextBox($"True min at: {Math.Round(trueMin, 5)}");
            AddToTextBox($"Best solution found:");
            ShowVector(bestPosition, 4, false);
            AddToTextBox($"Value of function at best position: {Math.Round(z, 5)}");
            AddToTextBox($"Error at best position: {Math.Round(error, 5)}");
        }

        public void Start(TextBox textBox)
        {
            //AddToTextBox("Begin firefly demo");
            //AddToTextBox("Goal is to solve the Michalewicz benchmark function");
            //AddToTextBox("The function has a known minimum value of -4.687658");
            //AddToTextBox("x = 2.2029 1.5707 1.2850 1.9231 1.7205");


            //AddToTextBox("Setting numFireflies = " + numFireflies);
            //AddToTextBox("Setting problem dim = " + dim);
            //AddToTextBox("Setting maxEpochs = " + maxEpochs);
            //AddToTextBox("Setting initialization seed = " + seed);

            //AddToTextBox("Starting firefly algorithm");
            //double[] bestPosition = Solve(numFireflies, dim, seed, maxEpochs);
            //AddToTextBox("Finished");

            //AddToTextBox("Best solution found: ");
            //AddToTextBoxWithoutNewLine("x = ");
            //ShowVector(bestPosition, 4, false);
            //double z = Michalewicz(bestPosition);
            //AddToTextBoxWithoutNewLine("Value of function at best position = ");
            //AddToTextBox(z.ToString("F6"));
            //double error = Error(bestPosition);
            //AddToTextBoxWithoutNewLine("Error at best position = ");
            //AddToTextBox(error.ToString("F4"));

            //AddToTextBox("End firefly demo");
            ////Console.ReadLine();
        }

        double[] Solve()
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
                swarm[i].error = Error(swarm[i].position);
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

                            swarm[i].error = Error(swarm[i].position);
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
                points.Add(Michalewicz(bestPosition));
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


        double Error(double[] xValues)
        {
            int dim = xValues.Length;
            //double trueMin = 0.0;
            //if (dim == 2)
            //    trueMin = -1.8013; // Approx.
            //else if (dim == 5)
            //    trueMin = -4.687658; // Approx.
            double calculated = Michalewicz(xValues);
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



