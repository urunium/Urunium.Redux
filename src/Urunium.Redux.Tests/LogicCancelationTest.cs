using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Logic;
using System.Threading;
using Moq;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class LogicCancelationTest
    {
        [Test]
        public void TestBasicCancelation()
        {
            ILogic<AppState, TestAction> logic = new CancelableLogic();
            Mock<IMultiDispatcher> mock = new Mock<IMultiDispatcher>();

            Task t = logic.Process(() => new AppState(), new TestAction(), mock.Object);

            logic.Cancel(new CancelTestAction());

            Assert.IsTrue(t.IsCanceled);
        }

        [Test]
        public void TestPoisionPill()
        {
            ILogic<AppState, TestAction> logic = new CancelableLogic();
            Mock<IMultiDispatcher> mock = new Mock<IMultiDispatcher>();

            Task t = logic.Process(() => new AppState(), new TestAction(), mock.Object);

            logic.Cancel(new PoisonPill("Killing"));

            Assert.IsTrue(t.IsCanceled);
        }

        [Test]
        public void TestDispatchingCancel()
        {
            Mock<IStore<AppState>> storeMock = new Mock<IStore<AppState>>();
            CancelableLogic logic = new CancelableLogic();

            IStore<AppState> store = storeMock.Object.ConfigureLogic(x => x.AddLogics(logic));

            store.Dispatch(new TestAction());

            store.Dispatch(new CancelTestAction());

            Assert.IsTrue(logic.ProcessTask.IsCanceled);
        }

        [Test]
        public void TestCanDispatchToCanceledLogic()
        {
            Mock<IStore<AppState>> storeMock = new Mock<IStore<AppState>>();
            CancelableLogic logic = new CancelableLogic();

            IStore<AppState> store = storeMock.Object.ConfigureLogic(x => x.AddLogics(logic));

            store.Dispatch(new TestAction());

            store.Dispatch(new CancelTestAction());

            Assert.IsTrue(logic.ProcessTask.IsCanceled);

            store.Dispatch(new TestAction());

            Assert.False(logic.ProcessTask.IsCanceled);
        }

        [Test]
        public void TestCannotDispatchToKilledLogic()
        {
            Mock<IStore<AppState>> storeMock = new Mock<IStore<AppState>>();
            CancelableLogic logic = new CancelableLogic();

            IStore<AppState> store = storeMock.Object.ConfigureLogic(x => x.AddLogics(logic));

            store.Dispatch(new TestAction());

            store.Dispatch(new PoisonPill("test"));

            Assert.IsTrue(logic.ProcessTask.IsCanceled);

            store.Dispatch(new TestAction());

            Assert.True(logic.ProcessTask.IsCanceled);
        }

        [Test]
        public void TestCannotPreprocessKilledLogic()
        {
            Mock<IStore<AppState>> storeMock = new Mock<IStore<AppState>>();
            CancelableLogic logic = new CancelableLogic();

            IStore<AppState> store = storeMock.Object.ConfigureLogic(x => x.AddLogics(logic));

            store.Dispatch(new TestAction());

            store.Dispatch(new PoisonPill("test"));

            Assert.IsTrue(logic.ProcessTask.IsCanceled);

            store.Dispatch(new TestAction());

            Assert.True(logic.ProcessTask.IsCanceled);

            var result = (logic as ILogic<AppState, TestAction>).PreProcess(store, new TestAction()).Result;

            Assert.False(result.ContinueToNextStep);
        }

        [Test]
        public void TestNonFatalPosionPill()
        {
            Mock<IStore<AppState>> storeMock = new Mock<IStore<AppState>>();
            CancelableLogic logic = new CancelableLogic();

            IStore<AppState> store = storeMock.Object.ConfigureLogic(x => x.AddLogics(logic));

            store.Dispatch(new TestAction());

            store.Dispatch(new PoisonPill("test", kill: false));

            Assert.IsTrue(logic.ProcessTask.IsCanceled);

            store.Dispatch(new TestAction());

            Assert.False(logic.ProcessTask.IsCanceled);
        }

        [Test]
        public void TestCannotProcessKilledLogic()
        {
            Mock<IStore<AppState>> storeMock = new Mock<IStore<AppState>>();
            CancelableLogic logic = new CancelableLogic();

            IStore<AppState> store = storeMock.Object.ConfigureLogic(x => x.AddLogics(logic));

            logic.OnProcessExecuted = false;
            store.Dispatch(new TestAction());

            Assert.True(logic.OnProcessExecuted);

            store.Dispatch(new PoisonPill("test"));

            logic.OnProcessExecuted = false;
            var result = (logic as ILogic<AppState, TestAction>).Process(() => store.State, new TestAction(), null);

            Assert.False(logic.OnProcessExecuted);
        }

        [Test]
        public void TestCancelActionWillBeFurtherProcessed()
        {
            Mock<IStore<AppState>> storeMock = new Mock<IStore<AppState>>();
            CancelableLogic logic = new CancelableLogic();
            CancelTestActionProcessor cancelActionProcessor = new CancelTestActionProcessor();

            IStore<AppState> store = storeMock.Object.ConfigureLogic(x =>
            {
                x.AddLogics(logic);
                x.AddLogics(cancelActionProcessor);
            });

            store.Dispatch(new TestAction());

            store.Dispatch(new CancelTestAction());

            Assert.IsTrue(logic.ProcessTask.IsCanceled);

            Assert.True(cancelActionProcessor.OnProcessExecuted);
        }


        [Test]
        public void TestPoisionPillWillNeverBeProcessed()
        {
            Mock<IStore<AppState>> storeMock = new Mock<IStore<AppState>>();
            CancelableLogic logic = new CancelableLogic();
            PoisonPillProcessor poisonPillProcessor = new PoisonPillProcessor();

            IStore<AppState> store = storeMock.Object.ConfigureLogic(x =>
            {
                x.AddLogics(logic);
                x.AddLogics(poisonPillProcessor);
            });

            store.Dispatch(new TestAction());

            store.Dispatch(new PoisonPill("test"));

            Assert.IsTrue(logic.ProcessTask.IsCanceled);

            Assert.False(poisonPillProcessor.OnProcessExecuted);
        }


        public class AppState
        {
        }

        public class TestAction
        {
        }

        public class CancelTestAction
        {
        }

        public class CancelableLogic : LogicBase<AppState, TestAction>
        {
            public override Type CancelType => typeof(CancelTestAction);

            public override uint Priority => 0;

            public Task ProcessTask { get; private set; }

            public bool OnProcessExecuted { get; set; }

            protected override Task OnProcess(Func<AppState> getState, TestAction action, IMultiDispatcher dispatcher, CancellationToken cancellationToken)
            {
                OnProcessExecuted = true;
                ProcessTask = Task.Delay(10000, cancellationToken);
                return ProcessTask;
            }
        }

        public class CancelTestActionProcessor : LogicBase<AppState, CancelTestAction>
        {
            public override Type CancelType => null;

            public override uint Priority => 0;

            public bool OnProcessExecuted { get; private set; }

            protected override Task OnProcess(Func<AppState> getState, CancelTestAction action, IMultiDispatcher dispatcher, CancellationToken cancellationToken)
            {
                OnProcessExecuted = true;
                return base.OnProcess(getState, action, dispatcher, cancellationToken);
            }
        }

        public class PoisonPillProcessor : LogicBase<AppState, PoisonPill>
        {
            public override Type CancelType => null;

            public override uint Priority => 0;

            public bool OnProcessExecuted { get; private set; }

            protected override Task OnProcess(Func<AppState> getState, PoisonPill action, IMultiDispatcher dispatcher, CancellationToken cancellationToken)
            {
                OnProcessExecuted = true;
                return base.OnProcess(getState, action, dispatcher, cancellationToken);
            }
        }
    }
}
