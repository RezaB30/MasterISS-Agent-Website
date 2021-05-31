using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;

namespace MasterISS_Agent_Website
{
    public static class CacheCast
    {
        public static T GetItem<T>(string cacheName, string key)
        {
            try
            {
                var cache = new MemoryCache(cacheName);
                return (T)cache[key];
            }
            catch (InvalidCastException ex)
            {
                // We can't return null because T can be value type
                // then returns default value for T. For classes it will be null.
                return default(T);
            }
        }
    }
}