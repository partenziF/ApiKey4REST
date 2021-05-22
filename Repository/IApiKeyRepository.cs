using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiKey4REST {
    public interface IApiKeyRepository   {
        public Task<Boolean> Read( );
        public Task<ApiKey> Find( string aProvidedApiKeyValue);

    }
}
