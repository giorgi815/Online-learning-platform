
using Org.BouncyCastle.Security.Certificates;

namespace OLP.Application.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
        {
            
        }

        public UnauthorizedException(string message) : base(message)
        {
            
        }
    }
}
