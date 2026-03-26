Namespace Models
    Public Class MovementRequest
        Public Property IsInbound As Boolean
        Public Property Buchungsdatum As Date
        Public Property LieferantId As Integer
        Public Property ArtikelId As Integer
        Public Property Stellplatz As String = String.Empty
        Public Property Lagereinheit As String = String.Empty
        Public Property Menge As Decimal
        Public Property Palettenanzahl As Integer
        Public Property Benutzer As String = String.Empty
    End Class
End Namespace
