using System.Collections.Generic;
using System.Linq;
using GraphieDb.Core.Exceptions;

namespace GraphieDb.Core
{
    public class Graph<TKey, TData>
    {
        private readonly Dictionary<TKey, Node<TKey, TData>> nodes = new();
        private readonly Dictionary<TKey, SortedList<TKey, Connection<TKey, TData>>> connections = new ();

        public void Add(Node<TKey,TData> node)
        {
            if (!this.nodes.TryAdd(node.Key, node))
            {
                throw new GraphieDbException($"The node with key '{node.Key}' already exists.");
            }

            this.connections.Add(node.Key, new SortedList<TKey, Connection<TKey, TData>>());
        }

        public void Delete(TKey key)
        {
            var nodeConnections = this.GetVertexEdges(key);
            
            if (nodeConnections.Any())
            {
                throw new GraphieDbException(
                    $"The node with key '{key}' cannot be deleted because it is connected. " +
                    "Delete all connections associated with this node before deleting this node.");
            }
            
            this.nodes.Remove(key);
        }

        public Node<TKey, TData> Find(TKey key)
        {
            this.nodes.TryGetValue(key, out var node);
            return node;
        }

        
        public void Connect(Node<TKey, TData> first, Node<TKey, TData> second)
        {
            var nodeConnections = this.GetVertexEdges(first.Key);
            
            if (!nodeConnections.TryAdd(second.Key, new Connection<TKey, TData>(first, second)))
            {
                throw new GraphieDbException(
                    $"The node with key '{first.Key}' is already connected to node with key '{second.Key}'");
            }
        }

        public void Disconnect(TKey first, TKey second)
        {
            var nodeConnections = this.GetVertexEdges(first);

            if (!nodeConnections.TryGetValue(second, out _))
            {
                throw new GraphieDbException($"Node with key '{first}' is not connected to node with key '{second}'.");
            }

            nodeConnections.Remove(second);
        }

        public IEnumerable<Connection<TKey, TData>> GetConnections(TKey key)
        {
            return this.connections[key].Values;
        }

        private SortedList<TKey, Connection<TKey, TData>> GetVertexEdges(TKey key)
        {
            if (!this.connections.TryGetValue(key, out var nodeConnections))
            {
                throw new GraphieDbException($"The node with key '{key}' does not exist.");
            }

            return nodeConnections;
        }

    }
}
