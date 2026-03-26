Imports System.Windows.Forms
Imports Lagerverwaltung.Backend.Abstractions

Namespace codex_2
    Public Class SupplierForm
        Inherits Form

        Private ReadOnly _service As IInventoryService
        Private ReadOnly _txtNr As TextBox
        Private ReadOnly _txtName As TextBox

        Public Sub New(service As IInventoryService)
            _service = service

            Text = "Lieferant anlegen"
            Width = 450
            Height = 220

            Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 3, .Padding = New Padding(12)}
            layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35))
            layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65))

            layout.Controls.Add(New Label With {.Text = "Lieferanten-Nr"}, 0, 0)
            _txtNr = New TextBox()
            layout.Controls.Add(_txtNr, 1, 0)

            layout.Controls.Add(New Label With {.Text = "Name"}, 0, 1)
            _txtName = New TextBox()
            layout.Controls.Add(_txtName, 1, 1)

            Dim btnSave As New Button With {.Text = "Speichern", .Dock = DockStyle.Right}
            AddHandler btnSave.Click, AddressOf Save_Click
            layout.Controls.Add(btnSave, 1, 2)

            Controls.Add(layout)
        End Sub

        Private Sub Save_Click(sender As Object, e As EventArgs)
            _service.AddSupplier(_txtNr.Text.Trim(), _txtName.Text.Trim())
            MessageBox.Show("Lieferant gespeichert.")
            Close()
        End Sub
    End Class
End Namespace
