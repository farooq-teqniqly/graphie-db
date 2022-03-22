using System.Linq;
using FluentAssertions;
using Xunit;

namespace GraphieDb.Core.Tests
{
    public class DirectedGraphTests
    {
        private readonly Graph<int, int> graph;

        public DirectedGraphTests()
        {
            this.graph = new Graph<int, int>();
        }

        [Fact]
        public void Connections_Are_One_Way()
        {
            this.graph.Add(new Node<int, int>(1, 1));
            this.graph.Add(new Node<int, int>(2, 2));
            this.graph.Connect(1, 2);

            var node1Connection = this.graph.GetConnections(1).Single();
            node1Connection.First.Should().Be(1);
            node1Connection.Second.Should().Be(2);

            this.graph.GetConnections(2).Any().Should().BeFalse();
        }
    }
}
