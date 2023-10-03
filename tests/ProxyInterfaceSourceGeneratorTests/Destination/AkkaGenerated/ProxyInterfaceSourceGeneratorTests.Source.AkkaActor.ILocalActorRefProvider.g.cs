//----------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/StefH/ProxyInterfaceSourceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//----------------------------------------------------------------------------------------

#nullable enable
using System;

namespace ProxyInterfaceSourceGeneratorTests.Source.AkkaActor
{
    public partial interface ILocalActorRefProvider
    {
        Akka.Actor.LocalActorRefProvider _Instance { get; }

        Akka.Actor.IActorRef DeadLetters { get; }

        Akka.Actor.IActorRef IgnoreRef { get; }

        Akka.Actor.Deployer Deployer { get; }

        Akka.Actor.IInternalActorRef RootGuardian { get; }

        Akka.Actor.ActorPath RootPath { get; }

        Akka.Actor.Settings Settings { get; }

        Akka.Actor.LocalActorRef SystemGuardian { get; }

        Akka.Actor.IInternalActorRef TempContainer { get; }

        System.Threading.Tasks.Task TerminationTask { get; }

        Akka.Actor.LocalActorRef Guardian { get; }

        Akka.Event.EventStream EventStream { get; }

        Akka.Actor.Address DefaultAddress { get; }

        Akka.Serialization.Information SerializationInformation { get; }

        Akka.Event.ILoggingAdapter Log { get; }



        Akka.Actor.ActorPath TempPath();

        void RegisterExtraName(string name, Akka.Actor.IInternalActorRef actor);

        Akka.Actor.IActorRef RootGuardianAt(Akka.Actor.Address address);

        void RegisterTempActor(Akka.Actor.IInternalActorRef actorRef, Akka.Actor.ActorPath path);

        void UnregisterTempActor(Akka.Actor.ActorPath path);

        Akka.Actor.FutureActorRef<T> CreateFutureRef<T>(System.Threading.Tasks.TaskCompletionSource<T> tcs);

        void Init(Akka.Actor.Internal.ActorSystemImpl system);

        Akka.Actor.IActorRef ResolveActorRef(string path);

        Akka.Actor.IActorRef ResolveActorRef(Akka.Actor.ActorPath path);

        Akka.Actor.IInternalActorRef ActorOf(Akka.Actor.Internal.ActorSystemImpl system, Akka.Actor.Props props, Akka.Actor.IInternalActorRef supervisor, Akka.Actor.ActorPath path, bool systemService, Akka.Actor.Deploy deploy, bool lookupDeploy, bool @async);

        Akka.Actor.Address GetExternalAddressFor(Akka.Actor.Address address);




    }
}