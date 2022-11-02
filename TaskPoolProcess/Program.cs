using TravelingSalesmanSolver;

//Point[] points = PointsLoader.LoadPoints(@"C:\Users\julek\Downloads\wi29.tsp");
//var result = PointExtension.DistanceSum(points);

//PMX pmx = new PMX(points);

//pmx.RunRound();


//ThreeOpt threeOpt = new ThreeOpt(pmx.BestFirst, pmx.BestSecond);
//threeOpt.RunRound();

//Console.WriteLine(10000);


Point[] points1 = PointsLoader.LoadPoints(@"D:\file.tsp");
Point[] points2 = PointsLoader.LoadPoints(@"D:\file2.tsp");

PMX pmx1 = new PMX(points1);
pmx1.subSequenceStart = 3;
pmx1.subSequenceEnd = 7;

Point[] child = new Point[points1.Length];

for(int i=3;i<7;i++)
{
    child[i] = points1[i].Clone();
}
pmx1.SecondStep(points1, points2, child);