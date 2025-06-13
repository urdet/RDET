<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Public Class JournalDC
    Inherits DevExpress.XtraReports.UI.XtraReport

    'XtraReport overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim StoredProcQuery1 As DevExpress.DataAccess.Sql.StoredProcQuery = New DevExpress.DataAccess.Sql.StoredProcQuery()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(JournalDC))
        Dim SelectQuery1 As DevExpress.DataAccess.Sql.SelectQuery = New DevExpress.DataAccess.Sql.SelectQuery()
        Dim Column1 As DevExpress.DataAccess.Sql.Column = New DevExpress.DataAccess.Sql.Column()
        Dim ColumnExpression1 As DevExpress.DataAccess.Sql.ColumnExpression = New DevExpress.DataAccess.Sql.ColumnExpression()
        Dim Table1 As DevExpress.DataAccess.Sql.Table = New DevExpress.DataAccess.Sql.Table()
        Dim Column2 As DevExpress.DataAccess.Sql.Column = New DevExpress.DataAccess.Sql.Column()
        Dim ColumnExpression2 As DevExpress.DataAccess.Sql.ColumnExpression = New DevExpress.DataAccess.Sql.ColumnExpression()
        Dim Column3 As DevExpress.DataAccess.Sql.Column = New DevExpress.DataAccess.Sql.Column()
        Dim ColumnExpression3 As DevExpress.DataAccess.Sql.ColumnExpression = New DevExpress.DataAccess.Sql.ColumnExpression()
        Dim Column4 As DevExpress.DataAccess.Sql.Column = New DevExpress.DataAccess.Sql.Column()
        Dim ColumnExpression4 As DevExpress.DataAccess.Sql.ColumnExpression = New DevExpress.DataAccess.Sql.ColumnExpression()
        Dim StoredProcQuery2 As DevExpress.DataAccess.Sql.StoredProcQuery = New DevExpress.DataAccess.Sql.StoredProcQuery()
        Dim StoredProcQuery3 As DevExpress.DataAccess.Sql.StoredProcQuery = New DevExpress.DataAccess.Sql.StoredProcQuery()
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand()
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand()
        Me.XrPageInfo1 = New DevExpress.XtraReports.UI.XRPageInfo()
        Me.XrPageInfo2 = New DevExpress.XtraReports.UI.XRPageInfo()
        Me.GroupHeader1 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrLabel4 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel5 = New DevExpress.XtraReports.UI.XRLabel()
        Me.ReportHeader = New DevExpress.XtraReports.UI.ReportHeaderBand()
        Me.XrLabel14 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrPictureBox1 = New DevExpress.XtraReports.UI.XRPictureBox()
        Me.XrLabel3 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel2 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel1 = New DevExpress.XtraReports.UI.XRLabel()
        Me.DetailReport = New DevExpress.XtraReports.UI.DetailReportBand()
        Me.Detail1 = New DevExpress.XtraReports.UI.DetailBand()
        Me.XrTable1 = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRow1 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell6 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell1 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell2 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell3 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell4 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell5 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.GroupHeaderBand1 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrLabel19 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrTable2 = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRow2 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell7 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell8 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell9 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell10 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell11 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell12 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.GroupHeaderBand2 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.GroupHeaderBand3 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrLabel20 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel6 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel7 = New DevExpress.XtraReports.UI.XRLabel()
        Me.GroupFooterBand2 = New DevExpress.XtraReports.UI.GroupFooterBand()
        Me.XrTable3 = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRow3 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell13 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell14 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell15 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.SqlDataSource1 = New DevExpress.DataAccess.Sql.SqlDataSource(Me.components)
        Me.XrControlStyle1 = New DevExpress.XtraReports.UI.XRControlStyle()
        Me.date1 = New DevExpress.XtraReports.Parameters.Parameter()
        Me.CalculatedField1 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.XrControlStyle2 = New DevExpress.XtraReports.UI.XRControlStyle()
        Me.DetailReport1 = New DevExpress.XtraReports.UI.DetailReportBand()
        Me.Detail2 = New DevExpress.XtraReports.UI.DetailBand()
        Me.GroupHeader2 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrTable9 = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRow13 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell49 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell50 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell51 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell52 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell53 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell54 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell55 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableRow14 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell56 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell57 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell58 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell59 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell60 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell61 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell62 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrLabel8 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrTable4 = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRow4 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell19 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell22 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell41 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell43 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell45 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell16 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell47 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableRow12 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell21 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell23 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell42 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell44 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell46 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell18 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell48 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.SqlDataSource2 = New DevExpress.DataAccess.Sql.SqlDataSource(Me.components)
        Me.GroupFooter1 = New DevExpress.XtraReports.UI.GroupFooterBand()
        Me.XrLabel10 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel9 = New DevExpress.XtraReports.UI.XRLabel()
        Me.SqlDataSource3 = New DevExpress.DataAccess.Sql.SqlDataSource(Me.components)
        Me.CalculatedField2 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField3 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.DetailReport3 = New DevExpress.XtraReports.UI.DetailReportBand()
        Me.Detail4 = New DevExpress.XtraReports.UI.DetailBand()
        Me.GroupHeader4 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrSubreport1 = New DevExpress.XtraReports.UI.XRSubreport()
        Me.XrTable8 = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRow8 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell29 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell30 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell31 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableRow9 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell32 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell33 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell34 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableRow11 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell38 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell39 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell40 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableRow10 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell35 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell36 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell37 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.SqlDataSource4 = New DevExpress.DataAccess.Sql.SqlDataSource(Me.components)
        Me.CalculatedField4 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField5 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField6 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField7 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField8 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField9 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField10 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField11 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField12 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField13 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField14 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField15 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField16 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.CalculatedField17 = New DevExpress.XtraReports.UI.CalculatedField()
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand()
        Me.XrControlStyle3 = New DevExpress.XtraReports.UI.XRControlStyle()
        Me.CalculatedField18 = New DevExpress.XtraReports.UI.CalculatedField()
        CType(Me.XrTable1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrTable2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrTable3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrTable9, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrTable4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrTable8, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'TopMargin
        '
        Me.TopMargin.HeightF = 17.0!
        Me.TopMargin.Name = "TopMargin"
        '
        'BottomMargin
        '
        Me.BottomMargin.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPageInfo1, Me.XrPageInfo2})
        Me.BottomMargin.HeightF = 42.0!
        Me.BottomMargin.Name = "BottomMargin"
        Me.BottomMargin.Scripts.OnBeforePrint = "BottomMargin_BeforePrint"
        '
        'XrPageInfo1
        '
        Me.XrPageInfo1.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrPageInfo1.LocationFloat = New DevExpress.Utils.PointFloat(665.0!, 0!)
        Me.XrPageInfo1.Name = "XrPageInfo1"
        Me.XrPageInfo1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrPageInfo1.SizeF = New System.Drawing.SizeF(100.0!, 23.0!)
        Me.XrPageInfo1.StylePriority.UseFont = False
        '
        'XrPageInfo2
        '
        Me.XrPageInfo2.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrPageInfo2.LocationFloat = New DevExpress.Utils.PointFloat(0!, 0!)
        Me.XrPageInfo2.Name = "XrPageInfo2"
        Me.XrPageInfo2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrPageInfo2.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime
        Me.XrPageInfo2.SizeF = New System.Drawing.SizeF(198.2084!, 23.0!)
        Me.XrPageInfo2.StylePriority.UseFont = False
        Me.XrPageInfo2.TextFormatString = "{0:dddd, dd MMMM, yyyy H:mm}"
        '
        'GroupHeader1
        '
        Me.GroupHeader1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel4})
        Me.GroupHeader1.HeightF = 36.54168!
        Me.GroupHeader1.Name = "GroupHeader1"
        '
        'XrLabel4
        '
        Me.XrLabel4.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel4.LocationFloat = New DevExpress.Utils.PointFloat(195.8333!, 0!)
        Me.XrLabel4.Multiline = True
        Me.XrLabel4.Name = "XrLabel4"
        Me.XrLabel4.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel4.SizeF = New System.Drawing.SizeF(377.0833!, 36.54168!)
        Me.XrLabel4.StylePriority.UseFont = False
        Me.XrLabel4.Text = "Gestion Damane Cash Agence"
        '
        'XrLabel5
        '
        Me.XrLabel5.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel5.LocationFloat = New DevExpress.Utils.PointFloat(411.4583!, 0!)
        Me.XrLabel5.Multiline = True
        Me.XrLabel5.Name = "XrLabel5"
        Me.XrLabel5.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel5.SizeF = New System.Drawing.SizeF(92.4583!, 23.0!)
        Me.XrLabel5.StylePriority.UseFont = False
        Me.XrLabel5.Text = "Rapport de : "
        '
        'ReportHeader
        '
        Me.ReportHeader.BackColor = System.Drawing.Color.LightCyan
        Me.ReportHeader.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel14, Me.XrLabel5, Me.XrPictureBox1, Me.XrLabel3, Me.XrLabel2, Me.XrLabel1})
        Me.ReportHeader.HeightF = 70.83334!
        Me.ReportHeader.Name = "ReportHeader"
        Me.ReportHeader.StylePriority.UseBackColor = False
        '
        'XrLabel14
        '
        Me.XrLabel14.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?date1")})
        Me.XrLabel14.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel14.LocationFloat = New DevExpress.Utils.PointFloat(503.9166!, 0!)
        Me.XrLabel14.Multiline = True
        Me.XrLabel14.Name = "XrLabel14"
        Me.XrLabel14.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel14.SizeF = New System.Drawing.SizeF(105.7501!, 23.0!)
        Me.XrLabel14.StylePriority.UseFont = False
        Me.XrLabel14.Text = "Rapport de : "
        Me.XrLabel14.TextFormatString = "{0:dd-MM-yyyy}"
        '
        'XrPictureBox1
        '
        Me.XrPictureBox1.ImageSource = New DevExpress.XtraPrinting.Drawing.ImageSource(Global.RDET.My.Resources.Resources._1696946891931, True)
        Me.XrPictureBox1.LocationFloat = New DevExpress.Utils.PointFloat(0!, 0!)
        Me.XrPictureBox1.Name = "XrPictureBox1"
        Me.XrPictureBox1.SizeF = New System.Drawing.SizeF(84.375!, 68.99999!)
        Me.XrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.ZoomImage
        '
        'XrLabel3
        '
        Me.XrLabel3.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel3.LocationFloat = New DevExpress.Utils.PointFloat(84.375!, 46.0!)
        Me.XrLabel3.Multiline = True
        Me.XrLabel3.Name = "XrLabel3"
        Me.XrLabel3.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel3.SizeF = New System.Drawing.SizeF(327.0833!, 23.0!)
        Me.XrLabel3.StylePriority.UseFont = False
        Me.XrLabel3.Text = "IF 24915458 - Pat 41900072"
        '
        'XrLabel2
        '
        Me.XrLabel2.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel2.LocationFloat = New DevExpress.Utils.PointFloat(84.375!, 23.00002!)
        Me.XrLabel2.Multiline = True
        Me.XrLabel2.Name = "XrLabel2"
        Me.XrLabel2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel2.SizeF = New System.Drawing.SizeF(327.0833!, 23.0!)
        Me.XrLabel2.StylePriority.UseFont = False
        Me.XrLabel2.Text = "Ouled Ali Eloued - Bradia - Fquih Ben salah"
        '
        'XrLabel1
        '
        Me.XrLabel1.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel1.LocationFloat = New DevExpress.Utils.PointFloat(84.375!, 0!)
        Me.XrLabel1.Multiline = True
        Me.XrLabel1.Name = "XrLabel1"
        Me.XrLabel1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel1.SizeF = New System.Drawing.SizeF(327.0833!, 23.0!)
        Me.XrLabel1.StylePriority.UseFont = False
        Me.XrLabel1.Text = "DC Ouled Ali - Code d'agence 6309"
        '
        'DetailReport
        '
        Me.DetailReport.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail1, Me.GroupHeaderBand1, Me.GroupHeaderBand2, Me.GroupHeaderBand3, Me.GroupFooterBand2})
        Me.DetailReport.DataMember = "damanecash_tranServicesReportByDate"
        Me.DetailReport.DataSource = Me.SqlDataSource1
        Me.DetailReport.FilterString = "[datex] Between(?date1, AddDays(?date1, 1))"
        Me.DetailReport.Level = 0
        Me.DetailReport.Name = "DetailReport"
        '
        'Detail1
        '
        Me.Detail1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTable1})
        Me.Detail1.HeightF = 15.0!
        Me.Detail1.Name = "Detail1"
        '
        'XrTable1
        '
        Me.XrTable1.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrTable1.EvenStyleName = "XrControlStyle2"
        Me.XrTable1.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTable1.LocationFloat = New DevExpress.Utils.PointFloat(10.0!, 0!)
        Me.XrTable1.Name = "XrTable1"
        Me.XrTable1.OddStyleName = "XrControlStyle1"
        Me.XrTable1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96.0!)
        Me.XrTable1.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow1})
        Me.XrTable1.SizeF = New System.Drawing.SizeF(748.0001!, 15.0!)
        Me.XrTable1.StylePriority.UseBorders = False
        Me.XrTable1.StylePriority.UseFont = False
        Me.XrTable1.StylePriority.UseTextAlignment = False
        Me.XrTable1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        '
        'XrTableRow1
        '
        Me.XrTableRow1.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell6, Me.XrTableCell1, Me.XrTableCell2, Me.XrTableCell3, Me.XrTableCell4, Me.XrTableCell5})
        Me.XrTableRow1.Name = "XrTableRow1"
        Me.XrTableRow1.Weight = 11.5R
        '
        'XrTableCell6
        '
        Me.XrTableCell6.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DataSource.CurrentRowIndex]+1")})
        Me.XrTableCell6.Multiline = True
        Me.XrTableCell6.Name = "XrTableCell6"
        Me.XrTableCell6.Text = "XrTableCell6"
        Me.XrTableCell6.Weight = 0.2168114728002406R
        '
        'XrTableCell1
        '
        Me.XrTableCell1.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[damanecash_tranServicesReportByDate].[Typ]")})
        Me.XrTableCell1.Multiline = True
        Me.XrTableCell1.Name = "XrTableCell1"
        Me.XrTableCell1.Text = "XrTableCell1"
        Me.XrTableCell1.TextFormatString = "{0:d}"
        Me.XrTableCell1.Weight = 0.17842903955658873R
        '
        'XrTableCell2
        '
        Me.XrTableCell2.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[damanecash_tranServicesReportByDate].[Service]")})
        Me.XrTableCell2.Multiline = True
        Me.XrTableCell2.Name = "XrTableCell2"
        Me.XrTableCell2.StylePriority.UseTextAlignment = False
        Me.XrTableCell2.Text = "XrTableCell2"
        Me.XrTableCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        Me.XrTableCell2.Weight = 0.63427986938561975R
        '
        'XrTableCell3
        '
        Me.XrTableCell3.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[damanecash_tranServicesReportByDate].[Mtn]")})
        Me.XrTableCell3.Multiline = True
        Me.XrTableCell3.Name = "XrTableCell3"
        Me.XrTableCell3.StylePriority.UseTextAlignment = False
        Me.XrTableCell3.Text = "XrTableCell3"
        Me.XrTableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight
        Me.XrTableCell3.Weight = 0.32758552636673205R
        '
        'XrTableCell4
        '
        Me.XrTableCell4.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[damanecash_tranServicesReportByDate].[Frais]")})
        Me.XrTableCell4.Multiline = True
        Me.XrTableCell4.Name = "XrTableCell4"
        Me.XrTableCell4.StylePriority.UseTextAlignment = False
        Me.XrTableCell4.Text = "XrTableCell4"
        Me.XrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight
        Me.XrTableCell4.Weight = 0.22737890926759641R
        '
        'XrTableCell5
        '
        Me.XrTableCell5.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[damanecash_tranServicesReportByDate].[datex]")})
        Me.XrTableCell5.Multiline = True
        Me.XrTableCell5.Name = "XrTableCell5"
        Me.XrTableCell5.Text = "XrTableCell5"
        Me.XrTableCell5.Weight = 0.596019091890819R
        '
        'GroupHeaderBand1
        '
        Me.GroupHeaderBand1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel19, Me.XrTable2})
        Me.GroupHeaderBand1.HeightF = 50.0!
        Me.GroupHeaderBand1.Level = 2
        Me.GroupHeaderBand1.Name = "GroupHeaderBand1"
        '
        'XrLabel19
        '
        Me.XrLabel19.BorderColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.XrLabel19.Borders = DevExpress.XtraPrinting.BorderSide.Top
        Me.XrLabel19.BorderWidth = 2.0!
        Me.XrLabel19.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel19.LocationFloat = New DevExpress.Utils.PointFloat(0!, 0!)
        Me.XrLabel19.Multiline = True
        Me.XrLabel19.Name = "XrLabel19"
        Me.XrLabel19.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel19.SizeF = New System.Drawing.SizeF(145.5833!, 23.0!)
        Me.XrLabel19.StylePriority.UseBorderColor = False
        Me.XrLabel19.StylePriority.UseBorders = False
        Me.XrLabel19.StylePriority.UseBorderWidth = False
        Me.XrLabel19.StylePriority.UseFont = False
        Me.XrLabel19.Text = "Les transactions"
        '
        'XrTable2
        '
        Me.XrTable2.BackColor = System.Drawing.Color.DimGray
        Me.XrTable2.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrTable2.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTable2.ForeColor = System.Drawing.Color.White
        Me.XrTable2.LocationFloat = New DevExpress.Utils.PointFloat(7.999897!, 25.0!)
        Me.XrTable2.Name = "XrTable2"
        Me.XrTable2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96.0!)
        Me.XrTable2.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow2})
        Me.XrTable2.SizeF = New System.Drawing.SizeF(748.0001!, 25.0!)
        Me.XrTable2.StylePriority.UseBackColor = False
        Me.XrTable2.StylePriority.UseBorders = False
        Me.XrTable2.StylePriority.UseFont = False
        Me.XrTable2.StylePriority.UseForeColor = False
        Me.XrTable2.StylePriority.UseTextAlignment = False
        Me.XrTable2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        '
        'XrTableRow2
        '
        Me.XrTableRow2.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell7, Me.XrTableCell8, Me.XrTableCell9, Me.XrTableCell10, Me.XrTableCell11, Me.XrTableCell12})
        Me.XrTableRow2.Name = "XrTableRow2"
        Me.XrTableRow2.Weight = 1.0R
        '
        'XrTableCell7
        '
        Me.XrTableCell7.Multiline = True
        Me.XrTableCell7.Name = "XrTableCell7"
        Me.XrTableCell7.Text = "No"
        Me.XrTableCell7.Weight = 1.0039507238358585R
        '
        'XrTableCell8
        '
        Me.XrTableCell8.Multiline = True
        Me.XrTableCell8.Name = "XrTableCell8"
        Me.XrTableCell8.Text = "Type"
        Me.XrTableCell8.Weight = 0.82622046295494422R
        '
        'XrTableCell9
        '
        Me.XrTableCell9.Multiline = True
        Me.XrTableCell9.Name = "XrTableCell9"
        Me.XrTableCell9.Text = "Service"
        Me.XrTableCell9.Weight = 2.9370483110409866R
        '
        'XrTableCell10
        '
        Me.XrTableCell10.Multiline = True
        Me.XrTableCell10.Name = "XrTableCell10"
        Me.XrTableCell10.Text = "Mantant"
        Me.XrTableCell10.Weight = 1.516892813072785R
        '
        'XrTableCell11
        '
        Me.XrTableCell11.Multiline = True
        Me.XrTableCell11.Name = "XrTableCell11"
        Me.XrTableCell11.Text = "Frais"
        Me.XrTableCell11.Weight = 1.0528836398837576R
        '
        'XrTableCell12
        '
        Me.XrTableCell12.Multiline = True
        Me.XrTableCell12.Name = "XrTableCell12"
        Me.XrTableCell12.Text = "Date"
        Me.XrTableCell12.Weight = 2.7598803370693781R
        '
        'GroupHeaderBand2
        '
        Me.GroupHeaderBand2.GroupFields.AddRange(New DevExpress.XtraReports.UI.GroupField() {New DevExpress.XtraReports.UI.GroupField("Service", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)})
        Me.GroupHeaderBand2.HeightF = 0!
        Me.GroupHeaderBand2.Level = 1
        Me.GroupHeaderBand2.Name = "GroupHeaderBand2"
        '
        'GroupHeaderBand3
        '
        Me.GroupHeaderBand3.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel20, Me.XrLabel6, Me.XrLabel7})
        Me.GroupHeaderBand3.GroupFields.AddRange(New DevExpress.XtraReports.UI.GroupField() {New DevExpress.XtraReports.UI.GroupField("Typ", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)})
        Me.GroupHeaderBand3.HeightF = 17.0!
        Me.GroupHeaderBand3.Name = "GroupHeaderBand3"
        '
        'XrLabel20
        '
        Me.XrLabel20.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Service] + ' '+ [Typ]")})
        Me.XrLabel20.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel20.LocationFloat = New DevExpress.Utils.PointFloat(12.25001!, 0!)
        Me.XrLabel20.Multiline = True
        Me.XrLabel20.Name = "XrLabel20"
        Me.XrLabel20.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel20.SizeF = New System.Drawing.SizeF(135.5833!, 17.0!)
        Me.XrLabel20.StylePriority.UseFont = False
        Me.XrLabel20.StylePriority.UseTextAlignment = False
        Me.XrLabel20.Text = "XrLabel6"
        Me.XrLabel20.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        '
        'XrLabel6
        '
        Me.XrLabel6.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Service]")})
        Me.XrLabel6.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel6.LocationFloat = New DevExpress.Utils.PointFloat(666.0001!, 0!)
        Me.XrLabel6.Multiline = True
        Me.XrLabel6.Name = "XrLabel6"
        Me.XrLabel6.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel6.SizeF = New System.Drawing.SizeF(23.12488!, 17.0!)
        Me.XrLabel6.StylePriority.UseFont = False
        Me.XrLabel6.StylePriority.UseTextAlignment = False
        Me.XrLabel6.Text = "XrLabel6"
        Me.XrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        Me.XrLabel6.Visible = False
        '
        'XrLabel7
        '
        Me.XrLabel7.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Typ]")})
        Me.XrLabel7.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel7.LocationFloat = New DevExpress.Utils.PointFloat(641.0001!, 0!)
        Me.XrLabel7.Multiline = True
        Me.XrLabel7.Name = "XrLabel7"
        Me.XrLabel7.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel7.SizeF = New System.Drawing.SizeF(25.0!, 17.0!)
        Me.XrLabel7.StylePriority.UseFont = False
        Me.XrLabel7.Text = "XrLabel6"
        Me.XrLabel7.Visible = False
        '
        'GroupFooterBand2
        '
        Me.GroupFooterBand2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTable3})
        Me.GroupFooterBand2.HeightF = 12.0!
        Me.GroupFooterBand2.Name = "GroupFooterBand2"
        '
        'XrTable3
        '
        Me.XrTable3.BackColor = System.Drawing.Color.MintCream
        Me.XrTable3.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid
        Me.XrTable3.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrTable3.BorderWidth = 1.0!
        Me.XrTable3.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.XrTable3.LocationFloat = New DevExpress.Utils.PointFloat(268.9999!, 0!)
        Me.XrTable3.Name = "XrTable3"
        Me.XrTable3.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96.0!)
        Me.XrTable3.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow3})
        Me.XrTable3.SizeF = New System.Drawing.SizeF(284.5417!, 12.0!)
        Me.XrTable3.StylePriority.UseBackColor = False
        Me.XrTable3.StylePriority.UseBorderDashStyle = False
        Me.XrTable3.StylePriority.UseBorders = False
        Me.XrTable3.StylePriority.UseBorderWidth = False
        Me.XrTable3.StylePriority.UseFont = False
        '
        'XrTableRow3
        '
        Me.XrTableRow3.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell13, Me.XrTableCell14, Me.XrTableCell15})
        Me.XrTableRow3.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableRow3.Name = "XrTableRow3"
        Me.XrTableRow3.StylePriority.UseFont = False
        Me.XrTableRow3.Weight = 0.8R
        '
        'XrTableCell13
        '
        Me.XrTableCell13.Multiline = True
        Me.XrTableCell13.Name = "XrTableCell13"
        Me.XrTableCell13.StylePriority.UseTextAlignment = False
        Me.XrTableCell13.Text = "Somme:"
        Me.XrTableCell13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight
        Me.XrTableCell13.Weight = 0.94166748046875015R
        '
        'XrTableCell14
        '
        Me.XrTableCell14.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Sum(Iif([Service] = [ReportItems.XrLabel6].[Text] && [Typ]=[ReportItems.XrLabel7]" &
                    ".[Text] && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [Mtn], 0))")})
        Me.XrTableCell14.Multiline = True
        Me.XrTableCell14.Name = "XrTableCell14"
        Me.XrTableCell14.StylePriority.UseTextAlignment = False
        Me.XrTableCell14.Text = "XrTableCell14"
        Me.XrTableCell14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight
        Me.XrTableCell14.Weight = 1.1237493896484374R
        '
        'XrTableCell15
        '
        Me.XrTableCell15.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Sum(Iif([Service] = [ReportItems.XrLabel6].[Text] && [Typ]=[ReportItems.XrLabel7]" &
                    ".[Text] && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [Frais], 0))")})
        Me.XrTableCell15.Multiline = True
        Me.XrTableCell15.Name = "XrTableCell15"
        Me.XrTableCell15.StylePriority.UseTextAlignment = False
        Me.XrTableCell15.Text = "frais"
        Me.XrTableCell15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight
        Me.XrTableCell15.Weight = 0.78000061035156254R
        '
        'SqlDataSource1
        '
        Me.SqlDataSource1.ConnectionName = "PC3_damanecash_Connection"
        Me.SqlDataSource1.Name = "SqlDataSource1"
        StoredProcQuery1.Name = "damanecash_tranServicesReportByDate"
        StoredProcQuery1.StoredProcName = "damanecash.tranServicesReportByDate"
        Me.SqlDataSource1.Queries.AddRange(New DevExpress.DataAccess.Sql.SqlQuery() {StoredProcQuery1})
        Me.SqlDataSource1.ResultSchemaSerializable = resources.GetString("SqlDataSource1.ResultSchemaSerializable")
        '
        'XrControlStyle1
        '
        Me.XrControlStyle1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrControlStyle1.Name = "XrControlStyle1"
        Me.XrControlStyle1.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.XrControlStyle1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        '
        'date1
        '
        Me.date1.Description = "Start Date"
        Me.date1.ExpressionBindings.AddRange(New DevExpress.XtraReports.Expressions.BasicExpressionBinding() {New DevExpress.XtraReports.Expressions.BasicExpressionBinding("Value", "GetDate(Now())")})
        Me.date1.Name = "date1"
        Me.date1.Type = GetType(Date)
        Me.date1.ValueInfo = "2024-03-09"
        '
        'CalculatedField1
        '
        Me.CalculatedField1.DataMember = "damanecash_tranServicesReportByDate"
        Me.CalculatedField1.DataSource = Me.SqlDataSource1
        Me.CalculatedField1.Expression = "Sum(Iif([Service] = 'WESTERN', [Mtn], 0))"
        Me.CalculatedField1.Name = "CalculatedField1"
        '
        'XrControlStyle2
        '
        Me.XrControlStyle2.BackColor = System.Drawing.Color.Gainsboro
        Me.XrControlStyle2.Name = "XrControlStyle2"
        Me.XrControlStyle2.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        '
        'DetailReport1
        '
        Me.DetailReport1.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail2, Me.GroupHeader2})
        Me.DetailReport1.DataMember = "compteshistoriquedata"
        Me.DetailReport1.DataSource = Me.SqlDataSource2
        Me.DetailReport1.FilterString = "[datex] Between(?date1, AddDays(?date1, 1))"
        Me.DetailReport1.Level = 2
        Me.DetailReport1.Name = "DetailReport1"
        '
        'Detail2
        '
        Me.Detail2.HeightF = 0!
        Me.Detail2.Name = "Detail2"
        '
        'GroupHeader2
        '
        Me.GroupHeader2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTable9, Me.XrLabel8, Me.XrTable4})
        Me.GroupHeader2.HeightF = 137.7084!
        Me.GroupHeader2.Name = "GroupHeader2"
        '
        'XrTable9
        '
        Me.XrTable9.BackColor = System.Drawing.Color.Navy
        Me.XrTable9.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrTable9.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTable9.ForeColor = System.Drawing.Color.White
        Me.XrTable9.LocationFloat = New DevExpress.Utils.PointFloat(2.000109!, 87.70839!)
        Me.XrTable9.Name = "XrTable9"
        Me.XrTable9.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96.0!)
        Me.XrTable9.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow13, Me.XrTableRow14})
        Me.XrTable9.SizeF = New System.Drawing.SizeF(756.0!, 45.0!)
        Me.XrTable9.StylePriority.UseBackColor = False
        Me.XrTable9.StylePriority.UseBorders = False
        Me.XrTable9.StylePriority.UseFont = False
        Me.XrTable9.StylePriority.UseForeColor = False
        Me.XrTable9.StylePriority.UseTextAlignment = False
        Me.XrTable9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        '
        'XrTableRow13
        '
        Me.XrTableRow13.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell49, Me.XrTableCell50, Me.XrTableCell51, Me.XrTableCell52, Me.XrTableCell53, Me.XrTableCell54, Me.XrTableCell55})
        Me.XrTableRow13.Name = "XrTableRow13"
        Me.XrTableRow13.Weight = 1.0R
        '
        'XrTableCell49
        '
        Me.XrTableCell49.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell49.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell49.Multiline = True
        Me.XrTableCell49.Name = "XrTableCell49"
        Me.XrTableCell49.StylePriority.UseBackColor = False
        Me.XrTableCell49.StylePriority.UseForeColor = False
        Me.XrTableCell49.Text = "Salaf"
        Me.XrTableCell49.Weight = 3.4025366972112R
        '
        'XrTableCell50
        '
        Me.XrTableCell50.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell50.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell50.Multiline = True
        Me.XrTableCell50.Name = "XrTableCell50"
        Me.XrTableCell50.StylePriority.UseBackColor = False
        Me.XrTableCell50.StylePriority.UseForeColor = False
        Me.XrTableCell50.Text = "Retrait"
        Me.XrTableCell50.Weight = 3.40253668885788R
        '
        'XrTableCell51
        '
        Me.XrTableCell51.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell51.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell51.Multiline = True
        Me.XrTableCell51.Name = "XrTableCell51"
        Me.XrTableCell51.StylePriority.UseBackColor = False
        Me.XrTableCell51.StylePriority.UseForeColor = False
        Me.XrTableCell51.Text = "Bank reél"
        Me.XrTableCell51.Weight = 3.4025366361130969R
        '
        'XrTableCell52
        '
        Me.XrTableCell52.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell52.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell52.Multiline = True
        Me.XrTableCell52.Name = "XrTableCell52"
        Me.XrTableCell52.StylePriority.UseBackColor = False
        Me.XrTableCell52.StylePriority.UseForeColor = False
        Me.XrTableCell52.Text = "Commission"
        Me.XrTableCell52.Weight = 3.4025365943435406R
        '
        'XrTableCell53
        '
        Me.XrTableCell53.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell53.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell53.Multiline = True
        Me.XrTableCell53.Name = "XrTableCell53"
        Me.XrTableCell53.StylePriority.UseBackColor = False
        Me.XrTableCell53.StylePriority.UseForeColor = False
        Me.XrTableCell53.Text = "Caisse reél"
        Me.XrTableCell53.Weight = 3.4025366977595342R
        '
        'XrTableCell54
        '
        Me.XrTableCell54.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell54.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell54.Multiline = True
        Me.XrTableCell54.Name = "XrTableCell54"
        Me.XrTableCell54.StylePriority.UseBackColor = False
        Me.XrTableCell54.StylePriority.UseForeColor = False
        Me.XrTableCell54.Text = "Non Payé"
        Me.XrTableCell54.Weight = 3.4025366977595324R
        '
        'XrTableCell55
        '
        Me.XrTableCell55.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell55.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell55.Multiline = True
        Me.XrTableCell55.Name = "XrTableCell55"
        Me.XrTableCell55.StylePriority.UseBackColor = False
        Me.XrTableCell55.StylePriority.UseForeColor = False
        Me.XrTableCell55.Text = "B Caisse"
        Me.XrTableCell55.Weight = 3.4025366977595346R
        '
        'XrTableRow14
        '
        Me.XrTableRow14.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell56, Me.XrTableCell57, Me.XrTableCell58, Me.XrTableCell59, Me.XrTableCell60, Me.XrTableCell61, Me.XrTableCell62})
        Me.XrTableRow14.Name = "XrTableRow14"
        Me.XrTableRow14.Weight = 0.8R
        '
        'XrTableCell56
        '
        Me.XrTableCell56.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell56.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum(Iif([CompteID]= 7 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [So" &
                    "lde], 0))")})
        Me.XrTableCell56.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell56.Multiline = True
        Me.XrTableCell56.Name = "XrTableCell56"
        Me.XrTableCell56.StylePriority.UseBackColor = False
        Me.XrTableCell56.StylePriority.UseForeColor = False
        Me.XrTableCell56.Weight = 3.402536697211199R
        '
        'XrTableCell57
        '
        Me.XrTableCell57.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell57.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum(Iif([CompteID]= 15 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [S" &
                    "olde], 0))")})
        Me.XrTableCell57.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell57.Multiline = True
        Me.XrTableCell57.Name = "XrTableCell57"
        Me.XrTableCell57.StylePriority.UseBackColor = False
        Me.XrTableCell57.StylePriority.UseForeColor = False
        Me.XrTableCell57.Text = "Iif([CompteID]= 2 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [Solde]" &
    ", 0)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.XrTableCell57.Weight = 3.4025366916783155R
        '
        'XrTableCell58
        '
        Me.XrTableCell58.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell58.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum(Iif([CompteID]= 12 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [S" &
                    "olde], 0))")})
        Me.XrTableCell58.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell58.Multiline = True
        Me.XrTableCell58.Name = "XrTableCell58"
        Me.XrTableCell58.StylePriority.UseBackColor = False
        Me.XrTableCell58.StylePriority.UseForeColor = False
        Me.XrTableCell58.Weight = 3.4025366332926605R
        '
        'XrTableCell59
        '
        Me.XrTableCell59.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell59.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum(Iif([CompteID]= 14 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [S" &
                    "olde], 0))" & Global.Microsoft.VisualBasic.ChrW(10))})
        Me.XrTableCell59.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell59.Multiline = True
        Me.XrTableCell59.Name = "XrTableCell59"
        Me.XrTableCell59.StylePriority.UseBackColor = False
        Me.XrTableCell59.StylePriority.UseForeColor = False
        Me.XrTableCell59.Weight = 3.4025365943435406R
        '
        'XrTableCell60
        '
        Me.XrTableCell60.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell60.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum(Iif([CompteID]= 11 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [S" &
                    "olde], 0))" & Global.Microsoft.VisualBasic.ChrW(10))})
        Me.XrTableCell60.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell60.Multiline = True
        Me.XrTableCell60.Name = "XrTableCell60"
        Me.XrTableCell60.StylePriority.UseBackColor = False
        Me.XrTableCell60.StylePriority.UseForeColor = False
        Me.XrTableCell60.Text = "Iif([CompteID]= 5 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [Solde]" &
    ", 0)"
        Me.XrTableCell60.Weight = 3.4025366977595342R
        '
        'XrTableCell61
        '
        Me.XrTableCell61.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell61.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum(Iif([CompteID]= 10 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [S" &
                    "olde], 0))")})
        Me.XrTableCell61.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell61.Multiline = True
        Me.XrTableCell61.Name = "XrTableCell61"
        Me.XrTableCell61.StylePriority.UseBackColor = False
        Me.XrTableCell61.StylePriority.UseForeColor = False
        Me.XrTableCell61.Weight = 3.4025366977595337R
        '
        'XrTableCell62
        '
        Me.XrTableCell62.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell62.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CalculatedField3]")})
        Me.XrTableCell62.ForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.XrTableCell62.Multiline = True
        Me.XrTableCell62.Name = "XrTableCell62"
        Me.XrTableCell62.StylePriority.UseBackColor = False
        Me.XrTableCell62.StylePriority.UseForeColor = False
        Me.XrTableCell62.Weight = 3.4025366977595346R
        '
        'XrLabel8
        '
        Me.XrLabel8.BorderColor = System.Drawing.Color.Blue
        Me.XrLabel8.Borders = DevExpress.XtraPrinting.BorderSide.Top
        Me.XrLabel8.BorderWidth = 2.0!
        Me.XrLabel8.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel8.LocationFloat = New DevExpress.Utils.PointFloat(0!, 0!)
        Me.XrLabel8.Multiline = True
        Me.XrLabel8.Name = "XrLabel8"
        Me.XrLabel8.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel8.SizeF = New System.Drawing.SizeF(145.5833!, 23.0!)
        Me.XrLabel8.StylePriority.UseBorderColor = False
        Me.XrLabel8.StylePriority.UseBorders = False
        Me.XrLabel8.StylePriority.UseBorderWidth = False
        Me.XrLabel8.StylePriority.UseFont = False
        Me.XrLabel8.Text = "Extrais des comptes"
        '
        'XrTable4
        '
        Me.XrTable4.BackColor = System.Drawing.Color.Navy
        Me.XrTable4.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrTable4.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTable4.ForeColor = System.Drawing.Color.White
        Me.XrTable4.LocationFloat = New DevExpress.Utils.PointFloat(2.000122!, 23.0!)
        Me.XrTable4.Name = "XrTable4"
        Me.XrTable4.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96.0!)
        Me.XrTable4.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow4, Me.XrTableRow12})
        Me.XrTable4.SizeF = New System.Drawing.SizeF(756.0!, 45.0!)
        Me.XrTable4.StylePriority.UseBackColor = False
        Me.XrTable4.StylePriority.UseBorders = False
        Me.XrTable4.StylePriority.UseFont = False
        Me.XrTable4.StylePriority.UseForeColor = False
        Me.XrTable4.StylePriority.UseTextAlignment = False
        Me.XrTable4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        '
        'XrTableRow4
        '
        Me.XrTableRow4.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell19, Me.XrTableCell22, Me.XrTableCell41, Me.XrTableCell43, Me.XrTableCell45, Me.XrTableCell16, Me.XrTableCell47})
        Me.XrTableRow4.Name = "XrTableRow4"
        Me.XrTableRow4.Weight = 1.0R
        '
        'XrTableCell19
        '
        Me.XrTableCell19.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell19.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell19.Multiline = True
        Me.XrTableCell19.Name = "XrTableCell19"
        Me.XrTableCell19.StylePriority.UseBackColor = False
        Me.XrTableCell19.StylePriority.UseForeColor = False
        Me.XrTableCell19.Text = "D Pay"
        Me.XrTableCell19.Weight = 3.4025366972112R
        '
        'XrTableCell22
        '
        Me.XrTableCell22.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell22.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell22.Multiline = True
        Me.XrTableCell22.Name = "XrTableCell22"
        Me.XrTableCell22.StylePriority.UseBackColor = False
        Me.XrTableCell22.StylePriority.UseForeColor = False
        Me.XrTableCell22.Text = "Fundex"
        Me.XrTableCell22.Weight = 3.40253668885788R
        '
        'XrTableCell41
        '
        Me.XrTableCell41.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell41.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell41.Multiline = True
        Me.XrTableCell41.Name = "XrTableCell41"
        Me.XrTableCell41.StylePriority.UseBackColor = False
        Me.XrTableCell41.StylePriority.UseForeColor = False
        Me.XrTableCell41.Text = "Bank"
        Me.XrTableCell41.Weight = 3.4025366361130969R
        '
        'XrTableCell43
        '
        Me.XrTableCell43.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell43.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell43.Multiline = True
        Me.XrTableCell43.Name = "XrTableCell43"
        Me.XrTableCell43.StylePriority.UseBackColor = False
        Me.XrTableCell43.StylePriority.UseForeColor = False
        Me.XrTableCell43.Text = "Coffre"
        Me.XrTableCell43.Weight = 3.4025365943435406R
        '
        'XrTableCell45
        '
        Me.XrTableCell45.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell45.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell45.Multiline = True
        Me.XrTableCell45.Name = "XrTableCell45"
        Me.XrTableCell45.StylePriority.UseBackColor = False
        Me.XrTableCell45.StylePriority.UseForeColor = False
        Me.XrTableCell45.Text = "Caisse cal"
        Me.XrTableCell45.Weight = 3.4025366977595342R
        '
        'XrTableCell16
        '
        Me.XrTableCell16.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell16.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell16.Multiline = True
        Me.XrTableCell16.Name = "XrTableCell16"
        Me.XrTableCell16.StylePriority.UseBackColor = False
        Me.XrTableCell16.StylePriority.UseForeColor = False
        Me.XrTableCell16.Text = "Capital"
        Me.XrTableCell16.Weight = 3.4025366977595324R
        '
        'XrTableCell47
        '
        Me.XrTableCell47.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell47.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell47.Multiline = True
        Me.XrTableCell47.Name = "XrTableCell47"
        Me.XrTableCell47.StylePriority.UseBackColor = False
        Me.XrTableCell47.StylePriority.UseForeColor = False
        Me.XrTableCell47.Text = "B Capital"
        Me.XrTableCell47.Weight = 3.4025366977595346R
        '
        'XrTableRow12
        '
        Me.XrTableRow12.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell21, Me.XrTableCell23, Me.XrTableCell42, Me.XrTableCell44, Me.XrTableCell46, Me.XrTableCell18, Me.XrTableCell48})
        Me.XrTableRow12.Name = "XrTableRow12"
        Me.XrTableRow12.Weight = 0.8R
        '
        'XrTableCell21
        '
        Me.XrTableCell21.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell21.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum(Iif([CompteID]= 1 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [So" &
                    "lde], 0))")})
        Me.XrTableCell21.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell21.Multiline = True
        Me.XrTableCell21.Name = "XrTableCell21"
        Me.XrTableCell21.StylePriority.UseBackColor = False
        Me.XrTableCell21.StylePriority.UseForeColor = False
        Me.XrTableCell21.Weight = 3.402536697211199R
        '
        'XrTableCell23
        '
        Me.XrTableCell23.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell23.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum(Iif([CompteID]= 2 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [So" &
                    "lde], 0))")})
        Me.XrTableCell23.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell23.Multiline = True
        Me.XrTableCell23.Name = "XrTableCell23"
        Me.XrTableCell23.StylePriority.UseBackColor = False
        Me.XrTableCell23.StylePriority.UseForeColor = False
        Me.XrTableCell23.Text = "Iif([CompteID]= 2 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [Solde]" &
    ", 0)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.XrTableCell23.Weight = 3.4025366916783155R
        '
        'XrTableCell42
        '
        Me.XrTableCell42.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell42.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum(Iif([CompteID]= 3 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [So" &
                    "lde], 0))")})
        Me.XrTableCell42.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell42.Multiline = True
        Me.XrTableCell42.Name = "XrTableCell42"
        Me.XrTableCell42.StylePriority.UseBackColor = False
        Me.XrTableCell42.StylePriority.UseForeColor = False
        Me.XrTableCell42.Weight = 3.4025366332926605R
        '
        'XrTableCell44
        '
        Me.XrTableCell44.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell44.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum(Iif([CompteID]= 4 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [So" &
                    "lde], 0))" & Global.Microsoft.VisualBasic.ChrW(10))})
        Me.XrTableCell44.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell44.Multiline = True
        Me.XrTableCell44.Name = "XrTableCell44"
        Me.XrTableCell44.StylePriority.UseBackColor = False
        Me.XrTableCell44.StylePriority.UseForeColor = False
        Me.XrTableCell44.Weight = 3.4025365943435406R
        '
        'XrTableCell46
        '
        Me.XrTableCell46.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell46.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum(Iif([CompteID]= 5 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [So" &
                    "lde], 0))" & Global.Microsoft.VisualBasic.ChrW(10))})
        Me.XrTableCell46.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell46.Multiline = True
        Me.XrTableCell46.Name = "XrTableCell46"
        Me.XrTableCell46.StylePriority.UseBackColor = False
        Me.XrTableCell46.StylePriority.UseForeColor = False
        Me.XrTableCell46.Text = "Iif([CompteID]= 5 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [Solde]" &
    ", 0)"
        Me.XrTableCell46.Weight = 3.4025366977595342R
        '
        'XrTableCell18
        '
        Me.XrTableCell18.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell18.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum(Iif([CompteID]= 9 && [datex] >= ?date1 && [datex] <= AddDays(?date1, 1) , [So" &
                    "lde], 0))")})
        Me.XrTableCell18.ForeColor = System.Drawing.Color.Black
        Me.XrTableCell18.Multiline = True
        Me.XrTableCell18.Name = "XrTableCell18"
        Me.XrTableCell18.StylePriority.UseBackColor = False
        Me.XrTableCell18.StylePriority.UseForeColor = False
        Me.XrTableCell18.Weight = 3.4025366977595337R
        '
        'XrTableCell48
        '
        Me.XrTableCell48.BackColor = System.Drawing.Color.Transparent
        Me.XrTableCell48.ExpressionBindings.AddRange(New DevExpress.XtraReports.UI.ExpressionBinding() {New DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CalculatedField2]")})
        Me.XrTableCell48.ForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.XrTableCell48.Multiline = True
        Me.XrTableCell48.Name = "XrTableCell48"
        Me.XrTableCell48.StylePriority.UseBackColor = False
        Me.XrTableCell48.StylePriority.UseForeColor = False
        Me.XrTableCell48.Weight = 3.4025366977595346R
        '
        'SqlDataSource2
        '
        Me.SqlDataSource2.ConnectionName = "PC3_damanecash_Connection"
        Me.SqlDataSource2.Name = "SqlDataSource2"
        ColumnExpression1.ColumnName = "CompteID"
        Table1.Name = "compteshistoriquedata"
        ColumnExpression1.Table = Table1
        Column1.Expression = ColumnExpression1
        ColumnExpression2.ColumnName = "CompteName"
        ColumnExpression2.Table = Table1
        Column2.Expression = ColumnExpression2
        ColumnExpression3.ColumnName = "Solde"
        ColumnExpression3.Table = Table1
        Column3.Expression = ColumnExpression3
        ColumnExpression4.ColumnName = "datex"
        ColumnExpression4.Table = Table1
        Column4.Expression = ColumnExpression4
        SelectQuery1.Columns.Add(Column1)
        SelectQuery1.Columns.Add(Column2)
        SelectQuery1.Columns.Add(Column3)
        SelectQuery1.Columns.Add(Column4)
        SelectQuery1.Name = "compteshistoriquedata"
        SelectQuery1.Tables.Add(Table1)
        Me.SqlDataSource2.Queries.AddRange(New DevExpress.DataAccess.Sql.SqlQuery() {SelectQuery1})
        Me.SqlDataSource2.ResultSchemaSerializable = resources.GetString("SqlDataSource2.ResultSchemaSerializable")
        '
        'GroupFooter1
        '
        Me.GroupFooter1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel10, Me.XrLabel9})
        Me.GroupFooter1.HeightF = 94.2499!
        Me.GroupFooter1.Name = "GroupFooter1"
        '
        'XrLabel10
        '
        Me.XrLabel10.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel10.LocationFloat = New DevExpress.Utils.PointFloat(494.5417!, 3.749974!)
        Me.XrLabel10.Multiline = True
        Me.XrLabel10.Name = "XrLabel10"
        Me.XrLabel10.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel10.SizeF = New System.Drawing.SizeF(133.3333!, 23.0!)
        Me.XrLabel10.StylePriority.UseFont = False
        Me.XrLabel10.Text = "Signature Chef:"
        '
        'XrLabel9
        '
        Me.XrLabel9.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel9.LocationFloat = New DevExpress.Utils.PointFloat(12.25001!, 3.749974!)
        Me.XrLabel9.Multiline = True
        Me.XrLabel9.Name = "XrLabel9"
        Me.XrLabel9.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel9.SizeF = New System.Drawing.SizeF(152.0833!, 23.0!)
        Me.XrLabel9.StylePriority.UseFont = False
        Me.XrLabel9.Text = "Signature Responsable:"
        '
        'SqlDataSource3
        '
        Me.SqlDataSource3.ConnectionName = "PC3_damanecash_Connection"
        Me.SqlDataSource3.Name = "SqlDataSource3"
        StoredProcQuery2.Name = "damanecash_RPAlimCais"
        StoredProcQuery2.StoredProcName = "damanecash.RPAlimCais"
        Me.SqlDataSource3.Queries.AddRange(New DevExpress.DataAccess.Sql.SqlQuery() {StoredProcQuery2})
        Me.SqlDataSource3.ResultSchemaSerializable = resources.GetString("SqlDataSource3.ResultSchemaSerializable")
        '
        'CalculatedField2
        '
        Me.CalculatedField2.DataMember = "compteshistoriquedata"
        Me.CalculatedField2.DataSource = Me.SqlDataSource2
        Me.CalculatedField2.Expression = resources.GetString("CalculatedField2.Expression")
        Me.CalculatedField2.FieldType = DevExpress.XtraReports.UI.FieldType.[Decimal]
        Me.CalculatedField2.Name = "CalculatedField2"
        '
        'CalculatedField3
        '
        Me.CalculatedField3.DataMember = "compteshistoriquedata"
        Me.CalculatedField3.DataSource = Me.SqlDataSource2
        Me.CalculatedField3.Expression = resources.GetString("CalculatedField3.Expression")
        Me.CalculatedField3.Name = "CalculatedField3"
        '
        'DetailReport3
        '
        Me.DetailReport3.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail4, Me.GroupHeader4})
        Me.DetailReport3.DataMember = "damanecash_GetFDIO"
        Me.DetailReport3.DataSource = Me.SqlDataSource4
        Me.DetailReport3.Level = 1
        Me.DetailReport3.Name = "DetailReport3"
        '
        'Detail4
        '
        Me.Detail4.HeightF = 5.166626!
        Me.Detail4.Name = "Detail4"
        '
        'GroupHeader4
        '
        Me.GroupHeader4.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrSubreport1, Me.XrTable8})
        Me.GroupHeader4.HeightF = 90.0!
        Me.GroupHeader4.Name = "GroupHeader4"
        '
        'XrSubreport1
        '
        Me.XrSubreport1.LocationFloat = New DevExpress.Utils.PointFloat(363.1667!, 10.0!)
        Me.XrSubreport1.Name = "XrSubreport1"
        Me.XrSubreport1.ParameterBindings.Add(New DevExpress.XtraReports.UI.ParameterBinding("Parameter1", Me.date1))
        Me.XrSubreport1.ReportSource = New RDET.RP1()
        Me.XrSubreport1.SizeF = New System.Drawing.SizeF(401.8334!, 79.99998!)
        '
        'XrTable8
        '
        Me.XrTable8.LocationFloat = New DevExpress.Utils.PointFloat(12.25001!, 10.0!)
        Me.XrTable8.Name = "XrTable8"
        Me.XrTable8.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96.0!)
        Me.XrTable8.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow8, Me.XrTableRow9, Me.XrTableRow11, Me.XrTableRow10})
        Me.XrTable8.SizeF = New System.Drawing.SizeF(350.9167!, 80.0!)
        '
        'XrTableRow8
        '
        Me.XrTableRow8.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell29, Me.XrTableCell30, Me.XrTableCell31})
        Me.XrTableRow8.Name = "XrTableRow8"
        Me.XrTableRow8.Weight = 0.8R
        '
        'XrTableCell29
        '
        Me.XrTableCell29.Borders = DevExpress.XtraPrinting.BorderSide.Right
        Me.XrTableCell29.BorderWidth = 1.0!
        Me.XrTableCell29.Multiline = True
        Me.XrTableCell29.Name = "XrTableCell29"
        Me.XrTableCell29.StylePriority.UseBorders = False
        Me.XrTableCell29.StylePriority.UseBorderWidth = False
        Me.XrTableCell29.Weight = 0.91450981829642264R
        '
        'XrTableCell30
        '
        Me.XrTableCell30.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell30.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right), DevExpress.XtraPrinting.BorderSide)
        Me.XrTableCell30.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableCell30.Multiline = True
        Me.XrTableCell30.Name = "XrTableCell30"
        Me.XrTableCell30.StylePriority.UseBackColor = False
        Me.XrTableCell30.StylePriority.UseBorders = False
        Me.XrTableCell30.StylePriority.UseFont = False
        Me.XrTableCell30.StylePriority.UseTextAlignment = False
        Me.XrTableCell30.Text = "IN + F"
        Me.XrTableCell30.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        Me.XrTableCell30.Weight = 0.96805929433913229R
        '
        'XrTableCell31
        '
        Me.XrTableCell31.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell31.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right), DevExpress.XtraPrinting.BorderSide)
        Me.XrTableCell31.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableCell31.Multiline = True
        Me.XrTableCell31.Name = "XrTableCell31"
        Me.XrTableCell31.StylePriority.UseBackColor = False
        Me.XrTableCell31.StylePriority.UseBorders = False
        Me.XrTableCell31.StylePriority.UseFont = False
        Me.XrTableCell31.StylePriority.UseTextAlignment = False
        Me.XrTableCell31.Text = "OUT + F"
        Me.XrTableCell31.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        Me.XrTableCell31.Weight = 1.117430887364445R
        '
        'XrTableRow9
        '
        Me.XrTableRow9.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell32, Me.XrTableCell33, Me.XrTableCell34})
        Me.XrTableRow9.Name = "XrTableRow9"
        Me.XrTableRow9.Weight = 0.8R
        '
        'XrTableCell32
        '
        Me.XrTableCell32.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell32.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right), DevExpress.XtraPrinting.BorderSide)
        Me.XrTableCell32.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableCell32.Multiline = True
        Me.XrTableCell32.Name = "XrTableCell32"
        Me.XrTableCell32.StylePriority.UseBackColor = False
        Me.XrTableCell32.StylePriority.UseBorders = False
        Me.XrTableCell32.StylePriority.UseFont = False
        Me.XrTableCell32.Text = "D.pay"
        Me.XrTableCell32.Weight = 0.91450981829642264R
        '
        'XrTableCell33
        '
        Me.XrTableCell33.BackColor = System.Drawing.Color.White
        Me.XrTableCell33.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right), DevExpress.XtraPrinting.BorderSide)
        Me.XrTableCell33.EvenStyleName = "XrControlStyle3"
        Me.XrTableCell33.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell33.Multiline = True
        Me.XrTableCell33.Name = "XrTableCell33"
        Me.XrTableCell33.StylePriority.UseBackColor = False
        Me.XrTableCell33.StylePriority.UseBorders = False
        Me.XrTableCell33.StylePriority.UseFont = False
        Me.XrTableCell33.Text = "[CalculatedField7] + [CalculatedField11]"
        Me.XrTableCell33.Weight = 0.96805929433913229R
        '
        'XrTableCell34
        '
        Me.XrTableCell34.BackColor = System.Drawing.Color.White
        Me.XrTableCell34.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right), DevExpress.XtraPrinting.BorderSide)
        Me.XrTableCell34.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell34.Multiline = True
        Me.XrTableCell34.Name = "XrTableCell34"
        Me.XrTableCell34.StylePriority.UseBackColor = False
        Me.XrTableCell34.StylePriority.UseBorders = False
        Me.XrTableCell34.StylePriority.UseFont = False
        Me.XrTableCell34.Text = "[CalculatedField8] + [CalculatedField4]"
        Me.XrTableCell34.Weight = 1.117430887364445R
        '
        'XrTableRow11
        '
        Me.XrTableRow11.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell38, Me.XrTableCell39, Me.XrTableCell40})
        Me.XrTableRow11.Name = "XrTableRow11"
        Me.XrTableRow11.Weight = 0.8R
        '
        'XrTableCell38
        '
        Me.XrTableCell38.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell38.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right), DevExpress.XtraPrinting.BorderSide)
        Me.XrTableCell38.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableCell38.Multiline = True
        Me.XrTableCell38.Name = "XrTableCell38"
        Me.XrTableCell38.StylePriority.UseBackColor = False
        Me.XrTableCell38.StylePriority.UseBorders = False
        Me.XrTableCell38.StylePriority.UseFont = False
        Me.XrTableCell38.Text = "Fundex"
        Me.XrTableCell38.Weight = 0.91450981829642264R
        '
        'XrTableCell39
        '
        Me.XrTableCell39.BackColor = System.Drawing.Color.White
        Me.XrTableCell39.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right), DevExpress.XtraPrinting.BorderSide)
        Me.XrTableCell39.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell39.Multiline = True
        Me.XrTableCell39.Name = "XrTableCell39"
        Me.XrTableCell39.StylePriority.UseBackColor = False
        Me.XrTableCell39.StylePriority.UseBorders = False
        Me.XrTableCell39.StylePriority.UseFont = False
        Me.XrTableCell39.Text = "[CalculatedField5] + [CalculatedField12]"
        Me.XrTableCell39.Weight = 0.96805929433913229R
        '
        'XrTableCell40
        '
        Me.XrTableCell40.BackColor = System.Drawing.Color.White
        Me.XrTableCell40.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right), DevExpress.XtraPrinting.BorderSide)
        Me.XrTableCell40.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell40.Multiline = True
        Me.XrTableCell40.Name = "XrTableCell40"
        Me.XrTableCell40.StylePriority.UseBackColor = False
        Me.XrTableCell40.StylePriority.UseBorders = False
        Me.XrTableCell40.StylePriority.UseFont = False
        Me.XrTableCell40.Text = "[CalculatedField6] + [CalculatedField13]"
        Me.XrTableCell40.Weight = 1.117430887364445R
        '
        'XrTableRow10
        '
        Me.XrTableRow10.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell35, Me.XrTableCell36, Me.XrTableCell37})
        Me.XrTableRow10.Name = "XrTableRow10"
        Me.XrTableRow10.Weight = 0.79999999999999993R
        '
        'XrTableCell35
        '
        Me.XrTableCell35.BackColor = System.Drawing.Color.Gainsboro
        Me.XrTableCell35.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrTableCell35.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableCell35.Multiline = True
        Me.XrTableCell35.Name = "XrTableCell35"
        Me.XrTableCell35.StylePriority.UseBackColor = False
        Me.XrTableCell35.StylePriority.UseBorders = False
        Me.XrTableCell35.StylePriority.UseFont = False
        Me.XrTableCell35.Text = "Total"
        Me.XrTableCell35.Weight = 0.91450981829642264R
        '
        'XrTableCell36
        '
        Me.XrTableCell36.BackColor = System.Drawing.Color.White
        Me.XrTableCell36.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrTableCell36.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell36.Multiline = True
        Me.XrTableCell36.Name = "XrTableCell36"
        Me.XrTableCell36.StylePriority.UseBackColor = False
        Me.XrTableCell36.StylePriority.UseBorders = False
        Me.XrTableCell36.StylePriority.UseFont = False
        Me.XrTableCell36.Text = "[CalculatedField16]"
        Me.XrTableCell36.Weight = 0.96805929433913229R
        '
        'XrTableCell37
        '
        Me.XrTableCell37.BackColor = System.Drawing.Color.White
        Me.XrTableCell37.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrTableCell37.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell37.Multiline = True
        Me.XrTableCell37.Name = "XrTableCell37"
        Me.XrTableCell37.StylePriority.UseBackColor = False
        Me.XrTableCell37.StylePriority.UseBorders = False
        Me.XrTableCell37.StylePriority.UseFont = False
        Me.XrTableCell37.Text = "[CalculatedField17]"
        Me.XrTableCell37.Weight = 1.117430887364445R
        '
        'SqlDataSource4
        '
        Me.SqlDataSource4.ConnectionName = "PC3_damanecash_Connection"
        Me.SqlDataSource4.Name = "SqlDataSource4"
        StoredProcQuery3.Name = "damanecash_GetFDIO"
        StoredProcQuery3.StoredProcName = "damanecash.GetFDIO"
        Me.SqlDataSource4.Queries.AddRange(New DevExpress.DataAccess.Sql.SqlQuery() {StoredProcQuery3})
        Me.SqlDataSource4.ResultSchemaSerializable = resources.GetString("SqlDataSource4.ResultSchemaSerializable")
        '
        'CalculatedField4
        '
        Me.CalculatedField4.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField4.DataSource = Me.SqlDataSource4
        Me.CalculatedField4.DisplayName = "f dp OUT"
        Me.CalculatedField4.Expression = "Sum(Iif([ID] = 1 && [Typ] = 'OUT' &&  [datex] >= ?date1 && [datex] <= AddDays(?da" &
    "te1, 1) ,[Frais], 0))" & Global.Microsoft.VisualBasic.ChrW(10)
        Me.CalculatedField4.Name = "CalculatedField4"
        '
        'CalculatedField5
        '
        Me.CalculatedField5.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField5.DataSource = Me.SqlDataSource4
        Me.CalculatedField5.DisplayName = "Fundex IN"
        Me.CalculatedField5.Expression = "Sum(Iif([ID] = 2 && [Typ] = 'IN' &&  [datex] >= ?date1 && [datex] <= AddDays(?dat" &
    "e1, 1) , [Mtn], 0))"
        Me.CalculatedField5.Name = "CalculatedField5"
        '
        'CalculatedField6
        '
        Me.CalculatedField6.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField6.DataSource = Me.SqlDataSource4
        Me.CalculatedField6.DisplayName = "Fundex OUT"
        Me.CalculatedField6.Expression = "Sum(Iif([ID] = 2 && [Typ] = 'OUT' &&  [datex] >= ?date1 && [datex] <= AddDays(?da" &
    "te1, 1) , [Mtn], 0))"
        Me.CalculatedField6.Name = "CalculatedField6"
        '
        'CalculatedField7
        '
        Me.CalculatedField7.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField7.DataSource = Me.SqlDataSource4
        Me.CalculatedField7.DisplayName = "dp IN"
        Me.CalculatedField7.Expression = "Sum(Iif([ID] = 1 && [Typ] = 'IN' &&  [datex] >= ?date1 && [datex] <= AddDays(?dat" &
    "e1, 1) , [Mtn], 0))"
        Me.CalculatedField7.Name = "CalculatedField7"
        '
        'CalculatedField8
        '
        Me.CalculatedField8.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField8.DataSource = Me.SqlDataSource4
        Me.CalculatedField8.DisplayName = "dp out"
        Me.CalculatedField8.Expression = "Sum(Iif([ID] = 1 && [Typ] = 'OUT' &&  [datex] >= ?date1 && [datex] <= AddDays(?da" &
    "te1, 1) , [Mtn], 0))"
        Me.CalculatedField8.Name = "CalculatedField8"
        '
        'CalculatedField9
        '
        Me.CalculatedField9.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField9.DataSource = Me.SqlDataSource4
        Me.CalculatedField9.DisplayName = "tlt IN"
        Me.CalculatedField9.Expression = "Sum(Iif(([ID] = 1 ||  [ID] = 2 ) && [Typ] = 'IN' && [datex] >= ?date1 && [datex] " &
    "<= AddDays(?date1, 1) , [Mtn], 0))" & Global.Microsoft.VisualBasic.ChrW(10)
        Me.CalculatedField9.Name = "CalculatedField9"
        '
        'CalculatedField10
        '
        Me.CalculatedField10.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField10.DataSource = Me.SqlDataSource4
        Me.CalculatedField10.DisplayName = "tlt OUT"
        Me.CalculatedField10.Expression = "Sum(Iif(([ID] = 1 ||  [ID] = 2 ) && [Typ] = 'OUT' && [datex] >= ?date1 && [datex]" &
    " <= AddDays(?date1, 1) , [Mtn], 0))" & Global.Microsoft.VisualBasic.ChrW(10)
        Me.CalculatedField10.Name = "CalculatedField10"
        '
        'CalculatedField11
        '
        Me.CalculatedField11.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField11.DataSource = Me.SqlDataSource4
        Me.CalculatedField11.DisplayName = "F dp IN"
        Me.CalculatedField11.Expression = "Sum(Iif([ID] = 1 && [Typ] = 'IN' &&  [datex] >= ?date1 && [datex] <= AddDays(?dat" &
    "e1, 1) , [Frais], 0))" & Global.Microsoft.VisualBasic.ChrW(10)
        Me.CalculatedField11.Name = "CalculatedField11"
        '
        'CalculatedField12
        '
        Me.CalculatedField12.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField12.DataSource = Me.SqlDataSource4
        Me.CalculatedField12.DisplayName = "f fun in"
        Me.CalculatedField12.Expression = "Sum(Iif([ID] = 2 && [Typ] = 'IN' &&  [datex] >= ?date1 && [datex] <= AddDays(?dat" &
    "e1, 1) , [Frais], 0))" & Global.Microsoft.VisualBasic.ChrW(10)
        Me.CalculatedField12.Name = "CalculatedField12"
        '
        'CalculatedField13
        '
        Me.CalculatedField13.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField13.DataSource = Me.SqlDataSource4
        Me.CalculatedField13.DisplayName = "f fun out"
        Me.CalculatedField13.Expression = "Sum(Iif([ID] = 2 && [Typ] = 'OUT' &&  [datex] >= ?date1 && [datex] <= AddDays(?da" &
    "te1, 1) , [Frais], 0))" & Global.Microsoft.VisualBasic.ChrW(10)
        Me.CalculatedField13.Name = "CalculatedField13"
        '
        'CalculatedField14
        '
        Me.CalculatedField14.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField14.DataSource = Me.SqlDataSource4
        Me.CalculatedField14.DisplayName = "tlt In frais"
        Me.CalculatedField14.Expression = "Sum(Iif(([ID] = 1 ||  [ID] = 2 ) && [Typ] = 'IN' && [datex] >= ?date1 && [datex] " &
    "<= AddDays(?date1, 1) , [Frais], 0))" & Global.Microsoft.VisualBasic.ChrW(10)
        Me.CalculatedField14.Name = "CalculatedField14"
        '
        'CalculatedField15
        '
        Me.CalculatedField15.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField15.DataSource = Me.SqlDataSource4
        Me.CalculatedField15.DisplayName = "tlt Out frais"
        Me.CalculatedField15.Expression = "Sum(Iif(([ID] = 1 ||  [ID] = 2 ) && [Typ] = 'OUT' && [datex] >= ?date1 && [datex]" &
    " <= AddDays(?date1, 1) , [Frais], 0))" & Global.Microsoft.VisualBasic.ChrW(10)
        Me.CalculatedField15.Name = "CalculatedField15"
        '
        'CalculatedField16
        '
        Me.CalculatedField16.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField16.DataSource = Me.SqlDataSource4
        Me.CalculatedField16.DisplayName = "tlt in f"
        Me.CalculatedField16.Expression = "[CalculatedField9]+[CalculatedField14]"
        Me.CalculatedField16.Name = "CalculatedField16"
        '
        'CalculatedField17
        '
        Me.CalculatedField17.DataMember = "damanecash_GetFDIO"
        Me.CalculatedField17.DataSource = Me.SqlDataSource4
        Me.CalculatedField17.DisplayName = "tlt out f"
        Me.CalculatedField17.Expression = "[CalculatedField10]+[CalculatedField15]"
        Me.CalculatedField17.Name = "CalculatedField17"
        '
        'Detail
        '
        Me.Detail.HeightF = 0!
        Me.Detail.Name = "Detail"
        '
        'XrControlStyle3
        '
        Me.XrControlStyle3.Name = "XrControlStyle3"
        Me.XrControlStyle3.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        '
        'CalculatedField18
        '
        Me.CalculatedField18.DataMember = "damanecash_RPAlimCais"
        Me.CalculatedField18.DataSource = Me.SqlDataSource3
        Me.CalculatedField18.Name = "CalculatedField18"
        '
        'JournalDC
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.TopMargin, Me.BottomMargin, Me.Detail, Me.GroupHeader1, Me.ReportHeader, Me.DetailReport, Me.DetailReport1, Me.GroupFooter1, Me.DetailReport3})
        Me.CalculatedFields.AddRange(New DevExpress.XtraReports.UI.CalculatedField() {Me.CalculatedField1, Me.CalculatedField2, Me.CalculatedField3, Me.CalculatedField4, Me.CalculatedField5, Me.CalculatedField6, Me.CalculatedField7, Me.CalculatedField8, Me.CalculatedField9, Me.CalculatedField10, Me.CalculatedField11, Me.CalculatedField12, Me.CalculatedField13, Me.CalculatedField14, Me.CalculatedField15, Me.CalculatedField16, Me.CalculatedField17, Me.CalculatedField18})
        Me.ComponentStorage.AddRange(New System.ComponentModel.IComponent() {Me.SqlDataSource1, Me.SqlDataSource2, Me.SqlDataSource3, Me.SqlDataSource4})
        Me.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.Margins = New System.Drawing.Printing.Margins(33, 29, 17, 42)
        Me.PageColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.PageHeight = 1169
        Me.PageWidth = 827
        Me.PaperKind = System.Drawing.Printing.PaperKind.A4
        Me.Parameters.AddRange(New DevExpress.XtraReports.Parameters.Parameter() {Me.date1})
        Me.ScriptLanguage = DevExpress.XtraReports.ScriptLanguage.VisualBasic
        Me.StyleSheet.AddRange(New DevExpress.XtraReports.UI.XRControlStyle() {Me.XrControlStyle1, Me.XrControlStyle2, Me.XrControlStyle3})
        Me.Version = "21.2"
        CType(Me.XrTable1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrTable2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrTable3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrTable9, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrTable4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrTable8, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub

    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents GroupHeader1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents XrLabel5 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel4 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents ReportHeader As DevExpress.XtraReports.UI.ReportHeaderBand
    Friend WithEvents XrPictureBox1 As DevExpress.XtraReports.UI.XRPictureBox
    Friend WithEvents XrLabel3 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel2 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel1 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents DetailReport As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents Detail1 As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents SqlDataSource1 As DevExpress.DataAccess.Sql.SqlDataSource
    Friend WithEvents XrControlStyle1 As DevExpress.XtraReports.UI.XRControlStyle
    Friend WithEvents date1 As DevExpress.XtraReports.Parameters.Parameter
    Friend WithEvents GroupHeaderBand1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents XrTable2 As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRow2 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell7 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell8 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell9 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell10 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell11 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell12 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents GroupHeaderBand2 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents CalculatedField1 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents XrLabel6 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents GroupHeaderBand3 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents GroupFooterBand2 As DevExpress.XtraReports.UI.GroupFooterBand
    Friend WithEvents XrTable3 As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRow3 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell14 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell15 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrControlStyle2 As DevExpress.XtraReports.UI.XRControlStyle
    Friend WithEvents DetailReport1 As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents Detail2 As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents SqlDataSource2 As DevExpress.DataAccess.Sql.SqlDataSource
    Friend WithEvents XrPageInfo2 As DevExpress.XtraReports.UI.XRPageInfo
    Friend WithEvents XrPageInfo1 As DevExpress.XtraReports.UI.XRPageInfo
    Friend WithEvents GroupFooter1 As DevExpress.XtraReports.UI.GroupFooterBand
    Friend WithEvents XrLabel10 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel9 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel19 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel20 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel7 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents SqlDataSource3 As DevExpress.DataAccess.Sql.SqlDataSource
    Friend WithEvents GroupHeader2 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents XrLabel8 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrTable4 As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRow4 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell19 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents CalculatedField2 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField3 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents XrLabel14 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents DetailReport3 As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents GroupHeader4 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents SqlDataSource4 As DevExpress.DataAccess.Sql.SqlDataSource
    Friend WithEvents CalculatedField4 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField5 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField6 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField7 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField8 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField9 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField10 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents XrTableCell13 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents CalculatedField11 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField12 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField13 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField14 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField15 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField16 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents CalculatedField17 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents XrTable8 As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRow8 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell29 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell30 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell31 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableRow9 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell32 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell33 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell34 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableRow11 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell38 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell39 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell40 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableRow10 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell35 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell36 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell37 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell22 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell41 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell43 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell45 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell16 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableRow12 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell21 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell23 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell42 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell44 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell46 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell18 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTable9 As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRow13 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell49 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell50 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell51 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell52 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell53 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell54 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell55 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableRow14 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell56 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell57 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell58 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell59 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell60 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell61 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell62 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell47 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell48 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents Detail4 As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents XrSubreport1 As DevExpress.XtraReports.UI.XRSubreport
    Friend WithEvents XrControlStyle3 As DevExpress.XtraReports.UI.XRControlStyle
    Friend WithEvents CalculatedField18 As DevExpress.XtraReports.UI.CalculatedField
    Friend WithEvents XrTable1 As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRow1 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell6 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell1 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell2 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell3 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell4 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell5 As DevExpress.XtraReports.UI.XRTableCell
End Class
