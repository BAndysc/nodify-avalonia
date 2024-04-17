using Avalonia;
using Avalonia.Controls;

namespace Nodify
{
    public partial class PendingConnection
    {
        private System.IDisposable? editorDisposableStarted;
        private System.IDisposable? editorDisposableDrag;
        private System.IDisposable? editorDisposableCompleted;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            Editor = this.GetParentOfType<NodifyEditor>();

            if (Editor != null)
            {
                editorDisposableStarted = Editor.AddDisposableHandler(Connector.PendingConnectionStartedEvent, OnPendingConnectionStarted);
                editorDisposableDrag = Editor.AddDisposableHandler(Connector.PendingConnectionDragEvent, OnPendingConnectionDrag);
                editorDisposableCompleted = Editor.AddDisposableHandler(Connector.PendingConnectionCompletedEvent, OnPendingConnectionCompleted);
                SetAllowOnlyConnectorsAttached(Editor, AllowOnlyConnectors);
            }

            UpdateVisibility(IsVisible);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            Editor = null;
            editorDisposableStarted?.Dispose();
            editorDisposableDrag?.Dispose();
            editorDisposableCompleted?.Dispose();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == IsVisibleProperty)
            {
                if (change.NewValue is bool isVisible)
                {
                    UpdateVisibility(isVisible);
                }
            }
        }

        private void UpdateVisibility(bool isVisible)
        {
            if (isVisible)
            {
                SetCurrentValue(Visual.IsVisibleProperty, true);
                Opacity = 1;
            }
            else
            {
                // hacky: we need to wait for the attaching to the visual tree before we set visible to false otherwise
                // the template will not be applied
                if (Editor == null)
                {
                    Opacity = 0;
                    return;
                }

                SetCurrentValue(Visual.IsVisibleProperty, false);
            }
        }
    }
}