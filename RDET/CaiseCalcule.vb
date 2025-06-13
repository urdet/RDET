Imports MySql.Data.MySqlClient

Public Class CaiseCalcule
    Public Shared connectiona As MySqlConnection = Class1.conectionscr
    Public Shared connectiono As MySqlConnection = Class1.conectiondmc
    Public Function somme() As Decimal
        Try
            Dim s1 As Decimal
            If TextBox1.Text = "" Or TextBox1.Text Is Nothing Then
                s1 = 0
            Else
                s1 = CDec(dixmille.Text) * CDec(TextBox1.Text.Replace(",", "."))
            End If

            Dim s2 As Decimal
            If TextBox2.Text = "" Or TextBox2.Text Is Nothing Then
                s2 = 0
            Else
                s2 = CDec(mille.Text) * CDec(TextBox2.Text.Replace(",", "."))
            End If


            Dim s3 As Decimal
            If TextBox3.Text = "" Or TextBox3.Text Is Nothing Then
                s3 = 0
            Else
                s3 = CDec(cent.Text) * CDec(TextBox3.Text.Replace(",", "."))
            End If


            Dim s4 As Decimal
            If TextBox4.Text = "" Or TextBox4.Text Is Nothing Then
                s4 = 0
            Else
                s4 = CDec(cinqente.Text) * CDec(TextBox4.Text.Replace(",", "."))
            End If

            Dim s5 As Decimal
            If TextBox5.Text = "" Or TextBox5.Text Is Nothing Then
                s5 = 0
            Else
                s5 = CDec(vinght.Text) * CDec(TextBox5.Text.Replace(",", "."))
            End If
            Dim s6 As Decimal
            If TextBox6.Text = "" Or TextBox6.Text Is Nothing Then
                s6 = 0
            Else
                s6 = CDec(dix.Text) * CDec(TextBox6.Text.Replace(",", "."))
            End If
            Dim s7 As Decimal
            If TextBox7.Text = "" Or TextBox7.Text Is Nothing Then
                s7 = 0
            Else
                s7 = CDec(cinq.Text) * CDec(TextBox7.Text.Replace(",", "."))
            End If
            Dim s8 As Decimal
            If TextBox8.Text = "" Or TextBox8.Text Is Nothing Then
                s8 = 0
            Else
                s8 = CDec(deux.Text) * CDec(TextBox8.Text.Replace(",", "."))
            End If
            Dim s9 As Decimal
            If TextBox9.Text = "" Or TextBox9.Text Is Nothing Then
                s9 = 0
            Else
                s9 = CDec(un.Text) * CDec(TextBox9.Text.Replace(",", "."))
            End If
            Dim s10 As Decimal
            If TextBox10.Text = "" Or TextBox10.Text Is Nothing Then
                s10 = 0
            Else
                s10 = CDec(demi.Text) * CDec(TextBox10.Text.Replace(",", "."))
            End If
            Dim s11 As Decimal
            If TextBox11.Text = "" Or TextBox11.Text Is Nothing Then
                s11 = 0
            Else
                s11 = CDec(deuxcent.Text) * CDec(TextBox11.Text.Replace(",", "."))
            End If
            Return (s1 + s2 + s3 + s4 + s5 + s6 + s7 + s8 + s9 + s10 + s11)
        Catch ex As Exception
            Return -1
        End Try
    End Function
    Private Sub filldataNonPayé()
        Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT * FROM nonpayedetail", 1, "1")
        If dt.Rows.Count > 0 Then
            Guna2DataGridView1.Rows.Clear()
            For i = 0 To dt.Rows.Count - 1
                Guna2DataGridView1.Rows.Add(dt.Rows(i)(0), dt.Rows(i)(1), dt.Rows(i)(2), dt.Rows(i)(3))
            Next
            Try
                Dim tlt As Decimal = 0
                For i = 0 To Guna2DataGridView1.Rows.Count - 1
                    tlt += Guna2DataGridView1.Rows(i).Cells(2).Value
                Next
                Label6.Text = tlt.ToString
            Catch ex As Exception

            End Try
        Else
            MsgBox("Aucune facture non payé")
        End If
    End Sub
    Private Sub filldataCasseCalculé()
        If connectiono.State = ConnectionState.Closed Then
            connectiono.Open()
        End If
        Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT * FROM caissecalcul", 1, $"datex LIKE '%" & DateTime.Now.ToString("yyyy-MM-dd") & "%' OR 2")
        If dt.Rows.Count > 0 Then
            Dim dt1 As DataTable = utilities.GetSomeValuesdmc("SELECT `Count` FROM caissecalcul", 1, $"datex LIKE '%" & DateTime.Now.ToString("yyyy-MM-dd") & $"%' AND Type = '{"10000.00"}' OR 2")
            If dt1.Rows(0)(0) = 0 Then
            Else
                TextBox1.Text = dt1.Rows(0)(0)
            End If

            Dim dt2 As DataTable = utilities.GetSomeValuesdmc("SELECT `Count` FROM caissecalcul", 1, $"datex LIKE '%" & DateTime.Now.ToString("yyyy-MM-dd") & $"%' AND Type = '{"1000.00"}' OR 2")
            If dt2.Rows(0)(0) = 0 Then
            Else
                TextBox2.Text = dt2.Rows(0)(0)
            End If
            Dim dt3 As DataTable = utilities.GetSomeValuesdmc("SELECT `Count` FROM caissecalcul", 1, $"datex Like '%" & DateTime.Now.ToString("yyyy-MM-dd") & $"%' AND Type = '{"200.00"}' OR 2")
            If dt3.Rows(0)(0) = 0 Then
            Else
                TextBox11.Text = dt3.Rows(0)(0)
            End If

            Dim dt4 As DataTable = utilities.GetSomeValuesdmc("SELECT `Count` FROM caissecalcul", 1, $"datex LIKE '%" & DateTime.Now.ToString("yyyy-MM-dd") & $"%' AND Type = '{"100.00"}' OR 2")
            If dt4.Rows(0)(0) = 0 Then
            Else
                TextBox3.Text = dt4.Rows(0)(0)
            End If
            Dim dt5 As DataTable = utilities.GetSomeValuesdmc("SELECT `Count` FROM caissecalcul", 1, $"datex LIKE '%" & DateTime.Now.ToString("yyyy-MM-dd") & $"%' AND Type = '{"50.00"}' OR 2")
            If dt5.Rows(0)(0) = 0 Then
            Else
                TextBox4.Text = dt5.Rows(0)(0)
            End If
            Dim dt6 As DataTable = utilities.GetSomeValuesdmc("SELECT `Count` FROM caissecalcul", 1, $"datex LIKE '%" & DateTime.Now.ToString("yyyy-MM-dd") & $"%' AND Type = '{"20.00"}' OR 2")
            If dt6.Rows(0)(0) = 0 Then
            Else
                TextBox5.Text = dt6.Rows(0)(0)
            End If
            Dim dt7 As DataTable = utilities.GetSomeValuesdmc("SELECT `Count` FROM caissecalcul", 1, $"datex LIKE '%" & DateTime.Now.ToString("yyyy-MM-dd") & $"%' AND Type = '{"10.00"}' OR 2")
            If dt7.Rows(0)(0) = 0 Then
            Else
                TextBox6.Text = dt7.Rows(0)(0)
            End If
            Dim dt8 As DataTable = utilities.GetSomeValuesdmc("SELECT `Count` FROM caissecalcul", 1, $"datex LIKE '%" & DateTime.Now.ToString("yyyy-MM-dd") & $"%' AND Type = '{"5.00"}' OR 2")
            If dt8.Rows(0)(0) = 0 Then
            Else
                TextBox7.Text = dt8.Rows(0)(0)
            End If
            Dim dt9 As DataTable = utilities.GetSomeValuesdmc("SELECT `Count` FROM caissecalcul", 1, $"datex LIKE '%" & DateTime.Now.ToString("yyyy-MM-dd") & $"%' AND Type = '{"2.00"}' OR 2")
            If dt9.Rows(0)(0) = 0 Then
            Else
                TextBox8.Text = dt9.Rows(0)(0)
            End If
            Dim dt10 As DataTable = utilities.GetSomeValuesdmc("SELECT `Count` FROM caissecalcul", 1, $"datex LIKE '%" & DateTime.Now.ToString("yyyy-MM-dd") & $"%' AND Type = '{"1.00"}' OR 2")
            If dt10.Rows(0)(0) = 0 Then
            Else
                TextBox9.Text = dt10.Rows(0)(0)
            End If
            Dim dt11 As DataTable = utilities.GetSomeValuesdmc("SELECT `Count` FROM caissecalcul", 1, $"datex LIKE '%" & DateTime.Now.ToString("yyyy-MM-dd") & $"%' AND Type = '{"0.50"}' OR 2")
            If dt11.Rows(0)(0) = 0 Then
            Else
                TextBox10.Text = dt11.Rows(0)(0)
            End If
        Else
            Dim comman As New MySqlCommand("INSERT INTO `caissecalcul` (`Type`, `Count`, `datex`) VALUES (@Type, @Count, @datex);", connectiono)
            comman.Parameters.AddWithValue("@Type", "10000.00")
            comman.Parameters.AddWithValue("@Count", 0)
            comman.Parameters.AddWithValue("@datex", DateAndTime.Now)
            comman.ExecuteNonQuery()
            'TextBox1.Text = "0"

            Dim comman1 As New MySqlCommand("INSERT INTO `caissecalcul` (`Type`, `Count`, `datex`) VALUES (@Type, @Count, @datex);", connectiono)
            comman1.Parameters.AddWithValue("@Type", "1000.00")
            comman1.Parameters.AddWithValue("@Count", 0)
            comman1.Parameters.AddWithValue("@datex", DateAndTime.Now)
            comman1.ExecuteNonQuery()
            'TextBox2.Text = "0"

            Dim comman2 As New MySqlCommand("INSERT INTO `caissecalcul` (`Type`, `Count`, `datex`) VALUES (@Type, @Count, @datex);", connectiono)
            comman2.Parameters.AddWithValue("@Type", "200.00")
            comman2.Parameters.AddWithValue("@Count", 0)
            comman2.Parameters.AddWithValue("@datex", DateAndTime.Now)
            comman2.ExecuteNonQuery()
            'TextBox11.Text = "0"

            Dim comman3 As New MySqlCommand("INSERT INTO `caissecalcul` (`Type`, `Count`, `datex`) VALUES (@Type, @Count, @datex);", connectiono)
            comman3.Parameters.AddWithValue("@Type", "100.00")
            comman3.Parameters.AddWithValue("@Count", 0)
            comman3.Parameters.AddWithValue("@datex", DateAndTime.Now)
            comman3.ExecuteNonQuery()
            'TextBox3.Text = "0"
            Dim comman4 As New MySqlCommand("INSERT INTO `caissecalcul` (`Type`, `Count`, `datex`) VALUES (@Type, @Count, @datex);", connectiono)
            comman4.Parameters.AddWithValue("@Type", "50.00")
            comman4.Parameters.AddWithValue("@Count", 0)
            comman4.Parameters.AddWithValue("@datex", DateAndTime.Now)
            comman4.ExecuteNonQuery()
            'TextBox4.Text = "0"
            Dim comman5 As New MySqlCommand("INSERT INTO `caissecalcul` (`Type`, `Count`, `datex`) VALUES (@Type, @Count, @datex);", connectiono)
            comman5.Parameters.AddWithValue("@Type", "20.00")
            comman5.Parameters.AddWithValue("@Count", 0)
            comman5.Parameters.AddWithValue("@datex", DateAndTime.Now)
            comman5.ExecuteNonQuery()
            'TextBox5.Text = "0"
            Dim comman6 As New MySqlCommand("INSERT INTO `caissecalcul` (`Type`, `Count`, `datex`) VALUES (@Type, @Count, @datex);", connectiono)
            comman6.Parameters.AddWithValue("@Type", "10.00")
            comman6.Parameters.AddWithValue("@Count", 0)
            comman6.Parameters.AddWithValue("@datex", DateAndTime.Now)
            comman6.ExecuteNonQuery()
            'TextBox6.Text = "0"
            Dim comman7 As New MySqlCommand("INSERT INTO `caissecalcul` (`Type`, `Count`, `datex`) VALUES (@Type, @Count, @datex);", connectiono)
            comman7.Parameters.AddWithValue("@Type", "5.00")
            comman7.Parameters.AddWithValue("@Count", 0)
            comman7.Parameters.AddWithValue("@datex", DateAndTime.Now)
            comman7.ExecuteNonQuery()
            'TextBox7.Text = "0"
            Dim comman8 As New MySqlCommand("INSERT INTO `caissecalcul` (`Type`, `Count`, `datex`) VALUES (@Type, @Count, @datex);", connectiono)
            comman8.Parameters.AddWithValue("@Type", "2.00")
            comman8.Parameters.AddWithValue("@Count", 0)
            comman8.Parameters.AddWithValue("@datex", DateAndTime.Now)
            comman8.ExecuteNonQuery()
            'TextBox8.Text = "0"
            Dim comman9 As New MySqlCommand("INSERT INTO `caissecalcul` (`Type`, `Count`, `datex`) VALUES (@Type, @Count, @datex);", connectiono)
            comman9.Parameters.AddWithValue("@Type", "1.00")
            comman9.Parameters.AddWithValue("@Count", 0)
            comman9.Parameters.AddWithValue("@datex", DateAndTime.Now)
            comman9.ExecuteNonQuery()
            'TextBox9.Text = "0"
            Dim comman10 As New MySqlCommand("INSERT INTO `caissecalcul` (`Type`, `Count`, `datex`) VALUES (@Type, @Count, @datex);", connectiono)
            comman10.Parameters.AddWithValue("@Type", "0.50")
            comman10.Parameters.AddWithValue("@Count", 0)
            comman10.Parameters.AddWithValue("@datex", DateAndTime.Now)
            comman10.ExecuteNonQuery()
            'TextBox10.Text = "0"
        End If
        connectiono.Close()
    End Sub

    Private Sub CaiseCalcule_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        filldataNonPayé()
        Csld.Text = $"Caisse ancien: {utilities.getsld(11).ToString}"
        Label7.Text = $"Non payé ancien: {utilities.getsld(10).ToString}"
        filldataCasseCalculé()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged, TextBox2.TextChanged, TextBox3.TextChanged, TextBox4.TextChanged, TextBox5.TextChanged, TextBox6.TextChanged, TextBox7.TextChanged, TextBox8.TextChanged, TextBox9.TextChanged, TextBox10.TextChanged, TextBox11.TextChanged
        Label4.Text = somme().ToString("0.00 DH")
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        If connectiona.State = ConnectionState.Closed Then
            connectiona.Open()
        End If
        If connectiono.State = ConnectionState.Closed Then
            connectiono.Open()
        End If
        Try
            Dim s1 As Decimal
            If TextBox1.Text = "" Or TextBox1.Text Is Nothing Then
                s1 = 0
            Else
                s1 = CDec(TextBox1.Text)
            End If

            Dim s2 As Decimal
            If TextBox2.Text = "" Or TextBox2.Text Is Nothing Then
                s2 = 0
            Else
                s2 = CDec(TextBox2.Text)
            End If


            Dim s3 As Decimal
            If TextBox3.Text = "" Or TextBox3.Text Is Nothing Then
                s3 = 0
            Else
                s3 = CDec(TextBox3.Text)
            End If


            Dim s4 As Decimal
            If TextBox4.Text = "" Or TextBox4.Text Is Nothing Then
                s4 = 0
            Else
                s4 = CDec(TextBox4.Text)
            End If

            Dim s5 As Decimal
            If TextBox5.Text = "" Or TextBox5.Text Is Nothing Then
                s5 = 0
            Else
                s5 = CDec(TextBox5.Text)
            End If
            Dim s6 As Decimal
            If TextBox6.Text = "" Or TextBox6.Text Is Nothing Then
                s6 = 0
            Else
                s6 = CDec(TextBox6.Text)
            End If
            Dim s7 As Decimal
            If TextBox7.Text = "" Or TextBox7.Text Is Nothing Then
                s7 = 0
            Else
                s7 = CDec(TextBox7.Text)
            End If
            Dim s8 As Decimal
            If TextBox8.Text = "" Or TextBox8.Text Is Nothing Then
                s8 = 0
            Else
                s8 = CDec(TextBox8.Text)
            End If
            Dim s9 As Decimal
            If TextBox9.Text = "" Or TextBox9.Text Is Nothing Then
                s9 = 0
            Else
                s9 = CDec(TextBox9.Text)
            End If
            Dim s10 As Decimal
            If TextBox10.Text = "" Or TextBox10.Text Is Nothing Then
                s10 = 0
            Else
                s10 = CDec(TextBox10.Text)
            End If
            Dim s11 As Decimal
            If TextBox11.Text = "" Or TextBox11.Text Is Nothing Then
                s11 = 0
            Else
                s11 = CDec(TextBox11.Text)
            End If
            Dim comman As New MySqlCommand($"UPDATE `damanecash`.`caissecalcul` SET `Count` = @Count WHERE `datex` LIKE '%{DateAndTime.Now.ToString("yyyy-MM-dd")}%' AND `Type` = @Type;", connectiono)
            comman.Parameters.AddWithValue("@Type", "10000.00")
            comman.Parameters.AddWithValue("@Count", s1)
            comman.ExecuteNonQuery()
            Dim comman1 As New MySqlCommand($"UPDATE `damanecash`.`caissecalcul` SET `Count` = @Count WHERE `datex` LIKE '%{DateAndTime.Now.ToString("yyyy-MM-dd")}%' AND `Type` = @Type;", connectiono)
            comman1.Parameters.AddWithValue("@Type", "1000.00")
            comman1.Parameters.AddWithValue("@Count", s2)
            comman1.ExecuteNonQuery()
            Dim comman2 As New MySqlCommand($"UPDATE `damanecash`.`caissecalcul` SET `Count` = @Count WHERE `datex` LIKE '%{DateAndTime.Now.ToString("yyyy-MM-dd")}%' AND `Type` = @Type;", connectiono)
            comman2.Parameters.AddWithValue("@Type", "200.00")
            comman2.Parameters.AddWithValue("@Count", s11)
            comman2.ExecuteNonQuery()
            Dim comman3 As New MySqlCommand($"UPDATE `damanecash`.`caissecalcul` SET `Count` = @Count WHERE `datex` LIKE '%{DateAndTime.Now.ToString("yyyy-MM-dd")}%' AND `Type` = @Type;", connectiono)
            comman3.Parameters.AddWithValue("@Type", "100.00")
            comman3.Parameters.AddWithValue("@Count", s3)
            comman3.ExecuteNonQuery()
            Dim comman4 As New MySqlCommand($"UPDATE `damanecash`.`caissecalcul` SET `Count` = @Count WHERE `datex` LIKE '%{DateAndTime.Now.ToString("yyyy-MM-dd")}%' AND `Type` = @Type;", connectiono)
            comman4.Parameters.AddWithValue("@Type", "50.00")
            comman4.Parameters.AddWithValue("@Count", s4)
            comman4.ExecuteNonQuery()
            Dim comman5 As New MySqlCommand($"UPDATE `damanecash`.`caissecalcul` SET `Count` = @Count WHERE `datex` LIKE '%{DateAndTime.Now.ToString("yyyy-MM-dd")}%' AND `Type` = @Type;", connectiono)
            comman5.Parameters.AddWithValue("@Type", "20.00")
            comman5.Parameters.AddWithValue("@Count", s5)
            comman5.ExecuteNonQuery()
            Dim comman6 As New MySqlCommand($"UPDATE `damanecash`.`caissecalcul` SET `Count` = @Count WHERE `datex` LIKE '%{DateAndTime.Now.ToString("yyyy-MM-dd")}%' AND `Type` = @Type;", connectiono)
            comman6.Parameters.AddWithValue("@Type", "10.00")
            comman6.Parameters.AddWithValue("@Count", s6)
            comman6.ExecuteNonQuery()
            Dim comman7 As New MySqlCommand($"UPDATE `damanecash`.`caissecalcul` SET `Count` = @Count WHERE `datex` LIKE '%{DateAndTime.Now.ToString("yyyy-MM-dd")}%' AND `Type` = @Type;", connectiono)
            comman7.Parameters.AddWithValue("@Type", "5.00")
            comman7.Parameters.AddWithValue("@Count", s7)
            comman7.ExecuteNonQuery()
            Dim comman8 As New MySqlCommand($"UPDATE `damanecash`.`caissecalcul` SET `Count` = @Count WHERE `datex` LIKE '%{DateAndTime.Now.ToString("yyyy-MM-dd")}%' AND `Type` = @Type;", connectiono)
            comman8.Parameters.AddWithValue("@Type", "2.00")
            comman8.Parameters.AddWithValue("@Count", s8)
            comman8.ExecuteNonQuery()
            Dim comman9 As New MySqlCommand($"UPDATE `damanecash`.`caissecalcul` SET `Count` = @Count WHERE `datex` LIKE '%{DateAndTime.Now.ToString("yyyy-MM-dd")}%' AND `Type` = @Type;", connectiono)
            comman9.Parameters.AddWithValue("@Type", "1.00")
            comman9.Parameters.AddWithValue("@Count", s9)
            comman9.ExecuteNonQuery()
            Dim comman10 As New MySqlCommand($"UPDATE `damanecash`.`caissecalcul` SET `Count` = @Count WHERE `datex` LIKE '%{DateAndTime.Now.ToString("yyyy-MM-dd")}%' AND `Type` = @Type;", connectiono)
            comman10.Parameters.AddWithValue("@Type", "0.50")
            comman10.Parameters.AddWithValue("@Count", s10)
            comman10.ExecuteNonQuery()



            Dim command3 As New MySqlCommand($"UPDATE hisabat SET Solde={CDec(Label4.Text.Replace(" DH", ""))} WHERE ID = {11}", connectiona)
            command3.ExecuteNonQuery()
            Csld.Text = $"Caisse actuel: {utilities.getsld(11).ToString}"

            Me.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        connectiona.Close()
        connectiono.Close()
    End Sub

    Private Sub Guna2CircleButton3_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton3.Click
        'Try
        Dim msgr As MsgBoxResult = MsgBox($"Voulez-vous supprimer le client : {Guna2DataGridView1.SelectedRows(0).Cells(1).Value}", MsgBoxStyle.YesNo)
        If connectiono.State = ConnectionState.Open Then
            Else
                connectiono.Open()
            End If
            If connectiona.State = ConnectionState.Closed Then
                connectiona.Open()
            End If
            If msgr = MsgBoxResult.Yes Then
                Dim comman As New MySqlCommand("DELETE FROM nonpayedetail WHERE ID = @uid;", connectiono)
            comman.Parameters.AddWithValue("@uid", Guna2DataGridView1.SelectedRows(0).Cells(0).Value)
            saveTrafficCompte(10, Guna2DataGridView1.SelectedRows(0).Cells(2).Value, Guna2DataGridView1.SelectedRows(0).Cells(1).Value, -1)
            comman.ExecuteNonQuery()
                Guna2DataGridView1.Rows.RemoveAt(Guna2DataGridView1.SelectedRows(0).Index)
                filldataNonPayé()
                Try
                    Dim tlt As Decimal = 0
                    For i = 0 To Guna2DataGridView1.Rows.Count - 1
                        tlt += Guna2DataGridView1.Rows(i).Cells(2).Value
                    Next
                    Label6.Text = tlt.ToString
                Catch ex As Exception

                End Try
                Dim last As Decimal = utilities.getsld(10)
                Dim sld1 As Decimal = utilities.getsld(11)
                If (CDec(Label6.Text.Replace(" DH", "") - last)) < 0 Then
                    Dim command2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld1 - (CDec(Label6.Text.Replace(" DH", "") - last))} WHERE ID = {11}", connectiona)
                    command2.ExecuteNonQuery()
                End If
                Dim command4 As New MySqlCommand($"UPDATE hisabat SET Solde={CDec(Label6.Text.Replace(" DH", ""))} WHERE ID = {10}", connectiona)
                command4.ExecuteNonQuery()
                Label7.Text = $"Non payé actuel: {utilities.getsld(10).ToString}"
                Csld.Text = $"Caisse actuel: {utilities.getsld(11).ToString}"
            End If
            connectiono.Close()
            connectiona.Close()
        'Catch ex1 As Exception

        'End Try

    End Sub
    Sub saveTrafficCompte(compteid As Integer, mtn As Decimal, person As String, coif As Integer)
        Try
            Dim comman5 As New MySqlCommand($"INSERT INTO `damanecash`.`detail_comptes_traffic`
(`CompteID`,
`Person`,
`Mantant`,
`date`)
VALUES
(@CompteID,
@Person,
@Mantant,
@date);
", connectiono)

            comman5.Parameters.AddWithValue("@CompteID", compteid)
            comman5.Parameters.AddWithValue("@Person", person)
            comman5.Parameters.AddWithValue("@Mantant", CDec(mtn) * coif)
            comman5.Parameters.AddWithValue("@date", DateAndTime.Now)
            comman5.ExecuteNonQuery()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
    Private Sub Guna2CircleButton2_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton2.Click
        If connectiono.State = ConnectionState.Open Then
        Else
            connectiono.Open()
        End If
        If connectiona.State = ConnectionState.Closed Then
            connectiona.Open()
        End If
        For i = 0 To Guna2DataGridView1.Rows.Count - 2

            If IsDBNull(Guna2DataGridView1.Rows(i).Cells(0).Value) Or Guna2DataGridView1.Rows(i).Cells(0).Value Is Nothing Then
                Dim dt2 As DataTable = utilities.GetSomeValuesdmc("SELECT * from detail_comptes_traffic", $"'{Guna2DataGridView1.Rows(i).Cells(1).Value}' AND CompteID={10}", "Person")
                'If dt2.Rows.Count <> 0 Then
                '    MsgBox($"الاسم ({Guna2DataGridView1.Rows(i).Cells(1).Value}) مكرر مسبقا")
                '    Exit Sub
                'End If
                Dim comman As New MySqlCommand("INSERT INTO `nonpayedetail` (`NomEtPrenom`, `Mnt`, `Descr`) VALUES (@CompteName, @Solde, @datex);", connectiono)
                comman.Parameters.AddWithValue("@CompteName", Guna2DataGridView1.Rows(i).Cells(1).Value)
                comman.Parameters.AddWithValue("@Solde", Guna2DataGridView1.Rows(i).Cells(2).Value)
                comman.Parameters.AddWithValue("@datex", Guna2DataGridView1.Rows(i).Cells(3).Value)
                comman.ExecuteNonQuery()
                saveTrafficCompte(10, Guna2DataGridView1.Rows(i).Cells(2).Value, Guna2DataGridView1.Rows(i).Cells(1).Value, 1)
            Else
                'check is existe
                Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT * FROM nonpayedetail", Guna2DataGridView1.Rows(i).Cells(0).Value, "ID")
                If dt.Rows.Count = 1 Then
                    Try
                        If dt.Rows(0)(1) = Guna2DataGridView1.Rows(i).Cells(1).Value And dt.Rows(0)(2) = Guna2DataGridView1.Rows(i).Cells(2).Value And dt.Rows(0)(3) = Guna2DataGridView1.Rows(i).Cells(3).Value Then
                        Else
                            Dim id As String = Guna2DataGridView1.Rows(i).Cells(0).Value
                            Dim np As String = Guna2DataGridView1.Rows(i).Cells(1).Value
                            Dim mnt As Decimal = Guna2DataGridView1.Rows(i).Cells(2).Value
                            Dim desc As String = Guna2DataGridView1.Rows(i).Cells(3).Value
                            Dim mtnAnc As Decimal = utilities.GetSomeValuesdmc("SELECT Mnt from nonpayedetail", $"'{np}'", "NomEtPrenom").Rows(0)(0)
                            Dim mtn As Decimal = -mnt + mtnAnc
                            Dim command3 As New MySqlCommand($"UPDATE nonpayedetail SET NomEtPrenom='{np}', Mnt = {mnt}, Descr = '{desc}' WHERE ID = {id}", connectiono)
                            command3.ExecuteNonQuery()
                            saveTrafficCompte(10, mtn, np, -1)
                        End If
                    Catch ex As Exception
                        Dim id As String = Guna2DataGridView1.Rows(i).Cells(0).Value
                        Dim np As String
                        Dim desc As String
                        Dim mnt As Decimal

                        Try
                            np = Guna2DataGridView1.Rows(i).Cells(1).Value
                        Catch ex1 As Exception
                            np = ""
                        End Try
                        Try
                            mnt = Guna2DataGridView1.Rows(i).Cells(2).Value
                        Catch ex1 As Exception
                            mnt = 0
                        End Try
                        Try
                            desc = Guna2DataGridView1.Rows(i).Cells(3).Value
                        Catch ex1 As Exception
                            desc = ""
                        End Try
                        Dim mtnAnc As Decimal = utilities.GetSomeValuesdmc("SELECT Mnt from nonpayedetail", $"'{np}'", "NomEtPrenom").Rows(0)(0)
                        Dim mtn As Decimal = -mnt + mtnAnc
                        Dim command3 As New MySqlCommand($"UPDATE nonpayedetail SET NomEtPrenom='{np}', Mnt = {mnt}, Descr = '{desc}' WHERE ID = {id}", connectiono)
                        command3.ExecuteNonQuery()

                        saveTrafficCompte(10, mtn, np, -1)

                    End Try
                Else
                    MsgBox("Erreur")
                End If
            End If
        Next
        filldataNonPayé()
        Try
            Dim tlt As Decimal = 0
            For i = 0 To Guna2DataGridView1.Rows.Count - 1
                tlt += Guna2DataGridView1.Rows(i).Cells(2).Value
            Next
            Label6.Text = tlt.ToString
        Catch ex As Exception

        End Try
        Dim last As Decimal = utilities.getsld(10)
        Dim sld1 As Decimal = utilities.getsld(11)
        If (CDec(Label6.Text.Replace(" DH", "") - last)) < 0 Then
            Dim command2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld1 - (CDec(Label6.Text.Replace(" DH", "") - last))} WHERE ID = {11}", connectiona)
            command2.ExecuteNonQuery()
        End If
        Dim command4 As New MySqlCommand($"UPDATE hisabat SET Solde={CDec(Label6.Text.Replace(" DH", ""))} WHERE ID = {10}", connectiona)
        command4.ExecuteNonQuery()
        Label7.Text = $"Non payé actuel: {utilities.getsld(10).ToString}"
        Csld.Text = $"Caisse actuel: {utilities.getsld(11).ToString}"
        Panel1.Enabled = True
        Panel2.Enabled = False
        connectiono.Close()
        connectiona.Close()
    End Sub

    Private Sub Guna2DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellContentClick
        If e.ColumnIndex = 4 Then
            addOrMinMtnNonPaye.typ = "+"
            addOrMinMtnNonPaye.Label1.Text = $"Ajouter - {Guna2DataGridView1.SelectedRows(0).Cells(1).Value.ToString()}"
            addOrMinMtnNonPaye.ShowDialog()
        ElseIf e.ColumnIndex = 5 Then
            addOrMinMtnNonPaye.typ = "-"
            addOrMinMtnNonPaye.Label1.Text = $"Retrancher - {Guna2DataGridView1.SelectedRows(0).Cells(1).Value.ToString()}"
            addOrMinMtnNonPaye.ShowDialog()
        ElseIf e.ColumnIndex = 2 Then
            detaillNonPerson.person = Guna2DataGridView1.SelectedRows(0).Cells(1).Value.ToString()
            detaillNonPerson.compteID = 10
            detaillNonPerson.ShowDialog()
        End If

    End Sub

    Private Sub Guna2DataGridView1_SelectionChanged(sender As Object, e As EventArgs) Handles Guna2DataGridView1.SelectionChanged
        Try
            Dim tlt As Decimal = 0
            For i = 0 To Guna2DataGridView1.Rows.Count - 1
                tlt += Guna2DataGridView1.Rows(i).Cells(2).Value
            Next
            Label6.Text = tlt.ToString
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Guna2CircleButton4_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton4.Click
        Salaf.isSalaf = False
        Salaf.CompteID = 10
        Salaf.Show()

    End Sub

    Private Sub Guna2DataGridView1_RowsAdded(sender As Object, e As DataGridViewRowsAddedEventArgs) Handles Guna2DataGridView1.RowsAdded
        Dim img As Image = My.Resources.plus_small
        Guna2DataGridView1.Rows(e.RowIndex).Cells(4).Value = img
        Dim img1 As Image = My.Resources.minus_small
        Guna2DataGridView1.Rows(e.RowIndex).Cells(5).Value = img1
    End Sub
End Class