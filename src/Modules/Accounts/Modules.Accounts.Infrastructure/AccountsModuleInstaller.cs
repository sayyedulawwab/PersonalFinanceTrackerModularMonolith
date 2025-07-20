using System.Reflection;
using Common.Application;
using Common.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.Accounts.Infrastructure;
public class AccountsModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        Assembly[] assemblies =
        [
            AssemblyReference.Assembly,             // Infrastructure
            Application.AssemblyReference.Assembly  // Application
        ];
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
