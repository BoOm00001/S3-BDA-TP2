using System;
using System.Collections.Generic;

namespace Vols.Data.DTO;

public partial class Circuit
{
    public string NoCircuit { get; set; } = null!;

    public string? CodeDepart { get; set; }

    public string? CodeDestination { get; set; }

    public int? Duree { get; set; }

    public virtual Aeroport? CodeDepartNavigation { get; set; }

    public virtual Aeroport? CodeDestinationNavigation { get; set; }

    public virtual ICollection<Vol> Vols { get; set; } = new List<Vol>();
}
