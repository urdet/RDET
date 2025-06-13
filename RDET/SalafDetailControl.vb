Public Class SalafDetailControl
    '' 204
    Public Invester As String

    Private Sub SalafDetailControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label1.Text = Invester
        Dim DataTb As DataTable = utilities.GetSomeValuesdmc("select * from salafdetail", $"'{Invester}'", "Invester")
        Dim t As New List(Of String)
        Dim s As New List(Of Decimal)
        Dim d As New List(Of DateTime)
        For i = 0 To DataTb.Rows.Count - 1
            t.Add(DataTb.Rows(i)(2))
            s.Add(DataTb.Rows(i)(3))
            d.Add(DataTb.Rows(i)(4))
        Next


        If s.Count = t.Count Then
            Dim total As Decimal = 0
            For i = 0 To t.Count - 1
                Dim re = t.Item(i)
                Dim colr As Color
                If re = "+" Then
                    colr = Color.Green
                    total += s.Item(i)
                ElseIf re = "- " Then
                    colr = Color.Red
                    total -= s.Item(i)
                End If

                Dim lb As New Label
                lb.Width = 200
                lb.Height = 35
                lb.AutoSize = False
                lb.ForeColor = colr
                lb.Text = re & " " & s.Item(i)
                ToolTip1.SetToolTip(lb, d.Item(i).ToString)
                lb.Font = New Font("Segoe UI", 20, FontStyle.Bold)
                FlowLayoutPanel1.Controls.Add(lb)
            Next
            Label2.Text = total
        End If
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub
End Class
