using Grant.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Core.Entities
{
    /// <summary>
    /// Настройка приложения
    /// </summary>
    public class Settings : BaseEntity
    {
        /// <summary>
        /// Ключ
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }
    }
}
