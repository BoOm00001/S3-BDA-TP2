using System;
using System.Collections.Generic;

namespace Vols.Data.DTO;

public partial class Client
{
    public int Noclient { get; set; }

    public string? NomClient { get; set; }

    public string? TelClient { get; set; }

    public string? CodepostalClient { get; set; }

    public string? AdresseClient { get; set; }

    public int IdVille { get; set; }

    public virtual Ville IdVilleNavigation { get; set; } = null!;

    public virtual ICollection<Inscription> Inscriptions { get; set; } = new List<Inscription>();
}
