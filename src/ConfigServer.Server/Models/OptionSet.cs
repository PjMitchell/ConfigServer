using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConfigServer.Server
{

    /// <summary>
    /// Collection of available options
    /// </summary>
    /// <typeparam name="TOption">Option Type</typeparam>
    public class OptionSet<TOption> : IOptionSet<TOption>
    {
        private readonly Dictionary<string, TOption> source;
        private readonly Func<TOption, string> descriptionSelector;
        private readonly Func<TOption, string> keySelector;


        internal OptionSet(IEnumerable<TOption> input, Func<TOption, string> keySelector, Func<TOption, string> descriptionSelector)
        {
            source = input.ToDictionary(keySelector);
            this.descriptionSelector = descriptionSelector;
            this.keySelector = keySelector;
        }

        /// <summary>
        /// Gets the element that has the specified key in the set.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The element that has the specified key in the set.</returns>
        public TOption this[string key] => source[key];

        /// <summary>
        /// Gets the count of the set
        /// </summary>
        public int Count => source.Count;

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the set.
        /// </summary>
        public IEnumerable<string> Keys => source.Keys;

        /// <summary>
        /// Gets an enumerable collection that contains the values in the set.
        /// </summary>
        public IEnumerable<TOption> Values => source.Values;

        /// <summary>
        /// Gets an enumerable collection that contains the option descriptions in the set.
        /// </summary>
        public IEnumerable<string> Descriptions => source.Values.Select(descriptionSelector);

        /// <summary>
        /// Gets an enumerable collection that contains the option selections available in the set.
        /// </summary>
        public IEnumerable<OptionSelection> OptionSelections => source.Select(kvp=> new OptionSelection(kvp.Key,descriptionSelector(kvp.Value)));

        /// <summary>
        /// Determines whether the set contains the specified key
        /// </summary>
        /// <param name="key">The key to locate in the set</param>
        /// <returns>true if the set contains an element with the specified key; otherwise, false.</returns>
        public bool ContainsKey(string key) => source.ContainsKey(key);
        
        /// <summary>
        /// Returns an enumerator that iterates through the set
        /// </summary>
        /// <returns>An enumerator that iterates through the set</returns>
        public IEnumerator<KeyValuePair<string, TOption>> GetEnumerator() => source.GetEnumerator();

        /// <summary>
        /// Gets the option associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the option associated with the specified key,
        ///     if the key is found; otherwise, the default option for the type of the value parameter.
        ///     This parameter is passed uninitialized.</param>
        /// <returns>true if the wet contains an element with the specified key; otherwise, false</returns>
        public bool TryGetValue(string key, out TOption value) => source.TryGetValue(key, out value);

        /// <summary>
        /// Tries to Get option from prexisting object. Useful when the option store has been updated a existing configs need to be updated
        /// </summary>
        /// <param name="existingObject">Existing object</param>
        /// <param name="actualValue">Value in store</param>
        /// <returns>Returns true if object has recongisable key else false</returns>
        public bool TryGetValue(object existingObject, out object actualValue)
        {
            
            if (existingObject is TOption && source.TryGetValue(keySelector((TOption)existingObject),out var result))
            {
                actualValue = result;
                return true;
            }
            actualValue = null;
            return false;

        }

        /// <summary>
        /// Gets the description for the option associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="description">When this method returns, contains the description associated with the specified key,
        ///     if the key is found; otherwise, an empty string.
        ///     This parameter is passed uninitialized.</param>
        /// <returns>true if the set contains an element with the specified key; otherwise, false</returns>
        public bool TryGetDescription(string key, out string description)
        {
            TOption value;
            if (source.TryGetValue(key, out value))
            {
                description = descriptionSelector(value);
                return true;
            }
            else
            {
                description = string.Empty;
                return false;
            }
                
        }

        /// <summary>
        /// Gets Description of the option for a given key
        /// </summary>
        /// <param name="key">Key being queried</param>
        /// <returns>Returns Description for the key</returns>
        public string GetDescription(string key) => descriptionSelector(source[key]);

        /// <summary>
        /// Determines if option has a key that is in the set
        /// </summary>
        /// <param name="option">Option being Queried</param>
        /// <returns>Returns true if option has a key in the set, else false</returns>
        public bool OptionKeyInSet(TOption option) => source.ContainsKey(keySelector(option));

        /// <summary>
        /// Determines if option has a key that is in the set
        /// </summary>
        /// <param name="option">Option being Queried</param>
        /// <returns>Returns true if option has a key in the set, else false</returns>
        public bool OptionKeyInSet(object option)
        {
            if (option is TOption)
                return OptionKeyInSet((TOption)option);
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();        

        IEnumerable<object> IOptionSet.Values => Values.Cast<object>();

        bool IOptionSet.TryGetValue(string key, out object value)
        {
            TOption option;
            var result = TryGetValue(key,out option);
            value = option;
            return result;
        }
    }

}
