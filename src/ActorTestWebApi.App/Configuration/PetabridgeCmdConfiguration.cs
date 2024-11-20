using Akka.Hosting;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Cluster.Sharding;
using Petabridge.Cmd.Host;
using Petabridge.Cmd.Remote;

namespace ActorTestWebApi.App.Configuration;

public static class PetabridgeCmdConfiguration
{
    public static AkkaConfigurationBuilder ConfigurePetabridgeCmd(this AkkaConfigurationBuilder builder)
    {
        return builder.AddPetabridgeCmd(cmd =>
        {
            cmd.RegisterCommandPalette(new RemoteCommands());
        });
    }
}
