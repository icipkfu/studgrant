using Grant.Core;
using Grant.Utils.Extensions;
using Grant.Install;

namespace Grant.FileStorage
{
    public class FileStorageInstaller : BaseInstaller
    {
        public override void Install()
        {
            Container.RegisterTransient<IFileManager, FileManager>();
        }
    }
}
