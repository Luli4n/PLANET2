using System;
using System.Collections.Generic;
using System.Text;

namespace TravelingSalesmanSolver
{
    public class Point
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public Point Clone()
        {
            return new Point { Id = this.Id, X = this.X, Y = this.Y };
        }

        public double Distance(Point point)
        {
            return Math.Sqrt(Math.Pow(this.X - point.X, 2) + Math.Pow(this.Y - point.Y, 2));
        }
    }
}
