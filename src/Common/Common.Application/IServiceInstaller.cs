using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Application;
public interface IServiceInstaller
{
    void Install(IServiceCollection services, IConfiguration configuration);
}
