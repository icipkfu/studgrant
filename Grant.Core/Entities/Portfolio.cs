namespace Grant.Core.Entities
{
    using System.Collections.Generic;
    using Base;

    public class Portfolio : BaseEntity
    {
        public ICollection<Achievement> Achievements { get; set; }
    }
}