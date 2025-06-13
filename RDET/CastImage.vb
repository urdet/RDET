Imports System
Imports System.Data
Imports System.IO
Imports MySql.Data.MySqlClient


Public Class CastImage
    Public Shared connectiona As MySqlConnection = Class1.conectionscr
    Public Shared connectiono As MySqlConnection = Class1.conectiondmc
    Public Shared Sub ReadImg(usernawm As String)
        connectiona.Open()
        Dim Sql As String = "SELECT * FROM users WHERE Usernam='" & usernawm & "'"
        Dim cmd As New MySqlCommand(Sql, connectiona)
        Dim dr As MySqlDataReader
        dr = cmd.ExecuteReader()
        dr.Read()
        Try
            Dim imagebytes As Byte() = CType(dr("image"), Byte())
            Using ms As New IO.MemoryStream(imagebytes)
                Form1.Guna2CirclePictureBox1.Image = Image.FromStream(ms)
                Form1.Guna2CirclePictureBox1.SizeMode = PictureBoxSizeMode.Zoom
            End Using
        Catch es As Exception
            MsgBox(es.ToString)
        End Try
        connectiona.Close()
    End Sub
    Public Shared Sub ReadImgID(id As Integer, pictureb As PictureBox)
        connectiona.Open()
        Dim Sql As String = "SELECT * FROM users WHERE ID='" & id & "'"
        Dim cmd As New MySqlCommand(Sql, connectiona)
        Dim dr As MySqlDataReader
        dr = cmd.ExecuteReader()
        dr.Read()
        Try
            Dim imagebytes As Byte() = CType(dr("image"), Byte())
            Using ms As New IO.MemoryStream(imagebytes)
                pictureb.Image = Image.FromStream(ms)
                pictureb.SizeMode = PictureBoxSizeMode.Zoom
            End Using
        Catch es As Exception
            MsgBox(es.ToString)
        End Try
        connectiona.Close()
    End Sub
    Public Shared Sub ReadImgIDPr(id As Integer, tabl As String, pr As Object, pictureb As PictureBox)
        If connectiono.State = ConnectionState.Closed Then
            connectiono.Open()
        End If

        Dim Sql As String = $"SELECT * FROM {tabl} WHERE ID='" & id & "'"
        Dim cmd As New MySqlCommand(Sql, connectiono)
        Dim dr As MySqlDataReader
        dr = cmd.ExecuteReader()
        dr.Read()
        Try
            Dim imagebytes As Byte() = CType(dr(pr), Byte())
            Using ms As New IO.MemoryStream(imagebytes)
                pictureb.Image = Image.FromStream(ms)
                pictureb.SizeMode = PictureBoxSizeMode.Zoom
            End Using
        Catch es As Exception
            'MsgBox(es.ToString)
        End Try
        connectiono.Close()
    End Sub
    Public Shared Function ImageToByteArray(ByVal img As Image) As Byte()
        Try
            Dim ms As New MemoryStream()
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
            Return ms.ToArray()
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class
