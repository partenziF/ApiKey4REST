using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiKey4REST {
    public class ApiKey {
        public string Token { get; }
        public string UserName { get; }
        public int UserId { get; }
        public DateTime Created { get; }
        public DateTime Expire { get; }
        public IReadOnlyCollection<string> Roles { get; }


        public ApiKey( string token , string userName , int userId , DateTime created, DateTime expire, IReadOnlyCollection<string> roles ) {
            this.Token = token ?? throw new ArgumentNullException( nameof( token ) );
            this.UserName = userName ?? throw new ArgumentNullException( nameof( userName ) );
            this.UserId = userId;
            this.Created = created;
            this.Expire = expire;
            this.Roles = roles ?? throw new ArgumentNullException( nameof( roles ) );
        }

    }
}
