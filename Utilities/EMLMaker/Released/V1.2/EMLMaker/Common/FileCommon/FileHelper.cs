using System.Diagnostics;
using System.IO;

namespace Common.FileCommon
{
    public static class FileHelper
    {
        public static bool IsExistsFile(string path)
        {
            return File.Exists(path);
        }

        public static bool IsExistsFolder(string path)
        {
            return Directory.Exists(path);
        }

        public static FileInfo GetFileInfo(string filePath)
        {
            if(!File.Exists(filePath))
            {
                return null;
            }

            return new FileInfo(filePath);
        }

        public static FileVersionInfo GetVersionFileInfo(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            return FileVersionInfo.GetVersionInfo(filePath);
        }

        #region Create Folder
        /// <summary>
        /// Create Folder in specific path
        /// </summary>
        /// <param name="directoryPath"> folder path </param>
        /// <param name="folderName"> folder name </param>
        /// <returns> is success created </returns>
        public static bool CreateFolder(string directoryPath, string folderName)
        {
            if (Directory.Exists(directoryPath) && !Directory.Exists(Path.Combine(directoryPath, folderName)))
            {
                Directory.CreateDirectory(Path.Combine(directoryPath, folderName));
            }

            return false;
        }

        /// <summary>
        /// Create Folder in specific path
        /// </summary>
        /// <param name="folderPath"> folder path </param>
        /// <returns> is success created </returns>
        public static bool CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);

                File.SetAttributes(folderPath, FileAttributes.Normal);
            }

            return false;
        }
        #endregion

        #region Copy, Delete, Rename
        /// <summary>
        /// Copy a whole directory to target path
        /// </summary>
        /// <param name="source">source folder to copy</param>
        /// <param name="target">target path copy to</param>
        public static void CopyDirectory(string source, string target)
        {
            DirectoryInfo rootFolder = new DirectoryInfo(source);

            if (!rootFolder.Exists)
            {
                throw new DirectoryNotFoundException("source path is not exsist!");
            }

            if (!Directory.Exists(target))
            {
                throw new DirectoryNotFoundException("target path is not exsist!");
            }

            string targetFolder = Path.Combine(target, rootFolder.Name);

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
                File.SetAttributes(targetFolder, FileAttributes.Normal);
            }

            FileInfo[] files = rootFolder.GetFiles();

            foreach (FileInfo filePath in files)
            {
                filePath.CopyTo(Path.Combine(targetFolder, filePath.Name), true);
                filePath.Attributes = FileAttributes.Normal;
                File.SetAttributes(Path.Combine(targetFolder, filePath.Name), FileAttributes.Normal);
            }

            DirectoryInfo[] children = rootFolder.GetDirectories();

            foreach (DirectoryInfo dirPath in children)
            {
                CopyDirectory(Path.Combine(source, dirPath.Name), targetFolder);
            }
        }

        /// <summary>
        /// Copy a single file to another path
        /// </summary>
        /// <param name="sourceFilePath">the path of source file</param>
        /// <param name="targetFilePath">the path of target file</param>
        /// <param name="targetFileName">the name of target file, if sent null, it will use the original file name</param>
        /// <param name="isOverwrite">overwrite the file in target path</param>
        public static void CopyFile(string sourceFilePath, string targetFilePath, string targetFileName, bool isOverwrite)
        {
            FileInfo sourceFile = new FileInfo(sourceFilePath);

            if (!sourceFile.Exists)
            {
                throw new FileNotFoundException("source file is not exsist!");
            }

            if (!Directory.Exists(targetFilePath))
            {
                throw new DirectoryNotFoundException("target path is not exsist!");
            }

            if (string.IsNullOrEmpty(targetFileName))
            {
                targetFileName = sourceFile.Name;
            }

            sourceFile.CopyTo(Path.Combine(targetFilePath, targetFileName), isOverwrite);

            File.SetAttributes(Path.Combine(targetFilePath, targetFileName), FileAttributes.Normal);
        }

        /// <summary>
        /// Copy a single file to target path
        /// </summary>
        /// <param name="sourceFilePath">the path of source file</param>
        /// <param name="targetFilePath">the path of target file</param>
        /// <param name="isOverwrite">overwrite the file in target path</param>
        public static void CopyFile(string sourceFilePath, string targetFilePath, bool isOverwrite)
        {
            FileInfo sourceFile = new FileInfo(sourceFilePath);

            if (!sourceFile.Exists)
            {
                throw new FileNotFoundException("source file is not exsist!");
            }

            if (!Directory.Exists(targetFilePath))
            {
                throw new DirectoryNotFoundException("target path is not exsist!");
            }

            sourceFile.CopyTo(Path.Combine(targetFilePath, sourceFile.Name), isOverwrite);

            File.SetAttributes(Path.Combine(targetFilePath, sourceFile.Name), FileAttributes.Normal);
        }

        /// <summary>
        /// Delete a file by path
        /// </summary>
        /// <param name="filePath"> file path </param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Rename a file
        /// </summary>
        /// <param name="filePath"> file path </param>
        /// <param name="newName"> file new name </param>
        public static void RenameFile(string filePath, string newName)
        {
            if (File.Exists(filePath))
            {
                FileInfo sourceFile = new FileInfo(filePath);

                CopyFile(filePath, sourceFile.Directory.ToString(), newName, true);

                DeleteFile(filePath);
            }
        }
        #endregion
    }
}
