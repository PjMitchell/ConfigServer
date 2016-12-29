using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    /// <summary>
    /// Collection of Options
    /// </summary>
    public interface IOptionSet
    {
        /// <summary>
        /// Gets an enumerable collection that contains the keys in the set.
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Gets an enumerable collection that contains the values in the set.
        /// </summary>
        IEnumerable<object> Values { get; }

        /// <summary>
        /// Gets an enumerable collection that contains the option descriptions in the set.
        /// </summary>
        IEnumerable<string> Descriptions { get; }

        /// <summary>
        /// Gets an enumerable collection that contains the option selections available in the set.
        /// </summary>
        IEnumerable<OptionSelection> OptionSelections { get; }

        /// <summary>
        /// Gets Description of the option for a given key
        /// </summary>
        /// <param name="key">Key being queried</param>
        /// <returns>Returns Description for the key</returns>
        string GetDescription(string key);

        /// <summary>
        /// Gets the option associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the option associated with the specified key,
        ///     if the key is found; otherwise, the default option for the type of the value parameter.
        ///     This parameter is passed uninitialized.</param>
        /// <returns>true if the wet contains an element with the specified key; otherwise, false</returns>
        bool TryGetValue(string key, out object value);

        /// <summary>
        /// Gets the description for the option associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="description">When this method returns, contains the description associated with the specified key,
        ///     if the key is found; otherwise, an empty string.
        ///     This parameter is passed uninitialized.</param>
        /// <returns>true if the set contains an element with the specified key; otherwise, false</returns>
        bool TryGetDescription(string key, out string description);

        /// <summary>
        /// Determines if option has a key that is in the set
        /// </summary>
        /// <param name="option">Option being Queried</param>
        /// <returns>Returns true if option has a key in the set, else false</returns>
        bool OptionKeyInSet(object option);
    }

    /// <summary>
    /// Collection of Options
    /// </summary>
    public interface IOptionSet<TOption> : IReadOnlyDictionary<string, TOption>, IOptionSet
    {

        /// <summary>
        /// Determines if option has a key that is in the set
        /// </summary>
        /// <param name="option">Option being Queried</param>
        /// <returns>Returns true if option has a key in the set, else false</returns>
        bool OptionKeyInSet(TOption option);
    }

}
