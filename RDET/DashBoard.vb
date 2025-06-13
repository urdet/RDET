Imports MySql.Data.MySqlClient
Public Class DashBoard
    Public Shared connectiona As MySqlConnection = Class1.conectionscr
    Public Shared connectiont As MySqlConnection = Class1.conectiontrk
    Public Shared connectiono As MySqlConnection = Class1.conectiondmc
    Private Sub edittrans_Click(sender As Object, e As EventArgs) Handles edittrans.Click, Guna2CircleButton6.Click
        Guna2ShadowPanel1.Visible = False
        FlowLayoutPanel1.Visible = True
    End Sub

    Private Sub Guna2CircleButton7_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton7.Click
        AutoOperations.UComptes1(Guna2DateTimePicker1.Value)
        TransManPanel.Visible = False
        Guna2ShadowPanel2.Visible = False
        FlowLayoutPanel1.Visible = True
        Dim ES As MsgBoxResult = MsgBox("Doyou want to delete user no: " & DGVTrans.SelectedRows(0).Cells(0).Value, MsgBoxStyle.Critical + MsgBoxStyle.YesNo)
        Try
            If ES = MsgBoxResult.Yes Then
                connectiona.Open()
                Dim sld1 As Decimal = utilities.getsld(DGVTrans.SelectedRows(0).Cells(1).Value)
                Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld1 + DGVTrans.SelectedRows(0).Cells(3).Value} WHERE ID = {DGVTrans.SelectedRows(0).Cells(1).Value}", connectiona)
                Dim sld2 As Decimal = utilities.getsld(DGVTrans.SelectedRows(0).Cells(2).Value)
                Dim command2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld2 - DGVTrans.SelectedRows(0).Cells(3).Value} WHERE ID = {DGVTrans.SelectedRows(0).Cells(2).Value}", connectiona)
                command1.ExecuteNonQuery()
                command2.ExecuteNonQuery()
                connectiona.Close()
                connectiono.Open()
                Dim comman As New MySqlCommand("DELETE FROM tansactionscomptes WHERE ID = @uid;", connectiono)
                comman.Parameters.AddWithValue("@uid", DGVTrans.SelectedRows(0).Cells(0).Value)
                comman.ExecuteNonQuery()
                connectiono.Close()

                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Delete", "Delete trans", $"Delete a trans.", "", DateTime.Now)
                filldat()
            Else
                Exit Sub
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        AutoOperations.UComptes1(Guna2DateTimePicker1.Value)
    End Sub

    Private Sub GoToTrabslactionsDetailsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GoToTrabslactionsDetailsToolStripMenuItem.Click
        Try
            If connectiont.State = ConnectionState.Closed Then
                connectiont.Open()
            End If
            Dim com As New MySqlCommand
            com.CommandType = CommandType.StoredProcedure
            com.CommandText = "GetTanslactionCompte"
            com.Connection = connectiono
            Dim adapter As New MySqlDataAdapter(com)
            Dim table As New DataTable()
            adapter.Fill(table)
            If table.Rows.Count > 0 Then
                GridControl1.DataSource = table

            Else
                MsgBox("no Data")
            End If
            FlowLayoutPanel1.Visible = False
            Guna2ShadowPanel1.Visible = True
        Catch ex As Exception
            MsgBox(ex.Message.ToString())
        Finally
            connectiont.Close()
        End Try
    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Me.Close()
    End Sub

    Public Sub filldat()
        If connectiona.State = ConnectionState.Closed Then
            connectiona.Open()
        Else

        End If

        Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT `tansactionscomptes`.`Mtn`, " &
                            "`tansactionscomptes`.`datex` " &
                            "FROM `damanecash`.`tansactionscomptes` ", 3, $"datex Like '%{DateTime.Now.ToString("yyyy-MM-dd")}%' And `tansactionscomptes`.`TO` = 4 And `tansactionscomptes`.`FROM`")
        If dt.Rows.Count > 0 Then
            Dim tlt As Decimal = 0
            For i = 0 To dt.Rows.Count - 1
                tlt += dt.Rows(i)(0)
            Next
            Dim comand1 As New MySqlCommand($"UPDATE hisabat SET Solde={tlt} WHERE ID = {15}", connectiona)
            comand1.ExecuteNonQuery()
        Else
            Dim comand1 As New MySqlCommand($"UPDATE hisabat SET Solde={0} WHERE ID = {15}", connectiona)
            comand1.ExecuteNonQuery()
        End If

        Dim command As New MySqlCommand($"SELECT * FROM hisabat", connectiona)

        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()

        adapter.Fill(table)
        Dim command1 As MySqlCommand
        If Form1.valdi <> "Admin" Then
            command1 = New MySqlCommand($"SELECT * FROM hisabat where visiblity =1", connectiona)
        Else
            command1 = New MySqlCommand($"SELECT * FROM hisabat", connectiona)
        End If

        Dim adapter1 As New MySqlDataAdapter(command1)
        Dim table1 As New DataTable()

        adapter1.Fill(table1)
        FlowLayoutPanel1.Controls.Clear()

        For ligne As Integer = 0 To table1.Rows.Count - 1
            Dim colonne As Integer = 1
            Dim nomBouton As String = table1.Rows(ligne)(colonne).ToString
            Dim bouton As New ComptesBox()
            bouton.Name = table1.Rows(ligne)(0).ToString
            bouton.CN.Text = table1.Rows(ligne)(1).ToString
            If table1.Rows(ligne)(0) = 1 Then
                'MsgBox(DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"))
                Try
                    Dim tdt As DataTable = utilities.GetSomeValuesdmc("SELECT Solde FROM damanecash.compteshistoriquedata ", 1, $" (datex LIKE '%{DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd")}%') AND CompteID ")
                    bouton.Label1.Text = "Anciene solde: " & tdt.Rows(0)(0)
                Catch ex As Exception

                End Try
                bouton.Guna2CircleButton1.Visible = True
                AddHandler bouton.Guna2CircleButton1.Click, Sub(s, args) VirmentDpÀBankToolStripMenuItem_Click()
            End If

            If table1.Rows(ligne)(0) = 4 Then
                bouton.Guna2CircleButton1.Visible = True
                AddHandler bouton.Guna2CircleButton1.Click, Sub(s, args) VirmentDeCoffreÀCaisseToolStripMenuItem_Click()
            End If
            If table1.Rows(ligne)(0) = 2 Then
                Try
                    Dim tdt As DataTable = utilities.GetSomeValuesdmc("SELECT Solde FROM damanecash.compteshistoriquedata ", 2, $" (datex LIKE '%{DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd")}%') AND CompteID ")
                    bouton.Label1.Text = "Anciene solde: " & tdt.Rows(0)(0)
                Catch ex As Exception

                End Try
                bouton.Guna2CircleButton1.Visible = True
                AddHandler bouton.Guna2CircleButton1.Click, Sub(s, args) VirmentDeFunÀDpToolStripMenuItem_Click()
            End If
            If table1.Rows(ligne)(0) = 8 Then
                bouton.Guna2CircleButton1.Visible = True
                AddHandler bouton.Guna2CircleButton1.Click, Sub(s, args) MontantVirment()
            End If

            If table1.Rows(ligne)(0) = 10 Then
                bouton.Guna2CircleButton1.Visible = True
                AddHandler bouton.Guna2CircleButton1.Click, Sub(s, args) openFrm(CaiseCalcule)
            End If
            If table1.Rows(ligne)(0) = 12 Then
                bouton.Guna2CircleButton1.Visible = True
                AddHandler bouton.Guna2CircleButton1.Click, Sub(s, args) VirmentSalafToolStripMenuItem_Click(12)
            End If
            If table1.Rows(ligne)(0) = 9 Then

                bouton.Label1.Text = $"Balance : {utilities.CalculeBalance().ToString}"
                ToolTipController1.SetTitle(bouton.Label1, "Calcul")
                ToolTipController1.SetToolTip(bouton.Label1, "(Fundex + Damane Pay + Bank + Coffre + Caisse Calculé) - Capital")

            End If
            If table1.Rows(ligne)(0) = 7 Then
                bouton.Guna2CircleButton1.Visible = True
                bouton.Label1.Text = $"Detail"
                AddHandler bouton.Guna2CircleButton1.Click, Sub(s, args) VirmentSalafToolStripMenuItem_Click(7)
                AddHandler bouton.Label1.Click, Sub(s, args) Opensalaf()
            End If
            If table1.Rows(ligne)(0) = 14 Then
                bouton.Guna2CircleButton1.Visible = True
                AddHandler bouton.Guna2CircleButton1.Click, Sub(s, args) VirmentSalafToolStripMenuItem_Click(14)
            End If
            If table1.Rows(ligne)(0) = 11 Then
                Dim sld1 As Decimal = utilities.getsld(5)
                Dim np As Decimal = utilities.getsld(10)
                Dim sld2 As Decimal = utilities.getsld(11)

                bouton.Label1.Text = $"Balance : {sld2 + np - sld1}"
                ToolTipController1.SetTitle(bouton.Label1, "Calcul")
                ToolTipController1.SetToolTip(bouton.Label1, "(Caisse Reél + Facture Non Payé) - Caisse Calculé")
                AddHandler bouton.Label1.Click, Sub(s, args) CaiseAremise()
                bouton.Guna2CircleButton1.Visible = True
                AddHandler bouton.Guna2CircleButton1.Click, Sub(s, args) openFrm(CaiseCalcule)
            End If
            If table1.Rows(ligne)(0) = 3 Then
                Dim sld1 As Decimal = utilities.getsld(12)
                Dim sld2 As Decimal = utilities.getsld(3)
                bouton.Label1.Text = $"Non verssé réel: {sld1 - sld2}"
                bouton.Label1.ForeColor = Color.DarkRed
                AddHandler bouton.Label1.Click, Sub(s, args) openFrm(CheckLesVersement)

                bouton.Guna2CircleButton1.Visible = True
                AddHandler bouton.Guna2CircleButton1.Click, Sub(s, args) VermentDeBankÀCoffreToolStripMenuItem_Click()
            End If
            bouton.Sld.Text = table1.Rows(ligne)(2).ToString

            Dim cld As String = table1.Rows(ligne)(5).ToString()
            Dim compNam As String = ""
            '' Get IDs
            If cld.Contains(",") Then
                Dim str1 As String = cld.Replace("[", "")
                Dim str2 As String = str1.Replace("]", "")
                Dim parts() As String = str2.Split(",")
                Dim inte As Integer = parts.Length

                For i = 0 To inte - 1
                    If i = inte - 1 Then
                        Dim nm As DataTable = utilities.GetSomeValuesscr("SELECT CompanyName FROM companies", CInt(parts(i)), "ID")
                        compNam = compNam & (nm.Rows(0)(0).ToString)
                    Else
                        Dim nm1 As DataTable = utilities.GetSomeValuesscr("SELECT CompanyName FROM companies", CInt(parts(i)), "ID")
                        compNam = compNam & (nm1.Rows(0)(0).ToString & " ,")
                    End If
                Next
            Else
                Try
                    Dim str1 As String = cld.Replace("[", "")
                    Dim str2 As String = str1.Replace("]", "")

                    Dim nm1 As DataTable = utilities.GetSomeValuesscr("SELECT CompanyName FROM companies", CInt(str2), "ID")
                    compNam = compNam & (nm1.Rows(0)(0).ToString)
                Catch ex As Exception

                End Try

            End If
            bouton.Comp.Text = compNam
            bouton.Text = nomBouton
            'AddHandler bouton.Click, Sub(s, args) BoutonClique(nomBouton)
            FlowLayoutPanel1.Controls.Add(bouton)
        Next

        Guna2ComboBox1.DataSource = table
        Guna2ComboBox1.DisplayMember = "CompteName"
        Guna2ComboBox1.ValueMember = "ID"
        CbCompte.DataSource = table
        CbCompte.DisplayMember = "CompteName"
        CbCompte.ValueMember = "ID"
        Guna2ComboBox2.DataSource = table1
        Guna2ComboBox2.DisplayMember = "CompteName"
        Guna2ComboBox2.ValueMember = "ID"


        connectiona.Close()
    End Sub
    Public Sub openFrm(f As Form)
        f.Show()
        f.BringToFront()
    End Sub
    Private Sub DashBoard_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        filldat()
        AutoOperations.UComptes1(DateTime.Now)
        If Form1.valdi = "Admin" Then
        Else

            NewVersementToolStripMenuItem.Visible = False
            SumprimerTransToolStripMenuItem.Visible = False
            NewTranslactionToolStripMenuItem.Visible = False

        End If
        ' Add the MouseDown event handler for all controls in the FlowLayoutPanel
        For Each ctrl As Control In FlowLayoutPanel1.Controls
            AddHandler ctrl.MouseDown, AddressOf Button_MouseDown
            AddHandler ctrl.MouseMove, AddressOf Button_MouseMove
            AddHandler ctrl.MouseUp, AddressOf Button_MouseUp
            EnableDoubleBuffering(ctrl)
        Next
    End Sub
    Public Sub Opensalaf()
        Salaf.Show()
        Salaf.isSalaf = True
    End Sub
    Private Sub Guna2GradientButton7_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        GridControl1.ShowRibbonPrintPreview()
        TrackClass.AddUserTLine(Form1.idd, Form1.first, "Print", "Print table of comptes translaction", $"", "", DateTime.Now)

    End Sub

    Private Sub Guna2CircleButton5_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton5.Click
        If connectiona.State = ConnectionState.Closed Then
            connectiona.Open()
        End If
        If connectiono.State = ConnectionState.Closed Then
            connectiono.Open()
        End If

        AutoOperations.UComptes1(Guna2DateTimePicker1.Value)
        If Guna2ComboBox1.Text <> "" And Guna2ComboBox2.Text <> "" And Guna2TextBox4.Text <> "" Then
            Dim a As Boolean = utilities.checksld(Guna2ComboBox2.SelectedValue, CDec(Guna2TextBox4.Text))
            If a Then

                'MsgBox("1")
                '' Add Statements
                ' get soldes
                Dim sld1 As Decimal = utilities.getsld(Guna2ComboBox2.SelectedValue)
                Dim sld2 As Decimal = utilities.getsld(Guna2ComboBox1.SelectedValue)

                Dim mtn As Decimal = CDec(Guna2TextBox4.Text)
                'add
                Dim comman As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse); ", connectiono)
                comman.Parameters.AddWithValue("@FROM", Guna2ComboBox2.SelectedValue)
                comman.Parameters.AddWithValue("@TO", Guna2ComboBox1.SelectedValue)
                comman.Parameters.AddWithValue("@Mtn", mtn)
                comman.Parameters.AddWithValue("@Desc", Nothing)
                comman.Parameters.AddWithValue("@datex", DateAndTime.Now)
                comman.Parameters.AddWithValue("@verse", 0)
                comman.ExecuteNonQuery()

                Dim lastIdQuery As String = "SELECT MAX(id) FROM `tansactionscomptes`;"
                Dim lastInsertedId As Integer
                Using command As New MySqlCommand(lastIdQuery, connectiono)
                    lastInsertedId = Convert.ToInt32(command.ExecuteScalar())
                End Using
                'update copm
                Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld1 - mtn} WHERE ID = {Guna2ComboBox2.SelectedValue}", connectiona)
                Dim command2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld2 + mtn} WHERE ID = {Guna2ComboBox1.SelectedValue}", connectiona)
                command1.ExecuteNonQuery()
                command2.ExecuteNonQuery()

                TransManPanel.Visible = False
                FlowLayoutPanel1.Visible = True
                TrackClass.AddCompteTLine(Form1.idd, Form1.first, Guna2ComboBox2.SelectedValue, Guna2ComboBox1.SelectedValue, lastInsertedId, mtn, "Add a new translaction.", DateTime.Now)
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Add", "Add translaction", $"Add a new Translaction to (tansactionscomptes) table.", "", DateTime.Now)
                filldat()
                Guna2TextBox4.Text = ""
                Guna2CircleButton10.PerformClick()
            Else
                MsgBox("The mtn > sld")
            End If
        End If
        'Panel1.Visible = True

        connectiona.Close()
        connectiono.Close()
        AutoOperations.UComptes1(Guna2DateTimePicker1.Value)
    End Sub



    Private Sub NewVersementToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewVersementToolStripMenuItem.Click
        Guna2ShadowPanel2.Visible = True
        FlowLayoutPanel1.Visible = False
        CbCompte.Enabled = True
        VRdtp.Value = DateTime.Now
        Guna2ShadowPanel1.Visible = False
        TransManPanel.Visible = False
    End Sub

    Private Sub Guna2CircleButton3_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton3.Click
        AutoOperations.UComptes1(VRdtp.Value)
        'Check validity
        If connectiona.State = ConnectionState.Closed Then
            connectiona.Open()
        End If
        If connectiono.State = ConnectionState.Closed Then
            connectiono.Open()
        End If
        Try
            Dim sld As Decimal = utilities.getsld(CbCompte.SelectedValue)
            If utilities.checksld(CbCompte.SelectedValue, CDec(VRmtn.Text.Replace(",", "."))) = False And Guna2ToggleSwitch1.Checked = False Then
                MsgBox("mantant > solde")
                Exit Sub
            ElseIf Guna2ToggleSwitch1.Checked = False And Guna2ToggleSwitch1.Visible = True And utilities.checksld(CbCompte.SelectedValue, CDec(VRmtn.Text.Replace(",", "."))) = True Then
                If CbCompte.SelectedValue = 7 Then

                    If ComboBox1.Text <> "" Then

                    Else
                        MsgBox("يجب ادراج اسم الممول")
                        Exit Sub
                    End If
                    'reg
                    Dim comman5 As New MySqlCommand($"INSERT INTO `damanecash`.`salafdetail`
                    (`Invester`,
                    `typ`,
                    `Mtn`,
                    `Date`)
                    VALUES
                    (@Invester,
                    @typ,
                    @Mtn,
                    @Date);", connectiono)

                    comman5.Parameters.AddWithValue("@Invester", ComboBox1.Text)
                    Dim typ As String = ""
                    If Guna2ToggleSwitch1.Checked = True Then
                        typ = "+"
                    Else
                        typ = "- "
                    End If
                    comman5.Parameters.AddWithValue("@typ", typ)
                    comman5.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman5.Parameters.AddWithValue("@Date", DateAndTime.Now)
                    comman5.ExecuteNonQuery()
                    '-7 -4


                    Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld - CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {CbCompte.SelectedValue}", connectiona)
                    Dim sld1 As Decimal = utilities.getsld(4)
                    Dim command2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld1 - CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {4}", connectiona)
                    Dim sld2 As Decimal = utilities.getsld(9)
                    Dim command3 As New MySqlCommand($"UPDATE hisabat SET Solde={sld2 - CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {9}", connectiona)
                    command3.ExecuteNonQuery()
                    command1.ExecuteNonQuery()
                    command2.ExecuteNonQuery()

                    '' Add to historique
                    Dim comman As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman.Parameters.AddWithValue("@FROM", CbCompte.SelectedValue)
                    comman.Parameters.AddWithValue("@TO", Nothing)
                    comman.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman.Parameters.AddWithValue("@Desc", Guna2TextBox1.Text)
                    comman.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman.Parameters.AddWithValue("@verse", 0)
                    comman.ExecuteNonQuery()

                    Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid As Integer = dt.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, CbCompte.SelectedValue, Nothing, lastinsertedid, CDec(VRmtn.Text.Replace(",", ".")), ComboBox1.Text, DateTime.Now)
                    '' Add to historique
                    Dim comman1 As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman1.Parameters.AddWithValue("@FROM", 4)
                    comman1.Parameters.AddWithValue("@TO", Nothing)
                    comman1.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman1.Parameters.AddWithValue("@Desc", Guna2TextBox1.Text)
                    comman1.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman1.Parameters.AddWithValue("@verse", 0)
                    comman1.ExecuteNonQuery()

                    Dim dt1 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid1 As Integer = dt1.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, 4, Nothing, lastinsertedid1, CDec(VRmtn.Text.Replace(",", ".")), ComboBox1.Text, DateTime.Now)
                    '' Add to historique
                    Dim comman2 As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman2.Parameters.AddWithValue("@FROM", 9)
                    comman2.Parameters.AddWithValue("@TO", Nothing)
                    comman2.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman2.Parameters.AddWithValue("@Desc", Guna2TextBox1.Text)
                    comman2.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman2.Parameters.AddWithValue("@verse", 0)
                    comman2.ExecuteNonQuery()

                    Dim dt2 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid2 As Integer = dt2.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, 9, Nothing, lastinsertedid2, CDec(VRmtn.Text.Replace(",", ".")), ComboBox1.Text, DateTime.Now)


                    Guna2CircleButton2.PerformClick()

                ElseIf CbCompte.SelectedValue = 10 Then

                    Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld - CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {CbCompte.SelectedValue}", connectiona)
                    Dim sld1 As Decimal = utilities.getsld(11)
                    Dim command2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld1 + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {11}", connectiona)
                    command1.ExecuteNonQuery()
                    command2.ExecuteNonQuery()

                    '' Add to historique
                    Dim comman As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman.Parameters.AddWithValue("@FROM", CbCompte.SelectedValue)
                    comman.Parameters.AddWithValue("@TO", 11)
                    comman.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman.Parameters.AddWithValue("@Desc", Nothing)
                    comman.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman.Parameters.AddWithValue("@verse", 0)
                    comman.ExecuteNonQuery()

                    Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid As Integer = dt.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, CbCompte.SelectedValue, 11, lastinsertedid, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)


                ElseIf CbCompte.SelectedValue = 8 Then
                    Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld - CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {CbCompte.SelectedValue}", connectiona)
                    Dim sld1 As Decimal = utilities.getsld(9)
                    Dim command2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld1 - CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {9}", connectiona)
                    command1.ExecuteNonQuery()
                    command2.ExecuteNonQuery()
                    Dim sld2 As Decimal = utilities.getsld(4)
                    Dim comman2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld2 - CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {4}", connectiona)
                    comman2.ExecuteNonQuery()

                    '' Add to historique
                    Dim comman As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman.Parameters.AddWithValue("@FROM", CbCompte.SelectedValue)
                    comman.Parameters.AddWithValue("@TO", Nothing)
                    comman.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman.Parameters.AddWithValue("@Desc", Nothing)
                    comman.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman.Parameters.AddWithValue("@verse", 0)
                    comman.ExecuteNonQuery()

                    Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid As Integer = dt.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, CbCompte.SelectedValue, Nothing, lastinsertedid, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)
                    '' Add to historique
                    Dim comman1 As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman1.Parameters.AddWithValue("@FROM", 9)
                    comman1.Parameters.AddWithValue("@TO", Nothing)
                    comman1.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman1.Parameters.AddWithValue("@Desc", Nothing)
                    comman1.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman1.Parameters.AddWithValue("@verse", 0)
                    comman1.ExecuteNonQuery()

                    Dim dt1 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid1 As Integer = dt1.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, 9, Nothing, lastinsertedid1, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)
                    '' Add to historique
                    Dim comman12 As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman12.Parameters.AddWithValue("@FROM", 4)
                    comman12.Parameters.AddWithValue("@TO", Nothing)
                    comman12.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman12.Parameters.AddWithValue("@Desc", Nothing)
                    comman12.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman12.Parameters.AddWithValue("@verse", 0)
                    comman12.ExecuteNonQuery()

                    Dim dt12 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid12 As Integer = dt12.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, 4, Nothing, lastinsertedid12, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)



                ElseIf CbCompte.SelectedValue = 14 Then
                    Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld - CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {CbCompte.SelectedValue}", connectiona)
                    Dim sld1 As Decimal = utilities.getsld(3)
                    Dim command2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld1 - CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {3}", connectiona)
                    Dim sld2 As Decimal = utilities.getsld(9)
                    Dim command3 As New MySqlCommand($"UPDATE hisabat SET Solde={sld2 - CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {9}", connectiona)
                    command1.ExecuteNonQuery()
                    command3.ExecuteNonQuery()
                    command2.ExecuteNonQuery()

                    '' Add to historique
                    Dim comman As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman.Parameters.AddWithValue("@FROM", 3)
                    comman.Parameters.AddWithValue("@TO", Nothing)
                    comman.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman.Parameters.AddWithValue("@Desc", Nothing)
                    comman.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman.Parameters.AddWithValue("@verse", 0)
                    comman.ExecuteNonQuery()

                    Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid As Integer = dt.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, 3, Nothing, lastinsertedid, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)
                    '' Add to historique
                    Dim comman11 As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman11.Parameters.AddWithValue("@FROM", 9)
                    comman11.Parameters.AddWithValue("@TO", Nothing)
                    comman11.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman11.Parameters.AddWithValue("@Desc", Nothing)
                    comman11.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman11.Parameters.AddWithValue("@verse", 0)
                    comman11.ExecuteNonQuery()

                    Dim dt11 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid11 As Integer = dt.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, 3, Nothing, lastinsertedid11, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)


                    '' Add to historique
                    Dim comman1 As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman1.Parameters.AddWithValue("@FROM", CbCompte.SelectedValue)
                    comman1.Parameters.AddWithValue("@TO", Nothing)
                    comman1.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman1.Parameters.AddWithValue("@Desc", Nothing)
                    comman1.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman1.Parameters.AddWithValue("@verse", 0)
                    comman1.ExecuteNonQuery()

                    Dim dt1 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid1 As Integer = dt1.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, CbCompte.SelectedValue, Nothing, lastinsertedid1, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)



                Else

                    '-7
                    Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld - CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {CbCompte.SelectedValue}", connectiona)
                    command1.ExecuteNonQuery()
                    Dim comman As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman.Parameters.AddWithValue("@FROM", CbCompte.SelectedValue)
                    comman.Parameters.AddWithValue("@TO", Nothing)
                    comman.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman.Parameters.AddWithValue("@Desc", Nothing)
                    comman.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman.Parameters.AddWithValue("@verse", 0)
                    comman.ExecuteNonQuery()
                    Dim dt1 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid As Integer = dt1.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, CbCompte.SelectedValue, Nothing, lastinsertedid, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)


                End If
            ElseIf Guna2ToggleSwitch1.Checked = True And Guna2ToggleSwitch1.Visible = True Then
                If CbCompte.SelectedValue = 7 Then
                    If ComboBox1.Text <> "" Then

                    Else
                        MsgBox("يجب ادراج اسم الممول")
                        Exit Sub
                    End If
                    Dim comman5 As New MySqlCommand($"INSERT INTO `damanecash`.`salafdetail`
                    (`Invester`,
                    `typ`,
                    `Mtn`,
                    `Date`)
                    VALUES
                    (@Invester,
                    @typ,
                    @Mtn,
                    @Date);", connectiono)

                    comman5.Parameters.AddWithValue("@Invester", ComboBox1.Text)
                    Dim typ As String = ""
                    If Guna2ToggleSwitch1.Checked = True Then
                        typ = "+"
                    Else
                        typ = "- "
                    End If
                    comman5.Parameters.AddWithValue("@typ", typ)
                    comman5.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman5.Parameters.AddWithValue("@Date", DateAndTime.Now)
                    comman5.ExecuteNonQuery()
                    '+7 +4
                    Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {CbCompte.SelectedValue}", connectiona)
                    Dim sld1 As Decimal = utilities.getsld(4)
                    Dim command2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld1 + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {4}", connectiona)
                    Dim sld2 As Decimal = utilities.getsld(9)
                    Dim command3 As New MySqlCommand($"UPDATE hisabat SET Solde={sld2 + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {9}", connectiona)
                    command3.ExecuteNonQuery()
                    command1.ExecuteNonQuery()
                    command2.ExecuteNonQuery()
                    '' Add to historique
                    Dim comman As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman.Parameters.AddWithValue("@FROM", Nothing)
                    comman.Parameters.AddWithValue("@TO", CbCompte.SelectedValue)
                    comman.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman.Parameters.AddWithValue("@Desc", Nothing)
                    comman.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman.Parameters.AddWithValue("@verse", 0)
                    comman.ExecuteNonQuery()

                    Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid As Integer = dt.Rows(0)(0)

                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, Nothing, CbCompte.SelectedValue, lastinsertedid, CDec(VRmtn.Text.Replace(",", ".")), ComboBox1.Text, DateTime.Now)
                    '' Add to historique
                    Dim comman1 As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman1.Parameters.AddWithValue("@FROM", Nothing)
                    comman1.Parameters.AddWithValue("@TO", 4)
                    comman1.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman1.Parameters.AddWithValue("@Desc", Nothing)
                    comman1.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman1.Parameters.AddWithValue("@verse", 0)
                    comman1.ExecuteNonQuery()

                    Dim dt1 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid1 As Integer = dt1.Rows(0)(0)

                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, Nothing, 4, lastinsertedid1, CDec(VRmtn.Text.Replace(",", ".")), ComboBox1.Text, DateTime.Now)
                    '' Add to historique
                    Dim comman2 As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman2.Parameters.AddWithValue("@FROM", Nothing)
                    comman2.Parameters.AddWithValue("@TO", 9)
                    comman2.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman2.Parameters.AddWithValue("@Desc", Nothing)
                    comman2.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman2.Parameters.AddWithValue("@verse", 0)
                    comman2.ExecuteNonQuery()

                    Dim dt2 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid2 As Integer = dt2.Rows(0)(0)

                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, Nothing, 9, lastinsertedid2, CDec(VRmtn.Text.Replace(",", ".")), ComboBox1.Text, DateTime.Now)


                    Guna2CircleButton2.PerformClick()
                ElseIf CbCompte.SelectedValue = 14 Then
                    Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {CbCompte.SelectedValue}", connectiona)
                    Dim sld1 As Decimal = utilities.getsld(3)
                    Dim command2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld1 + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {3}", connectiona)
                    Dim sld2 As Decimal = utilities.getsld(9)
                    Dim command3 As New MySqlCommand($"UPDATE hisabat SET Solde={sld2 + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {9}", connectiona)
                    command1.ExecuteNonQuery()
                    command2.ExecuteNonQuery()
                    command3.ExecuteNonQuery()
                    '' Add to historique
                    Dim comman As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman.Parameters.AddWithValue("@FROM", Nothing)
                    comman.Parameters.AddWithValue("@TO", 3)
                    comman.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman.Parameters.AddWithValue("@Desc", Nothing)
                    comman.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman.Parameters.AddWithValue("@verse", 0)
                    comman.ExecuteNonQuery()

                    Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid As Integer = dt.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, Nothing, 3, lastinsertedid, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)
                    Dim comman11 As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman11.Parameters.AddWithValue("@FROM", Nothing)
                    comman11.Parameters.AddWithValue("@TO", 9)
                    comman11.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman11.Parameters.AddWithValue("@Desc", Nothing)
                    comman11.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman11.Parameters.AddWithValue("@verse", 0)
                    comman11.ExecuteNonQuery()

                    Dim dt11 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid11 As Integer = dt.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, Nothing, 3, lastinsertedid11, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)


                    '' Add to historique
                    Dim comman1 As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman1.Parameters.AddWithValue("@FROM", Nothing)
                    comman1.Parameters.AddWithValue("@TO", CbCompte.SelectedValue)
                    comman1.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman1.Parameters.AddWithValue("@Desc", Nothing)
                    comman1.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman1.Parameters.AddWithValue("@verse", 0)
                    comman1.ExecuteNonQuery()

                    Dim dt1 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid1 As Integer = dt1.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, Nothing, CbCompte.SelectedValue, lastinsertedid1, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)



                ElseIf CbCompte.SelectedValue = 10 Then
                    Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {CbCompte.SelectedValue}", connectiona)
                    Dim sld1 As Decimal = utilities.getsld(11)
                    Dim command2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld1 - CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {11}", connectiona)
                    command1.ExecuteNonQuery()
                    command2.ExecuteNonQuery()

                    '' Add to historique
                    Dim comman As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman.Parameters.AddWithValue("@FROM", 11)
                    comman.Parameters.AddWithValue("@TO", CbCompte.SelectedValue)
                    comman.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman.Parameters.AddWithValue("@Desc", Nothing)
                    comman.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman.Parameters.AddWithValue("@verse", 0)
                    comman.ExecuteNonQuery()

                    Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid As Integer = dt.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, 11, CbCompte.SelectedValue, lastinsertedid, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)



                ElseIf CbCompte.SelectedValue = 8 Then
                    Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {CbCompte.SelectedValue}", connectiona)
                    Dim sld1 As Decimal = utilities.getsld(9)
                    Dim command2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld1 + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {9}", connectiona)
                    command1.ExecuteNonQuery()
                    command2.ExecuteNonQuery()
                    Dim sld2 As Decimal = utilities.getsld(4)
                    Dim comman2 As New MySqlCommand($"UPDATE hisabat SET Solde={sld2 + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {4}", connectiona)
                    comman2.ExecuteNonQuery()


                    '' Add to historique
                    Dim comman As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman.Parameters.AddWithValue("@FROM", Nothing)
                    comman.Parameters.AddWithValue("@TO", CbCompte.SelectedValue)
                    comman.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman.Parameters.AddWithValue("@Desc", Nothing)
                    comman.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman.Parameters.AddWithValue("@verse", 0)
                    comman.ExecuteNonQuery()

                    Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid As Integer = dt.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, Nothing, CbCompte.SelectedValue, lastinsertedid, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)
                    '' Add to historique
                    Dim comman1 As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman1.Parameters.AddWithValue("@FROM", Nothing)
                    comman1.Parameters.AddWithValue("@TO", 9)
                    comman1.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman1.Parameters.AddWithValue("@Desc", Nothing)
                    comman1.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman1.Parameters.AddWithValue("@verse", 0)
                    comman1.ExecuteNonQuery()

                    Dim dt1 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid1 As Integer = dt1.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, Nothing, 9, lastinsertedid1, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)
                    '' Add to historique
                    Dim comman12 As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman12.Parameters.AddWithValue("@FROM", Nothing)
                    comman12.Parameters.AddWithValue("@TO", 4)
                    comman12.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman12.Parameters.AddWithValue("@Desc", Nothing)
                    comman12.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman12.Parameters.AddWithValue("@verse", 0)
                    comman12.ExecuteNonQuery()

                    Dim dt12 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid12 As Integer = dt12.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, Nothing, 4, lastinsertedid12, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)


                ElseIf CbCompte.SelectedValue = 4 Then
                    '-7
                    Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {CbCompte.SelectedValue}", connectiona)
                    command1.ExecuteNonQuery()
                    Dim comman As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman.Parameters.AddWithValue("@FROM", Nothing)
                    comman.Parameters.AddWithValue("@TO", CbCompte.SelectedValue)
                    comman.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman.Parameters.AddWithValue("@Desc", Guna2TextBox1.Text)
                    comman.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman.Parameters.AddWithValue("@verse", 0)
                    comman.ExecuteNonQuery()
                    Dim dt1 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid As Integer = dt1.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, Nothing, CbCompte.SelectedValue, lastinsertedid, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)


                Else
                    '+7
                    Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={sld + CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {CbCompte.SelectedValue}", connectiona)
                    command1.ExecuteNonQuery()
                    Dim comman As New MySqlCommand("INSERT INTO `tansactionscomptes` (`FROM`, `TO`, `Mtn`, `Desc`, `datex`, `versé`) VALUES (@FROM, @TO, @Mtn, @Desc, @datex, @verse);", connectiono)
                    comman.Parameters.AddWithValue("@FROM", Nothing)
                    comman.Parameters.AddWithValue("@TO", CbCompte.SelectedValue)
                    comman.Parameters.AddWithValue("@Mtn", CDec(VRmtn.Text.Replace(",", ".")))
                    comman.Parameters.AddWithValue("@Desc", Nothing)
                    comman.Parameters.AddWithValue("@datex", VRdtp.Value)
                    comman.Parameters.AddWithValue("@verse", 0)
                    comman.ExecuteNonQuery()
                    Dim dt1 As DataTable = utilities.GetSomeValuesdmc("SELECT MAX(ID) FROM `tansactionscomptes`", 1, "1")
                    Dim lastinsertedid As Integer = dt1.Rows(0)(0)
                    TrackClass.AddCompteTLine(Form1.idd, Form1.first, Nothing, CbCompte.SelectedValue, lastinsertedid, CDec(VRmtn.Text.Replace(",", ".")), "Add a new versement/retrais.", DateTime.Now)


                End If
            ElseIf Guna2ToggleSwitch1.Visible = False Then

                If CbCompte.SelectedValue = 12 Then
                    Dim command1 As New MySqlCommand($"UPDATE hisabat SET Solde={CDec(VRmtn.Text.Replace(",", "."))} WHERE ID = {CbCompte.SelectedValue}", connectiona)
                    command1.ExecuteNonQuery()


                End If
            End If
            Dim person As String
            Dim coificient As Integer = 1
            If Guna2ToggleSwitch1.Visible = False Then
                GoTo lll
            End If
            If Guna2ToggleSwitch1.Checked = True Then

            ElseIf Guna2ToggleSwitch1.Checked = False Then
                coificient = -1
            End If
            If ComboBox1.Text = "" Or IsDBNull(ComboBox1.Text) = True Then
                person = "anonymos"

            Else
                person = ComboBox1.Text
            End If

            saveTrafficCompte(CbCompte.SelectedValue, person, coificient)
lll:
            filldat()
            VRmtn.Text = ""
            Guna2CircleButton2.PerformClick()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        connectiona.Close()
        connectiono.Close()
        AutoOperations.UComptes1(VRdtp.Value)
    End Sub
    Sub saveTrafficCompte(compteid As Integer, person As String, coif As Integer)
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
            comman5.Parameters.AddWithValue("@Mantant", CDec(VRmtn.Text.Replace(",", ".")) * coif)
            comman5.Parameters.AddWithValue("@date", DateAndTime.Now)
            comman5.ExecuteNonQuery()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
    Private Sub CbCompte_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CbCompte.SelectedIndexChanged
        Try
            Dim sld As Decimal = utilities.getsld(CbCompte.SelectedValue)
            Label6.Text = $"Current solde is: {sld}"
            If CbCompte.SelectedValue = 4 Then
                Guna2TextBox1.Visible = True
            Else
                Guna2TextBox1.Visible = False
            End If
            If CbCompte.SelectedValue = 7 Then
                'Guna2TextBox1.Visible = True
                'Guna2TextBox1.Text = ""
                ComboBox1.Visible = True
                Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT DISTINCT Invester from salafdetail", "1", "1")
                ComboBox1.Items.Clear()

                For i = 0 To dt.Rows.Count - 1
                    ComboBox1.Items.Add(dt.Rows(i)(0))
                Next
                ComboBox1.Text = ""
            Else
                Guna2TextBox1.Text = ""
                Guna2TextBox1.Visible = False
                ComboBox1.Visible = True
                Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT DISTINCT Person from detail_comptes_traffic", $"{CbCompte.SelectedValue}", "CompteID")
                ComboBox1.Items.Clear()

                For i = 0 To dt.Rows.Count - 1
                    ComboBox1.Items.Add(dt.Rows(i)(0))
                Next
                ComboBox1.Text = ""
            End If
            If CbCompte.SelectedValue = 12 Then
                Guna2ToggleSwitch1.Visible = False
                Label1.Visible = False
            Else
                Guna2ToggleSwitch1.Visible = True
                Label1.Visible = True
            End If
        Catch ex As Exception

        End Try

    End Sub


    Private Sub VermentDeBankÀCoffreToolStripMenuItem_Click()
        Guna2ComboBox2.SelectedValue = 3
        Guna2ComboBox2.Enabled = False
        Guna2ComboBox1.SelectedValue = 4
        Guna2ComboBox1.Enabled = False
        FlowLayoutPanel1.Visible = False
        TransManPanel.Visible = True
        Guna2DateTimePicker1.Value = DateTime.Now
        Guna2DateTimePicker1.Enabled = False
    End Sub

    Private Sub VirmentDeCoffreÀCaisseToolStripMenuItem_Click()
        Guna2ComboBox2.SelectedValue = 4
        Guna2ComboBox2.Enabled = False
        Guna2ComboBox1.SelectedValue = 5
        Guna2ComboBox1.Enabled = False
        FlowLayoutPanel1.Visible = False
        TransManPanel.Visible = True
        Guna2DateTimePicker1.Value = DateTime.Now
        Guna2DateTimePicker1.Enabled = False
    End Sub

    Private Sub VirmentDeFunÀDpToolStripMenuItem_Click()
        Guna2ComboBox2.SelectedValue = 2
        Guna2ComboBox2.Enabled = False
        Guna2ComboBox1.SelectedValue = 1
        Guna2ComboBox1.Enabled = False
        FlowLayoutPanel1.Visible = False
        TransManPanel.Visible = True
        Guna2DateTimePicker1.Value = DateTime.Now
        Guna2DateTimePicker1.Enabled = False
    End Sub

    Private Sub CaiseAremise()
        Guna2ComboBox2.SelectedValue = 11
        Guna2ComboBox2.Enabled = False
        Guna2ComboBox1.SelectedValue = 16
        Guna2ComboBox1.Enabled = False
        FlowLayoutPanel1.Visible = False
        TransManPanel.Visible = True
        Guna2DateTimePicker1.Value = DateTime.Now
        Guna2DateTimePicker1.Enabled = False

    End Sub
    Private Sub VirmentDpÀBankToolStripMenuItem_Click()
        Guna2ComboBox2.SelectedValue = 1
        Guna2ComboBox2.Enabled = False
        Guna2ComboBox1.SelectedValue = 3
        Guna2ComboBox1.Enabled = False
        FlowLayoutPanel1.Visible = False
        TransManPanel.Visible = True
        Guna2DateTimePicker1.Value = DateTime.Now
        Guna2DateTimePicker1.Enabled = False
    End Sub

    Private Sub NewTranslactionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewTranslactionToolStripMenuItem.Click
        FlowLayoutPanel1.Visible = False
        TransManPanel.Visible = True
        Guna2ComboBox2.Enabled = True
        Guna2ComboBox1.Enabled = True
        Guna2DateTimePicker1.Value = DateTime.Now
    End Sub


    Private Sub VirmentSalafToolStripMenuItem_Click(i As Integer)
        CbCompte.SelectedValue = i
        CbCompte.Enabled = False
        FlowLayoutPanel1.Visible = False
        Guna2ShadowPanel2.Visible = True
        VRdtp.Value = DateTime.Now
        VRdtp.Enabled = False
    End Sub
    Private Sub MontantVirment()
        CbCompte.SelectedValue = 8
        CbCompte.Enabled = False
        FlowLayoutPanel1.Visible = False
        Guna2ShadowPanel2.Visible = True
        VRdtp.Value = DateTime.Now
        VRdtp.Enabled = False
    End Sub
    Private Sub SumprimerTransToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SumprimerTransToolStripMenuItem.Click
        FlowLayoutPanel1.Visible = False
        Guna2ShadowPanel3.Visible = True
        Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT * FROM tansactionscomptes", 1, "1")
        If dt.Rows.Count > 0 Then
            DGVTrans.DataSource = dt
        Else
            MsgBox("Ereur tableau vide")
        End If
    End Sub

    Private Sub Guna2CircleButton2_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton2.Click
        FlowLayoutPanel1.Visible = True
        Guna2ShadowPanel2.Visible = False
    End Sub



    Private Sub Guna2CircleButton10_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton10.Click
        FlowLayoutPanel1.Visible = True
        TransManPanel.Visible = False
    End Sub

    Private Sub Guna2ToggleSwitch1_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch1.CheckedChanged
        If Guna2ToggleSwitch1.Checked = True Then
            Label1.Text = "Virment"
        Else
            Label1.Text = "Retait"
        End If
    End Sub

    Private Sub VirmentBankReélToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Guna2ComboBox2.SelectedValue = 3
        Guna2ComboBox2.Enabled = False
        Guna2ComboBox1.SelectedValue = 13
        Guna2ComboBox1.Enabled = False
        FlowLayoutPanel1.Visible = False
        TransManPanel.Visible = True
        Guna2DateTimePicker1.Value = DateTime.Now
        Guna2DateTimePicker1.Enabled = False
    End Sub
    Private Sub Button_MouseMove(sender As Object, e As MouseEventArgs)
        If draggedButton IsNot Nothing AndAlso e.Button = MouseButtons.Left Then
            ' Get the mouse position relative to the FlowLayoutPanel
            Dim newLocation As Point = FlowLayoutPanel1.PointToClient(MousePosition)
            ' Set the new location of the dragged control
            draggedButton.Location = New Point(newLocation.X - mouseOffset.X, newLocation.Y - mouseOffset.Y)
        End If
    End Sub
    Private Sub Button_MouseUp(sender As Object, e As MouseEventArgs)
        If draggedButton IsNot Nothing Then
            ' Make sure to correctly position the control when dropped
            Dim dropLocation As Point = FlowLayoutPanel1.PointToClient(MousePosition)
            draggedButton.Location = New Point(dropLocation.X - mouseOffset.X, dropLocation.Y - mouseOffset.Y)
            draggedButton = Nothing
        End If
    End Sub
    Private Sub ActualiserToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ActualiserToolStripMenuItem.Click, Guna2CircleButton4.Click
        filldat()
        AutoOperations.UComptes1(DateTime.Now)
    End Sub

    Private Sub Guna2CircleButton8_Click(sender As Object, e As EventArgs)
        CheckLesVersement.Show()
    End Sub
    Private draggedButton As ComptesBox
    Private mouseOffset As Point
    Private Function GetInsertionIndex(dropPoint As Point) As Integer
        For i As Integer = 0 To FlowLayoutPanel1.Controls.Count - 1
            Dim ctrl As Control = FlowLayoutPanel1.Controls(i)
            If ctrl.Bounds.Contains(dropPoint) Then
                Return i
            End If
        Next
        Return FlowLayoutPanel1.Controls.Count - 1 ' Insert at the end if not over any control
    End Function

    ' Handle MouseDown event to begin dragging
    Private Sub Button_MouseDown(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            draggedButton = DirectCast(sender, ComptesBox)
            mouseOffset = New Point(e.X, e.Y)
            draggedButton.BringToFront()
        End If
    End Sub
    ' Enable double buffering on the control to reduce flickering
    Private Sub EnableDoubleBuffering(control As Control)
        Dim doubleBufferProperty = control.GetType().GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        If doubleBufferProperty IsNot Nothing Then
            doubleBufferProperty.SetValue(control, True, Nothing)
        End If
    End Sub

End Class