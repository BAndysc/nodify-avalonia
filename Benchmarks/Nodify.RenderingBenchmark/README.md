# Connection rendering preparation

This benchmark records drawing commands for 100 and 1,000 connections, with
labels and outlines independently enabled. Each scene is warmed up for 10
redraws, followed by 100 measured redraws. It reports elapsed time, managed
allocations on the calling thread and geometry construction calls.

```sh
DOTNET_TieredCompilation=0 dotnet run -c Release --project Benchmarks/Nodify.RenderingBenchmark
```

The baseline is `af46875` on `avalonia_port`, using the same benchmark source.
Both versions use Avalonia 11.1.0 / Skia and .NET 8.0.30 on macOS arm64.
Tiered compilation was disabled for both. Results below are the median of three
separate processes, alternating baseline and changed builds. Raw runs are in
[results.csv](results.csv).

Results for 1,000 connections and 100 redraws:

| Labels | Outline | Before (ms) | After (ms) | Before (MB allocated) | After (MB allocated) |
| --- | --- | ---: | ---: | ---: | ---: |
| No | No | 61.92 | 63.04 | 95.33 | 95.33 |
| No | Yes | 412.42 | 155.88 | 258.57 | 190.57 |
| Yes | No | 2778.34 | 1591.70 | 954.39 | 617.69 |
| Yes | Yes | 3269.92 | 1817.93 | 1119.27 | 714.57 |

For outlined connections, repeated path construction drops from 100,000 calls
to zero during the measured redraws. Changing endpoints or routing settings
still invalidates the geometry. Changes to text, font, foreground, flow direction
or UI culture invalidate the text cache.

These are CPU drawing-preparation measurements, not GPU frame times or application
FPS. They do not measure panning an entire editor or rasterizing node templates.
The benchmark includes the allocations used to record the drawing commands.

Run the regression tests with:

```sh
dotnet test Tests/Nodify.Tests/Nodify.Tests.csproj -c Release
```
