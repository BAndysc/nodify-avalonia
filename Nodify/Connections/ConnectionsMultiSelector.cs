using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Nodify
{
    internal partial class ConnectionsMultiSelector : MultiSelector
    {
        public new static readonly StyledProperty<IList> SelectedItemsProperty = NodifyEditor.SelectedItemsProperty.AddOwner<ConnectionsMultiSelector>();
        public static readonly StyledProperty<bool> CanSelectMultipleItemsProperty = NodifyEditor.CanSelectMultipleItemsProperty.AddOwner<ConnectionsMultiSelector>(new StyledPropertyMetadata<bool>(true, coerce: CoerceCanSelectMultipleItems));

        private static void OnCanSelectMultipleItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((ConnectionsMultiSelector)d).CanSelectMultipleItemsBase = (bool)e.NewValue;

        private static bool CoerceCanSelectMultipleItems(DependencyObject d, bool baseValue)
            => ((ConnectionsMultiSelector)d).CanSelectMultipleItemsBase = (bool)baseValue;

        private static void OnSelectedItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((ConnectionsMultiSelector)d).OnSelectedItemsSourceChanged((IList)e.OldValue, (IList)e.NewValue);

        /// <summary>
        /// Gets or sets the selected connections in the <see cref="NodifyEditor"/>.
        /// </summary>
        public new IList? SelectedItems
        {
            get => (IList?)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// Gets or sets whether multiple connections can be selected.
        /// </summary>
        public new bool CanSelectMultipleItems
        {
            get => (bool)GetValue(CanSelectMultipleItemsProperty);
            set => SetValue(CanSelectMultipleItemsProperty, value);
        }

        private bool CanSelectMultipleItemsBase
        {
            get => base.CanSelectMultipleItems;
            set => base.CanSelectMultipleItems = value;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ConnectionContainer(this);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
            => item is ConnectionContainer;

        private void OnSelectedItemsSourceChanged(IList oldValue, IList newValue)
        {
            if (oldValue is INotifyCollectionChanged oc)
            {
                oc.CollectionChanged -= OnSelectedItemsChanged;
            }

            if (newValue is INotifyCollectionChanged nc)
            {
                nc.CollectionChanged += OnSelectedItemsChanged;
            }

            IList selectedItems = base.SelectedItems;

            BeginUpdateSelectedItems();
            selectedItems.Clear();
            if (newValue != null)
            {
                for (var i = 0; i < newValue.Count; i++)
                {
                    selectedItems.Add(newValue[i]);
                }
            }
            EndUpdateSelectedItems();
        }

        private void OnSelectedItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (!CanSelectMultipleItems)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    base.SelectedItems.Clear();
                    break;

                case NotifyCollectionChangedAction.Add:
                    IList? newItems = e.NewItems;
                    if (newItems != null)
                    {
                        IList selectedItems = base.SelectedItems;
                        for (var i = 0; i < newItems.Count; i++)
                        {
                            selectedItems.Add(newItems[i]);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    IList? oldItems = e.OldItems;
                    if (oldItems != null)
                    {
                        IList selectedItems = base.SelectedItems;
                        for (var i = 0; i < oldItems.Count; i++)
                        {
                            selectedItems.Remove(oldItems[i]);
                        }
                    }
                    break;
            }
        }

        protected override void OnSelectionChanged(SelectionModelSelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            IList? selected = SelectedItems;
            if (selected != null)
            {
                var added = e.SelectedItems;
                for (var i = 0; i < added.Count; i++)
                {
                    // Ensure no duplicates are added
                    if (!selected.Contains(added[i]))
                    {
                        selected.Add(added[i]);
                    }
                }

                var removed = e.DeselectedItems;
                for (var i = 0; i < removed.Count; i++)
                {
                    selected.Remove(removed[i]);
                }
            }
        }
    }
}
