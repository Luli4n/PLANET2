using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace TravelingSalesmanSolver
{
    public class ThreeOpt
    {


        public GraphPoint[] BestSolution { get; set; }
        public double BestDistance { get; set; }

        private StreamWriter Writer { get; set; }

        public ThreeOpt(StreamWriter writer, GraphPoint[] points)
        {

            this.Writer = writer;
            BestSolution = points;
            BestDistance = PointExtension.DistanceSum(points);
        }


        public GraphPoint[] RunRound(CancellationToken token)
        {
            double currentDistance = BestDistance;


            for (int i = 1; i < BestSolution.Count() - 2; i++)
            {
                for (int j = i + 1; j < BestSolution.Count() - 1; j++)
                {
                    for (int k = j + 1; k < BestSolution.Count() - 1; k++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return BestSolution;
                        }

                        var newSolution = BestSolution.Clone() as GraphPoint[];
                        Array.Reverse(newSolution, i, j - i);
                        Array.Reverse(newSolution, j + 1, k - j);

                        double newDistance = PointExtension.DistanceSum(newSolution);
                        if (newDistance < BestDistance)
                        {
                            BestSolution = newSolution;
                            BestDistance = newDistance;
                            WriteToUI(BestSolution);
                        }
                    }
                }
            }
            return BestSolution;
        }

        private void WriteToUI(GraphPoint[] points)
        {
            double distance = PointExtension.DistanceSum(points);
            bool write = false;
            lock (typeof(GlobalBestSolution))
            {
                if (distance < GlobalBestSolution.BestDistance)
                {
                    GlobalBestSolution.Best = points;
                    GlobalBestSolution.BestDistance = distance;
                    write = true;
                }
            }
            if (write)
            {
                lock (typeof(StreamWriter))
                {
                    Writer.WriteLine(JsonConvert.SerializeObject(points));
                    Writer.Flush();
                }
            }
        }
    }


}
