using FluentValidation;
using AdvertisingPlatforms.Web.HttpModels.Requests;

namespace AdvertisingPlatforms.Web.Validators
{
    public class AdvertisingPlatformRequestValidator : AbstractValidator<AdvertisingPlatformRequest>
    {
        public AdvertisingPlatformRequestValidator()
        {
            RuleFor(x => x.AdvertisementId)
                .NotEmpty()
                .WithMessage("Advertisement ID is required");

            RuleFor(x => x.LocationId)
                .NotEmpty()
                .WithMessage("Location ID is required");
        }
    }
} 