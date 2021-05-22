using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiKey4REST.JSON {

    public static class DefaultJsonSerializerOptions {

        public static JsonSerializerOptions Options => new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase ,
            IgnoreNullValues = true
        };
    }
}
