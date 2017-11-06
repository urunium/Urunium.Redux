using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows;
using Urunium.Redux;
using Urunium.Redux.Compose;
using Urunium.Redux.Logic;
using UruniumWPFExample.Fx.CustomTriggers;
using UruniumWPFExample.Logics;
using UruniumWPFExample.Reducers;
using UruniumWPFExample.States;

namespace UruniumWPFExample.Fx
{
    /// <summary>
    /// Bootstrapper for caliburn-micro application.
    /// </summary>
    public class TodoMvcBootstrapper : BootstrapperBase
    {
        private SimpleContainer _container;

        /// <summary>
        /// Create an instance of TodoMvcBootstrapper.
        /// </summary>
        public TodoMvcBootstrapper()
        {
            _container = new SimpleContainer();

            Initialize();
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void Configure()
        {
            AddCustomEventTriggers();

            _container.Singleton<IWindowManager, WindowManager>();

            var rootReducer = new ReducerComposer<TodoList>()
                                .AddSubTreeReducer(new TodoItemsReducer())
                                .AddSubTreeReducer(new EditingTodoItemsReducer())
                                .AddSubTreeReducer(new VisibilityFilterReducer());

            IStore<TodoList> Store = (new Store<TodoList>(rootReducer, TodoList.InitialState)).ConfigureLogic(config =>
            {
                config.AddLogics(new UpdateTodoTextHandler());
                config.AddLogics(new AddTodoHandler());
            });

            _container.Instance(Store);

            _container.PerRequest<IShell, ShellViewModel>();
            _container.PerRequest<TodoListAppViewModel>();
            _container.PerRequest<TodoItemViewModel>();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            dynamic settings = new System.Dynamic.ExpandoObject();
            settings.Height = 700;
            settings.Width = 600;
            settings.SizeToContent = SizeToContent.Manual;
            settings.Title = "Todo MVC";
            DisplayRootViewFor<IShell>(settings);
        }

        private void AddCustomEventTriggers()
        {
            var defaultCreateTrigger = Parser.CreateTrigger;

            Parser.CreateTrigger = (target, triggerText) =>
            {
                if (triggerText == null)
                {
                    return defaultCreateTrigger(target, null);
                }

                var triggerDetail = triggerText
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);

                var splits = triggerDetail.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                switch (splits[0])
                {
                    case "Key":
                        var key = (System.Windows.Input.Key)Enum.Parse(typeof(System.Windows.Input.Key), splits[1], true);
                        return new KeyTrigger { Key = key };

                    case "Gesture":
                        var mkg = (MultiKeyGesture)(new MultiKeyGestureConverter()).ConvertFrom(splits[1]);
                        return new KeyTrigger { Modifiers = mkg.KeySequences[0].Modifiers, Key = mkg.KeySequences[0].Keys[0] };

                    case "DblClick":
                        return new DblClickTrigger();
                }

                return defaultCreateTrigger(target, triggerText);
            };
        }
    }
}
