using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vols.Data.DTO;


namespace Vols.Data.Repositories
{
    public interface IVolsRepository
    {
        Vol? GetVol(int idVol);
        List<Vol> GetListVols();
        void UpdateVolChampsAutorises(int idVol, DateTime dateDepart, int nbPlaceMax, int prix);

        int AjouterVol(Vol vol);
        int SupprimerVol(int vol);
    }
}
