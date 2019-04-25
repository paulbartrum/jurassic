using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jurassic.Library
{
    internal static class TaskAwaiterCache
    {
        private readonly struct Key : IEquatable<Key>
        {
            public readonly RuntimeTypeHandle Generic;
            public readonly RuntimeTypeHandle Actual;

            public Key(RuntimeTypeHandle generic, RuntimeTypeHandle actual)
            {
                Generic = generic;
                Actual = actual;
            }

            public override bool Equals(object obj)
            {
                return obj is Key key && Equals(key);
            }

            public bool Equals(Key other)
            {
                return Generic.Equals(other.Generic) &&
                       Actual.Equals(other.Actual);
            }

            public override int GetHashCode()
            {
                var hashCode = -817328319;
                hashCode = hashCode * -1521134295 + EqualityComparer<RuntimeTypeHandle>.Default.GetHashCode(Generic);
                hashCode = hashCode * -1521134295 + EqualityComparer<RuntimeTypeHandle>.Default.GetHashCode(Actual);
                return hashCode;
            }
        }

        private static readonly Expression undefined = Expression.Convert(Expression.Constant(Undefined.Value), typeof(object));
        private static readonly ConcurrentDictionary<Key, Delegate> getResults = new ConcurrentDictionary<Key, Delegate>();

        public static object GetResult<T>(T notify)
            where T : INotifyCompletion
        {
            var key = new Key(typeof(T).TypeHandle, Type.GetTypeHandle(notify));
            var del = getResults.GetOrAdd(key, CreateGetResult<T>);
            var typed = (Func<T, object>)del;
            return typed(notify);
        }

        private static Func<T, object> CreateGetResult<T>(Key key)
        {
            var type = Type.GetTypeFromHandle(key.Actual);
            var getResultMethod = type.GetMethod("GetResult", BindingFlags.Public | BindingFlags.Instance);

            var parameter = Expression.Parameter(typeof(T), "notify");
            var convertedParameter = Expression.Convert(parameter, type);
            var body = new List<Expression>(2);

            if (getResultMethod == null)
            {
                body.Add(undefined);
            }
            else
            {
                var invoke = Expression.Call(convertedParameter, getResultMethod);

                if (getResultMethod.ReturnType == typeof(void))
                {
                    body.Add(invoke);
                    body.Add(undefined);
                }
                else
                {
                    body.Add(Expression.Convert(invoke, typeof(object)));
                }
            }

            Expression block = Expression.Block(body);
            while (block.CanReduce) block = block.ReduceAndCheck();

            var lambda = Expression.Lambda<Func<T, object>>(block, parameter);
            return lambda.Compile();
        }
    }
}
