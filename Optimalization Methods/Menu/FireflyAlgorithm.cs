using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Optimalization_Methods.Menu
{
    class FireflyAlgorithm
    {
        static private double trueMin = 0;

        static private TextBox _textBox;
        static void AddToTextBox(string s)
        {
            _textBox.Text += s + '\n';
        }

        static void AddToTextBoxWithoutNewLine(string s)
        {
            _textBox.Text += s;
        }

        static void ShowVector(double[] v, int dec, bool nl)
        {
            for (int i = 0; i < v.Length; ++i)
                AddToTextBoxWithoutNewLine(v[i].ToString("F" + dec) + " ");
            if (nl == true)
                AddToTextBox(" ");
            AddToTextBox(" ");
        }

        public static void Start(TextBox textBox)
        {
            _textBox = textBox;

            AddToTextBox("Begin firefly demo");
            AddToTextBox("Goal is to solve the Michalewicz benchmark function");
            AddToTextBox("The function has a known minimum value of -4.687658");
            AddToTextBox("x = 2.2029 1.5707 1.2850 1.9231 1.7205");

            //double[] p;// = { 2.2029, 1.5707, 1.2850, 1.9231, 1.7205 };
            List<double> lp = new List<double>
            {
                2.2029,
                1.5707,
                1.2850,
                1.9231,
                1.7205
            };
            trueMin = Michalewicz(lp.ToArray());

            int numFireflies = 40;
            int dim = 5;
            int maxEpochs = 1000;
            int seed = 0;

            AddToTextBox("Setting numFireflies = " + numFireflies);
            AddToTextBox("Setting problem dim = " + dim);
            AddToTextBox("Setting maxEpochs = " + maxEpochs);
            AddToTextBox("Setting initialization seed = " + seed);

            AddToTextBox("Starting firefly algorithm");
            double[] bestPosition = Solve(numFireflies, dim, seed, maxEpochs);
            AddToTextBox("Finished");

            AddToTextBox("Best solution found: ");
            AddToTextBoxWithoutNewLine("x = ");
            ShowVector(bestPosition, 4, false);
            double z = Michalewicz(bestPosition);
            AddToTextBoxWithoutNewLine("Value of function at best position = ");
            AddToTextBox(z.ToString("F6"));
            double error = Error(bestPosition);
            AddToTextBoxWithoutNewLine("Error at best position = ");
            AddToTextBox(error.ToString("F4"));

            AddToTextBox("End firefly demo");
            //Console.ReadLine();
        }

        static double[] Solve(int numFireflies, int dim,
          int seed, int maxEpochs)
        {
            Random rnd = new Random(seed);
            double minX = 0.0;
            double maxX = 3.2;
            double B0 = 1.0;
            double g = 1.0;
            double a = 0.20;
            int displayInterval = maxEpochs / 10;
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
                if (epoch % displayInterval == 0 && epoch < maxEpochs)
                {
                    string sEpoch = epoch.ToString().PadLeft(6);
                    AddToTextBoxWithoutNewLine("epoch = " + sEpoch);
                    AddToTextBox(" error = " + bestError.ToString("F14"));
                }
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
                ++epoch;
            } // While
            return bestPosition;
        } // Solve



        static double Distance(double[] posA, double[] posB)
        {
            double ssd = 0.0; // sum squared diffrences
            for (int i = 0; i < posA.Length; ++i)
                ssd += (posA[i] - posB[i]) * (posA[i] - posB[i]);
            return Math.Sqrt(ssd);
        }


        static double Michalewicz(double[] xValues)
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


        static double Error(double[] xValues)
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



