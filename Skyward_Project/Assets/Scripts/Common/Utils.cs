using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Skyward.Utils
{
    public static class ReflectionHelper
    {
        private static Dictionary<Assembly, List<string>> assemblyReferences = new();

        public static IEnumerable<Type> GetDerivedTypes<TBase>() where TBase : class
        {
            // Get all assemblies currently loaded
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // List to hold derived types
            List<Type> derivedTypes = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
                // Get all types in the assembly
                Type[] types = assembly.GetTypes();

                // Find types that are a subclass of TBase and not abstract
                derivedTypes.AddRange(types.Where(t => t.IsSubclassOf(typeof(TBase)) && !t.IsAbstract));
            }

            return derivedTypes;
        }

        public static IEnumerable<(Type type, T attribute)> AllTypesWithAttribute<T>() where T : Attribute
        {
            Assembly typeAssembly = Assembly.GetAssembly(typeof(T));
            foreach (var assembly in ReflectionHelper.AllAssembliesReferencing(typeAssembly))
            {
                foreach (var type in assembly.DefinedTypes)
                {
                    Attribute attribute = type.GetCustomAttribute(typeof(T));
                    if (attribute != null)
                        yield return (type, (T)attribute);
                }
            }
        }

        public static IEnumerable<Assembly> AllAssembliesReferencing(Assembly assembly)
        {
            foreach (Assembly other in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (other.ReferencesAssembly(assembly))
                {
                    yield return other;
                }
            }
        }

        public static bool ReferencesAssembly(this Assembly assembly, Assembly other)
        {
            if (!assemblyReferences.TryGetValue(assembly, out List<string> referencedAssemblies))
            {
                referencedAssemblies = new List<string>();
                CacheAssemblyReference(ref referencedAssemblies, assembly);
                assemblyReferences[assembly] = referencedAssemblies;
            }

            return referencedAssemblies.Contains(other.GetName().FullName);
        }

        private static void CacheAssemblyReference(ref List<string> referencedAssemblies, Assembly targetAssembly)
        {
            // Assumes self references
            referencedAssemblies.Add(targetAssembly.GetName().FullName);

            var asses = targetAssembly.GetReferencedAssemblies();
            foreach (AssemblyName assName in asses)
            {
                referencedAssemblies.Add(assName.FullName);
            }
        }
    }

    public static class GameObjectExtensions
    {
        public static Transform GetRoot(this Transform target)
        {
            Transform current = target;
            while (current.parent != null)
                current = current.parent;

            return current;
        }

        public static Transform GetRoot(this GameObject target) => GetRoot(target.transform);
        
        public static bool TryGetComponentInParent<T>(this Transform target, out T comp) where T : Component
        {
            comp = null;
            Transform current = target;
            while (current != null)
            {
                Transform root = current;
                current = current.parent;
                if (root.TryGetComponent(out comp))
                    return true;
            }

            return false;
        }
    }
}