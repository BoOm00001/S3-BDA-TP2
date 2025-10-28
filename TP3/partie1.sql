use VolsBD;
Go
 
 EXEC sp_rename 'dbo.Customer', 'Client';
  EXEC sp_rename 'dbo.ville', 'Villes';

 
                                         ------------------------- 1) Trigger tr_inscription (règles de gestion)  ------------------------------------

/* ============================================================
   TRIGGER : tr_inscription
   Table   : INSCRIPTION (noClient, id_vol, date_inscription)
   But     : 1) Refuser toute inscription datée après le départ
             2) Refuser si la capacité nbplacemax est dépassée
   Note    : Gestion SET-based (multi-lignes dans INSERT)
   ============================================================ */



IF OBJECT_ID('dbo.tr_inscription', 'TR') IS NOT NULL
    DROP TRIGGER dbo.tr_inscription;
GO


CREATE TRIGGER dbo.tr_inscription
ON dbo.INSCRIPTION
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    /* --------------------------------------------------------
       1) RÈGLE "date d'inscription <= date de départ"
          - On compare chaque ligne insérée à VOL.date_depart
          - Si une seule ligne viole la règle, on rejette tout
       -------------------------------------------------------- */
   
   IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN Vols v
          ON v.id_vol = i.id_vol
        WHERE i.date_inscription > v.date_depart  -- après départ = interdit
    )
    BEGIN
        RAISERROR(N'Inscription refusée : la date d''inscription ne peut pas être postérieure à la date de départ du vol.', 16, 1);
        ROLLBACK TRANSACTION;  -- on annule l'INSERT
        RETURN;
    END

    /* --------------------------------------------------------
       2) RÈGLE "vol complet"
          - On calcule, par vol concerné, le total après insertion
          - Si total > nbplacemax, on rejette
       -------------------------------------------------------- */

 IF EXISTS (
    SELECT 1
    FROM (SELECT DISTINCT id_vol FROM inserted) d
    JOIN dbo.Vols v ON v.id_vol = d.id_vol
    JOIN dbo.INSCRIPTION i ON i.id_vol = d.id_vol
    GROUP BY d.id_vol, v.nbplacemax
    HAVING COUNT(*) > v.nbplacemax
)
    BEGIN
        RAISERROR(N'Inscription refusée : le vol est complet.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
Go

   --------------- Testes -------------

---Inscription apres la date de départ ---

INSERT INTO dbo.INSCRIPTION (noClient, id_vol, date_inscription)
VALUES (30, 4, '2023-06-30T10:00:00');  

 
 ---Inscription vol complet ---

INSERT INTO dbo.Vols (id_vol,noCircuit, date_depart, nbplacemax, prix)   --Création d'un vol avec maximum 5 places
VALUES (565,'WS8700','2023-07-01T10:00:00', 5, 250);

INSERT INTO dbo.INSCRIPTION (noClient, id_vol, date_inscription)  -- On inscript 5 clients à ce vol.
VALUES 
(30, 565, '2023-06-30T09:00:00'),
(130, 565, '2023-06-30T09:05:00'),
(150, 565, '2023-06-30T09:10:00'),
(210, 565, '2023-06-30T09:15:00'),
(190, 565, '2023-06-30T09:20:00');

INSERT INTO dbo.INSCRIPTION (noClient, id_vol, date_inscription)   -- Test du trigger
VALUES (370, 565, '2023-06-30T09:30:00');



                               ------------------------ 2) Transaction : transfert d’un client d’un vol à un autre  -------------------------------


/* ============================================================
   TRANSFERT D'INSCRIPTION D'UN VOL A UN AUTRE
   Variables : @NoClient, @AncienVol, @NouveauVol
   Règle     : on vérifie d'abord la capacité du nouveau vol
   ============================================================ */


DECLARE @NoClient   INT = 1;    
DECLARE @AncienVol  INT = 100;  
DECLARE @NouveauVol INT = 200;  

