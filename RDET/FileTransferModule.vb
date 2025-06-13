Imports System.IO
Module FileTransferModule
    Public Sub Main_Execute(list As ListBox, src As String, updateUI As Action(Of String))
        Dim sourceFolder As String = src

        ' Define a list of transfer operations
        Dim operations As (String, String, String)() = {
            ($"\\pc3\Serveur DC\Archive\RSU", "_maj_", ".pdf"),
            ($"\\pc3\Serveur DC\Archive\ASD", "Récépissé_demande_", ".pdf"),
            ($"\\pc3\Serveur DC\Archive\RSU", "_inscription_", ".pdf"),
            ($"\\pc3\Serveur DC\Archive\Reçu", "Reçu", ".pdf"),
            ($"\\pc3\Serveur DC\Archive\Num Cnss", "Avis_Sort_", ".pdf"),
            ($"\\pc3\Serveur DC\Archive\Taawidat CNSS", "MAD_CNSS_", ".pdf"),
            ($"\\pc3\Serveur DC\Archive\Wasl idaa CNSS", "Recepisse_Depot_", ".pdf"),
            ($"\\pc3\Serveur DC\Archive\Mandat", "invoice-", ".pdf"),
            ($"\\pc3\Serveur DC\Archive\Certificat Cnss", "Certificat", ".pdf"),
            ($"\\pc3\Serveur DC\Archive\Compte DP", "FICHE_CONTRACTUEL_", ".pdf"),
            ($"\\pc3\Serveur DC\Archive\CIN", "Numérisation_", ""),
            ($"\\pc3\Serveur DC\Archive\Archive scolaire", "DemandeInscription_", ".pdf")
        }
        ' Perform file transfer for each operation
        For Each op In operations
            TransferFilteredFiles(sourceFolder, op.Item1, op.Item2, op.Item3, list, updateUI)
        Next
    End Sub

    Sub TransferFilteredFiles(sourceFolder As String, destinationFolder As String, keyword As String, fileExtension As String, list As ListBox, updateUI As Action(Of String))
        ' Ensure the destination folder exists
        If Not Directory.Exists(destinationFolder) Then
            Directory.CreateDirectory(destinationFolder)
        End If

        ' Iterate over all files in the source folder
        For Each filePath In Directory.GetFiles(sourceFolder)
            Dim fileName As String = Path.GetFileName(filePath)

            ' Check if it matches the criteria
            If fileName.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase) AndAlso fileName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 Then
                ' Move the file to the destination folder
                Dim uniqueFileName As String = GetUniqueFileName(destinationFolder, fileName)
                Dim destFilePath As String = Path.Combine(destinationFolder, uniqueFileName)

                File.Move(filePath, destFilePath)
                updateUI($"Transferred {uniqueFileName}")


            End If
        Next
    End Sub

    Function GetUniqueFileName(destFolder As String, fileName As String) As String
        Dim baseName As String = Path.GetFileNameWithoutExtension(fileName)
        Dim ext As String = Path.GetExtension(fileName)
        Dim newFileName As String = fileName
        Dim counter As Integer = 1

        While File.Exists(Path.Combine(destFolder, newFileName))
            newFileName = $"{baseName}_{counter}{ext}"
            counter += 1
        End While

        Return newFileName
    End Function
End Module
