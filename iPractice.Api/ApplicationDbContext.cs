using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using iPractice.Api.Models;
using iPractice.Api.Models.Clients;
using iPractice.Api.Models.Psychologists;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPractice.Api;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator) : DbContext(options)
{
    private readonly IMediator _mediator = mediator;

    public DbSet<Psychologist> Psychologists { get; set; }
    public DbSet<Client> Clients { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var domainEntities = ChangeTracker.Entries<Entity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .ToList();
    
            var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToList();
    
            domainEntities.ForEach(e => e.Entity.ClearDomainEvents());
    
            var result = await base.SaveChangesAsync(cancellationToken);
    
            foreach (var domainEvent in domainEvents)
                await _mediator.Publish(domainEvent, cancellationToken);
    
            return result;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Psychologist>(psychologist =>
        {
            psychologist.HasKey(p => p.Id);
            psychologist.OwnsOne(p => p.ClientAssignment, clientAssignment =>
            {
                clientAssignment
                    .Property(c => c.ClientIds)
                    .HasColumnName("AssignedClientIds");
            });

            psychologist.OwnsOne(p => p.Calendar, calendar =>
            {
                calendar.OwnsMany(c => c.AvailableTimeSlots, availableTimeSlot =>
                {
                    availableTimeSlot.HasKey(ats => ats.Id);
                    availableTimeSlot.Property<long>("PsychologistId");
                    availableTimeSlot.WithOwner().HasForeignKey("PsychologistId");
                    availableTimeSlot.Property(ats => ats.From);
                    availableTimeSlot.Property(ats => ats.To);
                    availableTimeSlot.ToTable("AvailableTimeSlotsOfPsychologists");
                });

                calendar.OwnsMany(c => c.BookedAppointments, bookedAppointment =>
                {
                    bookedAppointment.HasKey(ba => ba.Id);
                    bookedAppointment.Property<long>("PsychologistId");
                    bookedAppointment.WithOwner().HasForeignKey("PsychologistId");
                    bookedAppointment.Property(ba => ba.ClientId);
                    bookedAppointment.Property(ba => ba.From);
                    bookedAppointment.Property(ba => ba.To);
                    bookedAppointment.ToTable("BookedAppointmentsOfPsychologists");
                });
            });
        });

        modelBuilder.Entity<Client>(client =>
        {
            client.HasKey(p => p.Id);
            client.OwnsOne(p => p.PsychologistAssignment, clientAssignment =>
            {
                clientAssignment
                    .PrimitiveCollection(c => c.PsychologistIds)
                    .HasColumnName("AssignedPsychologistIds");
            });

            client.OwnsOne(p => p.Calendar, calendar =>
            {
                calendar.OwnsMany(c => c.Appointments, bookedAppointment =>
                {
                    bookedAppointment.HasKey(ba => ba.Id);
                    bookedAppointment.Property<long>("ClientId");
                    bookedAppointment.WithOwner().HasForeignKey("ClientId");
                    bookedAppointment.Property(ba => ba.PsychologistId);
                    bookedAppointment.Property(ba => ba.From);
                    bookedAppointment.Property(ba => ba.To);
                    bookedAppointment.ToTable("ClientScheduledAppointments");
                });
            });
        });
    }
}