using Caliburn.Micro;

namespace UruniumWPFExample.Fx
{
    /// <summary>
    /// Helper class that can update ui when state is changed.
    /// </summary>
    /// <remarks>
    /// Think of it like `props` class in react, when coding with typescript.
    /// Exception being, all event handlers are also kept in this class.
    /// </remarks>
    public abstract class Connectable : PropertyChangedBase
    {
        public void UpdateUi()
        {
            NotifyOfPropertyChange("");
        }
    }
}
