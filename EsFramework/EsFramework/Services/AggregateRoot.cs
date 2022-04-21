using EsFramework.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.EventSourcing.EsFramework
{
    public class AggregateRoot<TEntity> : IAggregateRoot<TEntity> where TEntity : class, new()
    {
        private readonly IEventResolver eventResolver;

        public AggregateRoot(IEventResolver eventResolver)
        {
            this.eventResolver = eventResolver;

            CurrentState = Activator.CreateInstance<TEntity>();
            ChangeHistory = new List<IEvent>();
        }

        public TEntity CurrentState { get; }

        public IList<IEvent> ChangeHistory { get; }

        public async Task Apply(IList<IEvent> events)
        {
            foreach (var evnt in events)
            {
                await eventResolver.Apply(evnt, CurrentState);
                ChangeHistory.Add(evnt);
            }
        }
    }
}
