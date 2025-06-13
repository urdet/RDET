<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class RP1
    Inherits DevExpress.XtraReports.UI.XtraReport

    'XtraReport overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Designer
    'It can be modified using the Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim StoredProcQuery1 As DevExpress.DataAccess.Sql.StoredProcQuery = New DevExpress.DataAccess.Sql.StoredProcQuery()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RP1))
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand()
        Me.XrTable7 = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRow7 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell24 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell25 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand()
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand()
        Me.GroupHeader1 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrTable6 = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRow6 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell26 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell27 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.GroupHeader2 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.Parameter1 = New DevExpress.XtraReports.Parameters.Parameter()
        Me.SqlDataSource2 = New DevExpress.DataAccess.Sql.SqlDataSource(Me.components)
        CType(Me.XrTable7, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrTable6, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTable7})
        Me.Detail.HeightF = 20.0!
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrTable7
        '
        Me.XrTable7.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrTable7.Font = New System.Drawing.Font("Arial", 11.25!)
        Me.XrTable7.LocationFloat = New DevExpress.Utils.PointFloat(10.00011!, 0!)
        Me.XrTable7.Name = "XrTable7"
        Me.XrTable7.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96.0!)
        Me.XrTable7.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow7})
        Me.XrTable7.SizeF = New System.Drawing.SizeF(278.0415!, 20.0!)
        Me.XrTable7.StylePriority.UseBorders = False
        Me.XrTable7.StylePriority.UseFont = False
        '
        'XrTableRow7
        '
        Me.XrTableRow7.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell24, Me.XrTableCell25})
        Me.XrTableRow7.Name = "XrTableRow7"
        Me.XrTableRow7.Weight = 15.333333333333334R
        '
        'XrTableCell24
        '
        Me.XrTableCell24.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[damanecash_RPAlimCais].[Mtn]")})
        Me.XrTableCell24.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell24.Multiline = True
        Me.XrTableCell24.Name = "XrTableCell24"
        Me.XrTableCell24.StylePriority.UseFont = False
        Me.XrTableCell24.StylePriority.UseTextAlignment = False
        Me.XrTableCell24.Text = "XrTableCell24"
        Me.XrTableCell24.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight
        Me.XrTableCell24.TextFormatString = "{0:}"
        Me.XrTableCell24.Weight = 0.050896865922591711R
        '
        'XrTableCell25
        '
        Me.XrTableCell25.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[damanecash_RPAlimCais].[datex]")})
        Me.XrTableCell25.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell25.Multiline = True
        Me.XrTableCell25.Name = "XrTableCell25"
        Me.XrTableCell25.StylePriority.UseFont = False
        Me.XrTableCell25.StylePriority.UseTextAlignment = False
        Me.XrTableCell25.Text = "XrTableCell25"
        Me.XrTableCell25.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        Me.XrTableCell25.Weight = 0.077364077542955378R
        '
        'TopMargin
        '
        Me.TopMargin.HeightF = 19.79167!
        Me.TopMargin.Name = "TopMargin"
        Me.TopMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'BottomMargin
        '
        Me.BottomMargin.Name = "BottomMargin"
        Me.BottomMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'GroupHeader1
        '
        Me.GroupHeader1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTable6})
        Me.GroupHeader1.HeightF = 20.0!
        Me.GroupHeader1.Name = "GroupHeader1"
        '
        'XrTable6
        '
        Me.XrTable6.BackColor = System.Drawing.Color.Green
        Me.XrTable6.BorderColor = System.Drawing.Color.White
        Me.XrTable6.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrTable6.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTable6.ForeColor = System.Drawing.Color.White
        Me.XrTable6.LocationFloat = New DevExpress.Utils.PointFloat(10.0001!, 0!)
        Me.XrTable6.Name = "XrTable6"
        Me.XrTable6.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96.0!)
        Me.XrTable6.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow6})
        Me.XrTable6.SizeF = New System.Drawing.SizeF(278.0416!, 20.0!)
        Me.XrTable6.StylePriority.UseBackColor = False
        Me.XrTable6.StylePriority.UseBorderColor = False
        Me.XrTable6.StylePriority.UseBorders = False
        Me.XrTable6.StylePriority.UseFont = False
        Me.XrTable6.StylePriority.UseForeColor = False
        Me.XrTable6.StylePriority.UseTextAlignment = False
        Me.XrTable6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        '
        'XrTableRow6
        '
        Me.XrTableRow6.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell26, Me.XrTableCell27})
        Me.XrTableRow6.Name = "XrTableRow6"
        Me.XrTableRow6.Weight = 0.8R
        '
        'XrTableCell26
        '
        Me.XrTableCell26.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell26.BorderColor = System.Drawing.Color.Black
        Me.XrTableCell26.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell26.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell26.Multiline = True
        Me.XrTableCell26.Name = "XrTableCell26"
        Me.XrTableCell26.StylePriority.UseBackColor = False
        Me.XrTableCell26.StylePriority.UseBorderColor = False
        Me.XrTableCell26.StylePriority.UseFont = False
        Me.XrTableCell26.StylePriority.UseForeColor = False
        Me.XrTableCell26.Text = "Alimentation"
        Me.XrTableCell26.Weight = 1.182435078984327R
        '
        'XrTableCell27
        '
        Me.XrTableCell27.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell27.BorderColor = System.Drawing.Color.Black
        Me.XrTableCell27.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell27.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell27.Multiline = True
        Me.XrTableCell27.Name = "XrTableCell27"
        Me.XrTableCell27.StylePriority.UseBackColor = False
        Me.XrTableCell27.StylePriority.UseBorderColor = False
        Me.XrTableCell27.StylePriority.UseFont = False
        Me.XrTableCell27.StylePriority.UseForeColor = False
        Me.XrTableCell27.Text = "Date"
        Me.XrTableCell27.Weight = 1.7973184376333204R
        '
        'GroupHeader2
        '
        Me.GroupHeader2.HeightF = 0!
        Me.GroupHeader2.Level = 1
        Me.GroupHeader2.Name = "GroupHeader2"
        '
        'Parameter1
        '
        Me.Parameter1.Description = "Parameter1"
        Me.Parameter1.Name = "Parameter1"
        Me.Parameter1.Type = GetType(Date)
        Me.Parameter1.ValueInfo = "2024-03-24"
        '
        'SqlDataSource2
        '
        Me.SqlDataSource2.ConnectionName = "PC3_damanecash_Connection"
        Me.SqlDataSource2.Name = "SqlDataSource2"
        StoredProcQuery1.Name = "damanecash_RPAlimCais"
        StoredProcQuery1.StoredProcName = "damanecash.RPAlimCais"
        Me.SqlDataSource2.Queries.AddRange(New DevExpress.DataAccess.Sql.SqlQuery() {StoredProcQuery1})
        Me.SqlDataSource2.ResultSchemaSerializable = resources.GetString("SqlDataSource2.ResultSchemaSerializable")
        '
        'RP1
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader1, Me.GroupHeader2})
        Me.ComponentStorage.AddRange(New System.ComponentModel.IComponent() {Me.SqlDataSource2})
        Me.DataMember = "damanecash_RPAlimCais"
        Me.DataSource = Me.SqlDataSource2
        Me.FilterString = "[datex] Between(?Parameter1, AddDays(?Parameter1, 1))"
        Me.Margins = New System.Drawing.Printing.Margins(100, 100, 20, 100)
        Me.Parameters.AddRange(New DevExpress.XtraReports.Parameters.Parameter() {Me.Parameter1})
        Me.Version = "21.2"
        CType(Me.XrTable7, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrTable6, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents GroupHeader1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents GroupHeader2 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents Parameter1 As DevExpress.XtraReports.Parameters.Parameter
    Friend WithEvents XrTable6 As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRow6 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell26 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell27 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents SqlDataSource2 As DevExpress.DataAccess.Sql.SqlDataSource
    Friend WithEvents XrTable7 As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRow7 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell24 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell25 As DevExpress.XtraReports.UI.XRTableCell
End Class
