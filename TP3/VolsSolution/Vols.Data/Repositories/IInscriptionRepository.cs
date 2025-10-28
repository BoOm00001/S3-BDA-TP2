using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vols.Data.DTO;


namespace Vols.Data.Repositories
{
    public interface IInscriptionRepository
    {
        Inscription? GetInscription(int idInscription);
        List<Inscription> GetListInscription();
        int AjouterInscription(Inscription client);
        int ModifierInscription(Inscription inscription);
        int SupprimerInscription(int inscription);
    }
}
