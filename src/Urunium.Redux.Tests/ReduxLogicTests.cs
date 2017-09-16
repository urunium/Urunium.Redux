using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Urunium.Redux.Compose;
using Urunium.Redux.Logic;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class ReduxLogicTests
    {
        [Test]
        public void TestConfigureLogic()
        {
            var reducer = new ReducerComposer<AppState>();

            var loginActionLogicMock = new Moq.Mock<ILogic<AppState, LoginAction>>();
            loginActionLogicMock.Setup(x => x.IsLongRunning).Returns(false);
            loginActionLogicMock.Setup(x => x.Priority).Returns(1);
            var loginResult = new PreProcessResult(true, new LoginAction("", "", true));
            loginActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<LoginAction>())).ReturnsAsync(loginResult);
            loginActionLogicMock.Setup(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<LoginAction>(), It.IsAny<IMultiDispatcher>()));

            IStore<AppState> store = new Store<AppState>(reducer, AppState.InitialState).ConfigureLogic(config =>
            {
                config.AddLogics(loginActionLogicMock.Object);
            });
            store.Dispatch(new LoginAction("Bob", "pwd", true));
            loginActionLogicMock.Verify(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<LoginAction>()), Times.Once());
            loginActionLogicMock.Verify(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<LoginAction>(), It.IsAny<IMultiDispatcher>()), Times.Once());
        }

        [Test]
        public void TestConfigureAnyActionLogic()
        {
            var reducer = new ReducerComposer<AppState>();

            var loginAction = new LoginAction("", "", true);

            var anyActionLogicMock = new Moq.Mock<ILogic<AppState, AnyAction>>();
            anyActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(new PreProcessResult(true, loginAction));
            anyActionLogicMock.Setup(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()));
            IStore<AppState> store = new Store<AppState>(reducer, AppState.InitialState).ConfigureLogic(config =>
            {
                config.AddLogics(anyActionLogicMock.Object);
            });
            store.Dispatch(new LoginAction("Bob", "pwd", true));
            anyActionLogicMock.Verify(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()), Times.Once());
        }

        [Test]
        public void TestConfigureMixedActionAndAnyActionLogic()
        {
            var reducer = new ReducerComposer<AppState>();

            var loginActionLogicMock = new Moq.Mock<ILogic<AppState, LoginAction>>();
            loginActionLogicMock.Setup(x => x.IsLongRunning).Returns(false);
            loginActionLogicMock.Setup(x => x.Priority).Returns(1);
            var loginResult = new PreProcessResult(true, new LoginAction("", "", true));
            loginActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<LoginAction>())).ReturnsAsync(loginResult);
            loginActionLogicMock.Setup(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<LoginAction>(), It.IsAny<IMultiDispatcher>()));

            var anyActionLogicMock = new Moq.Mock<ILogic<AppState, AnyAction>>();
            anyActionLogicMock.Setup(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<AnyAction>())).ReturnsAsync(new PreProcessResult(true, loginResult));
            anyActionLogicMock.Setup(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()));
            IStore<AppState> store = new Store<AppState>(reducer, AppState.InitialState).ConfigureLogic(config =>
            {
                config.AddLogics(loginActionLogicMock.Object);
                config.AddLogics(anyActionLogicMock.Object);
            });
            store.Dispatch(new LoginAction("Bob", "pwd", true));
            loginActionLogicMock.Verify(x => x.PreProcess(It.IsAny<IStore<AppState>>(), It.IsAny<LoginAction>()), Times.Once());
            loginActionLogicMock.Verify(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<LoginAction>(), It.IsAny<IMultiDispatcher>()), Times.Once());
            anyActionLogicMock.Verify(x => x.Process(It.IsAny<Func<AppState>>(), It.IsAny<AnyAction>(), It.IsAny<IMultiDispatcher>()), Times.Once());
        }

        [Test]
        public void TestSuccessfullLoginFlow()
        {
            // dispatching LoginAction should have following resulting sequence:
            //  -> LoginStartedAction
            //  -> loginService.Login()
            //  -> LoginSuccessfulAction
            //  -> LoadingFriendListAction
            //  -> friendsService.GetFriends()
            //  -> AddToFriendListAction
            Mock<ILoginService> loginService = new Mock<ILoginService>();
            Mock<IFriendsService> friendsService = new Mock<IFriendsService>();
            Mock<IStore<AppState>> storeMock = new Mock<IStore<AppState>>();

            int callCount = 0;
            var state = AppState.InitialState;
            storeMock.SetupGet(x => x.State).Returns(state);
            storeMock.Setup(x => x.Dispatch(It.IsAny<LoginAction>()));
            storeMock.Setup(x => x.Dispatch(It.IsAny<LoginStartedAction>())).Callback(() => Assert.AreEqual(++callCount, 1));
            loginService.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new LoginResult { Success = true, Token = "123" }).Callback(() => Assert.AreEqual(++callCount, 2));
            storeMock.Setup(x => x.Dispatch(It.IsAny<LoginSuccessfulAction>())).Callback(() => Assert.AreEqual(++callCount, 3));
            storeMock.Setup(x => x.Dispatch(It.IsAny<LoadingFriendListAction>())).Callback(() => Assert.AreEqual(++callCount, 4));
            friendsService.Setup(x => x.GetFriends(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<Friend>()).Callback(() => Assert.AreEqual(++callCount, 5));
            System.Threading.AutoResetEvent waiter = new System.Threading.AutoResetEvent(false);
            storeMock.Setup(x => x.Dispatch(It.IsAny<AddToFriendListAction>())).Callback(() => Assert.AreEqual(++callCount, 6));


            IStore<AppState> store = storeMock.Object.ConfigureLogic(config =>
            {
                config.AddLogics(new LoginLogic(loginService.Object, friendsService.Object));
            });
            store.Dispatch(new LoginAction("Bob", "pwd", true));

            loginService.Verify(x => x.Login(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            storeMock.Verify(x => x.Dispatch(It.IsAny<LoginStartedAction>()));
            storeMock.Verify(x => x.Dispatch(It.IsAny<LoginSuccessfulAction>()));
            storeMock.Verify(x => x.Dispatch(It.IsAny<LoadingFriendListAction>()));
            friendsService.Verify(x => x.GetFriends(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            storeMock.Verify(x => x.Dispatch(It.IsAny<AddToFriendListAction>()));
        }

        [Test]
        public void TestFailedLoginFlow()
        {
            // dispatching LoginAction should have following resulting sequence:
            //  -> LoginStartedAction
            //  -> loginService.Login()
            //  -> ValidationExceptions
            Mock<ILoginService> loginService = new Mock<ILoginService>();
            Mock<IFriendsService> friendsService = new Mock<IFriendsService>();
            Mock<IStore<AppState>> storeMock = new Mock<IStore<AppState>>();

            int callCount = 0;
            var state = AppState.InitialState;
            storeMock.SetupGet(x => x.State).Returns(state);
            storeMock.Setup(x => x.Dispatch(It.IsAny<LoginAction>()));
            storeMock.Setup(x => x.Dispatch(It.IsAny<LoginStartedAction>())).Callback(() => Assert.AreEqual(++callCount, 1));
            loginService.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new LoginResult { Success = false }).Callback(() => Assert.AreEqual(++callCount, 2));
            storeMock.Setup(x => x.Dispatch(It.IsAny<ValidationException>())).Callback(() => Assert.AreEqual(++callCount, 3));
            storeMock.Setup(x => x.Dispatch(It.IsAny<LoadingFriendListAction>())).Callback(() => Assert.AreEqual(++callCount, 4));
            friendsService.Setup(x => x.GetFriends(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<Friend>()).Callback(() => Assert.AreEqual(++callCount, 5));
            System.Threading.AutoResetEvent waiter = new System.Threading.AutoResetEvent(false);
            storeMock.Setup(x => x.Dispatch(It.IsAny<AddToFriendListAction>())).Callback(() => Assert.AreEqual(++callCount, 6));


            IStore<AppState> store = storeMock.Object.ConfigureLogic(config =>
            {
                config.AddLogics(new LoginLogic(loginService.Object, friendsService.Object));
            });
            store.Dispatch(new LoginAction("Bob", "pwd", true));

            loginService.Verify(x => x.Login(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            storeMock.Verify(x => x.Dispatch(It.IsAny<LoginStartedAction>()));
            storeMock.Verify(x => x.Dispatch(It.IsAny<ValidationException>()));
            storeMock.Verify(x => x.Dispatch(It.IsAny<LoadingFriendListAction>()), Times.Never());
            friendsService.Verify(x => x.GetFriends(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            storeMock.Verify(x => x.Dispatch(It.IsAny<AddToFriendListAction>()), Times.Never());
        }

        public class Friend
        {
            public string Name { get; set; }
        }

        public enum LoginStatus
        {
            UnKnown,
            InProgress,
            Success,
            Failure
        }

        public enum FriendListStatus
        {
            UnKnown,
            InProgress,
            Success,
            Failure
        }

        public class AppState
        {
            public string UserName { get; }
            public string AuthToken { get; }
            public LoginStatus LoginStatus { get; }
            public IReadOnlyList<string> LoginFailureReasons { get; }

            public FriendListStatus FriendListStatus { get; }
            public IReadOnlyList<Friend> Friends { get; }

            public AppState(string userName, string authToken, LoginStatus loginStatus, IReadOnlyList<string> loginFailureReasons, FriendListStatus friendListStatus, IReadOnlyList<Friend> friends)
            {
                UserName = userName;
                AuthToken = authToken;
                LoginStatus = loginStatus;
                LoginFailureReasons = loginFailureReasons;
                FriendListStatus = friendListStatus;
                Friends = friends;
            }

            public static AppState InitialState => new AppState("", "", LoginStatus.UnKnown, new List<string>().AsReadOnly(), FriendListStatus.UnKnown, new List<Friend>().AsReadOnly());
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

        public class LoginSuccessfulAction
        {
            public string UserName { get; }
            public string Token { get; }
            public LoginSuccessfulAction(string username, string token)
            {
                UserName = username;
                Token = token;
            }
        }

        public class LoginStartedAction
        {
        }

        public class LoadingFriendListAction
        {
        }

        public class FriendFailureListAction
        {
        }

        public class AddToFriendListAction
        {
            public IEnumerable<Friend> Friends { get; }
            public AddToFriendListAction(IEnumerable<Friend> friends)
            {
                Friends = friends;
            }
        }

        public class LoginResult
        {
            public bool Success { get; set; }

            public string Token { get; set; }
        }

        public interface ILoginService
        {
            Task<LoginResult> Login(string username, string password);
        }

        public interface IFriendsService
        {
            Task<List<Friend>> GetFriends(string username, string token);
        }

        public class LoginLogic : LogicBase<AppState, LoginAction>
        {
            ILoginService _loginService;
            IFriendsService _friendService;

            public override Type CancelType => null;

            public override uint Priority => 1;

            public LoginLogic(ILoginService service, IFriendsService friendService)
            {
                _loginService = service;
                _friendService = friendService;
            }

            protected override async Task<ValidationResult> OnValidate(Func<AppState> getState, LoginAction action, IMultiDispatcher dispatcher, CancellationToken cancellationToken)
            {
                dispatcher.DispatchImmediate(new LoginStartedAction());

                string authToken = "";
                List<ValidationDetails> validationDetails = new List<ValidationDetails>();
                if (string.IsNullOrWhiteSpace(action.UserName))
                {
                    validationDetails.Add(new ValidationDetails("UserName", "Username cannot be empty"));
                }
                else if (string.IsNullOrWhiteSpace(action.Password))
                {
                    validationDetails.Add(new ValidationDetails("Password", "Password cannot be empty"));
                }
                else
                {
                    var result = await _loginService.Login(action.UserName, action.Password);
                    if (!result.Success)
                    {
                        validationDetails.Add(new ValidationDetails("*", "Invalid username/password"));
                    }
                    authToken = result.Token;
                }

                if (validationDetails.Count > 0)
                {
                    return ValidationResult.Failure(new ValidationException("One or more error occured while loging in.", validationDetails.AsEnumerable()));
                }

                dispatcher.DispatchImmediate(new LoginSuccessfulAction(action.UserName, authToken));
                return ValidationResult.Success();
            }

            protected override async Task OnProcess(Func<AppState> getState, LoginAction action, IMultiDispatcher dispatcher, CancellationToken cancellationToken)
            {
                List<Friend> friends = new List<Friend>();
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    dispatcher.DispatchImmediate(new LoadingFriendListAction());
                    var state = getState();

                    friends = await _friendService.GetFriends(state.UserName, state.AuthToken).AsCancelable(cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    dispatcher.Dispatch(new AddToFriendListAction(friends));
                }
            }
        }
    }

    public static class LogicCancellationExtension
    {
        public static Task<TResult> AsCancelable<TResult>(this Task<TResult> originalTask, CancellationToken cancellationToken)
        {
            return Task.WhenAny(originalTask, cancellationToken.AsTask<TResult>()).Unwrap();
        }

        public static Task<TResult> AsTask<TResult>(this CancellationToken cancellationToken)
        {
            var completionSource = new TaskCompletionSource<TResult>();
            cancellationToken.Register(() => completionSource.TrySetCanceled());
            return completionSource.Task;
        }
    }
}
