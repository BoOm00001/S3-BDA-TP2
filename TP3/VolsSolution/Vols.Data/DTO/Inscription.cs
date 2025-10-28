using System;
using System.Collections.Generic;

namespace Vols.Data.DTO;

public partial class Inscription
{
    public int Noclient { get; set; }

    public int IdVol { get; set; }

    public DateTime DateInscription { get; set; }

    public virtual Vol IdVolNavigation { get; set; } = null!;

    public virtual Client NoclientNavigation { get; set; } = null!;
}
