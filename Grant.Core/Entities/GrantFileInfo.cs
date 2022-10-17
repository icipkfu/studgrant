using Grant.Core.Entities.Base;

namespace Grant.Core.Entities
{
    public class GrantFileInfo : BaseEntity
    {
        /// <summary>
        ///  Название файла 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///  Guid
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        ///  Расширение 
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        ///  Путь до файла (в системе хранения)
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///  Путь до файла (в системе хранения)
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        ///  Размер файла в байтах
        /// </summary>
        public long Size { get; set; }

    }
}