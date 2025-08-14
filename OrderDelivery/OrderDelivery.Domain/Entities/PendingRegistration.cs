using System.ComponentModel.DataAnnotations;

namespace OrderDelivery.Domain.Entities;

public class PendingRegistration
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsPhoneVerified { get; set; } = false;

    public string? Role { get; set; }

    // JSON data for each step
    public string? BasicInfoJson { get; set; }
    public string? MerchantInfoJson { get; set; }
    public string? DriverInfoJson { get; set; }
    public string? VehicleInfoJson { get; set; }
    public string? ResidenceInfoJson { get; set; }

    // Password hash (set only after phone verification)
    public string? Password { get; set; }

    // Track current registration step
    public int Step { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}