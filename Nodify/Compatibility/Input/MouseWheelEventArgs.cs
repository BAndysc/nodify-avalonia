namespace Nodify.Compatibility;

public class MouseWheelEventArgs : MouseEventArgs
{
    private readonly PointerWheelEventArgs? _args;

    public MouseWheelEventArgs()
    {
    }

    public MouseWheelEventArgs(PointerWheelEventArgs args)
    {
        _args = args;
    }

    /// <summary>
    /// Gets the delta in WPF-equivalent units (120 per notch).
    /// </summary>
    public int Delta => _args != null ? (int)(_args.Delta.Y * WpfDeltaScale) : 0;

    public override bool Handled
    {
        get => _args?.Handled ?? false;
        set
        {
            if (_args != null)
                _args.Handled = value;
        }
    }

    public override KeyModifiers KeyModifiers => _args?.KeyModifiers ?? default;

    public override object? Source => _args?.Source;

    public override Point GetPosition(Visual? relativeTo)
    {
        return _args?.GetPosition(relativeTo) ?? default;
    }

    // Avalonia delta is ~0.6 per notch, WPF is 120 per notch
    private const double WpfDeltaScale = 120.0 / 0.6;
}
