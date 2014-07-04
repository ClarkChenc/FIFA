using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FIFA.Test
{
    public class TestDiscoverer
    {
        AppDomain domain;
        public TestDiscovererProxy Proxy { get; private set; }
        /// <summary>
        /// Domain should be loaded before any operation on Proxy.
        /// </summary>
        public void LoadDomain()
        {
            if (domain == null)
            {
                var proxy_type = typeof(TestDiscovererProxy);
                //setup is quite important cz dependent assemblies may not be found.
                AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                domain = AppDomain.CreateDomain("scan_domain",null, setup);
               
                
                //this step loads this assembly as well as its dependencies
                Proxy = (TestDiscovererProxy)domain.CreateInstanceFromAndUnwrap(
                    proxy_type.Assembly.Location,
                    proxy_type.FullName);
            }

        }

        /// <summary>
        /// When Proxy has done its work, domain should be unloaded.
        /// </summary>
        public void UnloadDomain()
        {
            AppDomain.Unload(domain);
            domain = null;
        }
    }
}
