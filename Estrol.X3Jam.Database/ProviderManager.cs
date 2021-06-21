using Estrol.X3Jam.Database.Providers;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Database {
    public class ProviderManager {
        private List<Type> providers;

        public ProviderManager() {
            providers = new();

            var asmNames = DependencyContext.Default.GetDefaultAssemblyNames();
            var type = typeof(ProviderBase);

            providers = asmNames.Select(Assembly.Load)
                .SelectMany(t => t.GetTypes())
                .Where(p => p.GetTypeInfo().IsSubclassOf(type)).ToList();
        }

        public Type Get(string name) {
            Type type = providers.Find(p => p.GetTypeInfo().Name == name);
            if (type == null) {
                return null;
            }

            return type;
        }
    }
}
