Public Class connectionSettings
    Private Sub Frm_load(sender As Object, e As EventArgs) Handles MyBase.Load
        serverTB.Text = My.Settings.Server
        portTB.Text = My.Settings.Port
        UserNameTB.Text = My.Settings.UserID
        PWDTB.Text = My.Settings.Password
        TBAppPass.Text = My.Settings.AppPass
    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Try
            My.Settings.Server = serverTB.Text
            My.Settings.Port = portTB.Text
            My.Settings.UserID = UserNameTB.Text
            My.Settings.Password = PWDTB.Text
            My.Settings.AppPass = TBAppPass.Text
            MsgBox("Complete successfuly! ")
            Me.Close()
        Catch ex As Exception
            MsgBox(ex.ToString())
        End Try
    End Sub

    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        Me.Close()
    End Sub

End Class