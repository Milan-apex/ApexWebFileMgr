namespace ApexWebFileMgr.core.Services.FileMgrService
{
    public interface IFileManagerService
    {
        string GenerateFileName(string DocType, string fileName);
        byte[] CompressImage(Stream inputImageStream, long desiredSizeKb);
    }
}
