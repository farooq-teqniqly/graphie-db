namespace GraphieDb.Core
{
    public class Vertex<TKey, TData>
    {
        public TKey Key { get; }
        public TData Data { get; }

        public Vertex(TKey key, TData data)
        {
            this.Key = key;
            this.Data = data;
        }
    }
}