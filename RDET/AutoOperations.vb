Imports MySql.Data.MySqlClient
Public Class AutoOperations
    Public Shared consq As MySqlConnection = Class1.conectionscr
    Public Shared condmc As MySqlConnection = Class1.conectiondmc

    Public Shared Sub UComptes(CompteID As Integer, solde As Decimal, datea As DateTime)
        'Dim relType As String = utilities.getRelationOfCompte()
        Dim d1 As DataTable = utilities.GetSomeValuesdmc("SELECT * FROM compteshistoriquedata ", 2, $"`datex` LIKE '%{datea.ToString("yyyy-MM-dd")}%' OR `datex` ")
        If d1.Rows.Count > 0 Then
        Else
            Dim d2 As DataTable = utilities.GetSomeValuesscr("SELECT * FROM `hisabat` ", 1, $"1")
            If d2.Rows.Count > 0 Then
                For i = 0 To d2.Rows.Count - 1
                    Dim comman As New MySqlCommand("INSERT INTO `compteshistoriquedata` (`CompteID`, `CompteName`, `Solde`, `datex`) VALUES (@CompteID, @CompteName, @Solde, @datex);", condmc)
                    comman.Parameters.AddWithValue("@CompteID", d2.Rows(i)(0))
                    comman.Parameters.AddWithValue("@CompteName", d2.Rows(i)(1).ToString)
                    comman.Parameters.AddWithValue("@Solde", d2.Rows(i)(2))
                    comman.Parameters.AddWithValue("@datex", datea)
                    condmc.Open()
                    comman.ExecuteNonQuery()
                    condmc.Close()
                Next
            Else
                MsgBox("Error to get data from hisabat")
            End If
        End If
        Try
            Dim command1 As New MySqlCommand($"UPDATE compteshistoriquedata SET Solde={solde} WHERE `CompteID` = {CompteID} AND `datex` LIKE '%{datea.ToString("yyyy-MM-dd")}%'", condmc)
            condmc.Open()
            command1.ExecuteNonQuery()
            condmc.Close()
        Catch ex As Exception
        Finally
            condmc.Close()
        End Try
    End Sub
    Public Shared Sub UHIstComp(CompteID As Integer, solde As Decimal, datea As DateTime)
        'Dim relType As String = utilitiegetRelationOfCompte()
        Dim d1 As DataTable = utilities.GetSomeValuesdmc("SELECT `CompteID`, `CompteName`, `Solde` FROM compteshistoriquedata ", 2, $"`datex` LIKE '%{datea.ToString("yyyy-MM-dd")}%' OR `datex` ")
        If d1.Rows.Count > 0 Then

            For i = 0 To d1.Rows.Count - 1
                Dim comman As New MySqlCommand("INSERT INTO `compteshistoriquedata` (`CompteID`, `CompteName`, `Solde`, `datex`) VALUES (@CompteID, @CompteName, @Solde, @datex);", condmc)
                comman.Parameters.AddWithValue("@CompteID", d1.Rows(i)(0))
                comman.Parameters.AddWithValue("@CompteName", d1.Rows(i)(1).ToString)
                comman.Parameters.AddWithValue("s.@Solde", d1.Rows(i)(2))
                comman.Parameters.AddWithValue("@datex", datea)
                condmc.Open()
                comman.ExecuteNonQuery()
                condmc.Close()
            Next
        Else

        End If
        Try
            Dim command1 As New MySqlCommand($"UPDATE compteshistoriquedata SET Solde={solde} WHERE `CompteID` = {CompteID} AND `datex` LIKE '%{datea.ToString("yyyy-MM-dd")}%'", condmc)
            condmc.Open()
            command1.ExecuteNonQuery()
            condmc.Close()
        Catch ex As Exception
        Finally
            condmc.Close()
        End Try
    End Sub


    Public Shared Sub UComptes1(datea As DateTime)
        Dim d2 As DataTable = utilities.GetSomeValuesscr("SELECT * FROM `hisabat` ", 1, $"1")
        If d2.Rows.Count > 0 Then
            For i = 0 To d2.Rows.Count - 1
                UComptes(d2.Rows(i)(0), d2.Rows(i)(2), datea)
            Next
        End If
    End Sub
End Class
