<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ComptesBox
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Guna2GradientPanel1 = New Guna.UI2.WinForms.Guna2GradientPanel()
        Me.Guna2CircleButton1 = New Guna.UI2.WinForms.Guna2CircleButton()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Comp = New System.Windows.Forms.Label()
        Me.CN = New System.Windows.Forms.Label()
        Me.Sld = New System.Windows.Forms.Label()
        Me.Guna2GradientPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Guna2GradientPanel1
        '
        Me.Guna2GradientPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Guna2GradientPanel1.BorderColor = System.Drawing.Color.Purple
        Me.Guna2GradientPanel1.BorderRadius = 15
        Me.Guna2GradientPanel1.BorderStyle = System.Drawing.Drawing2D.DashStyle.Custom
        Me.Guna2GradientPanel1.BorderThickness = 2
        Me.Guna2GradientPanel1.Controls.Add(Me.Guna2CircleButton1)
        Me.Guna2GradientPanel1.Controls.Add(Me.Label1)
        Me.Guna2GradientPanel1.Controls.Add(Me.Comp)
        Me.Guna2GradientPanel1.Controls.Add(Me.CN)
        Me.Guna2GradientPanel1.Controls.Add(Me.Sld)
        Me.Guna2GradientPanel1.FillColor = System.Drawing.Color.Transparent
        Me.Guna2GradientPanel1.FillColor2 = System.Drawing.Color.Transparent
        Me.Guna2GradientPanel1.Location = New System.Drawing.Point(12, 12)
        Me.Guna2GradientPanel1.Name = "Guna2GradientPanel1"
        Me.Guna2GradientPanel1.ShadowDecoration.Parent = Me.Guna2GradientPanel1
        Me.Guna2GradientPanel1.Size = New System.Drawing.Size(239, 115)
        Me.Guna2GradientPanel1.TabIndex = 0
        '
        'Guna2CircleButton1
        '
        Me.Guna2CircleButton1.CheckedState.Parent = Me.Guna2CircleButton1
        Me.Guna2CircleButton1.CustomImages.Parent = Me.Guna2CircleButton1
        Me.Guna2CircleButton1.FillColor = System.Drawing.Color.Transparent
        Me.Guna2CircleButton1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Guna2CircleButton1.ForeColor = System.Drawing.Color.White
        Me.Guna2CircleButton1.HoverState.Parent = Me.Guna2CircleButton1
        Me.Guna2CircleButton1.Image = Global.RDET.My.Resources.Resources.enregistrer
        Me.Guna2CircleButton1.ImageSize = New System.Drawing.Size(25, 25)
        Me.Guna2CircleButton1.Location = New System.Drawing.Point(11, 78)
        Me.Guna2CircleButton1.Name = "Guna2CircleButton1"
        Me.Guna2CircleButton1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle
        Me.Guna2CircleButton1.ShadowDecoration.Parent = Me.Guna2CircleButton1
        Me.Guna2CircleButton1.Size = New System.Drawing.Size(30, 30)
        Me.Guna2CircleButton1.TabIndex = 3
        Me.Guna2CircleButton1.Visible = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(47, 76)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(0, 14)
        Me.Label1.TabIndex = 2
        '
        'Comp
        '
        Me.Comp.AutoSize = True
        Me.Comp.BackColor = System.Drawing.Color.Transparent
        Me.Comp.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Comp.Location = New System.Drawing.Point(45, 93)
        Me.Comp.Name = "Comp"
        Me.Comp.Size = New System.Drawing.Size(55, 13)
        Me.Comp.TabIndex = 1
        Me.Comp.Text = "Company"
        '
        'CN
        '
        Me.CN.AutoSize = True
        Me.CN.BackColor = System.Drawing.Color.Transparent
        Me.CN.Font = New System.Drawing.Font("Segoe UI", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CN.Location = New System.Drawing.Point(41, 0)
        Me.CN.Name = "CN"
        Me.CN.Size = New System.Drawing.Size(148, 30)
        Me.CN.TabIndex = 1
        Me.CN.Text = "Compte Name"
        Me.CN.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'Sld
        '
        Me.Sld.BackColor = System.Drawing.Color.Transparent
        Me.Sld.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Sld.Font = New System.Drawing.Font("Tahoma", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Sld.Location = New System.Drawing.Point(0, 0)
        Me.Sld.Name = "Sld"
        Me.Sld.Size = New System.Drawing.Size(239, 115)
        Me.Sld.TabIndex = 1
        Me.Sld.Text = "912896.90"
        Me.Sld.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ComptesBox
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.Guna2GradientPanel1)
        Me.Name = "ComptesBox"
        Me.Size = New System.Drawing.Size(266, 139)
        Me.Guna2GradientPanel1.ResumeLayout(False)
        Me.Guna2GradientPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Guna2GradientPanel1 As Guna.UI2.WinForms.Guna2GradientPanel
    Friend WithEvents Sld As Label
    Friend WithEvents Comp As Label
    Friend WithEvents CN As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Guna2CircleButton1 As Guna.UI2.WinForms.Guna2CircleButton
End Class
