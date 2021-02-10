namespace ODataTest
{
    public class ChildEntity<T> where T : struct
    {
        public T Id { get; set; }
    }
}