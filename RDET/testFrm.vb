Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient

Public Class testFrm
    Public Shared condmc As MySqlConnection = Class1.conectiondmc
    Private Sub testFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        condmc.Open()
        Dim com As New MySqlCommand
        com.CommandType = CommandType.StoredProcedure
        com.CommandText = "trans1"
        com.Connection = condmc
        com.Parameters.AddWithValue("FComp", 1)
        com.Parameters.AddWithValue("para1", DateAndTime.Now.AddDays(-1).ToString("yyyy-MM-dd"))
        com.ExecuteNonQuery()
        condmc.Close()
    End Sub
End Class