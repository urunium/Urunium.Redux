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
    public class ReduxLogicLongRunningProcessTests
    {
        [Test]
        public void TestLongRunningProcess()
        {
            var reducer = new ReducerComposer<AppState>();

            var anyActionLogicMock = new Moq.Mock<ILogic<AppState, ListenInternetConnection>>();
            anyActionLogicMock.SetupGet(x => x.IsLongRunning).Returns(true);
            anyActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<ListenInternetConnection>())).ReturnsAsync(new PreProcessResult(true, new ListenInternetConnection()));
            anyActionLogicMock.Setup(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<ListenInternetConnection>(), It.IsAny<IMultiDispatcher>())).Callback(() => 
            {
                // Basically hangs if IsLongRunning is set to false.
                Task.Delay(TimeSpan.MaxValue).Wait();
            });

            var nextActionLogicMock = new Moq.Mock<ILogic<AppState, AnyAction>>();
            nextActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(new PreProcessResult(true, new ListenInternetConnection()));
            IStore<AppState> store = new Store<AppState>(reducer, AppState.InitialState).ConfigureLogic(config =>
            {
                config.AddLogics(anyActionLogicMock.Object);
                config.AddLogics(nextActionLogicMock.Object);
            });
            store.Dispatch(new ListenInternetConnection());
            nextActionLogicMock.Verify(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>()), Times.Once());
            anyActionLogicMock.Verify(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<ListenInternetConnection>(), It.IsAny<IMultiDispatcher>()), Times.Once());
        }

        public class AppState
        {
            public static AppState InitialState => new AppState();
        }

        public class ListenInternetConnection
        {
        }
    }
}
