using iPractice.Api.Controllers.Clients.Dtos;
using iPractice.Api.Models.Clients;
using iPractice.Api.Repositories;
using iPractice.Api.UseCases.Clients;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using iPractice.Api.Services;

namespace iPractice.Api.Controllers.Clients;

[ApiController]
[Route("[controller]")]
public class ClientController(IMediator mediator, IClientSqlRepository repository, EmailService emailService) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<ClientDetailsDto>> RegisterClient([FromBody] CreateClientDto data)
    {
        if (string.IsNullOrEmpty(data.Name) || data.Name.Length < 2)
        {
            return BadRequest("Name is required and must be at least 2 characters");
        }

        if (data.InitialPsychologistIds == null || data.InitialPsychologistIds.Count != 2)
        {
            return BadRequest("Exactly 2 psychologists must be assigned");
        }

        var client = await mediator.Send(new RegisterClientCommand(data.Name, data.InitialPsychologistIds));

        await emailService.SendWelcomeEmail(data.Name, "welcome@ipractice.com");

        return Created($"/clients/{client.Id}", ClientDetailsDto.From(client));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDetailsDto>> GetClient([FromRoute] long id)
    {
        var client = await mediator.Send(new GetClientQuery(id));
        return Ok(ClientDetailsDto.From(client));
    }

    [HttpPost("{id}/bookings")]
    public async Task<ActionResult<Appointment>> BookAppointment([FromRoute] long id, [FromBody] CreateBookingDto data)
    {
        var client = await repository.GetClientByIdAsync(id, default);
        if (client == null)
        {
            return NotFound($"Client with ID {id} not found");
        }

        var appointment = await mediator.Send(new BookAppointmentCommand(id, data.PsychologistId, data.AvailableTimeSlotId));

        return Ok(appointment); 
    }

    [HttpDelete("{id}/bookings/{appointmentId}")]
    public async Task<ActionResult> CancelAppointment([FromRoute] long id, [FromRoute] string appointmentId)
    {
        await mediator.Send(new CancelAppointmentCommand(id, appointmentId));
        return NoContent();
    }

    [HttpGet("{id}/available-timeslots")]
    public async Task<ActionResult<List<AvailableTimeSlotsResult>>> GetAvailableTimeSlots([FromRoute] long id)
    {
        var availableTimeSlots = await mediator.Send(new GetAvailableTimeSlotsQuery(id));
        return Ok(availableTimeSlots);
    }
}
