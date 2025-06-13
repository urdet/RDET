Imports MySql.Data
Imports MySql
Imports System.Data
Imports MySql.Data.MySqlClient

Public Class TrackClass
    Public Shared connection As MySqlConnection = Class1.conectiontrk
    Public Shared Sub AddServiceLine(uid As Integer, uname As String, Actions As String, sID As Integer, serv As String,
                                     frComp As Integer, toComp As Integer, mtn As Decimal, descr As String, datec As DateTime)
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        Try
            Dim cmd As New MySqlCommand()
            cmd.Connection = connection
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "AddServiceLine2"
            cmd.Parameters.AddWithValue("uid", uid)
            cmd.Parameters.AddWithValue("uname", uname)
            cmd.Parameters.AddWithValue("Actions", Actions)
            cmd.Parameters.AddWithValue("sID", sID)
            cmd.Parameters.AddWithValue("serv", serv)
            cmd.Parameters.AddWithValue("frComp", frComp)
            cmd.Parameters.AddWithValue("toComp", toComp)
            cmd.Parameters.AddWithValue("descr", descr)
            cmd.Parameters.AddWithValue("Mtn", mtn.ToString.Replace(",", "."))
            cmd.Parameters.AddWithValue("datec", datec)
            cmd.ExecuteNonQuery()
            ' MsgBox()
        Catch ex As Exception
            MsgBox(ex.Message.ToString())
        End Try

        connection.Close()

    End Sub
    Public Shared Sub AddCompteTLine(uid As Integer, uName As String, FrComp As Integer, ToComp As Integer, transID As Integer, Mtn As Decimal, Descr As String, datec As DateTime)
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        Try
            Dim cmd As New MySqlCommand()
            cmd.Connection = connection
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "AddTransLine2"
            cmd.Parameters.AddWithValue("uid", uid)
            cmd.Parameters.AddWithValue("uName", uName)
            cmd.Parameters.AddWithValue("FrComp", FrComp)
            cmd.Parameters.AddWithValue("ToComp", ToComp)
            cmd.Parameters.AddWithValue("transID", transID)
            cmd.Parameters.AddWithValue("Descr", Descr)
            cmd.Parameters.AddWithValue("Mtn", Mtn.ToString.Replace(",", "."))
            cmd.Parameters.AddWithValue("datec", datec)
            cmd.ExecuteNonQuery()
            ' MsgBox()
        Catch ex As Exception
            MsgBox(ex.Message.ToString())
        End Try

        connection.Close()
    End Sub

    Public Shared Sub AddUserTLine(uId As Integer, NUser As String, uAction As String, uTopic As String, uDescription As String, uDescription2 As String, Datex As DateTime)
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        Try
            Dim cmd As New MySqlCommand()
            cmd.Connection = connection
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "AddUserLine2"
            cmd.Parameters.AddWithValue("uid", uId)
            cmd.Parameters.AddWithValue("NUser", NUser)
            cmd.Parameters.AddWithValue("uAction", uAction)
            cmd.Parameters.AddWithValue("uTopic", uTopic)
            cmd.Parameters.AddWithValue("uDescription", uDescription)
            cmd.Parameters.AddWithValue("uDescription2", uDescription2)
            cmd.Parameters.AddWithValue("Datex", Datex)
            cmd.ExecuteNonQuery()
            ' MsgBox()
        Catch ex As Exception
            MsgBox(ex.Message.ToString())
        End Try

        connection.Close()
    End Sub
End Class
