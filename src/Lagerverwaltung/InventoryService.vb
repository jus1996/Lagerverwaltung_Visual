Imports System.Data
Imports Lagerverwaltung.Backend.Abstractions
Imports Lagerverwaltung.Backend.Models

' Legacy-Fassade für Abwärtskompatibilität in der UI.
' Die eigentliche Logik liegt im Backend-Projekt (Lagerverwaltung.Backend).
Public Class InventoryService
    Private ReadOnly _inner As IInventoryService

    Public Sub New(inner As IInventoryService)
        _inner = inner
    End Sub

    Public Sub EnsureDatabase(schemaFilePath As String)
        _inner.EnsureDatabase(schemaFilePath)
    End Sub

    Public Sub AddSupplier(lieferantenNr As String, name As String)
        _inner.AddSupplier(lieferantenNr, name)
    End Sub

    Public Sub AddArticle(artikelNr As String, bezeichnung As String, lieferantId As Integer, lagereinheit As String)
        _inner.AddArticle(artikelNr, bezeichnung, lieferantId, lagereinheit)
    End Sub

    Public Function GetSuppliers() As DataTable
        Return _inner.GetSuppliers()
    End Function

    Public Function GetArticles() As DataTable
        Return _inner.GetArticles()
    End Function

    Public Sub BookMovement(request As MovementRequest)
        _inner.BookMovement(request)
    End Sub

    Public Function GetCurrentStock() As DataTable
        Return _inner.GetCurrentStock()
    End Function

    Public Function GetTotalStock() As DataTable
        Return _inner.GetTotalStock()
    End Function

    Public Function GetDailyList(datum As Date) As DataTable
        Return _inner.GetDailyList(datum)
    End Function
End Class
