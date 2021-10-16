using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Backups.TcpServer.Common;
using Backups.TcpServer.Common.Commands;

namespace Backups.Server
{
    public class TcpServer
    {
        private bool _canRun = true;
        
        public void Run()
        {
            _canRun = true;
            IPAddress localAddress = IPAddress.Parse("127.0.0.1");
            int port = 8080;
            TcpListener server = new TcpListener(localAddress, port);
            server.Start();
            while (_canRun)
            {
                TcpClient client = server.AcceptTcpClient();
                using NetworkStream stream = client.GetStream();
                byte[] infoBytes = AcceptBytes(stream);
                CommandInfo info = JsonSerializer.Deserialize<CommandInfo>(infoBytes);

                if (info is null)
                    throw new Exception("Wrong type of incoming data.");
                
                ProceedCommand(info, stream);
            }
        }

        public void Stop()
        {
            _canRun = false;
        }

        private void ProceedCommand(CommandInfo info, Stream stream)
        {
            switch (info.CommandName)
            {
                case nameof(SaveCommand):
                    ProceedSaveCommand(stream);
                    break;
            }
        }

        private void ProceedSaveCommand(Stream stream)
        {
            byte[] infoBytes = AcceptBytes(stream);
            SaveCommand info = JsonSerializer.Deserialize<SaveCommand>(infoBytes);

            if (info is null)
                throw new Exception("Wrong type of data provided");
            
            string localBackupPath = $"{Directory.GetCurrentDirectory()}\\{info.BackupPath}";
            if (!Directory.Exists(localBackupPath))
                Directory.CreateDirectory(localBackupPath);
                
            for (int index = 0; index < info.Count; index++)
            {
                byte[] archive = AcceptBytes(stream);
                using var archiveFileStream = new FileStream(
                    $"{localBackupPath}/{index}{info.ArchiveFormat}",
                    FileMode.Create);
                archiveFileStream.Write(archive);
            }
        }

        private byte[] AcceptBytes(Stream stream)
        {
            byte[] lengthBytes = new byte[4];
            stream.Read(lengthBytes);
            int length = BitConverter.ToInt32(lengthBytes);
            byte[] objectBytes = new byte[length];
            stream.Read(objectBytes);
            return objectBytes;
        }
    }
}