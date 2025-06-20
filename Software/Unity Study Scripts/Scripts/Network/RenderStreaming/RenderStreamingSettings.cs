using System;
using System.Threading;
using Unity.RenderStreaming.Signaling;
#if URS_USE_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

namespace Assets
{
    internal static class RenderStreamingSettings
    {
        public static bool EnableHWCodec { get; set; } = false;

        public static SignalingType SignalingType { get; set; } = SignalingType.WebSocket;

        public static string SignalingAddress { get; set; } = "192.168.0.0:80";

        public static bool SignalingSecured { get; set; } = false;

        public static float SignalingInterval { get; set; } = 5;

        public static ISignaling Signaling
        {
            get
            {
                switch (SignalingType)
                {
                    case SignalingType.Furioos:
                    {
                        var schema = SignalingSecured ? "https" : "http";
                        return new FurioosSignaling(
                            $"{schema}://{SignalingAddress}", SignalingInterval, SynchronizationContext.Current);
                    }
                    case SignalingType.WebSocket:
                    {
                        var schema = SignalingSecured ? "wss" : "ws";
                        return new WebSocketSignaling(
                            $"{schema}://{SignalingAddress}", SignalingInterval, SynchronizationContext.Current);
                    }
                    case SignalingType.Http:
                    {
                        var schema = SignalingSecured ? "https" : "http";
                        return new HttpSignaling(
                            $"{schema}://{SignalingAddress}", SignalingInterval, SynchronizationContext.Current);
                    }
                }

                throw new InvalidOperationException();
            }
        }
    }
}