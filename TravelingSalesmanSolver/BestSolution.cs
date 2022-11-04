using System;
using System.Collections.Generic;
using System.Text;

namespace TravelingSalesmanSolver
{
    public static class GlobalBestSolution
    {
        public static GraphPoint[] Best { get; set; }
        public static double BestDistance { get; set; }
    }
}
