Imports System
Imports System.Data
Imports System.Runtime.InteropServices
Imports MySql.Data.MySqlClient
Public Class LoginFrm

    Public connectiona As MySqlConnection = Class1.conectionscr

    Private Sub Guna2ControlBox2_Click(sender As Object, e As EventArgs) Handles Guna2ControlBox2.Click
        Application.Exit()
    End Sub
    Sub LOGINfUNCTION()
        If connectiona.State = ConnectionState.Closed Then
            connectiona.Open()
        End If


        Dim command As New MySqlCommand("SELECT Usernam, password, image, First, Last, ID, Company  FROM users WHERE Usernam = @username AND password = @password", connectiona)

        command.Parameters.Add("@username", MySqlDbType.VarChar).Value = usernamtb.Text
        command.Parameters.Add("@password", MySqlDbType.VarChar).Value = passwoertb.Text

        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()

        adapter.Fill(table)

        If table.Rows.Count = 0 Then

            MessageBox.Show("Invalid Username Or Password")

        Else

            'MessageBox.Show("Logged In")
            Dim first As String = table.Rows(0)(3).ToString
            Form1.first = first

            Form1.Compny = table.Rows(0)(6)
            Form1.Last = table.Rows(0)(4).ToString
            Form1.idd = table.Rows(0)(5)
            Form1.Label1.Text = "Bienvenue " & first & " " & table.Rows(0)(4).ToString
            Form1.Show()
            CastImage.ReadImg(usernamtb.Text)

            TrackClass.AddUserTLine(Form1.idd, Form1.first, "LogIn", "logIn to Application", $"", "", DateTime.Now)
            Me.Hide()
            Me.usernamtb.Text = ""
            Me.passwoertb.Text = ""
        End If
        connectiona.Close()
    End Sub
    Private Sub LoginFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        LOGINfUNCTION()
    End Sub

    Private Sub Enter2_Click(sender As Object, e As EventArgs) Handles Enter2.Click
        If TbAppPass.Text = My.Settings.AppPass Then
            connectionSettings.Show()
            ConnectionP.Visible = False
            Panel1.Visible = True
        Else
            MsgBox("Error in the password or);")
        End If
    End Sub

    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        If ConnectionP.Visible = False Then
            ConnectionP.Visible = True
            Panel1.Visible = False
        Else
            ConnectionP.Visible = False
            Panel1.Visible = True
        End If
    End Sub

    Private Sub passwoertb_KeyPress(sender As Object, e As KeyPressEventArgs) Handles passwoertb.KeyPress
        If e.KeyChar = ChrW(Keys.Enter) Then
            Guna2Button1.PerformClick()
        End If
    End Sub
End Class