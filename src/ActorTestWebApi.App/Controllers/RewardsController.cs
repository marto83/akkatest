using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.Mvc;
using ActorTestWebApi.App.Actors;

namespace ActorTestWebApi.App.Controllers;


public record UserDetailsResponse(string UserId, int ProcessedActivitys, List<string> Streaks);

[ApiController]
[Route("[controller]")]
public class RewardsController : ControllerBase
{
    private readonly ILogger<RewardsController> _logger;
    private readonly IActorRef _userActor;
    private readonly IActorRef _activityGenerator;

    public RewardsController(ILogger<RewardsController> logger, IRequiredActor<UserActor> userActor, IRequiredActor<ActivityGeneratorActor> activityGenerator)
    {
        _logger = logger;
        _userActor = userActor.ActorRef;
        _activityGenerator = activityGenerator.ActorRef;
    }

    [HttpGet("{userId}")]
    public async Task<UserDetailsResponse> Get(string userId)
    {
        var response = await _userActor.Ask<UserDetailsResponse>(new FetchUserDetails(userId), TimeSpan.FromSeconds(5));
        return response;
    }
    
    [HttpGet("children")]
    public async Task<string> GetChildren()
    {
        var children = await _userActor.Ask<IEnumerable<ActorPath>>(new GetChildren(), TimeSpan.FromSeconds(5));
        return string.Join(", ", children);
    }
    
    [HttpPost("generate/{count}")]
    public async Task<IActionResult> Generate(int count)
    {
        if (count > 0)
        {
            _activityGenerator.Tell(new GenerateActivities(count));
        }

        return Ok($"Generating {count} activities");
    }
}
