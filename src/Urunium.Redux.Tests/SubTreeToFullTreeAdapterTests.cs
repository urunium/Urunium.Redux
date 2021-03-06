﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Compose;
using System.Linq.Expressions;
using Urunium.Redux.Tests.TestHelpers;

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
    }
}