BEGIN TRANSACTION;
BEGIN TRY

     /* Petites vérivications*/
   
   IF @AncienVol = @NouveauVol
      BEGIN
        RAISERROR( N'AncienVol et NouveauVol doivent être différents.', 16, 1);
        RETURN;
    END

   IF NOT EXISTS (
    SELECT 1 FROM dbo.INSCRIPTION
    WHERE noClient = @NoClient AND id_vol = @AncienVol
    )
    BEGIN
      RAISERROR(N'Ancien vol : aucune inscription à transférer.', 16, 1);
      RETURN;
    END


    /* 1) Vérifier la capacité du nouveau vol */
   
   DECLARE @Capacite INT, @Occupe INT;
    SELECT @Capacite = nbplacemax FROM Vols WHERE id_vol = @NouveauVol;
    IF @Capacite IS NULL
     BEGIN
        RAISERROR(N'Le nouveau vol n''existe pas.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    SELECT @Occupe = COUNT(*) FROM dbo.INSCRIPTION WHERE id_vol = @NouveauVol;
    IF @Occupe >= @Capacite
     BEGIN
        RAISERROR(N'Nouveau vol complet : transfert impossible.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    /* 2) Supprimer l'ancienne inscription (si présente) */

    DELETE FROM dbo.INSCRIPTION
    WHERE noClient = @NoClient AND id_vol = @AncienVol;

    /* 3) Ajouter la nouvelle inscription (date = maintenant) */
    INSERT INTO dbo.INSCRIPTION(noClient, id_vol, date_inscription)
    VALUES (@NoClient, @NouveauVol, CONVERT(date, GETDATE()));

    /* 4) Tout s'est bien passé → on valide */
    COMMIT TRANSACTION;
    PRINT 'Transfert effectué avec succès.';
END TRY
BEGIN CATCH
    /* Une erreur quelque part → on annule tout */
    ROLLBACK TRANSACTION;
    PRINT CONCAT('Transfert annulé. Erreur : ', ERROR_MESSAGE());
END CATCH;
GO



                                      -------------------------------------------- 3) Procédures stockées  -------------------------------------



---- 3.A) sp_AnnulerVol (supprimer inscriptions + vol, atomique) ----
/* ============================================================
   PROC : sp_AnnulerVol
   Param : @IdVol
   Effet : supprime d'abord INSCRIPTION, puis VOL, en transaction
   ============================================================ */

IF OBJECT_ID('dbo.sp_AnnulerVol', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_AnnulerVol;
GO

CREATE PROCEDURE dbo.sp_AnnulerVol
    @IdVol INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        -- 1) Supprimer toutes les inscriptions du vol
        DELETE FROM dbo.INSCRIPTION WHERE id_vol = @IdVol;

        -- 2) Supprimer le vol lui-même
        DELETE FROM Vols WHERE id_vol = @IdVol;

        -- 3) Valider
        COMMIT TRANSACTION;
        PRINT 'Vol et inscriptions supprimés.';
    END TRY
    BEGIN CATCH
        -- Annuler en cas d'erreur
        ROLLBACK TRANSACTION;
        PRINT CONCAT('Annulation de sp_AnnulerVol. Erreur : ', ERROR_MESSAGE());
    END CATCH
END;
GO



--- 3.B) sp_TrouverCircuitsAvecCurseur (direct + 1 escale, avec curseur)----
/* ============================================================
   PROC : sp_TrouverCircuitsAvecCurseur
   Param : @Depart (IATA 3), @Arrivee (IATA 3)
   Règles: - direct OU une seule escale
           - utilisation d'un CURSEUR sur les 1ers segments
   ============================================================ */


