namespace Nodify;

public delegate void PendingConnectionConnectorOverrideEventHandler(object? sender, PendingConnectionConnectorOverrideEventArgs e);

public class PendingConnectionConnectorOverrideEventArgs : PendingConnectionEventArgs
{
    public FrameworkElement? PotentialConnector { get; set; }

    public PendingConnectionConnectorOverrideEventArgs(PendingConnectionEventArgs pendingConnectionEventArgs) :
        base(pendingConnectionEventArgs.SourceConnector, pendingConnectionEventArgs.MouseEventArgs)
    {
    }

    public PendingConnectionConnectorOverrideEventArgs(Connector source, MouseButtonEventArgs? e) : base(source, e)
    {
    }
}