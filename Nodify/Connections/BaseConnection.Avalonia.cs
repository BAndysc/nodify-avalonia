namespace Nodify;

public partial class BaseConnection
{
    public static readonly StyledProperty<double> TextPositionFactorProperty
        = AvaloniaProperty.Register<BaseConnection, double>(nameof(TextPositionFactor), 0.5);

    public double TextPositionFactor
    {
        get => GetValue(TextPositionFactorProperty);
        set => SetValue(TextPositionFactorProperty, value);
    }
}