using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vols.Data.DTO;

namespace Vols.Data.Repositories
{
    public interface IClientRepository
    {
        Client? GetClient(int idClient);
        List<Client> GetListClient();
        int AjouterClient(Client client);
        int ModifierClient(Client client);
        int SupprimerClient(int client);
    }
}
