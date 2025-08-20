using System;
using System.Collections.Generic;
using MediatR;

namespace iPractice.Api.Models;

public abstract class Entity
{
    public long Id { get; }
    public List<INotification> DomainEvents { get; } = new ();
    protected void AddDomainEvent(INotification domainEvent) => DomainEvents.Add(domainEvent);
    public void ClearDomainEvents() => DomainEvents.Clear();
    public DateTime CreatedOn { get; } = DateTime.UtcNow;
}