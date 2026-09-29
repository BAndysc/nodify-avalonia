using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.VisualTree;
using Nodify;

AppBuilder.Configure<Application>().UseSkia().WithInterFont()
    .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
    .SetupWithoutStarting();

Console.WriteLine("connections,labels,outlines,frames,elapsed_ms,allocated_bytes,geometry_builds");
foreach (int count in new[] { 100, 1000 })
foreach (bool labels in new[] { false, true })
foreach (bool outlines in new[] { false, true })
{
    var connections = Enumerable.Range(0, count).Select(i => new RenderProbe
    {
        Source = new Point(20, 20 + i % 10 * 25),
        Target = new Point(400, 40 + i % 10 * 25),
        Stroke = Brushes.DodgerBlue,
        Fill = Brushes.DodgerBlue,
        StrokeThickness = 3,
        OutlineBrush = outlines ? Brushes.White : null,
        Text = labels ? $"Connection {i}" : "",
        FontFamily = FontFamily.Default,
        FontSize = 12
    }).ToArray();

    foreach (var connection in connections)
    {
        connection.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        connection.Arrange(new Rect(0, 0, 800, 600));
        connection.Prepare();
    }

    for (int frame = 0; frame < 10; frame++)
        Draw(connections);

    GC.Collect();
    GC.WaitForPendingFinalizers();
    long allocated = GC.GetAllocatedBytesForCurrentThread();
    int builds = connections.Sum(x => x.GeometryBuilds);
    var stopwatch = Stopwatch.StartNew();
    const int frames = 100;
    for (int frame = 0; frame < frames; frame++)
        Draw(connections);
    stopwatch.Stop();
    allocated = GC.GetAllocatedBytesForCurrentThread() - allocated;
    builds = connections.Sum(x => x.GeometryBuilds) - builds;
    Console.WriteLine(FormattableString.Invariant($"{count},{labels},{outlines},{frames},{stopwatch.Elapsed.TotalMilliseconds:F2},{allocated},{builds}"));
}

static void Draw(RenderProbe[] connections)
{
    var drawing = new DrawingGroup();
    using var context = drawing.Open();
    foreach (var connection in connections)
        connection.Draw(context);
}

sealed class RenderProbe : Connection
{
    private Shape? _separateShape;
    public int GeometryBuilds { get; private set; }

    public void Prepare() => _separateShape = this.GetVisualChildren().OfType<Shape>().SingleOrDefault();

    protected override Geometry CreateDefiningGeometry()
    {
        GeometryBuilds++;
        return base.CreateDefiningGeometry();
    }

    public void Draw(DrawingContext context)
    {
        _separateShape?.Render(context);
        Render(context);
    }
}
