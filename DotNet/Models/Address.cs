using System;
using System.Collections.Generic;

namespace Endpoints.Models;

public partial class Address
{
    public long Id { get; set; }

    public string City { get; set; } = null!;

    public string Number { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string PostCode { get; set; } = null!;

    public string Country { get; set; } = null!;

    public decimal Longitude { get; set; }

    public decimal Latitude { get; set; }

    public virtual ICollection<Building> Buildings { get; set; } = new List<Building>();
}
