namespace GraphieDb.Core
{
    public class Node<TKey, TData>
    {
        public TKey Key { get; }
        public TData Data { get; set; }

        public Node(TKey key, TData data)
        {
            this.Key = key;
            this.Data = data;
        }
    }
}