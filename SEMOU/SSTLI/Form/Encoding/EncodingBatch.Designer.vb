<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EncodingBatch
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EncodingBatch))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel3 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.CmdFind = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtNotes = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtBatchName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ChkStaut = New Bunifu.Framework.UI.BunifuSwitch()
        Me.TxtCode = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtTypeGuid = New System.Windows.Forms.Label()
        Me.TxtGuid = New System.Windows.Forms.Label()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdSave = New DevExpress.XtraEditors.SimpleButton()
        Me.CMDNew = New DevExpress.XtraEditors.SimpleButton()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.MainBranchGrid = New DevComponents.DotNetBar.Controls.DataGridViewX()
        Me.DataGridViewTextBoxColumn21 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TxtTotal = New System.Windows.Forms.Label()
        Me.GroupPanel1.SuspendLayout()
        Me.GroupPanel3.SuspendLayout()
        Me.GroupPanel2.SuspendLayout()
        CType(Me.MainBranchGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.GroupPanel3)
        Me.GroupPanel1.Controls.Add(Me.GroupPanel2)
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(683, 755)
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
        Me.GroupPanel1.TabIndex = 860
        '
        'GroupPanel3
        '
        Me.GroupPanel3.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel3.Controls.Add(Me.TxtTotal)
        Me.GroupPanel3.Controls.Add(Me.MainBranchGrid)
        Me.GroupPanel3.Controls.Add(Me.Label2)
        Me.GroupPanel3.Controls.Add(Me.CmdFind)
        Me.GroupPanel3.Controls.Add(Me.TxtNotes)
        Me.GroupPanel3.Controls.Add(Me.Label4)
        Me.GroupPanel3.Controls.Add(Me.TxtBatchName)
        Me.GroupPanel3.Controls.Add(Me.Label1)
        Me.GroupPanel3.Controls.Add(Me.ChkStaut)
        Me.GroupPanel3.Controls.Add(Me.TxtCode)
        Me.GroupPanel3.Controls.Add(Me.Label5)
        Me.GroupPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel3.Location = New System.Drawing.Point(0, 89)
        Me.GroupPanel3.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupPanel3.Name = "GroupPanel3"
        Me.GroupPanel3.Size = New System.Drawing.Size(683, 666)
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
        Me.GroupPanel3.TabIndex = 862
        '
        'CmdFind
        '
        Me.CmdFind.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFind.ImageOptions.SvgImage = CType(resources.GetObject("CmdFind.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFind.Location = New System.Drawing.Point(69, 70)
        Me.CmdFind.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.CmdFind.Name = "CmdFind"
        Me.CmdFind.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFind.Size = New System.Drawing.Size(48, 32)
        Me.CmdFind.TabIndex = 958
        '
        'TxtNotes
        '
        Me.TxtNotes.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtNotes.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtNotes.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtNotes.ForeColor = System.Drawing.Color.Black
        Me.TxtNotes.Location = New System.Drawing.Point(125, 110)
        Me.TxtNotes.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TxtNotes.Multiline = True
        Me.TxtNotes.Name = "TxtNotes"
        Me.TxtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TxtNotes.Size = New System.Drawing.Size(428, 76)
        Me.TxtNotes.TabIndex = 954
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(578, 134)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(76, 24)
        Me.Label4.TabIndex = 953
        Me.Label4.Text = "ملاحظات"
        '
        'TxtBatchName
        '
        Me.TxtBatchName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBatchName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBatchName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBatchName.ForeColor = System.Drawing.Color.Black
        Me.TxtBatchName.Location = New System.Drawing.Point(125, 70)
        Me.TxtBatchName.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TxtBatchName.Name = "TxtBatchName"
        Me.TxtBatchName.Size = New System.Drawing.Size(428, 29)
        Me.TxtBatchName.TabIndex = 948
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(567, 78)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(87, 24)
        Me.Label1.TabIndex = 947
        Me.Label1.Text = "اسم الدفعة"
        '
        'ChkStaut
        '
        Me.ChkStaut.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ChkStaut.BorderRadius = 0
        Me.ChkStaut.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChkStaut.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ChkStaut.ForeColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ChkStaut.Location = New System.Drawing.Point(237, 31)
        Me.ChkStaut.Margin = New System.Windows.Forms.Padding(4, 6, 4, 6)
        Me.ChkStaut.Name = "ChkStaut"
        Me.ChkStaut.Oncolor = System.Drawing.Color.Chartreuse
        Me.ChkStaut.Onoffcolor = System.Drawing.Color.Red
        Me.ChkStaut.Size = New System.Drawing.Size(0, 0)
        Me.ChkStaut.TabIndex = 946
        Me.ChkStaut.Textcolor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ChkStaut.Value = True
        '
        'TxtCode
        '
        Me.TxtCode.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtCode.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCode.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCode.ForeColor = System.Drawing.Color.Black
        Me.TxtCode.Location = New System.Drawing.Point(437, 31)
        Me.TxtCode.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TxtCode.Name = "TxtCode"
        Me.TxtCode.ReadOnly = True
        Me.TxtCode.Size = New System.Drawing.Size(116, 29)
        Me.TxtCode.TabIndex = 945
        Me.TxtCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(616, 37)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(38, 24)
        Me.Label5.TabIndex = 944
        Me.Label5.Text = "كود"
        '
        'GroupPanel2
        '
        Me.GroupPanel2.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel2.Controls.Add(Me.TxtTypeGuid)
        Me.GroupPanel2.Controls.Add(Me.TxtGuid)
        Me.GroupPanel2.Controls.Add(Me.CmdExit)
        Me.GroupPanel2.Controls.Add(Me.CmdSave)
        Me.GroupPanel2.Controls.Add(Me.CMDNew)
        Me.GroupPanel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel2.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel2.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupPanel2.Name = "GroupPanel2"
        Me.GroupPanel2.Size = New System.Drawing.Size(683, 89)
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
        Me.GroupPanel2.TabIndex = 861
        '
        'TxtTypeGuid
        '
        Me.TxtTypeGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtTypeGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTypeGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtTypeGuid.Location = New System.Drawing.Point(140, 22)
        Me.TxtTypeGuid.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.TxtTypeGuid.Name = "TxtTypeGuid"
        Me.TxtTypeGuid.Size = New System.Drawing.Size(53, 38)
        Me.TxtTypeGuid.TabIndex = 960
        Me.TxtTypeGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtTypeGuid.Visible = False
        '
        'TxtGuid
        '
        Me.TxtGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtGuid.Location = New System.Drawing.Point(79, 22)
        Me.TxtGuid.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.TxtGuid.Name = "TxtGuid"
        Me.TxtGuid.Size = New System.Drawing.Size(53, 38)
        Me.TxtGuid.TabIndex = 959
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
        Me.CmdExit.Location = New System.Drawing.Point(359, 0)
        Me.CmdExit.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.CmdExit.Name = "CmdExit"
        Me.CmdExit.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdExit.Size = New System.Drawing.Size(108, 89)
        Me.CmdExit.TabIndex = 307
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
        Me.CmdSave.Location = New System.Drawing.Point(467, 0)
        Me.CmdSave.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.CmdSave.Name = "CmdSave"
        Me.CmdSave.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdSave.Size = New System.Drawing.Size(108, 89)
        Me.CmdSave.TabIndex = 305
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
        Me.CMDNew.Location = New System.Drawing.Point(575, 0)
        Me.CMDNew.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.CMDNew.Name = "CMDNew"
        Me.CMDNew.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CMDNew.Size = New System.Drawing.Size(108, 89)
        Me.CMDNew.TabIndex = 306
        Me.CMDNew.Text = "جديد F1"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.Red
        Me.Label2.Location = New System.Drawing.Point(379, 206)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(279, 24)
        Me.Label2.TabIndex = 959
        Me.Label2.Text = "عدد الطلاب المتاح تسجيلهم في كل فرع"
        '
        'MainBranchGrid
        '
        Me.MainBranchGrid.AllowUserToAddRows = False
        Me.MainBranchGrid.AllowUserToDeleteRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.FloralWhite
        Me.MainBranchGrid.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.MainBranchGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.MainBranchGrid.BackgroundColor = System.Drawing.Color.White
        Me.MainBranchGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.MainBranchGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.MainBranchGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn21, Me.Column1, Me.DataGridViewTextBoxColumn5})
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.MainBranchGrid.DefaultCellStyle = DataGridViewCellStyle4
        Me.MainBranchGrid.GridColor = System.Drawing.Color.FromArgb(CType(CType(208, Byte), Integer), CType(CType(215, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.MainBranchGrid.Location = New System.Drawing.Point(13, 245)
        Me.MainBranchGrid.Margin = New System.Windows.Forms.Padding(4)
        Me.MainBranchGrid.MultiSelect = False
        Me.MainBranchGrid.Name = "MainBranchGrid"
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.MainBranchGrid.RowHeadersDefaultCellStyle = DataGridViewCellStyle5
        Me.MainBranchGrid.RowHeadersVisible = False
        Me.MainBranchGrid.RowHeadersWidth = 51
        Me.MainBranchGrid.RowTemplate.Height = 33
        Me.MainBranchGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.MainBranchGrid.Size = New System.Drawing.Size(649, 408)
        Me.MainBranchGrid.TabIndex = 960
        '
        'DataGridViewTextBoxColumn21
        '
        Me.DataGridViewTextBoxColumn21.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.Format = "t"
        DataGridViewCellStyle2.NullValue = Nothing
        Me.DataGridViewTextBoxColumn21.DefaultCellStyle = DataGridViewCellStyle2
        Me.DataGridViewTextBoxColumn21.HeaderText = "اسم الفرع"
        Me.DataGridViewTextBoxColumn21.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn21.Name = "DataGridViewTextBoxColumn21"
        Me.DataGridViewTextBoxColumn21.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'Column1
        '
        Me.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column1.DefaultCellStyle = DataGridViewCellStyle3
        Me.Column1.HeaderText = "عدد الطلاب"
        Me.Column1.MinimumWidth = 6
        Me.Column1.Name = "Column1"
        Me.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column1.Width = 101
        '
        'DataGridViewTextBoxColumn5
        '
        Me.DataGridViewTextBoxColumn5.HeaderText = "guid"
        Me.DataGridViewTextBoxColumn5.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5"
        Me.DataGridViewTextBoxColumn5.Visible = False
        '
        'TxtTotal
        '
        Me.TxtTotal.AutoSize = True
        Me.TxtTotal.BackColor = System.Drawing.Color.Transparent
        Me.TxtTotal.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtTotal.ForeColor = System.Drawing.Color.Black
        Me.TxtTotal.Location = New System.Drawing.Point(325, 206)
        Me.TxtTotal.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.TxtTotal.Name = "TxtTotal"
        Me.TxtTotal.Size = New System.Drawing.Size(20, 24)
        Me.TxtTotal.TabIndex = 961
        Me.TxtTotal.Text = "0"
        '
        'EncodingBatch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(683, 755)
        Me.Controls.Add(Me.GroupPanel1)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EncodingBatch"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "دفعة دراسية جديدة"
        Me.GroupPanel1.ResumeLayout(False)
        Me.GroupPanel3.ResumeLayout(False)
        Me.GroupPanel3.PerformLayout()
        Me.GroupPanel2.ResumeLayout(False)
        CType(Me.MainBranchGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel3 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CMDNew As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtBatchName As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents ChkStaut As Bunifu.Framework.UI.BunifuSwitch
    Friend WithEvents TxtCode As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents TxtNotes As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents CmdFind As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtGuid As Label
    Friend WithEvents TxtTypeGuid As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents MainBranchGrid As DevComponents.DotNetBar.Controls.DataGridViewX
    Friend WithEvents DataGridViewTextBoxColumn21 As DataGridViewTextBoxColumn
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn5 As DataGridViewTextBoxColumn
    Friend WithEvents TxtTotal As Label
End Class
