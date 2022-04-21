# EsFramework
A simple .Net library for Event Sourcing
----

EsFramework is composed out of three actors: IEvent, IEventApplier<TEvent, TEntity>, IAggregatedRoot<TEntity> and EventResolver.

`IEvent` is the interface that has to be implemented by our events. It has a public member - Version (because an event has to have a version).

`IEventApplier<TEvent, TEntity>` - The interface of our event applier. An event applier contains the logic for applying an event of type TEvent to an entity of type TEntity. We can have as many IEventAppliers as we want.

`IAggregatedRoot<TEntity>` - A transient service which can be injected into any component that has to calculate the current state of a domain object TEntity based on a stream of events. IAggregatedRoot contains the current state, the history of changes and an Apply method which takes a collection of events as input parameter. 

`EventResolver` - it connects the dots between events, event appliers and entities. The EventResolver scans the application on startup and stores all instances of IEventAppliers in an in-memory dictionary (if needed, event appliers can be registered to DI container). Also, it exposes an Apply(TEvent event, TEntity entity) method. Internally, when this method is called by the client, the event resolver checks if there is an instance of IEventApplier<TEvent, TEntity>. If so, the event resolver will make use of that instance in order to mutate the entity.

Here is an example of how to use IEvent, IEventApplier and IAggregatedRoot:

```
Event OrderCanceled : IEvent
{
	Int Version;
}
```
  
```
EventApplier OrderCanceledApplier : IEventApplier<OrderCanceled, Order>
{
	Void Apply(OrderCanceled event, Order entity)
	{
		entity.Status = OrderStatus.Canceled;
	}
}
```
  
```
Void CancelOrderCommandHandler
{
	IAggregatedRoot<Order> aggregatedRoot;
	IOrdersRepository repository;

	Ctor(IAggregatedRoot<Order> aggregatedRoot, IOrdersRepository repository)
	{
		this.AggregatedRoot = aggregatedRoot;
		this.repository = repository;
	}
	
	Void Handle(string orderId)
	{
		var previusEvents = repository.GetEvents(orderId);
		aggregatedRoot.ApplyEvents(previousEvents);
		
		var orderCanceledEvent = new OrderCanceledEvent { Version = aggregatedRoot.LastVersion + 1 }
		aggregatedRoot.Apply(orderCanceledEvent);
		
		repository.AppendEvent(orderId, orderCanceledEvent);
	}
}
```

