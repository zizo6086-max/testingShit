using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models.Auth;

namespace Domain.Models.Store;

public class SellerApplication
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string IdCardUrl { get; set; }
    [Required]
    public string City {get; set;}
    [Required]
    public string Country {get; set;}
    [Required]
    public string PostalCode {get; set;}
    [Required]
    public string Address { get; set; }
    public string Note {get; set;}
    public DateTime DateSubmitted { get; set; }
    public string Status { get; set; }
    // nav prop
    public AppUser User { get; set; }
    
}