using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iPractice.Api.Controllers.Clients.Dtos;

public record CreateClientDto(
    [Required(ErrorMessage = "Name is required")]
    [MinLength(2, ErrorMessage = "Name must be atleast 2 characters")]
    string Name,

    [Required(ErrorMessage = "Exactly 2 psychologists must be assigned")]
    [MinLength(2, ErrorMessage = "Exactly 2 psychologists must be assigned")]
    [MaxLength(2, ErrorMessage = "Exactly 2 psychologists must be assigned")]
    List<long> InitialPsychologistIds
);
