# EsFramework
A simple .Net library for Event Sourcing
----

EsFramework is composed out of three actors: IEvent, IEventApplier<TEvent, TEntity>, IAggregatedRoot<TEntity> and EventResolver.

`IEvent` is the interface that has to be implemented by our events. It has a public member - Version (because an event has to have a version).

`IEventApplier<TEvent, TEntity>` - The interface of our event applier. An event applier contains the logic for applying an event of type TEvent to an entity of type TEntity. We can have as many IEventAppliers as we want.

`IAggregatedRoot<TEntity>` - A transient service which can be injected into any component that has to calculate the current state of a domain object TEntity based on a stream of events. IAggregatedRoot contains the current state, the history of changes and an Apply method which takes a collection of events as input parameter. 

`EventResolver` - it connects the dots between events, event appliers and entities. The EventResolver scans the application on startup and stores all instances of IEventAppliers in an in-memory dictionary (if needed, event appliers can be registered to DI container). Also, it exposes an Apply(TEvent event, TEntity entity) method. Internally, when this method is called by the client, the event resolver checks if there is an instance of IEventApplier<TEvent, TEntity>. If so, the event resolver will make use of that instance in order to mutate the entity.

Here is an example of how to use IEvent, IEventApplier and IAggregatedRoot:

First, register the event resolver and all aggregate roots that you're going to use.
```
services.AddEventResolver(Assembly.GetExecutingAssembly()); // specify which assemblies to be searched for IEventAppliers
services.AddTransient<IAggregateRoot<ShippingOrder>, AggregateRoot<ShippingOrder>>(); // ONLY TRANSIENT
```

```
class OrderCanceled : IEvent
{
	public int Version { get; set; }
}
```
  
```
class OrderCanceledApplier : IEventApplier<OrderCanceled, Order>
{
	public Task Apply(OrderCanceled event, Order entity)
	{
		entity.Status = OrderStatus.Canceled;
		return Task.CompletedTask;
	}
}
```
  
```
class CancelOrderCommandHandler
{
	private readonly IAggregatedRoot<Order> aggregatedRoot;
	private readonly IOrdersRepository repository;

	CancelOrderCommandHandler(IAggregatedRoot<Order> aggregatedRoot, IOrdersRepository repository)
	{
		this.AggregatedRoot = aggregatedRoot;
		this.repository = repository;
	}
	
	public async Task Handle(string orderId)
	{
		var previusEvents = await repository.GetEvents(orderId);
		await aggregatedRoot.ApplyEvents(previousEvents);
		
		var orderCanceledEvent = new OrderCanceledEvent 
		{
			Version = aggregatedRoot.LastVersion + 1
		};
	
		await aggregatedRoot.Apply(orderCanceledEvent);
		await repository.AppendEvent(orderId, orderCanceledEvent);
	}
}
```

