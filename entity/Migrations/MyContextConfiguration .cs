using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Migrations
{
    public class MyContextConfiguration : DbConfiguration
    {
        public MyContextConfiguration()
        {
            string cachePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YOUR_APP_NAME\EFCache\";
            MyDbModelStore cachedDbModelStore = new MyDbModelStore(cachePath);
            IDbDependencyResolver dependencyResolver = new SingletonDependencyResolver(cachedDbModelStore);
            AddDependencyResolver(dependencyResolver);
        }

        private class MyDbModelStore : DefaultDbModelStore
        {
            private static bool useCachedDbModelStore;

            // Note that you should only enable DbContextStore during normal run scenarios without migrations. Migrations are currently not supported and will crash.
            public static void Configure(bool useCachedDbModelStore)
            {
                MyContextConfiguration.useCachedDbModelStore = useCachedDbModelStore;
            }

            public MyContextConfiguration()
            {
                // CachedDbModel store wird derzeit nicht immer verwendet, da er z.b. bei Migrations derzeit noch nicht funktioniert (Exceptions im EF Code)
                if (useCachedDbModelStore)
                {
                    MyDbModelStore cachedDbModelStore = new MyDbModelStore(MyContext.EfCacheDirPath);
                    IDbDependencyResolver dependencyResolver = new SingletonDependencyResolver(cachedDbModelStore);
                    AddDependencyResolver(dependencyResolver);
                }
            }

            private class MyDbModelStore : DefaultDbModelStore
            {
                public MyDbModelStore(string location)
                    : base(location)
                { }

                public override DbCompiledModel TryLoad(Type contextType)
                {
                    string path = GetFilePath(contextType);
                    if (File.Exists(path))
                    {
                        DateTime lastWriteTime = File.GetLastWriteTimeUtc(path);
                        DateTime lastWriteTimeDomainAssembly = File.GetLastWriteTimeUtc(typeof(TypeInYourDomainAssembly).Assembly.Location);
                        if (lastWriteTimeDomainAssembly > lastWriteTime)
                        {
                            File.Delete(path);
                            Tracers.EntityFramework.TraceInformation("Cached db model obsolete. Re-creating cached db model edmx.");
                        }
                    }
                    else
                    {
                        Tracers.EntityFramework.TraceInformation("No cached db model found. Creating cached db model edmx.");
                    }

                    return base.TryLoad(contextType);
                }
            }
        }
    }
}
