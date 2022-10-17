namespace Grant.Core
{
    using System;

    public class DataFileResult
    {
        public DataFileResult(string virtPath, string name, string hash, DateTime editTime)
        {
            this.VirtualPath = virtPath;
            this.FullName = name;
            this.Hash = hash;
            this.EditDate = editTime;
        }
        public string VirtualPath { get; set; }

        public string FullName { get; set; }

        public string Hash { get; set; }

        public DateTime EditDate { get; set; }
    }
}
