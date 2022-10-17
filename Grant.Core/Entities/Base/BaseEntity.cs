namespace Grant.Core.Entities.Base
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Interfaces;
    using Newtonsoft.Json;
    using System.Data.Entity.Core.Objects.DataClasses;
    using System.Data.Entity.Core;

    public abstract class BaseEntity : IBaseEntity, IEntityWithKey
    {
        public EntityKey EntityKey { get; set; }

        [Key]
        public long Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime EditDate { get; set; }

        [JsonIgnore]
        public bool DeletedMark { get; set; }

        public long? UserId { get; set; }
    }
}