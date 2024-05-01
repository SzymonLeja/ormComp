using System;
using System.Collections.Generic;

namespace Endpoints.Models;

public partial class Flat
{
    public long Id { get; set; }

    public string? Description { get; set; }

    public short Capacity { get; set; }

    public float DailyPricePerPerson { get; set; }

    public long BuildingId { get; set; }

    public string? FlatNumber { get; set; }

    public virtual Building Building { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();
}
