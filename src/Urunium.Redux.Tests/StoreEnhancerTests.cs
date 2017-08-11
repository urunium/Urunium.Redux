using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Enhance;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class StoreEnhancerTests
    {
        [Test]
        public void EnhanceStateChangedTest()
        {
            var store = (
                          new Store<State>(
                            new RootReducer(),
                            new State(1)
                          )
                        ).EnhanceWith(typeof(MyEnhancer<State>)) as MyEnhancer<State>;

            int count = 0;
            store.StateChanged += (o, e) => count++;
            store.Dispatch(new SetIdAction(1));
            store.Dispatch(new SetIdAction(2));
            Assert.AreEqual(2, count);
            Assert.AreEqual(2, store.StateHistory.Count);
            Assert.AreEqual(1, store.StateHistory[0].Id);
            Assert.AreEqual(2, store.StateHistory[1].Id);
        }

        [Test]
        public void EnhanceDispatchTest()
        {
            var store = new MyEnhancer<State>(new Store<State>(new RootReducer(), new State(1)));
            store.Dispatch(new SetIdAction(1));
            store.Dispatch(new SetIdAction(2));
            Assert.AreEqual(2, store.ActionHistory.Count);
            Assert.AreEqual(1, (store.ActionHistory[0] as SetIdAction).Id);
            Assert.AreEqual(2, (store.ActionHistory[1] as SetIdAction).Id);
        }

        [Test]
        public void EnhanceStateTest()
        {
            var store = new MyEnhancer<State>(new Store<State>(new RootReducer(), new State(1)));
            store.Dispatch(new SetIdAction(1));
            store.Dispatch(new SetIdAction(2));
            var state = store.State;
            state = store.State;
            Assert.AreEqual(2, store.StateGet);
        }

        public class State
        {
            public int Id { get; }
            public State(int id)
            {
                Id = id;
            }
        }

        public class SetIdAction
        {
            public int Id { get; }
            public SetIdAction(int id)
            {
                Id = id;
            }
        }
        public class RootReducer : IReducer<State>
        {
            public State Apply(State previousState, object action)
            {
                if (action is SetIdAction setId)
                {
                    return new State(setId.Id);
                }
                return previousState;
            }
        }

        public class MyEnhancer<TState> : StoreEnhancer<TState>
        {
            public int StateGet;
            public List<object> ActionHistory = new List<object>();
            public List<TState> StateHistory = new List<TState>();
            public MyEnhancer(IStore<TState> store) : base(store)
            {
            }

            protected override void OnDispatch<TAction>(TAction action, Action<TAction> forward)
            {
                ActionHistory.Add(action);
                forward(action);
            }

            protected override TState OnState(Func<TState> forward)
            {
                StateGet++;
                return forward();
            }

            protected override void OnStateChanged(EventHandler<EventArgs> forward)
            {
                StateHistory.Add(Store.State);
                forward(this, new EventArgs());
            }
        }
    }
}
