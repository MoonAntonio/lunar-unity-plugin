//
//  CClassUtils.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2016 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using LunarPlugin;

using UnityEngine;

namespace LunarPluginInternal
{
    delegate bool CMethodsFilter(MethodInfo method);

    static class CClassUtils
    {
        public static T Cast<T>(object obj) where T : class
        {
            CAssert.IsNotNull(obj);
            CAssert.IsInstanceOfType<T>(obj);

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
                CLog.error(e, "Can't create instance of type: " + t);
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

        public static List<MethodInfo> ListInstanceMethods(Type type, CMethodsFilter filter)
        {
            return ListInstanceMethods(new List<MethodInfo>(), type, filter);
        }

        public static List<MethodInfo> ListInstanceMethods(List<MethodInfo> outList, Type type, CMethodsFilter filter)
        {
            return ListMethods(outList, type, filter, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static List<MethodInfo> ListMethods(List<MethodInfo> outList, Type type, CMethodsFilter filter, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            CAssert.IsNotNull(type, "Type is null");

            MethodInfo[] methods = type.GetMethods(flags);

            if (filter == null)
            {
                outList.AddRange(methods);
                return outList;
            }

            foreach (MethodInfo m in methods)
            {
                if (filter(m))
                {
                    outList.Add(m);
                }
            }
            return outList;
        }

        public static bool ShouldListMethod(MethodInfo m, string prefix)
        {
            return CStringUtils.StartsWithIgnoreCase(m.Name, prefix);
        }

        public static T GetObjectField<T>(object target, string name)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
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
                throw new ArgumentNullException("typeName");
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
