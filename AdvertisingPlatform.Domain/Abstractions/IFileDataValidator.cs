
namespace AdvertisingPlatforms.Domain.Abstractions
{
    public interface IFileDataValidator<TData> where TData : IFileData
    {
        void Validate(TData? data);
    }
}
