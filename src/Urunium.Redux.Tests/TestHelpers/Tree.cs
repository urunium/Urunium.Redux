using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Compose;

namespace Urunium.Redux.Tests.TestHelpers
{
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