IF OBJECT_ID('dbo.sp_TrouverCircuitsAvecCurseur', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_TrouverCircuitsAvecCurseur;
GO

CREATE PROCEDURE dbo.sp_TrouverCircuitsAvecCurseur
    @Depart  CHAR(3),
    @Arrivee CHAR(3)
AS
BEGIN
    SET NOCOUNT ON;

    /* 0) Vérifier que les deux aéroports existent */
    IF NOT EXISTS (SELECT 1 FROM dbo.AEROPORT WHERE IATA_CODE = @Depart)
       OR NOT EXISTS (SELECT 1 FROM dbo.AEROPORT WHERE IATA_CODE = @Arrivee)
    BEGIN
        SELECT 
            CAST(NULL AS CHAR(3)) AS code_depart,
            CAST(NULL AS CHAR(3)) AS code_intermediaire,
            CAST(NULL AS CHAR(3)) AS code_finale,
            CAST(NULL AS INT)     AS duree_totale;
        RETURN;
    END

    /* Table variable pour accumuler le résultat */
    DECLARE @R TABLE(
        code_depart       CHAR(3),
        code_intermediaire CHAR(3) NULL,
        code_finale       CHAR(3),
        duree_totale      INT
    );

    /* 1) Ajouter les trajets DIRECTS d'abord (pas besoin de curseur pour ça) */
    INSERT INTO @R(code_depart, code_intermediaire, code_finale, duree_totale)
    SELECT c.code_depart, NULL, c.code_destination, c.duree
    FROM dbo.CIRCUIT c
    WHERE c.code_depart = @Depart
      AND c.code_destination = @Arrivee;

    /* 2) Préparer un CURSEUR sur les 1ers segments qui partent de @Depart */
    DECLARE cur_firstlegs CURSOR FAST_FORWARD FOR
        SELECT code_destination, duree
        FROM dbo.CIRCUIT
        WHERE code_depart = @Depart;

    DECLARE @Escale CHAR(3), @Duree1 INT;

    OPEN cur_firstlegs;
    FETCH NEXT FROM cur_firstlegs INTO @Escale, @Duree1;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        /* 2.a) Si l'escale atteint directement l'arrivée → déjà couvert en DIRECT.
                On traite surtout le cas "UNE ESCALE" (escale -> arrivée). */
        INSERT INTO @R(code_depart, code_intermediaire, code_finale, duree_totale)
        SELECT @Depart, @Escale, c2.code_destination, @Duree1 + c2.duree
        FROM dbo.CIRCUIT c2
        WHERE c2.code_depart = @Escale
          AND c2.code_destination = @Arrivee;

        FETCH NEXT FROM cur_firstlegs INTO @Escale, @Duree1;
    END

    CLOSE cur_firstlegs;
    DEALLOCATE cur_firstlegs;

    /* 3) Si aucun chemin trouvé → retourner une ligne NULL */
    IF NOT EXISTS (SELECT 1 FROM @R)
    BEGIN
        SELECT 
            CAST(NULL AS CHAR(3)) AS code_depart,
            CAST(NULL AS CHAR(3)) AS code_intermediaire,
            CAST(NULL AS CHAR(3)) AS code_finale,
            CAST(NULL AS INT)     AS duree_totale;
        RETURN;
    END

    /* 4) Sinon, renvoyer le résultat */
    SELECT * FROM @R;
END;
GO

---------------- Exemple d'utilisation -------------------------
 EXEC dbo.sp_TrouverCircuitsAvecCurseur @Depart='ZRH', @Arrivee='FRA';
 



                                                    ---------------------------------------------- 4) Fonctions --------------------------------------------------




------------- 4.A) fn_TrouverCircuits (table-valued, direct + une escale)---------
/* ============================================================
   FONCTION : fn_TrouverCircuits
   Params   : @Depart, @Arrivee (IATA 3)
   Retour   : TABLE (code_depart, code_intermediaire, code_finale, duree_totale)
   Règles   : Direct OU une seule escale
   ============================================================ */


IF OBJECT_ID('dbo.fn_TrouverCircuits', 'IF') IS NOT NULL
    DROP FUNCTION dbo.fn_TrouverCircuits;
