﻿using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Protocol.Binary;
using System.Net;

namespace NHibernate.Caches.Elasticache
{
    /// <summary>
    /// Server pool implementing the binary protocol.
    /// </summary>
    public class BinaryPool : ElasticServerPool
    {
        ISaslAuthenticationProvider authenticationProvider;
        IMemcachedClientConfiguration configuration;

        public BinaryPool(IMemcachedClientConfiguration configuration, IElasticConfiguration elasticConfiguration)
            : base(configuration, elasticConfiguration, new BinaryOperationFactory())
        {
            this.authenticationProvider = GetProvider(configuration);
            this.configuration = configuration;
        }

        protected override IMemcachedNode CreateNode(IPEndPoint endpoint)
        {
            return new BinaryNode(endpoint, this.configuration.SocketPool, this.authenticationProvider);
        }

        private static ISaslAuthenticationProvider GetProvider(IMemcachedClientConfiguration configuration)
        {
            // create&initialize the authenticator, if any
            // we'll use this single instance everywhere, so it must be thread safe
            IAuthenticationConfiguration auth = configuration.Authentication;
            if (auth != null)
            {
                System.Type t = auth.Type;
                var provider = (t == null) ? null : Enyim.Reflection.FastActivator.Create(t) as ISaslAuthenticationProvider;

                if (provider != null)
                {
                    provider.Initialize(auth.Parameters);
                    return provider;
                }
            }

            return null;
        }
    }
}

#region [ License information          ]
/* ************************************************************
 * 
 *    Copyright (c) 2010 Attila Kiskó, enyim.com
 *    
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *    
 *        http://www.apache.org/licenses/LICENSE-2.0
 *    
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *    
 * ************************************************************/
#endregion