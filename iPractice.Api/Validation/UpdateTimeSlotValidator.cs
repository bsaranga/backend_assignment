using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using iPractice.Api.Models.Psychologists;
using iPractice.Api.Repositories;
using iPractice.Api.UseCases.Psychologists;

namespace iPractice.Api.Validation;

public class UpdateTimeSlotValidator : IValidator<UpdateAvailableTimeSlotCommand>
{
    private readonly IPsychologistSqlRepository psychologistSqlRepository;

    public UpdateTimeSlotValidator(IPsychologistSqlRepository psychologistSqlRepository)
    {
        this.psychologistSqlRepository = psychologistSqlRepository;
    }

    public async Task ValidateAsync(UpdateAvailableTimeSlotCommand request, CancellationToken cancellationToken)
    {
        var psychologist = await psychologistSqlRepository.GetPsychologistByIdAsync(request.PsychologistId, cancellationToken);
        var changedTimeSlot = new AvailableTimeSlot(request.From, request.To);

        var overlappingTimeSlots = psychologist.Calendar.AvailableTimeSlots.Where(ts =>
        {
            return ts.Id != request.ExistingAvailableTimeSlotId && ts.Intersects(changedTimeSlot);
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
