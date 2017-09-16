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
    public class ReduxLogicChainingTests
    {
        [Test]
        public void TestFirstValidationReturnsNull()
        {
            var reducer = new ReducerComposer<AppState>();

            var anyActionLogicMock = new Moq.Mock<ILogic<AppState, AnyAction>>();
            anyActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(null as PreProcessResult);
            anyActionLogicMock.Setup(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()));

            var nextActionLogicMock = new Moq.Mock<ILogic<AppState, AnyAction>>();
            nextActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(null as PreProcessResult);
            IStore<AppState> store = new Store<AppState>(reducer, AppState.InitialState).ConfigureLogic(config =>
            {
                config.AddLogics(anyActionLogicMock.Object);
                config.AddLogics(nextActionLogicMock.Object);
            });
            store.Dispatch(new LoginAction("Bob", "pwd", true));
            nextActionLogicMock.Verify(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>()), Times.Never());
            anyActionLogicMock.Verify(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()), Times.Never());
        }

        [Test]
        public void TestFirstValidationReturnsFalse()
        {
            var reducer = new ReducerComposer<AppState>();

            var loginAction = new LoginAction("", "", true);

            var anyActionLogicMock = new Moq.Mock<ILogic<AppState, AnyAction>>();
            anyActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(new PreProcessResult(false, loginAction));
            anyActionLogicMock.Setup(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()));

            var nextActionLogicMock = new Moq.Mock<ILogic<AppState, AnyAction>>();
            nextActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(null as PreProcessResult);
            IStore<AppState> store = new Store<AppState>(reducer, AppState.InitialState).ConfigureLogic(config =>
            {
                config.AddLogics(anyActionLogicMock.Object);
                config.AddLogics(nextActionLogicMock.Object);
            });
            store.Dispatch(new LoginAction("Bob", "pwd", true));
            nextActionLogicMock.Verify(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>()), Times.Never());
            anyActionLogicMock.Verify(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()), Times.Never());
        }

        [Test]
        public void TestFirstValidationReturnsTrueButSecondReturnsNull()
        {
            var reducer = new ReducerComposer<AppState>();

            var loginAction = new LoginAction("", "", true);

            var anyActionLogicMock = new Moq.Mock<ILogic<AppState, AnyAction>>();
            anyActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(new PreProcessResult(true, loginAction));
            anyActionLogicMock.Setup(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()));

            var nextActionLogicMock = new Moq.Mock<ILogic<AppState, AnyAction>>();
            nextActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(null as PreProcessResult);
            IStore<AppState> store = new Store<AppState>(reducer, AppState.InitialState).ConfigureLogic(config =>
            {
                config.AddLogics(anyActionLogicMock.Object);
                config.AddLogics(nextActionLogicMock.Object);
            });
            store.Dispatch(new LoginAction("Bob", "pwd", true));
            nextActionLogicMock.Verify(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>()), Times.Once());
            anyActionLogicMock.Verify(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()), Times.Never());
        }

        [Test]
        public void TestFirstValidationReturnsTrueButSecondReturnsFalse()
        {
            var reducer = new ReducerComposer<AppState>();

            var loginAction = new LoginAction("", "", true);

            var anyActionLogicMock = new Moq.Mock<ILogic<AppState, AnyAction>>();
            anyActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(new PreProcessResult(true, loginAction));
            anyActionLogicMock.Setup(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()));

            var nextActionLogicMock = new Moq.Mock<ILogic<AppState, AnyAction>>();
            nextActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(new PreProcessResult(false, loginAction));
            IStore<AppState> store = new Store<AppState>(reducer, AppState.InitialState).ConfigureLogic(config =>
            {
                config.AddLogics(anyActionLogicMock.Object);
                config.AddLogics(nextActionLogicMock.Object);
            });
            store.Dispatch(new LoginAction("Bob", "pwd", true));
            nextActionLogicMock.Verify(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>()), Times.Once());
            anyActionLogicMock.Verify(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()), Times.Never());
        }

        [Test]
        public void TestBothValidatorsReturnsTrue()
        {
            var reducer = new ReducerComposer<AppState>();

            var loginAction = new LoginAction("", "", true);

            var anyActionLogicMock = new Moq.Mock<ILogic<AppState, Logic.AnyAction>>();
            anyActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(new PreProcessResult(true, loginAction));
            anyActionLogicMock.Setup(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()));

            var nextActionLogicMock = new Moq.Mock<ILogic<AppState, AnyAction>>();
            nextActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(new PreProcessResult(true, loginAction));
            nextActionLogicMock.Setup(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()));
            IStore<AppState> store = new Store<AppState>(reducer, AppState.InitialState).ConfigureLogic(config =>
            {
                config.AddLogics(anyActionLogicMock.Object);
                config.AddLogics(nextActionLogicMock.Object);
            });
            store.Dispatch(new LoginAction("Bob", "pwd", true));
            nextActionLogicMock.Verify(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>()), Times.Once());
            nextActionLogicMock.Verify(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()), Times.Once());
            anyActionLogicMock.Verify(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()), Times.Once());
        }

        public class AppState
        {
            public static AppState InitialState => new AppState();
        }

        public class LoginAction
        {
            public string UserName { get; }
            public string Password { get; }
            public bool RememberMe { get; }
            public LoginAction(string username, string password, bool rememberMe)
            {
                UserName = username;
                Password = password;
                RememberMe = rememberMe;
            }
        }
    }
}
