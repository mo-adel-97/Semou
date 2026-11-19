<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EncodingAcceptOrder
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EncodingAcceptOrder))
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel3 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtBatchGuid = New System.Windows.Forms.Label()
        Me.TxtBranchGuid = New System.Windows.Forms.Label()
        Me.TxtAccountGuid = New System.Windows.Forms.Label()
        Me.ChkStaut = New Bunifu.Framework.UI.BunifuSwitch()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TxtSallesManName = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.TxtNotes = New System.Windows.Forms.TextBox()
        Me.CmdFindBatch = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtBatchName = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.TxtToDate = New System.Windows.Forms.MaskedTextBox()
        Me.TxtFromDate = New System.Windows.Forms.MaskedTextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtCompanyName = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.CmdFindBranch = New DevExpress.XtraEditors.SimpleButton()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.TxtBranchName = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TxtTel = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtNationalId = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TxtStudentName = New System.Windows.Forms.TextBox()
        Me.TxtDateReg = New System.Windows.Forms.DateTimePicker()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtCode = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtTypeGuid = New System.Windows.Forms.Label()
        Me.TxtGuid = New System.Windows.Forms.Label()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdSave = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtCompanyGuid = New System.Windows.Forms.Label()
        Me.CmdFindCompany = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtDate = New System.Windows.Forms.Label()
        Me.TxtStartDate = New System.Windows.Forms.DateTimePicker()
        Me.TxtEndDate = New System.Windows.Forms.DateTimePicker()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TxtDiplomGuid = New System.Windows.Forms.Label()
        Me.CmdFindDiplom = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtDiplomName = New System.Windows.Forms.TextBox()
        Me.TxtSallesManGuid = New System.Windows.Forms.Label()
        Me.GroupPanel2.SuspendLayout()
        Me.GroupPanel3.SuspendLayout()
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
        Me.GroupPanel2.Size = New System.Drawing.Size(1150, 626)
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
        Me.GroupPanel3.Controls.Add(Me.TxtSallesManGuid)
        Me.GroupPanel3.Controls.Add(Me.TxtDiplomGuid)
        Me.GroupPanel3.Controls.Add(Me.CmdFindDiplom)
        Me.GroupPanel3.Controls.Add(Me.TxtDiplomName)
        Me.GroupPanel3.Controls.Add(Me.Label13)
        Me.GroupPanel3.Controls.Add(Me.Label14)
        Me.GroupPanel3.Controls.Add(Me.TxtEndDate)
        Me.GroupPanel3.Controls.Add(Me.TxtStartDate)
        Me.GroupPanel3.Controls.Add(Me.TxtDate)
        Me.GroupPanel3.Controls.Add(Me.CmdFindCompany)
        Me.GroupPanel3.Controls.Add(Me.TxtCompanyGuid)
        Me.GroupPanel3.Controls.Add(Me.TxtBatchGuid)
        Me.GroupPanel3.Controls.Add(Me.TxtBranchGuid)
        Me.GroupPanel3.Controls.Add(Me.TxtAccountGuid)
        Me.GroupPanel3.Controls.Add(Me.ChkStaut)
        Me.GroupPanel3.Controls.Add(Me.Label12)
        Me.GroupPanel3.Controls.Add(Me.Label11)
        Me.GroupPanel3.Controls.Add(Me.TxtSallesManName)
        Me.GroupPanel3.Controls.Add(Me.Label10)
        Me.GroupPanel3.Controls.Add(Me.TxtNotes)
        Me.GroupPanel3.Controls.Add(Me.CmdFindBatch)
        Me.GroupPanel3.Controls.Add(Me.TxtBatchName)
        Me.GroupPanel3.Controls.Add(Me.Label9)
        Me.GroupPanel3.Controls.Add(Me.TxtToDate)
        Me.GroupPanel3.Controls.Add(Me.TxtFromDate)
        Me.GroupPanel3.Controls.Add(Me.Label2)
        Me.GroupPanel3.Controls.Add(Me.Label3)
        Me.GroupPanel3.Controls.Add(Me.Label1)
        Me.GroupPanel3.Controls.Add(Me.TxtCompanyName)
        Me.GroupPanel3.Controls.Add(Me.Label15)
        Me.GroupPanel3.Controls.Add(Me.CmdFindBranch)
        Me.GroupPanel3.Controls.Add(Me.Label18)
        Me.GroupPanel3.Controls.Add(Me.TxtBranchName)
        Me.GroupPanel3.Controls.Add(Me.Label8)
        Me.GroupPanel3.Controls.Add(Me.TxtTel)
        Me.GroupPanel3.Controls.Add(Me.Label7)
        Me.GroupPanel3.Controls.Add(Me.TxtNationalId)
        Me.GroupPanel3.Controls.Add(Me.Label6)
        Me.GroupPanel3.Controls.Add(Me.TxtStudentName)
        Me.GroupPanel3.Controls.Add(Me.TxtDateReg)
        Me.GroupPanel3.Controls.Add(Me.Label4)
        Me.GroupPanel3.Controls.Add(Me.TxtCode)
        Me.GroupPanel3.Controls.Add(Me.Label5)
        Me.GroupPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel3.Location = New System.Drawing.Point(0, 72)
        Me.GroupPanel3.Name = "GroupPanel3"
        Me.GroupPanel3.Size = New System.Drawing.Size(1150, 554)
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
        Me.GroupPanel3.TabIndex = 875
        '
        'TxtBatchGuid
        '
        Me.TxtBatchGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBatchGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtBatchGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtBatchGuid.Location = New System.Drawing.Point(651, 230)
        Me.TxtBatchGuid.Name = "TxtBatchGuid"
        Me.TxtBatchGuid.Size = New System.Drawing.Size(54, 19)
        Me.TxtBatchGuid.TabIndex = 1127
        Me.TxtBatchGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBatchGuid.Visible = False
        '
        'TxtBranchGuid
        '
        Me.TxtBranchGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBranchGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtBranchGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtBranchGuid.Location = New System.Drawing.Point(554, 141)
        Me.TxtBranchGuid.Name = "TxtBranchGuid"
        Me.TxtBranchGuid.Size = New System.Drawing.Size(54, 19)
        Me.TxtBranchGuid.TabIndex = 1126
        Me.TxtBranchGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBranchGuid.Visible = False
        '
        'TxtAccountGuid
        '
        Me.TxtAccountGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtAccountGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtAccountGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtAccountGuid.Location = New System.Drawing.Point(100, 44)
        Me.TxtAccountGuid.Name = "TxtAccountGuid"
        Me.TxtAccountGuid.Size = New System.Drawing.Size(40, 31)
        Me.TxtAccountGuid.TabIndex = 1125
        Me.TxtAccountGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtAccountGuid.Visible = False
        '
        'ChkStaut
        '
        Me.ChkStaut.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ChkStaut.BorderRadius = 0
        Me.ChkStaut.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChkStaut.ForeColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ChkStaut.Location = New System.Drawing.Point(660, 77)
        Me.ChkStaut.Margin = New System.Windows.Forms.Padding(4, 8, 4, 8)
        Me.ChkStaut.Name = "ChkStaut"
        Me.ChkStaut.Oncolor = System.Drawing.Color.Chartreuse
        Me.ChkStaut.Onoffcolor = System.Drawing.Color.Red
        Me.ChkStaut.Size = New System.Drawing.Size(51, 19)
        Me.ChkStaut.TabIndex = 1124
        Me.ChkStaut.Textcolor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ChkStaut.Value = False
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.BackColor = System.Drawing.Color.Transparent
        Me.Label12.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label12.ForeColor = System.Drawing.Color.Black
        Me.Label12.Location = New System.Drawing.Point(739, 77)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(85, 19)
        Me.Label12.TabIndex = 1123
        Me.Label12.Text = "قبول الموافقة"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.BackColor = System.Drawing.Color.Transparent
        Me.Label11.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label11.ForeColor = System.Drawing.Color.Black
        Me.Label11.Location = New System.Drawing.Point(241, 298)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(97, 19)
        Me.Label11.TabIndex = 1121
        Me.Label11.Text = "القائم بالتسجيل"
        '
        'TxtSallesManName
        '
        Me.TxtSallesManName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtSallesManName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtSallesManName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtSallesManName.Location = New System.Drawing.Point(13, 295)
        Me.TxtSallesManName.Name = "TxtSallesManName"
        Me.TxtSallesManName.ReadOnly = True
        Me.TxtSallesManName.Size = New System.Drawing.Size(222, 24)
        Me.TxtSallesManName.TabIndex = 1122
        Me.TxtSallesManName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.BackColor = System.Drawing.Color.Transparent
        Me.Label10.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label10.ForeColor = System.Drawing.Color.Black
        Me.Label10.Location = New System.Drawing.Point(1067, 328)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(62, 19)
        Me.Label10.TabIndex = 1119
        Me.Label10.Text = "ملاحظات"
        '
        'TxtNotes
        '
        Me.TxtNotes.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtNotes.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtNotes.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtNotes.Location = New System.Drawing.Point(12, 325)
        Me.TxtNotes.Multiline = True
        Me.TxtNotes.Name = "TxtNotes"
        Me.TxtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TxtNotes.Size = New System.Drawing.Size(959, 216)
        Me.TxtNotes.TabIndex = 1120
        '
        'CmdFindBatch
        '
        Me.CmdFindBatch.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindBatch.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindBatch.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindBatch.Location = New System.Drawing.Point(595, 229)
        Me.CmdFindBatch.Name = "CmdFindBatch"
        Me.CmdFindBatch.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindBatch.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindBatch.TabIndex = 1118
        '
        'TxtBatchName
        '
        Me.TxtBatchName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBatchName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBatchName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBatchName.ForeColor = System.Drawing.Color.Black
        Me.TxtBatchName.Location = New System.Drawing.Point(637, 229)
        Me.TxtBatchName.Name = "TxtBatchName"
        Me.TxtBatchName.ReadOnly = True
        Me.TxtBatchName.Size = New System.Drawing.Size(334, 24)
        Me.TxtBatchName.TabIndex = 1117
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.Color.Transparent
        Me.Label9.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.Location = New System.Drawing.Point(979, 232)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(150, 19)
        Me.Label9.TabIndex = 1116
        Me.Label9.Text = "الدفعه المراد التسجيل بها"
        '
        'TxtToDate
        '
        Me.TxtToDate.Enabled = False
        Me.TxtToDate.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtToDate.Location = New System.Drawing.Point(845, 292)
        Me.TxtToDate.Mask = "00/00/0000"
        Me.TxtToDate.Name = "TxtToDate"
        Me.TxtToDate.Size = New System.Drawing.Size(126, 27)
        Me.TxtToDate.TabIndex = 1115
        Me.TxtToDate.ValidatingType = GetType(Date)
        '
        'TxtFromDate
        '
        Me.TxtFromDate.Enabled = False
        Me.TxtFromDate.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtFromDate.Location = New System.Drawing.Point(845, 259)
        Me.TxtFromDate.Mask = "00/00/0000"
        Me.TxtFromDate.Name = "TxtFromDate"
        Me.TxtFromDate.Size = New System.Drawing.Size(126, 27)
        Me.TxtFromDate.TabIndex = 1114
        Me.TxtFromDate.ValidatingType = GetType(Date)
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(997, 294)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(132, 19)
        Me.Label2.TabIndex = 1113
        Me.Label2.Text = "تاريخ نهاية الدراسة هــ"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(1006, 262)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(123, 19)
        Me.Label3.TabIndex = 1112
        Me.Label3.Text = "تاريخ بدء الدراسة هــ"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(977, 202)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(152, 19)
        Me.Label1.TabIndex = 1111
        Me.Label1.Text = "البرنامج المراد التسجيل به"
        '
        'TxtCompanyName
        '
        Me.TxtCompanyName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtCompanyName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCompanyName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtCompanyName.Location = New System.Drawing.Point(484, 169)
        Me.TxtCompanyName.Name = "TxtCompanyName"
        Me.TxtCompanyName.ReadOnly = True
        Me.TxtCompanyName.Size = New System.Drawing.Size(487, 24)
        Me.TxtCompanyName.TabIndex = 1071
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.BackColor = System.Drawing.Color.Transparent
        Me.Label15.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label15.ForeColor = System.Drawing.Color.Black
        Me.Label15.Location = New System.Drawing.Point(986, 170)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(143, 19)
        Me.Label15.TabIndex = 1069
        Me.Label15.Text = "القطاع التابع له المتدرب"
        '
        'CmdFindBranch
        '
        Me.CmdFindBranch.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindBranch.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindBranch.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindBranch.Location = New System.Drawing.Point(442, 137)
        Me.CmdFindBranch.Name = "CmdFindBranch"
        Me.CmdFindBranch.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindBranch.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindBranch.TabIndex = 1068
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.BackColor = System.Drawing.Color.Transparent
        Me.Label18.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label18.ForeColor = System.Drawing.Color.Black
        Me.Label18.Location = New System.Drawing.Point(1002, 139)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(127, 19)
        Me.Label18.TabIndex = 1066
        Me.Label18.Text = "فرع الدراسة المطلوب"
        '
        'TxtBranchName
        '
        Me.TxtBranchName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBranchName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBranchName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtBranchName.Location = New System.Drawing.Point(484, 139)
        Me.TxtBranchName.Name = "TxtBranchName"
        Me.TxtBranchName.ReadOnly = True
        Me.TxtBranchName.Size = New System.Drawing.Size(487, 24)
        Me.TxtBranchName.TabIndex = 1067
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(396, 83)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(70, 19)
        Me.Label8.TabIndex = 1064
        Me.Label8.Text = "رقم الجوال"
        '
        'TxtTel
        '
        Me.TxtTel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtTel.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtTel.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtTel.Location = New System.Drawing.Point(239, 81)
        Me.TxtTel.Name = "TxtTel"
        Me.TxtTel.ReadOnly = True
        Me.TxtTel.Size = New System.Drawing.Size(146, 24)
        Me.TxtTel.TabIndex = 1065
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(401, 53)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(65, 19)
        Me.Label7.TabIndex = 1062
        Me.Label7.Text = "رقم الهوية"
        '
        'TxtNationalId
        '
        Me.TxtNationalId.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtNationalId.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtNationalId.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtNationalId.Location = New System.Drawing.Point(239, 51)
        Me.TxtNationalId.Name = "TxtNationalId"
        Me.TxtNationalId.ReadOnly = True
        Me.TxtNationalId.Size = New System.Drawing.Size(146, 24)
        Me.TxtNationalId.TabIndex = 1063
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(391, 21)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(75, 19)
        Me.Label6.TabIndex = 1060
        Me.Label6.Text = "اسم الطالب"
        '
        'TxtStudentName
        '
        Me.TxtStudentName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtStudentName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtStudentName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtStudentName.Location = New System.Drawing.Point(12, 19)
        Me.TxtStudentName.Name = "TxtStudentName"
        Me.TxtStudentName.ReadOnly = True
        Me.TxtStudentName.Size = New System.Drawing.Size(373, 24)
        Me.TxtStudentName.TabIndex = 1061
        '
        'TxtDateReg
        '
        Me.TxtDateReg.CustomFormat = "dd/MM/yyyy HH:mm"
        Me.TxtDateReg.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtDateReg.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.TxtDateReg.Location = New System.Drawing.Point(875, 71)
        Me.TxtDateReg.Name = "TxtDateReg"
        Me.TxtDateReg.Size = New System.Drawing.Size(192, 27)
        Me.TxtDateReg.TabIndex = 1059
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(1081, 73)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(48, 19)
        Me.Label4.TabIndex = 1058
        Me.Label4.Text = "التاريخ"
        '
        'TxtCode
        '
        Me.TxtCode.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtCode.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCode.Font = New System.Drawing.Font("Calibri", 15.75!, System.Drawing.FontStyle.Bold)
        Me.TxtCode.Location = New System.Drawing.Point(963, 39)
        Me.TxtCode.Name = "TxtCode"
        Me.TxtCode.ReadOnly = True
        Me.TxtCode.Size = New System.Drawing.Size(104, 26)
        Me.TxtCode.TabIndex = 1055
        Me.TxtCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(1098, 44)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(31, 19)
        Me.Label5.TabIndex = 1056
        Me.Label5.Text = "كود"
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.TxtTypeGuid)
        Me.GroupPanel1.Controls.Add(Me.TxtGuid)
        Me.GroupPanel1.Controls.Add(Me.CmdExit)
        Me.GroupPanel1.Controls.Add(Me.CmdSave)
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(1150, 72)
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
        Me.GroupPanel1.TabIndex = 874
        '
        'TxtTypeGuid
        '
        Me.TxtTypeGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtTypeGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTypeGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtTypeGuid.Location = New System.Drawing.Point(122, 20)
        Me.TxtTypeGuid.Name = "TxtTypeGuid"
        Me.TxtTypeGuid.Size = New System.Drawing.Size(40, 31)
        Me.TxtTypeGuid.TabIndex = 962
        Me.TxtTypeGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtTypeGuid.Visible = False
        '
        'TxtGuid
        '
        Me.TxtGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtGuid.Location = New System.Drawing.Point(66, 20)
        Me.TxtGuid.Name = "TxtGuid"
        Me.TxtGuid.Size = New System.Drawing.Size(40, 31)
        Me.TxtGuid.TabIndex = 961
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
        Me.CmdExit.Location = New System.Drawing.Point(988, 0)
        Me.CmdExit.Name = "CmdExit"
        Me.CmdExit.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdExit.Size = New System.Drawing.Size(81, 72)
        Me.CmdExit.TabIndex = 325
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
        Me.CmdSave.Location = New System.Drawing.Point(1069, 0)
        Me.CmdSave.Name = "CmdSave"
        Me.CmdSave.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdSave.Size = New System.Drawing.Size(81, 72)
        Me.CmdSave.TabIndex = 323
        Me.CmdSave.Text = "حفظ F2"
        '
        'TxtCompanyGuid
        '
        Me.TxtCompanyGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtCompanyGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtCompanyGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtCompanyGuid.Location = New System.Drawing.Point(554, 171)
        Me.TxtCompanyGuid.Name = "TxtCompanyGuid"
        Me.TxtCompanyGuid.Size = New System.Drawing.Size(54, 19)
        Me.TxtCompanyGuid.TabIndex = 1128
        Me.TxtCompanyGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtCompanyGuid.Visible = False
        '
        'CmdFindCompany
        '
        Me.CmdFindCompany.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindCompany.ImageOptions.SvgImage = CType(resources.GetObject("SimpleButton1.ImageOptions.SvgImage1"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindCompany.Location = New System.Drawing.Point(442, 168)
        Me.CmdFindCompany.Name = "CmdFindCompany"
        Me.CmdFindCompany.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindCompany.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindCompany.TabIndex = 1129
        '
        'TxtDate
        '
        Me.TxtDate.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtDate.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtDate.ForeColor = System.Drawing.Color.Black
        Me.TxtDate.Location = New System.Drawing.Point(614, 171)
        Me.TxtDate.Name = "TxtDate"
        Me.TxtDate.Size = New System.Drawing.Size(54, 19)
        Me.TxtDate.TabIndex = 1130
        Me.TxtDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtDate.Visible = False
        '
        'TxtStartDate
        '
        Me.TxtStartDate.CustomFormat = ""
        Me.TxtStartDate.Enabled = False
        Me.TxtStartDate.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtStartDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.TxtStartDate.Location = New System.Drawing.Point(554, 259)
        Me.TxtStartDate.Name = "TxtStartDate"
        Me.TxtStartDate.Size = New System.Drawing.Size(135, 27)
        Me.TxtStartDate.TabIndex = 1131
        '
        'TxtEndDate
        '
        Me.TxtEndDate.CustomFormat = ""
        Me.TxtEndDate.Enabled = False
        Me.TxtEndDate.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtEndDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.TxtEndDate.Location = New System.Drawing.Point(554, 292)
        Me.TxtEndDate.Name = "TxtEndDate"
        Me.TxtEndDate.Size = New System.Drawing.Size(135, 27)
        Me.TxtEndDate.TabIndex = 1132
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.BackColor = System.Drawing.Color.Transparent
        Me.Label13.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label13.ForeColor = System.Drawing.Color.Black
        Me.Label13.Location = New System.Drawing.Point(692, 294)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(128, 19)
        Me.Label13.TabIndex = 1134
        Me.Label13.Text = "تاريخ نهاية الدراسة م"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.BackColor = System.Drawing.Color.Transparent
        Me.Label14.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label14.ForeColor = System.Drawing.Color.Black
        Me.Label14.Location = New System.Drawing.Point(701, 262)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(119, 19)
        Me.Label14.TabIndex = 1133
        Me.Label14.Text = "تاريخ بدء الدراسة م"
        '
        'TxtDiplomGuid
        '
        Me.TxtDiplomGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtDiplomGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtDiplomGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtDiplomGuid.Location = New System.Drawing.Point(554, 201)
        Me.TxtDiplomGuid.Name = "TxtDiplomGuid"
        Me.TxtDiplomGuid.Size = New System.Drawing.Size(54, 19)
        Me.TxtDiplomGuid.TabIndex = 1137
        Me.TxtDiplomGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtDiplomGuid.Visible = False
        '
        'CmdFindDiplom
        '
        Me.CmdFindDiplom.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindDiplom.ImageOptions.SvgImage = CType(resources.GetObject("SimpleButton1.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindDiplom.Location = New System.Drawing.Point(442, 197)
        Me.CmdFindDiplom.Name = "CmdFindDiplom"
        Me.CmdFindDiplom.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindDiplom.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindDiplom.TabIndex = 1136
        '
        'TxtDiplomName
        '
        Me.TxtDiplomName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtDiplomName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtDiplomName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtDiplomName.Location = New System.Drawing.Point(484, 199)
        Me.TxtDiplomName.Name = "TxtDiplomName"
        Me.TxtDiplomName.ReadOnly = True
        Me.TxtDiplomName.Size = New System.Drawing.Size(487, 24)
        Me.TxtDiplomName.TabIndex = 1135
        '
        'TxtSallesManGuid
        '
        Me.TxtSallesManGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtSallesManGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSallesManGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtSallesManGuid.Location = New System.Drawing.Point(51, 259)
        Me.TxtSallesManGuid.Name = "TxtSallesManGuid"
        Me.TxtSallesManGuid.Size = New System.Drawing.Size(40, 31)
        Me.TxtSallesManGuid.TabIndex = 1138
        Me.TxtSallesManGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtSallesManGuid.Visible = False
        '
        'EncodingAcceptOrder
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1150, 626)
        Me.Controls.Add(Me.GroupPanel2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EncodingAcceptOrder"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "طلب موافقة دراسية"
        Me.GroupPanel2.ResumeLayout(False)
        Me.GroupPanel3.ResumeLayout(False)
        Me.GroupPanel3.PerformLayout()
        Me.GroupPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel3 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtTypeGuid As Label
    Friend WithEvents TxtGuid As Label
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtDateReg As DateTimePicker
    Friend WithEvents Label4 As Label
    Friend WithEvents TxtCode As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents TxtTel As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents TxtNationalId As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents TxtStudentName As TextBox
    Friend WithEvents Label18 As Label
    Friend WithEvents TxtBranchName As TextBox
    Friend WithEvents CmdFindBranch As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtCompanyName As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents TxtToDate As MaskedTextBox
    Friend WithEvents TxtFromDate As MaskedTextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents TxtNotes As TextBox
    Friend WithEvents CmdFindBatch As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtBatchName As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents TxtSallesManName As TextBox
    Friend WithEvents Label12 As Label
    Friend WithEvents ChkStaut As Bunifu.Framework.UI.BunifuSwitch
    Friend WithEvents TxtAccountGuid As Label
    Friend WithEvents TxtBranchGuid As Label
    Friend WithEvents TxtBatchGuid As Label
    Friend WithEvents CmdFindCompany As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtCompanyGuid As Label
    Friend WithEvents TxtDate As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents TxtEndDate As DateTimePicker
    Friend WithEvents TxtStartDate As DateTimePicker
    Friend WithEvents TxtDiplomGuid As Label
    Friend WithEvents CmdFindDiplom As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtDiplomName As TextBox
    Friend WithEvents TxtSallesManGuid As Label
End Class
