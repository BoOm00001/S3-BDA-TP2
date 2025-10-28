using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;
using Vols.Business;
using Vols.Data;
using Vols.Data.DTO;

namespace ConsoleAppUI
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var ctx = VolsContextFactory.Create();
            Console.WriteLine($"Connexion réussie ? {ctx.Database.CanConnect()}");
            Console.WriteLine($"Nombre de clients : {ctx.Clients.Count()}");

            VolsAppServices nouvelApp = new VolsAppServices();
            AjouterClient(ctx);
            AjouterAeroport(ctx);
            ModifierVol(ctx);
            AfficherVolsParDevise(ctx);

        }

        // US1 //
        public static void AjouterClient(VolsContext context)
        {

            try
            {
                Console.Write("Nom du client : ");
                string? nom = Console.ReadLine();

                Console.Write("Téléphone (ex: 418-555-1212) : ");
                string? telephone = Console.ReadLine();

                Console.Write("No Client unique : ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int code))
                {
                    Console.WriteLine($"Code client : {code}");
                }
                else
                {
                    Console.WriteLine("Entrée invalide. Veuillez entrer un nombre valide.");
                }

                if (string.IsNullOrWhiteSpace(nom))
                    throw new ArgumentException("Le nom est obligatoire.");
                if (string.IsNullOrWhiteSpace(telephone))
                    throw new ArgumentException("Le téléphone est obligatoire.");

                string telNormalise = Regex.Replace(telephone, @"\D", "");

                if (telNormalise.Length < 7 || telNormalise.Length > 15)
                    throw new ArgumentException("Le téléphone doit contenir entre 7 et 15 chiffres.");

                bool existe = context.Clients.Any(c => c.TelClient == telNormalise);
                if (existe)
                    throw new InvalidOperationException($"Un client avec le téléphone {telephone} existe déjà.");

                var client = new Client
                {
                    Noclient = code,
                    NomClient = nom.Trim(),
                    TelClient = telNormalise,
                    CodepostalClient = "G1H 6H8",
                    AdresseClient = "cfdkjsrek frnesjfk",
                    IdVille = 2
                };

                context.Clients.Add(client);
                context.SaveChanges();

                Console.WriteLine($" Client ajouté avec succès ! ID = {client.Noclient}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Autre erreur  " + ex.Message);
            }

        }

        // US2 //
        public static void AjouterAeroport(VolsContext context)
        {

            try
            {
                Console.Write("Code IATA (3 lettres) : ");
                string? iata = Console.ReadLine();

                Console.Write("Nom de l'aéroport : ");
                string? nomAeroport = Console.ReadLine();

                Console.Write("Nom de la ville : ");
                string? nomVille = Console.ReadLine();

                Console.Write("Nom du pays : ");
                string? nomPays = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(iata))
                    throw new ArgumentException("Le code IATA est obligatoire.");
                if (string.IsNullOrWhiteSpace(nomAeroport))
                    throw new ArgumentException("Le nom de l'aéroport est obligatoire.");
                if (string.IsNullOrWhiteSpace(nomVille))
                    throw new ArgumentException("La ville est obligatoire.");
                if (string.IsNullOrWhiteSpace(nomPays))
                    throw new ArgumentException("Le pays est obligatoire.");
                string code = iata.Trim().ToUpperInvariant();
                if (code.Length != 3 || !code.All(char.IsLetter))
                    throw new ArgumentException("Le code IATA doit contenir exactement 3 lettres (A–Z).");

                // 4) Unicité IATA

                bool iataExiste = context.Aeroports.Any(a => a.IataCode == code);
                if (iataExiste)
                    throw new InvalidOperationException($"Un aéroport avec le code IATA '{code}' existe déjà.");

                // 5) Résoudre Pays puis Ville (Ville doit appartenir à ce Pays)

                var pays = context.Pays.FirstOrDefault(p => p.NomPays == nomPays);
                if (pays == null)
                    throw new InvalidOperationException($"Le pays '{nomPays}' n'existe pas.");

                var ville = context.Ville.FirstOrDefault(v => v.NomVille == nomVille);
                if (ville == null)
                    throw new InvalidOperationException($"La ville '{nomVille}' n'existe pas dans le pays '{nomPays}'.");

                // 6) Construire l'entité Aeroport (éviter de pousser une navigation non suivie)

                var aeroport = new Aeroport
                {
                    IataCode = code,
                    NomAeroport = nomAeroport.Trim(),
                    IdVille = ville.IdVille,
                    IdVilleNavigation = null! 
                };

                // 7) Persister
                context.Aeroports.Add(aeroport);
                context.SaveChanges();

                // 8) Confirmation
                Console.WriteLine($"Aéroport ajouté : {aeroport.IataCode} – {aeroport.NomAeroport} ({nomVille}, {nomPays})");
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                Console.WriteLine("DbUpdateException capturée !");
                Console.WriteLine("Message EF : " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception : " + ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur inattendue : " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception : " + ex.InnerException.Message);
                }
            }
        }

        // US3 //

        public static void ModifierVol(VolsContext context)
        {

            try
            {
                // 1) Saisies

                Console.Write("ID du vol à modifier : ");
                if (!int.TryParse(Console.ReadLine(), out int idVol) || idVol <= 0)
                    throw new ArgumentException("ID de vol invalide.");

                Console.Write("Nouvelle date de départ (ex: 2025-10-30 14:30) : ");
                var dateTxt = Console.ReadLine();
                if (!DateTime.TryParseExact(dateTxt, new[] { "yyyy-MM-dd HH:mm", "yyyy-MM-dd", "dd/MM/yyyy HH:mm", "dd/MM/yyyy" },
                                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime nouvelleDate))
                    throw new ArgumentException("Format de date invalide.");

                Console.Write("Nouveau nombre de places max : ");
                if (!int.TryParse(Console.ReadLine(), out int nbPlaceMax) || nbPlaceMax <= 0)
                    throw new ArgumentException("Le nombre de places doit être plus grand 0.");

                Console.Write("Nouveau prix : ");
                if (!int.TryParse(Console.ReadLine(), out int prix) || prix < 0)
                    throw new ArgumentException("Le prix doit être plus grand 0.");

                // 2) Charger l'entité existante 
                var vol = context.Vols.FirstOrDefault(v => v.IdVol == idVol);
                if (vol == null)
                    throw new KeyNotFoundException($"Aucun vol avec l'ID {idVol}.");

                // 3) Modifier UNIQUEMENT les 3 champs autorisés
                vol.DateDepart = nouvelleDate;
                vol.Nbplacemax = nbPlaceMax;
                vol.Prix = prix;

                context.SaveChanges();

                // 4) Relecture/affichage de confirmation
                var volMaj = context.Vols.First(v => v.IdVol == idVol);
                Console.WriteLine($"Vol #{volMaj.IdVol} mis à jour.");
                Console.WriteLine($"   Date départ : {volMaj.DateDepart:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"   Places max  : {volMaj.Nbplacemax}");
                Console.WriteLine($"   Prix        : {volMaj.Prix}");
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                Console.WriteLine("Erreur BD : " + (ex.InnerException?.Message ?? ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        //US4

        public static void AfficherVolsParDevise(VolsContext context, string? nomPays = null)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(nomPays))
                {
                    Console.Write("Afficher les prix dans la devise choisit! Entrez le nom du pays : ");
                    var saisie = Console.ReadLine();

                    while (saisie != null && saisie.Trim() == "?")
                    {
                        // Liste rapide des pays disponibles
                        var paysDisponibles = context.Pays
                                                     .AsNoTracking()
                                                     .OrderBy(p => p.NomPays)
                                                     .Select(p => p.NomPays)
                                                     .ToList();

                        if (paysDisponibles.Count == 0)
                        {
                            Console.WriteLine("Aucun pays dans la table PAYS. Affichage par dfaut.");
                            saisie = ""; 
                            break;
                        }

                        Console.WriteLine("Pays disponibles :");
                        foreach (var nom in paysDisponibles)
                            Console.WriteLine(" - " + nom);

                        Console.Write("Choisissez un pays (ou Enter pour CAD) : ");
                        saisie = Console.ReadLine();
                    }

                    nomPays = string.IsNullOrWhiteSpace(saisie) ? null : saisie?.Trim();
                }

                // 1) Par défaut: CAD
                decimal taux = 1m;
                string devise = "CAD";

                // 2) Si un pays est fourni, on va chercher son taux et sa devise
                if (!string.IsNullOrWhiteSpace(nomPays))
                {
                    // Optionnel: normaliser la saisie pour une comparaison plus souple
                    string cible = nomPays.Trim();

                    var pays = context.Pays
                                      .AsNoTracking()
                                      .FirstOrDefault(p => p.NomPays == cible);

                    if (pays == null)
                        throw new InvalidOperationException($"Le pays '{nomPays}' n'existe pas dans la table PAYS.");

                    devise = pays.CodeMonnaie;
                    taux = pays.Taux;
                }

                // 3) Récupérer les vols (les prix en BD sont en CAD)
                var vols = context.Vols
                                  .AsNoTracking()
                                  .OrderBy(v => v.IdVol)
                                  .Select(v => new
                                  {
                                      v.IdVol,
                                      v.NoCircuit,
                                      v.DateDepart,
                                      v.Nbplacemax,
                                      PrixConverti = (decimal)v.Prix * taux
                                  })
                                  .ToList();

                // 4) Affichage — prix converti + devise
                if (vols.Count == 0)
                {
                    Console.WriteLine("Aucun vol trouvé.");
                    return;
                }

                Console.WriteLine($"\nDevise utilisée : {devise}  (taux appliqué: {taux})");
                foreach (var v in vols)
                {
                    Console.WriteLine(
                        $"Vol #{v.IdVol,-4} | Circuit: {v.NoCircuit,-8} | Prix: {v.PrixConverti:F2} {devise} | Départ: {v.DateDepart:yyyy-MM-dd HH:mm} | Places: {v.Nbplacemax}"
                    );
                }
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                Console.WriteLine("Erreur BD : " + (ex.InnerException?.Message ?? ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

    }
}


