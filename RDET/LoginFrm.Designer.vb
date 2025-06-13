<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LoginFrm
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Guna2ControlBox2 = New Guna.UI2.WinForms.Guna2ControlBox()
        Me.Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(Me.components)
        Me.Guna2Panel1 = New Guna.UI2.WinForms.Guna2Panel()
        Me.usernamtb = New Guna.UI2.WinForms.Guna2TextBox()
        Me.Guna2Panel2 = New Guna.UI2.WinForms.Guna2Panel()
        Me.passwoertb = New Guna.UI2.WinForms.Guna2TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Guna2Button1 = New Guna.UI2.WinForms.Guna2Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ConnectionP = New System.Windows.Forms.Panel()
        Me.Enter2 = New Guna.UI2.WinForms.Guna2Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Guna2Panel3 = New Guna.UI2.WinForms.Guna2Panel()
        Me.TbAppPass = New Guna.UI2.WinForms.Guna2TextBox()
        Me.Guna2CircleButton1 = New Guna.UI2.WinForms.Guna2CircleButton()
        Me.Guna2PictureBox3 = New Guna.UI2.WinForms.Guna2PictureBox()
        Me.Guna2PictureBox2 = New Guna.UI2.WinForms.Guna2PictureBox()
        Me.Guna2PictureBox4 = New Guna.UI2.WinForms.Guna2PictureBox()
        Me.Guna2PictureBox1 = New Guna.UI2.WinForms.Guna2PictureBox()
        Me.Guna2Panel1.SuspendLayout()
        Me.Guna2Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.ConnectionP.SuspendLayout()
        Me.Guna2Panel3.SuspendLayout()
        CType(Me.Guna2PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Guna2PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Guna2PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Guna2PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Guna2ControlBox2
        '
        Me.Guna2ControlBox2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Guna2ControlBox2.FillColor = System.Drawing.Color.Red
        Me.Guna2ControlBox2.HoverState.Parent = Me.Guna2ControlBox2
        Me.Guna2ControlBox2.IconColor = System.Drawing.Color.White
        Me.Guna2ControlBox2.Location = New System.Drawing.Point(751, 12)
        Me.Guna2ControlBox2.Name = "Guna2ControlBox2"
        Me.Guna2ControlBox2.ShadowDecoration.Parent = Me.Guna2ControlBox2
        Me.Guna2ControlBox2.Size = New System.Drawing.Size(37, 33)
        Me.Guna2ControlBox2.TabIndex = 1
        '
        'Guna2Elipse1
        '
        Me.Guna2Elipse1.BorderRadius = 20
        Me.Guna2Elipse1.TargetControl = Me
        '
        'Guna2Panel1
        '
        Me.Guna2Panel1.BorderColor = System.Drawing.Color.Black
        Me.Guna2Panel1.BorderRadius = 20
        Me.Guna2Panel1.BorderThickness = 1
        Me.Guna2Panel1.Controls.Add(Me.Guna2PictureBox2)
        Me.Guna2Panel1.FillColor = System.Drawing.Color.White
        Me.Guna2Panel1.Location = New System.Drawing.Point(12, 45)
        Me.Guna2Panel1.Name = "Guna2Panel1"
        Me.Guna2Panel1.ShadowDecoration.Parent = Me.Guna2Panel1
        Me.Guna2Panel1.Size = New System.Drawing.Size(388, 56)
        Me.Guna2Panel1.TabIndex = 1
        '
        'usernamtb
        '
        Me.usernamtb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.usernamtb.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dot
        Me.usernamtb.BorderThickness = 0
        Me.usernamtb.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.usernamtb.DefaultText = ""
        Me.usernamtb.DisabledState.BorderColor = System.Drawing.Color.FromArgb(CType(CType(208, Byte), Integer), CType(CType(208, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.usernamtb.DisabledState.FillColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.usernamtb.DisabledState.ForeColor = System.Drawing.Color.FromArgb(CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer))
        Me.usernamtb.DisabledState.Parent = Me.usernamtb
        Me.usernamtb.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer))
        Me.usernamtb.FocusedState.BorderColor = System.Drawing.Color.FromArgb(CType(CType(94, Byte), Integer), CType(CType(148, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.usernamtb.FocusedState.Parent = Me.usernamtb
        Me.usernamtb.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.usernamtb.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.usernamtb.HoverState.BorderColor = System.Drawing.Color.FromArgb(CType(CType(94, Byte), Integer), CType(CType(148, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.usernamtb.HoverState.Parent = Me.usernamtb
        Me.usernamtb.Location = New System.Drawing.Point(86, 52)
        Me.usernamtb.Margin = New System.Windows.Forms.Padding(4)
        Me.usernamtb.Name = "usernamtb"
        Me.usernamtb.PasswordChar = Global.Microsoft.VisualBasic.ChrW(0)
        Me.usernamtb.PlaceholderText = ""
        Me.usernamtb.SelectedText = ""
        Me.usernamtb.ShadowDecoration.Parent = Me.usernamtb
        Me.usernamtb.Size = New System.Drawing.Size(274, 44)
        Me.usernamtb.TabIndex = 1
        Me.usernamtb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Guna2Panel2
        '
        Me.Guna2Panel2.BorderColor = System.Drawing.Color.Black
        Me.Guna2Panel2.BorderRadius = 20
        Me.Guna2Panel2.BorderThickness = 1
        Me.Guna2Panel2.Controls.Add(Me.Guna2PictureBox3)
        Me.Guna2Panel2.FillColor = System.Drawing.Color.White
        Me.Guna2Panel2.Location = New System.Drawing.Point(12, 128)
        Me.Guna2Panel2.Name = "Guna2Panel2"
        Me.Guna2Panel2.ShadowDecoration.Parent = Me.Guna2Panel2
        Me.Guna2Panel2.Size = New System.Drawing.Size(388, 56)
        Me.Guna2Panel2.TabIndex = 2
        '
        'passwoertb
        '
        Me.passwoertb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.passwoertb.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dot
        Me.passwoertb.BorderThickness = 0
        Me.passwoertb.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.passwoertb.DefaultText = ""
        Me.passwoertb.DisabledState.BorderColor = System.Drawing.Color.FromArgb(CType(CType(208, Byte), Integer), CType(CType(208, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.passwoertb.DisabledState.FillColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.passwoertb.DisabledState.ForeColor = System.Drawing.Color.FromArgb(CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer))
        Me.passwoertb.DisabledState.Parent = Me.passwoertb
        Me.passwoertb.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer))
        Me.passwoertb.FocusedState.BorderColor = System.Drawing.Color.FromArgb(CType(CType(94, Byte), Integer), CType(CType(148, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.passwoertb.FocusedState.Parent = Me.passwoertb
        Me.passwoertb.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.passwoertb.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.passwoertb.HoverState.BorderColor = System.Drawing.Color.FromArgb(CType(CType(94, Byte), Integer), CType(CType(148, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.passwoertb.HoverState.Parent = Me.passwoertb
        Me.passwoertb.Location = New System.Drawing.Point(83, 137)
        Me.passwoertb.Margin = New System.Windows.Forms.Padding(5, 5, 5, 5)
        Me.passwoertb.Name = "passwoertb"
        Me.passwoertb.PasswordChar = Global.Microsoft.VisualBasic.ChrW(35)
        Me.passwoertb.PlaceholderText = ""
        Me.passwoertb.SelectedText = ""
        Me.passwoertb.ShadowDecoration.Parent = Me.passwoertb
        Me.passwoertb.Size = New System.Drawing.Size(274, 41)
        Me.passwoertb.TabIndex = 2
        Me.passwoertb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI Black", 26.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Gray
        Me.Label1.Location = New System.Drawing.Point(151, 31)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(205, 47)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Connexion"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Gray
        Me.Label2.Location = New System.Drawing.Point(12, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(266, 17)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Entrer nom d'utilisateur et mot de passe :"
        '
        'Guna2Button1
        '
        Me.Guna2Button1.BackColor = System.Drawing.Color.Transparent
        Me.Guna2Button1.BorderRadius = 20
        Me.Guna2Button1.CheckedState.Parent = Me.Guna2Button1
        Me.Guna2Button1.CustomImages.Parent = Me.Guna2Button1
        Me.Guna2Button1.Font = New System.Drawing.Font("Segoe UI", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Guna2Button1.ForeColor = System.Drawing.Color.White
        Me.Guna2Button1.HoverState.Parent = Me.Guna2Button1
        Me.Guna2Button1.Location = New System.Drawing.Point(25, 219)
        Me.Guna2Button1.Name = "Guna2Button1"
        Me.Guna2Button1.ShadowDecoration.Parent = Me.Guna2Button1
        Me.Guna2Button1.Size = New System.Drawing.Size(364, 69)
        Me.Guna2Button1.TabIndex = 3
        Me.Guna2Button1.Text = "Entrer"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.passwoertb)
        Me.Panel1.Controls.Add(Me.usernamtb)
        Me.Panel1.Controls.Add(Me.Guna2Button1)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.Guna2Panel2)
        Me.Panel1.Controls.Add(Me.Guna2Panel1)
        Me.Panel1.Location = New System.Drawing.Point(0, 81)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(407, 299)
        Me.Panel1.TabIndex = 7
        '
        'ConnectionP
        '
        Me.ConnectionP.Controls.Add(Me.Enter2)
        Me.ConnectionP.Controls.Add(Me.Label3)
        Me.ConnectionP.Controls.Add(Me.Guna2Panel3)
        Me.ConnectionP.Location = New System.Drawing.Point(0, 81)
        Me.ConnectionP.Name = "ConnectionP"
        Me.ConnectionP.Size = New System.Drawing.Size(407, 299)
        Me.ConnectionP.TabIndex = 7
        Me.ConnectionP.Visible = False
        '
        'Enter2
        '
        Me.Enter2.BorderRadius = 20
        Me.Enter2.CheckedState.Parent = Me.Enter2
        Me.Enter2.CustomImages.Parent = Me.Enter2
        Me.Enter2.Font = New System.Drawing.Font("Segoe UI", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Enter2.ForeColor = System.Drawing.Color.White
        Me.Enter2.HoverState.Parent = Me.Enter2
        Me.Enter2.Location = New System.Drawing.Point(25, 219)
        Me.Enter2.Name = "Enter2"
        Me.Enter2.ShadowDecoration.Parent = Me.Enter2
        Me.Enter2.Size = New System.Drawing.Size(364, 69)
        Me.Enter2.TabIndex = 5
        Me.Enter2.Text = "Entrer"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Gray
        Me.Label3.Location = New System.Drawing.Point(12, 16)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(283, 17)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Please enter Application password for enter:"
        '
        'Guna2Panel3
        '
        Me.Guna2Panel3.BorderColor = System.Drawing.Color.Black
        Me.Guna2Panel3.BorderRadius = 20
        Me.Guna2Panel3.BorderThickness = 1
        Me.Guna2Panel3.Controls.Add(Me.TbAppPass)
        Me.Guna2Panel3.Controls.Add(Me.Guna2PictureBox4)
        Me.Guna2Panel3.FillColor = System.Drawing.Color.White
        Me.Guna2Panel3.Location = New System.Drawing.Point(15, 99)
        Me.Guna2Panel3.Name = "Guna2Panel3"
        Me.Guna2Panel3.ShadowDecoration.Parent = Me.Guna2Panel3
        Me.Guna2Panel3.Size = New System.Drawing.Size(388, 56)
        Me.Guna2Panel3.TabIndex = 3
        '
        'TbAppPass
        '
        Me.TbAppPass.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TbAppPass.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dot
        Me.TbAppPass.BorderThickness = 0
        Me.TbAppPass.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.TbAppPass.DefaultText = ""
        Me.TbAppPass.DisabledState.BorderColor = System.Drawing.Color.FromArgb(CType(CType(208, Byte), Integer), CType(CType(208, Byte), Integer), CType(CType(208, Byte), Integer))
        Me.TbAppPass.DisabledState.FillColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.TbAppPass.DisabledState.ForeColor = System.Drawing.Color.FromArgb(CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer))
        Me.TbAppPass.DisabledState.Parent = Me.TbAppPass
        Me.TbAppPass.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer), CType(CType(138, Byte), Integer))
        Me.TbAppPass.FocusedState.BorderColor = System.Drawing.Color.FromArgb(CType(CType(94, Byte), Integer), CType(CType(148, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.TbAppPass.FocusedState.Parent = Me.TbAppPass
        Me.TbAppPass.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TbAppPass.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.TbAppPass.HoverState.BorderColor = System.Drawing.Color.FromArgb(CType(CType(94, Byte), Integer), CType(CType(148, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.TbAppPass.HoverState.Parent = Me.TbAppPass
        Me.TbAppPass.Location = New System.Drawing.Point(67, 9)
        Me.TbAppPass.Margin = New System.Windows.Forms.Padding(5, 5, 5, 5)
        Me.TbAppPass.Name = "TbAppPass"
        Me.TbAppPass.PasswordChar = Global.Microsoft.VisualBasic.ChrW(35)
        Me.TbAppPass.PlaceholderText = ""
        Me.TbAppPass.SelectedText = ""
        Me.TbAppPass.ShadowDecoration.Parent = Me.TbAppPass
        Me.TbAppPass.Size = New System.Drawing.Size(274, 41)
        Me.TbAppPass.TabIndex = 3
        Me.TbAppPass.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Guna2CircleButton1
        '
        Me.Guna2CircleButton1.CheckedState.Parent = Me.Guna2CircleButton1
        Me.Guna2CircleButton1.CustomImages.Parent = Me.Guna2CircleButton1
        Me.Guna2CircleButton1.FillColor = System.Drawing.Color.Transparent
        Me.Guna2CircleButton1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Guna2CircleButton1.ForeColor = System.Drawing.Color.White
        Me.Guna2CircleButton1.HoverState.Parent = Me.Guna2CircleButton1
        Me.Guna2CircleButton1.Image = Global.RDET.My.Resources.Resources.equalizer
        Me.Guna2CircleButton1.Location = New System.Drawing.Point(12, 402)
        Me.Guna2CircleButton1.Name = "Guna2CircleButton1"
        Me.Guna2CircleButton1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle
        Me.Guna2CircleButton1.ShadowDecoration.Parent = Me.Guna2CircleButton1
        Me.Guna2CircleButton1.Size = New System.Drawing.Size(37, 36)
        Me.Guna2CircleButton1.TabIndex = 6
        '
        'Guna2PictureBox3
        '
        Me.Guna2PictureBox3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Guna2PictureBox3.BackColor = System.Drawing.Color.Transparent
        Me.Guna2PictureBox3.Image = Global.RDET.My.Resources.Resources.lock
        Me.Guna2PictureBox3.Location = New System.Drawing.Point(13, 9)
        Me.Guna2PictureBox3.Name = "Guna2PictureBox3"
        Me.Guna2PictureBox3.ShadowDecoration.Parent = Me.Guna2PictureBox3
        Me.Guna2PictureBox3.Size = New System.Drawing.Size(46, 38)
        Me.Guna2PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.Guna2PictureBox3.TabIndex = 2
        Me.Guna2PictureBox3.TabStop = False
        '
        'Guna2PictureBox2
        '
        Me.Guna2PictureBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Guna2PictureBox2.BackColor = System.Drawing.Color.Transparent
        Me.Guna2PictureBox2.Image = Global.RDET.My.Resources.Resources.user1
        Me.Guna2PictureBox2.Location = New System.Drawing.Point(13, 8)
        Me.Guna2PictureBox2.Name = "Guna2PictureBox2"
        Me.Guna2PictureBox2.ShadowDecoration.Parent = Me.Guna2PictureBox2
        Me.Guna2PictureBox2.Size = New System.Drawing.Size(46, 38)
        Me.Guna2PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.Guna2PictureBox2.TabIndex = 2
        Me.Guna2PictureBox2.TabStop = False
        '
        'Guna2PictureBox4
        '
        Me.Guna2PictureBox4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Guna2PictureBox4.BackColor = System.Drawing.Color.Transparent
        Me.Guna2PictureBox4.Image = Global.RDET.My.Resources.Resources.lock
        Me.Guna2PictureBox4.Location = New System.Drawing.Point(13, 9)
        Me.Guna2PictureBox4.Name = "Guna2PictureBox4"
        Me.Guna2PictureBox4.ShadowDecoration.Parent = Me.Guna2PictureBox4
        Me.Guna2PictureBox4.Size = New System.Drawing.Size(46, 38)
        Me.Guna2PictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.Guna2PictureBox4.TabIndex = 2
        Me.Guna2PictureBox4.TabStop = False
        '
        'Guna2PictureBox1
        '
        Me.Guna2PictureBox1.BackColor = System.Drawing.Color.Transparent
        Me.Guna2PictureBox1.Image = Global.RDET.My.Resources.Resources.Computer_login_pana
        Me.Guna2PictureBox1.Location = New System.Drawing.Point(406, 51)
        Me.Guna2PictureBox1.Name = "Guna2PictureBox1"
        Me.Guna2PictureBox1.ShadowDecoration.Parent = Me.Guna2PictureBox1
        Me.Guna2PictureBox1.Size = New System.Drawing.Size(382, 362)
        Me.Guna2PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.Guna2PictureBox1.TabIndex = 2
        Me.Guna2PictureBox1.TabStop = False
        '
        'LoginFrm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.Guna2CircleButton1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Guna2ControlBox2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ConnectionP)
        Me.Controls.Add(Me.Guna2PictureBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "LoginFrm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "LoginFrm"
        Me.Guna2Panel1.ResumeLayout(False)
        Me.Guna2Panel2.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ConnectionP.ResumeLayout(False)
        Me.ConnectionP.PerformLayout()
        Me.Guna2Panel3.ResumeLayout(False)
        CType(Me.Guna2PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Guna2PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Guna2PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Guna2PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Guna2ControlBox2 As Guna.UI2.WinForms.Guna2ControlBox
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents Guna2PictureBox1 As Guna.UI2.WinForms.Guna2PictureBox
    Friend WithEvents Guna2Panel2 As Guna.UI2.WinForms.Guna2Panel
    Friend WithEvents Guna2Panel1 As Guna.UI2.WinForms.Guna2Panel
    Friend WithEvents Guna2PictureBox2 As Guna.UI2.WinForms.Guna2PictureBox
    Friend WithEvents Guna2PictureBox3 As Guna.UI2.WinForms.Guna2PictureBox
    Friend WithEvents passwoertb As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents usernamtb As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Guna2Button1 As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents ConnectionP As Panel
    Friend WithEvents Enter2 As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents Label3 As Label
    Friend WithEvents Guna2Panel3 As Guna.UI2.WinForms.Guna2Panel
    Friend WithEvents TbAppPass As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Guna2PictureBox4 As Guna.UI2.WinForms.Guna2PictureBox
    Friend WithEvents Guna2CircleButton1 As Guna.UI2.WinForms.Guna2CircleButton
    Friend WithEvents Panel1 As Panel
End Class
