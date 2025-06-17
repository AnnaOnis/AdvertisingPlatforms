using FluentValidation;
using AdvertisingPlatforms.Web.HttpModels.Requests;
using System.Text.RegularExpressions;

namespace AdvertisingPlatforms.Web.Validators
{
    public class LocationRequestValidator : AbstractValidator<LocationRequest>
    {
        public LocationRequestValidator()
        {
            RuleFor(x => x.Path)
                .NotEmpty()
                .WithMessage("Location path is required")
                .Matches(@"^[\p{L}\p{N}/\-_]+$", RegexOptions.Compiled)
                .WithMessage("Location path can only contain letters (including Cyrillic), numbers, slashes, hyphens and underscores")
                .MaximumLength(500)
                .WithMessage("Location path cannot exceed 500 characters");
        }
    }
} 