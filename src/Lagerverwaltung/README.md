# Lagerverwaltung – Professioneller Aufbau (Frontend + Backend)

Diese Lösung ist jetzt in **2 Schichten** aufgeteilt:

1. **Frontend (WinForms)**
   - Projekt: `src/Lagerverwaltung/Lagerverwaltung.vbproj`
   - Enthält die Benutzeroberfläche (`Form1`, `SupplierForm`, `ArticleForm`, `MovementForm`)
2. **Backend (Business + Datenzugriff)**
   - Projekt: `src/Lagerverwaltung.Backend/Lagerverwaltung.Backend.vbproj`
   - Enthält Schnittstelle (`IInventoryService`), DTO (`MovementRequest`) und SQLite-Implementierung (`SqliteInventoryService`)

## Solution öffnen

Öffne in Visual Studio 2022 die Datei:

- `Lagerverwaltung_Visual.sln`

Damit werden **Frontend + Backend** zusammen geladen.

## Architektur

- `Lagerverwaltung` (Frontend) referenziert `Lagerverwaltung.Backend`.
- `Program.vb` erstellt den Backend-Service und injiziert ihn in die Main-Form.
- Die Forms sprechen nur gegen `IInventoryService` (saubere Trennung).
- SQL-Schema liegt in `database/schema.sql` und wird ins Ausgabeverzeichnis kopiert.

## Debug / Start

1. In Visual Studio: `Lagerverwaltung` als **Startprojekt** setzen.
2. Konfiguration `Debug | Any CPU` oder `Debug | x64` wählen.
3. Build → **Projektmappe neu erstellen**.
4. Starten (F5).
5. In der App zuerst **DB initialisieren** klicken.

## Enthaltene Funktionen

- Stammdaten: Lieferanten + Artikel
- Buchung: Einlagerung / Auslagerung
- Felder: Datum, Lieferant, Artikelnummer, Stellplatz, Lagereinheit, Menge, Palettenanzahl
- Auswertungen: Bestand gesamt, Bestandsübersicht, Tagesliste
