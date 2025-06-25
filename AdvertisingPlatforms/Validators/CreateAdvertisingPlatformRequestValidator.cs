using FluentValidation;
using AdvertisingPlatforms.Web.HttpModels.Requests;

namespace AdvertisingPlatforms.Web.Validators
{
    public class CreateAdvertisingPlatformRequestValidator : AbstractValidator<CreateAdvertisingPlatformRequest>
    {
        public CreateAdvertisingPlatformRequestValidator()
        {
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