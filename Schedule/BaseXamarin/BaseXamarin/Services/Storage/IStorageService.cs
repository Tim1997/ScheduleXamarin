using PCLStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseXamarin.Services.Storage
{
    public interface IStorageService
    {
        Task<List<string>> GetFolders(string folderName);
        Task<List<string>> GetFiles(string folderName);
        Task<bool> IsFileExistAsync(string fileName);
        Task<bool> IsFolderExistAsync(string folderName);
        Task<IFolder> CreateFolder(string folderName);
        Task<IFile> CreateFile(string filename);
        Task<bool> WriteTextAllAsync(string filename, string content = "");
        Task<string> ReadAllTextAsync(string fileName);
        Task<bool> DeleteFile(string fileName);
        Task<bool> DeleteFolder(string folderName);
        Task SaveImage(byte[] image, String fileName);
        Task<byte[]> LoadImage(byte[] image, String fileName);
    }
}
