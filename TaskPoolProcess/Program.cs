using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO.Pipes;
using TravelingSalesmanSolver;





string pointsPath = args[0];

int taskCount;
int.TryParse(args[1], out taskCount);



var client = new NamedPipeClientStream("PipeOfCalculations");
client.Connect();
StreamReader reader = new StreamReader(client);
StreamWriter writer = new StreamWriter(client);

GraphPoint[] points = PointsLoader.LoadPoints(pointsPath);


List<Task<GraphPoint[]>> pmxTasks = new List<Task<GraphPoint[]>>();

for (int i = 0; i < taskCount; i++)
{
    var task = new Task<GraphPoint[]>(() =>
    {
        PMX pmx = new PMX(points.Select(p => p.Clone()).ToArray());
        return pmx.RunRound();
    });

    pmxTasks.Add(task);
    task.Start();
}


Task.WaitAll(pmxTasks.ToArray());

double median;
if (taskCount % 2 == 0)
{
    median = pmxTasks.Select(t => PointExtension.DistanceSum(t.Result)).OrderBy(x => x).Skip((taskCount / 2) - 1).Take(2).Average();
}
else
{
    median = pmxTasks.Select(t => PointExtension.DistanceSum(t.Result)).OrderBy(x => x).ElementAt(taskCount / 2);
}

var nextTourCandidates = pmxTasks.Where(t => PointExtension.DistanceSum(t.Result) < median).Select(t => t.Result).ToArray();


//ThreeOpt threeOpt = new ThreeOpt(pmx.BestFirst);
//threeOpt.RunRound();Console.WriteLine(10000);


//Point[] points1 = PointsLoader.LoadPoints(@"D:\file.tsp");
//Point[] points2 = PointsLoader.LoadPoints(@"D:\file2.tsp");

//PMX pmx1 = new PMX(points1);
//pmx1.subSequenceStart = 3;
//pmx1.subSequenceEnd = 7;

//Point[] child = new Point[points1.Length];

//for(int i=3;i<7;i++)
//{
//    child[i] = points1[i].Clone();
//}
//pmx1.SecondStep(points1, points2, child);


///////////////////
List<Task<GraphPoint[]>> threeOptTasks = new List<Task<GraphPoint[]>>();

for (int i = 0; i < nextTourCandidates.Count()-1; i++)
{
    var task = new Task<GraphPoint[]>(() =>
    {
        ThreeOpt threeOpt = new ThreeOpt(nextTourCandidates[i].Select(p => p.Clone()).ToArray());
        return threeOpt.RunRound();
    });

    threeOptTasks.Add(task);
    task.Start();
}

Task.WaitAll(threeOptTasks.ToArray());

var results = threeOptTasks.Select(t => t.Result).ToArray();

Console.WriteLine();

writer.WriteLine(JsonConvert.SerializeObject(results[0]));
writer.Flush();
Console.WriteLine(reader.ReadLine());

Console.ReadLine();