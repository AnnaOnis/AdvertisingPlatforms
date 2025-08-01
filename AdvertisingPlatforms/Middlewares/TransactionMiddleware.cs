using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.Web.Middlewares
{
    public class TransactionMiddleware
    {
        private readonly RequestDelegate _next;

        public TransactionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
        {
            try
            {
                await _next(context);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.Commit();
            }
            catch
            {
                await unitOfWork.RollBack();
                throw; 
            }
        }
    }
}
