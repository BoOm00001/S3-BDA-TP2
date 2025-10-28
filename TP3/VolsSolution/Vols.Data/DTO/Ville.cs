using System;
using System.Collections.Generic;

namespace Vols.Data.DTO;

public partial class Ville
{
    public int IdVille { get; set; }

    public string? NomVille { get; set; }

    public int? IdProvince { get; set; }

    public virtual ICollection<Aeroport> Aeroports { get; set; } = new List<Aeroport>();

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual Province? IdProvinceNavigation { get; set; }
}
