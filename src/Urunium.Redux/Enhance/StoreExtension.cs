using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Enhance
{
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
                if (!typeof(StoreEnhancer<TState>).IsAssignableFrom(enhancerType))
                    throw new ArgumentException($"{enhancerType.FullName} must inherit from {typeof(StoreEnhancer<TState>).FullName}");

                finalStore = (IStore<TState>)Activator.CreateInstance(enhancerType, finalStore);
            }
            return finalStore;
        }
    }
}
