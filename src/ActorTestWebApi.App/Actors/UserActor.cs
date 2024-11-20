using ActorTestWebApi.App.Controllers;
using Akka.Actor;

namespace ActorTestWebApi.App.Actors;

public record UserStreakEvent(string UserId, string ActivityType);

public class UserActor : ReceiveActor
{
    public HashSet<string> streaks = new();
    public int ProcessedActivities { get; private set; }
    public string UserId { get; }

    public List<IActorRef> processingActors = new List<IActorRef>();

    public UserActor(string userId)
    {
        UserId = userId;
        processingActors.Add(
            Context.ActorOf(Props.Create(() => new StreakActivityActor()), "streakActivityActor")
        );
        
        Receive<FetchUserDetails>(f => Sender.Tell(new UserDetailsResponse(UserId, ProcessedActivities, streaks.ToList())));
        Receive<Activity>(activity =>
        {
            processingActors.ForEach(actor => actor.Forward(activity));
            ProcessedActivities++;
            Context.Sender.Tell(new ActivityProcessed(UserId, activity.ActivityId));
            // Do something with the activity
        });

        Receive<UserStreakEvent>(@event =>
        {
            streaks.Add(@event.ActivityType);
        });
    }
}

public record ActivityProcessed(string UserId, string ActivityActivityId);
