using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Compose;
using Urunium.Redux.Logic;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class MultiDispatcherTest
    {
        [Test]
        public void TestMultipleCallsToDispatchMethodFiresTheEventOnlyOnce()
        {
            var reducer = new ReducerComposer<AppState>();
            Mock<IReducer<AppState>> mockReducer = new Mock<IReducer<AppState>>();
            mockReducer.Setup(x => x.Apply(It.IsAny<AppState>(), It.IsAny<object>())).Returns(() => new AppState());
            reducer.AddStateReducer(mockReducer.Object);
            Store<AppState> store = new Store<AppState>(reducer, new AppState());
            int countEvent = 0;
            store.StateChanged += (s, e) =>
            {
                countEvent++;
            };
            using (var dispatcher = MultiDispatcher.Create(store))
            {
                dispatcher.Dispatch("1");
                dispatcher.Dispatch("2");
                dispatcher.Dispatch("3");
                dispatcher.Dispatch("4");
                dispatcher.Dispatch("5");
            }
            Assert.AreEqual(1, countEvent);
        }

        [Test]
        public void TestMultipleCallsToDispatchImmediateMethodFiresTheEventSameNumberOfTimes()
        {
            var reducer = new ReducerComposer<AppState>();
            Mock<IReducer<AppState>> mockReducer = new Mock<IReducer<AppState>>();
            mockReducer.Setup(x => x.Apply(It.IsAny<AppState>(), It.IsAny<object>())).Returns(() => new AppState());
            reducer.AddStateReducer(mockReducer.Object);
            Store<AppState> store = new Store<AppState>(reducer, new AppState());
            int countEvent = 0;
            store.StateChanged += (s, e) =>
            {
                countEvent++;
            };
            using (var dispatcher = MultiDispatcher.Create(store))
            {
                dispatcher.DispatchImmediate("1");
                dispatcher.DispatchImmediate("2");
                dispatcher.DispatchImmediate("3");
            }
            Assert.AreEqual(3, countEvent);
        }

        [Test]
        public void TestDispatchWorksOutsideScopeOfMultiDispatcher()
        {
            var reducer = new ReducerComposer<AppState>();
            Mock<IReducer<AppState>> mockReducer = new Mock<IReducer<AppState>>();
            mockReducer.Setup(x => x.Apply(It.IsAny<AppState>(), It.IsAny<object>())).Returns(() => new AppState());
            reducer.AddStateReducer(mockReducer.Object);
            Store<AppState> store = new Store<AppState>(reducer, new AppState());
            int countEvent = 0;
            store.StateChanged += (s, e) =>
            {
                countEvent++;
            };
            store.Dispatch("1");
            using (var dispatcher = MultiDispatcher.Create(store))
            {
                dispatcher.Dispatch("2");
            }
            store.Dispatch("3");
            Assert.AreEqual(3, countEvent);
        }

        [Test]
        public void TestMultiDispatcherScope()
        {
            var reducer = new ReducerComposer<AppState>();
            Mock<IReducer<AppState>> mockReducer = new Mock<IReducer<AppState>>();
            mockReducer.Setup(x => x.Apply(It.IsAny<AppState>(), It.IsAny<object>())).Returns(() => new AppState());
            reducer.AddStateReducer(mockReducer.Object);
            Store<AppState> store = new Store<AppState>(reducer, new AppState());
            int countEvent = 0;
            store.StateChanged += (s, e) =>
            {
                countEvent++;
            };
            using (var dispatcher = MultiDispatcher.Create(store))
            {
                dispatcher.Dispatch("1");
                dispatcher.Dispatch("2");
                dispatcher.Dispatch("3");
                using (var scope = dispatcher.BeginScope())
                {
                    scope.Dispatch("s1");
                    scope.Dispatch("s2");
                    scope.Dispatch("s3");
                }
                dispatcher.Dispatch("4");
                dispatcher.Dispatch("5");
            }
            Assert.AreEqual(2, countEvent);

            countEvent = 0;
            using (var dispatcher = MultiDispatcher.Create(store))
            {
                dispatcher.Dispatch("1");
                dispatcher.Dispatch("2");
                dispatcher.Dispatch("3");
                using (var scope = dispatcher.BeginScope())
                {
                    scope.Dispatch("s1");
                    scope.Dispatch("s2");
                    scope.Dispatch("s3");
                }
                dispatcher.Dispatch("4");
                dispatcher.Dispatch("5");
                using (var scope = dispatcher.BeginScope())
                {
                    scope.Dispatch("s1");
                    scope.Dispatch("s2");
                    scope.Dispatch("s3");
                }
            }
            Assert.AreEqual(3, countEvent);
        }


        public class AppState
        {
        }
    }
}
