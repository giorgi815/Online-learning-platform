using OLP.Application.Exceptions;

namespace OLP.Application.Services
{
    internal static class ServiceChecks
    {
        public static T Found<T>(T? value, string name) where T : class => value ?? throw new NotFoundException($"{name} not found.");
        public static void Owner(Guid expected, Guid actual)
        {
            if (expected != actual) throw new ForbiddenException("You do not own this resource.");
        }
        public static void Require(bool condition, string message)
        {
            if (!condition) throw new BadRequestException(message);
        }
    }
}
