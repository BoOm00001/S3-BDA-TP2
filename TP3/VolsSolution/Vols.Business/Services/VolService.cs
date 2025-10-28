using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vols.Data;
using Vols.Data.Repositories;

namespace Vols.Business.Services
{
    public class VolService
    {
        private readonly VolsRepository _repo;
        private readonly VolsContext _context;

        public VolService(VolsContext context)
        {
            _context = context;
            _repo = new VolsRepository(context);
        }

        // Modifier un vol
        public void ModifierVol(int idVol, DateTime dateDepart, int nbPlaceMax, int prix)
        {
            if (nbPlaceMax <= 0)
                throw new ArgumentException("Le nombre de places doit être supérieur à 0.");
            if (prix < 0)
                throw new ArgumentException("Le prix doit être positif.");

            var vol = _repo.GetVol(idVol) ?? throw new InvalidDataException("Vol introuvable.");
            _repo.UpdateVolChampsAutorises(idVol, dateDepart, nbPlaceMax, prix); 
            Console.WriteLine("Vol mis à jour avec succès !");
        }

        // Afficher les prix selon la devise
        public void AfficherVolsParDevise(string? nomPays = null)
        {
            var vols = _repo.GetListVols();

            string devise = "CAD";
            decimal taux = 1m;

            if (!string.IsNullOrWhiteSpace(nomPays) && !nomPays.Equals("Canada", StringComparison.OrdinalIgnoreCase))
            {
                var pays = _context.Pays.FirstOrDefault(p => p.NomPays == nomPays)
                    ?? throw new InvalidOperationException($"Le pays '{nomPays}' n'existe pas.");
                devise = pays.CodeMonnaie;
                taux = pays.Taux;
            }

            foreach (var vol in vols)
            {
                Console.WriteLine($"Vol #{vol.IdVol} - Prix : {vol.Prix * taux:F2} {devise} - Départ : {vol.DateDepart:yyyy-MM-dd}");
            }
        }
    }
}
