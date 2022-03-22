namespace GraphieDb.Core
{
    public class Connection<TKey>
    {
        public object Data { get; }
        public TKey First { get; }
        public TKey Second { get; }
        public Connection(TKey first, TKey second, object data = null)
        {
            this.Data = data;
            this.First = first;
            this.Second = second;
            
        }
    }
}