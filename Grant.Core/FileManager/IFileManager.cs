namespace Grant.Core
{
    using System.IO;
    using System.Threading.Tasks;
    using Grant.Core.Entities;

    public interface IFileManager
    {
        Task<string> Upload(Stream stream, string fileName);

        Task<string> UploadOriginal(Stream stream, string fileName);

        Task<GrantFileInfo> UploadFile(Stream stream, string fileName);

        Task<GrantFileInfo> UploadHighResFile(Stream stream, string fileName);

        Task<string> UploadHighRes(Stream stream, string fileName);

        Task Delete(string guid);

        Task<GrantFileInfo> Get(string guid);

        Task LoadAll();
    }
}
