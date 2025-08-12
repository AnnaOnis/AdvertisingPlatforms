using FluentValidation;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Web.HttpModels.Requests;

namespace AdvertisingPlatforms.Web.Validators
{
    public class UploadFileRequestValidator : AbstractValidator<UploadFileRequest>
    {
        public UploadFileRequestValidator()
        {
            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("File is required");

            RuleFor(x => x.File.Length)
                .GreaterThan(0)
                .WithMessage("File cannot be empty")
                .LessThanOrEqualTo(10 * 1024 * 1024)
                .WithMessage("File size cannot exceed 10MB");

            RuleFor(x => x.File.ContentType)
                .Must(contentType => contentType.Equals(FileConstants.ALLOWED_CONTENT_TYPE, StringComparison.OrdinalIgnoreCase))
                .WithMessage($"Only {FileConstants.ALLOWED_CONTENT_TYPE} files are allowed");

            RuleFor(x => x.File.FileName)
                .Must(fileName => Path.GetExtension(fileName).Equals(FileConstants.ALLOWED_EXTENSION, StringComparison.OrdinalIgnoreCase))
                .WithMessage($"Only {FileConstants.ALLOWED_EXTENSION} files are allowed");
        }
    }
} 