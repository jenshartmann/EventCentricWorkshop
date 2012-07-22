using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Framework.EventStore.Implementations.Files
{
    public class FileEventStore : IEventStore
    {
        public IEnumerable<Record> EnumerateHistory()
        {
            // cleanup old pending files
            // load indexes
            // build and save missing indexes
            var datFiles = _path.EnumerateFiles("*.dat");

            foreach (var fileInfo in datFiles.OrderBy(fi => fi.Name))
            {
                // quick cleanup
                if (fileInfo.Length == 0)
                {
                    fileInfo.Delete();
                }

                using (var reader = fileInfo.OpenRead())
                using (var binary = new BinaryReader(reader, Encoding.UTF8))
                {
                    Record result;
                    while (TryReadRecord(binary, out result))
                    {
                        yield return result;
                    }
                }
            }
        }

        public void Append(IEnumerable<Record> newRecords)
        {
            try
            {
                _thread.EnterWriteLock();
                foreach (var record in newRecords)
                {
                    EnsureWriterExists(record.GlobalRevision);
                    PersistInFile(record);
                }
            }
            finally
            {
                _thread.ExitWriteLock();
            }
        }

        private readonly DirectoryInfo _path;

        FileStream _currentWriter;

        readonly ReaderWriterLockSlim _thread = new ReaderWriterLockSlim();

        public FileEventStore(string path)
        {
            _path = new DirectoryInfo(path);

            if (!_path.Exists)
                _path.Create();

            // grab the ownership
            new FileStream(Path.Combine(_path.FullName, "lock"),
                           FileMode.OpenOrCreate,
                           FileAccess.ReadWrite,
                           FileShare.None,
                           8,
                           FileOptions.DeleteOnClose);
        }

        static bool TryReadRecord(BinaryReader binary, out Record result)
        {
            result = null;
            try
            {
                var globalVersion = binary.ReadInt64();
                var version = binary.ReadInt64();
                Guid identifier = Guid.Parse(binary.ReadString());
                var len = binary.ReadInt32();
                var bytes = binary.ReadBytes(len);
                var sha = binary.ReadBytes(20);
                if (sha.All(s => s == 0))
                    throw new InvalidOperationException("definitely failed");

                result = new Record(identifier, version, globalVersion, bytes);
                return true;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }
        }

        void PersistInFile(Record record)
        {
            using (var sha1 = new SHA1Managed())
            {
                using (var memory = new MemoryStream())
                {
                    using (var crypto = new CryptoStream(memory, sha1, CryptoStreamMode.Write))
                    using (var binary = new BinaryWriter(crypto, Encoding.UTF8))
                    {
                        binary.Write(record.GlobalRevision);
                        binary.Write(record.Revision);
                        binary.Write(record.AggregateIdentifier.ToString());
                        binary.Write(record.Data.Length);
                        binary.Write(record.Data);
                    }
                    var bytes = memory.ToArray();

                    _currentWriter.Write(bytes, 0, bytes.Length);
                }
                _currentWriter.Write(sha1.Hash, 0, sha1.Hash.Length);
                _currentWriter.Flush(true);
            }
        }

        void EnsureWriterExists(long globalVersion)
        {
            if (_currentWriter != null) return;

            var fileName = string.Format("{0:00000000}-{1:yyyy-MM-dd-HHmmss}.dat", globalVersion, DateTime.UtcNow);
            _currentWriter = File.OpenWrite(Path.Combine(_path.FullName, fileName));
        }

    }
}