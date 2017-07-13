using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Jurassic
{
    internal static class PlatformExtensions
    {
#if !NETSTANDARD1_5
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        public static MethodInfo GetMethodInfo(this Delegate d)
        {
            return d.Method;
        }
#endif

        public static bool HasCustomAttributes<TAttribute>(this ParameterInfo param) where TAttribute : Attribute
        {
#if !NETSTANDARD1_5
            return Attribute.IsDefined(param, typeof(TAttribute));
#else
            return param.GetCustomAttributes(typeof(ParamArrayAttribute), true).Any();
#endif
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo member) where TAttribute : Attribute
        {
            return GetCustomAttribute<TAttribute>(member, false);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo member, bool inherit) where TAttribute : Attribute
        {
            return member.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().FirstOrDefault();
        }

#if NETSTANDARD1_5
        public static ConstructorInfo GetConstructor(this TypeInfo type, BindingFlags flags, object binder, Type[] parameterTypes, object parameterModifiers)
        {
            ConstructorInfo[] constructors = type.GetConstructors(flags).Where(c => c.GetParameters().Count() == parameterTypes.Length).ToArray();
            foreach (ConstructorInfo c in constructors)
            {
                bool ok = true;
                ParameterInfo[] parameters = c.GetParameters();
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    if(parameterTypes[i] != parameters[i].ParameterType)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                    return c; 
            }
            return null;
        }

        public static MethodInfo GetMethod(this TypeInfo type, string name, BindingFlags flags, object binder, Type[] parameterTypes, object parameterModifiers)
        {
            MethodInfo[] constructors = type.GetMethods(flags).Where(c => c.GetParameters().Count() == parameterTypes.Length && c.Name == name).ToArray();
            foreach (MethodInfo m in constructors)
            {
                bool ok = true;
                ParameterInfo[] parameters = m.GetParameters();
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    if (parameterTypes[i] != parameters[i].ParameterType)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                    return m;
            }
            return null;
        }
#endif

    }

#if NETSTANDARD1_5
    internal sealed class StackOverflowException : Exception
    {
        public StackOverflowException(string message) : base(message) { }
    }
#endif
}
