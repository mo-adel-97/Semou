<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EncodingBatchCount
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EncodingBatchCount))
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle14 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle15 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle13 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel3 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtGridCount = New System.Windows.Forms.Label()
        Me.CmdFindBranch = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtBranchGuid = New System.Windows.Forms.Label()
        Me.TxtBatchGuid = New System.Windows.Forms.Label()
        Me.DiplomGrid = New DevComponents.DotNetBar.Controls.DataGridViewX()
        Me.DataGridViewTextBoxColumn21 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtNowCount = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtMainCount = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TxtBranchName = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.CmdFind = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtBatchName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtTypeGuid = New System.Windows.Forms.Label()
        Me.TxtGuid = New System.Windows.Forms.Label()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdSave = New DevExpress.XtraEditors.SimpleButton()
        Me.CMDNew = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtBatchCode = New System.Windows.Forms.Label()
        Me.GroupPanel2.SuspendLayout()
        Me.GroupPanel3.SuspendLayout()
        CType(Me.DiplomGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel2
        '
        Me.GroupPanel2.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel2.Controls.Add(Me.GroupPanel3)
        Me.GroupPanel2.Controls.Add(Me.GroupPanel1)
        Me.GroupPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel2.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel2.Name = "GroupPanel2"
        Me.GroupPanel2.Size = New System.Drawing.Size(870, 563)
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
        Me.GroupPanel2.TabIndex = 870
        '
        'GroupPanel3
        '
        Me.GroupPanel3.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel3.Controls.Add(Me.TxtBatchCode)
        Me.GroupPanel3.Controls.Add(Me.TxtGridCount)
        Me.GroupPanel3.Controls.Add(Me.CmdFindBranch)
        Me.GroupPanel3.Controls.Add(Me.TxtBranchGuid)
        Me.GroupPanel3.Controls.Add(Me.TxtBatchGuid)
        Me.GroupPanel3.Controls.Add(Me.DiplomGrid)
        Me.GroupPanel3.Controls.Add(Me.Label4)
        Me.GroupPanel3.Controls.Add(Me.TxtNowCount)
        Me.GroupPanel3.Controls.Add(Me.Label3)
        Me.GroupPanel3.Controls.Add(Me.TxtMainCount)
        Me.GroupPanel3.Controls.Add(Me.Label8)
        Me.GroupPanel3.Controls.Add(Me.TxtBranchName)
        Me.GroupPanel3.Controls.Add(Me.Label2)
        Me.GroupPanel3.Controls.Add(Me.CmdFind)
        Me.GroupPanel3.Controls.Add(Me.TxtBatchName)
        Me.GroupPanel3.Controls.Add(Me.Label1)
        Me.GroupPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel3.Location = New System.Drawing.Point(0, 72)
        Me.GroupPanel3.Name = "GroupPanel3"
        Me.GroupPanel3.Size = New System.Drawing.Size(870, 491)
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
        Me.GroupPanel3.TabIndex = 871
        '
        'TxtGridCount
        '
        Me.TxtGridCount.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtGridCount.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtGridCount.ForeColor = System.Drawing.Color.Black
        Me.TxtGridCount.Location = New System.Drawing.Point(98, 147)
        Me.TxtGridCount.Name = "TxtGridCount"
        Me.TxtGridCount.Size = New System.Drawing.Size(54, 26)
        Me.TxtGridCount.TabIndex = 1039
        Me.TxtGridCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtGridCount.Visible = False
        '
        'CmdFindBranch
        '
        Me.CmdFindBranch.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindBranch.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindBranch.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindBranch.Location = New System.Drawing.Point(268, 22)
        Me.CmdFindBranch.Name = "CmdFindBranch"
        Me.CmdFindBranch.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindBranch.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindBranch.TabIndex = 1038
        '
        'TxtBranchGuid
        '
        Me.TxtBranchGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBranchGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtBranchGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtBranchGuid.Location = New System.Drawing.Point(208, 24)
        Me.TxtBranchGuid.Name = "TxtBranchGuid"
        Me.TxtBranchGuid.Size = New System.Drawing.Size(54, 19)
        Me.TxtBranchGuid.TabIndex = 1037
        Me.TxtBranchGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBranchGuid.Visible = False
        '
        'TxtBatchGuid
        '
        Me.TxtBatchGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBatchGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtBatchGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtBatchGuid.Location = New System.Drawing.Point(355, 57)
        Me.TxtBatchGuid.Name = "TxtBatchGuid"
        Me.TxtBatchGuid.Size = New System.Drawing.Size(54, 19)
        Me.TxtBatchGuid.TabIndex = 1036
        Me.TxtBatchGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBatchGuid.Visible = False
        '
        'DiplomGrid
        '
        Me.DiplomGrid.AllowUserToAddRows = False
        Me.DiplomGrid.AllowUserToDeleteRows = False
        DataGridViewCellStyle11.BackColor = System.Drawing.Color.FloralWhite
        Me.DiplomGrid.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle11
        Me.DiplomGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DiplomGrid.BackgroundColor = System.Drawing.Color.White
        Me.DiplomGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DiplomGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DiplomGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn21, Me.Column1, Me.DataGridViewTextBoxColumn5})
        DataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle14.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        DataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DiplomGrid.DefaultCellStyle = DataGridViewCellStyle14
        Me.DiplomGrid.GridColor = System.Drawing.Color.FromArgb(CType(CType(208, Byte), Integer), CType(CType(215, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.DiplomGrid.Location = New System.Drawing.Point(355, 169)
        Me.DiplomGrid.MultiSelect = False
        Me.DiplomGrid.Name = "DiplomGrid"
        DataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle15.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        DataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DiplomGrid.RowHeadersDefaultCellStyle = DataGridViewCellStyle15
        Me.DiplomGrid.RowHeadersVisible = False
        Me.DiplomGrid.RowHeadersWidth = 51
        Me.DiplomGrid.RowTemplate.Height = 33
        Me.DiplomGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DiplomGrid.Size = New System.Drawing.Size(487, 294)
        Me.DiplomGrid.TabIndex = 1035
        '
        'DataGridViewTextBoxColumn21
        '
        Me.DataGridViewTextBoxColumn21.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle12.Format = "t"
        DataGridViewCellStyle12.NullValue = Nothing
        Me.DataGridViewTextBoxColumn21.DefaultCellStyle = DataGridViewCellStyle12
        Me.DataGridViewTextBoxColumn21.HeaderText = "التخصص"
        Me.DataGridViewTextBoxColumn21.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn21.Name = "DataGridViewTextBoxColumn21"
        Me.DataGridViewTextBoxColumn21.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'Column1
        '
        Me.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column1.DefaultCellStyle = DataGridViewCellStyle13
        Me.Column1.HeaderText = "عدد الطلاب"
        Me.Column1.MinimumWidth = 6
        Me.Column1.Name = "Column1"
        Me.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column1.Width = 82
        '
        'DataGridViewTextBoxColumn5
        '
        Me.DataGridViewTextBoxColumn5.HeaderText = "guid"
        Me.DataGridViewTextBoxColumn5.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5"
        Me.DataGridViewTextBoxColumn5.Visible = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.ForeColor = System.Drawing.Color.Red
        Me.Label4.Location = New System.Drawing.Point(597, 147)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(249, 19)
        Me.Label4.TabIndex = 1034
        Me.Label4.Text = "عدد الطلاب المتاح تسجيلهم في كل تخصص"
        '
        'TxtNowCount
        '
        Me.TxtNowCount.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtNowCount.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtNowCount.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtNowCount.ForeColor = System.Drawing.Color.Red
        Me.TxtNowCount.Location = New System.Drawing.Point(672, 112)
        Me.TxtNowCount.Name = "TxtNowCount"
        Me.TxtNowCount.ReadOnly = True
        Me.TxtNowCount.Size = New System.Drawing.Size(73, 24)
        Me.TxtNowCount.TabIndex = 1033
        Me.TxtNowCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(748, 115)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(98, 19)
        Me.Label3.TabIndex = 1032
        Me.Label3.Text = "المقاعد المتبقية"
        '
        'TxtMainCount
        '
        Me.TxtMainCount.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtMainCount.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtMainCount.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtMainCount.ForeColor = System.Drawing.Color.Red
        Me.TxtMainCount.Location = New System.Drawing.Point(672, 82)
        Me.TxtMainCount.Name = "TxtMainCount"
        Me.TxtMainCount.ReadOnly = True
        Me.TxtMainCount.Size = New System.Drawing.Size(73, 24)
        Me.TxtMainCount.TabIndex = 1031
        Me.TxtMainCount.Text = "0"
        Me.TxtMainCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(751, 85)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(95, 19)
        Me.Label8.TabIndex = 1030
        Me.Label8.Text = "المقاعد المتاحة"
        '
        'TxtBranchName
        '
        Me.TxtBranchName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBranchName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBranchName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBranchName.ForeColor = System.Drawing.Color.Black
        Me.TxtBranchName.Location = New System.Drawing.Point(310, 22)
        Me.TxtBranchName.Name = "TxtBranchName"
        Me.TxtBranchName.Size = New System.Drawing.Size(479, 24)
        Me.TxtBranchName.TabIndex = 963
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(809, 25)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(37, 19)
        Me.Label2.TabIndex = 962
        Me.Label2.Text = "الفرع"
        '
        'CmdFind
        '
        Me.CmdFind.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFind.ImageOptions.SvgImage = CType(resources.GetObject("CmdFind.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFind.Location = New System.Drawing.Point(426, 52)
        Me.CmdFind.Name = "CmdFind"
        Me.CmdFind.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFind.Size = New System.Drawing.Size(36, 26)
        Me.CmdFind.TabIndex = 961
        '
        'TxtBatchName
        '
        Me.TxtBatchName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBatchName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBatchName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBatchName.ForeColor = System.Drawing.Color.Black
        Me.TxtBatchName.Location = New System.Drawing.Point(468, 52)
        Me.TxtBatchName.Name = "TxtBatchName"
        Me.TxtBatchName.Size = New System.Drawing.Size(321, 24)
        Me.TxtBatchName.TabIndex = 960
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(801, 55)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(45, 19)
        Me.Label1.TabIndex = 959
        Me.Label1.Text = "الدفعه"
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.TxtTypeGuid)
        Me.GroupPanel1.Controls.Add(Me.TxtGuid)
        Me.GroupPanel1.Controls.Add(Me.CmdExit)
        Me.GroupPanel1.Controls.Add(Me.CmdSave)
        Me.GroupPanel1.Controls.Add(Me.CMDNew)
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(870, 72)
        '
        '
        '
        Me.GroupPanel1.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel1.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel1.Style.BackColorGradientAngle = 90
        Me.GroupPanel1.Style.BorderBottomWidth = 1
        Me.GroupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel1.Style.BorderLeftWidth = 1
        Me.GroupPanel1.Style.BorderRightWidth = 1
        Me.GroupPanel1.Style.BorderTopWidth = 1
        Me.GroupPanel1.Style.CornerDiameter = 4
        Me.GroupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel1.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel1.TabIndex = 870
        '
        'TxtTypeGuid
        '
        Me.TxtTypeGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtTypeGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTypeGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtTypeGuid.Location = New System.Drawing.Point(39, 9)
        Me.TxtTypeGuid.Name = "TxtTypeGuid"
        Me.TxtTypeGuid.Size = New System.Drawing.Size(182, 31)
        Me.TxtTypeGuid.TabIndex = 1035
        Me.TxtTypeGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtTypeGuid.Visible = False
        '
        'TxtGuid
        '
        Me.TxtGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtGuid.Location = New System.Drawing.Point(266, 24)
        Me.TxtGuid.Name = "TxtGuid"
        Me.TxtGuid.Size = New System.Drawing.Size(54, 25)
        Me.TxtGuid.TabIndex = 1034
        Me.TxtGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtGuid.Visible = False
        '
        'CmdExit
        '
        Me.CmdExit.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdExit.Appearance.ForeColor = System.Drawing.Color.Black
        Me.CmdExit.Appearance.Options.UseFont = True
        Me.CmdExit.Appearance.Options.UseForeColor = True
        Me.CmdExit.Dock = System.Windows.Forms.DockStyle.Right
        Me.CmdExit.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter
        Me.CmdExit.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdExit.ImageOptions.SvgImage = CType(resources.GetObject("CmdExit.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdExit.Location = New System.Drawing.Point(627, 0)
        Me.CmdExit.Name = "CmdExit"
        Me.CmdExit.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdExit.Size = New System.Drawing.Size(81, 72)
        Me.CmdExit.TabIndex = 319
        Me.CmdExit.Text = "خروج F4"
        '
        'CmdSave
        '
        Me.CmdSave.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdSave.Appearance.ForeColor = System.Drawing.Color.Black
        Me.CmdSave.Appearance.Options.UseFont = True
        Me.CmdSave.Appearance.Options.UseForeColor = True
        Me.CmdSave.Dock = System.Windows.Forms.DockStyle.Right
        Me.CmdSave.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter
        Me.CmdSave.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdSave.ImageOptions.SvgImage = CType(resources.GetObject("CmdSave.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdSave.Location = New System.Drawing.Point(708, 0)
        Me.CmdSave.Name = "CmdSave"
        Me.CmdSave.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdSave.Size = New System.Drawing.Size(81, 72)
        Me.CmdSave.TabIndex = 317
        Me.CmdSave.Text = "حفظ F2"
        '
        'CMDNew
        '
        Me.CMDNew.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CMDNew.Appearance.ForeColor = System.Drawing.Color.Black
        Me.CMDNew.Appearance.Options.UseFont = True
        Me.CMDNew.Appearance.Options.UseForeColor = True
        Me.CMDNew.Dock = System.Windows.Forms.DockStyle.Right
        Me.CMDNew.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter
        Me.CMDNew.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CMDNew.ImageOptions.SvgImage = CType(resources.GetObject("CMDNew.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CMDNew.Location = New System.Drawing.Point(789, 0)
        Me.CMDNew.Name = "CMDNew"
        Me.CMDNew.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CMDNew.Size = New System.Drawing.Size(81, 72)
        Me.CMDNew.TabIndex = 318
        Me.CMDNew.Text = "جديد F1"
        '
        'TxtBatchCode
        '
        Me.TxtBatchCode.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBatchCode.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtBatchCode.ForeColor = System.Drawing.Color.Black
        Me.TxtBatchCode.Location = New System.Drawing.Point(295, 55)
        Me.TxtBatchCode.Name = "TxtBatchCode"
        Me.TxtBatchCode.Size = New System.Drawing.Size(54, 19)
        Me.TxtBatchCode.TabIndex = 1040
        Me.TxtBatchCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBatchCode.Visible = False
        '
        'EncodingBatchCount
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(870, 563)
        Me.Controls.Add(Me.GroupPanel2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EncodingBatchCount"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ظبط أعداد الدفعات"
        Me.GroupPanel2.ResumeLayout(False)
        Me.GroupPanel3.ResumeLayout(False)
        Me.GroupPanel3.PerformLayout()
        CType(Me.DiplomGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtTypeGuid As Label
    Friend WithEvents TxtGuid As Label
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CMDNew As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents GroupPanel3 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtBranchName As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents CmdFind As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtBatchName As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents TxtNowCount As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents TxtMainCount As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents DiplomGrid As DevComponents.DotNetBar.Controls.DataGridViewX
    Friend WithEvents DataGridViewTextBoxColumn21 As DataGridViewTextBoxColumn
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn5 As DataGridViewTextBoxColumn
    Friend WithEvents Label4 As Label
    Friend WithEvents TxtBatchGuid As Label
    Friend WithEvents TxtBranchGuid As Label
    Friend WithEvents CmdFindBranch As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtGridCount As Label
    Friend WithEvents TxtBatchCode As Label
End Class
