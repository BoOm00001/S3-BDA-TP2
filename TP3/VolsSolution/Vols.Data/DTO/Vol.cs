using System;
using System.Collections.Generic;

namespace Vols.Data.DTO;

public partial class Vol
{
    public int IdVol { get; set; }

    public string? NoCircuit { get; set; }

    public DateTime DateDepart { get; set; }

    public int Nbplacemax { get; set; }

    public int Prix { get; set; }

    public virtual ICollection<Inscription> Inscriptions { get; set; } = new List<Inscription>();

    public virtual Circuit? NoCircuitNavigation { get; set; }
}
