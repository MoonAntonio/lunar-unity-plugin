﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using LunarPlugin;

using UnityEngine;

namespace LunarPluginInternal
{
    delegate bool ListMethodsFilter(MethodInfo method);

    static class ClassUtils
    {
        public static T Cast<T>(object obj) where T : class
        {
            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType<T>(obj);

            return obj as T;
        }

        public static T TryCast<T>(object obj) where T : class
        {
            return obj as T;
        }

        public static T CreateInstance<T>(Type t, params object[] args) where T : class
        {
            try
            {
                return (T) Activator.CreateInstance(t, args);
            }
            catch (Exception e)
            {
                Log.error(e, "Can't create instance of type: " + t);
            }

            return null;
        }

        public static bool IsValidEnumValue<T>(int value)
        {
            return Enum.IsDefined(typeof(T), value);
        }

        public static bool IsValidEnumValue<T>(T value)
        {
            return Enum.IsDefined(typeof(T), value);
        }

        public static string TypeShortName(Type type)
        {
            if (type != null)
            {
                if (type == typeof(int)) return "int";
                if (type == typeof(float)) return "float";
                if (type == typeof(string)) return "string";
                if (type == typeof(long)) return "long";
                if (type == typeof(bool)) return "bool";

                return type.Name;
            }

            return null;
        }

        public static MethodInfo[] ListInstanceMethods(Type type, ListMethodsFilter filter)
        {
            return ListMethods(type, filter, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static MethodInfo[] ListMethods(Type type, ListMethodsFilter filter, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            Assert.IsNotNull(type, "Type is null");

            MethodInfo[] methods = type.GetMethods(flags);

            if (filter == null)
            {
                return methods;
            }

            List<MethodInfo> list = new List<MethodInfo>(methods.Length);
            foreach (MethodInfo m in methods)
            {
                if (filter(m))
                {
                    list.Add(m);
                }
            }
            return list.ToArray();
        }

        public static bool ShouldListMethod(MethodInfo m, string prefix)
        {
            return StringUtils.StartsWithIgnoreCase(m.Name, prefix);
        }

        public static T GetObjectField<T>(object target, string name)
        {
            if (target == null)
            {
                throw new ArgumentNullException("Target is null");
            }

            FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                if (field.Name == name)
                {
                    return (T)field.GetValue(target);
                }
            }

            throw new ArgumentException("Can't find field: " + name);
        }

        public static Type TypeForName(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("Type name is null");
            }

            try
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (type.FullName == typeName)
                            {
                                return type;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }
    }
}
