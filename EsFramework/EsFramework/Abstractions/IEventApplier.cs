﻿using System.Threading.Tasks;

namespace EsFramework.Abstractions
{
    public interface IEventApplier<TEvent, TEntity>
        where TEvent : IEvent
        where TEntity : class
    {
        Task Apply(TEvent @event, TEntity entity);
    }
}
