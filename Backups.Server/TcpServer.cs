using System;
using System.Collections.Generic;
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
                case nameof(RemoveCommand):
                    ProceedRemoveCommand(stream);
                    break;
                case nameof(ReadCommand):
                    ProceedReadCommand(stream);
                    break;
            }
        }

        private void ProceedReadCommand(Stream stream)
        {
            byte[] infoBytes = AcceptBytes(stream);
            ReadCommand info = JsonSerializer.Deserialize<ReadCommand>(infoBytes);

            if (!Directory.Exists(info.FolderName))
            {
                var countBytes = BitConverter.GetBytes(0);
                SendBytes(stream, countBytes);
                return;
            }

            var packages = new List<Package>();

            foreach (var filepath in Directory.EnumerateFiles(info.FolderName))
            {
                var ms = new MemoryStream();
                using var fs = File.OpenRead(filepath);
                fs.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                packages.Add(new Package(Path.GetFileName(filepath), ms));
            }

            var packagesCountBytes = BitConverter.GetBytes(packages.Count);
            SendBytes(stream, packagesCountBytes);

            foreach (var package in packages)
            {
                var bytePackage = JsonSerializer.SerializeToUtf8Bytes(package);
                SendBytes(stream, bytePackage);
            }
        }

        private void ProceedRemoveCommand(Stream stream)
        {
            byte[] infoBytes = AcceptBytes(stream);
            RemoveCommand info = JsonSerializer.Deserialize<RemoveCommand>(infoBytes);
            if (info is null)
                throw new FormatException("Wrong type of data provided");

            Directory.Delete(info.FolderName, true);
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

        private void SendBytes(Stream stream, byte[] array)
        {
            stream.Write(BitConverter.GetBytes(array.Length), 0, 4);
            stream.Write(array, 0, array.Length);
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