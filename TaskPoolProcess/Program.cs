using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading.Tasks;
using TravelingSalesmanSolver;


void StartServer(CancellationTokenSource c1, CancellationTokenSource c2)
{
    Task.Factory.StartNew(() =>
    {
        var server = new NamedPipeServerStream("PipeOfCancelation");
        server.WaitForConnection();
        StreamReader reader = new StreamReader(server);
        StreamWriter writer = new StreamWriter(server);
        
        var line = reader.ReadLine();
        if (line == "Exit")
        {
            c1.Cancel();
            c2.Cancel();
        }
            
        writer.Flush();
        
        server.Close();
    });
}


string pointsPath = args[0];
int taskCount;
int.TryParse(args[1], out taskCount);

int firstPhaseSeconds, secondPhaseSeconds;
int.TryParse(args[2], out firstPhaseSeconds);
int.TryParse(args[3], out secondPhaseSeconds);



var client = new NamedPipeClientStream("PipeOfCalculations");
client.Connect();
StreamReader reader = new StreamReader(client);
StreamWriter writer = new StreamWriter(client);


GraphPoint[] points = PointsLoader.LoadPoints(pointsPath);

GlobalBestSolution.Best = points;
GlobalBestSolution.BestDistance = PointExtension.DistanceSum(points);


CancellationTokenSource source = new CancellationTokenSource();
CancellationTokenSource source1 = new CancellationTokenSource();
CancellationToken token = source.Token;

StartServer(source, source1);

source.CancelAfter(firstPhaseSeconds*1000);
TaskFactory factory = new TaskFactory(token);
List<PMX> pmxList = new List<PMX>();
List<Task> pmxTasks = new List<Task>();

for (int i = 0; i < taskCount; i++)
{
    PMX pmx = new PMX(writer, points.Select(p => p.Clone()).ToArray());
    pmxList.Add(pmx);
    var task = factory.StartNew(() =>
    {
        pmx.RunRound(token);
    },token);

    pmxTasks.Add(task);
}
try
{
    Task.WaitAll(pmxTasks.ToArray());
}
catch (Exception ex)
{
    Console.WriteLine("Canceled");
}


if (!pmxList.Where(p => p.Best != null).Any())
{
    return;
}

double median;
if (pmxList.Where(p => p.Best != null).Count() % 2 == 0)
{
    median = pmxList.Where(p => p.Best != null).Select(t => PointExtension.DistanceSum(t.Best)).OrderBy(x => x).Skip((pmxList.Where(p => p.Best != null).Count() / 2) - 1).Take(2).Average();
}
else
{
    median = pmxList.Where(p => p.Best != null).Select(t => PointExtension.DistanceSum(t.Best)).OrderBy(x => x).ElementAt(pmxList.Where(p => p.Best != null).Count() / 2);
}

var nextTourCandidates = pmxList.Where(p => p.Best != null).Where(t => PointExtension.DistanceSum(t.Best) <= median).Select(t => t.Best).ToArray();



List<ThreeOpt> threeOptList = new List<ThreeOpt>();
List<Task> threeOptTasks = new List<Task>();

CancellationToken token1 = source1.Token;
TaskFactory factory1 = new TaskFactory(token);
source1.CancelAfter(secondPhaseSeconds*1000);

for (int i = 0; i < nextTourCandidates.Count(); i++)
{

    ThreeOpt threeOpt = new ThreeOpt(writer, nextTourCandidates[i].Select(p => p.Clone()).ToArray());
    threeOptList.Add(threeOpt);
    var task = factory1.StartNew(() =>
    {
        threeOpt.RunRound(token1);
    },token1);

    threeOptTasks.Add(task);
}

try
{
    Task.WaitAll(threeOptTasks.ToArray());
}
catch (Exception ex)
{
    Console.WriteLine("Canceled");
}

