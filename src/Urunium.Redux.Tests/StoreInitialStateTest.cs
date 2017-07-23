using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class StoreInitialStateTest
    {
        [TestCase]
        public void ConstructorCanSetInitialState()
        {
            Store<int> store1 = new Store<int>(new TestReducer(), 10);
            Store<int> store2 = new Store<int>(new TestReducer(), 20);

            Assert.AreEqual(10, store1.State);
            Assert.AreEqual(20, store2.State);
        }

        class TestReducer : IReducer<int>
        {
            public int Apply(int previousState, object action)
            {
                return previousState;
            }
        }
    }
}
