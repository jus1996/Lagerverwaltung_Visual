Imports System.Data
Imports Lagerverwaltung.Backend.Models

Namespace Abstractions
    Public Interface IInventoryService
        Sub EnsureDatabase(schemaFilePath As String)
        Sub AddSupplier(lieferantenNr As String, name As String)
        Sub AddArticle(artikelNr As String, bezeichnung As String, lieferantId As Integer, lagereinheit As String)
        Function GetSuppliers() As DataTable
        Function GetArticles() As DataTable
        Sub BookMovement(request As MovementRequest)
        Function GetCurrentStock() As DataTable
        Function GetTotalStock() As DataTable
        Function GetDailyList(datum As Date) As DataTable
    End Interface
End Namespace
