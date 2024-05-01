using System;
using System.Collections.Generic;

namespace Endpoints.Models;

public partial class Reservation
{
    public long Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public short GuestNumber { get; set; }

    public float TotalCost { get; set; }

    public long FlatId { get; set; }

    public long ReservedById { get; set; }

    public virtual Flat Flat { get; set; } = null!;

    public virtual User ReservedBy { get; set; } = null!;
}
