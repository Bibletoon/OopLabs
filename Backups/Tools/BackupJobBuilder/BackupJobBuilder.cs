using System;
using Backups.FileHandlers;
using Backups.FileReaders;
using Backups.Models;
using Backups.StorageAlgorithms;
using Backups.Storages;
using Backups.Tools.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backups.Tools.BackupJobBuilder
{
    public class BackupJobBuilder
        : ISetNameJobBuilder,
          ISetStorageJobBuilder,
          ISetStorageAlgorithmJobBuilder,
          IFinalJobBuilder
    {
        private string _name;
        private IFileReader _fileReader;
        private IStorage _storage;
        private IStorageAlgorithm _storageAlgorithm;
        private IFileArchiver _fileArchiver;

        public ISetNameJobBuilder SetFileReader(IFileReader fileReader)
        {
            ArgumentNullException.ThrowIfNull(fileReader, nameof(fileReader));
            _fileReader = fileReader;
            return this;
        }

        ISetStorageAlgorithmJobBuilder ISetNameJobBuilder.SetName(string name)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            _name = name;
            return this;
        }

        ISetStorageJobBuilder ISetStorageAlgorithmJobBuilder.SetStorageAlgorithm(IStorageAlgorithm algorithm)
        {
            ArgumentNullException.ThrowIfNull(algorithm, nameof(algorithm));
            _storageAlgorithm = algorithm;
            return this;
        }

        IFinalJobBuilder ISetStorageJobBuilder.SetStorage(IStorage storage)
        {
            ArgumentNullException.ThrowIfNull(storage, nameof(storage));
            _storage = storage;
            return this;
        }

        public IFinalJobBuilder SetFileArchiver(IFileArchiver fileArchiver)
        {
            ArgumentNullException.ThrowIfNull(fileArchiver, nameof(fileArchiver));
            _fileArchiver = fileArchiver;
            return this;
        }

        BackupJob IFinalJobBuilder.Build()
        {
            _ = _fileReader ?? throw BackupJobException.ServiceIsMissing(nameof(IFileReader));
            _fileArchiver ??= new ZipFileArchiver();
            _ = _storageAlgorithm ?? throw BackupJobException.ServiceIsMissing(nameof(IStorageAlgorithm));
            _ = _storage ?? throw BackupJobException.ServiceIsMissing(nameof(IStorage));
            return new BackupJob(
                _name,
                _fileReader,
                _fileArchiver,
                _storageAlgorithm,
                _storage);
        }
    }
}