using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nodify.Calculator
{
    public partial class EditorView : UserControl
    {
        private static readonly DataFormat<OperationInfoViewModel> _operationFormat =
            DataFormat.CreateInProcessFormat<OperationInfoViewModel>(typeof(OperationInfoViewModel).FullName!);

        public EditorView()
        {
            InitializeComponent();

            PointerPressedEvent.AddClassHandler<NodifyEditor>(CloseOperationsMenuPointerPressed);
            ItemContainer.DragStartedEvent.AddClassHandler<ItemContainer>(CloseOperationsMenu);
            PointerReleasedEvent.AddClassHandler<NodifyEditor>(OpenOperationsMenu);
            Editor.AddHandler(DragDrop.DropEvent, OnDropNode);
        }
        
        private void OpenOperationsMenu(object? sender, PointerReleasedEventArgs e)
        {
            if (!e.Handled && e.Source is NodifyEditor editor && !editor.IsPanning && editor.DataContext is CalculatorViewModel calculator &&
                e.InitialPressMouseButton == MouseButton.Right)
            {
                e.Handled = true;
                calculator.OperationsMenu.OpenAt(editor.MouseLocation);
            }
        }

        private void CloseOperationsMenuPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
                CloseOperationsMenu(sender, e);
        }
        
        private void CloseOperationsMenu(object? sender, RoutedEventArgs e)
        {
            ItemContainer? itemContainer = sender as ItemContainer;
            NodifyEditor? editor = sender as NodifyEditor ?? itemContainer?.Editor;

            if (!e.Handled && editor?.DataContext is CalculatorViewModel calculator)
            {
                calculator.OperationsMenu.Close();
            }
        }

        private void OnDropNode(object? sender, DragEventArgs e)
        {
            NodifyEditor? editor = (e.Source as NodifyEditor) ?? (e.Source as Control)?.GetLogicalParent() as NodifyEditor;
            OperationInfoViewModel? operation = null;
            foreach (var item in e.DataTransfer.Items)
            {
                if (item.TryGetRaw(_operationFormat) is OperationInfoViewModel op)
                {
                    operation = op;
                    break;
                }
            }
            if (editor != null && editor.DataContext is CalculatorViewModel calculator && operation != null)
            {
                OperationViewModel op = OperationFactory.GetOperation(operation);
                op.Location = editor.GetLocationInsideEditor(e);
                calculator.Operations.Add(op);

                e.Handled = true;
            }
        }
        
        private async void OnNodeDrag(object? sender, MouseEventArgs e)
        {
            if (leftButtonPressed && _lastPointerPressed != null && ((Control)sender).DataContext is OperationInfoViewModel operation)
            {
                leftButtonPressed = false;
                var item = new DataTransferItem();
                item.Set(_operationFormat, operation);
                var data = new DataTransfer();
                data.Add(item);
                await DragDrop.DoDragDropAsync(_lastPointerPressed, data, DragDropEffects.Copy);
            }
        }

        private void OnNodePressed(object? sender, PointerPressedEventArgs e)
        {
            leftButtonPressed = e.GetCurrentPoint(this).Properties.PointerUpdateKind ==
                                PointerUpdateKind.LeftButtonPressed;
            if (leftButtonPressed)
                _lastPointerPressed = e;
        }

        private void OnNodeExited(object? sender, PointerEventArgs e)
        {
            leftButtonPressed = false;
        }
        
        private bool leftButtonPressed;
        private PointerPressedEventArgs? _lastPointerPressed;
    }
}
