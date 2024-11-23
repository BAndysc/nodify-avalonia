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
            private set => SetAndRaise(IsPushingItemsProperty, ref isPushingItems, value);
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
        /// Gets or sets whether push items cancellation is allowed (see <see cref="EditorGestures.NodifyEditorGestures.CancelAction"/>).
        /// </summary>
        public static bool AllowPushItemsCancellation { get; set; } = true;

        private IPushStrategy? _pushStrategy;

        /// <summary>
        /// Starts the pushing items operation at the specified location with the specified orientation.
        /// </summary>
        /// <param name="location">The starting location for pushing items, in graph space coordinates.</param>
        /// <param name="orientation">The orientation of the <see cref="PushedArea"/>.</param>
        protected internal void StartPushingItems(Point location, Orientation orientation)
        {
            Debug.Assert(!IsPushingItems);

            IsPushingItems = true;
            PushedAreaOrientation = orientation;

            _pushStrategy = CreatePushStrategy(orientation);

            PushedArea = _pushStrategy.Start(location);
        }

        /// <summary>
        /// Cancels the current pushing operation and reverts the <see cref="PushedArea"/> to its initial state.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if pushing item cancellation is not allowed (see <see cref="AllowPushItemsCancellation"/>).</exception>
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

        /// <summary>
        /// Updates the pushed area based on the specified amount.
        /// </summary>
        /// <param name="offset">The amount to adjust the pushed area by.</param>
        /// <remarks>
        /// This method adjusts the pushed area incrementally. It should only be called while a pushing operation is active (see <see cref="StartPushingItems(Point, Orientation)"/>).
        /// </remarks>
        protected internal void PushItems(Vector amount)
        {
            Debug.Assert(IsPushingItems);
            PushedArea = _pushStrategy!.Push(amount);
        }

        /// <summary>
        /// Ends the current pushing operation and finalizes the pushed area state.
        /// </summary>
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
