using System.Diagnostics;
using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Sharding;
using Akka.Configuration;
using Akka.Discovery.Azure;
using Akka.Discovery.Config.Hosting;
using Akka.Hosting;
using Akka.Management;
using Akka.Management.Cluster.Bootstrap;
using Akka.Persistence.Azure;
using Akka.Persistence.Azure.Hosting;
using Akka.Persistence.Hosting;
using Akka.Remote.Hosting;
using Akka.Util;
using ActorTestWebApi.App.Actors;

namespace ActorTestWebApi.App.Configuration;

public static class AkkaConfiguration
{
    public static IServiceCollection ConfigureWebApiAkka(this IServiceCollection services, IConfiguration configuration
        )
    {
        var akkaSettings = configuration.GetRequiredSection("AkkaSettings").Get<AkkaSettings>();
        Debug.Assert(akkaSettings != null, nameof(akkaSettings) + " != null");

        services.AddSingleton(akkaSettings);

        return services.AddAkka(akkaSettings.ActorSystemName, (builder, sp) =>
        {
            builder.ConfigureActorSystem(sp);
        });
    }

    public static AkkaConfigurationBuilder ConfigureActorSystem(this AkkaConfigurationBuilder builder,
        IServiceProvider sp)
    {
        var settings = sp.GetRequiredService<AkkaSettings>();

        return builder
            .ConfigureLoggers(configBuilder =>
            {
                configBuilder.LogConfigOnStart = settings.LogConfigOnStart;
                configBuilder.AddLoggerFactory();
            })
            .ConfigureCounterActors(sp);
    }

    public static AkkaConfigurationBuilder ConfigureCounterActors(this AkkaConfigurationBuilder builder,
        IServiceProvider serviceProvider)
    {
        var extractor = CreateCounterMessageRouter();

        return builder.WithActors((system, registry, resolver) =>
        {
            var usersParent = system.ActorOf(
                GenericChildPerEntityParent.Props(extractor, s => Props.Create(() => new UserActor(s))),
                "users");
            var logger = serviceProvider.GetService<ILogger<string>>();
            
            logger.LogInformation("Path: " + usersParent.Path);
            registry.Register<UserActor>(usersParent);
            var activityGenerator = system.ActorOf<ActivityGeneratorActor>("generator");
            registry.Register<ActivityGeneratorActor>(activityGenerator);
        });
    }

    public static HashCodeMessageExtractor CreateCounterMessageRouter()
    {
        return HashCodeMessageExtractor.Create(30, o =>
        {
            return o switch
            {
                IWithUserId msg => msg.UserId,
                _ => null
            };
        }, o => o);
    }
}
