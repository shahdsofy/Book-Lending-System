namespace Book_Lending_System.Core.Entities.Base
{
    public abstract class BaseEntity<T> where T :IEquatable<T>
    {
        public required T Id { get; set; }
    }
}
