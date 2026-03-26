Imports System.Windows.Forms
Imports Lagerverwaltung.Backend.Abstractions

Namespace codex_2
    Public Class Form1
        Inherits Form

        Private ReadOnly _service As IInventoryService
        Private ReadOnly _grid As DataGridView

        Public Sub New(service As IInventoryService)
            _service = service

            Text = "Lagerverwaltung - MainPage"
            Width = 1200
            Height = 700

            Dim panel As New FlowLayoutPanel With {
                .Dock = DockStyle.Top,
                .AutoSize = True,
                .WrapContents = True,
                .Padding = New Padding(10)
            }

            Dim btnInitDb As New Button With {.Text = "DB initialisieren", .Width = 150}
            AddHandler btnInitDb.Click, Sub(sender, e)
                                            _service.EnsureDatabase("database/schema.sql")
                                            MessageBox.Show("Datenbank initialisiert.")
                                        End Sub

            Dim btnStammdatenLieferant As New Button With {.Text = "Stammdaten Lieferant", .Width = 170}
            AddHandler btnStammdatenLieferant.Click, Sub(sender, e)
                                                         Using frm As New SupplierForm(_service)
                                                             frm.ShowDialog(Me)
                                                         End Using
                                                     End Sub

            Dim btnStammdatenArtikel As New Button With {.Text = "Stammdaten Artikel", .Width = 170}
            AddHandler btnStammdatenArtikel.Click, Sub(sender, e)
                                                       Using frm As New ArticleForm(_service)
                                                           frm.ShowDialog(Me)
                                                       End Using
                                                   End Sub

            Dim btnEinAuslagerung As New Button With {.Text = "Ein-/Auslagerung", .Width = 150}
            AddHandler btnEinAuslagerung.Click, Sub(sender, e)
                                                    Using frm As New MovementForm(_service)
                                                        frm.ShowDialog(Me)
                                                    End Using
                                                End Sub

            Dim btnBestandGesamt As New Button With {.Text = "Bestand gesamt", .Width = 150}
            AddHandler btnBestandGesamt.Click, Sub(sender, e)
                                                   _grid.DataSource = _service.GetTotalStock()
                                               End Sub

            Dim btnBestandUebersicht As New Button With {.Text = "Bestandsübersicht", .Width = 150}
            AddHandler btnBestandUebersicht.Click, Sub(sender, e)
                                                       _grid.DataSource = _service.GetCurrentStock()
                                                   End Sub

            Dim btnTagesliste As New Button With {.Text = "Tagesliste", .Width = 120}
            AddHandler btnTagesliste.Click, Sub(sender, e)
                                                _grid.DataSource = _service.GetDailyList(Date.Today)
                                            End Sub

            panel.Controls.Add(btnInitDb)
            panel.Controls.Add(btnStammdatenLieferant)
            panel.Controls.Add(btnStammdatenArtikel)
            panel.Controls.Add(btnEinAuslagerung)
            panel.Controls.Add(btnBestandGesamt)
            panel.Controls.Add(btnBestandUebersicht)
            panel.Controls.Add(btnTagesliste)

            _grid = New DataGridView With {
                .Dock = DockStyle.Fill,
                .ReadOnly = True,
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            }

            Controls.Add(_grid)
            Controls.Add(panel)
        End Sub
    End Class
End Namespace
