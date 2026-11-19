<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class StudentCourseList
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(StudentCourseList))
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel5 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.StudentGrid = New DevComponents.DotNetBar.Controls.DataGridViewX()
        Me.DataGridViewTextBoxColumn22 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn21 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column9 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column6 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column10 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column7 = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.Column8 = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column11 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column12 = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.Column13 = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.Column14 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column15 = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.Column16 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column17 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column18 = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.GroupPanel4 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.CmdTransAll = New DevExpress.XtraEditors.SimpleButton()
        Me.ChkAll = New System.Windows.Forms.CheckBox()
        Me.TxtFind2 = New System.Windows.Forms.TextBox()
        Me.TxtCount = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel3 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.CmdView = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdPrint = New DevExpress.XtraEditors.SimpleButton()
        Me.GroupPanel6 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.CmdDelCourse = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtToDate = New System.Windows.Forms.MaskedTextBox()
        Me.TxtFromDate = New System.Windows.Forms.MaskedTextBox()
        Me.CmdDelCause = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtCaseGuid = New System.Windows.Forms.Label()
        Me.CmdFindCase = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtCaseName = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TxtCourseGuid = New System.Windows.Forms.Label()
        Me.CmdFindDiplom = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtCourseName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CmdFindDepart = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtBranchGuid = New System.Windows.Forms.Label()
        Me.TxtBranchName = New System.Windows.Forms.TextBox()
        Me.GroupPanel2.SuspendLayout()
        Me.GroupPanel5.SuspendLayout()
        CType(Me.StudentGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupPanel4.SuspendLayout()
        Me.GroupPanel1.SuspendLayout()
        Me.GroupPanel3.SuspendLayout()
        Me.GroupPanel6.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel2
        '
        Me.GroupPanel2.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel2.Controls.Add(Me.GroupPanel5)
        Me.GroupPanel2.Controls.Add(Me.GroupPanel4)
        Me.GroupPanel2.Controls.Add(Me.GroupPanel1)
        Me.GroupPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel2.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel2.Name = "GroupPanel2"
        Me.GroupPanel2.Size = New System.Drawing.Size(1370, 528)
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
        Me.GroupPanel2.TabIndex = 1034
        '
        'GroupPanel5
        '
        Me.GroupPanel5.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel5.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel5.Controls.Add(Me.StudentGrid)
        Me.GroupPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel5.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel5.Location = New System.Drawing.Point(0, 127)
        Me.GroupPanel5.Name = "GroupPanel5"
        Me.GroupPanel5.Size = New System.Drawing.Size(1370, 401)
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
        Me.GroupPanel5.TabIndex = 617
        '
        'StudentGrid
        '
        Me.StudentGrid.AllowUserToAddRows = False
        Me.StudentGrid.AllowUserToDeleteRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.FloralWhite
        Me.StudentGrid.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.StudentGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.StudentGrid.BackgroundColor = System.Drawing.Color.White
        Me.StudentGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.StudentGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.StudentGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn22, Me.Column5, Me.DataGridViewTextBoxColumn21, Me.Column1, Me.Column9, Me.Column6, Me.Column10, Me.Column4, Me.Column7, Me.Column8, Me.Column2, Me.Column3, Me.Column11, Me.Column12, Me.Column13, Me.Column14, Me.Column15, Me.Column16, Me.Column17, Me.Column18})
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        DataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.StudentGrid.DefaultCellStyle = DataGridViewCellStyle10
        Me.StudentGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StudentGrid.GridColor = System.Drawing.Color.FromArgb(CType(CType(208, Byte), Integer), CType(CType(215, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.StudentGrid.Location = New System.Drawing.Point(0, 0)
        Me.StudentGrid.Name = "StudentGrid"
        Me.StudentGrid.ReadOnly = True
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle11.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        DataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.StudentGrid.RowHeadersDefaultCellStyle = DataGridViewCellStyle11
        Me.StudentGrid.RowHeadersVisible = False
        Me.StudentGrid.RowTemplate.Height = 33
        Me.StudentGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.StudentGrid.Size = New System.Drawing.Size(1364, 395)
        Me.StudentGrid.TabIndex = 703
        '
        'DataGridViewTextBoxColumn22
        '
        Me.DataGridViewTextBoxColumn22.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.Format = "d"
        DataGridViewCellStyle2.NullValue = Nothing
        Me.DataGridViewTextBoxColumn22.DefaultCellStyle = DataGridViewCellStyle2
        Me.DataGridViewTextBoxColumn22.HeaderText = "كود اكاديمي"
        Me.DataGridViewTextBoxColumn22.Name = "DataGridViewTextBoxColumn22"
        Me.DataGridViewTextBoxColumn22.ReadOnly = True
        Me.DataGridViewTextBoxColumn22.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.DataGridViewTextBoxColumn22.Visible = False
        '
        'Column5
        '
        Me.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.Column5.DefaultCellStyle = DataGridViewCellStyle3
        Me.Column5.HeaderText = "اسم الطالب"
        Me.Column5.Name = "Column5"
        Me.Column5.ReadOnly = True
        Me.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column5.Width = 81
        '
        'DataGridViewTextBoxColumn21
        '
        Me.DataGridViewTextBoxColumn21.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.Format = "t"
        DataGridViewCellStyle4.NullValue = Nothing
        Me.DataGridViewTextBoxColumn21.DefaultCellStyle = DataGridViewCellStyle4
        Me.DataGridViewTextBoxColumn21.HeaderText = "رقم الهوية"
        Me.DataGridViewTextBoxColumn21.Name = "DataGridViewTextBoxColumn21"
        Me.DataGridViewTextBoxColumn21.ReadOnly = True
        Me.DataGridViewTextBoxColumn21.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.DataGridViewTextBoxColumn21.Width = 71
        '
        'Column1
        '
        Me.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column1.DefaultCellStyle = DataGridViewCellStyle5
        Me.Column1.HeaderText = "رقم الجوال"
        Me.Column1.Name = "Column1"
        Me.Column1.ReadOnly = True
        Me.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column1.Width = 76
        '
        'Column9
        '
        Me.Column9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column9.DefaultCellStyle = DataGridViewCellStyle6
        Me.Column9.HeaderText = "الحالة"
        Me.Column9.Name = "Column9"
        Me.Column9.ReadOnly = True
        Me.Column9.Width = 68
        '
        'Column6
        '
        Me.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.Column6.DefaultCellStyle = DataGridViewCellStyle7
        Me.Column6.HeaderText = "ملاحظات"
        Me.Column6.Name = "Column6"
        Me.Column6.ReadOnly = True
        Me.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column6.Width = 68
        '
        'Column10
        '
        Me.Column10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.Column10.DefaultCellStyle = DataGridViewCellStyle8
        Me.Column10.HeaderText = "الدورة"
        Me.Column10.Name = "Column10"
        Me.Column10.ReadOnly = True
        Me.Column10.Width = 66
        '
        'Column4
        '
        Me.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column4.HeaderText = "تبدأ في"
        Me.Column4.Name = "Column4"
        Me.Column4.ReadOnly = True
        Me.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column4.Width = 49
        '
        'Column7
        '
        Me.Column7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column7.HeaderText = ""
        Me.Column7.Name = "Column7"
        Me.Column7.ReadOnly = True
        Me.Column7.Text = "سداد الرسوم"
        Me.Column7.UseColumnTextForButtonValue = True
        Me.Column7.Visible = False
        '
        'Column8
        '
        Me.Column8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column8.HeaderText = ""
        Me.Column8.Name = "Column8"
        Me.Column8.ReadOnly = True
        Me.Column8.Text = "كشف حساب"
        Me.Column8.UseColumnTextForButtonValue = True
        Me.Column8.Width = 5
        '
        'Column2
        '
        Me.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column2.HeaderText = "regdocguid"
        Me.Column2.Name = "Column2"
        Me.Column2.ReadOnly = True
        Me.Column2.Visible = False
        '
        'Column3
        '
        Me.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column3.HeaderText = "accountguid"
        Me.Column3.Name = "Column3"
        Me.Column3.ReadOnly = True
        Me.Column3.Visible = False
        '
        'Column11
        '
        Me.Column11.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column11.HeaderText = "diplomguid"
        Me.Column11.Name = "Column11"
        Me.Column11.ReadOnly = True
        Me.Column11.Visible = False
        '
        'Column12
        '
        Me.Column12.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle9.BackColor = System.Drawing.Color.Transparent
        Me.Column12.DefaultCellStyle = DataGridViewCellStyle9
        Me.Column12.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.Column12.HeaderText = ""
        Me.Column12.Name = "Column12"
        Me.Column12.ReadOnly = True
        Me.Column12.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column12.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.Column12.Text = "أضافة ملاحظة"
        Me.Column12.UseColumnTextForButtonValue = True
        Me.Column12.Width = 19
        '
        'Column13
        '
        Me.Column13.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column13.HeaderText = ""
        Me.Column13.Name = "Column13"
        Me.Column13.ReadOnly = True
        Me.Column13.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column13.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.Column13.Text = "نقل الطالب"
        Me.Column13.UseColumnTextForButtonValue = True
        Me.Column13.Width = 19
        '
        'Column14
        '
        Me.Column14.HeaderText = "courseguid"
        Me.Column14.Name = "Column14"
        Me.Column14.ReadOnly = True
        Me.Column14.Visible = False
        '
        'Column15
        '
        Me.Column15.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column15.HeaderText = ""
        Me.Column15.Name = "Column15"
        Me.Column15.ReadOnly = True
        Me.Column15.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column15.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.Column15.Text = "تغيير الحالة"
        Me.Column15.UseColumnTextForButtonValue = True
        Me.Column15.Width = 19
        '
        'Column16
        '
        Me.Column16.HeaderText = "branchname"
        Me.Column16.Name = "Column16"
        Me.Column16.ReadOnly = True
        Me.Column16.Visible = False
        '
        'Column17
        '
        Me.Column17.HeaderText = "branchguid"
        Me.Column17.Name = "Column17"
        Me.Column17.ReadOnly = True
        Me.Column17.Visible = False
        '
        'Column18
        '
        Me.Column18.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column18.HeaderText = "تحديد"
        Me.Column18.Name = "Column18"
        Me.Column18.ReadOnly = True
        Me.Column18.Width = 50
        '
        'GroupPanel4
        '
        Me.GroupPanel4.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel4.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel4.Controls.Add(Me.CmdTransAll)
        Me.GroupPanel4.Controls.Add(Me.ChkAll)
        Me.GroupPanel4.Controls.Add(Me.TxtFind2)
        Me.GroupPanel4.Controls.Add(Me.TxtCount)
        Me.GroupPanel4.Controls.Add(Me.Label2)
        Me.GroupPanel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel4.Location = New System.Drawing.Point(0, 95)
        Me.GroupPanel4.Name = "GroupPanel4"
        Me.GroupPanel4.Size = New System.Drawing.Size(1370, 32)
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
        Me.GroupPanel4.TabIndex = 616
        '
        'CmdTransAll
        '
        Me.CmdTransAll.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdTransAll.Appearance.Options.UseFont = True
        Me.CmdTransAll.Dock = System.Windows.Forms.DockStyle.Right
        Me.CmdTransAll.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdTransAll.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdTransAll.ImageOptions.SvgImage = CType(resources.GetObject("CmdTransAll.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdTransAll.Location = New System.Drawing.Point(743, 0)
        Me.CmdTransAll.Name = "CmdTransAll"
        Me.CmdTransAll.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdTransAll.Size = New System.Drawing.Size(92, 26)
        Me.CmdTransAll.TabIndex = 1063
        Me.CmdTransAll.Text = "نقل الكل"
        '
        'ChkAll
        '
        Me.ChkAll.AutoSize = True
        Me.ChkAll.BackColor = System.Drawing.Color.Transparent
        Me.ChkAll.Dock = System.Windows.Forms.DockStyle.Right
        Me.ChkAll.Location = New System.Drawing.Point(835, 0)
        Me.ChkAll.Name = "ChkAll"
        Me.ChkAll.Size = New System.Drawing.Size(89, 26)
        Me.ChkAll.TabIndex = 1057
        Me.ChkAll.Text = "تحديد الكل"
        Me.ChkAll.UseVisualStyleBackColor = False
        '
        'TxtFind2
        '
        Me.TxtFind2.BackColor = System.Drawing.Color.Bisque
        Me.TxtFind2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtFind2.Dock = System.Windows.Forms.DockStyle.Right
        Me.TxtFind2.Font = New System.Drawing.Font("Calibri", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtFind2.ForeColor = System.Drawing.Color.Black
        Me.TxtFind2.Location = New System.Drawing.Point(924, 0)
        Me.TxtFind2.Name = "TxtFind2"
        Me.TxtFind2.Size = New System.Drawing.Size(341, 26)
        Me.TxtFind2.TabIndex = 871
        Me.TxtFind2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TxtCount
        '
        Me.TxtCount.AutoSize = True
        Me.TxtCount.BackColor = System.Drawing.Color.White
        Me.TxtCount.Dock = System.Windows.Forms.DockStyle.Right
        Me.TxtCount.Font = New System.Drawing.Font("Calibri", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCount.ForeColor = System.Drawing.Color.Red
        Me.TxtCount.Location = New System.Drawing.Point(1265, 0)
        Me.TxtCount.Name = "TxtCount"
        Me.TxtCount.Size = New System.Drawing.Size(23, 26)
        Me.TxtCount.TabIndex = 864
        Me.TxtCount.Text = "0"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.White
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Right
        Me.Label2.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(1288, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(76, 19)
        Me.Label2.TabIndex = 862
        Me.Label2.Text = "عدد الطلاب"
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.GroupPanel3)
        Me.GroupPanel1.Controls.Add(Me.GroupPanel6)
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(1370, 95)
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
        Me.GroupPanel1.TabIndex = 614
        '
        'GroupPanel3
        '
        Me.GroupPanel3.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel3.Controls.Add(Me.CmdView)
        Me.GroupPanel3.Controls.Add(Me.CmdExit)
        Me.GroupPanel3.Controls.Add(Me.CmdPrint)
        Me.GroupPanel3.Dock = System.Windows.Forms.DockStyle.Right
        Me.GroupPanel3.Location = New System.Drawing.Point(203, 0)
        Me.GroupPanel3.Name = "GroupPanel3"
        Me.GroupPanel3.Size = New System.Drawing.Size(234, 89)
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
        Me.GroupPanel3.TabIndex = 1033
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
        Me.CmdView.Location = New System.Drawing.Point(153, 11)
        Me.CmdView.Name = "CmdView"
        Me.CmdView.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdView.Size = New System.Drawing.Size(64, 65)
        Me.CmdView.TabIndex = 315
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
        Me.CmdExit.Location = New System.Drawing.Point(13, 11)
        Me.CmdExit.Name = "CmdExit"
        Me.CmdExit.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdExit.Size = New System.Drawing.Size(64, 65)
        Me.CmdExit.TabIndex = 313
        Me.CmdExit.Text = "خروج"
        '
        'CmdPrint
        '
        Me.CmdPrint.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdPrint.Appearance.ForeColor = System.Drawing.Color.Black
        Me.CmdPrint.Appearance.Options.UseFont = True
        Me.CmdPrint.Appearance.Options.UseForeColor = True
        Me.CmdPrint.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter
        Me.CmdPrint.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdPrint.ImageOptions.SvgImage = CType(resources.GetObject("CmdPrint.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdPrint.Location = New System.Drawing.Point(83, 11)
        Me.CmdPrint.Name = "CmdPrint"
        Me.CmdPrint.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdPrint.Size = New System.Drawing.Size(64, 65)
        Me.CmdPrint.TabIndex = 311
        Me.CmdPrint.Text = "طباعه"
        '
        'GroupPanel6
        '
        Me.GroupPanel6.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel6.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel6.Controls.Add(Me.CmdDelCourse)
        Me.GroupPanel6.Controls.Add(Me.TxtToDate)
        Me.GroupPanel6.Controls.Add(Me.TxtFromDate)
        Me.GroupPanel6.Controls.Add(Me.CmdDelCause)
        Me.GroupPanel6.Controls.Add(Me.TxtCaseGuid)
        Me.GroupPanel6.Controls.Add(Me.CmdFindCase)
        Me.GroupPanel6.Controls.Add(Me.TxtCaseName)
        Me.GroupPanel6.Controls.Add(Me.Label6)
        Me.GroupPanel6.Controls.Add(Me.TxtCourseGuid)
        Me.GroupPanel6.Controls.Add(Me.CmdFindDiplom)
        Me.GroupPanel6.Controls.Add(Me.TxtCourseName)
        Me.GroupPanel6.Controls.Add(Me.Label1)
        Me.GroupPanel6.Controls.Add(Me.Label4)
        Me.GroupPanel6.Controls.Add(Me.CmdFindDepart)
        Me.GroupPanel6.Controls.Add(Me.TxtBranchGuid)
        Me.GroupPanel6.Controls.Add(Me.TxtBranchName)
        Me.GroupPanel6.Dock = System.Windows.Forms.DockStyle.Right
        Me.GroupPanel6.Location = New System.Drawing.Point(437, 0)
        Me.GroupPanel6.Name = "GroupPanel6"
        Me.GroupPanel6.Size = New System.Drawing.Size(927, 89)
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
        'CmdDelCourse
        '
        Me.CmdDelCourse.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdDelCourse.ImageOptions.SvgImage = CType(resources.GetObject("CmdDelCourse.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdDelCourse.Location = New System.Drawing.Point(418, 49)
        Me.CmdDelCourse.Name = "CmdDelCourse"
        Me.CmdDelCourse.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdDelCourse.Size = New System.Drawing.Size(36, 26)
        Me.CmdDelCourse.TabIndex = 1070
        '
        'TxtToDate
        '
        Me.TxtToDate.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtToDate.Location = New System.Drawing.Point(103, 52)
        Me.TxtToDate.Mask = "00/00/0000"
        Me.TxtToDate.Name = "TxtToDate"
        Me.TxtToDate.Size = New System.Drawing.Size(126, 27)
        Me.TxtToDate.TabIndex = 1069
        Me.TxtToDate.ValidatingType = GetType(Date)
        '
        'TxtFromDate
        '
        Me.TxtFromDate.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtFromDate.Location = New System.Drawing.Point(235, 52)
        Me.TxtFromDate.Mask = "00/00/0000"
        Me.TxtFromDate.Name = "TxtFromDate"
        Me.TxtFromDate.Size = New System.Drawing.Size(126, 27)
        Me.TxtFromDate.TabIndex = 1068
        Me.TxtFromDate.ValidatingType = GetType(Date)
        '
        'CmdDelCause
        '
        Me.CmdDelCause.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdDelCause.ImageOptions.SvgImage = CType(resources.GetObject("CmdDelCause.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdDelCause.Location = New System.Drawing.Point(13, 19)
        Me.CmdDelCause.Name = "CmdDelCause"
        Me.CmdDelCause.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdDelCause.Size = New System.Drawing.Size(36, 26)
        Me.CmdDelCause.TabIndex = 1067
        '
        'TxtCaseGuid
        '
        Me.TxtCaseGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtCaseGuid.Font = New System.Drawing.Font("justagain DIN", 14.25!)
        Me.TxtCaseGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtCaseGuid.Location = New System.Drawing.Point(129, 20)
        Me.TxtCaseGuid.Name = "TxtCaseGuid"
        Me.TxtCaseGuid.Size = New System.Drawing.Size(54, 25)
        Me.TxtCaseGuid.TabIndex = 1066
        Me.TxtCaseGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtCaseGuid.Visible = False
        '
        'CmdFindCase
        '
        Me.CmdFindCase.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindCase.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindCase.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindCase.Location = New System.Drawing.Point(55, 19)
        Me.CmdFindCase.Name = "CmdFindCase"
        Me.CmdFindCase.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindCase.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindCase.TabIndex = 1065
        '
        'TxtCaseName
        '
        Me.TxtCaseName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtCaseName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCaseName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCaseName.ForeColor = System.Drawing.Color.Black
        Me.TxtCaseName.Location = New System.Drawing.Point(97, 20)
        Me.TxtCaseName.Name = "TxtCaseName"
        Me.TxtCaseName.ReadOnly = True
        Me.TxtCaseName.Size = New System.Drawing.Size(264, 24)
        Me.TxtCaseName.TabIndex = 1064
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(367, 24)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(43, 19)
        Me.Label6.TabIndex = 1063
        Me.Label6.Text = "الحالة"
        '
        'TxtCourseGuid
        '
        Me.TxtCourseGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtCourseGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCourseGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtCourseGuid.Location = New System.Drawing.Point(710, 50)
        Me.TxtCourseGuid.Name = "TxtCourseGuid"
        Me.TxtCourseGuid.Size = New System.Drawing.Size(40, 21)
        Me.TxtCourseGuid.TabIndex = 1062
        Me.TxtCourseGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtCourseGuid.Visible = False
        '
        'CmdFindDiplom
        '
        Me.CmdFindDiplom.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindDiplom.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindDiplom.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindDiplom.Location = New System.Drawing.Point(460, 50)
        Me.CmdFindDiplom.Name = "CmdFindDiplom"
        Me.CmdFindDiplom.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindDiplom.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindDiplom.TabIndex = 1061
        '
        'TxtCourseName
        '
        Me.TxtCourseName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtCourseName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCourseName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtCourseName.ForeColor = System.Drawing.Color.Black
        Me.TxtCourseName.Location = New System.Drawing.Point(502, 49)
        Me.TxtCourseName.Name = "TxtCourseName"
        Me.TxtCourseName.ReadOnly = True
        Me.TxtCourseName.Size = New System.Drawing.Size(361, 24)
        Me.TxtCourseName.TabIndex = 1060
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(869, 52)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 19)
        Me.Label1.TabIndex = 1059
        Me.Label1.Text = "الدورة"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(873, 21)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(37, 19)
        Me.Label4.TabIndex = 1022
        Me.Label4.Text = "الفرع"
        '
        'CmdFindDepart
        '
        Me.CmdFindDepart.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindDepart.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindDepart.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindDepart.Location = New System.Drawing.Point(460, 19)
        Me.CmdFindDepart.Name = "CmdFindDepart"
        Me.CmdFindDepart.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindDepart.Size = New System.Drawing.Size(32, 25)
        Me.CmdFindDepart.TabIndex = 1024
        '
        'TxtBranchGuid
        '
        Me.TxtBranchGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBranchGuid.Font = New System.Drawing.Font("justagain DIN", 14.25!)
        Me.TxtBranchGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtBranchGuid.Location = New System.Drawing.Point(807, 17)
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
        Me.TxtBranchName.Location = New System.Drawing.Point(502, 19)
        Me.TxtBranchName.Name = "TxtBranchName"
        Me.TxtBranchName.ReadOnly = True
        Me.TxtBranchName.Size = New System.Drawing.Size(361, 24)
        Me.TxtBranchName.TabIndex = 1023
        '
        'StudentCourseList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1370, 528)
        Me.Controls.Add(Me.GroupPanel2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "StudentCourseList"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.Text = "طلاب الدورات"
        Me.GroupPanel2.ResumeLayout(False)
        Me.GroupPanel5.ResumeLayout(False)
        CType(Me.StudentGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupPanel4.ResumeLayout(False)
        Me.GroupPanel4.PerformLayout()
        Me.GroupPanel1.ResumeLayout(False)
        Me.GroupPanel3.ResumeLayout(False)
        Me.GroupPanel6.ResumeLayout(False)
        Me.GroupPanel6.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel3 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CmdView As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdPrint As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents GroupPanel6 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtCourseGuid As Label
    Friend WithEvents CmdFindDiplom As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtCourseName As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents CmdFindDepart As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtBranchGuid As Label
    Friend WithEvents TxtBranchName As TextBox
    Friend WithEvents TxtCaseGuid As Label
    Friend WithEvents CmdFindCase As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtCaseName As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents GroupPanel4 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtFind2 As TextBox
    Friend WithEvents TxtCount As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents GroupPanel5 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents StudentGrid As DevComponents.DotNetBar.Controls.DataGridViewX
    Friend WithEvents CmdDelCause As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtToDate As MaskedTextBox
    Friend WithEvents TxtFromDate As MaskedTextBox
    Friend WithEvents CmdDelCourse As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents DataGridViewTextBoxColumn22 As DataGridViewTextBoxColumn
    Friend WithEvents Column5 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn21 As DataGridViewTextBoxColumn
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents Column9 As DataGridViewTextBoxColumn
    Friend WithEvents Column6 As DataGridViewTextBoxColumn
    Friend WithEvents Column10 As DataGridViewTextBoxColumn
    Friend WithEvents Column4 As DataGridViewTextBoxColumn
    Friend WithEvents Column7 As DataGridViewButtonColumn
    Friend WithEvents Column8 As DataGridViewButtonColumn
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents Column3 As DataGridViewTextBoxColumn
    Friend WithEvents Column11 As DataGridViewTextBoxColumn
    Friend WithEvents Column12 As DataGridViewButtonColumn
    Friend WithEvents Column13 As DataGridViewButtonColumn
    Friend WithEvents Column14 As DataGridViewTextBoxColumn
    Friend WithEvents Column15 As DataGridViewButtonColumn
    Friend WithEvents Column16 As DataGridViewTextBoxColumn
    Friend WithEvents Column17 As DataGridViewTextBoxColumn
    Friend WithEvents Column18 As DataGridViewCheckBoxColumn
    Friend WithEvents ChkAll As CheckBox
    Friend WithEvents CmdTransAll As DevExpress.XtraEditors.SimpleButton
End Class
