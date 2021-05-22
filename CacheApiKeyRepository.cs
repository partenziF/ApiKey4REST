using ApiKey4REST.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiKey4REST {
    public class CacheApiKeyRepository : IApiKeyRepository {
        private IDictionary<String , ApiKey> _cache;

        public CacheApiKeyRepository() {
            this._cache = null;
        }

        public Task<ApiKey> Find( string aProvidedApiKeyValue ) {

            ApiKey o = null;
            if ( _cache != null ) {

                var comparer = StringComparer.OrdinalIgnoreCase;
                var caseInsensitiveDictionary = new Dictionary<string , int>( comparer );

                if ( _cache.TryGetValue( aProvidedApiKeyValue , out o ) ) {

                    return Task.FromResult( o );
                } else {
                    return Task.FromResult( o );
                }
            } else {
                return Task.FromResult( o );
            }
        }

        public Task<Boolean> Read() {
            if ( _cache == null ) {
                this._cache = new List<ApiKey>() {
                    new ApiKey( "36c5f124-489b-4597-aa9c-314e20fdf585" , "Federica" , 1 , new DateTime(),new DateTime().AddYears(1),new List<String>{ApiKeyRoles.Developer } ),
                    new ApiKey( "9c583c0d-da13-4a53-b2c2-4589222baa69" , "Mario" , 2 , new DateTime(),new DateTime().AddYears(1),new List<String>{ApiKeyRoles.Production }  ),
                    new ApiKey( "d24a0a38-a2ea-4c28-ac48-a8cf766a6f5e" , "Stefania" , 3 , new DateTime(),new DateTime().AddYears(1),new List<String>{ApiKeyRoles.Developer,ApiKeyRoles.Tester }  )
                }.ToDictionary( x => x.Token , x => x );
                return Task.FromResult( true );
            } else {
                return Task.FromResult( true );
            }

        }
    }
}
