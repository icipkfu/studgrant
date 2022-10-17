using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using Grant.Core.Context;

namespace Grant.FileStorage
{
    using System;
    using System.Data.Entity.Migrations;
    using System.IO;
    using Grant.Core;
    using Grant.Core.Config;
    using Grant.Core.DbContext;
    using Grant.Core.Entities;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    class FileManager : IFileManager
    {
        private readonly IConfigProvider _configProvider;
        private GrantDbContext db = new GrantDbContext();

        // todo обработка исключений
        private string _storagePath;
        private string _virtualPath;

        private Dictionary<string, GrantFileInfo> FileDict = new Dictionary<string, GrantFileInfo>();
        

        public FileManager(IConfigProvider configProvider)
        {
            this._configProvider = configProvider;
        }

        protected string StoragePath
        {
            get
            {
                return _storagePath ??
                (_storagePath = _configProvider.GetConfig().GetAs<string>("FileStorage", "Path"));
            }
        }

        //todo переделать получение пути для клиента
        protected string VirtualPath
        {
            get
            {
                return _virtualPath ??
                (_virtualPath = "../images/");
            }
        }

        public async Task LoadAll()
        {
            var files = await db.FilesInfo.ToDictionaryAsync(x => x.Guid);

            FileDict = files;
        }


        public async Task<string> Upload(Stream stream, string fileName)
        {
            var guid = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(fileName);

            var newfile = new GrantFileInfo
            {
                Size = stream.Length,
                Extension = extension,
                VirtualPath = Path.Combine(VirtualPath, String.Format("{0}{1}", guid, extension)),
                Path = Path.Combine(StoragePath, String.Format("{0}{1}", guid, extension)),
                FileName = fileName,
                Guid = guid
            };

            using (var fileStream = new FileStream(newfile.Path, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrEmpty(newfile.Extension))
            {
                newfile.Extension = newfile.Extension.ToLower();
            }

            if (String.Compare(newfile.Extension, ".jpeg", StringComparison.CurrentCulture) == 0 ||
                String.Compare(newfile.Extension, ".jpg", StringComparison.CurrentCulture) == 0 ||
                String.Compare(newfile.Extension, ".png", StringComparison.CurrentCulture) == 0 ||
                String.Compare(newfile.Extension, ".bmp", StringComparison.CurrentCulture) == 0)
            {

                var miniPath = Path.Combine(StoragePath, String.Format("_{0}{1}", guid, ".png"));
                var miniVirtualPath = Path.Combine(VirtualPath, String.Format("_{0}{1}", guid, ".png"));

                 using (var image = Image.FromFile(newfile.Path))
                 using (var newImage = ScaleImage(image, 700, 500))
                 {
                     ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders(); 
                     ImageCodecInfo ici = null; 

                     foreach (ImageCodecInfo codec in codecs)
                     { 
                        if (codec.MimeType == "image/jpeg") 
                        ici = codec; 
                     } 

                     EncoderParameters ep = new EncoderParameters(); 
                     ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)60);
                     newImage.Save(miniPath, ici, ep);
                 }

                 try
                 {
                     if (File.Exists(newfile.Path))
                     {
                         File.Delete(newfile.Path);
                     }
                 }
                 catch (Exception)
                 {
                     // todo log
                 }

                newfile.Path = miniPath;
                newfile.VirtualPath = miniVirtualPath;

            }

            db.FilesInfo.AddOrUpdate(newfile);
            await db.SaveChangesAsync();

            return newfile.Guid;
        }

        public async Task<GrantFileInfo> UploadFile(Stream stream, string fileName)
        {
            var guid = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(fileName);

            var newfile = new GrantFileInfo
            {
                Size = stream.Length,
                Extension = extension,
                VirtualPath = Path.Combine(VirtualPath, String.Format("{0}{1}", guid, extension)),
                Path = Path.Combine(StoragePath, String.Format("{0}{1}", guid, extension)),
                FileName = fileName,
                Guid = guid
            };

            using (var fileStream = new FileStream(newfile.Path, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrEmpty(newfile.Extension))
            {
                newfile.Extension = newfile.Extension.ToLower();
            }

            if (String.Compare(newfile.Extension, ".jpeg", StringComparison.CurrentCulture) == 0 ||
                String.Compare(newfile.Extension, ".jpg", StringComparison.CurrentCulture) == 0 ||
                String.Compare(newfile.Extension, ".png", StringComparison.CurrentCulture) == 0 ||
                String.Compare(newfile.Extension, ".bmp", StringComparison.CurrentCulture) == 0)
            {

                var miniPath = Path.Combine(StoragePath, String.Format("_{0}{1}", guid, ".png"));
                var miniVirtualPath = Path.Combine(VirtualPath, String.Format("_{0}{1}", guid, ".png"));

                using (var image = Image.FromFile(newfile.Path))
                using (var newImage = ScaleImage(image, 700, 500))
                {
                    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                    ImageCodecInfo ici = null;

                    foreach (ImageCodecInfo codec in codecs)
                    {
                        if (codec.MimeType == "image/jpeg")
                            ici = codec;
                    }

                    EncoderParameters ep = new EncoderParameters();
                    ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)60);
                    newImage.Save(miniPath, ici, ep);
                }

                try
                {
                    if (File.Exists(newfile.Path))
                    {
                        File.Delete(newfile.Path);
                    }
                }
                catch (Exception)
                {
                   // todo log
                }
              

                newfile.Path = miniPath;
                newfile.VirtualPath = miniVirtualPath;

            }

            db.FilesInfo.AddOrUpdate(newfile);
            await db.SaveChangesAsync();

