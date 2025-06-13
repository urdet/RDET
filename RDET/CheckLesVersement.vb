
Public Class CheckLesVersement
    Dim ss As Integer = 1

    Private Function IsRangeChecked(dataGridView As DataGridView, dRow As Integer, Colun As Integer) As Boolean
        Dim row As Integer = dRow
        Dim col As Integer = Colun
        Dim checkBoxCell As DataGridViewCheckBoxCell = TryCast(dataGridView(col, row), DataGridViewCheckBoxCell)
        If checkBoxCell IsNot Nothing AndAlso checkBoxCell.Value IsNot Nothing AndAlso DirectCast(checkBoxCell.Value, Boolean) Then
            Return True

        Else
            Return False

        End If
    End Function
    Public Sub fillslds()
        Dim sld1 As Decimal = utilities.getsld(12)
        lbBr.Text = sld1.ToString

        Dim sld21 As Decimal = utilities.getsld(3)

        lbNVR.Text = (-(sld1 - sld21)).ToString

        Dim sldNonVersé As Decimal = 0
        Dim dt As DataTable = utilities.detailbankVersment(0)
        For i = 0 To dt.Rows.Count - 1
            sldNonVersé += dt.Rows(i)(2)
        Next
        lnNVC.Text = sldNonVersé.ToString
        Dim sldVersé As Decimal = 0
        Dim dt1 As DataTable = utilities.detailbankVersment(1)
        For i = 0 To dt1.Rows.Count - 1
            sldVersé += dt1.Rows(i)(2)
        Next
        Label6.Text = sldVersé.ToString

        lbFb.Text = (CDec(lnNVC.Text) - CDec(lbNVR.Text)).ToString
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Me.Close()
    End Sub

    Private Sub DGV_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGV.CellClick
        Try
            If e.ColumnIndex = 3 Then
                For i = 0 To DGV.Rows.Count - 1
                    Dim b As Boolean = IsRangeChecked(DGV, i, 3)
                    Dim s As String
                    If b = True Then
                        s = "1"
                    Else
                        s = "0"
                    End If
                    utilities.UpdateSomeValuesdmc($"UPDATE `damanecash`.`tansactionscomptes` SET `versé` = '{s}' ", DGV.Rows(i).Cells(0).Value, "ID")
                Next
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Private Sub GDTP2_ValueChanged(sender As Object, e As EventArgs)

    End Sub
    Public Sub filldata1()
        If ss = 2 Or ss = 1 Then
            ss = 1
            Dim dt As DataTable = utilities.detailbankVersment(ss)
            DGV.Rows.Clear()
            For i = 0 To dt.Rows.Count - 1
                If IsDBNull(dt.Rows(i)(1)) Or dt.Rows(i)(1) Is Nothing Then
                    DGV.Rows.Add(dt.Rows(i)(0), "Commission", dt.Rows(i)(2), CBool(CInt(dt.Rows(i)(4))), dt.Rows(i)(3))

                Else
                    DGV.Rows.Add(dt.Rows(i)(0), dt.Rows(i)(1), dt.Rows(i)(2), CBool(CInt(dt.Rows(i)(4))), dt.Rows(i)(3))
                End If
            Next
            Guna2Panel2.Visible = True
        ElseIf ss = 2 Or ss = 0 Then
            ss = 0
            Dim dt As DataTable = utilities.detailbankVersment(ss)
            DGV.Rows.Clear()
            For i = 0 To dt.Rows.Count - 1
                If IsDBNull(dt.Rows(i)(1)) Or dt.Rows(i)(1) Is Nothing Then
                    DGV.Rows.Add(dt.Rows(i)(0), "Commission", dt.Rows(i)(2), CBool(CInt(dt.Rows(i)(4))), dt.Rows(i)(3))

                Else
                    DGV.Rows.Add(dt.Rows(i)(0), dt.Rows(i)(1), dt.Rows(i)(2), CBool(CInt(dt.Rows(i)(4))), dt.Rows(i)(3))
                End If
            Next
            Guna2Panel2.Visible = True
        End If
    End Sub
    Public Sub filldata0()
        If ss = 2 Or ss = 1 Then
            ss = 0
            Dim dt As DataTable = utilities.detailbankVersment(ss)
            DGV.Rows.Clear()
            For i = 0 To dt.Rows.Count - 1
                If IsDBNull(dt.Rows(i)(1)) Or dt.Rows(i)(1) Is Nothing Then
                    DGV.Rows.Add(dt.Rows(i)(0), "Commission", dt.Rows(i)(2), CBool(CInt(dt.Rows(i)(4))), dt.Rows(i)(3))

                Else
                    DGV.Rows.Add(dt.Rows(i)(0), dt.Rows(i)(1), dt.Rows(i)(2), CBool(CInt(dt.Rows(i)(4))), dt.Rows(i)(3))
                End If
            Next
            Guna2Panel2.Visible = True
        ElseIf ss = 2 Or ss = 0 Then
            ss = 1
            Dim dt As DataTable = utilities.detailbankVersment(ss)
            DGV.Rows.Clear()
            For i = 0 To dt.Rows.Count - 1
                If IsDBNull(dt.Rows(i)(1)) Or dt.Rows(i)(1) Is Nothing Then
                    DGV.Rows.Add(dt.Rows(i)(0), "Commission", dt.Rows(i)(2), CBool(CInt(dt.Rows(i)(4))), dt.Rows(i)(3))

                Else
                    DGV.Rows.Add(dt.Rows(i)(0), dt.Rows(i)(1), dt.Rows(i)(2), CBool(CInt(dt.Rows(i)(4))), dt.Rows(i)(3))
                End If
            Next
            Guna2Panel2.Visible = True
        End If
    End Sub
    Private Sub CheckLesVersement_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        ' Set the form's start position to manual
        Me.StartPosition = FormStartPosition.Manual

        ' Calculate the position for the bottom left corner
        Dim screenRect As Rectangle = Screen.PrimaryScreen.WorkingArea
        Dim bottomLeftPoint As New Point(0, screenRect.Height - Me.Height)
        ' Set the form's location
        Me.Location = bottomLeftPoint

        filldata0()
        fillslds()
    End Sub

    Private Sub Guna2Button2_Click_1(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        DGV.Refresh()
        For i = 0 To DGV.Rows.Count - 1
            Dim b As Boolean = IsRangeChecked(DGV, i, 3)
            Dim s As String
            If b = True Then
                s = "1"
            Else
                s = "0"
            End If
            utilities.UpdateSomeValuesdmc($"UPDATE `damanecash`.`tansactionscomptes` SET `versé` = '{s}' ", DGV.Rows(i).Cells(0).Value, "ID")
        Next
        If ss = 0 Then
            Dim dt As DataTable = utilities.detailbankVersment(ss)
            DGV.Rows.Clear()
            For i = 0 To dt.Rows.Count - 1
                If IsDBNull(dt.Rows(i)(1)) Or dt.Rows(i)(1) Is Nothing Then
                    DGV.Rows.Add(dt.Rows(i)(0), "Commission", dt.Rows(i)(2), CBool(CInt(dt.Rows(i)(4))), dt.Rows(i)(3))

                Else
                    DGV.Rows.Add(dt.Rows(i)(0), dt.Rows(i)(1), dt.Rows(i)(2), CBool(CInt(dt.Rows(i)(4))), dt.Rows(i)(3))
                End If
            Next
            Guna2Panel2.Visible = True
        ElseIf ss = 1 Then
            Dim dt As DataTable = utilities.detailbankVersment(ss)
            DGV.Rows.Clear()
            For i = 0 To dt.Rows.Count - 1
                If IsDBNull(dt.Rows(i)(1)) Or dt.Rows(i)(1) Is Nothing Then
                    DGV.Rows.Add(dt.Rows(i)(0), "Commission", dt.Rows(i)(2), CBool(CInt(dt.Rows(i)(4))), dt.Rows(i)(3))

                Else
                    DGV.Rows.Add(dt.Rows(i)(0), dt.Rows(i)(1), dt.Rows(i)(2), CBool(CInt(dt.Rows(i)(4))), dt.Rows(i)(3))
                End If
            Next
            Guna2Panel2.Visible = True

        End If
        fillslds()
    End Sub

    Private Sub Guna2Panel3_Click(sender As Object, e As EventArgs) Handles Guna2Panel3.Click, lnNVC.Click, Label4.Click
        ss = 0
        Guna2Button2.PerformClick()
        Guna2Panel6.BorderColor = Color.FromArgb(0, 192, 0)
        Guna2Panel3.BorderColor = Color.DodgerBlue
    End Sub

    Private Sub Guna2Panel6_Click(sender As Object, e As EventArgs) Handles Guna2Panel6.Click, Label7.Click, Label6.Click
        ss = 1
        Guna2Button2.PerformClick()
        Guna2Panel3.BorderColor = Color.FromArgb(0, 192, 0)
        Guna2Panel6.BorderColor = Color.DodgerBlue
    End Sub

    Private Sub Guna2Panel3_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel3.Paint

    End Sub
End Class