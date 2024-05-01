using System;
using System.Collections.Generic;

namespace Endpoints.Models;

public partial class Facility
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Flat> Flats { get; set; } = new List<Flat>();
}
