Imports MySql.Data.MySqlClient
Public Class usersFrm
    Public Shared connectiona As MySqlConnection = Class1.conectionscr
    Public Shared connectiono As MySqlConnection = Class1.conectiondmc
    Public Shared connectiont As MySqlConnection = Class1.conectiontrk
    Public userpermability As String = "Admin"
    Dim usermanpaneltype As String = ""
    Public uid As Integer
    Public Pass As String
    Private Sub FillDat()
        Dim command As New MySqlCommand("SELECT * FROM users where First=@first", connectiona)
        command.Parameters.Add("@first", MySqlDbType.VarChar).Value = Form1.first
        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()

        adapter.Fill(table)
        If table.Rows.Count = 0 Then
            MsgBox("Error")
        Else
            uid = table.Rows(0)(0)

            Pass = table.Rows(0)(4).ToString
        End If
        ''Fill combo Company
        Dim command1 As New MySqlCommand("SELECT * FROM companies", connectiona)
        Dim adapter1 As New MySqlDataAdapter(command1)
        Dim table1 As New DataTable()

        adapter1.Fill(table1)
        If table1.Rows.Count = 0 Then
            MsgBox("Error")
        Else
            cbComp.DataSource = table1
            cbComp.DisplayMember = "CompanyName"
            cbComp.ValueMember = "ID"
        End If
    End Sub
    Private Sub usersFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FillDat()
    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click, Label7.Click

    End Sub

    Private Sub btnMdfFnLn_Click(sender As Object, e As EventArgs) Handles btnMdfFnLn.Click
        btnMdfFnLn.Visible = False
        tbLName.Enabled = True
        tbFName.Enabled = True

    End Sub

    Private Sub BtnMdfInfo_Click(sender As Object, e As EventArgs) Handles BtnMdfInfo.Click
        BtnMdfInfo.Visible = False
        tbInfo.Enabled = True
    End Sub

    Private Sub BtnMdfComp_Click(sender As Object, e As EventArgs) Handles BtnMdfComp.Click
        BtnMdfComp.Visible = False
        cbComp.Enabled = True
    End Sub

    Private Sub BtnMdfUsernam_Click(sender As Object, e As EventArgs) Handles BtnMdfUsernam.Click
        BtnMdfUsernam.Visible = False
        tbUsername.Enabled = True
    End Sub

    Private Sub BtnMdfPass_Click(sender As Object, e As EventArgs) Handles BtnMdfPass.Click
        BtnMdfPass.Visible = False
        tbCurrPass.Enabled = True
        tbNewPass.Visible = True
        tbConfNewPass.Visible = True
        Label10.Visible = True
        Label11.Visible = True
        tbCurrPass.Clear()

    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        MYAccount.Visible = True
        connectiona.Open()
        'Load img


        Dim commandim As New MySqlCommand("SELECT Usernam, password, image, First, Last  FROM users WHERE ID = @uid", connectiona)

        commandim.Parameters.AddWithValue("@uid", uid)

        Dim adapterim As New MySqlDataAdapter(commandim)
        Dim tableim As New DataTable()

        adapterim.Fill(tableim)

        If tableim.Rows.Count = 0 Then

            MessageBox.Show("Invalid user")

        Else
            CastImage.ReadImgID(uid, userspic)
        End If
        '''''''
        'Personal Info
        Dim command As New MySqlCommand("SELECT * FROM users where First=@first", connectiona)
        command.Parameters.Add("@first", MySqlDbType.VarChar).Value = Form1.first
        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()

        adapter.Fill(table)
        If table.Rows.Count = 0 Then
            MsgBox("Error")
        Else
            uid = table.Rows(0)(0)
            tbFName.Text = table.Rows(0)(1).ToString
            tbLName.Text = table.Rows(0)(2).ToString
            tbInfo.Text = table.Rows(0)(7).ToString
            tbUsername.Text = table.Rows(0)(3).ToString
            tbCurrPass.Text = table.Rows(0)(4).ToString
            cbComp.SelectedValue = table.Rows(0)(8)
            Pass = table.Rows(0)(4).ToString
            Try

                Dim command2 As New MySqlCommand("SELECT * FROM companies where ID=@uid", connectiona)
                command2.Parameters.AddWithValue("@uid", table.Rows(0)(8).ToString())
                Dim adapter2 As New MySqlDataAdapter(command2)
                Dim table2 As New DataTable()

                adapter2.Fill(table2)
                If table2.Rows.Count = 0 Then
                    MsgBox("Error Id of company")
                Else
                    tbUAd.Text = table2.Rows(0)(5).ToString
                    tbChef.Text = table2.Rows(0)(4).ToString
                    tbSEO.Text = table2.Rows(0)(3).ToString
                End If
            Catch es As Exception
            End Try
        End If
        connectiona.Close()
    End Sub

    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        UserMana.Visible = False
        MYAccount.Visible = False
    End Sub

    Private Sub BtnSaveLogininfo_Click(sender As Object, e As EventArgs) Handles BtnSaveLogininfo.Click
        connectiona.Open()
        'Personal Info
        If tbCurrPass.Enabled = False And tbCurrPass.Text <> "" And tbUsername.Text <> "" Then
            Dim command As New MySqlCommand("UPDATE users SET usernam = @username, password = @password WHERE ID = @uid", connectiona)
            command.Parameters.Add("@username", MySqlDbType.VarChar).Value = tbUsername.Text
            command.Parameters.Add("@uid", MySqlDbType.Int32).Value = uid
            command.Parameters.Add("@password", MySqlDbType.VarChar).Value = tbCurrPass.Text
            command.ExecuteNonQuery()
            TrackClass.AddUserTLine(Form1.idd, Form1.first, "Update", "Update User Information(U/P)", $"Update Username with {tbUsername.Text} of user Id= {uid}", "", DateTime.Now)

        ElseIf tbNewPass.Text = tbConfNewPass.Text And tbConfNewPass.Text <> "" Then
            If tbCurrPass.Text = Pass Then
                Dim command As New MySqlCommand("UPDATE users SET usernam = @username, password = @password WHERE ID = @uid", connectiona)
                command.Parameters.Add("@username", MySqlDbType.VarChar).Value = tbUsername.Text
                command.Parameters.Add("@password", MySqlDbType.VarChar).Value = tbNewPass.Text
                command.Parameters.Add("@uid", MySqlDbType.Int32).Value = uid
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Update", "Update User Information(U/P)", $"Update Username with {tbUsername.Text} And Password with {tbNewPass.Text} of user Id= {uid}", "", DateTime.Now)

                command.ExecuteNonQuery()
                BtnMdfUsernam.Visible = True
                tbUsername.Enabled = False
                BtnMdfPass.Visible = True
                tbCurrPass.Enabled = False
                tbNewPass.Visible = False
                tbConfNewPass.Visible = False
                Label10.Visible = False
                Label11.Visible = False
            Else
                MsgBox("The Current Password is incorrect (is : " & Pass & " )")
            End If
        Else
            MsgBox("Please check the confirmation of password or usernam and password or new password")

        End If

        connectiona.Close()

    End Sub

    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        UserMana.Visible = True
        connectiona.Open()
        'Load img


        Dim commandim As New MySqlCommand("SELECT Usernam, password, image, First, Last  FROM users WHERE ID = @uid", connectiona)

        commandim.Parameters.AddWithValue("@uid", uid)

        Dim adapterim As New MySqlDataAdapter(commandim)
        Dim tableim As New DataTable()

        adapterim.Fill(tableim)

        If tableim.Rows.Count = 0 Then

            MessageBox.Show("Invalid user")

        Else
            CastImage.ReadImgID(uid, usepic)
        End If
        '''''''
        Dim command As New MySqlCommand("SELECT * FROM users", connectiona)


        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()

        adapter.Fill(table)

        If table.Rows.Count = 0 Then

            MessageBox.Show("Invalid Username Or Password")

        Else
            dgvUsers.DataSource = table
        End If
        connectiona.Close()
    End Sub

    Private Sub Guna2CircleButton8_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton8.Click
        UserMana.Visible = False
        MYAccount.Visible = False
        CompanyManagement.Visible = False
    End Sub

    Private Sub Guna2GradientButton4_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton4.Click
        CompanyManagement.Visible = True
        connectiona.Open()
        'Load img


        Dim commandim As New MySqlCommand("SELECT Usernam, password, image, First, Last  FROM users WHERE ID = @uid", connectiona)

        commandim.Parameters.AddWithValue("@uid", uid)

        Dim adapterim As New MySqlDataAdapter(commandim)
        Dim tableim As New DataTable()

        adapterim.Fill(tableim)

        If tableim.Rows.Count = 0 Then

            MessageBox.Show("Invalid user")

        Else
            CastImage.ReadImgID(uid, usepic)
        End If
        '''''''
        Dim command As New MySqlCommand("SELECT * FROM Companies", connectiona)


        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()

        adapter.Fill(table)

        If table.Rows.Count = 0 Then

            MessageBox.Show("Invalid Username Or Password")

        Else
            DGVComp.DataSource = table
        End If
        connectiona.Close()
    End Sub

    Private Sub Guna2CircleButton4_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton4.Click
        UserMana.Visible = False
        MYAccount.Visible = False
        CompanyManagement.Visible = False

    End Sub

    Private Sub Guna2GradientButton5_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton5.Click
        ''Check count - 1 of dgvUsers
        connectiona.Open()
        Dim index As Integer = dgvUsers.Rows.Count - 2
        ''Check id of item is existe
        For i = 0 To index


            Dim command As New MySqlCommand("SELECT * FROM users where ID=@uid", connectiona)
            command.Parameters.AddWithValue("@uid", dgvUsers.Rows(i).Cells(0).Value)
            Dim adapter As New MySqlDataAdapter(command)
            Dim table As New DataTable()

            adapter.Fill(table)
            If table.Rows.Count = 0 Then
                ''ADD statement
                Dim comman As New MySqlCommand("INSERT INTO users (ID, First, Last, Usernam, password, image, datex, Info, Company, Validities) VALUES (@uid, @First, @Last, @Usernam, @password, @image, @datex,@INFO, @comp, @vald);", connectiona)
                comman.Parameters.AddWithValue("@uid", dgvUsers.Rows(i).Cells(0).Value)
                comman.Parameters.AddWithValue("@First", dgvUsers.Rows(i).Cells(1).Value.ToString)
                comman.Parameters.AddWithValue("@Last", dgvUsers.Rows(i).Cells(2).Value.ToString)
                comman.Parameters.AddWithValue("@Usernam", dgvUsers.Rows(i).Cells(3).Value.ToString)
                comman.Parameters.AddWithValue("@password", dgvUsers.Rows(i).Cells(4).Value.ToString)
                comman.Parameters.AddWithValue("@datex", DateAndTime.Now)
                comman.Parameters.AddWithValue("@INFO", dgvUsers.Rows(i).Cells(7).Value.ToString)
                comman.Parameters.AddWithValue("@vald", dgvUsers.Rows(i).Cells(9).Value.ToString)
                comman.Parameters.AddWithValue("@Comp", CInt(dgvUsers.Rows(i).Cells(8).Value.ToString))
                Using picture As Image = My.Resources.user
                    'Create an empty stream in memory.'
                    Using stream As New IO.MemoryStream
                        'Fill the stream with the binary data from the Image.'
                        picture.Save(stream, Imaging.ImageFormat.Jpeg)

                        'Get an array of Bytes from the stream and assign to the parameter.'
                        comman.Parameters.AddWithValue("@image", stream.GetBuffer())
                    End Using
                End Using
                comman.ExecuteNonQuery()
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Add", "Add User", $"Add a new User with Name: {dgvUsers.Rows(i).Cells(1).Value.ToString} .", "", DateTime.Now)

            Else
                ''Update Statement
                Dim comman As New MySqlCommand("UPDATE users SET First=@First, Last=@Last, Usernam=@Usernam, password=@password, datex=@datex, Info=@INFO , Company=@Comp, Validities=@vald WHERE ID = @uid;", connectiona)
                comman.Parameters.AddWithValue("@uid", dgvUsers.Rows(i).Cells(0).Value)
                comman.Parameters.AddWithValue("@First", dgvUsers.Rows(i).Cells(1).Value.ToString)
                comman.Parameters.AddWithValue("@Last", dgvUsers.Rows(i).Cells(2).Value.ToString)
                comman.Parameters.AddWithValue("@Usernam", dgvUsers.Rows(i).Cells(3).Value.ToString)
                comman.Parameters.AddWithValue("@password", dgvUsers.Rows(i).Cells(4).Value.ToString)
                comman.Parameters.AddWithValue("@datex", DateAndTime.Now)
                comman.Parameters.AddWithValue("@INFO", dgvUsers.Rows(i).Cells(7).Value.ToString)
                comman.Parameters.AddWithValue("@Comp", CInt(dgvUsers.Rows(i).Cells(8).Value.ToString))
                comman.Parameters.AddWithValue("@vald", dgvUsers.Rows(i).Cells(9).Value.ToString)
                comman.ExecuteNonQuery()
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Update", "Update User", $"Update a User with Name: {dgvUsers.Rows(i).Cells(1).Value.ToString} .", "", DateTime.Now)

            End If
        Next
        FillDat()
        connectiona.Close()
        Guna2GradientButton2.PerformClick()
    End Sub

    Private Sub Guna2CircleButton5_Click(sender As Object, e As EventArgs)
        If usermanpaneltype = "Add" Then
            'add algorithm
        ElseIf usermanpaneltype = "Update" Then
            'Updat
        Else
            MsgBox("Error User Management Pannel Type, Variable is: " & usermanpaneltype, MsgBoxStyle.Critical)
        End If
    End Sub

    Private Sub Guna2GradientButton6_Click(sender As Object, e As EventArgs)
        usermanpaneltype = "Update"
    End Sub

    Private Sub Guna2GradientButton7_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton7.Click
        Dim ES As MsgBoxResult = MsgBox("Doyou want to delete user no: " & dgvUsers.SelectedRows(0).Cells(0).Value, MsgBoxStyle.Critical + MsgBoxStyle.YesNo)
        Try
            If ES = MsgBoxResult.Yes Then
                connectiona.Open()
                Dim comman As New MySqlCommand("DELETE FROM users WHERE ID = @uid;", connectiona)
                comman.Parameters.AddWithValue("@uid", dgvUsers.SelectedRows(0).Cells(0).Value)
                comman.ExecuteNonQuery()
                connectiona.Close()
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Delete", "Delete User", $"Delete a User with Name: {dgvUsers.SelectedRows(0).Cells(1).Value.ToString} And ID= {dgvUsers.SelectedRows(0).Cells(0).Value} .", "", DateTime.Now)

                FillDat()
            Else
                Exit Sub
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Guna2GroupBox4_Click(sender As Object, e As EventArgs) Handles Guna2GroupBox4.Click

    End Sub

    Private Sub Guna2CircleButton7_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton7.Click
        GridControl1.ShowRibbonPrintPreview()
        TrackClass.AddUserTLine(Form1.idd, Form1.first, "Print", "Print table of Users History (This table)", $"", "", DateTime.Now)

    End Sub

    Private Sub Guna2GradientButton3_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton3.Click
        PannelHistory.Visible = True
        If connectiont.State = ConnectionState.Closed Then
            connectiont.Open()
        End If

        Dim command As New MySqlCommand("SELECT * FROM userstrack", connectiont)


        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()

        adapter.Fill(table)

        If table.Rows.Count = 0 Then

            MessageBox.Show("No Data")

        Else
            GridControl1.DataSource = table
        End If
        connectiont.Close()
    End Sub

    Private Sub Guna2CircleButton6_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Guna2CircleButton9_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton9.Click
        PannelHistory.Visible = False
    End Sub

    Private Sub Guna2CircleButton10_Click(sender As Object, e As EventArgs)
    End Sub

    Private Sub BtnSaveUserInfo_Click(sender As Object, e As EventArgs) Handles BtnSaveUserInfo.Click
        Try
            connectiona.Open()
            Dim command As New MySqlCommand("UPDATE users SET First = @first, Last = @last, Info = @info, image = @image WHERE ID = @uid", connectiona)
            command.Parameters.AddWithValue("@first", tbFName.Text)
            command.Parameters.AddWithValue("@last", tbLName.Text)
            command.Parameters.AddWithValue("@info", tbInfo.Text)
            command.Parameters.AddWithValue("@uid", uid)
            Using picture As Image = userspic.Image
                'Create an empty stream in memory.'
                Using stream As New IO.MemoryStream
                    'Fill the stream with the binary data from the Image.'
                    picture.Save(stream, Imaging.ImageFormat.Jpeg)

                    'Get an array of Bytes from the stream and assign to the parameter.'
                    command.Parameters.AddWithValue("@image", stream.GetBuffer())
                End Using
            End Using
            TrackClass.AddUserTLine(Form1.idd, Form1.first, "Update", "Update User Information(All)", $"Update Profile of this current user", "", DateTime.Now)

            command.ExecuteNonQuery()
            CastImage.ReadImgID(uid, Form1.Guna2CirclePictureBox1)
            FillDat()
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            connectiona.Close()
        End Try
        Guna2GradientButton1.PerformClick()
    End Sub

    Private Sub BtnMdfImageProfile_Click(sender As Object, e As EventArgs) Handles BtnMdfImageProfile.Click
        Try
            If OpenFileDialog1.ShowDialog <> Windows.Forms.DialogResult.Cancel Then

                userspic.Image = Image.FromFile(OpenFileDialog1.FileName)
            End If
        Catch ex As Exception
            MsgBox(ex.ToString())
        End Try


    End Sub

    Private Sub Guna2CircleButton3_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton3.Click
        ''Check count - 1 of dgvUsers
        connectiona.Open()
        Dim index As Integer = DGVComp.Rows.Count - 2
        ''Check id of item is existe
        For i = 0 To index

            Dim command As New MySqlCommand("SELECT * FROM companies where ID=@uid", connectiona)
            command.Parameters.AddWithValue("@uid", DGVComp.Rows(i).Cells(0).Value)
            Dim adapter As New MySqlDataAdapter(command)
            Dim table As New DataTable()

            adapter.Fill(table)
            If table.Rows.Count = 0 Then
                ''ADD statement
                Dim comman As New MySqlCommand("INSERT INTO companies (ID, CompanyName, CompanyDescription, SEO, Chef, AdminLocal, datex) VALUES (@uid, @CompanyName, @CompanyDescription, @SEO, @Chef, @AdminLocal, @datex);", connectiona)
                comman.Parameters.AddWithValue("@uid", DGVComp.Rows(i).Cells(0).Value)
                comman.Parameters.AddWithValue("@CompanyName", DGVComp.Rows(i).Cells(1).Value.ToString)
                comman.Parameters.AddWithValue("@CompanyDescription", DGVComp.Rows(i).Cells(2).Value.ToString)
                comman.Parameters.AddWithValue("@SEO", DGVComp.Rows(i).Cells(3).Value.ToString)
                comman.Parameters.AddWithValue("@Chef", DGVComp.Rows(i).Cells(4).Value.ToString)
                comman.Parameters.AddWithValue("@AdminLocal", DGVComp.Rows(i).Cells(5).Value.ToString)
                comman.Parameters.AddWithValue("@datex", DateAndTime.Now)

                comman.ExecuteNonQuery()
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Insert", "Insert Company", $"Add {DGVComp.Rows(i).Cells(1).Value.ToString} Company to this database.", "", DateTime.Now)
                FillDat()
            Else
                ''Update Statement
                Dim comman As New MySqlCommand("UPDATE companies SET CompanyName=@CompanyName, CompanyDescription=@CompanyDescription, SEO=@SEO, Chef=@Chef, datex=@datex, AdminLocal=@AdminLocal WHERE ID = @uid;", connectiona)
                comman.Parameters.AddWithValue("@uid", DGVComp.Rows(i).Cells(0).Value)
                comman.Parameters.AddWithValue("@CompanyName", DGVComp.Rows(i).Cells(1).Value.ToString)
                comman.Parameters.AddWithValue("@CompanyDescription", DGVComp.Rows(i).Cells(2).Value.ToString)
                comman.Parameters.AddWithValue("@SEO", DGVComp.Rows(i).Cells(3).Value.ToString)
                comman.Parameters.AddWithValue("@Chef", DGVComp.Rows(i).Cells(4).Value.ToString)
                comman.Parameters.AddWithValue("@datex", DateAndTime.Now)
                comman.Parameters.AddWithValue("@AdminLocal", DGVComp.Rows(i).Cells(5).Value.ToString)

                comman.ExecuteNonQuery()
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Update", "Update Company", $"Update {DGVComp.Rows(i).Cells(1).Value.ToString} Company informations.", "", DateTime.Now)

                FillDat()
            End If
        Next
        connectiona.Close()
        Guna2GradientButton4.PerformClick()
    End Sub

    Private Sub cbComp_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbComp.SelectedIndexChanged
        Try
            Dim hh As Integer = cbComp.SelectedValue
            Dim command As New MySqlCommand("SELECT * FROM companies where ID=@uid", connectiona)
            command.Parameters.AddWithValue("@uid", hh)
            Dim adapter As New MySqlDataAdapter(command)
            Dim table As New DataTable()

            adapter.Fill(table)
            If table.Rows.Count = 0 Then
                MsgBox("Error Id of company")
            Else
                tbUAd.Text = table.Rows(0)(5).Value.ToString
                tbChef.Text = table.Rows(0)(4).Value.ToString
                tbSEO.Text = table.Rows(0)(3).Value.ToString
            End If
        Catch es As Exception
        End Try
    End Sub

    Private Sub Guna2GradientButton6_Click_1(sender As Object, e As EventArgs) Handles Guna2GradientButton6.Click
        Me.Close()
        Form1.shawFrm(HisabatGestion)
        Form1.shawFrm(HisabatGestion)
    End Sub
End Class