using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiKey4REST.Authorization {
    public static class ApiKeyPolicies {
        public const string OnlyDeveloper = nameof( OnlyDeveloper );
        public const string OnlyProduction = nameof( OnlyProduction );
        public const string OnlyTester = nameof( OnlyTester );
    }
}
