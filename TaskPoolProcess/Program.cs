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

GlobalBestSolution.Best = points;
GlobalBestSolution.BestDistance = PointExtension.DistanceSum(points);

List<Task<GraphPoint[]>> pmxTasks = new List<Task<GraphPoint[]>>();

for (int i = 0; i < taskCount; i++)
{
    var task = new Task<GraphPoint[]>(() =>
    {
        PMX pmx = new PMX(writer ,points.Select(p => p.Clone()).ToArray());
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

var nextTourCandidates = pmxTasks.Where(t => PointExtension.DistanceSum(t.Result) <= median).Select(t => t.Result).ToArray();



List<Task<GraphPoint[]>> threeOptTasks = new List<Task<GraphPoint[]>>();

for (int i = 0; i < nextTourCandidates.Count(); i++)
{
    int j = i;
    var task = new Task<GraphPoint[]>(() =>
    {

        ThreeOpt threeOpt = new ThreeOpt(writer ,nextTourCandidates[j].Select(p => p.Clone()).ToArray());
        return threeOpt.RunRound();
    });

    threeOptTasks.Add(task);
    task.Start();
}

Task.WaitAll(threeOptTasks.ToArray());
