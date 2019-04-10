using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SSHTransfer
{
    public static class SSHTransfer
    {
        /// <summary>
        /// Create a test folder on a server
        /// </summary>
        /// <param name="serverName">The server to connect to</param>
        /// <param name="userName">The user to log into</param>
        /// <param name="password">The password of the user</param>
        public static string CreateFiles(string serverName, string userName, string password)
        {
            using (SftpClient client = new SftpClient(serverName, userName, password))
            {
                client.Connect();

                // cd TestDir
                if (!client.Exists("TestDir"))
                    client.CreateDirectory("TestDir");
                client.ChangeDirectory("TestDir");

                // Make new dir based on timestamp + random guid and cd to it
                string dirName = $"{DateTime.Now.ToString("ddMMyyyy-HHmmss")}_{Guid.NewGuid()}";
                client.CreateDirectory(dirName);
                client.ChangeDirectory(dirName);

                // xfer files from SampleData
                Parallel.ForEach(Directory.GetFiles(@"SampleData"), k =>
                {
                    client.UploadFile(File.OpenRead(k), k);
                });
                
                client.Disconnect();

                return dirName;
            }
        }

        /// <summary>
        /// Delete a test folder on a server
        /// </summary>
        /// <param name="serverName">The server to connect to</param>
        /// <param name="userName">The user to log into</param>
        /// <param name="password">The password of the user</param>
        /// <param name="folderName">The name of the test folder to delete</param>
        public static void DeleteFiles(string serverName, string userName, string password, string folderName)
        {
            using (SftpClient client = new SftpClient("serverName", userName, password))
            {
                client.Connect();

                // cd TestDir
                if (!client.Exists("TestDir"))
                    return;
                client.ChangeDirectory("TestDir");

                // Delete folder
                if (!string.IsNullOrWhiteSpace(folderName) && client.Exists(folderName))
                {
                    // client.DeleteDirectory is not recursive, so have to go in and delete every file first
                    Parallel.ForEach(client.ListDirectory(folderName), k =>
                    {
                        try
                        {
                            client.DeleteFile(k.FullName);
                        }
                        // Catch delete failures coming from trying to delete './' or '../'
                        catch (SshException) { }
                    });

                    client.DeleteDirectory(folderName);
                }

                client.Disconnect();
            }
        }
    }
}
