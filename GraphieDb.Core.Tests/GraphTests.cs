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
        
            var person = new Node<string, Person>(
                "person123", 
                new Person { Name = "Farooq", HireDate = DateTime.Parse("3-21-2019") });

            personDb.Add(person);

            var actualNode = personDb.Find(person.Key);

            actualNode.Should().Be(person);

        }

        [Fact]
        public void Inserting_Duplicate_Key_Throws()
        {
            personDb.Add(new Node<string, Person>(
                "person123",
                new Person {Name = "Farooq", HireDate = DateTime.Parse("3-21-2019")}));

            Action action = () =>
            {
                personDb.Add(new Node<string, Person>(
                    "person123",
                    new Person {Name = "Bubba", HireDate = DateTime.Parse("3-21-2019")}));
            };

            action.Should()
                .Throw<GraphieDbException>()
                .WithMessage("The node with key 'person123' already exists.");
        }

        [Fact]
        public void Can_Connect_Two_Verticies()
        {
            var first = new Node<string, Person>(
                "person123",
                new Person { Name = "Farooq", HireDate = DateTime.Parse("3-21-2019") });
            
            personDb.Add(first);

            var second = new Node<string, Person>(
                "person456",
                new Person { Name = "Bubba", HireDate = DateTime.Parse("6-17-2020") });

            personDb.Add(second);

            personDb.Connect(first, second);

            var connections = personDb.GetConnections(first.Key);

            connections.Count().Should().Be(1);
        }

        [Fact]
        public void GetEdges_When_Node_Not_Connected_Returns_Empty_List()
        {
            var person = new Node<string, Person>(
                "person123",
                new Person { Name = "Farooq", HireDate = DateTime.Parse("3-21-2019") });

            personDb.Add(person);

            var connections = personDb.GetConnections(person.Key);

            connections.Count().Should().Be(0);
        }

        [Fact]
        public void Connect_When_Verticies_Already_Connected_Throws()
        {
            var first = new Node<string, Person>(
                "person123",
                new Person { Name = "Farooq", HireDate = DateTime.Parse("3-21-2019") });

            personDb.Add(first);

            var second = new Node<string, Person>(
                "person456",
                new Person { Name = "Bubba", HireDate = DateTime.Parse("6-17-2020") });

            personDb.Add(second);

            personDb.Connect(first, second);

            Action action = () => personDb.Connect(first, second);

            action.Should()
                .Throw<GraphieDbException>()
                .WithMessage("The node with key 'person123' is already connected to node with key 'person456'");
        }

        [Fact]
        public void Node_Can_Have_Multiple_Connections()
        {
            var v1 = new Node<string, Person>("p1", new Person());
            var v2 = new Node<string, Person>("p2", new Person());
            var v3 = new Node<string, Person>("p3", new Person());
            var v4 = new Node<string, Person>("p4", new Person());
            
            this.personDb.Add(v1);
            this.personDb.Add(v2);
            this.personDb.Add(v3);
            this.personDb.Add(v4);

            this.personDb.Connect(v1, v2);
            this.personDb.Connect(v1, v3);
            this.personDb.Connect(v1, v4);

            var connections = personDb.GetConnections(v1.Key);

            connections.Count().Should().Be(3);
        }

        [Fact]
        public void Can_Disconnect()
        {
            var v1 = new Node<string, Person>("p1", new Person());
            var v2 = new Node<string, Person>("p2", new Person());

            this.personDb.Add(v1);
            this.personDb.Add(v2);

            this.personDb.Connect(v1, v2);

            this.personDb.Disconnect(v1.Key, v2.Key);

            this.personDb.GetConnections(v1.Key).Count().Should().Be(0);
        }

        [Fact]
        public void Disconnect_When_Verticies_Not_Connected_Throws()
        {
            var v1 = new Node<string, Person>("p1", new Person());
            var v2 = new Node<string, Person>("p2", new Person());

            this.personDb.Add(v1);
            this.personDb.Add(v2);

            Action action = () => this.personDb.Disconnect(v1.Key, v2.Key);

            action.Should().Throw<GraphieDbException>()
                .WithMessage("Node with key 'p1' is not connected to node with key 'p2'.");
            
        }

        [Fact]
        public void Can_Delete_Node()
        {
            var v1 = new Node<string, Person>("p1", new Person());
            var v2 = new Node<string, Person>("p2", new Person());

            this.personDb.Add(v1);
            this.personDb.Add(v2);

            this.personDb.Delete(v1.Key);

            Action action = () => this.personDb.Find(v1.Key);
            action.Should().Throw<GraphieDbException>()
                .WithMessage("The node with key 'p1' does not exist.");
        }

        [Fact]
        public void Delete_When_Node_Does_Not_Exist_Throws()
        {
            Action action = () => this.personDb.Delete("foo");

            action.Should().Throw<GraphieDbException>()
                .WithMessage("The node with key 'foo' does not exist.");

        }

        [Fact]
        public void Connect_When_Node_Does_Not_Exist_Throws()
        {
            var v1 = new Node<string, Person>("p1", new Person());
            var v2 = new Node<string, Person>("p2", new Person());

            Action action = () => this.personDb.Connect(v1, v2);

            action.Should().Throw<GraphieDbException>()
                .WithMessage("The node with key 'p1' does not exist.");
        }

        [Fact]
        public void Delete_Connected_Node_Throws()
        {
            var v1 = new Node<string, Person>("p1", new Person());
            var v2 = new Node<string, Person>("p2", new Person());

            this.personDb.Add(v1);
            this.personDb.Add(v2);

            this.personDb.Connect(v1, v2);

            Action action = () => this.personDb.Delete(v1.Key);

            action.Should().Throw<GraphieDbException>()
                .WithMessage("The node with key 'p1' cannot be deleted because it is connected. " +
                             "Delete all connections associated with this node before deleting this node.");
        }

        [Fact]
        public void Disconnect_When_First_Node_Does_Not_Exist_Throws()
        {
            var v1 = new Node<string, Person>("p1", new Person());
            var v2 = new Node<string, Person>("p2", new Person());

            Action action = () => this.personDb.Disconnect(v1.Key, v2.Key);

            action.Should().Throw<GraphieDbException>()
                .WithMessage("The node with key 'p1' does not exist.");
        }

        [Fact]
        public void Can_Update_Node_Data()
        {
            var person = new Node<string, Person>("p1", new Person());
            this.personDb.Add(person);
            var updated = this.personDb.Update(person.Key, new Person() {Name = "foo"});

            updated.Data.Name.Should().Be("foo");
        }

        [Fact]
        public void Update_When_Node_Does_Not_Exist_Throws()
        {
            Action action = () => this.personDb.Update("foo", new Person() { Name = "foo" });

            action.Should().Throw<GraphieDbException>()
                .WithMessage("The node with key 'foo' does not exist.");
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
