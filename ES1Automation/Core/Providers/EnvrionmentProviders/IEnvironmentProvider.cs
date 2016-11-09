using System.Collections.Generic;
using Core.Model;

namespace Core.Providers.EnvrionmentProviders
{
    public interface IEnvironmentProvider : IProvider
    {
        IList<string> GetAllEnvironment();

        bool IsEnvironmentExist(string environmentName);

        void RequestEnvironment(TestEnvironment environment);

        IList<IDictionary<string, string>> GetEnvironmentConfig(TestEnvironment environment);

        void RefreshEnvironmentStatus(TestEnvironment environment);

        void UpdateEnvironmentConfig(TestEnvironment environment);

        bool IsAllEnvironmentReady();

        bool DisposeEnvironment(TestEnvironment environment);
    }
}
