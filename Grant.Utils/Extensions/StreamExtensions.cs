namespace Grant.Utils.Extensions
{
    using System;
    using System.IO;

    public static class StreamExtensions
    {
        /// <summary>
        /// Чтение контента потока в массив байт.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(this Stream source)
        {
            if (source == null || !source.CanRead)
            {
                return new byte[0];
            }

            var originalPosition = source.Position;
            if (source.CanSeek)
                source.Seek(0, SeekOrigin.Begin);

            try
            {
                var readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = source.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = source.ReadByte();
                        if (nextByte != -1)
                        {
                            var temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                var buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }

                return buffer;
            }
            finally
            {
                if(source.CanSeek)
                    source.Seek(originalPosition, SeekOrigin.Begin);
            }
        }
    }
}