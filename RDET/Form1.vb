Imports System.Diagnostics
Imports System.IO.Pipes
Imports System.Text
Imports System.IO
Public Class Form1
    Public first As String
    Public Last As String
    Public Shared idd As Integer
    Public Compny As Integer
    Public Shared valdi As String
    Dim CurrentFrm As Form
    Private Sub Guna2CircleButton7_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton7.Click
        If CurrentFrm IsNot Nothing Then
            CurrentFrm.Close()
        End If
        Dim f As Form = CleanTele
        shawFrm(f)
        For i = 0 To 1
            shawFrm(f)
        Next

        Dim leftPanelWidth As Integer = Guna2GradientPanel1.Width ' Adjust this according to your left panel width
        Dim newWidth As Integer = Me.ClientSize.Width - leftPanelWidth
        Dim newHeight As Integer = Me.ClientSize.Height - (Guna2Panel1.Height)
        ConseptionPN.Size = New Size(newWidth, newHeight)
        If ConseptionPN.Controls.Count > 0 AndAlso TypeOf ConseptionPN.Controls(0) Is Form Then
            'MsgBox("y")
            Dim form As Form = DirectCast(ConseptionPN.Controls(0), Form)
            form.WindowState = FormWindowState.Normal
            form.Size = ConseptionPN.ClientSize
            form.WindowState = FormWindowState.Maximized
        End If

    End Sub

    Private Sub Guna2CircleButton8_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton8.Click
        If CurrentFrm IsNot Nothing Then
            CurrentFrm.Close()
        End If
        Dim f As Form = Settings
        shawFrm(f)
        For i = 0 To 0
            shawFrm(f)
        Next

        Dim leftPanelWidth As Integer = Guna2GradientPanel1.Width ' Adjust this according to your left panel width
        Dim newWidth As Integer = Me.ClientSize.Width - leftPanelWidth
        Dim newHeight As Integer = Me.ClientSize.Height - (Guna2Panel1.Height)
        ConseptionPN.Size = New Size(newWidth, newHeight)
        If ConseptionPN.Controls.Count > 0 AndAlso TypeOf ConseptionPN.Controls(0) Is Form Then
            'MsgBox("y")
            Dim form As Form = DirectCast(ConseptionPN.Controls(0), Form)
            form.WindowState = FormWindowState.Normal
            form.Size = ConseptionPN.ClientSize
            form.WindowState = FormWindowState.Maximized
        End If

    End Sub



    Private Sub Guna2CircleButton6_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton6.Click
        If valdi = "Admin" Or valdi = "Chef" Then
            CaiseCalcule.Show()
            CaiseCalcule.BringToFront()
        End If
    End Sub
    Public Shared Sub shawFrm(frm As Form)
        Dim f As Form = frm
        f.TopLevel = False
        f.WindowState = FormWindowState.Maximized
        f.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        f.Visible = True
        Form1.ConseptionPN.Controls.Clear()
        Form1.ConseptionPN.Controls.Add(f)
        Form1.CurrentFrm = f
    End Sub
    Private Sub Guna2CircleButton2_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton2.Click
        If valdi = "Admin" Then
            If CurrentFrm IsNot Nothing Then
                CurrentFrm.Close()
            End If
            Dim f As Form = usersFrm
            shawFrm(f)
            For i = 0 To 1
                shawFrm(f)
            Next
        ElseIf valdi = "Chef" Then
            If CurrentFrm IsNot Nothing Then
                CurrentFrm.Close()
            End If
            Dim f As Form = usersFrm
            shawFrm(f)
            For i = 0 To 1
                shawFrm(f)
            Next
            usersFrm.Guna2GradientButton2.Visible = False
            usersFrm.Guna2GradientButton4.Visible = False
            usersFrm.Guna2GradientButton6.Visible = False

            Dim leftPanelWidth As Integer = Guna2GradientPanel1.Width ' Adjust this according to your left panel width
            Dim newWidth As Integer = Me.ClientSize.Width - leftPanelWidth
            Dim newHeight As Integer = Me.ClientSize.Height - (Guna2Panel1.Height)
            ConseptionPN.Size = New Size(newWidth, newHeight)
            If ConseptionPN.Controls.Count > 0 AndAlso TypeOf ConseptionPN.Controls(0) Is Form Then
                'MsgBox("y")
                Dim form As Form = DirectCast(ConseptionPN.Controls(0), Form)
                form.WindowState = FormWindowState.Normal
                form.Size = ConseptionPN.ClientSize
                form.WindowState = FormWindowState.Maximized
            End If
        Else
            If CurrentFrm IsNot Nothing Then
                CurrentFrm.Close()
            End If
            Dim f As Form = usersFrm
            shawFrm(f)
            For i = 0 To 1
                shawFrm(f)
            Next
            usersFrm.Guna2GradientButton2.Visible = False
            usersFrm.Guna2GradientButton4.Visible = False
            usersFrm.Guna2GradientButton3.Visible = False
            usersFrm.Guna2GradientButton6.Visible = False

            Dim leftPanelWidth As Integer = Guna2GradientPanel1.Width ' Adjust this according to your left panel width
            Dim newWidth As Integer = Me.ClientSize.Width - leftPanelWidth
            Dim newHeight As Integer = Me.ClientSize.Height - (Guna2Panel1.Height)
            ConseptionPN.Size = New Size(newWidth, newHeight)
            If ConseptionPN.Controls.Count > 0 AndAlso TypeOf ConseptionPN.Controls(0) Is Form Then
                'MsgBox("y")
                Dim form As Form = DirectCast(ConseptionPN.Controls(0), Form)
                form.WindowState = FormWindowState.Normal
                form.Size = ConseptionPN.ClientSize
                form.WindowState = FormWindowState.Maximized
            End If
        End If
    End Sub

    Private Sub Guna2CircleButton3_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton3.Click
        If valdi = "Admin" Then
            If CurrentFrm IsNot Nothing Then
                CurrentFrm.Close()
            End If
            Dim f As Form = servicesManagement
            shawFrm(f)
            For i = 0 To 1
                shawFrm(f)
            Next

            Dim leftPanelWidth As Integer = Guna2GradientPanel1.Width ' Adjust this according to your left panel width
            Dim newWidth As Integer = Me.ClientSize.Width - leftPanelWidth
            Dim newHeight As Integer = Me.ClientSize.Height - (Guna2Panel1.Height)
            ConseptionPN.Size = New Size(newWidth, newHeight)
            If ConseptionPN.Controls.Count > 0 AndAlso TypeOf ConseptionPN.Controls(0) Is Form Then
                'MsgBox("y")
                Dim form As Form = DirectCast(ConseptionPN.Controls(0), Form)
                form.WindowState = FormWindowState.Normal
                form.Size = ConseptionPN.ClientSize
                form.WindowState = FormWindowState.Maximized
            End If
        Else
            MsgBox("Sorry but cou can't enter to this part")
        End If
    End Sub

    Private Sub Guna2CircleButton4_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton4.Click
        If valdi = "Admin" Or valdi = "Chef" Then
            If CurrentFrm IsNot Nothing Then
                CurrentFrm.Close()
            End If
            Dim f As Form = tras
            shawFrm(f)
            For i = 0 To 0
                shawFrm(f)
            Next

            Dim leftPanelWidth As Integer = Guna2GradientPanel1.Width ' Adjust this according to your left panel width
            Dim newWidth As Integer = Me.ClientSize.Width - leftPanelWidth
            Dim newHeight As Integer = Me.ClientSize.Height - (Guna2Panel1.Height)
            ConseptionPN.Size = New Size(newWidth, newHeight)
            If ConseptionPN.Controls.Count > 0 AndAlso TypeOf ConseptionPN.Controls(0) Is Form Then
                'MsgBox("y")
                Dim form As Form = DirectCast(ConseptionPN.Controls(0), Form)
                form.WindowState = FormWindowState.Normal
                form.Size = ConseptionPN.ClientSize
                form.WindowState = FormWindowState.Maximized
            End If
        End If

    End Sub

    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        If valdi = "Admin" Or valdi = "Chef" Then
            If CurrentFrm IsNot Nothing Then
                CurrentFrm.Close()
            End If
            Dim f As Form = DashBoard
            shawFrm(f)
            For i = 0 To 1
                shawFrm(f)
            Next

            Dim leftPanelWidth As Integer = Guna2GradientPanel1.Width ' Adjust this according to your left panel width
            Dim newWidth As Integer = Me.ClientSize.Width - leftPanelWidth
            Dim newHeight As Integer = Me.ClientSize.Height - (Guna2Panel1.Height)
            ConseptionPN.Size = New Size(newWidth, newHeight)
            If ConseptionPN.Controls.Count > 0 AndAlso TypeOf ConseptionPN.Controls(0) Is Form Then
                'MsgBox("y")
                Dim form As Form = DirectCast(ConseptionPN.Controls(0), Form)
                form.WindowState = FormWindowState.Normal
                form.Size = ConseptionPN.ClientSize
                form.WindowState = FormWindowState.Maximized
            End If
        Else
            MsgBox("Sorry but cou can't enter to this part")
        End If

    End Sub

    Private Sub Guna2CirclePictureBox1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles Guna2CirclePictureBox1.MouseDoubleClick
        TrackClass.AddUserTLine(Form1.idd, first, "LogOut", "LogOut the application", $"", "", DateTime.Now)
        Me.Visible = False
        LoginFrm.Visible = True
        LoginFrm.usernamtb.Text = ""
        LoginFrm.passwoertb.Text = ""

    End Sub

    Private Sub Guna2CircleButton5_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton5.Click
        If valdi = "Admin" Or valdi = "Chef" Then
            If CurrentFrm IsNot Nothing Then
                CurrentFrm.Close()
            End If
            Dim f As Form = rapports
            shawFrm(f)
            For i = 0 To 1
                shawFrm(f)
            Next

            Dim leftPanelWidth As Integer = Guna2GradientPanel1.Width ' Adjust this according to your left panel width
            Dim newWidth As Integer = Me.ClientSize.Width - leftPanelWidth
            Dim newHeight As Integer = Me.ClientSize.Height - (Guna2Panel1.Height)
            ConseptionPN.Size = New Size(newWidth, newHeight)
            If ConseptionPN.Controls.Count > 0 AndAlso TypeOf ConseptionPN.Controls(0) Is Form Then
                'MsgBox("y")
                Dim form As Form = DirectCast(ConseptionPN.Controls(0), Form)
                form.WindowState = FormWindowState.Normal
                form.Size = ConseptionPN.ClientSize
                form.WindowState = FormWindowState.Maximized
            End If
        Else
            MsgBox("Sorry but cou can't enter to this part")
        End If
    End Sub

    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        TrackClass.AddUserTLine(Form1.idd, first, "LogOut", "LogOut the application", $"", "", DateTime.Now)
        LoginFrm.Visible = True
        LoginFrm.usernamtb.Text = ""
        LoginFrm.passwoertb.Text = ""
        Application.Exit()
    End Sub

    Private Sub Guna2CircleButton9_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton9.Click
        'If CurrentFrm IsNot Nothing Then
        '    CurrentFrm.Close()
        'End If
        'Dim f As Form = Register
        'shawFrm(f)
        'For i = 0 To 1
        '    shawFrm(f)
        'Next
        register_inf.Show()
        register_inf.BringToFront()
        'estFrm.Show()
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AutoOperations.UComptes1(DateTime.Now)
        Dim valstr As String = utilities.checkValiditi(first, idd)
        If valstr = "Admin" Then
            ' Guna2CircleButton11.Visible = True
        End If

        If valstr = "Admin" Or valstr = "Chef" Or valstr = "User" Then

            valdi = valstr
            Me.Text = first & " " & Last
        Else
            MsgBox("Error in validity of user")
            Application.Exit()
        End If
        Try
            Guna2GradientPanel1.FillColor = My.Settings.FirstColor
            Guna2GradientPanel1.FillColor2 = My.Settings.SecondColor
        Catch ex As Exception

        End Try
        'ToolTip1.SetToolTip(Guna2CircleButton10, "Home")
    End Sub

    Private Sub Guna2CircleButton9_Resize(sender As Object, e As EventArgs) Handles Guna2CircleButton9.Resize

    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize, ConseptionPN.Resize

        Dim leftPanelWidth As Integer = Guna2GradientPanel1.Width ' Adjust this according to your left panel width
        Dim newWidth As Integer = Me.ClientSize.Width - leftPanelWidth
        Dim newHeight As Integer = Me.ClientSize.Height - (Guna2Panel1.Height)
        ConseptionPN.Size = New Size(newWidth, newHeight)
        If ConseptionPN.Controls.Count > 0 AndAlso TypeOf ConseptionPN.Controls(0) Is Form Then
            'MsgBox("y")
            Dim form As Form = DirectCast(ConseptionPN.Controls(0), Form)
            form.WindowState = FormWindowState.Normal
            form.Size = ConseptionPN.ClientSize
            form.WindowState = FormWindowState.Maximized
        End If

    End Sub

    Private Sub Guna2CircleButton10_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton10.Click
        If valdi = "Admin" Or valdi = "Chef" Then
            If CurrentFrm IsNot Nothing Then
                CurrentFrm.Close()
            End If
            Dim f As Form = Home
            shawFrm(f)
            For i = 0 To 0
                shawFrm(f)
            Next

            Dim leftPanelWidth As Integer = Guna2GradientPanel1.Width ' Adjust this according to your left panel width
            Dim newWidth As Integer = Me.ClientSize.Width - leftPanelWidth
            Dim newHeight As Integer = Me.ClientSize.Height - (Guna2Panel1.Height)
            ConseptionPN.Size = New Size(newWidth, newHeight)
            If ConseptionPN.Controls.Count > 0 AndAlso TypeOf ConseptionPN.Controls(0) Is Form Then
                'MsgBox("y")
                Dim form As Form = DirectCast(ConseptionPN.Controls(0), Form)
                form.WindowState = FormWindowState.Normal
                form.Size = ConseptionPN.ClientSize
                form.WindowState = FormWindowState.Maximized
            End If
        End If
    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click, Guna2GradientButton1.Click
        Guna2CircleButton1.PerformClick()
    End Sub

    Private Sub Guna2GradientButton4_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click, Guna2GradientButton4.Click
        Guna2CircleButton4.PerformClick()
    End Sub

    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click, Guna2GradientButton2.Click
        Guna2CircleButton5.PerformClick()
    End Sub

    Private Sub PictureBox4_Click(sender As Object, e As EventArgs) Handles PictureBox4.Click, Guna2GradientButton3.Click
        Guna2CircleButton9.PerformClick()
    End Sub

    Private Sub Guna2CircleButton11_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton11.Click
        ' Path to the application you want to open \\PC3\Users\ppp\Desktop\Dossier de partage\برانم\PM\DrogrieCalc.exe
        Dim applicationPath As String = "\\pc3\Serveur DC\برانم\PM\DrogrieCalc.exe"

        Try
            Dim variable1 As String = Me.first

            Dim variable2 As String = valdi

            ' Write variables to a file
            Dim lines As String() = {variable1, variable2}
            File.WriteAllLines("\\pc3\Serveur DC\برانم\PM\sharedVariables.txt", lines)
            ' Start the application
            Process.Start(applicationPath)


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
End Class
