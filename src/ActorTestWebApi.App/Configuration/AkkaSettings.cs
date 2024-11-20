using System.Net;
using Akka.Cluster.Hosting;
using Akka.Remote.Hosting;

namespace ActorTestWebApi.App.Configuration;

public enum PersistenceMode
{
    InMemory,
    Azure
}
public class AkkaSettings
{
    public string ActorSystemName { get; set; } = "AkkaWeb";

    public bool LogConfigOnStart { get; set; } = false;

    
    

    public PersistenceMode PersistenceMode { get; set; } = PersistenceMode.InMemory;

}
