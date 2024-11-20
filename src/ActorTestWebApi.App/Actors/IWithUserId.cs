namespace ActorTestWebApi.App.Actors;

public interface IWithUserId
{
    string UserId { get; }
}

public sealed record FetchUserDetails(string UserId) : IWithUserId;

public class GetChildren
{
}

public class Activity(string userId, string type, string activityId, string data) : IWithUserId
{
    public string UserId { get; } = userId;
    public string Type { get;  } = type;
    public string ActivityId { get; } = activityId;
    public string Data { get; } = data;
}
