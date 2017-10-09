using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Compose;
using System.Linq.Expressions;
using Urunium.Redux.Typed;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class IgnoreStaticPropertiesInState
    {
        [TestCase]
        public void SubTreeToFullTreeAdapterShouldWorkWhenStateHasStaticProperties()
        {
            var target = new SubTreeToFullTreeAdapter<AppState, int>(new IdReducer());
            var newState = target.Apply(AppState.InitialState, 1);
            Assert.AreEqual(1, newState.Id);
        }

        public class IdReducer : TypedReducer<int>, ISubTreeReducer<AppState, int>
        {
            public Expression<Func<AppState, int>> PropertySelector => x => x.Id;

            public int Apply(int previousState, int id)
            {
                return id;
            }
        }

        public class AppState
        {
            public int Id { get; }
            public AppState(int id)
            {
                Id = id;
            }

            public static AppState InitialState => new AppState(0);
        }
    }
}
