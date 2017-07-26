using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Typed;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class TypedReducerTest
    {
        [Test]
        public void TypedReducerIsWorkingAsExpectedTest()
        {
            IReducer<int> reducer = new MathReducer();
            var result = reducer.Apply(0, new Add(1));
            Assert.AreEqual(1, result);
            result = reducer.Apply(10, new Subtract(2));
            Assert.AreEqual(8, result);
            result = reducer.Apply(2, new Multiply(10));
            Assert.AreEqual(20, result);
            result = reducer.Apply(10, new Divide(2));
            Assert.AreEqual(5, result);
        }

        [Test]
        public void TypedReducerWithoutImplementingIApplyInterfaceTest()
        {
            IReducer<int> reducer = new MathReducerWithoutIApplyInterfaces();
            var result = reducer.Apply(0, new Add(1));
            Assert.AreEqual(1, result);
            result = reducer.Apply(10, new Subtract(2));
            Assert.AreEqual(8, result);
            result = reducer.Apply(2, new Multiply(10));
            Assert.AreEqual(20, result);
            result = reducer.Apply(10, new Divide(2));
            Assert.AreEqual(5, result);
        }

        [Test]
        public void TypedReducerWithNoApplyMethodWorksTest()
        {
            IReducer<int> reducer = new MathReducerNoImplementation();
            var result = reducer.Apply(0, new Add(1));
            Assert.AreEqual(0, result);
            result = reducer.Apply(10, new Subtract(2));
            Assert.AreEqual(10, result);
            result = reducer.Apply(2, new Multiply(10));
            Assert.AreEqual(2, result);
            result = reducer.Apply(10, new Divide(2));
            Assert.AreEqual(10, result);
        }

        [Test]
        public void TypedReducerFallsbackWhenActionIsNotSupportedTest()
        {
            IReducer<int> reducer = new AddReducer();
            // Supported action
            var result = reducer.Apply(0, new Add(1));
            Assert.AreEqual(1, result);
            // Fallback
            result = reducer.Apply(10, new Subtract(2));
            Assert.AreEqual(10, result);
            result = reducer.Apply(2, new Multiply(10));
            Assert.AreEqual(2, result);
            result = reducer.Apply(10, new Divide(2));
            Assert.AreEqual(10, result);
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

        class MathReducer : TypedReducer<int>,
            IApply<int, Add>,
            IApply<int, Subtract>,
            IApply<int, Multiply>,
            IApply<int, Divide>
        {
            public int Apply(int previousState, Add action)
            {
                return previousState + action.Addend;
            }

            public int Apply(int previousState, Subtract action)
            {
                return previousState - action.Subtrahend;
            }

            public int Apply(int previousState, Multiply action)
            {
                return previousState * action.Multiplier;
            }

            public int Apply(int previousState, Divide action)
            {
                return previousState / action.Divisor;
            }
        }

        class MathReducerWithoutIApplyInterfaces : TypedReducer<int>
        {
            public int Apply(int previousState, Add action)
            {
                return previousState + action.Addend;
            }

            public int Apply(int previousState, Subtract action)
            {
                return previousState - action.Subtrahend;
            }

            public int Apply(int previousState, Multiply action)
            {
                return previousState * action.Multiplier;
            }

            public int Apply(int previousState, Divide action)
            {
                return previousState / action.Divisor;
            }
        }

        class MathReducerNoImplementation : TypedReducer<int>
        {
        }

        class AddReducer : TypedReducer<int>, IApply<int, Add>
        {
            public int Apply(int previousState, Add action)
            {
                return previousState + action.Addend;
            }
        }

    }
}
