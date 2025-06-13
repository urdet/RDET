Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient
Public Class tras
    Public Shared connectiono As MySqlConnection = Class1.conectiondmc
    Public Shared connectiono1 As MySqlConnection = Class1.conectionscr
    Public Servic As String = ""
    Public servId As Integer
    Public Selectedtable As String
    Private Sub showServices()
        connectiono.Open()
        Try

            Dim command2 As New MySqlCommand("SELECT * FROM services;", connectiono)
            Dim adapter2 As New MySqlDataAdapter(command2)
            Dim table2 As New DataTable()

            adapter2.Fill(table2)
            If table2.Rows.Count = 0 Then
                'MsgBox("Error")
            Else

                FlowLayoutPanel1.Controls.Clear()

                For ligne As Integer = 0 To table2.Rows.Count - 1
                    Dim colonne As Integer = 1
                    Dim nomBouton As String = table2.Rows(ligne)(colonne).ToString
                    Dim bouton As New Guna2GradientButton()
                    Dim id = table2.Rows(ligne)(0)
                    bouton.Name = table2.Rows(ligne)(0).ToString
                    bouton.Text = nomBouton
                    bouton.Font = New Font("Segoe UI", 14)
                    bouton.BorderRadius = 10
                    If table2.Rows(ligne)(5) = 1.0 Or table2.Rows(ligne)(6) = 1.0 Then
                        bouton.FillColor = Color.IndianRed
                        bouton.FillColor2 = Color.Orange
                    ElseIf table2.Rows(ligne)(5) = 2.0 Or table2.Rows(ligne)(6) = 2.0 Then

                        bouton.FillColor = Color.Turquoise
                        bouton.FillColor2 = Color.CadetBlue
                    Else
                        bouton.FillColor = Color.DarkOrange
                        bouton.FillColor2 = Color.Orange
                    End If
                    AddHandler bouton.Click, Sub(s, args) BoutonClique(nomBouton, id)
                    FlowLayoutPanel1.Controls.Add(bouton)
                Next
            End If
            connectiono.Close()
        Catch ex As Exception
            connectiono.Close()
        End Try
    End Sub
    Private Sub BoutonClique(nomB, id)
        If connectiono.State = ConnectionState.Closed Then
            connectiono.Open()
        End If
        GroupeEnter.Visible = True
        Buttons_Pannel.Visible = False
        LbSName.Text = nomB
        servId = id
        Servic = nomB
        '' get types
        'get type of service
        Try
            Dim com As New MySqlCommand($"SELECT Type, ID, serviceImage, Para2, Para3 FROM services WHERE serviceName = '{nomB}';", connectiono)
            Dim ada As New MySqlDataAdapter(com)
            Dim ta As New DataTable()
            ada.Fill(ta)
            If ta.Rows.Count > 0 Then
                Dim strr As String = ta.Rows(0)(0).ToString
                servId = ta.Rows(0)(1)
                Try
                    Dim imagebytes As Byte() = CType(ta.Rows(0)(2), Byte())
                    Using ms As New IO.MemoryStream(imagebytes)
                        'picture.Image = Image.FromStream(ms)
                        PictureBox1.Image = Image.FromStream(ms)
                    End Using
                Catch ex As Exception

                End Try
                ''Show Comptes related to 
                Dim command1 As New MySqlCommand($"SELECT * FROM compteshistoriquedata WHERE (CompteID = {Convert.ToInt32(ta.Rows(0)(3))} Or CompteID = {Convert.ToInt32(ta.Rows(0)(4))}) AND datex like '%{DTP.Value.ToString("yyyy-MM-dd")}%'", connectiono)

                Dim adapter1 As New MySqlDataAdapter(command1)
                Dim table1 As New DataTable()

                adapter1.Fill(table1)
                FlowLayoutPanel2.Controls.Clear()
                For ligne As Integer = 0 To table1.Rows.Count - 1
                    Label1.Text = $" lié au comptes: {DTP.Value.ToString("yyyy-MM-dd")}"
                    Dim colonne As Integer = 1
                    Dim nomBouton As String = table1.Rows(ligne)(colonne).ToString
                    Dim bouton As New ComptesBox()
                    bouton.Name = table1.Rows(ligne)(0).ToString
                    bouton.CN.Text = table1.Rows(ligne)(2).ToString

                    Dim cld As String = table1.Rows(ligne)(3).ToString()
                    bouton.Sld.Text = cld
                    Dim compNam As String = ""
                    bouton.Comp.Text = compNam
                    bouton.Text = nomBouton
                    'AddHandler bouton.Click, Sub(s, args) BoutonClique(nomBouton)
                    FlowLayoutPanel2.Controls.Add(bouton)
                Next

                ''
                Dim t1 As DataTable = utilities.GetSomeValuesdmc("SELECT * FROM fraisargumenttable", ta.Rows(0)(1), "ServiceID")
                If t1.Rows.Count > 0 Then
                    'DGVF.Visible = True
                    'Label4.Visible = True
                    'Label4.Text = $"Frais for {t1.Rows(0)(2).ToString}"
                    Dim stf1 As String = t1.Rows(0)(2).Replace("[", "")
                    Dim stf2 As String = stf1.Replace("]", "")
                    ''Clear 
                    If DataGridView1.Columns.Count = 4 Then
                        DataGridView1.Columns.RemoveAt(2)
                        DataGridView1.Rows.Clear()
                    End If
                    If DataGridView2.Columns.Count = 4 Then
                        DataGridView2.Columns.RemoveAt(2)
                        DataGridView2.Rows.Clear()
                    End If
                    Label3.Text = "0.00"
                    Label6.Text = "0.00"
                    If stf2 = "IN" Then
                        Dim ecol As New DataGridViewTextBoxColumn
                        ecol.HeaderText = "Frais"
                        DataGridView1.Columns.Insert(2, ecol)
                    ElseIf stf2 = "OUT" Then
                        Dim ecol As New DataGridViewTextBoxColumn
                        ecol.HeaderText = "Frais"
                        DataGridView2.Columns.Insert(2, ecol)
                    End If
                End If
                ''
                If strr.Contains("][") Then
                    Dim parts() As String = strr.Split("][")

                    Dim str1 As String = parts(0).Replace("[", "")
                    Dim str2 As String = str1.Replace("]", "")

                    Dim st1 As String = parts(1).Replace("[", "")
                    Dim st2 As String = st1.Replace("]", "")
                    'DGVIN.Visible = True
                    'DGVOUT.Visible = True
                    ' MsgBox("1")
                    'cearch in tranServ
                    DataGridView1.Visible = True
                    Label2.Visible = True
                    Label3.Visible = True
                    DataGridView2.Visible = True
                    Label5.Visible = True
                    Label6.Visible = True


                    Dim comman As New MySqlCommand($"SELECT ID, Mtn, Frais FROM transactionsservices WHERE Service = '{nomB}' AND datex LIKE '%{DTP.Value.ToString("yyyy-MM-dd")}%' AND Typ = 'IN';", connectiono)
                    Dim adapte As New MySqlDataAdapter(comman)
                    Dim tabl As New DataTable()
                    '' Fill In table
                    adapte.Fill(tabl)
                    If tabl.Rows.Count > 0 Then

                        '' Add in New table
                        If DataGridView1.Columns.Count = 4 Then

                            DataGridView1.Rows.Clear()
                            For i = 0 To tabl.Rows.Count - 1
                                If i = 0 Then
                                    Dim y
                                    'DataGridView1.Height += 22
                                    y = Label3.Location.Y + 22
                                    Dim x = Label3.Location.X
                                    Dim p As New Point()
                                    p.Y = y
                                    p.X = x
                                    Label3.Location = p
                                End If
                                DataGridView1.Rows.Add(tabl.Rows(i)(0), tabl.Rows(i)(1), tabl.Rows(i)(2))
                            Next
                        ElseIf DataGridView1.Columns.Count = 3 Then
                            DataGridView1.Rows.Clear()
                            For i = 0 To tabl.Rows.Count - 1
                                If i = 0 Then
                                    Dim y
                                    ' DataGridView1.Height += 22
                                    y = Label3.Location.Y + 22
                                    Dim x = Label3.Location.X
                                    Dim p As New Point()
                                    p.Y = y
                                    p.X = x
                                    Label3.Location = p
                                End If
                                DataGridView1.Rows.Add(tabl.Rows(i)(0), tabl.Rows(i)(1))
                            Next
                        End If


                    End If


                    'get out indos
                    Dim comman1 As New MySqlCommand($"SELECT ID, Mtn, Frais FROM transactionsservices WHERE Service = '{nomB}' AND datex LIKE '%{DTP.Value.ToString("yyyy-MM-dd")}%' AND Typ = 'OUT';", connectiono)
                    Dim adapte1 As New MySqlDataAdapter(comman1)
                    Dim tabl1 As New DataTable()
                    '' Fill OUt table
                    adapte1.Fill(tabl1)
                    If tabl1.Rows.Count > 0 Then
                        '' Add in New table
                        If DataGridView2.Columns.Count = 4 Then

                            DataGridView2.Rows.Clear()
                            For i = 0 To tabl1.Rows.Count - 1
                                If i = 0 Then
                                    Dim y
                                    ' DataGridView2.Height += 22
                                    y = Label6.Location.Y + 22
                                    Dim x = Label6.Location.X
                                    Dim p As New Point()
                                    p.Y = y
                                    p.X = x
                                    Label6.Location = p
                                End If
                                DataGridView2.Rows.Add(tabl1.Rows(i)(0), tabl1.Rows(i)(1), tabl1.Rows(i)(2))
                            Next
                        ElseIf DataGridView2.Columns.Count = 3 Then
                            DataGridView2.Rows.Clear()
                            For i = 0 To tabl1.Rows.Count - 1
                                If i = 0 Then
                                    Dim y
                                    'DataGridView2.Height += 22
                                    y = Label6.Location.Y + 22
                                    Dim x = Label6.Location.X
                                    Dim p As New Point()
                                    p.Y = y
                                    p.X = x
                                    Label6.Location = p
                                End If
                                DataGridView2.Rows.Add(tabl1.Rows(i)(0), tabl1.Rows(i)(1))
                            Next
                        End If

                        '
                    End If
                Else
                    Dim str1 As String = strr.Replace("[", "")
                    Dim str2 As String = str1.Replace("]", "")
                    'MsgBox("2")
                    If str2 = "IN" Then


                        'DGVIN.Visible = True
                        DataGridView1.Visible = True
                        Label6.Visible = False
                        Label3.Visible = True
                        DataGridView2.Visible = False
                        Label5.Visible = False
                        Label2.Visible = True
                        Dim comman As New MySqlCommand($"SELECT ID, Mtn, Frais FROM transactionsservices WHERE Service = '{nomB}' AND datex LIKE '%{DTP.Value.ToString("yyyy-MM-dd")}%' AND Typ = 'IN';", connectiono)
                        Dim adapte As New MySqlDataAdapter(comman)
                        Dim tabl As New DataTable()

                        adapte.Fill(tabl)
                        If tabl.Rows.Count > 0 Then

                            If DataGridView1.Columns.Count = 4 Then

                                DataGridView1.Rows.Clear()
                                For i = 0 To tabl.Rows.Count - 1
                                    If i = 0 Then
                                        Dim y
                                        'DataGridView1.Height += 22
                                        y = Label3.Location.Y + 22
                                        Dim x = Label3.Location.X
                                        Dim p As New Point()
                                        p.Y = y
                                        p.X = x
                                        Label3.Location = p
                                    End If
                                    DataGridView1.Rows.Add(tabl.Rows(i)(0), tabl.Rows(i)(1), tabl.Rows(i)(2))
                                Next
                            ElseIf DataGridView1.Columns.Count = 3 Then
                                DataGridView1.Rows.Clear()
                                For i = 0 To tabl.Rows.Count - 1
                                    If i = 0 Then
                                        Dim y
                                        'DataGridView1.Height += 22
                                        y = Label3.Location.Y + 22
                                        Dim x = Label3.Location.X
                                        Dim p As New Point()
                                        p.Y = y
                                        p.X = x
                                        Label3.Location = p
                                    End If
                                    DataGridView1.Rows.Add(tabl.Rows(i)(0), tabl.Rows(i)(1))
                                Next
                            End If


                            '
                        End If


                    ElseIf str2 = "OUT" Then


                        'DGVOUT.Visible = True
                        DataGridView1.Visible = False
                        DataGridView2.Visible = True
                        Label6.Visible = True
                        Label3.Visible = False
                        Label5.Visible = True
                        Label2.Visible = False
                        Dim comman1 As New MySqlCommand($"SELECT ID, Mtn FROM transactionsservices WHERE Service = '{nomB}' AND datex LIKE '%{DTP.Value.ToString("yyyy-MM-dd")}%' AND Typ = 'OUT';", connectiono)
                        Dim adapte1 As New MySqlDataAdapter(comman1)
                        Dim tabl1 As New DataTable()

                        adapte1.Fill(tabl1)
                        If tabl1.Rows.Count > 0 Then

                            If DataGridView2.Columns.Count = 4 Then

                                DataGridView2.Rows.Clear()
                                For i = 0 To tabl1.Rows.Count - 1
                                    If i = 0 Then
                                        Dim y
                                        'DataGridView2.Height += 22
                                        y = Label6.Location.Y + 22
                                        Dim x = Label6.Location.X
                                        Dim p As New Point()
                                        p.Y = y
                                        p.X = x
                                        Label6.Location = p
                                    End If
                                    DataGridView2.Rows.Add(tabl1.Rows(i)(0), tabl1.Rows(i)(1), tabl1.Rows(i)(2))
                                Next
                            ElseIf DataGridView2.Columns.Count = 3 Then
                                DataGridView2.Rows.Clear()
                                For i = 0 To tabl1.Rows.Count - 1
                                    If i = 0 Then
                                        Dim y
                                        'DataGridView2.Height += 22
                                        y = Label6.Location.Y + 22
                                        Dim x = Label6.Location.X
                                        Dim p As New Point()
                                        p.Y = y
                                        p.X = x
                                        Label6.Location = p
                                    End If
                                    DataGridView2.Rows.Add(tabl1.Rows(i)(0), tabl1.Rows(i)(1))
                                Next
                            End If



                        End If

                    End If
                End If
            End If
            'End If
        Catch ex As Exception

        End Try

        Me.Servic = nomB
        connectiono.Close()
    End Sub
    Public Sub Initialize()
        'Label2536   398; 90
        Label2.Visible = True
        Label5.Visible = True
        Label3.Visible = True
        Label6.Visible = True
        Dim p1 As New Point
        p1.X = 16
        p1.Y = 92
        Label3.Location = p1
        Dim p2 As New Point
        p2.X = 398
        p2.Y = 90
        Label6.Location = p2
        For i = 0 To DataGridView1.Rows.Count - 2
            DataGridView1.Rows.RemoveAt(0)
        Next

        For i = 0 To DataGridView2.Rows.Count - 2
            DataGridView2.Rows.RemoveAt(0)
        Next
    End Sub
    Private Sub TranslactionFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        showServices()
        DTP.Value = DateAndTime.Now
        If Form1.valdi = "Admin" Then
            DTP.Enabled = True
        Else
            DTP.Enabled = False
        End If
    End Sub



    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()

    End Sub

    Public Function CheckID(ByVal ID As Integer)
        Dim query As String = "CheckIDExists" ' Name of your stored procedure


        Using command As New MySqlCommand(query, connectiono)
            command.CommandType = CommandType.StoredProcedure

            ' Input parameter
            command.Parameters.AddWithValue("checkID", ID)

            ' Output parameter
            command.Parameters.Add("@idExists", MySqlDbType.Bit)
            command.Parameters("@idExists").Direction = ParameterDirection.Output

            Try
                connectiono.Open()
                command.ExecuteNonQuery()

                Dim exists As Boolean = Convert.ToBoolean(command.Parameters("@idExists").Value)
                If exists Then
                    Return True
                Else
                    Return False
                End If
                connectiono.Close()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
                Return False
            End Try
        End Using

    End Function

    Private Sub Guna2CircleButton3_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton3.Click
        GridControl1.ShowRibbonPrintPreview()
    End Sub

    Private Sub DataGridView1_RowsAdded(sender As Object, e As DataGridViewRowsAddedEventArgs) Handles DataGridView1.RowsAdded

        Dim y
        If DataGridView1.Rows.Count = 1 Then
            y = Label3.Location.Y
        Else
            'DataGridView1.Height += 22
            y = Label3.Location.Y + 22
        End If

        If DataGridView1.ColumnCount = 4 Then
            Dim img As Image = My.Resources.delete
            DataGridView1.Rows(e.RowIndex).Cells(3).Value = img
        Else
            Dim img As Image = My.Resources.delete
            DataGridView1.Rows(e.RowIndex).Cells(2).Value = img
        End If

        Dim x = Label3.Location.X
        Dim p As New Point()
        p.Y = y
        p.X = x
        Label3.Location = p
    End Sub



    Private Sub DataGridView2_RowsAdded(sender As Object, e As DataGridViewRowsAddedEventArgs) Handles DataGridView2.RowsAdded
        Dim y
        If DataGridView2.Rows.Count = 1 Then

            y = Label6.Location.Y


        Else
            'DataGridView2.Height += 22
            y = Label6.Location.Y + 22

        End If

        If DataGridView2.ColumnCount = 4 Then
            Dim img As Image = My.Resources.delete
            DataGridView2.Rows(e.RowIndex).Cells(3).Value = img
        Else
            Dim img As Image = My.Resources.delete
            DataGridView2.Rows(e.RowIndex).Cells(2).Value = img
        End If

        Dim x = Label6.Location.X
        Dim p As New Point()
        p.Y = y
        p.X = x
        Label6.Location = p


    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        Dim Typ = "IN" ''IN
        Dim DataGridVie As DataGridView = DataGridView1
        If DTP.Value.ToString("yyyy-MM-dd") = DateTime.Now.ToString("yyyy-MM-dd") Then

            If DataGridView1.Columns.Count = 3 Then
                If e.ColumnIndex = 2 Then
                    If IsDBNull(DataGridView1.Rows(e.RowIndex).Cells(1).Value) = True Or DataGridView1.Rows(e.RowIndex).Cells(1).Value Is Nothing Then
                        Exit Sub
                    End If
                    If IsDBNull(DataGridView1.Rows(e.RowIndex).Cells(0).Value) = True Or DataGridView1.Rows(e.RowIndex).Cells(0).Value Is Nothing Then
                        DataGridView1.Rows.RemoveAt(e.RowIndex)
                        Exit Sub
                    End If
                    '' delete procudures
                    Dim f As Decimal
                    f = 0

                    If DTP.Value.ToString("yyyy-MM-dd") = DateAndTime.Now.ToString("yyyy-MM-dd") Then
                        Dim bl As Boolean
                        If DataGridView1.Visible = True And DataGridView2.Visible = True Then
                            bl = True
                        Else
                            bl = False
                        End If
                        utilities.DeleteTranslation(DataGridVie.Rows(e.RowIndex).Cells(1).Value, 0, DateAndTime.Now, DataGridVie.Rows(e.RowIndex).Cells(0).Value, Typ, servId)

                        'utilities.Deletetrans(bl, DataGridView1.Rows(e.RowIndex).Cells(0).Value, "IN", servId, LbSName.Text, DataGridView1.Rows(e.RowIndex).Cells(1).Value, "", DTP.Value, Form1.first, f)
                        DataGridView1.Rows.RemoveAt(e.RowIndex)
                    Else
                        MsgBox("Imposible de effectuer")
                    End If
                End If
            ElseIf DataGridView1.Columns.Count = 4 Then
                If e.ColumnIndex = 3 Then
                    '' delete procudures with frais
                    Try
                        If IsDBNull(DataGridView1.Rows(e.RowIndex).Cells(1).Value) = True Or DataGridView1.Rows(e.RowIndex).Cells(1).Value Is Nothing Then
                            Exit Sub
                        End If
                        If IsDBNull(DataGridView1.Rows(e.RowIndex).Cells(0).Value) = True Or DataGridView1.Rows(e.RowIndex).Cells(0).Value Is Nothing Then
                            DataGridView1.Rows.RemoveAt(e.RowIndex)
                            Exit Sub
                        End If
                        Dim f As Decimal
                        If IsDBNull(DataGridView1.Rows(e.RowIndex).Cells(2).Value) Then
                            f = 0
                        Else
                            f = DataGridView1.Rows(e.RowIndex).Cells(2).Value
                        End If

                        If DTP.Value.ToString("yyyy-MM-dd") = DateAndTime.Now.ToString("yyyy-MM-dd") Then
                            Dim bl As Boolean
                            If DataGridView1.Visible = True And DataGridView2.Visible = True Then
                                bl = True
                            Else
                                bl = False
                            End If
                            utilities.DeleteTranslation(DataGridVie.Rows(e.RowIndex).Cells(1).Value, DataGridVie.Rows(e.RowIndex).Cells(2).Value, DateAndTime.Now, DataGridVie.Rows(e.RowIndex).Cells(0).Value, Typ, servId)

                            ''utilities.Deletetrans(bl, DataGridView1.Rows(e.RowIndex).Cells(0).Value, "IN", servId, LbSName.Text, DataGridView1.Rows(e.RowIndex).Cells(1).Value, "", DTP.Value, Form1.first, f)
                            DataGridView1.Rows.RemoveAt(e.RowIndex)
                        Else
                            MsgBox("Imposible de effectuer")
                        End If
                    Catch ex As Exception

                    End Try


                End If
            Else
                MsgBox("There is a error in column count")
            End If
            AutoOperations.UComptes1(DTP.Value)
        Else
            MsgBox("Imposible de effectuer")
        End If
    End Sub

    Private Sub DataGridView2_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellClick
        Dim DataGridVie As DataGridView = DataGridView2
        Dim typ = "OUT"
        If DataGridView2.Columns.Count = 3 Then

            If e.ColumnIndex = 2 Then
                If IsDBNull(DataGridVie.Rows(e.RowIndex).Cells(1).Value) = True Or DataGridVie.Rows(e.RowIndex).Cells(1).Value Is Nothing Then
                    Exit Sub
                End If
                If IsDBNull(DataGridVie.Rows(e.RowIndex).Cells(0).Value) = True Or DataGridVie.Rows(e.RowIndex).Cells(0).Value Is Nothing Then
                    DataGridVie.Rows.RemoveAt(e.RowIndex)
                    Exit Sub
                End If
                If DTP.Value.ToString("yyyy-MM-dd") = DateAndTime.Now.ToString("yyyy-MM-dd") Then
                    '' delete procudures
                    Dim f As Decimal
                    f = 0

                    Dim bl As Boolean
                    If DataGridVie.Visible = True And DataGridView1.Visible = True Then
                        bl = True
                    Else
                        bl = False
                    End If

                    'utilities.Deletetrans(bl, DataGridVie.Rows(e.RowIndex).Cells(0).Value, "OUT", servId, LbSName.Text, DataGridVie.Rows(e.RowIndex).Cells(1).Value, "", DTP.Value, Form1.first, f)

                    utilities.DeleteTranslation(DataGridVie.Rows(e.RowIndex).Cells(1).Value, 0, DateAndTime.Now, DataGridVie.Rows(e.RowIndex).Cells(0).Value, typ, servId)

                    DataGridVie.Rows.RemoveAt(e.RowIndex)

                Else
                    MsgBox("Imposible de effectuer")
                End If



            End If
        ElseIf DataGridVie.Columns.Count = 4 Then
            If e.ColumnIndex = 3 Then
                '' delete procudures with frais
                Try

                    If IsDBNull(DataGridVie.Rows(e.RowIndex).Cells(1).Value) = True Or DataGridVie.Rows(e.RowIndex).Cells(1).Value Is Nothing Then
                        Exit Sub
                    End If
                    If IsDBNull(DataGridVie.Rows(e.RowIndex).Cells(0).Value) = True Or DataGridVie.Rows(e.RowIndex).Cells(0).Value Is Nothing Then
                        DataGridVie.Rows.RemoveAt(e.RowIndex)
                        Exit Sub
                    End If
                    If DTP.Value.ToString("yyyy-MM-dd") = DateAndTime.Now.ToString("yyyy-MM-dd") Then
                        Dim f As Decimal
                        If IsDBNull(DataGridVie.Rows(e.RowIndex).Cells(2).Value) Then
                            f = 0
                        Else
                            f = DataGridVie.Rows(e.RowIndex).Cells(2).Value
                        End If

                        Dim bl As Boolean
                        If DataGridVie.Visible = True And DataGridView1.Visible = True Then
                            bl = True
                        Else
                            bl = False
                        End If

                        ''utilities.Deletetrans(bl, DataGridVie.Rows(e.RowIndex).Cells(0).Value, "OUT", servId, LbSName.Text, DataGridVie.Rows(e.RowIndex).Cells(1).Value, "", DTP.Value, Form1.first, f)
                        utilities.DeleteTranslation(DataGridVie.Rows(e.RowIndex).Cells(1).Value, DataGridVie.Rows(e.RowIndex).Cells(2).Value, DateAndTime.Now, DataGridVie.Rows(e.RowIndex).Cells(0).Value, typ, servId)

                        DataGridVie.Rows.RemoveAt(e.RowIndex)
                    Else
                        MsgBox("Imposible de effectuer")
                    End If
                Catch ex As Exception

                End Try



                '' delete procudures with frais
            End If
            fillComptesdta()
        Else
            MsgBox("There is a error in column count")
        End If
    End Sub

    Private Sub DataGridView1_RowsRemoved(sender As Object, e As DataGridViewRowsRemovedEventArgs) Handles DataGridView1.RowsRemoved
        'If DataGridView1.Rows.Count <> 0 Then
        '    Dim y
        '    DataGridView1.Height -= 22
        '    y = Label3.Location.Y - 22
        '    Dim x = Label3.Location.X
        '    Dim p As New Point()
        '    p.Y = y
        '    p.X = x
        '    Label3.Location = p
        'End If
    End Sub

    Private Sub DataGridView2_RowsRemoved(sender As Object, e As DataGridViewRowsRemovedEventArgs) Handles DataGridView2.RowsRemoved
        'If DataGridView2.Rows.Count <> 0 Then
        '    Dim y

        '    DataGridView2.Height -= 22
        '    y = Label6.Location.Y - 22

        '    Dim x = Label6.Location.X
        '    Dim p As New Point()
        '    p.Y = y
        '    p.X = x
        '    Label6.Location = p
        'End If

    End Sub
    Public Sub fillComptesdta()
        Dim com As New MySqlCommand($"SELECT Type, ID, serviceImage, Para2, Para3 FROM services WHERE ID = '{servId}';", connectiono)
        Dim ada As New MySqlDataAdapter(com)
        Dim ta As New DataTable()
        ada.Fill(ta)
        If ta.Rows.Count > 0 Then
            Dim command1 As New MySqlCommand($"SELECT * FROM compteshistoriquedata WHERE (CompteID = {Convert.ToInt32(ta.Rows(0)(3))} Or CompteID = {Convert.ToInt32(ta.Rows(0)(4))}) AND datex like '%{DTP.Value.ToString("yyyy-MM-dd")}%'", connectiono)

            Dim adapter1 As New MySqlDataAdapter(command1)
            Dim table1 As New DataTable()

            adapter1.Fill(table1)
            FlowLayoutPanel2.Controls.Clear()
            For ligne As Integer = 0 To table1.Rows.Count - 1
                Label1.Text = $" lié au comptes: {DTP.Value.ToString("yyyy-MM-dd")}"
                Dim colonne As Integer = 1
                Dim nomBouton As String = table1.Rows(ligne)(colonne).ToString
                Dim bouton As New ComptesBox()
                bouton.Name = table1.Rows(ligne)(0).ToString
                bouton.CN.Text = table1.Rows(ligne)(2).ToString

                Dim cld As String = table1.Rows(ligne)(3).ToString()
                bouton.Sld.Text = cld
                Dim compNam As String = ""
                bouton.Comp.Text = compNam
                bouton.Text = nomBouton
                'AddHandler bouton.Click, Sub(s, args) BoutonClique(nomBouton)
                FlowLayoutPanel2.Controls.Add(bouton)
            Next
        End If

    End Sub
    Public Async Function identifyAndSave(Dataa As DataGridView) As Task
        'MsgBox(Dataa.Rows(0).Cells(1).Value)
        If DTP.Value.ToString("yyyy-MM-dd") = DateAndTime.Now.ToString("yyyy-MM-dd") Then
            Dim typp As String
            If Dataa.Name = "DataGridView1" Then
                '' IN
                typp = "IN"
            ElseIf Dataa.Name = "DataGridView2" Then
                '' OUT
                typp = "OUT"
            Else
                MsgBox("error")
                Exit Function
            End If
            Dim bl As Boolean
            If DataGridView1.Visible = True And DataGridView2.Visible = True Then
                bl = True
            Else
                bl = False
            End If
            If Dataa.Columns.Count = 4 Then
                '' frais
                For i = 0 To Dataa.Rows.Count - 2
                    If IsDBNull(Dataa.Rows(i).Cells(0).Value) = True Or Dataa.Rows(i).Cells(0).Value Is Nothing Then
                        '
                        '' Calcule frais
                        ''MsgBox("Frais ADD")
                        Try
                            '' Get Frais
                            Dim dt123 As Decimal = utilities.Getfrais(servId, Dataa.Rows(i).Cells(1).Value, typp)
                            Dataa.Rows(i).Cells(2).Value = dt123
                            ''
                            Dim f As Decimal
                            If IsDBNull(Dataa.Rows(i).Cells(2).Value) Then
                                f = 0
                            Else
                                f = Dataa.Rows(i).Cells(2).Value
                            End If
                            ''Add
                            utilities.AddTransServices(bl, typp, servId, LbSName.Text, Dataa.Rows(i).Cells(1).Value, "", DTP.Value, Form1.first, f)
                            Dim dt As DataTable = utilities.GetSomeValuesdmc("Select ID FROM transactionsservices", $"1 Order By ID DESC limit 1", "1")
                            Dataa.Rows(i).Cells(0).Value = dt.Rows(0)(0)

                        Catch ex As Exception
                            MsgBox(ex.Message)
                        End Try

                    Else
                        ''Update
                        'MsgBox(" Frais update")
                        Try
                            Dim f As Decimal
                            If IsDBNull(Dataa.Rows(i).Cells(2).Value) Then
                                f = 0
                            Else
                                f = Dataa.Rows(i).Cells(2).Value
                            End If
                            utilities.Updatetrans(bl, Dataa.Rows(i).Cells(0).Value, typp, servId, LbSName.Text, Dataa.Rows(i).Cells(1).Value, "", DTP.Value, Form1.first, f)

                        Catch ex As Exception
                            MsgBox(ex.Message)
                        End Try

                    End If
                    Await Task.Delay(10)
                Next

            ElseIf Dataa.Columns.Count = 3 Then
                For i = 0 To Dataa.Rows.Count - 2
                    If IsDBNull(Dataa.Rows(i).Cells(0).Value) = True Or Dataa.Rows(i).Cells(0).Value Is Nothing Then
                        'MsgBox("Non Frais ADD")
                        'Add
                        Dim f As Decimal
                        f = 0

                        utilities.AddTransServices(bl, typp, servId, LbSName.Text, Dataa.Rows(i).Cells(1).Value, "", DTP.Value, Form1.first, f)

                        Dim dt As DataTable = utilities.GetSomeValuesdmc("Select ID FROM transactionsservices", $"1 Order By ID DESC limit 1", "1")
                        Dataa.Rows(i).Cells(0).Value = dt.Rows(0)(0)
                        'fillComptesdta()
                    Else
                        ''Update
                        'MsgBox("Non Frais Update")
                        Dim f As Decimal
                        f = 0

                        utilities.Updatetrans(bl, Dataa.Rows(i).Cells(0).Value, typp, servId, LbSName.Text, Dataa.Rows(i).Cells(1).Value, "", DTP.Value, Form1.first, f)
                        'fillComptesdta()
                    End If
                    Await Task.Delay(10)
                Next
                ' fillComptesdta()
            End If

        Else
            MsgBox("Imposible de effectuer")
        End If

    End Function
    Private Async Sub DataGridView1_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridView1.SelectionChanged
        ''
        'MsgBox("ok")
        If ToggleSwitch1.IsOn = True Then
            Await identifyAndSave(DataGridView1)
            fillComptesdta()
        End If

        Try

            Dim tlt As Decimal = 0
            For i = 0 To DataGridView1.Rows.Count - 1
                tlt += DataGridView1.Rows(i).Cells(1).Value
            Next
            Label3.Text = tlt.ToString("0.00")
        Catch ex As Exception

        End Try
    End Sub

    Private Async Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown

        If e.KeyCode = Keys.Enter Then
            ''
            'MsgBox("ok")
            If ToggleSwitch1.IsOn = True Then
                Await identifyAndSave(DataGridView1)
                fillComptesdta()
            End If

            Try

                Dim tlt As Decimal = 0
                For i = 0 To DataGridView1.Rows.Count - 1
                    tlt += DataGridView1.Rows(i).Cells(1).Value
                Next
                Label3.Text = tlt.ToString("0.00")
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Async Sub DataGridView2_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridView2.SelectionChanged
        If ToggleSwitch1.IsOn = True Then
            Await identifyAndSave(DataGridView2)
            fillComptesdta()
        End If

        Try
            'MsgBox("ok")

            Dim tlt As Decimal = 0
            For i = 0 To DataGridView2.Rows.Count - 1
                tlt += DataGridView2.Rows(i).Cells(1).Value
            Next
            Label6.Text = tlt.ToString("0.00")

        Catch ex As Exception

        End Try
    End Sub

    Private Async Sub DataGridView2_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView2.KeyDown

        If e.KeyCode = Keys.Enter Then
            'MsgBox("ok")
            If ToggleSwitch1.IsOn = True Then
                Await identifyAndSave(DataGridView2)
                fillComptesdta()
            End If

            Try

                Dim tlt As Decimal = 0
                For i = 0 To DataGridView2.Rows.Count - 1
                    tlt += DataGridView2.Rows(i).Cells(1).Value
                Next
                Label6.Text = tlt.ToString
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub Guna2CircleButton8_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton8.Click
        GroupeEnter.Visible = False
        Buttons_Pannel.Visible = True
        Initialize()
    End Sub

    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        AutoOperations.UComptes1(DTP.Value)
        fillComptesdta()
    End Sub

    Private Async Sub Guna2CircleButton4_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton4.Click
        Try
            FlowLayoutPanel2.Visible = False
            Await identifyAndSave(DataGridView1)
            Await identifyAndSave(DataGridView2)
            AutoOperations.UComptes1(DTP.Value)
            fillComptesdta()
            FlowLayoutPanel2.Visible = True
            MsgBox("تم الحفظ بنجاح")
        Catch ex As Exception

        End Try

    End Sub

    Private Sub ToggleSwitch1_Toggled(sender As Object, e As EventArgs) Handles ToggleSwitch1.Toggled
        If ToggleSwitch1.IsOn = False Then
            Guna2CircleButton4.Visible = True

        Else
            Guna2CircleButton4.Visible = False
        End If
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub ToolStripTextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ToolStripTextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim sldAnc As Decimal = utilities.getsld(1)
            Dim CurrentSolde As Decimal = CDec(ToolStripTextBox1.Text)
            Dim fawatir As Decimal = sldAnc - CurrentSolde
            DataGridView1.Rows.Add(Nothing, fawatir.ToString())
        End If
    End Sub
End Class