using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Compose;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class ReducerComposerTests
    {
        [Test]
        public void ApplyShowForwardCallToRegisteredReducer()
        {
            Mock<IReducer<TestState>> mockIReducer = new Mock<IReducer<TestState>>();
            mockIReducer.Setup(x => x.Apply(It.IsAny<TestState>(), It.IsAny<object>())).Returns(new TestState());
            ReducerComposer<TestState> composer = new ReducerComposer<TestState>();
            composer.AddStateReducer(mockIReducer.Object);
            var result = composer.Apply(new TestState(), "");
            mockIReducer.Verify(x => x.Apply(It.IsAny<TestState>(), It.IsAny<object>()));
        }

        [Test]
        public void CanBeUsedAsRootReducer()
        {
            Mock<IReducer<TestState>> mockIReducer = new Mock<IReducer<TestState>>();
            mockIReducer.Setup(x => x.Apply(It.IsAny<TestState>(), It.IsAny<object>())).Returns(new TestState());
            ReducerComposer<TestState> composer = new ReducerComposer<TestState>();
            composer.AddStateReducer(mockIReducer.Object);

            Store<TestState> store = new Store<TestState>(composer, new TestState());
            store.Dispatch("");
            mockIReducer.Verify(x => x.Apply(It.IsAny<TestState>(), It.IsAny<object>()));
        }

        public class TestState
        {

        }
    }
}
