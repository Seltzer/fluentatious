using System;

namespace CSharp.Fluency.Extensions.Tests
{

    public sealed class EquatableTestClass : IEquatable<EquatableTestClass>
    {
        public readonly string Key;


        public EquatableTestClass(string key)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            Key = key;
        }


        public bool Equals(EquatableTestClass other)
        {
            return other != null && Key.Equals(other.Key);
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as EquatableTestClass);
        }


        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }

}
