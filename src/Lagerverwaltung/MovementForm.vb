Imports System.Data
Imports System.Windows.Forms
Imports Lagerverwaltung.Backend.Abstractions
Imports Lagerverwaltung.Backend.Models

Namespace codex_2
    Public Class MovementForm
        Inherits Form

        Private ReadOnly _service As IInventoryService
        Private ReadOnly _dtDatum As DateTimePicker
        Private ReadOnly _cbLieferant As ComboBox
        Private ReadOnly _cbArtikel As ComboBox
        Private ReadOnly _txtStellplatz As TextBox
        Private ReadOnly _txtLagereinheit As TextBox
        Private ReadOnly _numMenge As NumericUpDown
        Private ReadOnly _numPaletten As NumericUpDown
        Private ReadOnly _cbTyp As ComboBox

        Public Sub New(service As IInventoryService)
            _service = service

            Text = "Ein- und Auslagerung buchen"
            Width = 650
            Height = 420

            Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 10, .Padding = New Padding(12)}
            layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35))
            layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65))

            layout.Controls.Add(New Label With {.Text = "Buchungstyp"}, 0, 0)
            _cbTyp = New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList}
            _cbTyp.Items.Add("Einlagerung")
            _cbTyp.Items.Add("Auslagerung")
            _cbTyp.SelectedIndex = 0
            layout.Controls.Add(_cbTyp, 1, 0)

            layout.Controls.Add(New Label With {.Text = "Datum"}, 0, 1)
            _dtDatum = New DateTimePicker With {.Format = DateTimePickerFormat.Short}
            layout.Controls.Add(_dtDatum, 1, 1)

            layout.Controls.Add(New Label With {.Text = "Lieferant"}, 0, 2)
            _cbLieferant = New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList}
            layout.Controls.Add(_cbLieferant, 1, 2)

            layout.Controls.Add(New Label With {.Text = "Artikelnummer"}, 0, 3)
            _cbArtikel = New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList}
            layout.Controls.Add(_cbArtikel, 1, 3)

            layout.Controls.Add(New Label With {.Text = "Stellplatz"}, 0, 4)
            _txtStellplatz = New TextBox()
            layout.Controls.Add(_txtStellplatz, 1, 4)

            layout.Controls.Add(New Label With {.Text = "Lagereinheit"}, 0, 5)
            _txtLagereinheit = New TextBox()
            layout.Controls.Add(_txtLagereinheit, 1, 5)

            layout.Controls.Add(New Label With {.Text = "Menge"}, 0, 6)
            _numMenge = New NumericUpDown With {.DecimalPlaces = 2, .Maximum = 1000000D, .Value = 1D}
            layout.Controls.Add(_numMenge, 1, 6)

            layout.Controls.Add(New Label With {.Text = "Palettenanzahl"}, 0, 7)
            _numPaletten = New NumericUpDown With {.Maximum = 1000000D, .Value = 1D}
            layout.Controls.Add(_numPaletten, 1, 7)

            Dim btnBook As New Button With {.Text = "Buchen", .Dock = DockStyle.Right}
            AddHandler btnBook.Click, AddressOf Book_Click
            layout.Controls.Add(btnBook, 1, 8)

            Controls.Add(layout)
            LoadMasterData()
        End Sub

        Private Sub LoadMasterData()
            Dim suppliers As DataTable = _service.GetSuppliers()
            _cbLieferant.DataSource = suppliers
            _cbLieferant.DisplayMember = "Anzeige"
            _cbLieferant.ValueMember = "LieferantId"

            Dim articles As DataTable = _service.GetArticles()
            _cbArtikel.DataSource = articles
            _cbArtikel.DisplayMember = "Anzeige"
            _cbArtikel.ValueMember = "ArtikelId"
        End Sub

        Private Sub Book_Click(sender As Object, e As EventArgs)
            Dim request As New MovementRequest With {
                .IsInbound = _cbTyp.SelectedItem.ToString() = "Einlagerung",
                .Buchungsdatum = _dtDatum.Value.Date,
                .LieferantId = CInt(_cbLieferant.SelectedValue),
                .ArtikelId = CInt(_cbArtikel.SelectedValue),
                .Stellplatz = _txtStellplatz.Text.Trim(),
                .Lagereinheit = _txtLagereinheit.Text.Trim(),
                .Menge = _numMenge.Value,
                .Palettenanzahl = CInt(_numPaletten.Value),
                .Benutzer = Environment.UserName
            }

            _service.BookMovement(request)
            MessageBox.Show("Buchung erfolgreich gespeichert.")
            Close()
        End Sub
    End Class
End Namespace
