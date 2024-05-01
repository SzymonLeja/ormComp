using System;
using System.Collections.Generic;

namespace Endpoints.Models;

public partial class Building
{
    public long Id { get; set; }

    public long AddressId { get; set; }

    public string? Description { get; set; }

    public long OwnerId { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<Flat> Flats { get; set; } = new List<Flat>();

    public virtual User Owner { get; set; } = null!;
}
