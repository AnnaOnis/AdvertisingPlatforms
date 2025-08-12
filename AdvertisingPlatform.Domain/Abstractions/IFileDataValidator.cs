
namespace AdvertisingPlatforms.Domain.Abstractions
{
    public interface IFileDataValidator<in TData> where TData : IFileData
    {
        void Validate(TData? data);
    }
}
