using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace UruniumWPFExample.Fx.CustomTriggers
{
    /// <summary>
    /// Class used to support double click event on elements,
    /// that doesn't have in-built support for double-click event, e.g Grid.
    /// </summary>
    public class DblClickTrigger : TriggerBase<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                InvokeActions(e);
            }
        }
    }
}
