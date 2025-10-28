using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vols.Data;
using Vols.Data.Repositories;
using Vols.Data.DTO;

namespace Vols.Business.Services
{
    public class AeroportService
    {
        private readonly IAeroportRepository m_repo;

        public AeroportService(VolsContext context)
        {
            m_repo = new AeroportRepository(context);
        }

        public int AjouterAeroport(Aeroport aeroport)
        {
            if (aeroport is null)
                throw new ArgumentNullException(nameof(aeroport));

            // Champs obligatoires
            if (string.IsNullOrWhiteSpace(aeroport.IataCode))
                throw new ArgumentException("Le code IATA est obligatoire.", nameof(aeroport.IataCode));
            if (string.IsNullOrWhiteSpace(aeroport.NomAeroport))
                throw new ArgumentException("Le nom de l'aéroport est obligatoire.", nameof(aeroport.NomAeroport));
            if (aeroport.IdVille <= 0)
                throw new ArgumentException("La ville est obligatoire.", nameof(aeroport.IdVille));

            // Normalisation + validation IATA
            var code = aeroport.IataCode.Trim().ToUpperInvariant();
            if (code.Length != 3 || !code.All(char.IsLetter))
                throw new ArgumentException("Le code IATA doit contenir exactement 3 lettres (A–Z).", nameof(aeroport.IataCode));

            // Unicité
            if (m_repo.ExistsByIata(code))
                throw new InvalidOperationException($"L'aéroport avec le code IATA '{code}' existe déjà.");

            // Appliquer la normalisation avant l'ajout
            aeroport.IataCode = code;

            // Persistance via le repository
            return m_repo.AjouterAeroport(aeroport);
        }


    }
}
