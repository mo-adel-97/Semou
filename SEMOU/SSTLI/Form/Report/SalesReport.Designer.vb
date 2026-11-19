<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SalesReport
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SalesReport))
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel3 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.TxtMsg = New System.Windows.Forms.Label()
        Me.TxtTypeGuid = New System.Windows.Forms.Label()
        Me.BranchGrid = New DevComponents.DotNetBar.Controls.DataGridViewX()
        Me.DataGridViewTextBoxColumn22 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column14 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn21 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column15 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column6 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column7 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column11 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column13 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column8 = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.GroupPanel4 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel8 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtFind2 = New System.Windows.Forms.TextBox()
        Me.FindByName = New System.Windows.Forms.RadioButton()
        Me.FindById = New System.Windows.Forms.RadioButton()
        Me.TxtSubTotal = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.TxtTax = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.TxtTotal = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TxtCount = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel7 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.CmdView = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdSave = New DevExpress.XtraEditors.SimpleButton()
        Me.GroupPanel6 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.ChkFessPay = New System.Windows.Forms.CheckBox()
        Me.ChkOflineInvoice = New System.Windows.Forms.RadioButton()
        Me.ChkMoPay = New System.Windows.Forms.CheckBox()
        Me.ChkActiveImvoice = New System.Windows.Forms.RadioButton()
        Me.ChkStartPay = New System.Windows.Forms.CheckBox()
        Me.CmdExport = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtDocGuid = New System.Windows.Forms.Label()
        Me.TxtDocName = New System.Windows.Forms.TextBox()
        Me.TxtUserGuid = New System.Windows.Forms.Label()
        Me.TxtUserName = New System.Windows.Forms.TextBox()
        Me.CmdDelBatch = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdFindBatch = New DevExpress.XtraEditors.SimpleButton()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.CmdDelSalesMan = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdFindSalesMan = New DevExpress.XtraEditors.SimpleButton()
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
        Me.GroupPanel2.SuspendLayout()
        Me.GroupPanel3.SuspendLayout()
        CType(Me.BranchGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupPanel4.SuspendLayout()
        Me.GroupPanel8.SuspendLayout()
        Me.GroupPanel1.SuspendLayout()
        Me.GroupPanel7.SuspendLayout()
        Me.GroupPanel6.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel2
        '
        Me.GroupPanel2.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel2.Controls.Add(Me.GroupPanel3)
        Me.GroupPanel2.Controls.Add(Me.GroupPanel4)
        Me.GroupPanel2.Controls.Add(Me.GroupPanel1)
        Me.GroupPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel2.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel2.Name = "GroupPanel2"
        Me.GroupPanel2.Size = New System.Drawing.Size(1496, 739)
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
        'GroupPanel3
        '
        Me.GroupPanel3.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel3.Controls.Add(Me.ProgressBar1)
        Me.GroupPanel3.Controls.Add(Me.TxtMsg)
        Me.GroupPanel3.Controls.Add(Me.TxtTypeGuid)
        Me.GroupPanel3.Controls.Add(Me.BranchGrid)
        Me.GroupPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel3.Location = New System.Drawing.Point(0, 133)
        Me.GroupPanel3.Name = "GroupPanel3"
        Me.GroupPanel3.Size = New System.Drawing.Size(1496, 606)
        '
        '
        '
        Me.GroupPanel3.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel3.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel3.Style.BackColorGradientAngle = 90
        Me.GroupPanel3.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel3.Style.BorderBottomWidth = 1
        Me.GroupPanel3.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel3.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel3.Style.BorderLeftWidth = 1
        Me.GroupPanel3.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel3.Style.BorderRightWidth = 1
        Me.GroupPanel3.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel3.Style.BorderTopWidth = 1
        Me.GroupPanel3.Style.CornerDiameter = 4
        Me.GroupPanel3.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel3.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel3.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel3.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel3.TabIndex = 616
        '
        'ProgressBar1
        '
        Me.ProgressBar1.BackColor = System.Drawing.Color.White
        Me.ProgressBar1.ForeColor = System.Drawing.Color.Lime
        Me.ProgressBar1.Location = New System.Drawing.Point(623, 183)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(396, 42)
        Me.ProgressBar1.TabIndex = 1062
        Me.ProgressBar1.Visible = False
        '
        'TxtMsg
        '
        Me.TxtMsg.AutoSize = True
        Me.TxtMsg.BackColor = System.Drawing.Color.Transparent
        Me.TxtMsg.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtMsg.ForeColor = System.Drawing.Color.Red
        Me.TxtMsg.Location = New System.Drawing.Point(756, 275)
        Me.TxtMsg.Name = "TxtMsg"
        Me.TxtMsg.Size = New System.Drawing.Size(217, 33)
        Me.TxtMsg.TabIndex = 1061
        Me.TxtMsg.Text = "لا تتوفر بيانات للعرض"
        Me.TxtMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.TxtMsg.Visible = False
        '
        'TxtTypeGuid
        '
        Me.TxtTypeGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtTypeGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtTypeGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtTypeGuid.Location = New System.Drawing.Point(123, 80)
        Me.TxtTypeGuid.Name = "TxtTypeGuid"
        Me.TxtTypeGuid.Size = New System.Drawing.Size(30, 25)
        Me.TxtTypeGuid.TabIndex = 1060
        Me.TxtTypeGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtTypeGuid.Visible = False
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
        Me.BranchGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn22, Me.Column5, Me.Column14, Me.DataGridViewTextBoxColumn21, Me.Column1, Me.Column15, Me.Column6, Me.Column7, Me.Column11, Me.Column13, Me.Column3, Me.Column4, Me.Column2, Me.Column8})
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
        Me.BranchGrid.Size = New System.Drawing.Size(1490, 600)
        Me.BranchGrid.TabIndex = 702
        '
        'DataGridViewTextBoxColumn22
        '
        Me.DataGridViewTextBoxColumn22.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.Format = "d"
        DataGridViewCellStyle2.NullValue = Nothing
        Me.DataGridViewTextBoxColumn22.DefaultCellStyle = DataGridViewCellStyle2
        Me.DataGridViewTextBoxColumn22.HeaderText = "التاريخ"
        Me.DataGridViewTextBoxColumn22.Name = "DataGridViewTextBoxColumn22"
        Me.DataGridViewTextBoxColumn22.ReadOnly = True
        Me.DataGridViewTextBoxColumn22.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.DataGridViewTextBoxColumn22.Width = 54
        '
        'Column5
        '
        Me.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column5.DefaultCellStyle = DataGridViewCellStyle3
        Me.Column5.HeaderText = "رقم الفاتورة"
        Me.Column5.Name = "Column5"
        Me.Column5.ReadOnly = True
        Me.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column5.Width = 79
        '
        'Column14
        '
        Me.Column14.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column14.FillWeight = 30.0!
        Me.Column14.HeaderText = "نوع المستند"
        Me.Column14.Name = "Column14"
        Me.Column14.ReadOnly = True
        Me.Column14.Width = 99
        '
        'DataGridViewTextBoxColumn21
        '
        Me.DataGridViewTextBoxColumn21.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.Format = "t"
        DataGridViewCellStyle4.NullValue = Nothing
        Me.DataGridViewTextBoxColumn21.DefaultCellStyle = DataGridViewCellStyle4
        Me.DataGridViewTextBoxColumn21.FillWeight = 50.0!
        Me.DataGridViewTextBoxColumn21.HeaderText = "اسم الطالب"
        Me.DataGridViewTextBoxColumn21.Name = "DataGridViewTextBoxColumn21"
        Me.DataGridViewTextBoxColumn21.ReadOnly = True
        Me.DataGridViewTextBoxColumn21.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.DataGridViewTextBoxColumn21.Width = 81
        '
        'Column1
        '
        Me.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column1.DefaultCellStyle = DataGridViewCellStyle5
        Me.Column1.FillWeight = 25.0!
        Me.Column1.HeaderText = "رقم الجوال"
        Me.Column1.Name = "Column1"
        Me.Column1.ReadOnly = True
        Me.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column1.Width = 76
        '
        'Column15
        '
        Me.Column15.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column15.FillWeight = 25.0!
        Me.Column15.HeaderText = "رقم الهوية"
        Me.Column15.Name = "Column15"
        Me.Column15.ReadOnly = True
        Me.Column15.Width = 90
        '
        'Column6
        '
        Me.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.Column6.DefaultCellStyle = DataGridViewCellStyle6
        Me.Column6.HeaderText = "ملاحظات"
        Me.Column6.Name = "Column6"
        Me.Column6.ReadOnly = True
        Me.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column6.Width = 68
        '
        'Column7
        '
        Me.Column7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column7.DefaultCellStyle = DataGridViewCellStyle7
        Me.Column7.FillWeight = 20.0!
        Me.Column7.HeaderText = "الإجمالي"
        Me.Column7.Name = "Column7"
        Me.Column7.ReadOnly = True
        Me.Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column7.Width = 58
        '
        'Column11
        '
        Me.Column11.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column11.DefaultCellStyle = DataGridViewCellStyle8
        Me.Column11.FillWeight = 20.0!
        Me.Column11.HeaderText = "الضريبة"
        Me.Column11.Name = "Column11"
        Me.Column11.ReadOnly = True
        Me.Column11.Width = 75
        '
        'Column13
        '
        Me.Column13.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Column13.DefaultCellStyle = DataGridViewCellStyle9
        Me.Column13.FillWeight = 20.0!
        Me.Column13.HeaderText = "الصافي"
        Me.Column13.Name = "Column13"
        Me.Column13.ReadOnly = True
        Me.Column13.Width = 69
        '
        'Column3
        '
        Me.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column3.HeaderText = "الفرع"
        Me.Column3.Name = "Column3"
        Me.Column3.ReadOnly = True
        Me.Column3.Width = 62
        '
        'Column4
        '
        Me.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column4.HeaderText = "المحصل"
        Me.Column4.Name = "Column4"
        Me.Column4.ReadOnly = True
        Me.Column4.Width = 81
        '
        'Column2
        '
        Me.Column2.HeaderText = "guid"
        Me.Column2.Name = "Column2"
        Me.Column2.ReadOnly = True
        Me.Column2.Visible = False
        '
        'Column8
        '
        Me.Column8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Column8.HeaderText = ""
        Me.Column8.Name = "Column8"
        Me.Column8.ReadOnly = True
        Me.Column8.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column8.Text = "عرض"
        Me.Column8.UseColumnTextForButtonValue = True
        Me.Column8.Width = 5
        '
        'GroupPanel4
        '
        Me.GroupPanel4.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel4.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel4.Controls.Add(Me.GroupPanel8)
        Me.GroupPanel4.Controls.Add(Me.TxtSubTotal)
        Me.GroupPanel4.Controls.Add(Me.Label12)
        Me.GroupPanel4.Controls.Add(Me.TxtTax)
        Me.GroupPanel4.Controls.Add(Me.Label10)
        Me.GroupPanel4.Controls.Add(Me.TxtTotal)
        Me.GroupPanel4.Controls.Add(Me.Label8)
        Me.GroupPanel4.Controls.Add(Me.TxtCount)
        Me.GroupPanel4.Controls.Add(Me.Label6)
        Me.GroupPanel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel4.Location = New System.Drawing.Point(0, 101)
        Me.GroupPanel4.Name = "GroupPanel4"
        Me.GroupPanel4.Size = New System.Drawing.Size(1496, 32)
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
        'GroupPanel8
        '
        Me.GroupPanel8.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel8.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel8.Controls.Add(Me.TxtFind2)
        Me.GroupPanel8.Controls.Add(Me.FindByName)
        Me.GroupPanel8.Controls.Add(Me.FindById)
        Me.GroupPanel8.Dock = System.Windows.Forms.DockStyle.Right
        Me.GroupPanel8.Location = New System.Drawing.Point(566, 0)
        Me.GroupPanel8.Name = "GroupPanel8"
        Me.GroupPanel8.Size = New System.Drawing.Size(610, 26)
        '
        '
        '
        Me.GroupPanel8.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel8.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel8.Style.BackColorGradientAngle = 90
        Me.GroupPanel8.Style.BorderBottomWidth = 1
        Me.GroupPanel8.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel8.Style.BorderLeftWidth = 1
        Me.GroupPanel8.Style.BorderRightWidth = 1
        Me.GroupPanel8.Style.BorderTopWidth = 1
        Me.GroupPanel8.Style.CornerDiameter = 4
        Me.GroupPanel8.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel8.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel8.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel8.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel8.TabIndex = 1062
        '
        'TxtFind2
        '
        Me.TxtFind2.BackColor = System.Drawing.Color.Bisque
        Me.TxtFind2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtFind2.Dock = System.Windows.Forms.DockStyle.Right
        Me.TxtFind2.Font = New System.Drawing.Font("Calibri", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtFind2.ForeColor = System.Drawing.Color.Black
        Me.TxtFind2.Location = New System.Drawing.Point(85, 0)
        Me.TxtFind2.Name = "TxtFind2"
        Me.TxtFind2.Size = New System.Drawing.Size(341, 26)
        Me.TxtFind2.TabIndex = 871
        Me.TxtFind2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'FindByName
        '
        Me.FindByName.AutoSize = True
        Me.FindByName.BackColor = System.Drawing.Color.Transparent
        Me.FindByName.Dock = System.Windows.Forms.DockStyle.Right
        Me.FindByName.Location = New System.Drawing.Point(426, 0)
        Me.FindByName.Name = "FindByName"
        Me.FindByName.Size = New System.Drawing.Size(93, 26)
        Me.FindByName.TabIndex = 510
        Me.FindByName.Text = "اسم الطالب"
        Me.FindByName.UseVisualStyleBackColor = False
        '
        'FindById
        '
        Me.FindById.AutoSize = True
        Me.FindById.BackColor = System.Drawing.Color.Transparent
        Me.FindById.Checked = True
        Me.FindById.Dock = System.Windows.Forms.DockStyle.Right
        Me.FindById.Location = New System.Drawing.Point(519, 0)
        Me.FindById.Name = "FindById"
        Me.FindById.Size = New System.Drawing.Size(91, 26)
        Me.FindById.TabIndex = 508
        Me.FindById.TabStop = True
        Me.FindById.Text = "رقم الفاتورة"
        Me.FindById.UseVisualStyleBackColor = False
        '
        'TxtSubTotal
        '
        Me.TxtSubTotal.AutoSize = True
        Me.TxtSubTotal.BackColor = System.Drawing.Color.White
        Me.TxtSubTotal.Dock = System.Windows.Forms.DockStyle.Right
        Me.TxtSubTotal.Font = New System.Drawing.Font("Calibri", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSubTotal.ForeColor = System.Drawing.Color.Red
        Me.TxtSubTotal.Location = New System.Drawing.Point(1176, 0)
        Me.TxtSubTotal.Name = "TxtSubTotal"
        Me.TxtSubTotal.Size = New System.Drawing.Size(23, 26)
        Me.TxtSubTotal.TabIndex = 870
        Me.TxtSubTotal.Text = "0"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.BackColor = System.Drawing.Color.White
        Me.Label12.Dock = System.Windows.Forms.DockStyle.Right
        Me.Label12.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label12.ForeColor = System.Drawing.Color.Black
        Me.Label12.Location = New System.Drawing.Point(1199, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(44, 19)
        Me.Label12.TabIndex = 869
        Me.Label12.Text = "الصافي"
        '
        'TxtTax
        '
        Me.TxtTax.AutoSize = True
        Me.TxtTax.BackColor = System.Drawing.Color.White
        Me.TxtTax.Dock = System.Windows.Forms.DockStyle.Right
        Me.TxtTax.Font = New System.Drawing.Font("Calibri", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTax.ForeColor = System.Drawing.Color.Red
        Me.TxtTax.Location = New System.Drawing.Point(1243, 0)
        Me.TxtTax.Name = "TxtTax"
        Me.TxtTax.Size = New System.Drawing.Size(23, 26)
        Me.TxtTax.TabIndex = 868
        Me.TxtTax.Text = "0"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.BackColor = System.Drawing.Color.White
        Me.Label10.Dock = System.Windows.Forms.DockStyle.Right
        Me.Label10.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label10.ForeColor = System.Drawing.Color.Black
        Me.Label10.Location = New System.Drawing.Point(1266, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(50, 19)
        Me.Label10.TabIndex = 867
        Me.Label10.Text = "الضريبة"
        '
        'TxtTotal
        '
        Me.TxtTotal.AutoSize = True
        Me.TxtTotal.BackColor = System.Drawing.Color.White
        Me.TxtTotal.Dock = System.Windows.Forms.DockStyle.Right
        Me.TxtTotal.Font = New System.Drawing.Font("Calibri", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTotal.ForeColor = System.Drawing.Color.Red
        Me.TxtTotal.Location = New System.Drawing.Point(1316, 0)
        Me.TxtTotal.Name = "TxtTotal"
        Me.TxtTotal.Size = New System.Drawing.Size(23, 26)
        Me.TxtTotal.TabIndex = 866
        Me.TxtTotal.Text = "0"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.White
        Me.Label8.Dock = System.Windows.Forms.DockStyle.Right
        Me.Label8.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(1339, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(52, 19)
        Me.Label8.TabIndex = 865
        Me.Label8.Text = "الإجمالي"
        '
        'TxtCount
        '
        Me.TxtCount.AutoSize = True
        Me.TxtCount.BackColor = System.Drawing.Color.White
        Me.TxtCount.Dock = System.Windows.Forms.DockStyle.Right
        Me.TxtCount.Font = New System.Drawing.Font("Calibri", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCount.ForeColor = System.Drawing.Color.Red
        Me.TxtCount.Location = New System.Drawing.Point(1391, 0)
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
        Me.Label6.Location = New System.Drawing.Point(1415, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(75, 19)
        Me.Label6.TabIndex = 862
        Me.Label6.Text = "عدد الفواتير"
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.GroupPanel7)
        Me.GroupPanel1.Controls.Add(Me.GroupPanel6)
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(1496, 101)
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
        'GroupPanel7
        '
        Me.GroupPanel7.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel7.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel7.Controls.Add(Me.CmdView)
        Me.GroupPanel7.Controls.Add(Me.CmdExit)
        Me.GroupPanel7.Controls.Add(Me.CmdSave)
        Me.GroupPanel7.Dock = System.Windows.Forms.DockStyle.Right
        Me.GroupPanel7.Location = New System.Drawing.Point(10, 0)
        Me.GroupPanel7.Name = "GroupPanel7"
        Me.GroupPanel7.Size = New System.Drawing.Size(231, 95)
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
        Me.GroupPanel7.TabIndex = 1033
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
        Me.CmdView.Location = New System.Drawing.Point(159, 9)
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
        Me.CmdExit.Location = New System.Drawing.Point(19, 9)
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
        Me.CmdSave.Location = New System.Drawing.Point(89, 9)
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
        Me.GroupPanel6.Controls.Add(Me.ChkFessPay)
        Me.GroupPanel6.Controls.Add(Me.ChkOflineInvoice)
        Me.GroupPanel6.Controls.Add(Me.ChkMoPay)
        Me.GroupPanel6.Controls.Add(Me.ChkActiveImvoice)
        Me.GroupPanel6.Controls.Add(Me.ChkStartPay)
        Me.GroupPanel6.Controls.Add(Me.CmdExport)
        Me.GroupPanel6.Controls.Add(Me.TxtDocGuid)
        Me.GroupPanel6.Controls.Add(Me.TxtDocName)
        Me.GroupPanel6.Controls.Add(Me.TxtUserGuid)
        Me.GroupPanel6.Controls.Add(Me.TxtUserName)
        Me.GroupPanel6.Controls.Add(Me.CmdDelBatch)
        Me.GroupPanel6.Controls.Add(Me.CmdFindBatch)
        Me.GroupPanel6.Controls.Add(Me.Label3)
        Me.GroupPanel6.Controls.Add(Me.CmdDelSalesMan)
        Me.GroupPanel6.Controls.Add(Me.CmdFindSalesMan)
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
        Me.GroupPanel6.Location = New System.Drawing.Point(241, 0)
        Me.GroupPanel6.Name = "GroupPanel6"
        Me.GroupPanel6.Size = New System.Drawing.Size(1249, 95)
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
        'ChkFessPay
        '
        Me.ChkFessPay.AutoSize = True
        Me.ChkFessPay.BackColor = System.Drawing.Color.Transparent
        Me.ChkFessPay.Checked = True
        Me.ChkFessPay.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChkFessPay.Location = New System.Drawing.Point(124, 55)
        Me.ChkFessPay.Name = "ChkFessPay"
        Me.ChkFessPay.Size = New System.Drawing.Size(58, 23)
        Me.ChkFessPay.TabIndex = 1065
        Me.ChkFessPay.Text = "رسوم"
        Me.ChkFessPay.UseVisualStyleBackColor = False
        '
        'ChkOflineInvoice
        '
        Me.ChkOflineInvoice.AutoSize = True
        Me.ChkOflineInvoice.BackColor = System.Drawing.Color.Transparent
        Me.ChkOflineInvoice.Location = New System.Drawing.Point(419, 69)
        Me.ChkOflineInvoice.Name = "ChkOflineInvoice"
        Me.ChkOflineInvoice.Size = New System.Drawing.Size(165, 23)
        Me.ChkOflineInvoice.TabIndex = 1061
        Me.ChkOflineInvoice.Text = "عرض الفواتير الغير نشطة"
        Me.ChkOflineInvoice.UseVisualStyleBackColor = False
        '
        'ChkMoPay
        '
        Me.ChkMoPay.AutoSize = True
        Me.ChkMoPay.BackColor = System.Drawing.Color.Transparent
        Me.ChkMoPay.Checked = True
        Me.ChkMoPay.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChkMoPay.Location = New System.Drawing.Point(188, 56)
        Me.ChkMoPay.Name = "ChkMoPay"
        Me.ChkMoPay.Size = New System.Drawing.Size(97, 23)
        Me.ChkMoPay.TabIndex = 1064
        Me.ChkMoPay.Text = "قسط شهري"
        Me.ChkMoPay.UseVisualStyleBackColor = False
        '
        'ChkActiveImvoice
        '
        Me.ChkActiveImvoice.AutoSize = True
        Me.ChkActiveImvoice.BackColor = System.Drawing.Color.Transparent
        Me.ChkActiveImvoice.Checked = True
        Me.ChkActiveImvoice.Location = New System.Drawing.Point(439, 40)
        Me.ChkActiveImvoice.Name = "ChkActiveImvoice"
        Me.ChkActiveImvoice.Size = New System.Drawing.Size(145, 23)
        Me.ChkActiveImvoice.TabIndex = 1060
        Me.ChkActiveImvoice.TabStop = True
        Me.ChkActiveImvoice.Text = "عرض الفواتير النشطة"
        Me.ChkActiveImvoice.UseVisualStyleBackColor = False
        '
        'ChkStartPay
        '
        Me.ChkStartPay.AutoSize = True
        Me.ChkStartPay.BackColor = System.Drawing.Color.Transparent
        Me.ChkStartPay.Checked = True
        Me.ChkStartPay.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChkStartPay.Location = New System.Drawing.Point(291, 57)
        Me.ChkStartPay.Name = "ChkStartPay"
        Me.ChkStartPay.Size = New System.Drawing.Size(97, 23)
        Me.ChkStartPay.TabIndex = 1063
        Me.ChkStartPay.Text = "دفعه مقدمة"
        Me.ChkStartPay.UseVisualStyleBackColor = False
        '
        'CmdExport
        '
        Me.CmdExport.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdExport.Appearance.ForeColor = System.Drawing.Color.Black
        Me.CmdExport.Appearance.Options.UseFont = True
        Me.CmdExport.Appearance.Options.UseForeColor = True
        Me.CmdExport.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdExport.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdExport.ImageOptions.SvgImage = CType(resources.GetObject("CmdExport.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdExport.Location = New System.Drawing.Point(14, 50)
        Me.CmdExport.Name = "CmdExport"
        Me.CmdExport.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdExport.Size = New System.Drawing.Size(81, 36)
        Me.CmdExport.TabIndex = 1062
        Me.CmdExport.Text = "تصدير"
        '
        'TxtDocGuid
        '
        Me.TxtDocGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtDocGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtDocGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtDocGuid.Location = New System.Drawing.Point(267, 11)
        Me.TxtDocGuid.Name = "TxtDocGuid"
        Me.TxtDocGuid.Size = New System.Drawing.Size(30, 25)
        Me.TxtDocGuid.TabIndex = 1059
        Me.TxtDocGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtDocGuid.Visible = False
        '
        'TxtDocName
        '
        Me.TxtDocName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtDocName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtDocName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtDocName.Location = New System.Drawing.Point(200, 9)
        Me.TxtDocName.Name = "TxtDocName"
        Me.TxtDocName.ReadOnly = True
        Me.TxtDocName.Size = New System.Drawing.Size(257, 24)
        Me.TxtDocName.TabIndex = 1058
        '
        'TxtUserGuid
        '
        Me.TxtUserGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtUserGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtUserGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtUserGuid.Location = New System.Drawing.Point(748, 46)
        Me.TxtUserGuid.Name = "TxtUserGuid"
        Me.TxtUserGuid.Size = New System.Drawing.Size(30, 25)
        Me.TxtUserGuid.TabIndex = 1057
        Me.TxtUserGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtUserGuid.Visible = False
        '
        'TxtUserName
        '
        Me.TxtUserName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtUserName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtUserName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtUserName.Location = New System.Drawing.Point(724, 46)
        Me.TxtUserName.Name = "TxtUserName"
        Me.TxtUserName.Size = New System.Drawing.Size(200, 24)
        Me.TxtUserName.TabIndex = 1056
        '
        'CmdDelBatch
        '
        Me.CmdDelBatch.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdDelBatch.ImageOptions.SvgImage = CType(resources.GetObject("CmdDelBatch.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdDelBatch.Location = New System.Drawing.Point(124, 11)
        Me.CmdDelBatch.Name = "CmdDelBatch"
        Me.CmdDelBatch.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdDelBatch.Size = New System.Drawing.Size(32, 25)
        Me.CmdDelBatch.TabIndex = 1055
        '
        'CmdFindBatch
        '
        Me.CmdFindBatch.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindBatch.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindBatch.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindBatch.Location = New System.Drawing.Point(162, 9)
        Me.CmdFindBatch.Name = "CmdFindBatch"
        Me.CmdFindBatch.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindBatch.Size = New System.Drawing.Size(32, 25)
        Me.CmdFindBatch.TabIndex = 1053
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(463, 11)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(74, 19)
        Me.Label3.TabIndex = 1051
        Me.Label3.Text = "نوع المستند"
        '
        'CmdDelSalesMan
        '
        Me.CmdDelSalesMan.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdDelSalesMan.ImageOptions.SvgImage = CType(resources.GetObject("CmdDelSalesMan.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdDelSalesMan.Location = New System.Drawing.Point(648, 47)
        Me.CmdDelSalesMan.Name = "CmdDelSalesMan"
        Me.CmdDelSalesMan.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdDelSalesMan.Size = New System.Drawing.Size(32, 25)
        Me.CmdDelSalesMan.TabIndex = 1046
        '
        'CmdFindSalesMan
        '
        Me.CmdFindSalesMan.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindSalesMan.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindSalesMan.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindSalesMan.Location = New System.Drawing.Point(686, 47)
        Me.CmdFindSalesMan.Name = "CmdFindSalesMan"
        Me.CmdFindSalesMan.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindSalesMan.Size = New System.Drawing.Size(32, 25)
        Me.CmdFindSalesMan.TabIndex = 1044
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(930, 51)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(56, 19)
        Me.Label5.TabIndex = 1042
        Me.Label5.Text = "المحصل"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.White
        Me.Label2.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(1170, 20)
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
        Me.Label1.Location = New System.Drawing.Point(1172, 53)
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
        Me.TxtFromDate.Location = New System.Drawing.Point(1025, 14)
        Me.TxtFromDate.Name = "TxtFromDate"
        Me.TxtFromDate.Size = New System.Drawing.Size(123, 27)
        Me.TxtFromDate.TabIndex = 1032
        '
        'TxtToDate
        '
        Me.TxtToDate.CustomFormat = ""
        Me.TxtToDate.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtToDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.TxtToDate.Location = New System.Drawing.Point(1025, 47)
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
        Me.Label4.Location = New System.Drawing.Point(949, 11)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(37, 19)
        Me.Label4.TabIndex = 1022
        Me.Label4.Text = "الفرع"
        '
        'CmdFindDepart
        '
        Me.CmdFindDepart.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindDepart.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindDepart.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindDepart.Location = New System.Drawing.Point(590, 12)
        Me.CmdFindDepart.Name = "CmdFindDepart"
        Me.CmdFindDepart.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindDepart.Size = New System.Drawing.Size(32, 25)
        Me.CmdFindDepart.TabIndex = 1024
        '
        'CmdDelBranch
        '
        Me.CmdDelBranch.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdDelBranch.ImageOptions.SvgImage = CType(resources.GetObject("CmdDelBranch.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdDelBranch.Location = New System.Drawing.Point(552, 11)
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
        Me.TxtBranchGuid.Location = New System.Drawing.Point(866, 9)
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
        Me.TxtBranchName.Location = New System.Drawing.Point(628, 11)
        Me.TxtBranchName.Name = "TxtBranchName"
        Me.TxtBranchName.ReadOnly = True
        Me.TxtBranchName.Size = New System.Drawing.Size(294, 24)
        Me.TxtBranchName.TabIndex = 1023
        '
        'SalesReport
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1496, 739)
        Me.Controls.Add(Me.GroupPanel2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SalesReport"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.Text = "تقرير المبيعات"
        Me.GroupPanel2.ResumeLayout(False)
        Me.GroupPanel3.ResumeLayout(False)
        Me.GroupPanel3.PerformLayout()
        CType(Me.BranchGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupPanel4.ResumeLayout(False)
        Me.GroupPanel4.PerformLayout()
        Me.GroupPanel8.ResumeLayout(False)
        Me.GroupPanel8.PerformLayout()
        Me.GroupPanel1.ResumeLayout(False)
        Me.GroupPanel7.ResumeLayout(False)
        Me.GroupPanel6.ResumeLayout(False)
        Me.GroupPanel6.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel7 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CmdView As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents GroupPanel6 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CmdDelBatch As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdFindBatch As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents Label3 As Label
    Friend WithEvents CmdDelSalesMan As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdFindSalesMan As DevExpress.XtraEditors.SimpleButton
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
    Friend WithEvents TxtUserGuid As Label
    Friend WithEvents TxtUserName As TextBox
    Friend WithEvents TxtDocGuid As Label
    Friend WithEvents TxtDocName As TextBox
    Friend WithEvents GroupPanel4 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtFind2 As TextBox
    Friend WithEvents TxtSubTotal As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents TxtTax As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents TxtTotal As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents TxtCount As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents GroupPanel3 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents BranchGrid As DevComponents.DotNetBar.Controls.DataGridViewX
    Friend WithEvents TxtTypeGuid As Label
    Friend WithEvents GroupPanel8 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents FindByName As RadioButton
    Friend WithEvents FindById As RadioButton
    Friend WithEvents ChkOflineInvoice As RadioButton
    Friend WithEvents ChkActiveImvoice As RadioButton
    Friend WithEvents TxtMsg As Label
    Friend WithEvents CmdExport As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
    Friend WithEvents DataGridViewTextBoxColumn22 As DataGridViewTextBoxColumn
    Friend WithEvents Column5 As DataGridViewTextBoxColumn
    Friend WithEvents Column14 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn21 As DataGridViewTextBoxColumn
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents Column15 As DataGridViewTextBoxColumn
    Friend WithEvents Column6 As DataGridViewTextBoxColumn
    Friend WithEvents Column7 As DataGridViewTextBoxColumn
    Friend WithEvents Column11 As DataGridViewTextBoxColumn
    Friend WithEvents Column13 As DataGridViewTextBoxColumn
    Friend WithEvents Column3 As DataGridViewTextBoxColumn
    Friend WithEvents Column4 As DataGridViewTextBoxColumn
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents Column8 As DataGridViewButtonColumn
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents ChkFessPay As CheckBox
    Friend WithEvents ChkMoPay As CheckBox
    Friend WithEvents ChkStartPay As CheckBox
End Class
