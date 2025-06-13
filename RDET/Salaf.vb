Public Class Salaf
    Public isSalaf As Boolean = True
    Public isglobal As Boolean = False
    Public CompteID As Integer
    Private Sub Salaf_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If isSalaf Then
            Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT DISTINCT Invester from salafdetail", "1", "1")
            For i = 0 To dt.Rows.Count - 1
                Dim cont As New SalafDetailControl
                cont.Invester = dt.Rows(i)(0)
                FlowLayoutPanel1.Controls.Add(cont)
            Next
        Else
            Dim dt As DataTable
            If isglobal = True Then
                dt = utilities.GetSomeValuesdmc("SELECT DISTINCT Person FROM damanecash.detail_comptes_traffic", $"{CompteID}", "CompteID")
            Else
                dt = utilities.GetSomeValuesdmc("SELECT DISTINCT c.Person FROM damanecash.detail_comptes_traffic as c, nonpayedetail as a ", $"{CompteID} and c.Person = a.NomEtPrenom", "c.CompteID")
            End If
            For i = 0 To dt.Rows.Count - 1
                Dim cont As New TrafficPerson
                cont.CompteID = CompteID
                cont.PersonName = dt.Rows(i)(0)
                FlowLayoutPanel1.Controls.Add(cont)
            Next
        End If

    End Sub
End Class