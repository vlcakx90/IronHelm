
namespace Helm.Shared.Models.HelmTabs
{
    public class KnightTab : HelmTab
    {
        public override string Id { get; set; } = string.Empty;

        public override string Title { get; set; } = string.Empty;

        public override HelmTabType Type { get; } = HelmTabType.KNIGHT;

        //public string KnightId { get; set; } = string.Empty;
    }
}
