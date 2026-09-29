using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Xunit;

namespace Nodify.Tests;

public class RenderingCacheTests
{
    [AvaloniaFact]
    public void RedrawingOutlineReusesPathUntilEndpointsChange()
    {
        var connection = new CountingConnection
        {
            Source = new Point(20, 30), Target = new Point(300, 140), OutlineBrush = Brushes.Red
        };
        using var view = new ConnectionView(connection);
        int before = connection.GeometryBuilds;
        connection.DrawOverlay();
        connection.DrawOverlay();
        Assert.Equal(before, connection.GeometryBuilds);

        connection.Target = new Point(320, 180);
        view.Flush();
        connection.DrawOverlay();
        Assert.Equal(before + 1, connection.GeometryBuilds);
    }

    [AvaloniaFact]
    public void MovingConnectionReusesTextButTextAndFontChangesRefreshIt()
    {
        var connection = new CountingConnection { Text = "flow", Source = new Point(20, 30), Target = new Point(300, 140) };
        using var view = new ConnectionView(connection);
        connection.DrawOverlay();
        var original = connection.LastText;
        connection.Target = new Point(350, 180);
        view.Flush();
        connection.DrawOverlay();
        Assert.Same(original, connection.LastText);
        connection.Text = "a longer label";
        connection.DrawOverlay();
        Assert.NotSame(original, connection.LastText);
        Assert.True(connection.LastText!.Width > original!.Width);
        var updated = connection.LastText;
        connection.FontSize *= 2;
        connection.DrawOverlay();
        Assert.True(connection.LastText!.Height > updated!.Height);
    }

    [AvaloniaFact]
    public void TextCacheTracksBrushDirectionAndCulture()
    {
        var connection = new CountingConnection { Text = "flow" };
        using var view = new ConnectionView(connection);
        connection.DrawOverlay();
        var original = connection.LastText;
        connection.Foreground = Brushes.Red;
        connection.DrawOverlay();
        Assert.NotSame(original, connection.LastText);
        original = connection.LastText;
        connection.FlowDirection = FlowDirection.RightToLeft;
        connection.DrawOverlay();
        Assert.NotSame(original, connection.LastText);
        original = connection.LastText;
        var culture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture.Name == "fr-FR" ? "de-DE" : "fr-FR");
            connection.DrawOverlay();
            Assert.NotSame(original, connection.LastText);
        }
        finally
        {
            CultureInfo.CurrentUICulture = culture;
        }
    }

    [AvaloniaFact]
    public void OutlineStyleChangesDoNotRebuildPath()
    {
        var connection = new CountingConnection { Source = new Point(20, 30), Target = new Point(300, 140), OutlineBrush = Brushes.Red };
        using var view = new ConnectionView(connection);
        int before = connection.GeometryBuilds;
        double width = connection.DrawOverlay().Children.OfType<GeometryDrawing>().Single().Pen!.Thickness;
        connection.StrokeThickness = 12;
        connection.OutlineThickness = 4;
        connection.OutlineBrush = Brushes.Green;
        view.Flush();
        var drawing = connection.DrawOverlay().Children.OfType<GeometryDrawing>().Single();
        Assert.True(drawing.Pen!.Thickness > width);
        Assert.Equal(20, drawing.Pen.Thickness);
        Assert.Equal(Brushes.Green, drawing.Pen.Brush);
        Assert.Equal(before, connection.GeometryBuilds);
    }

    [AvaloniaFact]
    public void CircuitAngleInvalidatesCachedGeometry()
    {
        var connection = new TestCircuit { Source = new Point(20, 30), Target = new Point(300, 140) };
        using var view = new ConnectionView(connection);
        var original = connection.Path;
        connection.Angle = 30;
        Assert.NotSame(original, connection.Path);
    }

    [AvaloniaFact]
    public void StepPositionInvalidatesCachedGeometry()
    {
        var connection = new TestStep { Source = new Point(20, 30), Target = new Point(300, 140) };
        using var view = new ConnectionView(connection);
        var original = connection.Path;
        connection.SourcePosition = ConnectorPosition.Top;
        Assert.NotSame(original, connection.Path);
        original = connection.Path;
        connection.TargetPosition = ConnectorPosition.Bottom;
        Assert.NotSame(original, connection.Path);
    }

    private sealed class TestCircuit : CircuitConnection
    {
        protected override Type StyleKeyOverride => typeof(CircuitConnection);
        public Geometry? Path => RenderedGeometry;
    }

    private sealed class TestStep : StepConnection
    {
        protected override Type StyleKeyOverride => typeof(StepConnection);
        public Geometry? Path => RenderedGeometry;
    }

    private sealed class CountingConnection : Connection
    {
        protected override Type StyleKeyOverride => typeof(Connection);
        public int GeometryBuilds { get; private set; }
        public FormattedText? LastText { get; private set; }
        protected override Geometry CreateDefiningGeometry()
        {
            GeometryBuilds++;
            return base.CreateDefiningGeometry();
        }
        protected override Point GetTextPosition(FormattedText text, Point source, Point target)
        {
            LastText = text;
            return base.GetTextPosition(text, source, target);
        }
        public DrawingGroup DrawOverlay()
        {
            var drawing = new DrawingGroup();
            using (var context = drawing.Open())
                Render(context);
            return drawing;
        }
    }
}

internal sealed class ConnectionView : IDisposable
{
    private readonly Window _window;

    public ConnectionView(BaseConnection connection)
    {
        _window = new Window { Width = 800, Height = 600, Content = new Canvas { Children = { connection } } };
        _window.Show();
        Flush();
    }

    public void Flush()
    {
        _window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
    }

    public void Dispose()
    {
        _window.Close();
        Dispatcher.UIThread.RunJobs();
    }
}
