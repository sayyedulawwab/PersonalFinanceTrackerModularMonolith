using System.Reflection;
using Common.Application;
using Common.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.Users.Infrastructure;
public class UsersModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        Assembly[] assemblies = new[]
        {
            AssemblyReference.Assembly,             // Common.Infrastructure
            Application.AssemblyReference.Assembly  // Common.Application
        };
        var serviceInstallerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IServiceInstaller).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToList();

        foreach (Type installerType in serviceInstallerTypes)
        {
            var installer = (IServiceInstaller)Activator.CreateInstance(installerType)!;
            installer.Install(services, configuration);
        }
    }
}
