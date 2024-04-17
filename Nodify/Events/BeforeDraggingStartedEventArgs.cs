namespace Nodify;

public class BeforeDraggingStartedEventArgs : RoutedEventArgs
{
    public List<ItemContainer>? AdditionalItemsToDrag { get; set; }
}