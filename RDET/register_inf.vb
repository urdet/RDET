Imports System.Globalization
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraPrinting
Imports MySql.Data.MySqlClient
Public Class register_inf
    Public Shared connection As MySqlConnection = Class1.conectionreg
    Public Function CheckID(id As Integer)
        Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT * FROM `information_client`", id, "ID")
        If dt.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Sub fillDataIC()
        Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT `Id` as 'ر-ت', `NomEtPrenom` as 'الإسم والنسب', `CIN`, `NumDTele` as 'رقم الهاتف', `ONE` as 'Contrat ONE', `EAU` as 'Contrat EAU', `IDCS`, `CNSS` as 'Code CNSS', `Identité fiscale` as 'التعريف الضريبي' , `Observation` as 'ملاحظة' FROM `resgister_db`.`information_client`", 1, "1")
        GridControl1.DataSource = dt
    End Sub
    Private Function IsTextArabic(text As String) As Boolean
        For Each c As Char In text
            ' Check if the Unicode value of the character falls within the range of Arabic characters
            If CharUnicodeInfo.GetUnicodeCategory(c) = UnicodeCategory.OtherLetter AndAlso
           c >= ChrW(&H600) AndAlso c <= ChrW(&H6FF) Then
                Return True
            End If
        Next
        Return False
    End Function
    Private Sub register_inf_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fillDataIC()
        GridView1.Columns(0).Width = 20
        GridView1.Columns(1).Width = 120
    End Sub

    Private Sub BarButtonItem1_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem1.ItemClick
        Dim dataTable As DataTable = CType(GridControl1.DataSource, DataTable)
        Dim row As DataRow = dataTable.NewRow()

        dataTable.Rows.InsertAt(row, 0)
        'dataTable.Rows(dataTable.Rows.Count - 1)(0) = dataTable.Rows.Count
        GridControl1.RefreshDataSource()
        Dim gridView As DevExpress.XtraGrid.Views.Grid.GridView = CType(GridControl1.MainView, DevExpress.XtraGrid.Views.Grid.GridView)
        If gridView IsNot Nothing Then
            gridView.FocusedRowHandle = gridView.GetRowHandle(0) ' Focus the first row
        End If
    End Sub
    Private Function GetSelectedRowIndex()
        Dim view As DevExpress.XtraGrid.Views.Grid.GridView = GridControl1.MainView
        Dim selectedRow As Integer = view.GetSelectedRows()(0)
        Return (selectedRow)
    End Function
    Private Sub BarButtonItem3_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem3.ItemClick
        Dim rindex As Integer = GetSelectedRowIndex()
        If connection.State = ConnectionState.Open Then
        Else
            connection.Open()
        End If
        Dim dataTable1 As DataTable = CType(GridControl1.DataSource, DataTable)
        Dim es As MsgBoxResult = MsgBox($"Voulez-vous suprimer No: {dataTable1.Rows(rindex)(0)}", MsgBoxStyle.YesNo)
        If es = MsgBoxResult.Yes Then
            Dim comman As New MySqlCommand("DELETE FROM `information_client` WHERE `Id` = @ID ;", connection)
            comman.Parameters.AddWithValue("@ID", dataTable1.Rows(rindex)(0))
            comman.ExecuteNonQuery()
            MsgBox("Delete Successfuly")
            Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT `Id` as 'ر-ت', `NomEtPrenom` as 'الإسم والنسب', `CIN`, `NumDTele` as 'رقم الهاتف', `ONE` as 'Contrat ONE', `EAU` as 'Contrat EAU', `IDCS`, `CNSS` as 'Code CNSS', `Identité fiscale` as 'التعريف الضريبي' , `Observation` as 'ملاحظة' FROM `resgister_db`.`information_client`", 1, "1")
            GridControl1.DataSource = dt
        End If
        connection.Close()
    End Sub

    Private Sub BarButtonItem4_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem4.ItemClick
        If connection.State = ConnectionState.Open Then
        Else
            connection.Open()
        End If
        Dim dataTable1 As DataTable = CType(GridControl1.DataSource, DataTable)
        For i = 0 To dataTable1.Rows.Count - 1
            If IsDBNull(dataTable1.Rows(i)(0)) Or dataTable1.Rows(i)(0) Is Nothing Then
                'Insert Statement

                'Dim dtt As String =
                Dim command1 As New MySqlCommand($"INSERT INTO `information_client` (`NomEtPrenom`, `CIN`, `NumDTele`, `ONE`, `EAU`, `IDCS`, `CNSS`, `Observation`, `Identité fiscale`) VALUES " &
                                        $"(@Np, @CIN, @NTF, @One, @Eauu, @idcs, @cnss, @Obser, @Dateq);", connection)
                command1.Parameters.AddWithValue("@Np", dataTable1.Rows(i)(1))
                command1.Parameters.AddWithValue("@CIN", dataTable1.Rows(i)(2))
                command1.Parameters.AddWithValue("@NTF", dataTable1.Rows(i)(3))
                command1.Parameters.AddWithValue("@One", dataTable1.Rows(i)(4))
                command1.Parameters.AddWithValue("@Eauu", dataTable1.Rows(i)(5))
                command1.Parameters.AddWithValue("@idcs", dataTable1.Rows(i)(6))
                command1.Parameters.AddWithValue("@cnss", dataTable1.Rows(i)(7))
                command1.Parameters.AddWithValue("@Obser", dataTable1.Rows(i)(9))
                command1.Parameters.AddWithValue("@Dateq", dataTable1.Rows(i)(8))
                command1.ExecuteNonQuery()
            Else

                'Check ID
                Dim isExiste As Boolean = CheckID(dataTable1.Rows(i)(0))

                If isExiste = True Then
                    Dim command1 As New MySqlCommand($"UPDATE `resgister_db`.`information_client` " &
                            "Set " &
                            $"`NomEtPrenom` = '{dataTable1.Rows(i)(1)}', " &
                            $"`CIN` = '{dataTable1.Rows(i)(2) }', " &
                            $"`NumDTele` = '{dataTable1.Rows(i)(3) }', " &
                            $"`ONE` = '{dataTable1.Rows(i)(4)}', " &
                            $"`EAU` = '{dataTable1.Rows(i)(5)}', " &
                            $"`IDCS` = '{dataTable1.Rows(i)(6)}', " &
                            $"`CNSS` = '{dataTable1.Rows(i)(7)}', " &
                            $"`Identité fiscale` = '{dataTable1.Rows(i)(8)}', " &
                            $"`Observation` = '{dataTable1.Rows(i)(9)}' " &
                            $"WHERE `Id` = {dataTable1.Rows(i)(0)}; ", connection)
                    command1.ExecuteNonQuery()
                End If
            End If
        Next
        Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT `Id` as 'ر-ت', `NomEtPrenom` as 'الإسم والنسب', `CIN`, `NumDTele` as 'رقم الهاتف', `ONE` as 'Contrat ONE', `EAU` as 'Contrat EAU', `IDCS`, `CNSS` as 'Code CNSS', `Identité fiscale` as 'التعريف الضريبي' , `Observation` as 'ملاحظة' FROM `resgister_db`.`information_client`", 1, "1")
        GridControl1.DataSource = dt
        connection.Close()
    End Sub

    Private Sub BarButtonItem5_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem5.ItemClick
        Dim view As DevExpress.XtraGrid.Views.Grid.GridView = GridControl1.MainView
        If view.OptionsView.ShowAutoFilterRow = True Then
            view.OptionsView.ShowAutoFilterRow = False
        Else
            view.OptionsView.ShowAutoFilterRow = True
        End If
    End Sub

    Private Sub BarButtonItem6_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem6.ItemClick
        GridControl1.ShowRibbonPrintPreview()
    End Sub

    Private Sub BarButtonItem8_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem8.ItemClick
        Dim printableComponentLink As New PrintableComponentLink(New PrintingSystem())

        ' Set the grid control as a component to be printed
        printableComponentLink.Component = GridControl1

        ' Set the paper size to A4
        printableComponentLink.PaperKind = System.Drawing.Printing.PaperKind.A4

        ' Set margins (0.5cm)
        Dim leftMargin As Single = Convert.ToSingle(0.5 * 100 / 2.54) ' Convert cm to inches
        Dim rightMargin As Single = leftMargin
        Dim topMargin As Single = leftMargin
        Dim bottomMargin As Single = leftMargin

        printableComponentLink.Margins.Left = CInt(leftMargin)
        printableComponentLink.Margins.Right = CInt(rightMargin)
        printableComponentLink.Margins.Top = CInt(topMargin)
        printableComponentLink.Margins.Bottom = CInt(bottomMargin)

        ' Generate PDF document
        printableComponentLink.CreateDocument()

        ' Save the document to a file
        Dim saveFileDialog As New SaveFileDialog()
        saveFileDialog.Filter = "PDF Files|*.pdf"
        If saveFileDialog.ShowDialog() = DialogResult.OK Then
            printableComponentLink.ExportToPdf(saveFileDialog.FileName)
        End If
    End Sub

    Private Sub BarButtonItem7_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem7.ItemClick
        Dim saveFileDialog As New SaveFileDialog()
        saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx"
        saveFileDialog.FilterIndex = 2
        saveFileDialog.RestoreDirectory = True

        If saveFileDialog.ShowDialog() = DialogResult.OK Then
            Dim options As New XlsxExportOptions()
            options.TextExportMode = TextExportMode.Text
            options.ExportMode = XlsxExportMode.SingleFile
            options.SheetName = "Sheet1"  ' You can set the sheet name here

            GridControl1.ExportToXlsx(saveFileDialog.FileName, options)
        End If
    End Sub

    Private Sub GridView1_RowCellClick(sender As Object, e As RowCellClickEventArgs) Handles GridView1.RowCellClick

    End Sub

    Private Sub BarButtonItem9_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem9.ItemClick

    End Sub

    Private Sub GridView1_CustomDrawCell(sender As Object, e As DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs) Handles GridView1.CustomDrawCell
        'If e.Column.AbsoluteIndex = 1 Then
        '    For i = 0 To GridView1.RowCount - 1
        '        If IsTextArabic(GridView1.GetRowCellValue(i, "Nom et Prénom").ToString) Then
        '            MsgBox(GridView1.GetRowCellValue(i, "Nom et Prénom").ToString)
        '            e.Appearance.TextOptions.RightToLeft = DevExpress.Utils.DefaultBoolean.True
        '        End If
        '    Next
        'End If
    End Sub

    Private Sub RibbonControl_Click(sender As Object, e As EventArgs) Handles RibbonControl.Click

    End Sub

    Private Sub BarButtonItem2_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem2.ItemClick

    End Sub
End Class