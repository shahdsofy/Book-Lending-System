namespace Book_Lending_System.Core.Entities.Base
{
    public abstract class BaseAuditableEntity<T> : BaseEntity<T> where T : IEquatable<T>
    {
        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public string? LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }
}
