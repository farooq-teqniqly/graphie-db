using System.Collections.Generic;
using System.Linq;
using GraphieDb.Core.Exceptions;

namespace GraphieDb.Core
{
    public class Graph<TKey, TData>
    {
        private readonly Dictionary<TKey, Node<TKey, TData>> nodes = new();
        private readonly Dictionary<TKey, SortedList<TKey, Edge<TKey, TData>>> edges = new ();

        public void Add(Node<TKey,TData> node)
        {
            if (!this.nodes.TryAdd(node.Key, node))
            {
                throw new GraphieDbException($"The node with key '{node.Key}' already exists.");
            }

            this.edges.Add(node.Key, new SortedList<TKey, Edge<TKey, TData>>());
        }

        public void Delete(TKey key)
        {
            var nodeEdges = this.GetVertexEdges(key);
            
            if (nodeEdges.Any())
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
            var nodeEdges = this.GetVertexEdges(first.Key);
            
            if (!nodeEdges.TryAdd(second.Key, new Edge<TKey, TData>(first, second)))
            {
                throw new GraphieDbException(
                    $"The node with key '{first.Key}' is already connected to node with key '{second.Key}'");
            }
        }

        public void Disconnect(TKey first, TKey second)
        {
            var nodeEdges = this.GetVertexEdges(first);

            if (!nodeEdges.TryGetValue(second, out _))
            {
                throw new GraphieDbException($"Node with key '{first}' is not connected to node with key '{second}'.");
            }

            nodeEdges.Remove(second);
        }

        public IEnumerable<Edge<TKey, TData>> GetEdges(TKey key)
        {
            return this.edges[key].Values;
        }

        private SortedList<TKey, Edge<TKey, TData>> GetVertexEdges(TKey key)
        {
            if (!this.edges.TryGetValue(key, out var nodeEdges))
            {
                throw new GraphieDbException($"The node with key '{key}' does not exist.");
            }

            return nodeEdges;
        }

    }
}
