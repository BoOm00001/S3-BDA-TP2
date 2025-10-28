using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vols.Data.DTO;


namespace Vols.Data.Repositories
{
    public class VolsRepository : IVolsRepository
    {
        private readonly VolsContext m_context;


        public VolsRepository(VolsContext context)
        {
            m_context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public int AjouterVol(Vol vol)
        {
            throw new NotImplementedException();
        }

        public void UpdateVolChampsAutorises(int idVol, DateTime dateDepart, int nbPlaceMax, int prix)
        {
            var existing = m_context.Vols.FirstOrDefault(v => v.IdVol == idVol)
                ?? throw new KeyNotFoundException($"Vol {idVol} introuvable.");

            existing.DateDepart = dateDepart;
            existing.Nbplacemax = nbPlaceMax;
            existing.Prix = prix;

            m_context.SaveChanges();
        }

        public List<Vol> GetListVols()
        {
            return m_context.Vols.AsNoTracking().OrderBy(v => v.IdVol).ToList();
        }


        public Vol? GetVol(int idVol)
        {
            if (idVol <= 0 || idVol == null)
            {
                throw new ArgumentException(nameof(idVol) + "Est invalide");
            }

            Vol volRecherche = m_context.Vols.FirstOrDefault(v => v.IdVol == idVol);

            if (volRecherche == null)
            {
                throw new InvalidDataException($"Aucun vol n'a cet ID: {idVol}");
            }

            return volRecherche;
        }

        public int SupprimerVol(int iDVol)
        {
            var vol = GetVol(iDVol);
            m_context.Vols.Remove(vol);
            return m_context.SaveChanges();
        }
    }
}
