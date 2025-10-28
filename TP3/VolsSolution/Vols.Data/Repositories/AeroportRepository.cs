using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vols.Data.DTO;


namespace Vols.Data.Repositories
{
    public class AeroportRepository : IAeroportRepository
    {
        private readonly VolsContext m_context;


        public AeroportRepository(VolsContext context)
        {
            m_context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int AjouterAeroport(Aeroport aeroport)
        {
            m_context.Aeroports.Add(aeroport);
            return m_context.SaveChanges();
        }

        public bool ExistsByIata(string codeIata)
        {
           bool existeDeja = m_context.Aeroports.Any(a => a.IataCode == codeIata);
            return existeDeja;
        }

        public Aeroport? GetAeroportById(int idAeroprt)
        {
            throw new NotImplementedException();
        }

        public List<Aeroport> GetListAeroports()
        {
            throw new NotImplementedException();
        }

        public int ModifierAeroport(Aeroport aeroport)
        {
            throw new NotImplementedException();
        }

        public int SupprimerAeroport(int idAeroport)
        {
            throw new NotImplementedException();
        }
    }
}
