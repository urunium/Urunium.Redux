using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Compose
{
    /// <summary>
    /// Helps adapt a subtree reducer into full state tree reducer.
    /// </summary>
    /// <typeparam name="TState">Type of application state</typeparam>
    /// <typeparam name="TPart">Type of property which subtree reduce works with.</typeparam>
    public class SubTreeToFullTreeAdapter<TState, TPart> : IReducer<TState>
    {
        private ISubTreeReducer<TState, TPart> _subTreeReducer;
        private Func<TState, TPart> _getter;
        private Func<TState, TPart, TState> _setter;

        /// <summary>
        /// Constructor for SubTreeToFullTreeAdapter
        /// </summary>
        /// <param name="subTreeReducer">Instance of subtree reducer that needs to adapt.</param>
        public SubTreeToFullTreeAdapter(ISubTreeReducer<TState, TPart> subTreeReducer)
        {
            _subTreeReducer = subTreeReducer;
            var propertySelector = _subTreeReducer.PropertySelector;
            var memberExpr = propertySelector.Body as MemberExpression ?? throw new ArgumentException(string.Format(
                    "Expression '{0}' should be a field.",
                    propertySelector.ToString()));
            MemberInfo memberInfo = memberExpr.Member;
            CompileGetterFunction(propertySelector, memberExpr);
            CompileSetterFunction(memberInfo);
            if (_setter is null)
            {
                throw new Exception("TState must provide an Immutable constructor, or public property setter");
            }
        }

        /// <summary>
        /// Apply subtree reducer and adapt to application state
        /// </summary>
        /// <param name="previousState">Old state of property</param>
        /// <param name="action">Action to be applied</param>
        /// <returns>New state of property</returns>
        public TState Apply(TState previousState, object action)
        {
            var prevPropertyValue = _getter(previousState);
            var newPropertyValue = _subTreeReducer.Apply(prevPropertyValue, action);
            return _setter(previousState, newPropertyValue);
        }

        private void CompileGetterFunction(Expression<Func<TState, TPart>> propertySelector, MemberExpression memberExpr)
        {
            // compiles to something similar to : (state) => state.Prop1;
            _getter = propertySelector.Compile();
        }

        private void CompileSetterFunction(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo propInfo && (propInfo.SetMethod?.IsPublic).GetValueOrDefault() || memberInfo is FieldInfo fieldInfo && fieldInfo.IsPublic)
            {
                CompileSetterForMutableObject(memberInfo);
                return;
            }

            CompileSetterForImmutableObject(memberInfo);
        }

        private void CompileSetterForMutableObject(MemberInfo memberInfo)
        {
            // compiles to something similar to: (state, value) => { state.Prop = value; return state; }

            // `state` variable in (state, value)
            ParameterExpression targetExp = Expression.Parameter(typeof(TState), "state");
            // `value` variable in (state, value)
            ParameterExpression valueExp = Expression.Parameter(typeof(TPart), "value");
            // Property name which needs to be assigned value (e.g. state.Prop1)
            MemberExpression fieldExp = Expression.PropertyOrField(targetExp, memberInfo.Name);
            // Assignment (e.g: state.Prop1 = value)
            BinaryExpression assignExp = Expression.Assign(fieldExp, valueExp);

            // Combine above parts to form full method body with return statement
            // e.g. (state, value) => { state.Prop = value; return state; }
            LabelTarget returnTarget = Expression.Label(typeof(TState));
            Expression returnExpr = Expression.Return(returnTarget, targetExp);
            var blockExpr = Expression.Block(
                    assignExp,
                    returnExpr,
                    Expression.Label(returnTarget, targetExp));

            // Compile expression tree to executible function.
            var setterLamda = Expression.Lambda<Func<TState, TPart, TState>>(
                blockExpr, targetExp, valueExp
            );
            _setter = setterLamda.Compile();
            return;
        }

        private void CompileSetterForImmutableObject(MemberInfo memberInfo)
        {
            // compiles into something similar to : (state, value) => { var nextState = new State(state.Prop1, value); return nextState; }
            // Immutable constructor consist of parameter for each public property of that class.
            // The parameter is identfied by it's name and data-type, hence name must be same as public property.
            // e.g:
            // class Imut 
            // { 
            //  public string Prop1{get;} 
            //  public string Prop2{get;}
            //  public Imut(string prop1, string prop2)
            //  { 
            //  }
            // }
            // Note: comparision for property name and constructor argument name is case insensitive.
            var constructors = typeof(TState).GetTypeInfo().DeclaredConstructors.ToArray();
            if (constructors.Length > 0)
            {
                var properties = typeof(TState).GetRuntimeProperties();
                ConstructorInfo immutableConstructorInfo = null;
                Dictionary<string, ParameterInfo> constructorParameters = null;
                List<Expression> parameterExpressions = new List<Expression>();
                // `state` variable in (state, value)
                ParameterExpression targetExp = Expression.Parameter(typeof(TState), "state");
                // `value` variable in (state, value)
                ParameterExpression valueExp = Expression.Parameter(typeof(TPart), "value");
                foreach (var ctor in constructors)
                {
                    immutableConstructorInfo = ctor;
                    constructorParameters = ctor.GetParameters().ToDictionary(x => x.Name.ToLower());

                    foreach (var property in properties)
                    {
                        if (property.GetMethod.IsStatic)
                        {
                            continue;
                        }

                        if (!(constructorParameters.TryGetValue(property.Name.ToLower(), out ParameterInfo pinfo) && pinfo.ParameterType == property.PropertyType))
                        {
                            immutableConstructorInfo = null;
                            parameterExpressions.Clear();
                            break;
                        }
                        else
                        {
                            if (string.Compare(pinfo.Name, memberInfo.Name, StringComparison.CurrentCultureIgnoreCase) == 0 && pinfo.ParameterType == typeof(TPart))
                            {
                                parameterExpressions.Add(valueExp);
                                continue;
                            }
                            MemberExpression fieldExp = Expression.PropertyOrField(targetExp, pinfo.Name);
                            parameterExpressions.Add(fieldExp);
                        }
                    }
                    if (immutableConstructorInfo != null)
                        break;
                }

                if (immutableConstructorInfo != null)
                {
                    var constructorCallExpr = Expression.New(immutableConstructorInfo, parameterExpressions);
                    var nextStateParam = Expression.Parameter(typeof(TState), "nextState");
                    LabelTarget returnTarget = Expression.Label(typeof(TState));

                    Expression returnExpr = Expression.Return(returnTarget, nextStateParam);

                    var blockExpr = Expression.Block(
                        new[] { nextStateParam },
                        Expression.Assign(nextStateParam, constructorCallExpr),
                        returnExpr,
                        Expression.Label(returnTarget, nextStateParam));

                    var setterLamda = Expression.Lambda<Func<TState, TPart, TState>>(
                        blockExpr, targetExp, valueExp
                    );
                    _setter = setterLamda.Compile();
                }
            }
        }
    }
}
