using FluentValidation;
using AdvertisingPlatforms.Web.HttpModels.Requests;
using System.Text.RegularExpressions;

namespace AdvertisingPlatforms.Web.Validators
{
    public class UpdateLocationRequestValidator : AbstractValidator<UpdateLocationRequest>
    {
        public UpdateLocationRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Location Id is required")
                .NotEqual(Guid.Empty)
                .WithMessage("Location Id cannot be empty GUID");
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