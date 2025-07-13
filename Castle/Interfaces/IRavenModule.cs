using Castle.Models.Raven;
using Castle.Services;

namespace Castle.Interfaces
{
    public interface IRavenModule
    {
        void Init(RavenService ravenService);
        Task ProcessRaven(Raven raven);
    }
}
