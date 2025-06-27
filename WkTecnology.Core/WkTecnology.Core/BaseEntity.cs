namespace Portifolio.Core
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }
        public void MarkAsDeleted()
        {
            IsActive = false;
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
        public void MarkAsActive()
        {
            IsActive = true;
            IsDeleted = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public override bool Equals(object? obj)
        {
            if (obj is BaseEntity entity)
            {
                return Id == entity.Id;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name} [Id={Id}, CreatedAt={CreatedAt}, UpdatedAt={UpdatedAt}, IsActive={IsActive}, IsDeleted={IsDeleted}]";
        }
        public virtual void Validate()
        {
            if (Id <= 0)
            {
                throw new InvalidOperationException("Id deve ser maior que zero.");
            }
        }

        public virtual void SetId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id deve ser maior que zero.", nameof(id));
            }
            Id = id;
        }
        public virtual void SetCreatedAt(DateTime createdAt)
        {
            if (createdAt == default)
            {
                throw new ArgumentException("CreatedAt não pode ser o default.", nameof(createdAt));
            }
            CreatedAt = createdAt;
        }

        public virtual void SetUpdatedAt(DateTime? updatedAt)
        {
            if (updatedAt.HasValue && updatedAt.Value < CreatedAt)
            {
                throw new ArgumentException("UpdatedAt não pode ser anterior a CreatedAt.", nameof(updatedAt));
            }
            UpdatedAt = updatedAt;

        }

        public virtual void SetIsActive(bool isActive)
        {
            IsActive = isActive;
        }

        public virtual void SetIsDeleted(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }

        public virtual void SetBaseEntity(int id, DateTime createdAt, DateTime? updatedAt = null, bool isActive = true, bool isDeleted = false)
        {
            SetId(id);
            SetCreatedAt(createdAt);
            SetUpdatedAt(updatedAt);
            SetIsActive(isActive);
            SetIsDeleted(isDeleted);


        }

        public virtual void SetBaseEntity(BaseEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entidade não pode ser vazia.");
            }
            SetBaseEntity(entity.Id, entity.CreatedAt, entity.UpdatedAt, entity.IsActive, entity.IsDeleted);
        }

        public virtual void SetBaseEntity(int id, DateTime createdAt, DateTime? updatedAt = null)
        {
            SetId(id);
            SetCreatedAt(createdAt);
            SetUpdatedAt(updatedAt);
            SetIsActive(true);
            SetIsDeleted(false);

        }

        public virtual void SetBaseEntity(int id, DateTime createdAt, DateTime? updatedAt = null, bool isActive = true)
        {
            SetId(id);
            SetCreatedAt(createdAt);
            SetUpdatedAt(updatedAt);
            SetIsActive(isActive);
            SetIsDeleted(false);
        }
    }
}
