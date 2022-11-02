using System;
using System.Collections.Generic;
using System.Text;

namespace TravelingSalesmanSolver
{
    public class ThreeOpt
    {

        Point[] FirstDefault;
        Point[] SecondDefault;

        public Point[] BestSolution { get; set; }
        public double BestDistance { get; set; }
        
        public ThreeOpt(Point[] FirstDefault, Point[] SecondDefault)
        {
            this.FirstDefault = FirstDefault;
            this.SecondDefault = SecondDefault;
            BestSolution = FirstDefault;
            BestDistance = PointExtension.DistanceSum(FirstDefault);
        }

        public void RunRound()
        {
            for (int i = 0; i < FirstDefault.Count() - 2; i++)
            {
                for (int j = i + 1; j < FirstDefault.Count() - 1; j++)
                {
                    ThreeOptSwap(FirstDefault, i, j, FirstDefault.Count());
                    ThreeOptSwap(SecondDefault, i, j, FirstDefault.Count());
                }
            }
        }

        private void ThreeOptSwap(Point[] points, int i, int j, int k)
        {
            List<Point> deepCopy = points.Select(p => p.Clone()).ToList();

            List<Point> a = deepCopy.GetRange(0, i);
            List<Point> b = deepCopy.GetRange(i, j - i);
            b.Reverse();
            List<Point> c = deepCopy.GetRange(j, k-j);
        
            Point[] firstResult = a.Concat(c).Concat(b).ToArray();
            var firstResultDistance = PointExtension.DistanceSum(firstResult);
            if(firstResultDistance < BestDistance)
            {
                BestDistance = firstResultDistance;
                BestSolution = firstResult.Select(p => p.Clone()).ToArray();
            }
            c.Reverse();
            Point[] secondResult = a.Concat(b).Concat(c).ToArray();
            var secondResultDistance = PointExtension.DistanceSum(secondResult);
            if (secondResultDistance < BestDistance)
            {
                BestDistance = secondResultDistance;
                BestSolution = secondResult.Select(p => p.Clone()).ToArray();
            }
        }
            

    }

    
}
