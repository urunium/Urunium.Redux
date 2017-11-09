using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace WpfTodoMvcExample.Fx.CustomTriggers
{
    /// <summary>
    /// Class to support handling specific keyboard keys, and gestures.
    /// </summary>
    public class KeyTrigger : TriggerBase<UIElement>
    {
        /// <summary>
        /// `Key` property.
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(KeyTrigger), null);

        /// <summary>
        /// `Modifiers` property.
        /// </summary>
        public static readonly DependencyProperty ModifiersProperty =
            DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(KeyTrigger), null);

        /// <summary>
        /// Get pressed key.
        /// </summary>
        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        /// Get modifiers, if any.
        /// </summary>
        public ModifierKeys Modifiers
        {
            get { return (ModifierKeys)GetValue(ModifiersProperty); }
            set { SetValue(ModifiersProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.KeyDown += OnAssociatedObjectKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.KeyDown -= OnAssociatedObjectKeyDown;
        }

        private void OnAssociatedObjectKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key) && (Keyboard.Modifiers == GetActualModifiers(e.Key, Modifiers)))
            {
                InvokeActions(e);
            }
        }

        static ModifierKeys GetActualModifiers(Key key, ModifierKeys modifiers)
        {
            switch (key)
            {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    modifiers |= ModifierKeys.Control;
                    return modifiers;

                case Key.LeftAlt:
                case Key.RightAlt:
                    modifiers |= ModifierKeys.Alt;
                    return modifiers;

                case Key.LeftShift:
                case Key.RightShift:
                    modifiers |= ModifierKeys.Shift;
                    break;
            }

            return modifiers;
        }
    }
}
