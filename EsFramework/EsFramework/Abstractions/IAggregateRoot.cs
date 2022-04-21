using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsFramework.Abstractions
{
    public interface IAggregateRoot<TEntity> where TEntity : class, new()
    {
        TEntity CurrentState { get; }
        IList<IEvent> ChangeHistory { get; }

        Task Apply(IList<IEvent> events);
    }
}
