using Castle.Models.Raven;
using Castle.Models.Knight;

namespace Castle.Interfaces
{
    public interface IRavenService
    {
        Task HandleInboundRaven(Raven raven);
        Task HandleInboundRavens(IEnumerable<Raven> ravens);
                
        //Task<IEnumerable<Raven>> GetOutboundRavens(string soldierId);
        Task<IEnumerable<Raven>> GetOutboundRavens(KnightMetadata metadata);
    }
}
