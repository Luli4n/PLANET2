using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
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


        public GraphPoint[] RunRound()
        {
            double currentDistance = BestDistance;


            for (int i = 0; i < BestSolution.Count() - 2; i++)
            {
                for (int j = i + 1; j < BestSolution.Count() - 1; j++)
                {
                    for (int k = j + 1; k < BestSolution.Count(); k++)
                    {
                        if (i == 0 && k == BestSolution.Count() - 1)
                            continue;

                        var newSolution = BestSolution.Clone() as GraphPoint[];
                        Array.Reverse(newSolution, i, j - i);
                        Array.Reverse(newSolution, j + 1, k - j);
                        if (i == 0)
                            Array.Reverse(newSolution, k, BestSolution.Count() - 1 - k);

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
