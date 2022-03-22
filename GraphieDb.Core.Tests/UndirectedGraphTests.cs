using System.Linq;
using FluentAssertions;
using Xunit;

namespace GraphieDb.Core.Tests
{
    public class UndirectedGraphTests
    {
        private readonly Graph<int, int> graph;

        public UndirectedGraphTests()
        {
            this.graph = new Graph<int, int>(directed:true);
        }

        [Fact]
        public void Connections_Are_Two_Way()
        {
            this.graph.Add(new Node<int, int>(1, 1));
            this.graph.Add(new Node<int, int>(2, 2));
            this.graph.Connect(1, 2);

            var node1Connection = this.graph.GetConnections(1).Single();
            node1Connection.First.Should().Be(1);
            node1Connection.Second.Should().Be(2);

            var node2Connection = this.graph.GetConnections(2).Single();
            node2Connection.First.Should().Be(2);
            node2Connection.Second.Should().Be(1);
        }

        [Fact]
        public void Disconnect_Removes_Connections_Both_Ways()
        {
            this.graph.Add(new Node<int, int>(1, 1));
            this.graph.Add(new Node<int, int>(2, 2));
            this.graph.Connect(1, 2);
            this.graph.Disconnect(1, 2);

            this.graph.GetConnections(1).Any().Should().BeFalse();
            this.graph.GetConnections(2).Any().Should().BeFalse();
        }

        [Fact]
        public void Can_Associate_Data_With_Connections()
        {
            var connectionData = new { Weight = 10 };

            this.graph.Add(new Node<int, int>(1, 1));
            this.graph.Add(new Node<int, int>(2, 2));
            this.graph.Connect(1, 2, connectionData);

            var connection1 = this.graph.GetConnections(1).Single();
            var connection2 = this.graph.GetConnections(2).Single();

            Assert.Equal(10, ((dynamic)connection1.Data).Weight);
            Assert.Equal(10, ((dynamic)connection2.Data).Weight);
        }
    }
}