using Akka.Actor;

namespace ActorTestWebApi.App.Actors;

public class StreakActivityActor : ReceiveActor
{
    private const int NumberOfActivitiesPerStreak = 10;
    private readonly Dictionary<string, int> _streaks = new();
    
    public StreakActivityActor()
    {
        Receive<Activity>(activity =>
        {
            if (!_streaks.TryAdd(activity.Type, 1))
            {
                _streaks[activity.Type]++;
            }

            if (_streaks[activity.Type] >= NumberOfActivitiesPerStreak)
            {
                _streaks[activity.Type] = 0;
                var streakEvent = new UserStreakEvent(activity.UserId, activity.Type);
                Context.Parent.Tell(streakEvent);
            }
        });
    }
}