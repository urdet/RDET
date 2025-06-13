Imports System.IO
Imports DevExpress
Imports DevExpress.XtraReports.UI
Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient

Public Class rapports
    Public Shared connectiono As MySqlConnection = Class1.conectiondmc

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Dim rp As New JournalDC
        rp.ShowRibbonPreview
    End Sub

    Private Sub Guna2GradientButton1_DoubleClick(sender As Object, e As EventArgs) Handles Guna2GradientButton1.DoubleClick
        Dim rp As New JournalDC
        rp.ShowRibbonDesigner
    End Sub



    Private Sub Guna2CircleButton2_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton2.Click
        Guna2CircleButton2.Visible = False
    End Sub

    Private Sub Guna2CircleButton5_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton5.Click
        tbId.Text = ""
        TextBox2.Text = ""
        Fexten.Text = ""
    End Sub

    Private Sub Guna2GradientButton3_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton3.Click
        Panel1.Visible = True
        'Try

        Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT `storedfiles`.`Id`, `storedfiles`.`Name`, `storedfiles`.`datex`, `storedfiles`.`File_extension` FROM `damanecash`.`storedfiles`", 1, "1")
        'Guna2DataGridView1.Rows.Clear()
        Guna2DataGridView1.DataSource = dt
        'Catch ex As Exception
        '    MsgBox(ex.Message) '
        'End Try
    End Sub
    Private Sub UploadExcelToDatabase(filePath As String)
        Try
            ' Read the .xls file into a byte array
            Dim fileBytes As Byte() = File.ReadAllBytes(filePath)

            ' Connect to the MySQL database

            If connectiono.State = ConnectionState.Closed Then
                connectiono.Open()
            End If

            ' Insert the file data into a table (assuming a table with a BLOB column named 'file_data')
            Dim query As String = "INSERT INTO my_table (file_data) VALUES (@fileData)"
            Using command As New MySqlCommand(query, connectiono)
                ' Add parameter for the file data
                command.Parameters.AddWithValue("@fileData", fileBytes)

                ' Execute the query
                command.ExecuteNonQuery()
            End Using

            ' Close the database connection
            connectiono.Close()


            MessageBox.Show("File uploaded successfully to MySQL database.")
        Catch ex As Exception
            MessageBox.Show("Error uploading file to MySQL database: " & ex.Message)
        End Try
    End Sub
    Private Sub Guna2CircleButton3_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton3.Click
        If tbId.Text = "" And TextBox2.Text <> "" Then
            If connectiono.State = ConnectionState.Closed Then
                connectiono.Open()
            End If
            Dim fileBytes As Byte() = File.ReadAllBytes(TextBox2.Text)
            Dim comman As New MySqlCommand("INSERT INTO `damanecash`.`storedfiles`(`Name`,`File_data`,`datex`,File_extension) VALUES (@Name, @File_data, @datex,@File_extension) ;", connectiono)
            comman.Parameters.AddWithValue("@Name", TextBox3.Text)
            comman.Parameters.AddWithValue("@File_data", fileBytes)
            comman.Parameters.AddWithValue("@datex", DateTime.Now)
            comman.Parameters.AddWithValue("@File_extension", Fexten.Text)
            comman.ExecuteNonQuery()
            connectiono.Close()
        ElseIf TextBox2.Text <> "" Then
            If connectiono.State = ConnectionState.Closed Then
                connectiono.Open()
            End If
            Dim fileBytes As Byte() = File.ReadAllBytes(TextBox2.Text)
            Dim comman As New MySqlCommand("UPDATE `damanecash`.`storedfiles` SET `Name` = @namex, `File_data` = @FileData, `datex` = @Datex, File_extension = @File_extension WHERE `Id` = @id;", connectiono)
            comman.Parameters.AddWithValue("@id", CInt(tbId.Text))
            comman.Parameters.AddWithValue("@namex", TextBox3.Text)
            comman.Parameters.AddWithValue("@FileData", fileBytes)
            comman.Parameters.AddWithValue("@Datex", DateTime.Now)
            comman.Parameters.AddWithValue("@File_extension", Fexten.Text)
            comman.ExecuteNonQuery()
            connectiono.Close()
        End If
        Guna2GradientButton3.PerformClick()
    End Sub

    Private Sub Guna2CircleButton4_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton4.Click
        If tbId.Text <> "" Then
            If connectiono.State = ConnectionState.Closed Then
                connectiono.Open()
            End If

            Dim comman As New MySqlCommand("DELETE FROM `damanecash`.`storedfiles` WHERE Id = @id;", connectiono)
            comman.Parameters.AddWithValue("@id", CInt(tbId.Text))
            comman.ExecuteNonQuery()
            connectiono.Close()
        End If
        Guna2GradientButton3.PerformClick()
    End Sub

    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click

        OpenFileDialog1.Title = "Select a File"

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then

            TextBox2.Text = OpenFileDialog1.FileName
            ' Get the name and extension of the selected file
            Dim fileName As String = Path.GetFileName(TextBox2.Text)
            Dim fileExtension As String = Path.GetExtension(TextBox2.Text)
            Fexten.Text = fileName
            ' Display the name and extension in a message box
        End If
    End Sub

    Private Sub Guna2CircleButton6_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton6.Click
        Panel1.Visible = False
    End Sub

    Private Sub Guna2CircleButton7_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton7.Click
        Panel2.Visible = False
    End Sub

    Private Sub Guna2GradientButton4_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton4.Click
        If connectiono.State = ConnectionState.Closed Then
            connectiono.Open()
        End If
        Try

            Dim command2 As New MySqlCommand("SELECT * FROM `damanecash`.`storedfiles`;", connectiono)
            Dim adapter2 As New MySqlDataAdapter(command2)
            Dim table2 As New DataTable()
            Panel2.Visible = True
            adapter2.Fill(table2)
            FlowLayoutPanel1.Controls.Clear()
            If table2.Rows.Count = 0 Then
                'MsgBox("Error")
            Else
                For ligne As Integer = 0 To table2.Rows.Count - 1
                    Dim colonne As Integer = 1
                    Dim idFile As String = table2.Rows(ligne)(0).ToString
                    Dim nomBouton As String = table2.Rows(ligne)(colonne).ToString
                    Dim bouton As New Guna2GradientButton()
                    bouton.Name = table2.Rows(ligne)(0).ToString
                    bouton.Text = nomBouton
                    bouton.Font = New Font("Segoe UI", 14)
                    bouton.BorderRadius = 10
                    bouton.FillColor = Color.Turquoise
                    bouton.FillColor2 = Color.CadetBlue
                    AddHandler bouton.Click, Sub(s, args) BoutonClique(idFile)
                    FlowLayoutPanel1.Controls.Add(bouton)
                Next
            End If
            connectiono.Close()
        Catch ex As Exception
            connectiono.Close()
        End Try
    End Sub
    Private Sub BoutonClique(id As Integer)
        ' Connection string to connect to the MySQL database

        ' Query to retrieve the file data from the database
        Dim query As String = $"SELECT File_data, File_extension FROM `damanecash`.`storedfiles` WHERE Id = {id};"
        If connectiono.State = ConnectionState.Closed Then
            connectiono.Open()
        End If

        ' Create a temporary file path
        Dim dt As DataTable = utilities.GetSomeValuesdmc("SELECT File_data, File_extension FROM `damanecash`.`storedfiles`", id, "Id")
        Dim tempFilePath As String = Path.Combine(Path.GetTempPath(), dt.Rows(0)(1))
        connectiono.Close()

        Try
            ' Connect to the MySQL database
            If connectiono.State = ConnectionState.Closed Then
                connectiono.Open()
            End If

            ' Execute the query to retrieve the file data
            Using command As New MySqlCommand(query, connectiono)
                Using reader As MySqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        ' Get the file data from the database
                        Dim fileData As Byte() = DirectCast(reader("File_data"), Byte())

                        ' Write the file data to a temporary file
                        File.WriteAllBytes(tempFilePath, fileData)

                        ' Open the temporary file using its associated program
                        Process.Start(tempFilePath)
                    Else
                        MessageBox.Show("No file found in the database.")
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error opening file from database: " & ex.Message)
        End Try
    End Sub

    Private Sub rapports_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Guna2DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellClick
        Try
            Dim index As Integer = Guna2DataGridView1.SelectedRows(0).Index
            tbId.Text = Guna2DataGridView1.Rows(index).Cells(0).Value
            TextBox3.Text = Guna2DataGridView1.Rows(index).Cells(1).Value
            Fexten.Text = Guna2DataGridView1.Rows(index).Cells(3).Value
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        If tbId.Text Is Nothing Or tbId.Text = "" Then
            Dim filePath As String = TextBox2.Text

            If System.IO.File.Exists(filePath) Then
                Process.Start(filePath)
            Else
                MessageBox.Show("The file does not exist.")
            End If
        Else
            BoutonClique(CInt(tbId.Text))
        End If
    End Sub

    Private Sub tbId_TextChanged(sender As Object, e As EventArgs) Handles tbId.TextChanged
        If tbId.Text = "" Then
            Guna2CircleButton1.Image = My.Resources.televerser_un_fichier
        Else
            Guna2CircleButton1.Image = My.Resources.actualiser_les_donnees
        End If
    End Sub

    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        Dim rp As New XtraReport1
        rp.ShowRibbonPreview
    End Sub

    Private Sub Guna2GradientButton5_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton5.Click
        Dim rp As New XtraReportDetaill
        rp.ShowRibbonPreview
    End Sub

    Private Sub ReportDesignerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReportDesignerToolStripMenuItem.Click

        Dim rp As New XtraReport2
        rp.ShowRibbonDesigner
    End Sub
End Class