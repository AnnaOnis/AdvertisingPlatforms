using FluentValidation;
using AdvertisingPlatforms.Web.HttpModels.Requests;

namespace AdvertisingPlatforms.Web.Validators
{
    public class UpdateAdvertisingPlatformRequestValidator : AbstractValidator<UpdateAdvertisingPlatformRequest>
    {
        public UpdateAdvertisingPlatformRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("AdvertisingPlatform Id is required")
                .NotEqual(Guid.Empty)
                .WithMessage("AdvertisingPlatform Id cannot be empty GUID");

            RuleFor(x => x.AdvertisementId)
                .NotEmpty()
                .WithMessage("Advertisement Id is required")
                .NotEqual(Guid.Empty)
                .WithMessage("Advertisement Id cannot be empty GUID");

            RuleFor(x => x.LocationId)
                .NotEmpty()
                .WithMessage("Location Id is required")
                .NotEqual(Guid.Empty)
                .WithMessage("Location Id cannot be empty GUID");
        }
    }
} 