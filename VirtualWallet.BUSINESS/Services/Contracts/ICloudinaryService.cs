using Microsoft.AspNetCore.Http;

namespace VirtualWallet.BUSINESS.Services.Contracts;

public interface ICloudinaryService
{
    public string UploadProfilePicture(IFormFile file);
}
