﻿namespace TcbInternetSolutions.Vulcan.Core.Extensions
{
    using EPiServer.Core;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Type extensions
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, List<Type>> ResolvedTypes = new ConcurrentDictionary<Type, List<Type>>();        

        /// <summary>
        /// Get search types for given T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Type> GetSearchTypesFor<T>(Func<Type, bool> filter = null) where T : class, IContent =>
            GetSearchTypesFor(typeof(T), filter);

        /// <summary>
        /// Get search types for given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Type> GetSearchTypesFor(this Type type, Func<Type,bool> filter = null)
        {
            if (!ResolvedTypes.TryGetValue(type, out var allTypesForGiven))
            {
                allTypesForGiven = new List<Type>();

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    allTypesForGiven.AddRange(assembly.GetTypes()
                        .Where(t =>  type.IsAssignableFrom(t) && t.FullName?.EndsWith("Proxy") == false));
                }

                ResolvedTypes.TryAdd(type, allTypesForGiven);
            }

            if (filter != null)
                allTypesForGiven = allTypesForGiven.Where(filter).ToList();

            return allTypesForGiven;
        }

    }
}
