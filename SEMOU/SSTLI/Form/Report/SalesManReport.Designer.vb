<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SalesManReport
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SalesManReport))
        Me.GroupPanel7 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel3 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel5 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtMsg = New System.Windows.Forms.Label()
        Me.BranchGrid = New DevComponents.DotNetBar.Controls.DataGridViewX()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn22 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn21 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column14 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column11 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column13 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GroupPanel4 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtCount = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.CmdExport = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdView = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdSave = New DevExpress.XtraEditors.SimpleButton()
        Me.GroupPanel6 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.CboType = New System.Windows.Forms.ComboBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.CmdDelSalesMan = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtSalesManGuid = New System.Windows.Forms.Label()
        Me.CmdFindSalesMan = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtSalesManName = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtFromDate = New System.Windows.Forms.DateTimePicker()
        Me.TxtToDate = New System.Windows.Forms.DateTimePicker()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CmdFindDepart = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdDelBranch = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtBranchGuid = New System.Windows.Forms.Label()
        Me.TxtBranchName = New System.Windows.Forms.TextBox()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.GroupPanel7.SuspendLayout()
        Me.GroupPanel3.SuspendLayout()
        Me.GroupPanel5.SuspendLayout()
        CType(Me.BranchGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupPanel4.SuspendLayout()
        Me.GroupPanel1.SuspendLayout()
        Me.GroupPanel2.SuspendLayout()
        Me.GroupPanel6.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel7
        '
        Me.GroupPanel7.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel7.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel7.Controls.Add(Me.GroupPanel3)
        Me.GroupPanel7.Controls.Add(Me.GroupPanel1)
        Me.GroupPanel7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel7.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel7.Name = "GroupPanel7"
        Me.GroupPanel7.Size = New System.Drawing.Size(1171, 511)
        '
        '
        '
        Me.GroupPanel7.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel7.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel7.Style.BackColorGradientAngle = 90
        Me.GroupPanel7.Style.BorderBottomWidth = 1
        Me.GroupPanel7.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel7.Style.BorderLeftWidth = 1
        Me.GroupPanel7.Style.BorderRightWidth = 1
        Me.GroupPanel7.Style.BorderTopWidth = 1
        Me.GroupPanel7.Style.CornerDiameter = 4
        Me.GroupPanel7.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel7.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel7.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel7.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel7.TabIndex = 1034
        '
        'GroupPanel3
        '
        Me.GroupPanel3.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel3.Controls.Add(Me.GroupPanel5)
        Me.GroupPanel3.Controls.Add(Me.GroupPanel4)
        Me.GroupPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel3.Location = New System.Drawing.Point(0, 111)
        Me.GroupPanel3.Name = "GroupPanel3"
        Me.GroupPanel3.Size = New System.Drawing.Size(1171, 400)
        '
        '
        '
        Me.GroupPanel3.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel3.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel3.Style.BackColorGradientAngle = 90
        Me.GroupPanel3.Style.BorderBottomWidth = 1
        Me.GroupPanel3.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel3.Style.BorderLeftWidth = 1
        Me.GroupPanel3.Style.BorderRightWidth = 1
        Me.GroupPanel3.Style.BorderTopWidth = 1
        Me.GroupPanel3.Style.CornerDiameter = 4
        Me.GroupPanel3.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel3.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel3.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel3.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel3.TabIndex = 1034
        '
        'GroupPanel5
        '
        Me.GroupPanel5.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel5.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel5.Controls.Add(Me.TxtMsg)
        Me.GroupPanel5.Controls.Add(Me.BranchGrid)
        Me.GroupPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel5.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel5.Location = New System.Drawing.Point(0, 32)
        Me.GroupPanel5.Name = "GroupPanel5"
        Me.GroupPanel5.Size = New System.Drawing.Size(1171, 368)
        '
        '
        '
        Me.GroupPanel5.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel5.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel5.Style.BackColorGradientAngle = 90
        Me.GroupPanel5.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel5.Style.BorderBottomWidth = 1
        Me.GroupPanel5.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel5.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel5.Style.BorderLeftWidth = 1
        Me.GroupPanel5.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel5.Style.BorderRightWidth = 1
        Me.GroupPanel5.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel5.Style.BorderTopWidth = 1
        Me.GroupPanel5.Style.CornerDiameter = 4
        Me.GroupPanel5.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel5.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel5.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel5.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel5.TabIndex = 616
        '
        'TxtMsg
        '
        Me.TxtMsg.AutoSize = True
        Me.TxtMsg.BackColor = System.Drawing.Color.Transparent
        Me.TxtMsg.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtMsg.ForeColor = System.Drawing.Color.Red
        Me.TxtMsg.Location = New System.Drawing.Point(474, 139)
        Me.TxtMsg.Name = "TxtMsg"
        Me.TxtMsg.Size = New System.Drawing.Size(217, 33)
        Me.TxtMsg.TabIndex = 1062
        Me.TxtMsg.Text = "لا تتوفر بيانات للعرض"
        Me.TxtMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.TxtMsg.Visible = False
        '
        'BranchGrid
        '
        Me.BranchGrid.AllowUserToAddRows = False
        Me.BranchGrid.AllowUserToDeleteRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.FloralWhite
        Me.BranchGrid.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.BranchGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.BranchGrid.BackgroundColor = System.Drawing.Color.White
        Me.BranchGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.BranchGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.BranchGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column2, Me.DataGridViewTextBoxColumn22, Me.Column5, Me.DataGridViewTextBoxColumn21, Me.Column14, Me.Column1, Me.Column11, Me.Column13})
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        DataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BranchGrid.DefaultCellStyle = DataGridViewCellStyle10
        Me.BranchGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BranchGrid.GridColor = System.Drawing.Color.FromArgb(CType(CType(208, Byte), Integer), CType(CType(215, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.BranchGrid.Location = New System.Drawing.Point(0, 0)
        Me.BranchGrid.Name = "BranchGrid"
        Me.BranchGrid.ReadOnly = True
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle11.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        DataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.BranchGrid.RowHeadersDefaultCellStyle = DataGridViewCellStyle11
        Me.BranchGrid.RowHeadersVisible = False
        Me.BranchGrid.RowTemplate.Height = 33
        Me.BranchGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.BranchGrid.Size = New System.Drawing.Size(1165, 362)
        Me.BranchGrid.TabIndex = 702
        '
        'Column2
        '
        Me.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column2.DefaultCellStyle = DataGridViewCellStyle2
        Me.Column2.HeaderText = "رقم الاستمارة"
        Me.Column2.Name = "Column2"
        Me.Column2.ReadOnly = True
        Me.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column2.Width = 89
        '
        'DataGridViewTextBoxColumn22
        '
        Me.DataGridViewTextBoxColumn22.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle3.Format = "d"
        DataGridViewCellStyle3.NullValue = Nothing
        Me.DataGridViewTextBoxColumn22.DefaultCellStyle = DataGridViewCellStyle3
        Me.DataGridViewTextBoxColumn22.HeaderText = "المندوب"
        Me.DataGridViewTextBoxColumn22.Name = "DataGridViewTextBoxColumn22"
        Me.DataGridViewTextBoxColumn22.ReadOnly = True
        Me.DataGridViewTextBoxColumn22.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.DataGridViewTextBoxColumn22.Width = 61
        '
        'Column5
        '
        Me.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column5.DefaultCellStyle = DataGridViewCellStyle4
        Me.Column5.HeaderText = "الفرع"
        Me.Column5.Name = "Column5"
        Me.Column5.ReadOnly = True
        Me.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column5.Width = 43
        '
        'DataGridViewTextBoxColumn21
        '
        Me.DataGridViewTextBoxColumn21.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.Format = "t"
        DataGridViewCellStyle5.NullValue = Nothing
        Me.DataGridViewTextBoxColumn21.DefaultCellStyle = DataGridViewCellStyle5
        Me.DataGridViewTextBoxColumn21.HeaderText = "اسم الطالب"
        Me.DataGridViewTextBoxColumn21.Name = "DataGridViewTextBoxColumn21"
        Me.DataGridViewTextBoxColumn21.ReadOnly = True
        Me.DataGridViewTextBoxColumn21.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.DataGridViewTextBoxColumn21.Width = 81
        '
        'Column14
        '
        Me.Column14.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column14.DefaultCellStyle = DataGridViewCellStyle6
        Me.Column14.HeaderText = "رقم الهوية"
        Me.Column14.Name = "Column14"
        Me.Column14.ReadOnly = True
        Me.Column14.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column14.Width = 71
        '
        'Column1
        '
        Me.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column1.DefaultCellStyle = DataGridViewCellStyle7
        Me.Column1.HeaderText = "رقم الجوال"
        Me.Column1.Name = "Column1"
        Me.Column1.ReadOnly = True
        Me.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column1.Width = 76
        '
        'Column11
        '
        Me.Column11.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column11.DefaultCellStyle = DataGridViewCellStyle8
        Me.Column11.HeaderText = "البيان"
        Me.Column11.Name = "Column11"
        Me.Column11.ReadOnly = True
        Me.Column11.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column11.Width = 45
        '
        'Column13
        '
        Me.Column13.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column13.DefaultCellStyle = DataGridViewCellStyle9
        Me.Column13.HeaderText = "نوع التسجيل"
        Me.Column13.Name = "Column13"
        Me.Column13.ReadOnly = True
        Me.Column13.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column13.Width = 85
        '
        'GroupPanel4
        '
        Me.GroupPanel4.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel4.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel4.Controls.Add(Me.TxtCount)
        Me.GroupPanel4.Controls.Add(Me.Label6)
        Me.GroupPanel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel4.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel4.Name = "GroupPanel4"
        Me.GroupPanel4.Size = New System.Drawing.Size(1171, 32)
        '
        '
        '
        Me.GroupPanel4.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel4.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel4.Style.BackColorGradientAngle = 90
        Me.GroupPanel4.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel4.Style.BorderBottomWidth = 1
        Me.GroupPanel4.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel4.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel4.Style.BorderLeftWidth = 1
        Me.GroupPanel4.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel4.Style.BorderRightWidth = 1
        Me.GroupPanel4.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel4.Style.BorderTopWidth = 1
        Me.GroupPanel4.Style.CornerDiameter = 4
        Me.GroupPanel4.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel4.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel4.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel4.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel4.TabIndex = 615
        '
        'TxtCount
        '
        Me.TxtCount.AutoSize = True
        Me.TxtCount.BackColor = System.Drawing.Color.White
        Me.TxtCount.Dock = System.Windows.Forms.DockStyle.Right
        Me.TxtCount.Font = New System.Drawing.Font("Calibri", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCount.ForeColor = System.Drawing.Color.Red
        Me.TxtCount.Location = New System.Drawing.Point(1045, 0)
        Me.TxtCount.Name = "TxtCount"
        Me.TxtCount.Size = New System.Drawing.Size(24, 26)
        Me.TxtCount.TabIndex = 864
        Me.TxtCount.Text = "--"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.White
        Me.Label6.Dock = System.Windows.Forms.DockStyle.Right
        Me.Label6.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(1069, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(96, 19)
        Me.Label6.TabIndex = 862
        Me.Label6.Text = "عدد الإستمارات"
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.GroupPanel2)
        Me.GroupPanel1.Controls.Add(Me.GroupPanel6)
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(1171, 111)
        '
        '
        '
        Me.GroupPanel1.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel1.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel1.Style.BackColorGradientAngle = 90
        Me.GroupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderBottomWidth = 1
        Me.GroupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderLeftWidth = 1
        Me.GroupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderRightWidth = 1
        Me.GroupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderTopWidth = 1
        Me.GroupPanel1.Style.CornerDiameter = 4
        Me.GroupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel1.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel1.TabIndex = 613
        '
        'GroupPanel2
        '
        Me.GroupPanel2.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel2.Controls.Add(Me.CmdExport)
        Me.GroupPanel2.Controls.Add(Me.CmdView)
        Me.GroupPanel2.Controls.Add(Me.CmdExit)
        Me.GroupPanel2.Controls.Add(Me.CmdSave)
        Me.GroupPanel2.Dock = System.Windows.Forms.DockStyle.Right
        Me.GroupPanel2.Location = New System.Drawing.Point(130, 0)
        Me.GroupPanel2.Name = "GroupPanel2"
        Me.GroupPanel2.Size = New System.Drawing.Size(335, 105)
        '
        '
        '
        Me.GroupPanel2.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel2.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel2.Style.BackColorGradientAngle = 90
        Me.GroupPanel2.Style.BorderBottomWidth = 1
        Me.GroupPanel2.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel2.Style.BorderLeftWidth = 1
        Me.GroupPanel2.Style.BorderRightWidth = 1
        Me.GroupPanel2.Style.BorderTopWidth = 1
        Me.GroupPanel2.Style.CornerDiameter = 4
        Me.GroupPanel2.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel2.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel2.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel2.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel2.TabIndex = 1033
        '
        'CmdExport
        '
        Me.CmdExport.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdExport.Appearance.ForeColor = System.Drawing.Color.Black
        Me.CmdExport.Appearance.Options.UseFont = True
        Me.CmdExport.Appearance.Options.UseForeColor = True
        Me.CmdExport.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter
        Me.CmdExport.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdExport.ImageOptions.SvgImage = CType(resources.GetObject("CmdExport.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdExport.Location = New System.Drawing.Point(115, 21)
        Me.CmdExport.Name = "CmdExport"
        Me.CmdExport.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdExport.Size = New System.Drawing.Size(64, 65)
        Me.CmdExport.TabIndex = 315
        Me.CmdExport.Text = "تصدير"
        '
        'CmdView
        '
        Me.CmdView.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdView.Appearance.ForeColor = System.Drawing.Color.Black
        Me.CmdView.Appearance.Options.UseFont = True
        Me.CmdView.Appearance.Options.UseForeColor = True
        Me.CmdView.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter
        Me.CmdView.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdView.ImageOptions.SvgImage = CType(resources.GetObject("CmdView.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdView.Location = New System.Drawing.Point(255, 21)
        Me.CmdView.Name = "CmdView"
        Me.CmdView.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdView.Size = New System.Drawing.Size(64, 65)
        Me.CmdView.TabIndex = 314
        Me.CmdView.Text = "عرض"
        '
        'CmdExit
        '
        Me.CmdExit.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdExit.Appearance.ForeColor = System.Drawing.Color.Black
        Me.CmdExit.Appearance.Options.UseFont = True
        Me.CmdExit.Appearance.Options.UseForeColor = True
        Me.CmdExit.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter
        Me.CmdExit.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdExit.ImageOptions.SvgImage = CType(resources.GetObject("CmdExit.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdExit.Location = New System.Drawing.Point(45, 21)
        Me.CmdExit.Name = "CmdExit"
        Me.CmdExit.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdExit.Size = New System.Drawing.Size(64, 65)
        Me.CmdExit.TabIndex = 313
        Me.CmdExit.Text = "خروج"
        '
        'CmdSave
        '
        Me.CmdSave.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdSave.Appearance.ForeColor = System.Drawing.Color.Black
        Me.CmdSave.Appearance.Options.UseFont = True
        Me.CmdSave.Appearance.Options.UseForeColor = True
        Me.CmdSave.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter
        Me.CmdSave.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdSave.ImageOptions.SvgImage = CType(resources.GetObject("CmdSave.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdSave.Location = New System.Drawing.Point(185, 21)
        Me.CmdSave.Name = "CmdSave"
        Me.CmdSave.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdSave.Size = New System.Drawing.Size(64, 65)
        Me.CmdSave.TabIndex = 311
        Me.CmdSave.Text = "طباعه"
        '
        'GroupPanel6
        '
        Me.GroupPanel6.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel6.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel6.Controls.Add(Me.CboType)
        Me.GroupPanel6.Controls.Add(Me.Label7)
        Me.GroupPanel6.Controls.Add(Me.CmdDelSalesMan)
        Me.GroupPanel6.Controls.Add(Me.TxtSalesManGuid)
        Me.GroupPanel6.Controls.Add(Me.CmdFindSalesMan)
        Me.GroupPanel6.Controls.Add(Me.TxtSalesManName)
        Me.GroupPanel6.Controls.Add(Me.Label5)
        Me.GroupPanel6.Controls.Add(Me.Label2)
        Me.GroupPanel6.Controls.Add(Me.Label1)
        Me.GroupPanel6.Controls.Add(Me.TxtFromDate)
        Me.GroupPanel6.Controls.Add(Me.TxtToDate)
        Me.GroupPanel6.Controls.Add(Me.Label4)
        Me.GroupPanel6.Controls.Add(Me.CmdFindDepart)
        Me.GroupPanel6.Controls.Add(Me.CmdDelBranch)
        Me.GroupPanel6.Controls.Add(Me.TxtBranchGuid)
        Me.GroupPanel6.Controls.Add(Me.TxtBranchName)
        Me.GroupPanel6.Dock = System.Windows.Forms.DockStyle.Right
        Me.GroupPanel6.Location = New System.Drawing.Point(465, 0)
        Me.GroupPanel6.Name = "GroupPanel6"
        Me.GroupPanel6.Size = New System.Drawing.Size(700, 105)
        '
        '
        '
        Me.GroupPanel6.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel6.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel6.Style.BackColorGradientAngle = 90
        Me.GroupPanel6.Style.BorderBottomWidth = 1
        Me.GroupPanel6.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel6.Style.BorderLeftWidth = 1
        Me.GroupPanel6.Style.BorderRightWidth = 1
        Me.GroupPanel6.Style.BorderTopWidth = 1
        Me.GroupPanel6.Style.CornerDiameter = 4
        Me.GroupPanel6.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel6.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel6.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel6.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel6.TabIndex = 1032
        '
        'CboType
        '
        Me.CboType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.CboType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.CboType.BackColor = System.Drawing.Color.WhiteSmoke
        Me.CboType.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.CboType.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.CboType.ForeColor = System.Drawing.Color.Black
        Me.CboType.FormattingEnabled = True
        Me.CboType.Items.AddRange(New Object() {"دبلوم", "دورة", "الكل"})
        Me.CboType.Location = New System.Drawing.Point(482, 2)
        Me.CboType.Name = "CboType"
        Me.CboType.Size = New System.Drawing.Size(123, 31)
        Me.CboType.TabIndex = 1057
        Me.CboType.Text = "الكل"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(611, 8)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(79, 19)
        Me.Label7.TabIndex = 1056
        Me.Label7.Text = "نوع التسجيل"
        '
        'CmdDelSalesMan
        '
        Me.CmdDelSalesMan.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdDelSalesMan.ImageOptions.SvgImage = CType(resources.GetObject("CmdDelSalesMan.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdDelSalesMan.Location = New System.Drawing.Point(123, 39)
        Me.CmdDelSalesMan.Name = "CmdDelSalesMan"
        Me.CmdDelSalesMan.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdDelSalesMan.Size = New System.Drawing.Size(32, 25)
        Me.CmdDelSalesMan.TabIndex = 1046
        '
        'TxtSalesManGuid
        '
        Me.TxtSalesManGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtSalesManGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSalesManGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtSalesManGuid.Location = New System.Drawing.Point(302, 43)
        Me.TxtSalesManGuid.Name = "TxtSalesManGuid"
        Me.TxtSalesManGuid.Size = New System.Drawing.Size(40, 19)
        Me.TxtSalesManGuid.TabIndex = 1045
        Me.TxtSalesManGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtSalesManGuid.Visible = False
        '
        'CmdFindSalesMan
        '
        Me.CmdFindSalesMan.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindSalesMan.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindSalesMan.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindSalesMan.Location = New System.Drawing.Point(161, 39)
        Me.CmdFindSalesMan.Name = "CmdFindSalesMan"
        Me.CmdFindSalesMan.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindSalesMan.Size = New System.Drawing.Size(32, 25)
        Me.CmdFindSalesMan.TabIndex = 1044
        '
        'TxtSalesManName
        '
        Me.TxtSalesManName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtSalesManName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtSalesManName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtSalesManName.Location = New System.Drawing.Point(199, 39)
        Me.TxtSalesManName.Name = "TxtSalesManName"
        Me.TxtSalesManName.ReadOnly = True
        Me.TxtSalesManName.Size = New System.Drawing.Size(211, 24)
        Me.TxtSalesManName.TabIndex = 1043
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(416, 42)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(55, 19)
        Me.Label5.TabIndex = 1042
        Me.Label5.Text = "المندوب"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.White
        Me.Label2.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(632, 45)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(58, 19)
        Me.Label2.TabIndex = 1034
        Me.Label2.Text = "الفترة من"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.White
        Me.Label1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(634, 78)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(56, 19)
        Me.Label1.TabIndex = 1035
        Me.Label1.Text = "الفترة الي"
        '
        'TxtFromDate
        '
        Me.TxtFromDate.CustomFormat = ""
        Me.TxtFromDate.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtFromDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.TxtFromDate.Location = New System.Drawing.Point(482, 39)
        Me.TxtFromDate.Name = "TxtFromDate"
        Me.TxtFromDate.Size = New System.Drawing.Size(123, 27)
        Me.TxtFromDate.TabIndex = 1032
        '
        'TxtToDate
        '
        Me.TxtToDate.CustomFormat = ""
        Me.TxtToDate.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtToDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.TxtToDate.Location = New System.Drawing.Point(482, 72)
        Me.TxtToDate.Name = "TxtToDate"
        Me.TxtToDate.Size = New System.Drawing.Size(123, 27)
        Me.TxtToDate.TabIndex = 1033
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(433, 11)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(37, 19)
        Me.Label4.TabIndex = 1022
        Me.Label4.Text = "الفرع"
        '
        'CmdFindDepart
        '
        Me.CmdFindDepart.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindDepart.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindDepart.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindDepart.Location = New System.Drawing.Point(47, 7)
        Me.CmdFindDepart.Name = "CmdFindDepart"
        Me.CmdFindDepart.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindDepart.Size = New System.Drawing.Size(32, 25)
        Me.CmdFindDepart.TabIndex = 1024
        '
        'CmdDelBranch
        '
        Me.CmdDelBranch.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdDelBranch.ImageOptions.SvgImage = CType(resources.GetObject("CmdDelBranch.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdDelBranch.Location = New System.Drawing.Point(9, 7)
        Me.CmdDelBranch.Name = "CmdDelBranch"
        Me.CmdDelBranch.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdDelBranch.Size = New System.Drawing.Size(32, 25)
        Me.CmdDelBranch.TabIndex = 1030
        '
        'TxtBranchGuid
        '
        Me.TxtBranchGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBranchGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtBranchGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtBranchGuid.Location = New System.Drawing.Point(312, 7)
        Me.TxtBranchGuid.Name = "TxtBranchGuid"
        Me.TxtBranchGuid.Size = New System.Drawing.Size(30, 25)
        Me.TxtBranchGuid.TabIndex = 1025
        Me.TxtBranchGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBranchGuid.Visible = False
        '
        'TxtBranchName
        '
        Me.TxtBranchName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBranchName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBranchName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBranchName.ForeColor = System.Drawing.Color.Black
        Me.TxtBranchName.Location = New System.Drawing.Point(85, 9)
        Me.TxtBranchName.Name = "TxtBranchName"
        Me.TxtBranchName.ReadOnly = True
        Me.TxtBranchName.Size = New System.Drawing.Size(325, 24)
        Me.TxtBranchName.TabIndex = 1023
        '
        'SalesManReport
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1171, 511)
        Me.Controls.Add(Me.GroupPanel7)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SalesManReport"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.Text = "تقرير المسوقين"
        Me.GroupPanel7.ResumeLayout(False)
        Me.GroupPanel3.ResumeLayout(False)
        Me.GroupPanel5.ResumeLayout(False)
        Me.GroupPanel5.PerformLayout()
        CType(Me.BranchGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupPanel4.ResumeLayout(False)
        Me.GroupPanel4.PerformLayout()
        Me.GroupPanel1.ResumeLayout(False)
        Me.GroupPanel2.ResumeLayout(False)
        Me.GroupPanel6.ResumeLayout(False)
        Me.GroupPanel6.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel7 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CmdView As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents GroupPanel6 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CboType As ComboBox
    Friend WithEvents Label7 As Label
    Friend WithEvents CmdDelSalesMan As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtSalesManGuid As Label
    Friend WithEvents CmdFindSalesMan As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtSalesManName As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents TxtFromDate As DateTimePicker
    Friend WithEvents TxtToDate As DateTimePicker
    Friend WithEvents Label4 As Label
    Friend WithEvents CmdFindDepart As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdDelBranch As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtBranchGuid As Label
    Friend WithEvents TxtBranchName As TextBox
    Friend WithEvents GroupPanel3 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel4 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtCount As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents GroupPanel5 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtMsg As Label
    Friend WithEvents BranchGrid As DevComponents.DotNetBar.Controls.DataGridViewX
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn22 As DataGridViewTextBoxColumn
    Friend WithEvents Column5 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn21 As DataGridViewTextBoxColumn
    Friend WithEvents Column14 As DataGridViewTextBoxColumn
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents Column11 As DataGridViewTextBoxColumn
    Friend WithEvents Column13 As DataGridViewTextBoxColumn
    Friend WithEvents CmdExport As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
End Class
