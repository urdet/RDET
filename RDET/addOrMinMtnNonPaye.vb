Public Class addOrMinMtnNonPaye
    Public typ As String = "+"
    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        If typ = "+" Then
            CaiseCalcule.Guna2DataGridView1.Rows(CaiseCalcule.Guna2DataGridView1.SelectedRows(0).Index).Cells(2).Value += CDec(TextBox1.Text)
            TextBox1.Clear()
            Me.Close()
        Else
            CaiseCalcule.Guna2DataGridView1.Rows(CaiseCalcule.Guna2DataGridView1.SelectedRows(0).Index).Cells(2).Value -= CDec(TextBox1.Text)
            TextBox1.Clear()
            Me.Close()
        End If
    End Sub
End Class