PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Lieferanten (
    LieferantId      INTEGER PRIMARY KEY AUTOINCREMENT,
    LieferantenNr    TEXT NOT NULL UNIQUE,
    Name             TEXT NOT NULL,
    Aktiv            INTEGER NOT NULL DEFAULT 1,
    AngelegtAm       TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS Artikel (
    ArtikelId        INTEGER PRIMARY KEY AUTOINCREMENT,
    ArtikelNr        TEXT NOT NULL UNIQUE,
    Bezeichnung      TEXT NOT NULL,
    Einheit          TEXT NOT NULL,
    Mindestbestand   REAL NOT NULL DEFAULT 0,
    LieferantId      INTEGER NOT NULL,
    Aktiv            INTEGER NOT NULL DEFAULT 1,
    AngelegtAm       TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (LieferantId) REFERENCES Lieferanten (LieferantId)
);

CREATE TABLE IF NOT EXISTS Bestand (
    BestandId        INTEGER PRIMARY KEY AUTOINCREMENT,
    ArtikelId        INTEGER NOT NULL,
    Lagerplatz       TEXT NOT NULL,
    Lagereinheit     TEXT NOT NULL,
    Menge            REAL NOT NULL DEFAULT 0,
    Palettenanzahl   INTEGER NOT NULL DEFAULT 0,
    LetzteBewegungAm TEXT,
    UNIQUE(ArtikelId, Lagerplatz, Lagereinheit),
    FOREIGN KEY (ArtikelId) REFERENCES Artikel (ArtikelId)
);

CREATE TABLE IF NOT EXISTS Einlagerungslogg (
    EinlagerungId    INTEGER PRIMARY KEY AUTOINCREMENT,
    Buchungsdatum    TEXT NOT NULL,
    LieferantId      INTEGER NOT NULL,
    ArtikelId        INTEGER NOT NULL,
    Lagerplatz       TEXT NOT NULL,
    Lagereinheit     TEXT NOT NULL,
    Menge            REAL NOT NULL,
    Palettenanzahl   INTEGER NOT NULL,
    Benutzer         TEXT,
    FOREIGN KEY (LieferantId) REFERENCES Lieferanten (LieferantId),
    FOREIGN KEY (ArtikelId) REFERENCES Artikel (ArtikelId)
);

CREATE TABLE IF NOT EXISTS Auslagerungslogg (
    AuslagerungId    INTEGER PRIMARY KEY AUTOINCREMENT,
    Buchungsdatum    TEXT NOT NULL,
    LieferantId      INTEGER NOT NULL,
    ArtikelId        INTEGER NOT NULL,
    Lagerplatz       TEXT NOT NULL,
    Lagereinheit     TEXT NOT NULL,
    Menge            REAL NOT NULL,
    Palettenanzahl   INTEGER NOT NULL,
    Benutzer         TEXT,
    FOREIGN KEY (LieferantId) REFERENCES Lieferanten (LieferantId),
    FOREIGN KEY (ArtikelId) REFERENCES Artikel (ArtikelId)
);

CREATE INDEX IF NOT EXISTS ix_bestand_artikel ON Bestand (ArtikelId);
CREATE INDEX IF NOT EXISTS ix_einlagerung_datum ON Einlagerungslogg (Buchungsdatum);
CREATE INDEX IF NOT EXISTS ix_auslagerung_datum ON Auslagerungslogg (Buchungsdatum);
