using System;
using System.Threading.Tasks;
using Urunium.Redux;

namespace WpfTodoMvcExample.Fx
{
    /// <summary>
    /// IStore extension
    /// </summary>
    public static class StoreExtension
    {
        /// <summary>
        /// Simulates Connect functionality of react-redux. But not exactly.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TProps"></typeparam>
        /// <param name="store"></param>
        /// <param name="mapStateToProps"></param>
        /// <param name="mapDispatchToProps"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        public static Func<TProps, TProps> Connect<TState, TProps>(
            this IStore<TState> store,
            Action<TState, TProps> mapStateToProps,
            Action<IStore<TState>, TState, TProps> mapDispatchToProps) where TProps : Connectable
        {
            return (props) =>
            {
                if (mapStateToProps == null)
                    throw new ArgumentNullException(nameof(mapStateToProps));

                mapStateToProps(store.State, props);
                mapDispatchToProps(store, store.State, props);

                EventHandler<EventArgs> subscription = (sender, earg) =>
                {
                    mapStateToProps(store.State, props);
                    mapDispatchToProps(store, store.State, props);
                    props.UpdateUi();
                };

                store.StateChanged += subscription;


                return props;
            };
        }

        /// <summary>
        /// Dispatch on new thread.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TAction"></typeparam>
        /// <param name="store"></param>
        /// <param name="action"></param>
        public static void DispatchAsync<TState, TAction>(
            this IStore<TState> store,
            TAction action)
        {
            Task.Run(() =>
            {
                store.Dispatch(action);
            });
        }
    }
}
