using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class StoreDispatchTest
    {
        [TestCase]
        public void DispatchWillCallReducerApply()
        {
            Mock<IReducer<int>> moqReducer = new Mock<IReducer<int>>();

            moqReducer.Setup(x => x.Apply(It.IsAny<int>(), It.Is<string>(y => y == "+"))).Returns(11);
            moqReducer.Setup(x => x.Apply(It.IsAny<int>(), It.Is<string>(y => y == "-"))).Returns(10);
            Store<int> store = new Store<int>(moqReducer.Object, 10);
            store.Dispatch("+");
            Assert.AreEqual(11, store.State);
            store.Dispatch("-");
            Assert.AreEqual(10, store.State);
        }

        [TestCase]
        public void DispatchWillFireStateChangedEvent()
        {
            Mock<IReducer<int>> moqReducer = new Mock<IReducer<int>>();
            List<EventArgs> firedEvents = new List<EventArgs>();

            moqReducer.Setup(x => x.Apply(It.IsAny<int>(), It.Is<string>(y => y == "+"))).Returns(11);
            moqReducer.Setup(x => x.Apply(It.IsAny<int>(), It.Is<string>(y => y == "-"))).Returns(10);
            Store<int> store = new Store<int>(moqReducer.Object, 10);
            store.StateChanged += (sender, eventArg) => firedEvents.Add(eventArg);
            store.Dispatch("+");
            Assert.AreEqual(11, store.State);
            store.Dispatch("-");
            Assert.AreEqual(10, store.State);
            Assert.AreEqual(2, firedEvents.Count);
        }

        [TestCase]
        public void DispatchWillFireStateChangedEventOnlyIfStateChanges()
        {
            Mock<IReducer<int>> moqReducer = new Mock<IReducer<int>>();
            List<EventArgs> firedEvents = new List<EventArgs>();

            moqReducer.Setup(x => x.Apply(It.IsAny<int>(), It.Is<string>(y => y == "+"))).Returns(10);
            moqReducer.Setup(x => x.Apply(It.IsAny<int>(), It.Is<string>(y => y == "-"))).Returns(10);
            Store<int> store = new Store<int>(moqReducer.Object, 10);
            store.StateChanged += (sender, eventArg) => firedEvents.Add(eventArg);
            store.Dispatch("+");
            store.Dispatch("-");
            Assert.AreEqual(0, firedEvents.Count);
        }

        class TestState
        {
            public int Prop1 { get; }

            public TestState(int prop1)
            {
                Prop1 = prop1;
            }
        }
    }
}
