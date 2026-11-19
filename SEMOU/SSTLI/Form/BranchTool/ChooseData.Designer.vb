<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ChooseData
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
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ChooseData))
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel3 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtBranchGuid = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.RoomGrid = New DevComponents.DotNetBar.Controls.DataGridViewX()
        Me.MDayName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MDayGuid = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtRoomGuid = New System.Windows.Forms.Label()
        Me.TxtTranierGuid = New System.Windows.Forms.Label()
        Me.TxtBookGuid = New System.Windows.Forms.Label()
        Me.TxttimeGuid = New System.Windows.Forms.Label()
        Me.TxtDayGuid = New System.Windows.Forms.Label()
        Me.TxtDayName = New System.Windows.Forms.TextBox()
        Me.TxtTimeName = New System.Windows.Forms.TextBox()
        Me.TxtBookName = New System.Windows.Forms.TextBox()
        Me.TxtTranierName = New System.Windows.Forms.TextBox()
        Me.TxtRoomName = New System.Windows.Forms.TextBox()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.CmdClose = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtRowIndex = New System.Windows.Forms.Label()
        Me.CmdSave = New DevExpress.XtraEditors.SimpleButton()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.GroupPanel2.SuspendLayout()
        Me.GroupPanel3.SuspendLayout()
        CType(Me.RoomGrid, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.GroupPanel2.Size = New System.Drawing.Size(979, 450)
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
        Me.GroupPanel2.TabIndex = 874
        '
        'GroupPanel3
        '
        Me.GroupPanel3.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel3.Controls.Add(Me.TxtBranchGuid)
        Me.GroupPanel3.Controls.Add(Me.Label2)
        Me.GroupPanel3.Controls.Add(Me.Label1)
        Me.GroupPanel3.Controls.Add(Me.RoomGrid)
        Me.GroupPanel3.Controls.Add(Me.Label8)
        Me.GroupPanel3.Controls.Add(Me.Label7)
        Me.GroupPanel3.Controls.Add(Me.Label6)
        Me.GroupPanel3.Controls.Add(Me.Label5)
        Me.GroupPanel3.Controls.Add(Me.Label4)
        Me.GroupPanel3.Controls.Add(Me.TxtRoomGuid)
        Me.GroupPanel3.Controls.Add(Me.TxtTranierGuid)
        Me.GroupPanel3.Controls.Add(Me.TxtBookGuid)
        Me.GroupPanel3.Controls.Add(Me.TxttimeGuid)
        Me.GroupPanel3.Controls.Add(Me.TxtDayGuid)
        Me.GroupPanel3.Controls.Add(Me.TxtDayName)
        Me.GroupPanel3.Controls.Add(Me.TxtTimeName)
        Me.GroupPanel3.Controls.Add(Me.TxtBookName)
        Me.GroupPanel3.Controls.Add(Me.TxtTranierName)
        Me.GroupPanel3.Controls.Add(Me.TxtRoomName)
        Me.GroupPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel3.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel3.Name = "GroupPanel3"
        Me.GroupPanel3.Size = New System.Drawing.Size(979, 378)
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
        Me.GroupPanel3.TabIndex = 1016
        '
        'TxtBranchGuid
        '
        Me.TxtBranchGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBranchGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBranchGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtBranchGuid.Location = New System.Drawing.Point(633, 314)
        Me.TxtBranchGuid.Name = "TxtBranchGuid"
        Me.TxtBranchGuid.Size = New System.Drawing.Size(40, 23)
        Me.TxtBranchGuid.TabIndex = 1079
        Me.TxtBranchGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBranchGuid.Visible = False
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Calibri", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Red
        Me.Label2.Location = New System.Drawing.Point(480, 24)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(446, 106)
        Me.Label2.TabIndex = 1078
        Me.Label2.Text = "بعد اختيار القاعه لطفا يرجي مراجعه البيانات التى قمت باختيارها قبل الضغط على تأكي" &
    "د"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Red
        Me.Label1.Location = New System.Drawing.Point(138, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(148, 19)
        Me.Label1.TabIndex = 1077
        Me.Label1.Text = "القاعات الدراسية المتاحة"
        '
        'RoomGrid
        '
        Me.RoomGrid.AllowUserToAddRows = False
        Me.RoomGrid.AllowUserToDeleteRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.FloralWhite
        Me.RoomGrid.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.RoomGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.RoomGrid.BackgroundColor = System.Drawing.Color.White
        Me.RoomGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.RoomGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.RoomGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.MDayName, Me.MDayGuid})
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.RoomGrid.DefaultCellStyle = DataGridViewCellStyle3
        Me.RoomGrid.GridColor = System.Drawing.Color.FromArgb(CType(CType(208, Byte), Integer), CType(CType(215, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.RoomGrid.Location = New System.Drawing.Point(49, 31)
        Me.RoomGrid.MultiSelect = False
        Me.RoomGrid.Name = "RoomGrid"
        Me.RoomGrid.ReadOnly = True
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.RoomGrid.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.RoomGrid.RowHeadersVisible = False
        Me.RoomGrid.RowTemplate.Height = 33
        Me.RoomGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.RoomGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.RoomGrid.Size = New System.Drawing.Size(233, 341)
        Me.RoomGrid.TabIndex = 1076
        '
        'MDayName
        '
        Me.MDayName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.Format = "t"
        DataGridViewCellStyle2.NullValue = Nothing
        Me.MDayName.DefaultCellStyle = DataGridViewCellStyle2
        Me.MDayName.HeaderText = "اسم القاعه"
        Me.MDayName.Name = "MDayName"
        Me.MDayName.ReadOnly = True
        Me.MDayName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'MDayGuid
        '
        Me.MDayGuid.HeaderText = "guid"
        Me.MDayGuid.Name = "MDayGuid"
        Me.MDayGuid.ReadOnly = True
        Me.MDayGuid.Visible = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(866, 270)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(94, 19)
        Me.Label8.TabIndex = 1070
        Me.Label8.Text = "القاعه الدراسية"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(884, 240)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(76, 19)
        Me.Label7.TabIndex = 1069
        Me.Label7.Text = "اسم المدرب"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(872, 210)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 19)
        Me.Label6.TabIndex = 1068
        Me.Label6.Text = "المقرر الدراسي"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(922, 180)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(38, 19)
        Me.Label5.TabIndex = 1067
        Me.Label5.Text = "الفترة"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(925, 150)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(35, 19)
        Me.Label4.TabIndex = 1066
        Me.Label4.Text = "اليوم"
        '
        'TxtRoomGuid
        '
        Me.TxtRoomGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtRoomGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtRoomGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtRoomGuid.Location = New System.Drawing.Point(751, 268)
        Me.TxtRoomGuid.Name = "TxtRoomGuid"
        Me.TxtRoomGuid.Size = New System.Drawing.Size(40, 23)
        Me.TxtRoomGuid.TabIndex = 1065
        Me.TxtRoomGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtRoomGuid.Visible = False
        '
        'TxtTranierGuid
        '
        Me.TxtTranierGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtTranierGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTranierGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtTranierGuid.Location = New System.Drawing.Point(687, 238)
        Me.TxtTranierGuid.Name = "TxtTranierGuid"
        Me.TxtTranierGuid.Size = New System.Drawing.Size(40, 23)
        Me.TxtTranierGuid.TabIndex = 1064
        Me.TxtTranierGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtTranierGuid.Visible = False
        '
        'TxtBookGuid
        '
        Me.TxtBookGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBookGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBookGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtBookGuid.Location = New System.Drawing.Point(751, 211)
        Me.TxtBookGuid.Name = "TxtBookGuid"
        Me.TxtBookGuid.Size = New System.Drawing.Size(40, 23)
        Me.TxtBookGuid.TabIndex = 1063
        Me.TxtBookGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBookGuid.Visible = False
        '
        'TxttimeGuid
        '
        Me.TxttimeGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxttimeGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxttimeGuid.ForeColor = System.Drawing.Color.Black
        Me.TxttimeGuid.Location = New System.Drawing.Point(782, 180)
        Me.TxttimeGuid.Name = "TxttimeGuid"
        Me.TxttimeGuid.Size = New System.Drawing.Size(40, 23)
        Me.TxttimeGuid.TabIndex = 1062
        Me.TxttimeGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxttimeGuid.Visible = False
        '
        'TxtDayGuid
        '
        Me.TxtDayGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtDayGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDayGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtDayGuid.Location = New System.Drawing.Point(728, 151)
        Me.TxtDayGuid.Name = "TxtDayGuid"
        Me.TxtDayGuid.Size = New System.Drawing.Size(40, 23)
        Me.TxtDayGuid.TabIndex = 1061
        Me.TxtDayGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtDayGuid.Visible = False
        '
        'TxtDayName
        '
        Me.TxtDayName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtDayName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtDayName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDayName.ForeColor = System.Drawing.Color.Black
        Me.TxtDayName.Location = New System.Drawing.Point(657, 147)
        Me.TxtDayName.Name = "TxtDayName"
        Me.TxtDayName.ReadOnly = True
        Me.TxtDayName.Size = New System.Drawing.Size(200, 24)
        Me.TxtDayName.TabIndex = 1071
        Me.TxtDayName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TxtTimeName
        '
        Me.TxtTimeName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtTimeName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtTimeName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTimeName.ForeColor = System.Drawing.Color.Black
        Me.TxtTimeName.Location = New System.Drawing.Point(657, 177)
        Me.TxtTimeName.Name = "TxtTimeName"
        Me.TxtTimeName.ReadOnly = True
        Me.TxtTimeName.Size = New System.Drawing.Size(200, 24)
        Me.TxtTimeName.TabIndex = 1072
        Me.TxtTimeName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TxtBookName
        '
        Me.TxtBookName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBookName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBookName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBookName.ForeColor = System.Drawing.Color.Black
        Me.TxtBookName.Location = New System.Drawing.Point(450, 207)
        Me.TxtBookName.Name = "TxtBookName"
        Me.TxtBookName.ReadOnly = True
        Me.TxtBookName.Size = New System.Drawing.Size(407, 24)
        Me.TxtBookName.TabIndex = 1073
        '
        'TxtTranierName
        '
        Me.TxtTranierName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtTranierName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtTranierName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTranierName.ForeColor = System.Drawing.Color.Black
        Me.TxtTranierName.Location = New System.Drawing.Point(450, 237)
        Me.TxtTranierName.Name = "TxtTranierName"
        Me.TxtTranierName.ReadOnly = True
        Me.TxtTranierName.Size = New System.Drawing.Size(407, 24)
        Me.TxtTranierName.TabIndex = 1074
        '
        'TxtRoomName
        '
        Me.TxtRoomName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtRoomName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtRoomName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtRoomName.ForeColor = System.Drawing.Color.Black
        Me.TxtRoomName.Location = New System.Drawing.Point(614, 267)
        Me.TxtRoomName.Name = "TxtRoomName"
        Me.TxtRoomName.ReadOnly = True
        Me.TxtRoomName.Size = New System.Drawing.Size(243, 24)
        Me.TxtRoomName.TabIndex = 1075
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.CmdClose)
        Me.GroupPanel1.Controls.Add(Me.TxtRowIndex)
        Me.GroupPanel1.Controls.Add(Me.CmdSave)
        Me.GroupPanel1.Controls.Add(Me.Label3)
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 378)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(979, 72)
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
        Me.GroupPanel1.TabIndex = 1015
        '
        'CmdClose
        '
        Me.CmdClose.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdClose.Appearance.ForeColor = System.Drawing.Color.Black
        Me.CmdClose.Appearance.Options.UseFont = True
        Me.CmdClose.Appearance.Options.UseForeColor = True
        Me.CmdClose.Dock = System.Windows.Forms.DockStyle.Right
        Me.CmdClose.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter
        Me.CmdClose.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdClose.ImageOptions.SvgImage = CType(resources.GetObject("CmdClose.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdClose.Location = New System.Drawing.Point(817, 0)
        Me.CmdClose.Name = "CmdClose"
        Me.CmdClose.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdClose.Size = New System.Drawing.Size(81, 72)
        Me.CmdClose.TabIndex = 1042
        Me.CmdClose.Text = "إلغاء"
        '
        'TxtRowIndex
        '
        Me.TxtRowIndex.AutoSize = True
        Me.TxtRowIndex.BackColor = System.Drawing.Color.Transparent
        Me.TxtRowIndex.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtRowIndex.ForeColor = System.Drawing.Color.Black
        Me.TxtRowIndex.Location = New System.Drawing.Point(57, 18)
        Me.TxtRowIndex.Name = "TxtRowIndex"
        Me.TxtRowIndex.Size = New System.Drawing.Size(73, 19)
        Me.TxtRowIndex.TabIndex = 944
        Me.TxtRowIndex.Text = "اسم الدبلوم"
        Me.TxtRowIndex.Visible = False
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
        Me.CmdSave.Location = New System.Drawing.Point(898, 0)
        Me.CmdSave.Name = "CmdSave"
        Me.CmdSave.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdSave.Size = New System.Drawing.Size(81, 72)
        Me.CmdSave.TabIndex = 317
        Me.CmdSave.Text = "تأكيد"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(291, 18)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(68, 19)
        Me.Label3.TabIndex = 947
        Me.Label3.Text = "رقم الصف"
        Me.Label3.Visible = False
        '
        'ChooseData
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(979, 450)
        Me.Controls.Add(Me.GroupPanel2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ChooseData"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "اختيار القاعه وتأكيد البيانات"
        Me.GroupPanel2.ResumeLayout(False)
        Me.GroupPanel3.ResumeLayout(False)
        Me.GroupPanel3.PerformLayout()
        CType(Me.RoomGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupPanel1.ResumeLayout(False)
        Me.GroupPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents Label3 As Label
    Friend WithEvents TxtRowIndex As Label
    Friend WithEvents GroupPanel3 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CmdClose As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents TxtRoomGuid As Label
    Friend WithEvents TxtTranierGuid As Label
    Friend WithEvents TxtBookGuid As Label
    Friend WithEvents TxttimeGuid As Label
    Friend WithEvents TxtDayGuid As Label
    Friend WithEvents TxtDayName As TextBox
    Friend WithEvents TxtTimeName As TextBox
    Friend WithEvents TxtBookName As TextBox
    Friend WithEvents TxtTranierName As TextBox
    Friend WithEvents TxtRoomName As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents RoomGrid As DevComponents.DotNetBar.Controls.DataGridViewX
    Friend WithEvents MDayName As DataGridViewTextBoxColumn
    Friend WithEvents MDayGuid As DataGridViewTextBoxColumn
    Friend WithEvents TxtBranchGuid As Label
End Class
