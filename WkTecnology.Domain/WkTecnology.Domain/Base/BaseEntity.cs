namespace Portifolio.Dominio.Base
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }
        public DateTime CreatedAt { get;  set; }
        public DateTime? UpdatedAt { get;  set; }
    }
}
