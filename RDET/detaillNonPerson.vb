Public Class detaillNonPerson
    Public person As String
    Public compteID As Integer
    Private Sub detaillNonPerson_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim p As New TrafficPerson
        p.PersonName = person
        p.CompteID = compteID
        Panel1.Controls.Clear()
        Panel1.Controls.Add(p)
    End Sub
End Class