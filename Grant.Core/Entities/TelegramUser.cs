using Grant.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Core.Entities
{
    public class TelegramUser : BaseEntity
    {
        public string FirstName { get; set; }

        public string Username { get; set; }

        public bool CanSendCommmand { get; set; }

        public int? PictureId { get; set; }

        public GrantFileInfo Picture { get; set; }

        public string TelegramId { get; set; }

        [NotMapped]
        public string PicUrl { get; set; }
    }
}
