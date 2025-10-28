using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vols.Data.DTO;


namespace Vols.Data.Repositories
{
    public interface IAeroportRepository
    {
        Aeroport? GetAeroportById(int idAeroport);
        public bool ExistsByIata(string codeIata);

        List<Aeroport> GetListAeroports();
        int AjouterAeroport(Aeroport aeroport);
        int ModifierAeroport(Aeroport aeroport);
        int SupprimerAeroport(int idAeroport);

    }
}
