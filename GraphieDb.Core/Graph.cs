using System.Collections.Generic;
using System.Linq;
using GraphieDb.Core.Exceptions;

namespace GraphieDb.Core
{
    public class Graph<TKey, TData>
    {
        public bool Directed { get; }
        private readonly Dictionary<TKey, Node<TKey, TData>> nodes = new();
        private readonly Dictionary<TKey, SortedList<TKey, Connection<TKey>>> connections = new ();

        public Graph(bool directed = false)
        {
            Directed = directed;
        }

        public void Add(Node<TKey,TData> node)
        {
            if (!this.nodes.TryAdd(node.Key, node))
            {
                throw new GraphieDbException($"The node with key '{node.Key}' already exists.");
            }

            this.connections.Add(node.Key, new SortedList<TKey, Connection<TKey>>());
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
            return this.GetNode(key);
        }

        
        public void Connect(TKey first, TKey second, object connectionData = null)
        {
            var nodeConnections = this.GetVertexEdges(first);
            
            if (!nodeConnections.TryAdd(second, new Connection<TKey>(first, second, connectionData)))
            {
                throw new GraphieDbException(
                    $"The node with key '{first}' is already connected to node with key '{second}'");
            }

            if (!this.Directed)
            {
                return;
            }

            nodeConnections = this.GetVertexEdges(second);
            nodeConnections.Add(first, new Connection<TKey>(second, first, connectionData));
        }

        public void Disconnect(TKey first, TKey second)
        {
            var nodeConnections = this.GetVertexEdges(first);

            if (!nodeConnections.TryGetValue(second, out _))
            {
                throw new GraphieDbException($"Node with key '{first}' is not connected to node with key '{second}'.");
            }

            nodeConnections.Remove(second);

            if (!this.Directed)
            {
                return;
            }

            nodeConnections = this.GetVertexEdges(second);
            nodeConnections.Remove(first);
        }

        public Node<TKey, TData> Update(TKey key, TData data)
        {
            var node = this.GetNode(key);
            node.Data = data;

            return node;
        }

        public IEnumerable<Connection<TKey>> GetConnections(TKey key)
        {
            return this.connections[key].Values;
        }

        private Node<TKey, TData> GetNode(TKey key)
        {
            if (!this.nodes.TryGetValue(key, out var node))
            {
                throw new GraphieDbException($"The node with key '{key}' does not exist.");
            }

            return node;
        }

        private SortedList<TKey, Connection<TKey>> GetVertexEdges(TKey key)
        {
            if (!this.connections.TryGetValue(key, out var nodeConnections))
            {
                throw new GraphieDbException($"The node with key '{key}' does not exist.");
            }

            return nodeConnections;
        }
    }
}
