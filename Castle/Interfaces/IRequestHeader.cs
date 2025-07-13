namespace Castle.Interfaces
{
    public interface IRequestHeader
    {
        string GetUsername(string? authorization);
    }
}
