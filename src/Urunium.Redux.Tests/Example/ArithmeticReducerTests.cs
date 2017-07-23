using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Tests.Example
{
    [TestFixture]
    public class ArithmeticReducerTests
    {
        [Test]
        public void TestIncrement()
        {
            Store<int> store = new Store<int>(new ArithMeticReducer(), 0);
            store.Dispatch("++");
            Assert.AreEqual(1, store.State);
        }

        [Test]
        public void TestDecrement()
        {
            Store<int> store = new Store<int>(new ArithMeticReducer(), 4);
            store.Dispatch("--");
            Assert.AreEqual(3, store.State);
        }

        [Test]
        public void TestAdd()
        {
            Store<int> store = new Store<int>(new ArithMeticReducer(), 0);
            store.Dispatch(new Add(1));
            Assert.AreEqual(1, store.State);
        }

        [Test]
        public void TestSubtract()
        {
            Store<int> store = new Store<int>(new ArithMeticReducer(), 10);
            store.Dispatch(new Subtract(1));
            Assert.AreEqual(9, store.State);
        }

        [Test]
        public void TestMultiply()
        {
            Store<int> store = new Store<int>(new ArithMeticReducer(), 10);
            store.Dispatch(new Multiply(2));
            Assert.AreEqual(20, store.State);
        }

        [Test]
        public void TestDivide()
        {
            Store<int> store = new Store<int>(new ArithMeticReducer(), 10);
            store.Dispatch(new Divide(2));
            Assert.AreEqual(5, store.State);
        }

        [Test]
        public void TestEventSubscription()
        {
            List<int> states = new List<int>();
            Store<int> store = new Store<int>(new ArithMeticReducer(), 10);
            store.StateChanged += (sender, eventargs) => states.Add(store.State);
            store.Dispatch(new Divide(2));
            store.Dispatch(new Add(1));
            store.Dispatch(new Subtract(2));
            Assert.AreEqual(4, store.State);
            Assert.AreEqual(3, states.Count);
            Assert.AreEqual(5, states[0]);
            Assert.AreEqual(6, states[1]);
            Assert.AreEqual(4, states[2]);
        }

        class ArithMeticReducer : IReducer<int>
        {
            public int Apply(int previousState, object action)
            {
                switch (action)
                {
                    case string operation when operation == "++":
                        return previousState + 1;
                    case string operation when operation == "--":
                        return previousState - 1;
                    case Add operation:
                        return previousState + operation.Addend;
                    case Subtract operation:
                        return previousState - operation.Subtrahend;
                    case Multiply operation:
                        return previousState * operation.Multiplier;
                    case Divide operation:
                        return previousState / operation.Divisor;
                }
                return previousState;
            }
        }

        class Add
        {
            public int Addend { get; }
            public Add(int addend)
            {
                Addend = addend;
            }
        }

        class Subtract
        {
            public int Subtrahend { get; }
            public Subtract(int subtrahend)
            {
                Subtrahend = subtrahend;
            }
        }

        class Multiply
        {
            public int Multiplier { get; }
            public Multiply(int multiplier)
            {
                Multiplier = multiplier;
            }
        }

        class Divide
        {
            public int Divisor { get; }
            public Divide(int divisor)
            {
                Divisor = divisor;
            }
        }
    }
}
