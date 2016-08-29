﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfigServer.Server
{
    /// <summary>
    /// Aids in the population of Configuration Collection
    /// </summary>
    public abstract class CollectionBuilder
    {
        /// <summary>
        /// Collection built by builder 
        /// </summary>
        public abstract object Collection { get; }

        /// <summary>
        /// Adds value to collection being built
        /// </summary>
        /// <param name="value">value to add to collection</param>
        public abstract void Add(object value);

        /// <summary>
        /// Creates a new Item for the collection
        /// </summary>
        /// <returns>Initialized Item</returns>
        public abstract object IntializeNewItem();
    }

    /// <summary>
    /// Aids in the population of Configuration Collection
    /// </summary>
    /// <typeparam name="TConfig">Type of config in the collection</typeparam>
    public class CollectionBuilder<TConfig> : CollectionBuilder where TConfig : new ()
    {
        private readonly ICollection<TConfig> source;

        /// <summary>
        /// Type of collection to be build
        /// </summary>
        /// <param name="collectionType"></param>
        public CollectionBuilder(Type collectionType)
        {
            if (collectionType == typeof(ICollection<TConfig>))
                source = new List<TConfig>();
            else
            {
                var constructor = collectionType.GetConstructor(Type.EmptyTypes);
                source = (ICollection<TConfig>)constructor.Invoke(new object[0]);
            }
        }
        /// <summary>
        /// Collection built by builder 
        /// </summary>
        public override object Collection => source;
        /// <summary>
        /// Adds value to collection being built
        /// </summary>
        /// <param name="value">value to add to collection</param>
        public override void Add(object value) => source.Add((TConfig)value);

        /// <summary>
        /// Creates a new Item for the collection
        /// </summary>
        /// <returns>Initialized Item</returns>
        public override object IntializeNewItem() => new TConfig();
    }
}
