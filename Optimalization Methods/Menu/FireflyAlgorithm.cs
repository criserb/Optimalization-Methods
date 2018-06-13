using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveCharts.Defaults;
using System.Threading;

namespace Optimalization_Methods.Menu
{
    public class FireflyAlgorithm
    {
        private double trueMin = 0;
        private List<double> values;
        private int numFireflies, maxEpochs, dim, seed = 0;
        private double minX, maxX;
        private Queue<ObservablePoint> pointsToPlot;



        public FireflyAlgorithm(int numFireflies, int maxEpochs, double minX, double maxX, List<double> values)
        {
            this.maxEpochs = maxEpochs;
            this.numFireflies = numFireflies;
            this.minX = minX;
            this.maxX = maxX;
            this.values = values;
            this.dim = values.Count;
        }

        public PlotInfo Start(FunctionDelegate functionDelegate)
        {
            FunctionDelegate myFunction = functionDelegate;

            trueMin = myFunction(values.ToArray(), dim);

            double[] bestPosition = Solve(myFunction);
            double z = myFunction(bestPosition, dim);
            double error = Error(bestPosition, myFunction);

            PlotInfo plotInfo = new PlotInfo
            {
                SeriesIn = pointsToPlot,
                Error = error,
                Val = z,
                TrueMin = trueMin,
                BestPostion = bestPosition
            };

            // ShowDetails(bestPosition, trueMin, z, error);

            return plotInfo;
        }

        private double[] Solve(FunctionDelegate function)
        {
            pointsToPlot = new Queue<ObservablePoint>();
            Random rnd = new Random(seed);
            double B0 = 1.0;
            double g = 1.0;
            double a = 0.20;
            double bestError = double.MaxValue;
            double[] bestPosition = new double[dim]; // Best ever
            Firefly[] swarm = new Firefly[numFireflies]; // All null
            int step = maxEpochs / 10;

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
                // add points to plot list
                if ((epoch +1) % step == 0)
                    pointsToPlot.Enqueue(new ObservablePoint(epoch + 1, function(bestPosition, dim)));
                ++epoch;
            } // While            
            return bestPosition;
        } // Solve



        double Distance(double[] posA, double[] posB)
        {
            double ssd = 0.0; // sum squaOrangeRed diffrences
            for (int i = 0; i < posA.Length; ++i)
                ssd += (posA[i] - posB[i]) * (posA[i] - posB[i]);
            return Math.Sqrt(ssd);
        }

        double Error(double[] xValues, FunctionDelegate function)
        {
            double calculated = function(xValues, dim);
            return (trueMin - calculated) * (trueMin - calculated);
        }

    }

    public class PlotInfo
    {
        public Queue<ObservablePoint> SeriesIn { get; set; }
        public double Error { get; set; }
        public double Val { get; set; }
        public double TrueMin { get; set; }
        public double[] BestPostion { get; set; }
        public PlotInfo() { }
        public PlotInfo(Queue<ObservablePoint> seriesIn, double error, double val, double trueMin, double[] bestPostion)
        {
            SeriesIn = seriesIn;
            Error = error;
            Val = val;
            TrueMin = trueMin;
            BestPostion = bestPostion;
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



