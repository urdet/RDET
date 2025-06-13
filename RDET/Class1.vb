Imports System
Imports System.Data
Imports MySql.Data.MySqlClient
Public Class Class1
    Public Shared Function conectionscr() As MySqlConnection
        Dim server As String = My.Settings.Server
        Dim userid As String = My.Settings.UserID
        Dim pwd As String = My.Settings.Password
        Dim port As String = My.Settings.Port
        Dim connstring As String = $"server={server};port={port};userid={userid};password={pwd};database=security_db"
        Dim conn As MySqlConnection = New MySqlConnection(connstring)
        Return conn
    End Function
    Public Shared Function conectiondmc() As MySqlConnection
        Dim server As String = My.Settings.Server
        Dim userid As String = My.Settings.UserID
        Dim pwd As String = My.Settings.Password
        Dim port As String = My.Settings.Port
        Dim connstring As String = $"server={server};port={port};userid={userid};password={pwd};database=damanecash"
        Dim conn As MySqlConnection = New MySqlConnection(connstring)
        Return conn
    End Function
    Public Shared Function conectiontrk() As MySqlConnection
        Dim server As String = My.Settings.Server
        Dim userid As String = My.Settings.UserID
        Dim pwd As String = My.Settings.Password
        Dim port As String = My.Settings.Port
        Dim connstring As String = $"server={server};port={port};userid={userid};password={pwd};database=trackerlines"
        Dim conn As MySqlConnection = New MySqlConnection(connstring)
        Return conn
    End Function
    Public Shared Function conectionreg() As MySqlConnection
        Dim server As String = My.Settings.Server
        Dim userid As String = My.Settings.UserID
        Dim pwd As String = My.Settings.Password
        Dim port As String = My.Settings.Port
        Dim connstring As String = $"server={server};port={port};userid={userid};password={pwd};database=resgister_db"
        Dim conn As MySqlConnection = New MySqlConnection(connstring)
        Return conn
    End Function
End Class
