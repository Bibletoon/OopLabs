using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backups.Entities;
using Backups.FileHandlers;
using Backups.FileReaders;
using Backups.Models;
using Backups.StorageAlgorithms;
using Backups.Storages;
using Backups.TcpServer.Common;
using Backups.TcpServer.Common.Commands;
using Backups.Tools;
using Backups.Tools.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Backups.Client
{
    public class TcpStorage : IStorage
    {
        private ConnectionConfig _config;

        public TcpStorage(ConnectionConfig config)
        {
            _config = config;
        }

        public void WriteFiles(string folderPath, List<Package> files)
        {
            using TcpClient client = new TcpClient();

            client.Connect(_config.Ip,  _config.Port);

            using NetworkStream clientStream = client.GetStream();

            var commandInfo = new CommandInfo(nameof(SaveCommand));

            byte[] infoBytes = JsonSerializer.SerializeToUtf8Bytes(commandInfo);

            var command = new SaveCommand(folderPath, files.Count);

            byte[] commandBytes = JsonSerializer.SerializeToUtf8Bytes(command);

            SendBytes(clientStream, infoBytes);
            SendBytes(clientStream, commandBytes);

            foreach (var file in files)
            {
                byte[] array = JsonSerializer.SerializeToUtf8Bytes(file);
                SendBytes(clientStream, array);
            }
            client.Close();
        }

        public void RemoveFolder(string folderPath)
        {
            using TcpClient client = new TcpClient();

            client.Connect(_config.Ip, _config.Port);

            using NetworkStream clientStream = client.GetStream();

            var commandInfo = new CommandInfo(nameof(RemoveCommand));

            byte[] infoBytes = JsonSerializer.SerializeToUtf8Bytes(commandInfo);

            var command = new RemoveCommand(folderPath);

            byte[] commandBytes = JsonSerializer.SerializeToUtf8Bytes(command);

            SendBytes(clientStream, infoBytes);
            SendBytes(clientStream, commandBytes);

            client.Close();
        }

        public List<Package> ReadFiles(string folderPath)
        {
            using TcpClient client = new TcpClient();

            client.Connect(_config.Ip, _config.Port);

            using NetworkStream clientStream = client.GetStream();

            var commandInfo = new CommandInfo(nameof(ReadCommand));

            byte[] infoBytes = JsonSerializer.SerializeToUtf8Bytes(commandInfo);

            var command = new ReadCommand(folderPath);

            byte[] commandBytes = JsonSerializer.SerializeToUtf8Bytes(command);

            SendBytes(clientStream, infoBytes);
            SendBytes(clientStream, commandBytes);

            byte[] countBytes = AcceptBytes(clientStream);
            int count = BitConverter.ToInt32(countBytes);

            var result = new List<Package>();

            for (int i = 0; i < count; i++)
            {
                var packageBytes = AcceptBytes(clientStream);
                var package = JsonSerializer.Deserialize<Package>(packageBytes);
                result.Add(package);
            }

            client.Close();
            return result;
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