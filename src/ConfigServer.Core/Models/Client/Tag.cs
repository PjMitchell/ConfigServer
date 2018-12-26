using System;
using System.Collections.Generic;

namespace ConfigServer.Core
{
    /// <summary>
    /// Tag applies additional meta data to client
    /// </summary>
    public class Tag : IEquatable<Tag>
    {
        /// <summary>
        /// Value of the tag
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Determines whether this instance and another specified object have the same value.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Tag);
        }

        /// <summary>
        /// Determines whether this instance and another specified Tag have the same value.
        /// </summary>
        /// <param name="other">The tag to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public bool Equals(Tag other)
        {
            return other != null &&
                   Value == other.Value;
        }

        /// <summary>
        /// Returns the hash code for this object.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<string>.Default.GetHashCode(Value);
        }

        /// <summary>
        /// Determines whether two Tags have the same value.
        /// </summary>
        /// <param name="tag1">Tag one</param>
        /// <param name="tag2">Tag two</param>
        /// <returns>true if the values are equal</returns>
        public static bool operator ==(Tag tag1, Tag tag2)
        {
            return EqualityComparer<Tag>.Default.Equals(tag1, tag2);
        }

        /// <summary>
        /// Determines whether two Tags do not have the same value.
        /// </summary>
        /// <param name="tag1">Tag one</param>
        /// <param name="tag2">Tag two</param>
        /// <returns>true if the values are not equal</returns>
        public static bool operator !=(Tag tag1, Tag tag2)
        {
            return !(tag1 == tag2);
        }
    }
}
