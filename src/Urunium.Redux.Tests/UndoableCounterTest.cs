using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Urunium.Redux.Undoable;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class UndoableCounterTest
    {
        [Test]
        public void TestIncrement()
        {
            IStore<UndoableState<int>> store = new Store<UndoableState<int>>(
                                                                    new UndoableReducer<int>(new Counter()),
                                                                    new UndoableState<int>(0));
            store.Dispatch(new Increment());
            Assert.AreEqual(1, store.State.Present);

            store.Dispatch(new Increment());
            Assert.AreEqual(2, store.State.Present);

            store.Dispatch(new Increment());
            Assert.AreEqual(3, store.State.Present);
        }

        [Test]
        public void TestDecrement()
        {
            IStore<UndoableState<int>> store = new Store<UndoableState<int>>(
                                                                    new UndoableReducer<int>(new Counter()),
                                                                    new UndoableState<int>(10));
            store.Dispatch(new Decrement());
            Assert.AreEqual(9, store.State.Present);

            store.Dispatch(new Decrement());
            Assert.AreEqual(8, store.State.Present);

            store.Dispatch(new Decrement());
            Assert.AreEqual(7, store.State.Present);
        }

        [Test]
        public void TestUndo()
        {
            IStore<UndoableState<int>> store = new Store<UndoableState<int>>(
                                                                    new UndoableReducer<int>(new Counter()),
                                                                    new UndoableState<int>(0));
            store.Dispatch(new Increment());
            Assert.AreEqual(1, store.State.Present);

            store.Dispatch(new Increment());
            Assert.AreEqual(2, store.State.Present);

            store.Dispatch(new Increment());
            Assert.AreEqual(3, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(2, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(1, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(0, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(0, store.State.Present);
        }

        [Test]
        public void TestRedo()
        {
            IStore<UndoableState<int>> store = new Store<UndoableState<int>>(
                                                                    new UndoableReducer<int>(new Counter()),
                                                                    new UndoableState<int>(0));
            store.Dispatch(new Increment());
            Assert.AreEqual(1, store.State.Present);

            store.Dispatch(new Increment());
            Assert.AreEqual(2, store.State.Present);

            store.Dispatch(new Increment());
            Assert.AreEqual(3, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(2, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(1, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(0, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(0, store.State.Present);

            store.Dispatch(new Redo());
            Assert.AreEqual(1, store.State.Present);

            store.Dispatch(new Redo());
            Assert.AreEqual(2, store.State.Present);

            store.Dispatch(new Redo());
            Assert.AreEqual(3, store.State.Present);

            store.Dispatch(new Redo());
            Assert.AreEqual(3, store.State.Present);
        }

        [Test]
        public void TestNewActionInMiddleOfRedo()
        {
            IStore<UndoableState<int>> store = new Store<UndoableState<int>>(
                                                                    new UndoableReducer<int>(new Counter()),
                                                                    new UndoableState<int>(0));
            store.Dispatch(new Increment());
            Assert.AreEqual(1, store.State.Present);

            store.Dispatch(new Increment());
            Assert.AreEqual(2, store.State.Present);

            store.Dispatch(new Increment());
            Assert.AreEqual(3, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(2, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(1, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(0, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(0, store.State.Present);

            store.Dispatch(new Redo());
            Assert.AreEqual(1, store.State.Present);

            store.Dispatch(new Redo());
            Assert.AreEqual(2, store.State.Present);

            store.Dispatch(new Decrement());
            Assert.AreEqual(1, store.State.Present);

            store.Dispatch(new Undo());
            Assert.AreEqual(2, store.State.Present);

            store.Dispatch(new Redo());
            Assert.AreEqual(1, store.State.Present);
        }
        
        public class Increment
        {
        }

        public class Decrement
        {
        }

        public class Counter : Typed.TypedReducer<int>
        {
            public int Apply(int previousState, Increment action)
            {
                return previousState + 1;
            }

            public int Apply(int previousState, Decrement action)
            {
                return previousState - 1;
            }
        }
    }
}
