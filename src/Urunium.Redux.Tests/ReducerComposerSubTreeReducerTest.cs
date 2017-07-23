using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Compose;
using Urunium.Redux.Tests.TestHelpers;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class ReducerComposerSubTreeReducerTest
    {
        [Test]
        public void ApplyShouldWorkOnSelectedPropertyOfState()
        {
            Tree state = new Tree();
            state.IntProp = 1;
            state.BoolProp = false;
            ReducerComposer<Tree> composer = new ReducerComposer<Tree>();
            composer.AddSubTreeReducer(new IntPropReducer())
                    .AddSubTreeReducer(new BoolPropReducer());
            composer.Apply(state, "");
            Assert.AreEqual(2, state.IntProp);
            Assert.AreEqual(true, state.BoolProp);
        }

        [Test]
        public void ApplyShouldWorkOnSelectedPropertyOfImmutableState()
        {
            ImmutableTree state = new ImmutableTree(1, false);

            ReducerComposer<ImmutableTree> composer = new ReducerComposer<ImmutableTree>();
            composer.AddSubTreeReducer(new ImmutableIntPropReducer())
                    .AddSubTreeReducer(new ImmutableBoolPropReducer());
            state = composer.Apply(state, "");
            Assert.AreEqual(2, state.IntProp);
            Assert.AreEqual(true, state.BoolProp);
        }
    }
}
