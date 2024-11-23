using System.Diagnostics;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Nodify
{
    [StyleTypedProperty(Property = nameof(PushedAreaStyle), StyleTargetType = typeof(Rectangle))]
    public partial class NodifyEditor
    {
        public static readonly StyledProperty<ControlTheme> PushedAreaStyleProperty = AvaloniaProperty.Register<NodifyEditor, ControlTheme>(nameof(PushedAreaStyle));

        public static readonly DirectProperty<NodifyEditor, Rect> PushedAreaProperty = AvaloniaProperty.RegisterDirect<NodifyEditor, Rect>(nameof(PushedArea), x => x.PushedArea);

        public static readonly DirectProperty<NodifyEditor, bool> IsPushingItemsProperty = AvaloniaProperty.RegisterDirect<NodifyEditor, bool>(nameof(IsPushingItems), x => x.IsPushingItems);

        public static readonly DirectProperty<NodifyEditor, Orientation> PushedAreaOrientationProperty = AvaloniaProperty.RegisterDirect<NodifyEditor, Orientation>(nameof(PushedAreaOrientation), x => x.PushedAreaOrientation);

        private static void OnIsPushingItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (NodifyEditor)d;

            if ((bool)e.NewValue == true)
            {
                editor.OnItemsPushStarted();
            }
            else
            {
                editor.OnItemsPushCompleted();
            }
        }

        private void OnItemsPushCompleted()
        {
            if (ItemsDragCompletedCommand?.CanExecute(DataContext) ?? false)
                ItemsDragCompletedCommand.Execute(DataContext);
        }

        private void OnItemsPushStarted()
        {
            if (ItemsDragStartedCommand?.CanExecute(DataContext) ?? false)
                ItemsDragStartedCommand.Execute(DataContext);
        }

        private Rect pushedArea;
        /// <summary>
        /// Gets the currently pushed area while <see cref="IsPushingItems"/> is true.
        /// </summary>
        public Rect PushedArea
        {
            get => pushedArea;
            private set => SetAndRaise(PushedAreaProperty, ref pushedArea, value);
        }

        private bool isPushingItems;
        /// <summary>
        /// Gets a value that indicates whether a pushing operation is in progress.
        /// </summary>
        public bool IsPushingItems
        {
            get => isPushingItems;
            private set
            {
                if (SetAndRaise(IsPushingItemsProperty, ref isPushingItems, value))
                {
                    if (value)
                        OnItemsPushStarted();
                    else
                        OnItemsPushCompleted();
                }
            }
        }

        private Orientation pushedAreaOrientation;
        /// <summary>
        /// Gets the orientation of the <see cref="PushedArea"/>.
        /// </summary>
        public Orientation PushedAreaOrientation
        {
            get => pushedAreaOrientation;
            private set => SetAndRaise(PushedAreaOrientationProperty, ref pushedAreaOrientation, value);
        }

        /// <summary>
        /// Gets or sets the style to use for the pushed area.
        /// </summary>
        public ControlTheme PushedAreaStyle
        {
            get => GetValue(PushedAreaStyleProperty);
            set => SetValue(PushedAreaStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets whether push items cancellation is allowed.
        /// </summary>
        public static bool AllowPushItemsCancellation { get; set; } = true;

        private IPushStrategy? _pushStrategy;

        protected internal void StartPushingItems(Point position, Orientation orientation)
        {
            IsPushingItems = true;
            PushedAreaOrientation = orientation;

            _pushStrategy = CreatePushStrategy(orientation);

            PushedArea = _pushStrategy.Start(position);
        }

        protected internal void CancelPushingItems()
        {
            if (!AllowPushItemsCancellation)
                throw new InvalidOperationException("Push items cancellation is not allowed");

            Debug.Assert(IsPushingItems);
            if (IsPushingItems)
            {
                PushedArea = _pushStrategy!.Cancel();
                IsPushingItems = false;
            }
        }

        protected internal void PushItems(Vector offset)
        {
            Debug.Assert(IsPushingItems);
            PushedArea = _pushStrategy!.Push(offset);
        }

        protected internal void EndPushingItems()
        {
            Debug.Assert(IsPushingItems);
            if (IsPushingItems)
            {
                PushedArea = _pushStrategy!.End();
                _pushStrategy = null;
                IsPushingItems = false;
            }
        }

        private void UpdatePushedArea()
        {
            if (IsPushingItems)
            {
                PushedArea = _pushStrategy!.OnViewportChanged();
            }
        }

        private IPushStrategy CreatePushStrategy(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                return new HorizontalPushStrategy(this);
            }

            return new VerticalPushStrategy(this);
        }
    }
}
