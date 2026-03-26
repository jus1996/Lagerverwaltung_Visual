Imports System.IO
Imports System.Windows.Forms
Imports Lagerverwaltung.Backend.Abstractions
Imports Lagerverwaltung.Backend.Data

Public Module Program
    <STAThread>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim dbPath = Path.Combine(Application.StartupPath, "lagerverwaltung.db")
        Dim service As IInventoryService = New SqliteInventoryService("Data Source=" & dbPath)

        Application.Run(New codex_2.Form1(service))
    End Sub
End Module
