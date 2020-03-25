using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Teams.ConferenceApi.Models;
using Teams.ConferenceApi.Repositories.Interfaces;

namespace Teams.ConferenceApi.Repositories
{
    public class DataRepository<T>
        : IDataRepository<T>
        where T : BaseModel
    {
        private readonly IJsonTextSerializer jsonTextSerializer;
        private readonly BlobContainerClient blobContainerClient;

        public DataRepository(IOptions<StorageConfiguration> storageConfigurationOptions, IJsonTextSerializer jsonTextSerializer)
        {
            this.jsonTextSerializer = jsonTextSerializer ?? throw new ArgumentNullException(nameof(jsonTextSerializer));

            var storageConfiguration = storageConfigurationOptions.Value;
            blobContainerClient = new BlobContainerClient(storageConfiguration.BlobStorage, typeof(T).Name.ToLowerInvariant());
        }

        public async Task<T> CreateItemAsync(T item)
        {
            item.Id = Guid.NewGuid();

            var blobClient = await EnsureBlobItemAsync(item.Id.ToString());
            using var memoryStream = new MemoryStream();
            await jsonTextSerializer.SerializeObjectAsync<T>(memoryStream, item);
            await blobClient.UploadAsync(memoryStream);

            return item;
        }

        public async Task<IEnumerable<T>> FetchAllUsersAsync()
        {
            var results = new List<T>();
            var fileNames = await GetFileNamesAsync();
            foreach (var fileName in fileNames)
            {
                var item = await GetItemByFileNameAsync(fileName);
                results.Add(item);
            }

            return results;
        }

        public async Task<T> FetchItemByIdAsync(string id)
        {
            return await GetItemByFileNameAsync(id);
        }

        public async Task DeleteItemByIdAsync(string id)
        {
            var blobClient = await EnsureBlobItemAsync(id);
            await blobClient.DeleteAsync();
        }

        private async Task<IEnumerable<string>> GetFileNamesAsync()
        {
            await EnsureContainerAsync();

            var results = new List<string>();
            var blobItems = blobContainerClient.GetBlobsAsync();
            var pages = blobItems.AsPages();
            await foreach (var page in pages)
            {
                results.AddRange(page.Values.Select(bi => bi.Name));
            }

            return results;
        }

        private async Task<T> GetItemByFileNameAsync(string fileName)
        {
            var blobClient = await EnsureBlobItemAsync(fileName);
            var azureResponse = await blobClient.DownloadAsync();

            using var memoryStream = new MemoryStream();
            await azureResponse.Value.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return await jsonTextSerializer.DeserializeObjectAsync<T>(memoryStream);
        }

        private async Task EnsureContainerAsync()
        {
            await blobContainerClient.CreateIfNotExistsAsync();
        }

        private async Task<BlobClient> EnsureBlobItemAsync(string fileName)
        {
            await EnsureContainerAsync();
            return blobContainerClient.GetBlobClient(fileName);
        }
    }
}
