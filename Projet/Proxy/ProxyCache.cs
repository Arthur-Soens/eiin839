using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    internal class ProxyCache<T>
    {
        ObjectCache cache;

        public ProxyCache()
        {
            cache = MemoryCache.Default;
        }

        public T Get(string name)
        {
            if (cache.Get(name) != null)
            {
                return (T)cache.Get(name);
            }
            T obj = (T)Activator.CreateInstance(typeof(T), new object[] { name });
            cache.Add(name, obj, ObjectCache.InfiniteAbsoluteExpiration);
            return obj;
        }

        public T Get(string name, double time)
        {
            if (cache.Get(name) != null)
            {
                return (T)cache.Get(name);
            }
            T obj = (T)Activator.CreateInstance(typeof(T), new object[] { name });
            var expirationTime = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(time),
            };
            cache.Add(name, obj, expirationTime);
            return obj;
        }

        public T Get(string name, DateTimeOffset dt)
        {
            if (cache.Get(name) != null)
            {
                return (T)cache.Get(name);
            }

            T obj = (T)Activator.CreateInstance(typeof(T), new object[] { name });
            var expirationTime = new CacheItemPolicy
            {
                AbsoluteExpiration = dt,
            };
            cache.Add(name, obj, expirationTime);
            return obj;
        }
    }
}
