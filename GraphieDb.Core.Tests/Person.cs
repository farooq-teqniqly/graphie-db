using System;

namespace GraphieDb.Core.Tests
{
    internal class Person
    {
        public string Name { get; set; }
        public DateTime HireDate { get; set; }

        protected bool Equals(Person other)
        {
            return Name == other.Name && HireDate.Equals(other.HireDate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Person) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, HireDate);
        }
    }
}