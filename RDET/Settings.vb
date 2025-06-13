Public Class Settings
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ColorDialog1.ShowDialog() = DialogResult.OK Then
            ' Get the selected color
            Dim selectedColor As Color = ColorDialog1.Color

            Label5.Text = selectedColor.ToString()
            My.Settings.FirstColor = selectedColor
            Form1.Guna2GradientPanel1.FillColor = My.Settings.FirstColor
            Form1.Guna2GradientPanel1.FillColor2 = My.Settings.SecondColor
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ColorDialog1.ShowDialog() = DialogResult.OK Then
            ' Get the selected color
            Dim selectedColor As Color = ColorDialog1.Color

            Label6.Text = selectedColor.ToString()
            My.Settings.SecondColor = selectedColor
            Form1.Guna2GradientPanel1.FillColor = My.Settings.FirstColor
            Form1.Guna2GradientPanel1.FillColor2 = My.Settings.SecondColor
        End If
    End Sub

    Private Sub Settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label6.Text = My.Settings.SecondColor.ToString
        Label5.Text = My.Settings.FirstColor.ToString

    End Sub
End Class