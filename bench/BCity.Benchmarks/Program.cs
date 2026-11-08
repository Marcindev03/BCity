using System.Diagnostics;
using System.Text.Json;
using BCity.Simulation.Traffic;

// Headless performance gate. Exit code 1 = budget exceeded.
// Budgets are product requirements (docs/MVP_VISION.md, section 8). Do not raise them to make a change pass.
const int Vehicles = 5_000;
const int WarmupTicks = 500;
const int MeasuredTicks = 4_000;
const double BudgetMeanTickMs = 1.0;
const double BudgetP99TickMs = 3.0;

var sim = new RingRoadSimulation(rings: 20, vehiclesPerRing: Vehicles / 20, innerRadius: 400f, ringSpacing: 6f, seed: 7);

for (int i = 0; i < WarmupTicks; i++)
{
    sim.Step();
}

var samples = new double[MeasuredTicks];
long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
int gcBefore = GC.CollectionCount(0);
for (int i = 0; i < MeasuredTicks; i++)
{
    long start = Stopwatch.GetTimestamp();
    sim.Step();
    samples[i] = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
}

long allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
int gcCount = GC.CollectionCount(0) - gcBefore;
Array.Sort(samples);
double mean = samples.Average();
double p99 = samples[(int)(MeasuredTicks * 0.99)];

bool pass = mean <= BudgetMeanTickMs && p99 <= BudgetP99TickMs && allocated == 0;
var result = new
{
    benchmark = "ring_road_traffic",
    vehicles = sim.VehicleCount,
    ticks = MeasuredTicks,
    meanTickMs = Math.Round(mean, 4),
    p99TickMs = Math.Round(p99, 4),
    budgetMeanTickMs = BudgetMeanTickMs,
    budgetP99TickMs = BudgetP99TickMs,
    allocatedBytes = allocated,
    gen0Collections = gcCount,
    pass,
};

Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
return pass ? 0 : 1;
