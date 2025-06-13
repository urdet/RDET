Imports System.IO
Imports MySql.Data.MySqlClient
Imports System.Drawing
Public Class servicesManagement
    Public Shared connectiono As MySqlConnection = Class1.conectiondmc
    Public Shared connectiona As MySqlConnection = Class1.conectionscr
    Private Sub Guna2DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellClick
        Dim i As Integer = Guna2DataGridView1.SelectedRows(0).Index
        If e.ColumnIndex = 2 And e.RowIndex = i Then
            If OpenFileDialog1.ShowDialog <> Windows.Forms.DialogResult.Cancel Then
                Try
                    Guna2DataGridView1.Rows(i).Cells(2).Value = Image.FromFile(OpenFileDialog1.FileName)
                    PictureBox1.Image = Guna2DataGridView1.Rows(i).Cells(2).Value

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
            End If
        End If
    End Sub
    Private Sub FillDat()
        If connectiono.State = ConnectionState.Closed Then
            connectiono.Open()
        End If
        If connectiona.State = ConnectionState.Closed Then
            connectiona.Open()
        End If
        Dim command As New MySqlCommand("SELECT `ID`, `services`.`serviceName`, `services`.`serviceImage`, `services`.`Type`, `services`.`Para1`, `services`.`Para2`, `services`.`Para3`, `services`.`Datex` FROM `damanecash`.`services`;", connectiono)

        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()

        adapter.Fill(table)
        If table.Rows.Count = 0 Then
            MsgBox("No Data")
        Else
            Guna2DataGridView1.Rows.Clear()
            Guna2DataGridView1.Columns.Clear()
            For i = 0 To (table.Columns.Count - 1)
                Dim txt As String = table.Columns(i).Caption
                If txt = "Para2" Or txt = "Para3" Then
                    Dim comboBoxColumn As New DataGridViewComboBoxColumn()
                    comboBoxColumn.Name = txt
                    comboBoxColumn.HeaderText = txt
                    Dim command1 As New MySqlCommand("SELECT * FROM hisabat;", connectiona)

                    Dim adapter1 As New MySqlDataAdapter(command1)
                    Dim table1 As New DataTable()

                    adapter1.Fill(table1)
                    If table1.Rows.Count = 0 Then

                    Else
                        comboBoxColumn.DataSource = table1
                    End If
                    comboBoxColumn.DisplayMember = "CompteName"
                    comboBoxColumn.ValueMember = "ID"
                    Guna2DataGridView1.Columns.Add(comboBoxColumn)
                ElseIf txt = "serviceImage" Then
                    Dim imageColumn As New DataGridViewImageColumn()
                    imageColumn.Name = txt
                    imageColumn.HeaderText = txt
                    Guna2DataGridView1.Columns.Add(imageColumn)
                Else
                    Dim textBoxColumn1 As New DataGridViewTextBoxColumn()
                    textBoxColumn1.Name = txt
                    textBoxColumn1.HeaderText = txt
                    Guna2DataGridView1.Columns.Add(textBoxColumn1)
                End If
            Next
            For i = 0 To (table.Rows.Count - 1)
                If IsDBNull(table.Rows(i)(5)) And IsDBNull(table.Rows(i)(5)) Then
                Else
                    Dim str1 As DataTable = utilities.GetSomeValuesscr("SELECT CompteName FROM hisabat ", CInt(table.Rows(i)(5)), "`ID`")
                    Dim str2 As DataTable = utilities.GetSomeValuesscr("SELECT CompteName FROM hisabat ", CInt(table.Rows(i)(6)), "`ID`")
                    CastImage.ReadImgIDPr(table.Rows(i)(0), "services", 2, PictureBox1)
                    Guna2DataGridView1.Rows.Add(table.Rows(i)(0), table.Rows(i)(1), PictureBox1.Image, table.Rows(i)(3), table.Rows(i)(4), str1.Rows(0)(0), str2.Rows(0)(0), table.Rows(i)(7))
                End If

            Next
        End If
        connectiona.Close()
        connectiono.Close()
        ''Fill combo Company
    End Sub
    Private Sub servicesManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FillDat()
    End Sub
    Public Function CheckID(ByVal ID As Integer)
        Dim query As String = "CheckIDServ" ' Name of your stored procedure


        Using command As New MySqlCommand(query, connectiono)
            command.CommandType = CommandType.StoredProcedure

            ' Input parameter
            command.Parameters.AddWithValue("checkID", ID)
            ' Output parameter
            command.Parameters.Add("@idExists", MySqlDbType.Bit)
            command.Parameters("@idExists").Direction = ParameterDirection.Output

            Try
                If connectiono.State = ConnectionState.Closed Then
                    connectiono.Open()
                End If
                command.ExecuteNonQuery()

                Dim exists As Boolean = Convert.ToBoolean(command.Parameters("@idExists").Value)
                If exists Then
                    Return True
                Else
                    Return False
                End If
                connectiono.Close()
            Catch ex As Exception
                If connectiono.State = ConnectionState.Open Then
                    connectiono.Close()
                End If
                Return False
                MessageBox.Show("Error: " & ex.Message)
            End Try
        End Using
    End Function
    Private Sub Guna2GradientButton5_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton5.Click
        If connectiono.State = ConnectionState.Closed Then
            connectiono.Open()
        End If
        ''Check id of item is existe
        For i = 0 To (Guna2DataGridView1.Rows.Count - 2)
            If (IsDBNull(Guna2DataGridView1.Rows(i).Cells(0).Value) = True) Or (IsNothing(Guna2DataGridView1.Rows(i).Cells(0).Value)) Then

                ''ADD statement
                Dim str1 As DataTable = utilities.GetSomeValuesscr("SELECT ID FROM hisabat ", "'" & Guna2DataGridView1.Rows(i).Cells(5).FormattedValue & "'", "`CompteName`")
                Dim str2 As DataTable = utilities.GetSomeValuesscr("SELECT ID FROM hisabat ", "'" & Guna2DataGridView1.Rows(i).Cells(6).FormattedValue & "'", "`CompteName`")

                Dim comman As New MySqlCommand("INSERT INTO Services (serviceName, serviceImage, Type, Para1, Para2, datex, Para3) VALUES (@serviceName, @serviceImage, @Type, @Para1, @Para2, @datex, @Para3);", connectiono)
                comman.Parameters.AddWithValue("@serviceName", Guna2DataGridView1.Rows(i).Cells(1).Value)
                comman.Parameters.AddWithValue("@Type", Guna2DataGridView1.Rows(i).Cells(3).Value)
                comman.Parameters.AddWithValue("@Para1", Guna2DataGridView1.Rows(i).Cells(4).Value)
                comman.Parameters.AddWithValue("@Para2", str1.Rows(0)(0))
                comman.Parameters.AddWithValue("@Para3", str2.Rows(0)(0))
                comman.Parameters.AddWithValue("@datex", DateAndTime.Now)
                ' PictureBox1.Image = Guna2DataGridView1.Rows(i).Cells(2).Value
                comman.Parameters.AddWithValue("@serviceImage", CastImage.ImageToByteArray(Guna2DataGridView1.Rows(i).Cells(2).Value))
                comman.ExecuteNonQuery()
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Add", "Add item", $"Add Service with name {Guna2DataGridView1.Rows(i).Cells(1).Value}.", "", DateTime.Now)

            Else
                ''Update Statement
                Dim ia As Boolean = CheckID(Guna2DataGridView1.Rows(i).Cells(0).Value)
                If ia = True Then
                    Dim str1 As DataTable = utilities.GetSomeValuesscr("SELECT ID FROM hisabat ", "'" & Guna2DataGridView1.Rows(i).Cells(5).FormattedValue & "'", "`CompteName`")
                    Dim str2 As DataTable = utilities.GetSomeValuesscr("SELECT ID FROM hisabat ", "'" & Guna2DataGridView1.Rows(i).Cells(6).FormattedValue & "'", "`CompteName`")

                    Dim comman As New MySqlCommand("UPDATE Services SET serviceName=@serviceName, serviceImage=@serviceImage, Type=@Type, Para1=@Para1, Para2=@Para2, Para3=@Para3, datex=@datex WHERE ID=@id", connectiono)
                    comman.Parameters.AddWithValue("@id", Guna2DataGridView1.Rows(i).Cells(0).Value)
                    comman.Parameters.AddWithValue("@serviceName", Guna2DataGridView1.Rows(i).Cells(1).Value.ToString())
                    comman.Parameters.AddWithValue("@Type", Guna2DataGridView1.Rows(i).Cells(3).Value.ToString())
                    comman.Parameters.AddWithValue("@Para1", Guna2DataGridView1.Rows(i).Cells(4).Value.ToString())
                    comman.Parameters.AddWithValue("@Para2", str1.Rows(0)(0))
                    comman.Parameters.AddWithValue("@Para3", str2.Rows(0)(0))
                    comman.Parameters.AddWithValue("@datex", Guna2DataGridView1.Rows(i).Cells(7).Value)
                    'PictureBox1.Image = Guna2DataGridView1.Rows(i).Cells(2).Value
                    comman.Parameters.AddWithValue("@serviceImage", CastImage.ImageToByteArray(Guna2DataGridView1.Rows(i).Cells(2).Value))


                    comman.ExecuteNonQuery()
                    TrackClass.AddUserTLine(Form1.idd, Form1.first, "Update", "Update item", $"Update Service of/with name {Guna2DataGridView1.Rows(i).Cells(1).Value.ToString()}.", "", DateTime.Now)
                Else
                    MsgBox($"Id is not exist (ID: {Guna2DataGridView1.Rows(i).Cells(0).Value})")
                End If


            End If
        Next
        FillDat()
        connectiono.Close()
    End Sub
    Function ImageFromBytes(ByVal bytes As Byte()) As Image
        Using ms As New MemoryStream(bytes)
            Return Image.FromStream(ms)
        End Using
    End Function
    Private Sub تحديثToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles تحديثToolStripMenuItem.Click
        FillDat()
    End Sub

    Private Sub Guna2DataGridView1_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles Guna2DataGridView1.DataError
        If Guna2DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value Is DBNull.Value Then
            Guna2DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = 0
        End If
    End Sub

    Private Sub Guna2GradientButton7_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton7.Click
        Dim ES As MsgBoxResult = MsgBox("Do you want to delete user no: " & Guna2DataGridView1.SelectedRows(0).Cells(0).Value, MsgBoxStyle.Critical + MsgBoxStyle.YesNo)
        connectiono.Open()

        Try
            If ES = MsgBoxResult.Yes Then
                Dim comman As New MySqlCommand("DELETE FROM `damanecash`.`services` WHERE `ID` = @ID ;", connectiono)
                comman.Parameters.AddWithValue("@ID", Guna2DataGridView1.SelectedRows(0).Cells(0).Value)

                comman.ExecuteNonQuery()
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Delete", "Delete item", $"Delete of name {Guna2DataGridView1.SelectedRows(0).Cells(1).Value} and id = {Guna2DataGridView1.SelectedRows(0).Cells(0).Value}.", "", DateTime.Now)
                MsgBox("Delete successfuly")
                FillDat()
            End If
        Catch ex As Exception
        End Try
        connectiono.Close()
    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Me.Close()
    End Sub

    Private Sub Label12_Click(sender As Object, e As EventArgs) Handles Label12.Click, Label1.Click

    End Sub

    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        fraisGest.Visible = False
        Try
            DGVFrais.Rows.Clear()
        Catch ex As Exception

        End Try

        fID.Clear()
        svID.Clear()
        svName.Clear()
        bet1.Clear()
        bet2.Clear()
        ftb.Clear()
        tySRel.Clear()
    End Sub

    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        fraisGest.Visible = True
        fraisGest.BringToFront()
        fID.Text = Guna2DataGridView1.SelectedRows(0).Cells(4).Value.ToString
        svID.Text = Guna2DataGridView1.SelectedRows(0).Cells(0).Value.ToString
        svName.Text = Guna2DataGridView1.SelectedRows(0).Cells(1).Value.ToString
        DGVFrais.DataSource = utilities.GetSomeValuesdmc("SELECT * FROM fraisargumenttable", Guna2DataGridView1.SelectedRows(0).Cells(0).Value, "ServiceID")
    End Sub

    Private Sub Savebtn_Click(sender As Object, e As EventArgs) Handles Savebtn.Click
        connectiono.Open()

        If fID.Text = "" Or fID.Text Is Nothing Then
            'Add
            Dim comman As New MySqlCommand("INSERT INTO `damanecash`.`fraisargumenttable` (`ServiceID`,`ServiceType`,`betwen`,`frais`) VALUES (@ServiceID, @ServiceType, @betwen, @frais) ;", connectiono)
            comman.Parameters.AddWithValue("@ServiceID", CInt(svID.Text))
            comman.Parameters.AddWithValue("@ServiceType", tySRel.Text)
            comman.Parameters.AddWithValue("@betwen", $"[{CDec(bet1.Text.Replace(",", ".")).ToString}][{CDec(bet2.Text.Replace(",", ".")).ToString}]")
            comman.Parameters.AddWithValue("@frais", CDec(ftb.Text.Replace(",", ".")))
            comman.ExecuteNonQuery()
            TrackClass.AddUserTLine(Form1.idd, Form1.first, "Add", "Add item", $"Add frais argument.", "", DateTime.Now)
            DGVFrais.DataSource = utilities.GetSomeValuesdmc("SELECT * FROM fraisargumenttable", Guna2DataGridView1.SelectedRows(0).Cells(0).Value, "ServiceID")
        Else

            If utilities.CheckIfIdExistsdmc(CInt(fID.Text), "fraisargumenttable") Then
                'Update
                Dim comman As New MySqlCommand("UPDATE `damanecash`.`fraisargumenttable` SET `ServiceID` = @ServiceID, `ServiceType` = @ServiceType, `betwen` = @betwen,`frais` = @frais WHERE `ID` = @ID;", connectiono)
                comman.Parameters.AddWithValue("@ServiceID", CInt(svID.Text))
                comman.Parameters.AddWithValue("@ServiceType", tySRel.Text)
                comman.Parameters.AddWithValue("@betwen", $"[{CDec(bet1.Text.Replace(",", ".")).ToString}][{CDec(bet2.Text.Replace(",", ".")).ToString}]")
                comman.Parameters.AddWithValue("@frais", CDec(ftb.Text.Replace(",", ".")))
                comman.Parameters.AddWithValue("@ID", CInt(fID.Text))
                comman.ExecuteNonQuery()
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Update", "Update item", $"Update Frais arguments.", "", DateTime.Now)
                DGVFrais.DataSource = utilities.GetSomeValuesdmc("SELECT * FROM fraisargumenttable", Guna2DataGridView1.SelectedRows(0).Cells(0).Value, "ServiceID")


            End If
        End If
        connectiono.Close()
    End Sub


    Private Sub Guna2GradientButton3_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton3.Click
        Dim ES As MsgBoxResult = MsgBox("Do you want to delete user no: " & CInt(fID.Text), MsgBoxStyle.Critical + MsgBoxStyle.YesNo)
        connectiono.Open()

        Try
            If ES = MsgBoxResult.Yes Then
                Dim comman As New MySqlCommand("DELETE FROM `damanecash`.`fraisargumenttable` WHERE `ID` = @ID ;", connectiono)
                comman.Parameters.AddWithValue("@ID", CInt(fID.Text))

                comman.ExecuteNonQuery()
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Delete", "Delete item", $"Delete Frais Argument", "", DateTime.Now)
                MsgBox("Delete successfuly")
                ' FillDat()
            End If
        Catch ex As Exception
        End Try
        connectiono.Close()
    End Sub

    Private Sub DGVFrais_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGVFrais.CellClick
        Try
            fID.Text = DGVFrais.SelectedRows(0).Cells(0).Value.ToString
            svID.Text = DGVFrais.SelectedRows(0).Cells(1).Value.ToString()
            Dim parts() As String = DGVFrais.SelectedRows(0).Cells(3).Value.ToString().Split("][")
            Dim str As String = parts(0)
            Dim str1 As String = str.Replace("[", "")
            Dim str2 As String = str1.Replace("]", "")
            bet1.Text = str2
            Dim sr As String = parts(1)
            Dim sr1 As String = sr.Replace("[", "")
            Dim sr2 As String = sr1.Replace("]", "")
            bet2.Text = sr2
            tySRel.Text = DGVFrais.SelectedRows(0).Cells(2).Value.ToString
            ftb.Text = DGVFrais.SelectedRows(0).Cells(4).Value.ToString
        Catch ex As Exception

        End Try
    End Sub
End Class