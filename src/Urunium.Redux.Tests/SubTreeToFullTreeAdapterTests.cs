using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Compose;
using System.Linq.Expressions;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class SubTreeToFullTreeAdapterTests
    {
        [Test]
        public void ApplyShouldWorkOnSelectedPropertyOfState()
        {
            Tree state = new Tree();
            state.IntProp = 1;
            state.BoolProp = false;
            SubTreeToFullTreeAdapter<Tree, int> adapter1 = new SubTreeToFullTreeAdapter<Tree, int>(new IntPropReducer());
            SubTreeToFullTreeAdapter<Tree, bool> adapter2 = new SubTreeToFullTreeAdapter<Tree, bool>(new BoolPropReducer());
            adapter1.Apply(state, "");
            adapter2.Apply(state, "");
            Assert.AreEqual(2, state.IntProp);
            Assert.AreEqual(true, state.BoolProp);
        }

        [Test]
        public void ApplyShouldWorkOnSelectedPropertyOfImmutableState()
        {
            ImmutableTree state = new ImmutableTree(1, false);

            SubTreeToFullTreeAdapter<ImmutableTree, int> adapter1 = new SubTreeToFullTreeAdapter<ImmutableTree, int>(new ImmutableIntPropReducer());
            SubTreeToFullTreeAdapter<ImmutableTree, bool> adapter2 = new SubTreeToFullTreeAdapter<ImmutableTree, bool>(new ImmutableBoolPropReducer());
            state = adapter1.Apply(state, "");
            state = adapter2.Apply(state, "");
            Assert.AreEqual(2, state.IntProp);
            Assert.AreEqual(true, state.BoolProp);
        }

        public class IntPropReducer : ISubTreeReducer<Tree, int>
        {
            public Expression<Func<Tree, int>> PropertySelector => state => state.IntProp;

            public int Apply(int previousState, object action)
            {
                return previousState + 1;
            }
        }

        public class BoolPropReducer : ISubTreeReducer<Tree, bool>
        {
            public Expression<Func<Tree, bool>> PropertySelector => state => state.BoolProp;

            public bool Apply(bool previousState, object action)
            {
                return !previousState;
            }
        }

        public class ImmutableIntPropReducer : ISubTreeReducer<ImmutableTree, int>
        {
            public Expression<Func<ImmutableTree, int>> PropertySelector => state => state.IntProp;

            public int Apply(int previousState, object action)
            {
                return previousState + 1;
            }
        }

        public class ImmutableBoolPropReducer : ISubTreeReducer<ImmutableTree, bool>
        {
            public Expression<Func<ImmutableTree, bool>> PropertySelector => state => state.BoolProp;

            public bool Apply(bool previousState, object action)
            {
                return !previousState;
            }
        }

        public class Tree
        {
            public int IntProp { get; set; }
            public bool BoolProp { get; set; }
        }

        public class ImmutableTree
        {
            public int IntProp { get; }
            public bool BoolProp { get; }

            public ImmutableTree(int intProp, bool boolProp)
            {
                IntProp = intProp;
                BoolProp = boolProp;
            }
        }
    }
}
