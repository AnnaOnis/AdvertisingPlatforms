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
                .Must(contentType => FileConstants.ALLOWED_CONTENT_TYPE.Contains(contentType, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Допустимые типы файлов: {string.Join(", ", FileConstants.ALLOWED_CONTENT_TYPE)}");

            RuleFor(x => x.File.FileName)
                .Must(fileName => FileConstants.ALLOWED_EXTENSION.Contains(Path.GetExtension(fileName), StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Допустимые расширения файлов: {string.Join(", ", FileConstants.ALLOWED_EXTENSION)}");
        }
    }
} 