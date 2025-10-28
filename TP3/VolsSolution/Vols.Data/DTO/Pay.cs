using System;
using System.Collections.Generic;

namespace Vols.Data.DTO;

public partial class Pay
{
    public int IdPays { get; set; }

    public string NomPays { get; set; } = null!;

    public decimal Taux { get; set; }

    public string CodeMonnaie { get; set; } = null!;

    public virtual ICollection<Province> Provinces { get; set; } = new List<Province>();
}
