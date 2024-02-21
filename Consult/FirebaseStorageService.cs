using Google.Cloud.Storage.V1;

public class FirebaseStorageService
{
    private readonly StorageClient _storageClient;
    private const string BucketName = "gs://consult673.appspot.com";
    public FirebaseStorageService(StorageClient storageClient)
    {
        _storageClient = storageClient;
    }
    public async Task<Uri> UploadFile(string name, IFormFile file)
    {
        var randomGuid = Guid.NewGuid();
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var blob = await _storageClient.UploadObjectAsync(BucketName, $"{name}-{randomGuid}", file.ContentType, stream);
        var photoUri = new Uri(blob.MediaLink);
        return photoUri;
    }
}

