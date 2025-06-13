Imports MySql.Data.MySqlClient
Imports Guna.UI2.WinForms
Public Class HisabatGestion
    Public Shared connectiono As MySqlConnection = Class1.conectiondmc
    Public Shared connectiono1 As MySqlConnection = Class1.conectionscr

    Sub fillComp(id As Integer)
        connectiono1.Open()
        Try

            Dim command2 As New MySqlCommand("SELECT * FROM companies;", connectiono1)
            Dim adapter2 As New MySqlDataAdapter(command2)
            Dim table2 As New DataTable()

            adapter2.Fill(table2)
            If table2.Rows.Count = 0 Then
                'MsgBox("Error")
            Else
                'Guna2DataGridView1.DataSource = table2
                FlowLayoutPanel1.Controls.Clear()

                For ligne As Integer = 0 To table2.Rows.Count - 1
                    Dim colonne As Integer = 1
                    Dim nomBouton As String = table2.Rows(ligne)(colonne).ToString
                    Dim bouton As New Guna2CheckBox()
                    bouton.Name = "Check_" & table2.Rows(ligne)(0).ToString
                    bouton.Tag = table2.Rows(ligne)(0).ToString
                    bouton.Text = nomBouton
                    'If if is the id
                    Dim command1 As New MySqlCommand($"SELECT * FROM hisabat WHERE ID = {id};", connectiono1)
                    Dim adapter1 As New MySqlDataAdapter(command1)
                    Dim table1 As New DataTable()

                    adapter1.Fill(table1)
                    If table1.Rows(0)(5).ToString.Contains(table2.Rows(ligne)(0).ToString) Then
                        bouton.Checked = True
                    End If
                    'AddHandler bouton.CheckedChanged, AddressOf CheckBox_CheckedChanged
                    FlowLayoutPanel1.Controls.Add(bouton)
                Next
            End If
            connectiono1.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
            connectiono1.Close()
        End Try
    End Sub
    Sub filldata()
        If connectiono1.State = ConnectionState.Open Then
        Else
            connectiono1.Open()
        End If

        Try

            Dim command2 As New MySqlCommand("SELECT * FROM hisabat;", connectiono1)
            Dim adapter2 As New MySqlDataAdapter(command2)
            Dim table2 As New DataTable()

            adapter2.Fill(table2)
            If table2.Rows.Count = 0 Then
                'MsgBox("Error")
            Else
                Guna2DataGridView1.DataSource = table2


            End If
            connectiono1.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
            connectiono1.Close()
        End Try
    End Sub
    Private Sub HisabatGestion_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'fillComp()
        TrackClass.AddUserTLine(Form1.idd, Form1.first, "Open", "Open Form", $"Gestion of hisabat", "", DateTime.Now)
        Guna2DateTimePicker1.Value = Date.Today
        filldata()
    End Sub

    Private Sub RefrechToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefrechToolStripMenuItem.Click

    End Sub
    Private Sub CheckBox_CheckedChanged(typ As String)
        connectiono1.Open()
        Try
            Dim checkedCheckBoxes As New List(Of String)

            ' Iterate through checkboxes to find checked ones
            For Each ctrl As Control In FlowLayoutPanel1.Controls
                If TypeOf ctrl Is Guna2CheckBox Then
                    Dim checkbox As Guna2CheckBox = DirectCast(ctrl, Guna2CheckBox)
                    If checkbox.Checked Then
                        checkedCheckBoxes.Add(checkbox.Tag.ToString())
                    End If
                End If
            Next
            Dim str As String = $"[{String.Join("],[", checkedCheckBoxes)}]"
            If typ = "Add" Then
                Dim formattedDateTime As String = Guna2DateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss")
                Dim comman As New MySqlCommand("INSERT INTO hisabat (CompteName, `Solde`, `Description`, Datex, `Company`) VALUES" &
                                               $" ('{tbName.Text}', {CDec(tbSld.Text)}, '{tbDesc.Text}', '{formattedDateTime}', '{str}');", connectiono1)
                comman.ExecuteNonQuery()
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Add", "Add item", $"Add hisab with name {tbName.Text}.", "", DateTime.Now)

            ElseIf typ = "Update" Then
                Dim comman As New MySqlCommand($"UPDATE `hisabat` SET CompteName='{tbName.Text}', `Solde`={CDec(tbSld.Text)}, `Description`='{tbDesc.Text}', Datex='{Guna2DateTimePicker1.Value.ToString("yyyy-MM-dd HH-mm-ss")}', `Company`='{str}' WHERE ID={CInt(tbID.Text)};", connectiono1)
                comman.ExecuteNonQuery()
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Update", "Update item", $"Update hisab id = {tbID.Text}, name {tbName.Text}.", "", DateTime.Now)
            End If
            filldata()
        Catch ex As Exception
            MsgBox(ex.ToString())
        End Try

        connectiono1.Close()
    End Sub


    Private Sub ReloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReloadToolStripMenuItem.Click
        Dim f As Form = Me
        f.TopLevel = False
        f.WindowState = FormWindowState.Maximized
        f.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        f.Visible = True
        Form1.ConseptionPN.Controls.Clear()
        Form1.ConseptionPN.Controls.Add(f)
    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Savebtn.Click
        CheckBox_CheckedChanged(Savebtn.Tag.ToString())
        AutoOperations.UComptes1(Guna2DateTimePicker1.Value)
        Guna2ShadowPanel2.Visible = False
        Guna2ShadowPanel1.Visible = True
    End Sub

    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        Guna2ShadowPanel1.Visible = False
        Guna2ShadowPanel2.Visible = True
        Savebtn.Tag = "Update"
        Dim dt As DataTable = utilities.GetSomeValuesscr("SELECT * FROM hisabat", Guna2DataGridView1.SelectedRows(0).Cells(0).Value, "ID")
        tbID.Text = dt.Rows(0)(0).ToString()
        tbName.Text = dt.Rows(0)(1).ToString()
        tbSld.Text = dt.Rows(0)(2).ToString()
        tbDesc.Text = dt.Rows(0)(3).ToString()
        'Guna2DateTimePicker1.Value = dt.Rows(0)(4).ToString()
        sld.Text = dt.Rows(0)(2).ToString()
        Dim ds As DataTable = utilities.GetSomeValuesdmc("SELECT * FROM services", Guna2DataGridView1.SelectedRows(0).Cells(0).Value, $"Para2 = {Guna2DataGridView1.SelectedRows(0).Cells(0).Value} OR Para3")
        ServicesDGV.DataSource = ds
        fillComp(Guna2DataGridView1.SelectedRows(0).Cells(0).Value)
    End Sub

    Private Sub Guna2CircleButton2_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton2.Click
        Guna2ShadowPanel1.Visible = False
        Guna2ShadowPanel2.Visible = True
        Savebtn.Tag = "Add"
        tbID.Text = ""
        tbID.Enabled = False
        tbName.Text = ""
        tbName.Enabled = True
        tbSld.Text = ""
        tbSld.Enabled = True
        tbDesc.Text = ""
        tbDesc.Enabled = True
        'Dim ds As DataTable = utilities.GetSomeValuesdmc("SELECT * FROM services", Guna2DataGridView1.SelectedRows(0).Cells(0).Value, $" Para2 = {Guna2DataGridView1.SelectedRows(0).Cells(0).Value} OR Para3")
        ' ServicesDGV.DataSource = ds
        ' fillComp(Guna2DataGridView1.SelectedRows(0).Cells(0).Value)
    End Sub

    Private Sub Guna2ShadowPanel2_Paint(sender As Object, e As PaintEventArgs) Handles Guna2ShadowPanel2.Paint

    End Sub

    Private Sub Label7_Click(sender As Object, e As EventArgs) Handles sld.Click

    End Sub

    Private Sub btnMdfFnLn_Click(sender As Object, e As EventArgs) Handles btnMdfFnLn.Click
        tbName.Enabled = True
        btnMdfFnLn.Visible = False
    End Sub

    Private Sub BtnMdfInfo_Click(sender As Object, e As EventArgs) Handles BtnMdfInfo.Click
        tbDesc.Enabled = True
        BtnMdfInfo.Enabled = False

    End Sub

    Private Sub BtnMdfComp_Click(sender As Object, e As EventArgs) Handles BtnMdfComp.Click
        tbSld.Enabled = True
        BtnMdfComp.Enabled = False
    End Sub

    Private Sub Guna2GradientButton1_Click_1(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Guna2ShadowPanel1.Visible = True
        Guna2ShadowPanel2.Visible = False
        Savebtn.Tag = ""
        Savebtn.Visible = True
    End Sub

    Private Sub Guna2CircleButton3_Click(sender As Object, e As EventArgs) 
        Guna2ShadowPanel1.Visible = False
        Guna2ShadowPanel2.Visible = True
        Savebtn.Visible = False
    End Sub

    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        Me.Close()
    End Sub

    Private Sub Guna2CircleButton4_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton4.Click
        connectiono1.Open()
        Dim ea As MsgBoxResult = MsgBox($"Do you want to delete item No: {Guna2DataGridView1.SelectedRows(0).Cells(0).Value}.", MsgBoxStyle.YesNo)
        If ea = MsgBoxResult.Yes Then
            Try

                Dim comman As New MySqlCommand("DELETE FROM hisabat WHERE ID = @uid;", connectiono1)
                comman.Parameters.AddWithValue("@uid", Guna2DataGridView1.SelectedRows(0).Cells(0).Value)
                comman.ExecuteNonQuery()
                TrackClass.AddUserTLine(Form1.idd, Form1.first, "Delete", "Delete item", $"deelete hisab of name {Guna2DataGridView1.SelectedRows(0).Cells(1).Value.ToString} and id = {Guna2DataGridView1.SelectedRows(0).Cells(0).Value}.", "", DateTime.Now)
                filldata()
                MsgBox("Delete Successfuly")
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End If
        connectiono1.Close()
    End Sub

    Private Sub Guna2CircleButton3_Click_1(sender As Object, e As EventArgs) Handles Guna2CircleButton3.Click
        Salaf.CompteID = Guna2DataGridView1.SelectedRows(0).Cells(0).Value
        Salaf.isSalaf = False
        Salaf.isglobal = True
        Salaf.Show()
    End Sub
End Class