using System.Windows.Input;

namespace Nodify
{
    /// <summary>The base class for container states.</summary>
    public abstract class ConnectionState
    {
        /// <summary>Constructs a new <see cref="ConnectionState"/>.</summary>
        /// <param name="container">The owner of the state.</param>
        public ConnectionState(ConnectionContainer container)
        {
            Container = container;
        }

        /// <summary>The owner of the state.</summary>
        protected ConnectionContainer Container { get; }

        /// <summary>The owner of the state.</summary>
        protected NodifyEditor Editor => Container.Editor;

        /// <inheritdoc cref="ConnectionContainer.OnMouseDown(MouseButtonEventArgs)"/>
        public virtual void HandleMouseDown(MouseButtonEventArgs e) { }

        /// <inheritdoc cref="ConnectionContainer.OnMouseDown(MouseButtonEventArgs)"/>
        public virtual void HandleMouseUp(MouseButtonEventArgs e) { }

        /// <inheritdoc cref="ConnectionContainer.OnMouseMove(MouseEventArgs)"/>
        public virtual void HandleMouseMove(MouseEventArgs e) { }

        /// <inheritdoc cref="ConnectionContainer.OnMouseWheel(MouseWheelEventArgs)"/>
        public virtual void HandleMouseWheel(MouseWheelEventArgs e) { }

        /// <inheritdoc cref="ConnectionContainer.OnKeyUp(KeyEventArgs)"/>
        public virtual void HandleKeyUp(KeyEventArgs e) { }

        /// <inheritdoc cref="ConnectionContainer.OnKeyDown(KeyEventArgs)"/>
        public virtual void HandleKeyDown(KeyEventArgs e) { }

        /// <summary>Called when <see cref="ConnectionContainer.PushState(ConnectionState)"/> or <see cref="ConnectionContainer.PopState"/> is called.</summary>
        /// <param name="from">The state we enter from (is null for root state).</param>
        public virtual void Enter(ConnectionState? from, MouseEventArgs? e) { }

        /// <summary>Called when <see cref="ConnectionContainer.PopState"/> is called.</summary>
        public virtual void Exit() { }

        /// <summary>Called when <see cref="ConnectionContainer.PopState"/> is called.</summary>
        /// <param name="from">The state we re-enter from.</param>
        public virtual void ReEnter(ConnectionState from) { }

        /// <summary>Pushes a new state into the stack.</summary>
        /// <param name="newState">The new state.</param>
        /// <param name="pointerEventArgs"></param>
        public virtual void PushState(ConnectionState newState, MouseEventArgs e) => Container.PushState(newState, e);

        /// <summary>Pops the current state from the stack.</summary>
        public virtual void PopState() => Container.PopState();
    }
}
