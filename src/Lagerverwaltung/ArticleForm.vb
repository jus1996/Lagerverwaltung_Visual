Imports System.Data
Imports System.Windows.Forms
Imports Lagerverwaltung.Backend.Abstractions

Namespace codex_2
    Public Class ArticleForm
        Inherits Form

        Private ReadOnly _service As IInventoryService
        Private ReadOnly _txtArtikelNr As TextBox
        Private ReadOnly _txtBezeichnung As TextBox
        Private ReadOnly _txtLagereinheit As TextBox
        Private ReadOnly _cbLieferant As ComboBox

        Public Sub New(service As IInventoryService)
            _service = service

            Text = "Artikel anlegen"
            Width = 520
            Height = 280

            Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 5, .Padding = New Padding(12)}
            layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35))
            layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65))

            layout.Controls.Add(New Label With {.Text = "Lieferant"}, 0, 0)
            _cbLieferant = New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList}
            layout.Controls.Add(_cbLieferant, 1, 0)

            layout.Controls.Add(New Label With {.Text = "Artikelnummer"}, 0, 1)
            _txtArtikelNr = New TextBox()
            layout.Controls.Add(_txtArtikelNr, 1, 1)

            layout.Controls.Add(New Label With {.Text = "Bezeichnung"}, 0, 2)
            _txtBezeichnung = New TextBox()
            layout.Controls.Add(_txtBezeichnung, 1, 2)

            layout.Controls.Add(New Label With {.Text = "Lagereinheit"}, 0, 3)
            _txtLagereinheit = New TextBox()
            layout.Controls.Add(_txtLagereinheit, 1, 3)

            Dim btnSave As New Button With {.Text = "Speichern", .Dock = DockStyle.Right}
            AddHandler btnSave.Click, AddressOf Save_Click
            layout.Controls.Add(btnSave, 1, 4)

            Controls.Add(layout)
            LoadSuppliers()
        End Sub

        Private Sub LoadSuppliers()
            Dim suppliers As DataTable = _service.GetSuppliers()
            _cbLieferant.DataSource = suppliers
            _cbLieferant.DisplayMember = "Anzeige"
            _cbLieferant.ValueMember = "LieferantId"
        End Sub

        Private Sub Save_Click(sender As Object, e As EventArgs)
            Dim lieferantId As Integer = CInt(_cbLieferant.SelectedValue)
            _service.AddArticle(_txtArtikelNr.Text.Trim(), _txtBezeichnung.Text.Trim(), lieferantId, _txtLagereinheit.Text.Trim())
            MessageBox.Show("Artikel gespeichert.")
            Close()
        End Sub
    End Class
End Namespace
