namespace Grant.Core.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Base;

    public class DataFile : BaseEntity
    {
        public byte[] Content { get; set; }

        public string Extension { get; set; }

        public int Size { get; set; }
    }
}