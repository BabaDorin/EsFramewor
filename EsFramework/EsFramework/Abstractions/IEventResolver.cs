using System.Threading.Tasks;

namespace EsFramework.Abstractions
{
    public interface IEventResolver
    {
        Task Apply<TEvent, TEntity>(TEvent @event, TEntity entity)
            where TEvent : IEvent
            where TEntity : class;
    }
}
