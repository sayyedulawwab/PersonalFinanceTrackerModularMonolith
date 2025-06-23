using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure;
public interface IModuleInstaller
{
    void Install(IServiceCollection services, IConfiguration configuration);
}
