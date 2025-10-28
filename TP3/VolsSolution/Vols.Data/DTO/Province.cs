using System;
using System.Collections.Generic;

namespace Vols.Data.DTO;

public partial class Province
{
    public int IdProvince { get; set; }

    public string? NomProvince { get; set; }

    public int? IdPays { get; set; }

    public virtual Pay? IdPaysNavigation { get; set; }

    public virtual ICollection<Ville> Villes { get; set; } = new List<Ville>();
}
