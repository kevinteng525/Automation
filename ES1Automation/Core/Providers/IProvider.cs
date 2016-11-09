using Core.Model;

namespace Core.Providers
{
    public interface IProvider
    {
        /// <summary>
        /// entity help to create IProvider subClass
        /// </summary>
        Provider Provider { get; set; }

        /// <summary>
        /// Apply configs to the instance
        /// </summary>
        /// <param name="config"></param>
        void ApplyConfig(string config);
    }
}
