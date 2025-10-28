using System;
using System.Collections.Generic;

namespace Vols.Data.DTO;

public partial class Aeroport
{
    public string? NomAeroport { get; set; }

    public int? IdVille { get; set; }

    public string IataCode { get; set; } = null!;

    public virtual ICollection<Circuit> CircuitCodeDepartNavigations { get; set; } = new List<Circuit>();

    public virtual ICollection<Circuit> CircuitCodeDestinationNavigations { get; set; } = new List<Circuit>();

    public virtual Ville? IdVilleNavigation { get; set; }
}
