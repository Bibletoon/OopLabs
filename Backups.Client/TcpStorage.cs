using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using Backups.Domain.Entities;
using Backups.Domain.FileHandlers;
using Backups.Domain.FileReaders;
using Backups.Domain.Models;
using Backups.Domain.StorageAlgorithms;
using Backups.Domain.Storages;
using Backups.TcpServer.Common;
using Backups.TcpServer.Common.Commands;
using Backups.Tools.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Backups.Client
{
    public class TcpStorage : IStorage
    {
        private readonly IStorageAlgorithm _storageAlgorithm;
        private readonly IFileArchiver _fileArchiver;
        private readonly IConfiguration _configuration;
        private readonly IFileReader _fileReader;

        public TcpStorage(
            IStorageAlgorithm storageAlgorithm,
            IFileArchiver fileArchiver,
            IConfiguration configuration,
            IFileReader fileReader)
        {
            _storageAlgorithm = storageAlgorithm;
            _fileArchiver = fileArchiver;
            _configuration = configuration;
            _fileReader = fileReader;
        }
        
        public RestorePointInfo CreateBackup(string jobName, List<JobObject> objects)
        {
            TcpClient client = new TcpClient();

            if (String.IsNullOrEmpty(_configuration["serverPort"]))
                throw BackupJobException.MissingConfigurationParameter("serverPort");
            
            if (String.IsNullOrEmpty(_configuration["serverIp"]))
                throw BackupJobException.MissingConfigurationParameter("serverIp");
            
            int port = int.Parse(_configuration["serverPort"]);
            client.Connect(_configuration["serverIp"],  port);
            
            using NetworkStream clientStream = client.GetStream();
            List<JobsGroup> fileGroups = _storageAlgorithm.ProceedFiles(objects);
            DateTime creationDateTime = DateTime.Now;
            string backupName = creationDateTime.ToString("dd-MM-yyy-HH-mm-ss-f");
            
            if (string.IsNullOrEmpty(_configuration["jobsPath"]))
                throw BackupJobException.MissingConfigurationParameter("jobsPath");
            
            string currentBackupPath = $"{_configuration["jobsPath"]}/{jobName}/{backupName}";

            var commandInfo = new CommandInfo(nameof(SaveCommand));

            byte[] infoBytes = JsonSerializer.SerializeToUtf8Bytes(commandInfo);
            
            var command = new SaveCommand(currentBackupPath, fileGroups.Count, $".{_fileArchiver.GetArchiveExtension()}");
            
            byte[] commandBytes = JsonSerializer.SerializeToUtf8Bytes(command);

            SendBytes(clientStream, infoBytes);
            SendBytes(clientStream, commandBytes);

            foreach (JobsGroup filesToArchive in fileGroups)
            {
                var filePaths = filesToArchive.Jobs.Select(s => s.Path).ToList();
                using MemoryStream archiveFileStream = new MemoryStream();
                var files = filePaths.Select(_fileReader.ReadFile).ToList();
                _fileArchiver.ArchiveFiles(files, archiveFileStream);
                files.ForEach(f => f.Dispose());
                byte[] array = archiveFileStream.ToArray();
                SendBytes(clientStream, array);
            }
            client.Close();
            return new RestorePointInfo(creationDateTime, objects);
        }

        private void SendBytes(Stream stream, byte[] array)
        {
            stream.Write(BitConverter.GetBytes(array.Length), 0, 4);
            stream.Write(array, 0, array.Length);
        }
    }
}