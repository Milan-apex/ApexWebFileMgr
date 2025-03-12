using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp;

namespace ApexWebFileMgr.core.Services.FileMgrService
{
    public class FileManagerService : IFileManagerService
    {
        public string GenerateFileName(string DocType, string fileName)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            string generatedFileName = $"{timestamp}_{DocType}_{fileName}";
            return generatedFileName;
        }

        public byte[] CompressImage(Stream inputImageStream, long desiredSizeKb)
        {
            try
            {
                using (var image = Image.Load(inputImageStream))
                {
                    var quality = 100;
                    byte[] compressedBytes;

                    do
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            var encoder = new JpegEncoder { Quality = quality };
                            image.Save(memoryStream, encoder);
                            compressedBytes = memoryStream.ToArray();

                            // Reduce the quality incrementally
                            quality -= 5;
                        }
                    } while (compressedBytes.Length > desiredSizeKb * 1024 && quality > 0);

                    return compressedBytes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

    }
}
