using iPractice.Api.DomainEvents;
using iPractice.Api.UseCases;
using System.Collections.Generic;

namespace iPractice.Api.Models.Clients;

public class Client : Entity
{
    public string Name { get; private set; }
    public Calendar Calendar { get; } = Calendar.Empty();
    public PsychologistAssignment PsychologistAssignment { get; }

    private Client()
    {

    }

    private Client(string name, PsychologistAssignment psychologistAssignment)
    {
        Name = name;
        PsychologistAssignment = psychologistAssignment;
    }

    public static Client Create(string name, List<long> psychologistIds) =>
        new(name, PsychologistAssignment.InitializeFrom(psychologistIds));

    public List<long> GetPsychologists() => PsychologistAssignment.PsychologistIds;

    public Appointment BookAppointment(TimeRange timeSlot, long psychologistId)
    {
        var appointment = Calendar.BookAppointment(timeSlot, psychologistId);
        AddDomainEvent(new AppointmentBookedEvent(appointment, Name));
        return appointment;
    }

    public CancelledAppointment CancelBookedAppointment(string bookedAppointmentId) => Calendar.CancelAppointment(bookedAppointmentId);
}