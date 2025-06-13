Imports MySql.Data.MySqlClient
Public Class Register
    Public Shared connection As MySqlConnection = Class1.conectionreg
    Dim tbIDa As Integer

    Public Sub fillDataIC()
        Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT `Id` as 'No', `NomEtPrenom` as 'Nom et Prénom', `CIN`, `ONE` as 'Contrat ONE', `EAU` as 'Contrat EAU', `IDCS`, `CNSS` as 'Code CNSS', `Observation`, `datex` as 'Date' FROM `resgister_db`.`information_client`", 1, "1")
        GridControl1.DataSource = dt
    End Sub
    Public Function getMaxRowsToAdd(tid As Integer) As Integer
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        Else
        End If
        'Dim command2 As New MySqlCommand($"SELECT cID, COUNT(cID) AS occurrences " &
        '                  "FROM `values` " &
        '                  "GROUP BY cID " &
        '                  "ORDER BY occurrences DESC " &
        '                  "LIMIT 1;", connection)
        'Dim adapter2 As New MySqlDataAdapter(command2)
        'Dim table2 As New DataTable()
        'If table2.Rows.Count > 0 Then
        '    Return table2.Rows(0)(1)
        'Else
        '    MsgBox("Err")
        '    Return 0
        'End If
        Dim query As String = $"SELECT cID, COUNT(cID) AS occurrences " &
                          $"FROM `values` WHERE tID = {tid} " &
                          "GROUP BY cID " &
                          "ORDER BY occurrences DESC " &
                          "LIMIT 1;"
        Using command As New MySqlCommand(query, connection)
            ' Execute the query and retrieve the result
            Using reader As MySqlDataReader = command.ExecuteReader()
                ' Check if there are any rows returned
                If reader.Read() Then
                    ' Retrieve the value of the occurrences column from the first row
                    Dim occurrences As Integer = Convert.ToInt32(reader("occurrences"))

                    ' Now you can use the 'occurrences' variable as needed
                    'MsgBox("Max occurrences: " & occurrences)
                    Return occurrences
                Else
                    ' Handle the case where no rows are returned
                    MsgBox("No data found.")
                    Return 0
                End If
            End Using
        End Using

    End Function
    Public Function getdatacolumn(cid As Integer, rowi As Integer)
        Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT * FROM `values` ", cid, $"`cID`")
        Try
            If dt.Rows.Count > 0 Then
                Return dt.Rows(rowi)(4)
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try

    End Function
    Public Function checkId(table As String, id As Integer) As Boolean
        Dim command2 As New MySqlCommand($"SELECT * FROM {table} WHERE ID = {id};", connection)
        Dim adapter2 As New MySqlDataAdapter(command2)
        Dim table2 As New DataTable()

        adapter2.Fill(table2)
        If table2.Rows.Count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function
    Private Sub showtables()
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If

        Try

            Dim command2 As New MySqlCommand("SELECT * FROM tables;", connection)
            Dim adapter2 As New MySqlDataAdapter(command2)
            Dim table2 As New DataTable()

            adapter2.Fill(table2)
            If table2.Rows.Count = 0 Then
                'MsgBox("Error")
            Else

                FlowLayoutPanel1.Controls.Clear()

                For ligne As Integer = 0 To table2.Rows.Count - 1
                    Dim nomBouton As String = table2.Rows(ligne)(2).ToString
                    Dim DescBouton As String = table2.Rows(ligne)(3).ToString
                    Dim id As Integer = table2.Rows(ligne)(0)
                    Dim bouton As New Guna.UI2.WinForms.Guna2GradientButton
                    bouton.Name = "b" & table2.Rows(ligne)(0).ToString
                    bouton.Text = nomBouton
                    bouton.Font = New Font("Segoe UI", 12)
                    bouton.BorderRadius = 5
                    bouton.FillColor = Color.Orange
                    bouton.FillColor = Color.DarkOrange
                    AddHandler bouton.Click, Sub(s, args) boutonClique(id)
                    FlowLayoutPanel1.Controls.Add(bouton)
                Next
            End If
            connection.Close()
        Catch ex As Exception
        Finally
            connection.Close()
        End Try
    End Sub
    Private Sub boutonClique(idt As Integer)
        TableSelection.Visible = False
        Guna2ShadowPanel2.Visible = True
        Dim isCol As Boolean
        '' Get Columns
        Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT * FROM columns", idt, $"tID")
        Try
            tbDGV.Rows.Clear()
            tbDGV.Columns.Clear()
        Catch ex As Exception

        End Try
        If dt.Rows.Count > 0 Then
            tbIDa = idt
            '' Create columns
            '  MsgBox("column Add")
            For i = 0 To dt.Rows.Count - 1
                tbDGV.Columns.Add(dt.Rows(i)(3), dt.Rows(i)(4))
            Next


            Dim maxRowsToAdd As Integer = getMaxRowsToAdd(idt) - 1
            '  MsgBox("Rows Add")
            For i = 0 To maxRowsToAdd
                tbDGV.Rows.Add()
            Next
            ' MsgBox("column")

            For t = 0 To dt.Rows.Count - 1
                'Get Value 

                For i = 0 To tbDGV.Rows.Count - 2

                    Dim val As String = getdatacolumn(dt.Rows(t)(0), i)
                    'MsgBox(val)
                    If tbDGV.Rows(i).Cells(t).Value Is Nothing Then
                        tbDGV.Rows(i).Cells(t).Value = val
                    End If
                Next

            Next
        Else
            isCol = False
            MsgBox("No Columns")


        End If
        If isCol = True Then

        End If
    End Sub
    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        TableSelection.Visible = False
        TableMng.Visible = True
        Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT * FROM tables", 1, "1")
        DGVtab.DataSource = dt
    End Sub

    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        If idtb.Text = "" Or IsDBNull(idtb.Text) Then
            'Add
            Dim com As New MySqlCommand
            com.CommandType = CommandType.StoredProcedure
            com.CommandText = "insertTable"
            com.Connection = connection
            'com.Parameters.AddWithValue("id", CInt(idtb.Text))
            com.Parameters.AddWithValue("uID", Form1.idd)
            com.Parameters.AddWithValue("tablename", tbN.Text)
            com.Parameters.AddWithValue("descriptio", descN.Text)
            com.Parameters.AddWithValue("datew", DateTime.Now)
            com.ExecuteNonQuery()
        End If
        Guna2GradientButton1.PerformClick()
        connection.Close()
    End Sub

    Private Sub Guna2CircleButton2_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton2.Click
        connection.Open()

        If idtb.Text <> "" Or IsDBNull(idtb.Text) = False Then
            'check ID
            Dim iss As Boolean = checkId("tables", CInt(idtb.Text))
            If iss = True Then
                'Id is existe
                'Update

                Dim id As Integer = CInt(idtb.Text)
                Dim uid As Integer = Form1.idd
                Dim tbd As String = tbN.Text
                Dim desc As String = descN.Text
                Dim datet As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                Dim command1 As New MySqlCommand($"UPDATE `resgister_db`.`tables` SET `uID`={uid}, `tableName` = '{tbd}' , `Description` = '{desc}' , `datex` = '{datet}' WHERE `ID` = {id};", connection)
                command1.ExecuteNonQuery()

                Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT * FROM tables", 1, "1")
                DGVtab.DataSource = dt
            Else
                MsgBox("id Is Not existe")
            End If
        Else
            MsgBox("id box empty")
        End If
        'Guna2GradientButton1.PerformClick()
        connection.Close()
    End Sub

    Private Sub Register_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'If Form1.valdi = "Admin" Then
        '    Guna2GradientButton1.Visible = True
        'Else
        '    Guna2GradientButton1.Visible = False
        'End If
        'showtables()
        register_inf.Show()
    End Sub

    Private Sub Guna2CircleButton6_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton6.Click
        ColMng.Visible = True
        TableMng.Visible = False
        tbid.Text = DGVtab.SelectedRows(0).Cells(0).Value
        Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT * FROM `columns`", CInt(tbid.Text), "`tID`")
        If dt.Rows.Count = 0 Then
            MsgBox("No column")
        Else
            DGVCol.Rows.Clear()

            For i = 0 To dt.Rows.Count - 1
                DGVCol.Rows.Add(dt.Rows(i)(0), dt.Rows(i)(3), dt.Rows(i)(4))
            Next
        End If
    End Sub

    Private Sub Guna2CircleButton5_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton5.Click, Guna2CircleButton11.Click
        connection.Open()

        For i = 0 To DGVCol.Rows.Count - 2
            Dim str As String = ""
            Try
                str = DGVCol.Rows(i).Cells(0).Value
            Catch ex As Exception
                str = "12"
            End Try

            If IsDBNull(DGVCol.Rows(i).Cells(0).Value) Or IsNothing(DGVCol.Rows(i).Cells(0).Value) Or str = "" Then
                Dim com As New MySqlCommand
                com.CommandType = CommandType.StoredProcedure
                com.CommandText = "insertColumn"
                com.Connection = connection
                com.Parameters.AddWithValue("uID", Form1.idd)
                com.Parameters.AddWithValue("tid", CInt(tbid.Text))
                com.Parameters.AddWithValue("cl", DGVCol.Rows(i).Cells(1).Value)
                com.Parameters.AddWithValue("ht", DGVCol.Rows(i).Cells(2).Value)
                com.Parameters.AddWithValue("dt", DateTime.Now)
                com.ExecuteNonQuery()

            Else
                Dim issa As Boolean = checkId("columns", DGVCol.Rows(i).Cells(0).Value)
                If issa = True Then

                    Dim command1 As New MySqlCommand($"UPDATE `resgister_db`.`columns` SET `uID`={Form1.idd}, `tID` = {CInt(tbid.Text)} , `ColName` = '{DGVCol.Rows(i).Cells(1).Value}' , `HeaderText` = '{DGVCol.Rows(i).Cells(2).Value}'  , `datex` = '" & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & $"' WHERE `ID` = {DGVCol.Rows(i).Cells(0).Value};", connection)
                    command1.ExecuteNonQuery()

                Else
                    MsgBox("Id ERROR")
                End If
            End If
        Next
        Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT * FROM `columns`", CInt(tbid.Text), "`tID`")
        If dt.Rows.Count = 0 Then
            'MsgBox("No column")
        Else
            DGVCol.Rows.Clear()

            For i1 = 0 To dt.Rows.Count - 1
                DGVCol.Rows.Add(dt.Rows(i1)(0), dt.Rows(i1)(3), dt.Rows(i1)(4))
            Next
        End If
        connection.Close()
    End Sub

    Private Sub Guna2CircleButton4_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton4.Click
        ColMng.Visible = False
        TableMng.Visible = True
        DGVCol.Rows.Clear()
    End Sub

    Private Sub Guna2CircleButton3_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton3.Click
        TableSelection.Visible = True
        TableMng.Visible = False
        showtables()
    End Sub

    Private Sub dl_Click(sender As Object, e As EventArgs) Handles dl.Click
        Guna2ShadowPanel2.Visible = False
        TableSelection.Visible = True
        showtables()
    End Sub

    Private Sub Guna2CircleButton8_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton8.Click
        Guna2GradientButton1.PerformClick()
    End Sub

    Private Sub sv_Click(sender As Object, e As EventArgs) Handles sv.Click
        'Delete All Values
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        ' delete recent
        For i = 0 To tbDGV.Columns.Count - 1
            Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT * FROM `columns` ", "'" & tbDGV.Columns(i).HeaderText & "'", $" `tID` = {tbIDa} AND `ColName` = '{tbDGV.Columns(i).Name}' AND HeaderText")
                Dim cID As Integer = dt.Rows(0)(0)

            Dim comman As New MySqlCommand("DELETE FROM `values` WHERE `cID` = @ID ;", connection)
            comman.Parameters.AddWithValue("@ID", cID)
            comman.ExecuteNonQuery()
        Next
        ' Add New
        For i = 0 To tbDGV.Columns.Count - 1
            Dim dt As DataTable = utilities.GetSomeValuesreg("SELECT * FROM `columns` ", "'" & tbDGV.Columns(i).HeaderText & "'", $" tID = {tbIDa} And ColName = '{tbDGV.Columns(i).Name}' AND HeaderText")
            Dim cID As Integer = dt.Rows(0)(0)

            For t = 0 To tbDGV.Rows.Count - 2
                If tbDGV.Rows(t).Cells(i).Value Is Nothing Or IsDBNull(tbDGV.Rows(t).Cells(i).Value) Or tbDGV.Rows(t).Cells(i).Value = "" Then
                Else
                    Dim com As New MySqlCommand
                    com.CommandType = CommandType.StoredProcedure
                    com.CommandText = "insertValue"
                    com.Connection = connection
                    com.Parameters.AddWithValue("uid", Form1.idd)
                    com.Parameters.AddWithValue("tid", tbIDa)
                    com.Parameters.AddWithValue("cid", cID)
                    com.Parameters.AddWithValue("vl", tbDGV.Rows(t).Cells(i).Value)
                    com.Parameters.AddWithValue("datew", DateTime.Now)
                    com.ExecuteNonQuery()
                End If
            Next
        Next
        connection.Close()
        'tbIDa is id on table
    End Sub
End Class