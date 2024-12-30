using System.ComponentModel.DataAnnotations;

public class UserPreferencesDTO
{

    [Required]
    [MinLength(3, ErrorMessage = "Please select at least 3 categories")]
    public List<Guid> CategoryIds { get; set; }
} 