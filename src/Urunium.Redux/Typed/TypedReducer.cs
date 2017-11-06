using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Typed
{
    public class TypedReducer<TState> : IReducer<TState>
    {
        Func<TState, object, TState> _applyFunction;
        public TypedReducer()
        {
            _applyFunction = GenerateApplyFunction();
        }

        public TState Apply(TState previousState, object action)
        {
            return _applyFunction(previousState, action);
        }


        private Func<TState, object, TState> GenerateApplyFunction()
        {
            // compile into:
            // (previousState, action) => 
            // { 
            //      if(action is Add)
            //          return Apply(previousState, (Add)action); 
            //      if(action is Subtract)
            //          return Apply(previousState, (Subtract)action); 
            //   ...
            //      return previousState; 
            // }
            ParameterExpression p1 = Expression.Parameter(typeof(TState), "previousState");
            ParameterExpression p2 = Expression.Parameter(typeof(object), "action");
            LabelTarget returnTarget = Expression.Label(typeof(TState));
            var applyMethods = GetApplyMethods();
            List<Expression> blockStatements = new List<Expression>(applyMethods.Count + 1);
            foreach (var item in applyMethods)
            {
                var methodInfo = item.FirstOrDefault();
                if (methodInfo == null)
                    continue;
                var actionType = item.Key;
                var callAction = Expression.Call(
                                    Expression.Constant(this),
                                    methodInfo,
                                    p1,
                                    Expression.Convert(p2, actionType));

                Expression returnExpr = Expression.Return(returnTarget, callAction);

                blockStatements.Add(Expression.IfThen(Expression.TypeEqual(p2, actionType), returnExpr));
            }
            blockStatements.Add(Expression.Label(returnTarget, p1));
            var block = Expression.Block(blockStatements);
            var lamda = Expression.Lambda<Func<TState, object, TState>>(block, p1, p2);
            return lamda.Compile();
        }

        private ILookup<Type, MethodInfo> GetApplyMethods()
        {
            var methods = this.GetType().GetTypeInfo().DeclaredMethods;
            return (from m in methods
                    let Params = m.GetParameters()
                    where
                       m.Name == "Apply"
                       && m.ReturnType == typeof(TState)
                       && Params.Length == 2
                       && Params[0].ParameterType == typeof(TState)
                    select new { Type = Params[1].ParameterType, Method = m }).ToLookup(x => x.Type, x => x.Method);
        }
    }
}
