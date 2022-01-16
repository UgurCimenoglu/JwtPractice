using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtPractice.JWT
{
    public interface IJwtAuthenticationService
    {
        string Authentication(string userName, string password);
    }
}
