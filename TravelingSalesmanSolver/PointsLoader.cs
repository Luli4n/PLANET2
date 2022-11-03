using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;

namespace TravelingSalesmanSolver
{
    public static class PointsLoader
    {
        public static GraphPoint[] LoadPoints(string filePath)
        {
            string[] lines = System.IO.File.ReadAllLines(filePath);

            GraphPoint[] points = new GraphPoint[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] line = lines[i].Split(' ');
                points[i] = new GraphPoint { Id = int.Parse(line[0]), X = double.Parse(line[1], CultureInfo.InvariantCulture), Y = double.Parse(line[2], CultureInfo.InvariantCulture) };
            }
            return points;
        }

    }
}
