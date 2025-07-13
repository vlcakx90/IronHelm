using Knight.Models.Knight;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Knight.Models.Comm
{
    public abstract class CommModule
    {
        public abstract Task SendCheckIn(KnightMetadata knightMetadata);
        public abstract Task Start();

        public abstract void Stop();

        protected KnightMetadata KnightMetadata;

        protected ConcurrentQueue<Raven.Raven> Inbound = new ConcurrentQueue<Raven.Raven>();
        protected ConcurrentQueue<Raven.Raven> Outbound = new ConcurrentQueue<Raven.Raven>();
        public virtual void Init(KnightMetadata metadata)
        {
            KnightMetadata= metadata;
        }

        public bool RecvData(out IEnumerable<Raven.Raven> ravens)
        {
            if (Inbound.IsEmpty)
            {
                ravens = null;
                return false;
            }

            var list = new List<Raven.Raven>();

            while (Inbound.TryDequeue(out var raven))
            {
                list.Add(raven);
            }

            ravens = list;
            return true;
        }

        public void SendData(Raven.Raven raven)
        {
            Outbound.Enqueue(raven);
        }

        protected IEnumerable<Raven.Raven> GetOutbound()
        {
            var outbound = new List<Raven.Raven>();

            while (Outbound.TryDequeue(out var raven))
            {
                outbound.Add(raven);
            }

            return outbound;
        }
    }

    public abstract class AllyCommModule
    {
        public abstract bool Alive { get; protected set; }

        public abstract ConnectionMode ConnectionMode { get; }

        public abstract void Init();
        public abstract Task Start();
        public abstract Task Run();
        public abstract void Stop();


        public abstract event Func<Raven.Raven, Task> RavenRecieved;
        public abstract event Action OnException;
        public abstract Task SendRaven(Raven.Raven raven);
    }

    public enum ConnectionMode
    {
        SERVER,
        CLIENT
    }
}
