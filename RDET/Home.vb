Imports DevExpress.XtraCharts
Public Class Home


    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        ChartControl1.Series.Clear()
        Dim todayDate As String = DateTimePicker1.Value.ToString("yyyy-MM-dd")
        Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT * FROM damanecash.services", "1", "1")
        Dim barSeriesIN As New Series("IN", ViewType.Bar)
        Dim barSeriesOUT As New Series("OUT", ViewType.Bar)
        For i = 0 To dt.Rows.Count - 1
            Dim argumentName As String = dt.Rows(i)(1)
            Dim dt1 As DataTable = utilities.GetSomeValuesdmc("SELECT Sum(Mtn) FROM damanecash.transactionsservices ", $"'{argumentName}' and datex like '%{todayDate}%' and Typ = 'IN'", "Service")
            Dim valueIN As Decimal
            Dim valueOUT As Decimal
            Try
                If IsDBNull(dt1.Rows(0)(0)) = True Then
                    valueIN = 0
                Else
                    valueIN = dt1.Rows(0)(0)
                End If
            Catch ex As Exception

            End Try
            Dim dt2 As DataTable = utilities.GetSomeValuesdmc("SELECT Sum(Mtn) FROM damanecash.transactionsservices ", $"'{argumentName}' and datex like '%{todayDate}%' and Typ = 'OUT'", "Service")

            Try
                If IsDBNull(dt2.Rows(0)(0)) = True Then
                    valueOUT = 0
                Else
                    valueOUT = dt2.Rows(0)(0)
                End If
            Catch ex As Exception

            End Try

            barSeriesIN.Points.Add(New SeriesPoint(argumentName, valueIN))
            barSeriesOUT.Points.Add(New SeriesPoint(argumentName, valueOUT))


        Next
        ChartControl1.Series.Add(barSeriesIN)
        ChartControl1.Series.Add(barSeriesOUT)

        ' Optionally, customize the chart's appearance
        Dim diagram As XYDiagram = CType(ChartControl1.Diagram, XYDiagram)

        diagram.AxisX.Title.Text = "Services"
        diagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True
        diagram.AxisY.Title.Text = "Valeurs"
        diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True

        ' Refresh the chart
        ChartControl1.Refresh()
    End Sub

    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        ChartControl1.Series.Clear()
        Dim todayDate As String = DateTimePicker1.Value.ToString("yyyy-MM")
        Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT * FROM damanecash.services", "1", "1")
        Dim barSeriesIN As New Series("IN", ViewType.Bar)
        Dim barSeriesOUT As New Series("OUT", ViewType.Bar)
        For i = 0 To dt.Rows.Count - 1
            Dim argumentName As String = dt.Rows(i)(1)
            Dim dt1 As DataTable = utilities.GetSomeValuesdmc("SELECT Sum(Mtn) FROM damanecash.transactionsservices ", $"'{argumentName}' and datex like '%{todayDate}%' and Typ = 'IN'", "Service")
            Dim valueIN As Decimal
            Dim valueOUT As Decimal
            Try
                If IsDBNull(dt1.Rows(0)(0)) = True Then
                    valueIN = 0
                Else
                    valueIN = dt1.Rows(0)(0)
                End If
            Catch ex As Exception

            End Try
            Dim dt2 As DataTable = utilities.GetSomeValuesdmc("SELECT Sum(Mtn) FROM damanecash.transactionsservices ", $"'{argumentName}' and datex like '%{todayDate}%' and Typ = 'OUT'", "Service")

            Try
                If IsDBNull(dt2.Rows(0)(0)) = True Then
                    valueOUT = 0
                Else
                    valueOUT = dt2.Rows(0)(0)
                End If
            Catch ex As Exception

            End Try

            barSeriesIN.Points.Add(New SeriesPoint(argumentName, valueIN))
            barSeriesOUT.Points.Add(New SeriesPoint(argumentName, valueOUT))


        Next
        ChartControl1.Series.Add(barSeriesIN)
        ChartControl1.Series.Add(barSeriesOUT)

        ' Optionally, customize the chart's appearance
        Dim diagram As XYDiagram = CType(ChartControl1.Diagram, XYDiagram)
        diagram.AxisX.Title.Text = "Services"
        diagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True
        diagram.AxisY.Title.Text = "Valeurs"
        diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True

        ' Refresh the chart
        ChartControl1.Refresh()
    End Sub
End Class