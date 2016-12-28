namespace Mood.Services
{
    public interface ISecurity
    {
        bool IsAuthenticated { get; }

        string UserName { get; }
    }
}