GO

CREATE FUNCTION dbo.fn_TrouverCircuits
(
    @Depart  CHAR(3),
    @Arrivee CHAR(3)
)
RETURNS TABLE
AS
RETURN
(
    /* On filtre implicitement par l'existence des aéroports via des JOINs.
       Si l'un n'existe pas, aucune ligne ne ressortira. */

    /* 1) Trajets DIRECTS */
    SELECT
        c.code_depart,
        CAST(NULL AS CHAR(3)) AS code_intermediaire,
        c.code_destination     AS code_finale,
        c.duree                AS duree_totale
    FROM dbo.CIRCUIT c
    JOIN dbo.AEROPORT a1 ON a1.IATA_CODE = @Depart
    JOIN dbo.AEROPORT a2 ON a2.IATA_CODE = @Arrivee
    WHERE c.code_depart     = @Depart
      AND c.code_destination= @Arrivee

    UNION ALL

    /* 2) Trajets à UNE ESCALE : c1 (départ -> escale) + c2 (escale -> arrivée) */
    SELECT
        c1.code_depart,
        c1.code_destination   AS code_intermediaire,
        c2.code_destination   AS code_finale,
        (c1.duree + c2.duree) AS duree_totale
    FROM dbo.CIRCUIT c1
    JOIN dbo.CIRCUIT c2
      ON c2.code_depart = c1.code_destination           -- enchaînement par l'escale
    JOIN dbo.AEROPORT a1 ON a1.IATA_CODE = @Depart
    JOIN dbo.AEROPORT a2 ON a2.IATA_CODE = @Arrivee
    WHERE c1.code_depart      = @Depart
      AND c2.code_destination = @Arrivee
);
GO

-- -- Exemple :
 SELECT * FROM dbo.fn_TrouverCircuits('YUL', 'YQB');




----------- 4.B) fn_AnalyseInscriptionsAvancee (rang, total, cumul, délai) ----------
/* ============================================================
   FONCTION : fn_AnalyseInscriptionsAvancee
   Retour   : TABLE avec rang, total, cumul, délai
   Remarque : cumul = ROW_NUMBER (1,2,3,...) dans l'ordre chrono
   ============================================================ */



IF OBJECT_ID('dbo.fn_AnalyseInscriptionsAvancee', 'IF') IS NOT NULL
    DROP FUNCTION dbo.fn_AnalyseInscriptionsAvancee;
GO

CREATE FUNCTION dbo.fn_AnalyseInscriptionsAvancee()
RETURNS TABLE
AS
RETURN
(
    SELECT
        i.id_vol,                         -- vol
        i.noClient,                       -- client
        i.date_inscription,               -- date d'inscription

        -- rang du client dans son vol selon la date (tie-breaker = noClient)
        ROW_NUMBER() OVER (
            PARTITION BY i.id_vol
            ORDER BY i.date_inscription, i.noClient
        ) AS rang_client,

        -- nombre total d'inscriptions pour ce vol
        COUNT(*) OVER (PARTITION BY i.id_vol) AS nb_total_par_vol,

        -- cumul progressif (1,2,3,...) dans l'ordre chronologique
        ROW_NUMBER() OVER (
            PARTITION BY i.id_vol
            ORDER BY i.date_inscription, i.noClient
        ) AS cumul_progressif,

        -- délai (jours) depuis l'inscription précédente du même vol
        DATEDIFF(
            DAY,
            LAG(i.date_inscription) OVER (
                PARTITION BY i.id_vol
                ORDER BY i.date_inscription, i.noClient
            ),
            i.date_inscription
        ) AS delai_depuis_precedente
    FROM dbo.INSCRIPTION i
);
GO

------ Exemple -----
SELECT * FROM dbo.fn_AnalyseInscriptionsAvancee()
ORDER BY id_vol, rang_client;










