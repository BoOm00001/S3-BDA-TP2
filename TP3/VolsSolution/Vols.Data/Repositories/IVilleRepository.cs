using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vols.Data.DTO;


namespace Vols.Data.Repositories
{
    public interface IVilleRepository
    {
        Ville? GetClient(int idVille);
        List<Ville> GetListVille();
        int AjouterVille(Ville ville);
        int ModifierVille(Ville ville);
        int SupprimerVille(int ville);
    }
}
