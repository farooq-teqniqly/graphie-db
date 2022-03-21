using System.Collections.Generic;
using GraphieDb.Core.Exceptions;

namespace GraphieDb.Core
{
    public class Graph<TKey, TData>
    {
        private readonly Dictionary<TKey, Vertex<TKey, TData>> nodes = new();
        private readonly Dictionary<TKey, List<Edge<TKey, TData>>> edges = new ();

        public void Add(Vertex<TKey,TData> vertex)
        {
            if (!this.nodes.TryAdd(vertex.Key, vertex))
            {
                throw new GraphieDbException($"The vertex with key '{vertex.Key}' already exists.");
            }

            this.edges.Add(vertex.Key, new List<Edge<TKey, TData>>());
        }

        public Vertex<TKey, TData> Find(TKey key)
        {
            this.nodes.TryGetValue(key, out var node);
            return node;
        }

        public void Connect(Vertex<TKey, TData> first, Vertex<TKey, TData> second)
        {
            if (this.edges.TryGetValue(first.Key, out var nodeEdges))
            {
                nodeEdges.Add(new Edge<TKey, TData>(first, second));
            }
            else
            {
                this.edges[first.Key] = new List<Edge<TKey, TData>>
                {
                    new Edge<TKey, TData>(first, second)
                };

                this.edges.TryAdd(first.Key, new List<Edge<TKey, TData>>());
            }
        }

        public IEnumerable<Edge<TKey, TData>> GetEdges(TKey key)
        {
            return this.edges[key];
        }
    }

    public class Edge<TKey, TData>
    {
        public Edge(Vertex<TKey, TData> first, Vertex<TKey, TData> second)
        {
            
        }
    }
}
