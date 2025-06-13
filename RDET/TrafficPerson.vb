Imports System.Math
Public Class TrafficPerson
    Public PersonName As String
    Public CompteID As Integer

    Function FillBody()
        Try
            Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT * FROM damanecash.detail_comptes_traffic", $"'{PersonName}' AND CompteID ={CompteID}", "Person")
            FlowLayoutPanel1.Controls.Clear()
            Dim total As Decimal = 0
            For i = 0 To dt.Rows.Count - 1
                Dim mtn As Decimal = dt.Rows(i)(3)
                Dim dat As DateTime = dt.Rows(i)(4)
                total += mtn
                Dim fontColor As Color
                Dim sg As String = "+"
                If mtn < 0 Then
                    sg = "- "
                    fontColor = Color.Green
                ElseIf mtn > 0 Then
                    sg = "+"
                    fontColor = Color.Red
                Else
                    fontColor = Color.Black
                End If
                Dim lb As New Label
                lb.Width = 180
                lb.Height = 35
                lb.AutoSize = False
                lb.ForeColor = fontColor
                lb.Text = sg & " " & Math.Abs(mtn)
                ToolTip1.SetToolTip(lb, dat.ToString("yyyy-MM-dd"))
                lb.Font = New Font("Segoe UI", 20, FontStyle.Bold)
                FlowLayoutPanel1.Controls.Add(lb)
            Next

            Return total.ToString("0.00")
        Catch ex As Exception
            Return 0.ToString("0.00")
            MsgBox(ex.Message)
        End Try
    End Function
    Private Sub TrafficPerson_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Label1.Text = PersonName
            Dim getTotal = FillBody()
            Label2.Text = getTotal
            If getTotal < 0 Then
                Guna2GradientPanel1.FillColor2 = Color.Honeydew
                Guna2GradientPanel1.BorderColor = Color.Green
            ElseIf getTotal > 0 Then
                Guna2GradientPanel1.FillColor2 = Color.MistyRose
                Guna2GradientPanel1.BorderColor = Color.Red
            Else
                Guna2GradientPanel1.FillColor2 = Color.LightCyan
                Guna2GradientPanel1.BorderColor = Color.Blue
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub FlowLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles FlowLayoutPanel1.Paint

    End Sub
End Class
