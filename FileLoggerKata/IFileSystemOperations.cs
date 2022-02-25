using System;

namespace FileLoggerKata
{
    public interface IFileSystemOperations
    {
        void AppendText(string filePath, string text);

        bool FileExist(string filepath);

        DateTime GetFileModifiedDate(string filePath);

        bool RenameFile(string srcFilePath, string newFilePath);
    }

    
}
