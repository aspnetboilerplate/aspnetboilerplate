using Abp.Dependency;
using Abp.IO;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System;
using Abp.IO.Extensions;

namespace Abp.BlobStoring.FileSystem
{
    public class FileSystemBlobProvider : BlobProviderBase, ITransientDependency
    {
        protected IBlobFilePathCalculator FilePathCalculator { get; }

        public FileSystemBlobProvider(IBlobFilePathCalculator filePathCalculator)
        {
            FilePathCalculator = filePathCalculator;
        }

        public override async Task SaveAsync(BlobProviderSaveArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);

            if (!args.OverrideExisting && await ExistsAsync(filePath))
            {
                throw new BlobAlreadyExistsException($"Saving BLOB '{args.BlobName}' does already exists in the container '{args.ContainerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            DirectoryHelper.CreateIfNotExists(Path.GetDirectoryName(filePath));

            var fileMode = args.OverrideExisting
                ? FileMode.Create
                : FileMode.CreateNew;

            await ExecuteWithRetryAsync(async () =>
            {
                using (var fileStream = File.Open(filePath, fileMode, FileAccess.Write))
                {
                    await args.BlobStream.CopyToAsync(
                        fileStream,
                        args.CancellationToken
                    );

                    await fileStream.FlushAsync();
                }

                return true;
            }, args.CancellationToken);
        }

        public override Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            return Task.FromResult(FileHelper.TryToDeleteIfExists(filePath));
        }

        public override Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            return ExistsAsync(filePath);
        }

        public override async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);

            if (!File.Exists(filePath))
            {
                return null;
            }

            return await ExecuteWithRetryAsync(async () =>
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    return await TryCopyToMemoryStreamAsync(fileStream, args.CancellationToken);
                }
            }, args.CancellationToken);
        }

        protected virtual Task<bool> ExistsAsync(string filePath)
        {
            return Task.FromResult(File.Exists(filePath));
        }

        /// <summary>
        /// Executes the given action and retries it (waiting 1 second, then 2 seconds)
        /// if it throws an <see cref="IOException"/>.
        /// </summary>
        protected virtual async Task<T> ExecuteWithRetryAsync<T>(
            Func<Task<T>> action,
            CancellationToken cancellationToken = default)
        {
            const int maxRetryCount = 2;

            for (var retryCount = 1; ; retryCount++)
            {
                try
                {
                    return await action();
                }
                catch (IOException) when (retryCount <= maxRetryCount)
                {
                    await Task.Delay(TimeSpan.FromSeconds(retryCount), cancellationToken);
                }
            }
        }
    }
}