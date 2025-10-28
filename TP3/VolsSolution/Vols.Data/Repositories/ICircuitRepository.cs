using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vols.Data.DTO;

namespace Vols.Data.Repositories
{
    public interface ICircuitRepository
    {
          Circuit? GetCircuit(int idArtiste);
        List<Circuit> GetListCircuit();
        int AjouterCircuit(Circuit circuit);
        int ModifierCircuit(Circuit circuit);
        int SupprimerCircuit(int circuit);
    }
}
