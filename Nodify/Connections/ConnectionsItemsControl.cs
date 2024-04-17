using System.Collections;

namespace Nodify;

public class ConnectionsItemsControl : MultiSelector
{
    protected override Type StyleKeyOverride => typeof(ItemsControl);

    public new static readonly DirectProperty<SelectingItemsControl, IList?> SelectedItemsProperty = SelectingItemsControl.SelectedItemsProperty;

    public new static readonly DirectProperty<SelectingItemsControl, ISelectionModel> SelectionProperty = SelectingItemsControl.SelectionProperty;

    /// <inheritdoc />
    protected override DependencyObject GetContainerForItemOverride()
        => new ConnectionContainer(this);

    /// <inheritdoc />
    protected override bool IsItemItsOwnContainerOverride(object item)
        => item is ConnectionContainer;

    public NodifyEditor? Editor { get; private set; }

    /// <inheritdoc />
    public new IList? SelectedItems
    {
        get => base.SelectedItems;
        set => base.SelectedItems = value;
    }

    /// <inheritdoc />
    public new ISelectionModel Selection
    {
        get => base.Selection;
        set => base.Selection = value;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Editor = this.FindAncestorOfType<NodifyEditor>();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Editor = null;
    }
}