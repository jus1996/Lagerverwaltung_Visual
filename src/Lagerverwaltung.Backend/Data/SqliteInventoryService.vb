Imports System.Data
Imports Microsoft.Data.Sqlite
Imports Lagerverwaltung.Backend.Abstractions
Imports Lagerverwaltung.Backend.Models

Namespace Data
    Public Class SqliteInventoryService
        Implements IInventoryService

        Private ReadOnly _connectionString As String

        Public Sub New(connectionString As String)
            _connectionString = connectionString
        End Sub

        Public Sub EnsureDatabase(schemaFilePath As String) Implements IInventoryService.EnsureDatabase
            Dim schema = IO.File.ReadAllText(schemaFilePath)

            Using connection As New SqliteConnection(_connectionString)
                connection.Open()
                Using command = connection.CreateCommand()
                    command.CommandText = schema
                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub AddSupplier(lieferantenNr As String, name As String) Implements IInventoryService.AddSupplier
            Const sql As String = "INSERT INTO Lieferanten (LieferantenNr, Name) VALUES ($nr, $name)"
            ExecuteNonQuery(sql,
                            New KeyValuePair(Of String, Object)("$nr", lieferantenNr),
                            New KeyValuePair(Of String, Object)("$name", name))
        End Sub

        Public Sub AddArticle(artikelNr As String, bezeichnung As String, lieferantId As Integer, lagereinheit As String) Implements IInventoryService.AddArticle
            Const sql As String = "INSERT INTO Artikel (ArtikelNr, Bezeichnung, Einheit, LieferantId) VALUES ($nr, $name, $einheit, $lieferantId)"
            ExecuteNonQuery(sql,
                            New KeyValuePair(Of String, Object)("$nr", artikelNr),
                            New KeyValuePair(Of String, Object)("$name", bezeichnung),
                            New KeyValuePair(Of String, Object)("$einheit", lagereinheit),
                            New KeyValuePair(Of String, Object)("$lieferantId", lieferantId))
        End Sub

        Public Function GetSuppliers() As DataTable Implements IInventoryService.GetSuppliers
            Const sql As String = "SELECT LieferantId, LieferantenNr || ' - ' || Name AS Anzeige FROM Lieferanten WHERE Aktiv = 1 ORDER BY Name"
            Return QueryAsDataTable(sql)
        End Function

        Public Function GetArticles() As DataTable Implements IInventoryService.GetArticles
            Const sql As String = "SELECT ArtikelId, ArtikelNr || ' - ' || Bezeichnung AS Anzeige FROM Artikel WHERE Aktiv = 1 ORDER BY ArtikelNr"
            Return QueryAsDataTable(sql)
        End Function

        Public Sub BookMovement(request As MovementRequest) Implements IInventoryService.BookMovement
            Using connection As New SqliteConnection(_connectionString)
                connection.Open()
                Using tx = connection.BeginTransaction()
                    If request.IsInbound Then
                        ExecuteInbound(connection, tx, request)
                    Else
                        ExecuteOutbound(connection, tx, request)
                    End If
                    tx.Commit()
                End Using
            End Using
        End Sub

        Private Sub ExecuteInbound(connection As SqliteConnection, tx As SqliteTransaction, request As MovementRequest)
            Const logSql As String = "INSERT INTO Einlagerungslogg (Buchungsdatum, LieferantId, ArtikelId, Lagerplatz, Lagereinheit, Menge, Palettenanzahl, Benutzer) VALUES ($datum, $lieferantId, $artikelId, $lagerplatz, $lagereinheit, $menge, $paletten, $benutzer)"
            ExecuteNonQuery(connection, tx, logSql,
                            New KeyValuePair(Of String, Object)("$datum", request.Buchungsdatum.ToString("yyyy-MM-dd")),
                            New KeyValuePair(Of String, Object)("$lieferantId", request.LieferantId),
                            New KeyValuePair(Of String, Object)("$artikelId", request.ArtikelId),
                            New KeyValuePair(Of String, Object)("$lagerplatz", request.Stellplatz),
                            New KeyValuePair(Of String, Object)("$lagereinheit", request.Lagereinheit),
                            New KeyValuePair(Of String, Object)("$menge", request.Menge),
                            New KeyValuePair(Of String, Object)("$paletten", request.Palettenanzahl),
                            New KeyValuePair(Of String, Object)("$benutzer", request.Benutzer))

            Const stockSql As String = "INSERT INTO Bestand (ArtikelId, Lagerplatz, Lagereinheit, Menge, Palettenanzahl, LetzteBewegungAm) VALUES ($artikelId, $lagerplatz, $lagereinheit, $menge, $paletten, datetime('now')) ON CONFLICT(ArtikelId, Lagerplatz, Lagereinheit) DO UPDATE SET Menge = Menge + excluded.Menge, Palettenanzahl = Palettenanzahl + excluded.Palettenanzahl, LetzteBewegungAm = datetime('now')"
            ExecuteNonQuery(connection, tx, stockSql,
                            New KeyValuePair(Of String, Object)("$artikelId", request.ArtikelId),
                            New KeyValuePair(Of String, Object)("$lagerplatz", request.Stellplatz),
                            New KeyValuePair(Of String, Object)("$lagereinheit", request.Lagereinheit),
                            New KeyValuePair(Of String, Object)("$menge", request.Menge),
                            New KeyValuePair(Of String, Object)("$paletten", request.Palettenanzahl))
        End Sub

        Private Sub ExecuteOutbound(connection As SqliteConnection, tx As SqliteTransaction, request As MovementRequest)
            Const stockSql As String = "UPDATE Bestand SET Menge = Menge - $menge, Palettenanzahl = Palettenanzahl - $paletten, LetzteBewegungAm = datetime('now') WHERE ArtikelId = $artikelId AND Lagerplatz = $lagerplatz AND Lagereinheit = $lagereinheit AND Menge >= $menge AND Palettenanzahl >= $paletten"
            Dim affected = ExecuteNonQuery(connection, tx, stockSql,
                                           New KeyValuePair(Of String, Object)("$menge", request.Menge),
                                           New KeyValuePair(Of String, Object)("$paletten", request.Palettenanzahl),
                                           New KeyValuePair(Of String, Object)("$artikelId", request.ArtikelId),
                                           New KeyValuePair(Of String, Object)("$lagerplatz", request.Stellplatz),
                                           New KeyValuePair(Of String, Object)("$lagereinheit", request.Lagereinheit))

            If affected = 0 Then
                Throw New InvalidOperationException("Nicht genügend Bestand oder Paletten für die Auslagerung vorhanden.")
            End If

            Const logSql As String = "INSERT INTO Auslagerungslogg (Buchungsdatum, LieferantId, ArtikelId, Lagerplatz, Lagereinheit, Menge, Palettenanzahl, Benutzer) VALUES ($datum, $lieferantId, $artikelId, $lagerplatz, $lagereinheit, $menge, $paletten, $benutzer)"
            ExecuteNonQuery(connection, tx, logSql,
                            New KeyValuePair(Of String, Object)("$datum", request.Buchungsdatum.ToString("yyyy-MM-dd")),
                            New KeyValuePair(Of String, Object)("$lieferantId", request.LieferantId),
                            New KeyValuePair(Of String, Object)("$artikelId", request.ArtikelId),
                            New KeyValuePair(Of String, Object)("$lagerplatz", request.Stellplatz),
                            New KeyValuePair(Of String, Object)("$lagereinheit", request.Lagereinheit),
                            New KeyValuePair(Of String, Object)("$menge", request.Menge),
                            New KeyValuePair(Of String, Object)("$paletten", request.Palettenanzahl),
                            New KeyValuePair(Of String, Object)("$benutzer", request.Benutzer))
        End Sub

        Public Function GetCurrentStock() As DataTable Implements IInventoryService.GetCurrentStock
            Const sql As String = "SELECT a.ArtikelNr, a.Bezeichnung, b.Lagerplatz, b.Lagereinheit, b.Menge, b.Palettenanzahl, b.LetzteBewegungAm FROM Bestand b INNER JOIN Artikel a ON a.ArtikelId = b.ArtikelId ORDER BY a.ArtikelNr, b.Lagerplatz"
            Return QueryAsDataTable(sql)
        End Function

        Public Function GetTotalStock() As DataTable Implements IInventoryService.GetTotalStock
            Const sql As String = "SELECT a.ArtikelNr, a.Bezeichnung, SUM(b.Menge) AS Gesamtmenge, SUM(b.Palettenanzahl) AS Gesamtpaletten FROM Bestand b INNER JOIN Artikel a ON a.ArtikelId = b.ArtikelId GROUP BY a.ArtikelNr, a.Bezeichnung ORDER BY a.ArtikelNr"
            Return QueryAsDataTable(sql)
        End Function

        Public Function GetDailyList(datum As Date) As DataTable Implements IInventoryService.GetDailyList
            Const sql As String = "SELECT 'EIN' AS Typ, e.Buchungsdatum, l.Name AS Lieferant, a.ArtikelNr, a.Bezeichnung, e.Lagerplatz, e.Lagereinheit, e.Menge, e.Palettenanzahl FROM Einlagerungslogg e INNER JOIN Artikel a ON a.ArtikelId = e.ArtikelId INNER JOIN Lieferanten l ON l.LieferantId = e.LieferantId WHERE date(e.Buchungsdatum) = date($datum) UNION ALL SELECT 'AUS' AS Typ, x.Buchungsdatum, l.Name AS Lieferant, a.ArtikelNr, a.Bezeichnung, x.Lagerplatz, x.Lagereinheit, x.Menge, x.Palettenanzahl FROM Auslagerungslogg x INNER JOIN Artikel a ON a.ArtikelId = x.ArtikelId INNER JOIN Lieferanten l ON l.LieferantId = x.LieferantId WHERE date(x.Buchungsdatum) = date($datum) ORDER BY Buchungsdatum"
            Return QueryAsDataTable(sql, New KeyValuePair(Of String, Object)("$datum", datum.ToString("yyyy-MM-dd")))
        End Function

        Private Sub ExecuteNonQuery(sql As String, ParamArray parameters() As KeyValuePair(Of String, Object))
            Using connection As New SqliteConnection(_connectionString)
                connection.Open()
                ExecuteNonQuery(connection, Nothing, sql, parameters)
            End Using
        End Sub

        Private Function ExecuteNonQuery(connection As SqliteConnection,
                                         tx As SqliteTransaction,
                                         sql As String,
                                         ParamArray parameters() As KeyValuePair(Of String, Object)) As Integer
            Using command = connection.CreateCommand()
                command.CommandText = sql
                If tx IsNot Nothing Then
                    command.Transaction = tx
                End If

                For Each p In parameters
                    command.Parameters.AddWithValue(p.Key, p.Value)
                Next

                Return command.ExecuteNonQuery()
            End Using
        End Function

        Private Function QueryAsDataTable(sql As String, ParamArray parameters() As KeyValuePair(Of String, Object)) As DataTable
            Dim table As New DataTable()

            Using connection As New SqliteConnection(_connectionString)
                connection.Open()
                Using command = connection.CreateCommand()
                    command.CommandText = sql
                    For Each p In parameters
                        command.Parameters.AddWithValue(p.Key, p.Value)
                    Next

                    Using reader = command.ExecuteReader()
                        table.Load(reader)
                    End Using
                End Using
            End Using

            Return table
        End Function
    End Class
End Namespace
