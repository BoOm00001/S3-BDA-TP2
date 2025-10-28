using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vols.Business.Services;
using Vols.Data;

namespace Vols.Business
{
    public class VolsAppServices
    {
        private readonly VolsContext _context;

        // Expose uniquement en lecture (get)
        public AeroportService AeroportService { get; } 
        public VolService VolService { get; }
        // Ajoute ici d’autres services selon ton projet (ex: PaysService, PrixService, etc.)

        public VolsAppServices()
        {
            // 1) Crée UNE instance partagée du DbContext
            _context = VolsContextFactory.Create();

            // 2) Injecte le même contexte dans chaque service
            AeroportService = new AeroportService(_context);
            VolService = new VolService(_context);
        }

    }
}
