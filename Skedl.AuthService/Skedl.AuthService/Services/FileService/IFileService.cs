namespace Skedl.AuthService.Services.FileService
{
    public interface IFileService
    {
        bool ByteArrayToFile(string fileName, byte[] byteArray);
        byte[] ConvertToByteArray(string filePath);
    }
}
