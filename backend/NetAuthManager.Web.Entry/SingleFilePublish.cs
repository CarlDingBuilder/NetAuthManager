using Furion;
using System.Reflection;

namespace NetAuthManager.Web.Entry;

public class SingleFilePublish : ISingleFilePublish
{
    public Assembly[] IncludeAssemblies()
    {
        return Array.Empty<Assembly>();
    }

    public string[] IncludeAssemblyNames()
    {
        return new[]
        {
            "NetAuthManager.Application",
            "NetAuthManager.Core",
            "NetAuthManager.EntityFramework.Core",
            "NetAuthManager.Web.Core"
        };
    }
}