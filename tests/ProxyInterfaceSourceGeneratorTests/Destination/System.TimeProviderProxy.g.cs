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

namespace ProxyInterfaceSourceGeneratorTests.Source
{
    public partial class TimeProviderProxy : global::ProxyInterfaceSourceGeneratorTests.Source.ITimeProvider
    {
        static TimeProviderProxy()
        {
            Mapster.TypeAdapterConfig<global::System.TimeProvider, global::ProxyInterfaceSourceGeneratorTests.Source.ITimeProvider>.NewConfig().ConstructUsing(instance98737229 => new global::ProxyInterfaceSourceGeneratorTests.Source.TimeProviderProxy(instance98737229));
            Mapster.TypeAdapterConfig<global::ProxyInterfaceSourceGeneratorTests.Source.ITimeProvider, global::System.TimeProvider>.NewConfig().MapWith(proxy_979750559 => ((global::ProxyInterfaceSourceGeneratorTests.Source.TimeProviderProxy) proxy_979750559)._Instance);

        }


        public global::System.TimeProvider _Instance { get; }
        
        public global::ProxyInterfaceSourceGeneratorTests.Source.ITimeProvider System { get => Mapster.TypeAdapter.Adapt<global::ProxyInterfaceSourceGeneratorTests.Source.ITimeProvider>(TimeProvider.System); }

        public virtual global::System.TimeZoneInfo LocalTimeZone { get => _Instance.LocalTimeZone; }

        public virtual long TimestampFrequency { get => _Instance.TimestampFrequency; }

        public virtual global::System.DateTimeOffset GetUtcNow()
        {
            var result__1870298920 = _Instance.GetUtcNow();
            return result__1870298920;
        }

        public global::System.DateTimeOffset GetLocalNow()
        {
            var result__1410738147 = _Instance.GetLocalNow();
            return result__1410738147;
        }

        public virtual long GetTimestamp()
        {
            var result__1193196790 = _Instance.GetTimestamp();
            return result__1193196790;
        }

        public global::System.TimeSpan GetElapsedTime(long startingTimestamp, long endingTimestamp)
        {
            long startingTimestamp_ = startingTimestamp;
            long endingTimestamp_ = endingTimestamp;
            var result__865310895 = _Instance.GetElapsedTime(startingTimestamp_, endingTimestamp_);
            return result__865310895;
        }

        public global::System.TimeSpan GetElapsedTime(long startingTimestamp)
        {
            long startingTimestamp_ = startingTimestamp;
            var result__865310895 = _Instance.GetElapsedTime(startingTimestamp_);
            return result__865310895;
        }

        public virtual global::System.Threading.ITimer CreateTimer(global::System.Threading.TimerCallback callback, object? state, global::System.TimeSpan dueTime, global::System.TimeSpan period)
        {
            global::System.Threading.TimerCallback callback_ = callback;
            object? state_ = state;
            global::System.TimeSpan dueTime_ = dueTime;
            global::System.TimeSpan period_ = period;
            var result_1335635543 = _Instance.CreateTimer(callback_, state_, dueTime_, period_);
            return result_1335635543;
        }


        public TimeProviderProxy(global::System.TimeProvider instance)
        {
            _Instance = instance;
            
        }
    }
}
#nullable restore