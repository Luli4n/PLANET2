using System;
using System.Collections.Generic;
using System.Text;

namespace TravelingSalesmanSolver
{
    public class PMX
    {
        private Point[] Points;
        
        private Point[] FirstParent;
        private Point[] SecondParent;
        private Point[] FirstChild;
        private Point[] SecondChild;

        public int subSequenceStart;
        public int subSequenceEnd;

        public Point[] BestFirst { get; set; }
        private double BestFirstDistance;
        public Point[] BestSecond { get; set; }
        private double BestSecondDistance;

        public bool IsFinished { get; private set; } = false;
        public PMX(Point[] points)
        {
            Points = points.Select(p=>p.Clone()).ToArray();
        }

        public void RunRound()
        {

            for (int i = 0; i < 1000000; i++)
            {
                InitializeParents();
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
            
            FirstChild = new Point[Points.Count()];
            SecondChild = new Point[Points.Count()];
            
            for(int i=subSequenceStart; i<subSequenceEnd; i++)
            {
                FirstChild[i] = FirstParent[i].Clone();
                SecondChild[i] = SecondParent[i].Clone();
            }
        }

        public void SecondStep(Point[] firstParent, Point[] secondParent, Point[] child)
        {
            for (int i = subSequenceStart; i < subSequenceEnd; i++)
            {
                var index = Array.IndexOf(firstParent, firstParent.Where(f => f.Id == secondParent[i].Id).First());
                if (index >= subSequenceStart && index < subSequenceEnd)
                {
                    continue;
                }
                
                Point relocationPoint = secondParent[i].Clone();
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
            
            if (BestFirst is null || firstChildDistance < BestFirstDistance)
            {
                BestFirst = FirstChild.Select(c => c.Clone()).ToArray();
                BestFirstDistance = firstChildDistance;
            }
            
            if(BestSecond is null || secondChildDistance < BestSecondDistance)
            {
                BestSecond = SecondChild.Select(c => c.Clone()).ToArray();
                BestSecondDistance = secondChildDistance;
            }
        }
        
    }
}
