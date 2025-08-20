using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using iPractice.Api.Models.Psychologists;
using iPractice.Api.Repositories;
using iPractice.Api.UseCases.Psychologists;

namespace iPractice.Api.Validation;

public class TimeSlotValidator : IValidator<CreateNewAvailableTimeSlotCommand>
{
    private readonly IPsychologistSqlRepository psychologistSqlRepository;

    public TimeSlotValidator(IPsychologistSqlRepository psychologistSqlRepository)
    {
        this.psychologistSqlRepository = psychologistSqlRepository;
    }

    public async Task ValidateAsync(CreateNewAvailableTimeSlotCommand request, CancellationToken cancellationToken)
    {
        var incomingTimeSlot = new AvailableTimeSlot(request.From, request.To);
        var overlappingTimeSlots = new List<AvailableTimeSlot>();

        var psychologist = await psychologistSqlRepository.GetPsychologistByIdAsync(request.PsychologistId, cancellationToken);
        psychologist.Calendar.AvailableTimeSlots.ForEach(ts =>
        {
            if (ts.Intersects(incomingTimeSlot))
            {
                overlappingTimeSlots.Add(ts);
            }
        });

        if (overlappingTimeSlots.Any())
        {
            var overlappingDetails = overlappingTimeSlots
                .Select(ts => $"from: {ts.From} to: {ts.To}")
                .Aggregate((current, next) => current + Environment.NewLine + next);
                
            throw new Exception($"The timeslot overlaps with following timeslots:\n{overlappingDetails}");
        }
    }
}
