using Castle.Interfaces;

namespace Castle.Helpers
{
    public class RequestHeader : IRequestHeader
    {
        private readonly IJwtUtils _jwtUtils;

        public RequestHeader(IJwtUtils jwtUtils)
        {
            _jwtUtils = jwtUtils;
        }
        public string GetUsername(string? authorization)
        {
            var notFound = "???";

            if (authorization == null || authorization == string.Empty)
            {
                return notFound;
            }

            var jwtToken = authorization.Replace("Bearer ", "");
            var username = _jwtUtils.GetTokenUserName(jwtToken);
            //var username = jwtToken.ToString();

            if (username == null)
            {
                return notFound;
            }

            return username;
        }
    }
}
