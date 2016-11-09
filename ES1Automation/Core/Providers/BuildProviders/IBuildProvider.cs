using Core.Model;

namespace Core.Providers.BuildProviders
{
    public interface IBuildProvider : IProvider
    {
        void SyncAllBuilds();

        void UpdateBuildStatus(Build build);

        void DeleteBuild(Build build);
    }
}
