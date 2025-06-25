using FluentValidation;
using AdvertisingPlatforms.Web.HttpModels.Requests;
using System.Text.RegularExpressions;

namespace AdvertisingPlatforms.Web.Validators
{
    public class UpdateAdvertisementRequestValidator : AbstractValidator<UpdateAdvertisementRequest>
    {
        public UpdateAdvertisementRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Advertisement Id is required")
                .NotEqual(Guid.Empty)
                .WithMessage("Advertisement Id cannot be empty GUID");
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Advertisement name is required")
                .MaximumLength(200)
                .WithMessage("Advertisement name cannot exceed 200 characters")
                .Matches(@"^[\p{L}\p{N}\s\-_]+$", RegexOptions.Compiled)
                .WithMessage("Advertisement name can only contain letters (including Cyrillic), numbers, spaces, hyphens and underscores");
        }
    }
} 