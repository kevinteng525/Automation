using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.SshNet;
using System.IO;

namespace Common.SSH
{
    public class SSHWrapper
    {
        public static string RunCommand(string host, string user, string password, string command)
        {
            using (SshClient client = new SshClient(host, user, password))
            {
                client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(5400);
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);//send keep-alive request to server every minute
                client.Connect();
                var output =client.RunCommand(command);
                client.Disconnect();
                return output.Result;
            }
        }

        public static void CopyDirectoryToRemote(string host, string user, string password, string localPath, string remotePath)
        {

            string[] folders = System.IO.Directory.GetDirectories(localPath);
            if (!DoexRemoteFolderExist(host, user, password, remotePath))
            {
                CreateRemoteFolder(host, user, password, remotePath);
            }
            remotePath = remotePath.EndsWith("/") ? remotePath : remotePath + "/";
            foreach (string f in folders)
            {
                DirectoryInfo di = new DirectoryInfo(f);

                CopyDirectoryToRemote(host, user, password, f, remotePath + di.Name + "/");
            }
            string[] files = System.IO.Directory.GetFiles(localPath);
            foreach (string s in files)
            {
                var fi = new System.IO.FileInfo(s);
                if (fi.Length > 0)
                {
                    CopyFileFromLocalToRemote(host, user, password, s, remotePath);
                }
            }
        }

        public static List<Renci.SshNet.Sftp.SftpFile> GetRemoteFilesOrFoldersList(string host, string user, string password, string remotePath, int type)
        {
            List<Renci.SshNet.Sftp.SftpFile> fList = new List<Renci.SshNet.Sftp.SftpFile>();
            using (SftpClient client = new SftpClient(host, user, password))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();
                var fileList = client.ListDirectory(remotePath);
                foreach (var f in fileList)
                {
                    if (f.Name != "." && f.Name != "..")
                    {
                        if (type == 1)
                        {// Get folders
                            if (f.IsDirectory)
                            {
                                fList.Add(f);
                            }
                        }
                        else
                        {
                            if (f.IsRegularFile)
                            {
                                fList.Add(f);
                            }
                        }
                    }
                }
            }
            return fList;
        }

        public static void CopyDirectoryFromRemoteToLocal(string host, string user, string password, string localPath, string remotePath)
        {
            List<Renci.SshNet.Sftp.SftpFile> folders = GetRemoteFilesOrFoldersList(host, user, password, remotePath, 1);
            if (!FileCommon.FileHelper.IsExistsFolder(localPath))
            {
                FileCommon.FileHelper.CreateFolder(localPath);
            }
            foreach (Renci.SshNet.Sftp.SftpFile f in folders)
            {
                CopyDirectoryFromRemoteToLocal(host, user, password, System.IO.Path.Combine(localPath, f.Name) + @"\", f.FullName);
            }
            List<Renci.SshNet.Sftp.SftpFile> files = GetRemoteFilesOrFoldersList(host, user, password, remotePath, 0);
            foreach (var s in files)
            {
                CopyFileFromRemoteToLocal(host, user, password, System.IO.Path.Combine(localPath, s.Name), s.FullName);
            }
        }

        public static void CopyFileFromRemoteToLocal(string host, string user, string password, string localPath, string remotePath)
        {
            using (SftpClient client = new SftpClient(host, user, password))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();
                bool connected = client.IsConnected;
                // RunCommand(host, user, password, "sudo chmod 777 -R " + remotePath);                    
                var file = File.OpenWrite(localPath);
                client.DownloadFile(remotePath, file);

                file.Close();
                client.Disconnect();
            }
        }

        public static void CopyFileFromLocalToRemote(string host, string user, string password, string localPath, string remotePath)
        {
            using (SftpClient client = new SftpClient(host, user, password))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();
                bool connected = client.IsConnected;
                // RunCommand(host, user, password, "sudo chmod 777 -R " + remotePath);                    
                FileInfo fi = new FileInfo(localPath);
                client.UploadFile(fi.OpenRead(), remotePath + fi.Name, true);
                client.Disconnect();
            }
        }

        public static void DeleteRemoteFolder(string host, string user, string password, string remotePath)
        {
            using (SftpClient client = new SftpClient(host, user, password))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();
                if (DoexRemoteFolderExist(host, user, password, remotePath))
                {
                    RunCommand(host, user, password, "sudo rm -rf '" + remotePath + "'");
                }
                client.Disconnect();
            }
        }

        public static bool DoexRemoteFolderExist(string host, string user, string password, string remotePath)
        {
            using (SftpClient client = new SftpClient(host, user, password))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();
                bool r = client.Exists(remotePath);
                client.Disconnect();
                return r;
            }
        }

        public static void CreateRemoteFolder(string host, string user, string password, string remotePath)
        {
            using (SftpClient client = new SftpClient(host, user, password))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();
                if (!DoexRemoteFolderExist(host, user, password, remotePath))
                {
                    RunCommand(host, user, password, "sudo mkdir -p '" + remotePath + "'");
                    RunCommand(host, user, password, "sudo chmod -R 777 '" + remotePath + "'");
                }
                client.Disconnect();
            }
        }
    }
}
