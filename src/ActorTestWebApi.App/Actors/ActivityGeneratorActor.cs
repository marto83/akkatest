using System.Diagnostics;
using Akka.Actor;
using Akka.Event;
using Akka.Util;
using Activity = ActorTestWebApi.App.Actors.Activity;

namespace ActorTestWebApi.App.Actors;

public record GenerateActivities(int Count);



public class ActivityGeneratorActor : ReceiveActor
{
    List<string> ActivityTypes = ["Login", "QuizAnswered", "VideoWatched", "Like", "ArticleRead", "GameWatched"];
    List<string> UserIds = ["123", "124", "125","126","127","128","129","130","131","132","133","134","135","136","137","138","139","140"];

    private int remainingActivities = 0;
    private long startTime = 0;
    
    public ActivityGeneratorActor()
    {
        
        Receive<GenerateActivities>(message =>
        {
            var random = ThreadLocalRandom.Current;
            startTime = Stopwatch.GetTimestamp();
            var activitiesActor = Context.ActorSelection("/user/users");
            var logger = Context.GetLogger();
            logger.Info("Generating activities: " + message.Count);
            remainingActivities = message.Count;
            for (var i = 0; i < message.Count; i++)
            {
                var userId = UserIds[random.Next(0, UserIds.Count)];
                var type = ActivityTypes[random.Next(0, ActivityTypes.Count)];
                var activity = new Activity(userId, type, Guid.NewGuid().ToString("N"), "{}");
                
                activitiesActor.Tell(activity);
            }
            
        });
        
        Receive<ActivityProcessed>(message =>
        {
            remainingActivities--;
            if (remainingActivities == 0)
            {
                Context.GetLogger().Info("All activities processed");
                if (startTime > 0)
                {
                    var endTime = Stopwatch.GetElapsedTime(startTime);
                    Context.GetLogger().Info("Elapsed time: " + endTime);                    
                }
            }
            // Do something with the activity
        });
    }
}
