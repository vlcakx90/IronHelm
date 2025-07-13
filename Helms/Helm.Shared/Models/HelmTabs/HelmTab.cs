namespace Helm.Shared.Models.HelmTabs
{
    public abstract class HelmTab
    {
        public abstract string Id { get; set; }
        public abstract string Title { get; set; }
        public abstract HelmTabType Type { get; }
    }

    public enum HelmTabType
    {
        COMMANDER,
        KNIGHT,        
        HOSTEDFILES,
        SAVEDEVIDENCE,
        EVENTLOGS,
        SERVERLOGS,
        TEAMCHAT
    }
}
