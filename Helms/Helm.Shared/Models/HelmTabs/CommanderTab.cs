
namespace Helm.Shared.Models.HelmTabs
{
    public class CommanderTab : HelmTab
    {
        public override string Id { get; set; } = string.Empty;
        public override string Title { get; set; } = string.Empty;
        public override HelmTabType Type { get; } = HelmTabType.COMMANDER;
    }
}
