using System;
using System.Collections.Generic;
using System.Text;

namespace TravelingSalesmanSolver
{
    public class GraphPoint
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public GraphPoint Clone()
        {
            return new GraphPoint { Id = this.Id, X = this.X, Y = this.Y };
        }

        public double Distance(GraphPoint point)
        {
            return Math.Sqrt(Math.Pow(this.X - point.X, 2) + Math.Pow(this.Y - point.Y, 2));
        }
    }
}
