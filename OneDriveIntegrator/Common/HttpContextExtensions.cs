namespace OneDriveIntegrator.Common;

public static class HttpContextExtensions
{
    public static string GetSignedInUser(this IHttpContextAccessor contextAccessor)
        => contextAccessor.HttpContext?.User.Claims.First(c => c.Type == Constants.UserClaimName).Value!;
}