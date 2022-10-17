namespace Grant.Core.Interfaces
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public interface IBaseEntity
    {
        [Key]
        [Column("ID")]
        long Id { get; set; }

        [MaxLength(255)]
        [Column("NAME")]
        string Name { get; set; }

        [Column("CREATE_DATE")]
        DateTime CreateDate { get; set; }

        [Column("EDIT_DATE")]
        DateTime EditDate { get; set; }

        [Column("DELETED_MARK")]
        bool DeletedMark { get; set; }

        [Column("UserId")]
        long? UserId { get; set; }
    }
}
