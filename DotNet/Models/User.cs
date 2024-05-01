using System;
using System.Collections.Generic;

namespace Endpoints.Models;

public partial class User
{
    public long Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<Building> Buildings { get; set; } = new List<Building>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
