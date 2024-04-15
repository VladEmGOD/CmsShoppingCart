using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Infrastucture
{
    public static class SessionExtentions
    {
        public static void SetJson(this ISession session, string key, object value) 
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetJson<T>(this ISession session, string key)
        {
            var sessinData = session.GetString(key);
            return sessinData == null ? default(T) : JsonConvert.DeserializeObject<T>(sessinData);
        }
    }
}
