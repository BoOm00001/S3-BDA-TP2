TP2 - Bases de donnees avancees (VolsBD)
=========================================================

Cours : 420-W34-SF - Bases de donnees avancees
Professeur : Ali Awde
Etablissement : Cegep de Sainte-Foy
Environnement : SQL Server + .NET 8 / Entity Framework Core
Travail en equipe

---------------------------------------------------------

Objectif du projet
------------------
Ce travail pratique vise a renforcer la maitrise de SQL Server (T-SQL) et a introduire la programmation orientee objet via Entity Framework Core.
L'objectif est de manipuler la base de donnees VolsBD a la fois en SQL direct et a travers un modele objet organise selon une architecture en couches (DAL / BLL / UI).

---------------------------------------------------------

Partie 1 - SQL Server (T-SQL)
-----------------------------
Travail effectue directement dans SQL Server Management Studio (SSMS).

Triggers
- tr_inscription sur la table INSCRIPTION :
  - Empeche qu'une inscription se fasse avant la date de depart du vol
  - Empeche qu'un client s'inscrive a un vol complet

Transactions
- Implementation d'une transaction atomique pour transferer un client :
  - Supprime l'inscription de l'ancien vol
  - Ajoute l'inscription dans le nouveau vol (si places disponibles)
  - Assure la coherence des donnees en cas d'erreur

Procedures stockees
- sp_AnnulerVol : Supprime toutes les inscriptions liees a un vol puis le vol lui-meme (dans une transaction).
- sp_TrouverCircuitsAvecCurseur(@Depart, @Arrivee) : Retourne la liste des circuits directs ou avec une escale entre deux aeroports a l'aide d'un curseur.

Fonctions definies par l'utilisateur
- fn_TrouverCircuits(@Depart, @Arrivee) : Retourne une table de circuits (directs ou avec une escale).
- fn_AnalyseInscriptionsAvancee() : Analyse les inscriptions (rang du client, cumul progressif, delai entre inscriptions, etc.).

---------------------------------------------------------

Partie 2 - Entity Framework Core (C#)
-------------------------------------
Developpement d'une solution Visual Studio en architecture 3 couches (DAL / BLL / UI).

Concepts appliques
- Utilisation de Entity Framework Core (Code-First / Database-First)
- Requetes LINQ pour manipuler les entites
- Separation claire des responsabilites (pattern Clean Architecture)
- Validation des regles metier dans la couche BLL

User Stories (US)
US1 - Ajouter un client : verifier les donnees, eviter les doublons de telephone.
US2 - Ajouter un aeroport : code IATA unique, validation des informations.
US3 - Modifier un vol : modifier uniquement date_depart, nbplacemax, prix.
US4 - Afficher les prix selon la devise d'un pays : conversion selon la devise (defaut : CAD).

---------------------------------------------------------

Tests et exemples
-----------------
Partie 1 - SQL
INSERT INTO INSCRIPTION(id_client, id_vol, date_inscription)
VALUES (1, 5, '2024-10-01');

EXEC sp_AnnulerVol @id_vol = 3;

SELECT * FROM dbo.fn_TrouverCircuits('YUL', 'YQB');

Partie 2 - C#
dotnet build
dotnet run

---------------------------------------------------------

Technologies utilisees
----------------------
- SQL Server / T-SQL
- Entity Framework Core 8.0
- .NET 8 / C#
- LINQ
- Visual Studio 2022
- Architecture 3-couches (DAL/BLL/UI)

---------------------------------------------------------

Remise finale
-------------
1. partie1.sql -> tous les scripts SQL
2. Solution Visual Studio complete (EF Core)
3. Compressez les deux dans un fichier ZIP avant depot sur LEA.

Important : aucune modification de la structure des tables VolsBD n'est autorisee (colonnes, cles primaires ou contraintes).

---------------------------------------------------------

Auteur
------
Cherif Ouattara - Etudiant AEC Programmation, bases de donnees et serveurs
Cegep de Sainte-Foy
GitHub : https://github.com/BoOm00001
LinkedIn : https://www.linkedin.com/in/cherif-ouattara/ 