            return newfile;
        }

        public async Task<GrantFileInfo> UploadHighResFile(Stream stream, string fileName)
        {
            var guid = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(fileName);

            var newfile = new GrantFileInfo
            {
                Size = stream.Length,
                Extension = extension,
                VirtualPath = Path.Combine(VirtualPath, String.Format("{0}{1}", guid, extension)),
                Path = Path.Combine(StoragePath, String.Format("{0}{1}", guid, extension)),
                FileName = fileName,
                Guid = guid
            };

            using (var fileStream = new FileStream(newfile.Path, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrEmpty(newfile.Extension))
            {
                newfile.Extension = newfile.Extension.ToLower();
            }

            if (String.Compare(newfile.Extension, ".jpeg", StringComparison.CurrentCulture) == 0 ||
                String.Compare(newfile.Extension, ".jpg", StringComparison.CurrentCulture) == 0 ||
                String.Compare(newfile.Extension, ".png", StringComparison.CurrentCulture) == 0 ||
                String.Compare(newfile.Extension, ".bmp", StringComparison.CurrentCulture) == 0)
            {

                var miniPath = Path.Combine(StoragePath, String.Format("_{0}{1}", guid, ".png"));
                var miniVirtualPath = Path.Combine(VirtualPath, String.Format("_{0}{1}", guid, ".png"));

                using (var image = Image.FromFile(newfile.Path))
                using (var newImage = ScaleImage(image, 2000, 1400))
                {
                    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                    ImageCodecInfo ici = null;

                    foreach (ImageCodecInfo codec in codecs)
                    {
                        if (codec.MimeType == "image/jpeg")
                            ici = codec;
                    }

                    EncoderParameters ep = new EncoderParameters();
                    ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)60);
                    newImage.Save(miniPath, ici, ep);
                }

                try
                {
                    if (File.Exists(newfile.Path))
                    {
                        File.Delete(newfile.Path);
                    }
                }
                catch (Exception)
                {
                    // todo log
                }

                newfile.Path = miniPath;
                newfile.VirtualPath = miniVirtualPath;

            }

            db.FilesInfo.AddOrUpdate(newfile);
            await db.SaveChangesAsync();

            return newfile;
        }

        public async Task<string> UploadHighRes(Stream stream, string fileName)
        {
            var guid = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(fileName);

            var newfile = new GrantFileInfo
            {
                Size = stream.Length,
                Extension = extension,
                VirtualPath = Path.Combine(VirtualPath, String.Format("{0}{1}", guid, extension)),
                Path = Path.Combine(StoragePath, String.Format("{0}{1}", guid, extension)),
                FileName = fileName,
                Guid = guid
            };

            using (var fileStream = new FileStream(newfile.Path, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrEmpty(newfile.Extension))
            {
                newfile.Extension = newfile.Extension.ToLower();
            }

            if (String.Compare(newfile.Extension, ".jpeg", StringComparison.CurrentCulture) == 0 ||
                String.Compare(newfile.Extension, ".jpg", StringComparison.CurrentCulture) == 0 ||
                String.Compare(newfile.Extension, ".png", StringComparison.CurrentCulture) == 0 ||
                String.Compare(newfile.Extension, ".bmp", StringComparison.CurrentCulture) == 0)
            {

                var miniPath = Path.Combine(StoragePath, String.Format("_{0}{1}", guid, ".png"));
                var miniVirtualPath = Path.Combine(VirtualPath, String.Format("_{0}{1}", guid, ".png"));

                using (var image = Image.FromFile(newfile.Path))
                using (var newImage = ScaleImage(image, 1000, 700))
                {
                    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                    ImageCodecInfo ici = null;

                    foreach (ImageCodecInfo codec in codecs)
                    {
                        if (codec.MimeType == "image/jpeg")
                            ici = codec;
                    }

                    EncoderParameters ep = new EncoderParameters();
                    ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);
                    newImage.Save(miniPath, ici, ep);
                }

                try
                {
                    if (File.Exists(newfile.Path))
                    {
                        File.Delete(newfile.Path);
                    }
                }
                catch (Exception)
                {
                    // todo log
                }

                newfile.Path = miniPath;
                newfile.VirtualPath = miniVirtualPath;

            }

            db.FilesInfo.AddOrUpdate(newfile);
            await db.SaveChangesAsync();

            return newfile.Guid;
        }

        //public static void Test()
        //{
        //    using (var image = Image.FromFile(@"c:\logo.png"))
        //    using (var newImage = ScaleImage(image, 300, 400))
        //    {
        //        newImage.Save(@"c:\test.png", ImageFormat.Png);
        //    }
        //}

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public async Task Delete(string guid)
        {
            var fileinfo = await Get(guid);

            if (File.Exists(fileinfo.Path))
            {
                File.Delete(fileinfo.Path);
            }
        }

        public async Task<GrantFileInfo> Get(string guid)
        {
           /* if (FileDict.ContainsKey(guid))
            {
                return FileDict[guid];
            } */

            var file = await db.FilesInfo.SingleOrDefaultAsync(x => x.Guid == guid);

            //FileDict.Add(guid, file);

            return  file;
        }

        public async Task<string> UploadOriginal(Stream stream, string fileName)
        {
            var guid = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(fileName);

            var newfile = new GrantFileInfo
            {
                Size = stream.Length,
                Extension = extension,
                VirtualPath = Path.Combine(VirtualPath, String.Format("{0}{1}", guid, extension)),
                Path = Path.Combine(StoragePath, String.Format("{0}{1}", guid, extension)),
                FileName = fileName,
                Guid = guid
            };

            using (var fileStream = new FileStream(newfile.Path, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrEmpty(newfile.Extension))
            {
                newfile.Extension = newfile.Extension.ToLower();
            }

            db.FilesInfo.AddOrUpdate(newfile);
            await db.SaveChangesAsync();

            return newfile.Guid;
        }
    }
}
