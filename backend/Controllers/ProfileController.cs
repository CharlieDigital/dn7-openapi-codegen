using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace backend;

/// <summary>
/// Provides endpoints to work with profiles.
/// </summary>
[ApiController]
public class ProfileController : ControllerBase {
  /// <summary>
  /// Injection constructor.
  /// </summary>
  public ProfileController() {

  }

  /// <summary>
  /// Gets the profiles from the backend
  /// </summary>
  [HttpGet("/profiles", Name = nameof(GetProfiles))]
  [ProducesResponseType(typeof(List<Profile>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<Results<UnauthorizedHttpResult, BadRequest<string>, Ok<List<Profile>>>> GetProfiles() {
    // TODO: Check authorization and return
    // return TypedResults.Unauthorized();

    var profiles = new List<Profile> {
      new (
        Guid.NewGuid(),
        "Steve",
        "steve@example.com",
        "555-555-5555",
        new( "1234 Street",
          "Acme City",
          "NJ",
          "10001"
        )
      )
    };

    return await Task.FromResult(TypedResults.Ok(profiles));
  }
}

/// <summary>
/// Represents a user profile.
/// </summary>
/// <param name="Id">The ID of the profile.</param>
/// <param name="Name">The name of the user.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="PhoneNumber">The phone number of the user.</param>
/// <param name="Address">An address associated with the user.</param>
public record Profile(
  [property: Required] Guid Id,
  [property: Required] string Name,
  [property: Required] string Email,
   string PhoneNumber,
  [property: Required] Address Address
);

/// <summary>
/// An address associated with a user profile.
/// </summary>
/// <param name="Street">The street address of the user.</param>
/// <param name="City">The city that the address is located in.</param>
/// <param name="State">The state that the address is located in.</param>
/// <param name="PostalCode">The postal code that the address is located in.</param>
public record Address(
  [property: Required] string Street,
  [property: Required] string City,
  [property: Required] string State,
  [property: Required] string PostalCode
);