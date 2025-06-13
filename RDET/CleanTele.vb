Imports System.Security.Principal
Imports System.Threading.Tasks
Public Class CleanTele
    Public Sub UpdateListBox(message As String)
        If ListBox1.InvokeRequired Then
            ListBox1.Invoke(Sub() ListBox1.Items.Add(message))
            ListBox1.Invoke(Sub() ListBox1.SelectedIndex = ListBox1.Items.Count - 1)
            Label2.Invoke(Sub() Label2.Text = ListBox1.Items.Count)
        Else
            ListBox1.Items.Add(message)
            Label2.Text = ListBox1.Items.Count
            ListBox1.SelectedIndex = ListBox1.Items.Count - 1
        End If

    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ListBox1.Items.Clear()
        Task.Run(Sub()
                     FileTransferModule.Main_Execute(ListBox1, Label1.Text, AddressOf UpdateListBox)
                     MsgBox("Opération terminer avec success")
                 End Sub)

    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

        Dim folderBrowser As New FolderBrowserDialog()

        ' Set the description (optional)
        folderBrowser.Description = "Select a folder"

        ' Show the FolderBrowserDialog and check if the user clicked OK
        If folderBrowser.ShowDialog() = DialogResult.OK Then
            ' Get the selected folder path
            Dim selectedPath As String = folderBrowser.SelectedPath
            ' Display the selected folder path in a message box
            Label1.Text = selectedPath
            My.Settings.PathLocation = selectedPath
        End If
    End Sub

    Private Sub CleanTele_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Settings.PathLocation = "" Then
            Dim identity As WindowsIdentity = WindowsIdentity.GetCurrent()
            Dim username As String = Environment.UserName
            Label1.Text = $"C:\Users\{username}\Downloads"
        Else
            Label1.Text = My.Settings.PathLocation
        End If
    End Sub
End Class