using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Urunium.Redux.Logic;

namespace Urunium.Redux.Tests
{
    [TestFixture]
    public class LogicBaseTest
    {
        [Test]
        public void TestTransfrom()
        {
            LogicImpl logic = new LogicImpl();
            Moq.Mock<IStore<AppState>> store = new Moq.Mock<IStore<AppState>>();

            logic.OnTransform = (s, a) => Task.FromResult(a + "World");
            var result = ((ILogic<AppState, string>)logic).PreProcess(store.Object, "Hello");
            Assert.AreEqual("HelloWorld", result.Result.Action);
        }

        [Test]
        public void TestValidationFailure()
        {
            LogicImpl logic = new LogicImpl();
            Moq.Mock<IStore<AppState>> store = new Moq.Mock<IStore<AppState>>();
            store.Setup(s => s.Dispatch(It.IsAny<ValidationException>()));

            logic.OnValidation = (s, a, d) => Task.FromResult(ValidationResult.Failure(new ValidationException("", new[] { new ValidationDetails("", "") })));
            var result = ((ILogic<AppState, string>)logic).PreProcess(store.Object, "Hello");
            Assert.AreEqual(false, result.Result.ContinueToNextStep);
            store.Verify(s => s.Dispatch(It.IsAny<ValidationException>()), Times.Once());
        }

        [Test]
        public void TestValidationSuccess()
        {
            LogicImpl logic = new LogicImpl();
            Moq.Mock<IStore<AppState>> store = new Moq.Mock<IStore<AppState>>();
            store.Setup(s => s.Dispatch(It.IsAny<ValidationException>()));

            logic.OnValidation = (s, a, d) => Task.FromResult(ValidationResult.Success());
            var result = ((ILogic<AppState, string>)logic).PreProcess(store.Object, "Hello");
            Assert.AreEqual(true, result.Result.ContinueToNextStep);
            store.Verify(s => s.Dispatch(It.IsAny<ValidationException>()), Times.Never());
        }

        [Test]
        public void TestTransformWithValidationSuccess()
        {
            LogicImpl logic = new LogicImpl();
            Moq.Mock<IStore<AppState>> store = new Moq.Mock<IStore<AppState>>();
            store.Setup(s => s.Dispatch(It.IsAny<ValidationException>()));

            logic.OnTransform = (s, a) => Task.FromResult(a + "World");
            logic.OnValidation = (s, a, d) => Task.FromResult(ValidationResult.Success());
            var result = ((ILogic<AppState, string>)logic).PreProcess(store.Object, "Hello");
            Assert.AreEqual(true, result.Result.ContinueToNextStep);
            Assert.AreEqual("HelloWorld", result.Result.Action);
            store.Verify(s => s.Dispatch(It.IsAny<ValidationException>()), Times.Never());
        }

        [Test]
        public void TestTransfromAndValidationFailure()
        {
            LogicImpl logic = new LogicImpl();
            Moq.Mock<IStore<AppState>> store = new Moq.Mock<IStore<AppState>>();
            store.Setup(s => s.Dispatch(It.IsAny<ValidationException>()));

            logic.OnTransform = (s, a) => Task.FromResult(a + "World");
            logic.OnValidation = (s, a, d) => Task.FromResult(ValidationResult.Failure(new ValidationException("", new[] { new ValidationDetails("", "") })));
            var result = ((ILogic<AppState, string>)logic).PreProcess(store.Object, "Hello");
            Assert.AreEqual(false, result.Result.ContinueToNextStep);
            Assert.AreEqual("HelloWorld", result.Result.Action);
            store.Verify(s => s.Dispatch(It.IsAny<ValidationException>()), Times.Once());
        }

        public class AppState { }

        public class LogicImpl : LogicBase<AppState, string>
        {
            public override uint Priority => 1;
            public override Type CancelType => null;

            public Func<Func<AppState>, string, Task<string>> OnTransform { get; set; }
            public Func<Func<AppState>, string, IMultiDispatcher, Task<ValidationResult>> OnValidation { get; set; }

            protected override Task<ValidationResult> OnValidate(Func<AppState> getState, string action, IMultiDispatcher dispatcher, CancellationToken token)
            {
                return OnValidation?.Invoke(getState, action, dispatcher) ?? Task.FromResult(ValidationResult.Failure(new ValidationException("No handler", new[] { new ValidationDetails("", "") })));
            }

            protected override Task<string> OnTransfrom(Func<AppState> getState, string action, CancellationToken token)
            {
                return OnTransform?.Invoke(getState, action) ?? Task.FromResult(action);
            }
        }
    }
}
