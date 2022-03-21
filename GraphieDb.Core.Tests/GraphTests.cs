using System;
using System.Linq;
using FluentAssertions;
using GraphieDb.Core.Exceptions;
using Xunit;

namespace GraphieDb.Core.Tests
{
    public class GraphTests
    {
        private readonly Graph<string, Person> personDb;

        public GraphTests()
        {
            this.personDb = new Graph<string, Person>();
        }

        [Fact]
        public void Can_Find_Node()
        {
        
            var person = new Vertex<string, Person>(
                "person123", 
                new Person { Name = "Farooq", HireDate = DateTime.Parse("3-21-2019") });

            personDb.Add(person);

            var actualNode = personDb.Find(person.Key);

            actualNode.Should().Be(person);

        }

        [Fact]
        public void Inserting_Duplicate_Key_Throws()
        {
            personDb.Add(new Vertex<string, Person>(
                "person123",
                new Person {Name = "Farooq", HireDate = DateTime.Parse("3-21-2019")}));

            Action action = () =>
            {
                personDb.Add(new Vertex<string, Person>(
                    "person123",
                    new Person {Name = "Bubba", HireDate = DateTime.Parse("3-21-2019")}));
            };

            action.Should()
                .Throw<GraphieDbException>()
                .WithMessage("The vertex with key 'person123' already exists.");
        }

        [Fact]
        public void Find_When_Node_Not_Found_Returns_Null()
        {
            this.personDb.Find("foo").Should().BeNull();
        }

        [Fact]
        public void Can_Connect_Two_Verticies()
        {
            personDb.Add(new Vertex<string, Person>(
                "person123",
                new Person { Name = "Farooq", HireDate = DateTime.Parse("3-21-2019") }));

            personDb.Add(new Vertex<string, Person>(
                "person456",
                new Person { Name = "Bubba", HireDate = DateTime.Parse("6-17-2020") }));

            personDb.Connect(this.personDb.Find("person123"), this.personDb.Find("person456"));

            var edges = personDb.GetEdges("person123");

            edges.Count().Should().Be(1);
        }

        [Fact]
        public void GetEdges_When_Vertex_Not_Connected_Returns_Null()
        {
            personDb.Add(new Vertex<string, Person>(
                "person123",
                new Person { Name = "Farooq", HireDate = DateTime.Parse("3-21-2019") }));

            var edges = personDb.GetEdges("person123");

            edges.Count().Should().Be(0);
        }

    }

    class Person
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
