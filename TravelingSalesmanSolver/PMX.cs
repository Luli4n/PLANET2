using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace TravelingSalesmanSolver
{
    public class PMX
    {
        private GraphPoint[] Points;
        
        private GraphPoint[] FirstParent;
        private GraphPoint[] SecondParent;
        private GraphPoint[] FirstChild;
        private GraphPoint[] SecondChild;

        public int subSequenceStart;
        public int subSequenceEnd;

        public GraphPoint[] Best { get; set; }
        private double BestDistance;
        private StreamWriter Writer;


        public bool IsFinished { get; private set; } = false;
        public PMX(StreamWriter writer, GraphPoint[] points)
        {
            Points = points.Select(p=>p.Clone()).ToArray();
            this.Writer = writer;
            InitializeParents();
        }

        public GraphPoint[] RunRound(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return Best;
                }
                InitializeChildren(Points.Count() / 2);
                SecondStep(FirstParent, SecondParent, FirstChild);
                SecondStep(SecondParent, FirstParent, SecondChild);
                SetBestSolution();
                FirstParent = FirstChild;
                SecondParent = SecondChild;
            }

        }
                    

        private void InitializeParents()
        {
            Random rng = new Random();
            FirstParent = Points.Select(p => p.Clone()).OrderBy(p => rng.Next()).ToArray();
            SecondParent = Points.Select(p => p.Clone()).OrderBy(p => rng.Next()).ToArray();
        }

        private void InitializeChildren(int subSequenceLength)
        {
            Random rng = new Random();
            subSequenceStart = rng.Next(0, Points.Count() - subSequenceLength);
            subSequenceEnd = subSequenceStart + subSequenceLength;
            
            FirstChild = new GraphPoint[Points.Count()];
            SecondChild = new GraphPoint[Points.Count()];
            
            for(int i=subSequenceStart; i<subSequenceEnd; i++)
            {
                FirstChild[i] = FirstParent[i].Clone();
                SecondChild[i] = SecondParent[i].Clone();
            }
        }

        private void SecondStep(GraphPoint[] firstParent, GraphPoint[] secondParent, GraphPoint[] child)
        {
            for (int i = subSequenceStart; i < subSequenceEnd; i++)
            {
                var index = Array.IndexOf(firstParent, firstParent.Where(f => f.Id == secondParent[i].Id).First());
                if (index >= subSequenceStart && index < subSequenceEnd)
                {
                    continue;
                }
                
                GraphPoint relocationPoint = secondParent[i].Clone();
                int firstParentId = firstParent[i].Id;
                while (true)
                {
                    int copyIndex = Array.IndexOf(secondParent, secondParent.Where(p => p.Id == firstParentId).First());
                    if (copyIndex < subSequenceStart || copyIndex >= subSequenceEnd)
                    {
                        child[copyIndex] = relocationPoint;
                        break;
                    }
                    firstParentId = firstParent[copyIndex].Id;

                }
            }

            for (int i = 0; i < Points.Count(); i++)
            {
                if (child[i] == null)
                    child[i] = secondParent[i].Clone();
            }
        }

        private void SetBestSolution()
        {
            var firstChildDistance = PointExtension.DistanceSum(FirstChild);
            var secondChildDistance = PointExtension.DistanceSum(SecondChild);
            
            if (Best is null || firstChildDistance < BestDistance)
            {
                Best = FirstChild.Select(c => c.Clone()).ToArray();
                BestDistance = firstChildDistance;
                WriteToUI(Best);
            }

            if (Best is null || secondChildDistance < BestDistance)
            {
                Best = SecondChild.Select(c => c.Clone()).ToArray();
                BestDistance = secondChildDistance;
                WriteToUI(Best);
            }
            
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
