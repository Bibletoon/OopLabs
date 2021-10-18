using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Backups.Entities;
using Backups.TcpServer.Common;
using Backups.TcpServer.Common.Commands;

namespace Backups.Server
{
    public class TcpServer
    {
        private ConnectionConfig _config;

        public TcpServer(ConnectionConfig config)
        {
            _config = config;
        }

        public void Run()
        {
            TcpListener server = new TcpListener(IPAddress.Parse(_config.Ip), _config.Port);
            server.Start();
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                using NetworkStream stream = client.GetStream();
                byte[] infoBytes = AcceptBytes(stream);
                CommandInfo info = JsonSerializer.Deserialize<CommandInfo>(infoBytes);

                if (info is null)
                    throw new  FormatException("Wrong format of incoming data.");
                
                ProceedCommand(info, stream);
            }
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
                throw new FormatException("Wrong type of data provided");
            
            string localBackupPath = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}{info.BackupPath}";
            if (!Directory.Exists(localBackupPath))
                Directory.CreateDirectory(localBackupPath);
                
            for (int index = 0; index < info.Count; index++)
            {
                byte[] archiveBytes = AcceptBytes(stream);
                using Package archive = JsonSerializer.Deserialize<Package>(archiveBytes);
                using var archiveFileStream = new FileStream(
                    $"{localBackupPath}/{archive.Name}",
                    FileMode.Create);
                
                archive.Content.CopyTo(archiveFileStream);
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