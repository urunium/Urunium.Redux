using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Enhance
{
    /// <summary>
    /// Extension methods for <see cref="IStore{TState}"/> to enhance store instances.
    /// </summary>
    public static class StoreExtension
    {
        /// <summary>
        /// Applies enhancers to given store.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="originalStore">IStore instance</param>
        /// <param name="enhancerTypes">
        /// All store enhancers must inherit from <seealso cref="StoreEnhancer{TState}"/>
        /// </param>
        /// <returns>instance of IStore after applying all enhancers.</returns>
        public static IStore<TState> EnhanceWith<TState>(this IStore<TState> originalStore, params Type[] enhancerTypes)
        {
            IStore<TState> finalStore = originalStore;
            foreach (var enhancerType in enhancerTypes)
            {
                if (!typeof(StoreEnhancer<TState>).GetTypeInfo().IsAssignableFrom(enhancerType.GetTypeInfo()))
                    throw new ArgumentException($"{enhancerType.FullName} must inherit from {typeof(StoreEnhancer<TState>).FullName}");

                finalStore = (IStore<TState>)Activator.CreateInstance(enhancerType, finalStore);
            }
            return finalStore;
        }

        /// <summary>
        /// Locate a particular store enhancer applied to current store.
        /// Note: Search is inwards, i.e while locating, traversal is done from 
        /// outer most enhacer to inner-most IStore.
        /// </summary>
        /// <typeparam name="TEnhancer">Type of Enhancer to locate.</typeparam>
        /// <typeparam name="TState">Type of state.</typeparam>
        /// <param name="originalStore">Instance of IStore from which Enhancer is to be located.</param>
        /// <returns>Enhancer instance if found, or null.</returns>
        public static TEnhancer FindEnhancer<TEnhancer, TState>(this IStore<TState> originalStore) where TEnhancer : StoreEnhancer<TState>
        {
            if (originalStore is StoreEnhancer<TState> enhancer)
            {
                return enhancer.Find<TEnhancer>();
            }
            return null;
        }
    }
}
