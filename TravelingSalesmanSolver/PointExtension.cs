﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TravelingSalesmanSolver
{
    public static class PointExtension
    {

        public static double DistanceSum(Point[] points)
        {
            
            double sum = 0;
            for (int i = 0; i < points.Count() - 1; i++)
            {
                sum += points[i].Distance(points[i + 1]);
            }
            sum += points[points.Count() - 1].Distance(points[0]);
            return sum;
            
        }
    }
}
