<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EncodingCashBox
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EncodingCashBox))
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel3 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.ChkUseSales = New System.Windows.Forms.CheckBox()
        Me.TxtCostCenterGuid = New System.Windows.Forms.Label()
        Me.CmdFindCostCenter = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtCostCenterName = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtAccountGuid = New System.Windows.Forms.Label()
        Me.TxtEndAccount = New System.Windows.Forms.Label()
        Me.TxtParentCode = New System.Windows.Forms.Label()
        Me.TxtParentGuid = New System.Windows.Forms.Label()
        Me.TxtAccountCode = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.ChkStaut = New Bunifu.Framework.UI.BunifuSwitch()
        Me.CmdFindCashBox = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtCashName = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtCode = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TxtParentAccount = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtTypeGuid = New System.Windows.Forms.Label()
        Me.TxtGuid = New System.Windows.Forms.Label()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdSave = New DevExpress.XtraEditors.SimpleButton()
        Me.CMDNew = New DevExpress.XtraEditors.SimpleButton()
        Me.ChkIsPublic = New System.Windows.Forms.CheckBox()
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
        Me.GroupPanel2.Size = New System.Drawing.Size(586, 309)
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
        Me.GroupPanel2.TabIndex = 866
        '
        'GroupPanel3
        '
        Me.GroupPanel3.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel3.Controls.Add(Me.ChkIsPublic)
        Me.GroupPanel3.Controls.Add(Me.ChkUseSales)
        Me.GroupPanel3.Controls.Add(Me.TxtCostCenterGuid)
        Me.GroupPanel3.Controls.Add(Me.CmdFindCostCenter)
        Me.GroupPanel3.Controls.Add(Me.TxtCostCenterName)
        Me.GroupPanel3.Controls.Add(Me.Label7)
        Me.GroupPanel3.Controls.Add(Me.TxtAccountGuid)
        Me.GroupPanel3.Controls.Add(Me.TxtEndAccount)
        Me.GroupPanel3.Controls.Add(Me.TxtParentCode)
        Me.GroupPanel3.Controls.Add(Me.TxtParentGuid)
        Me.GroupPanel3.Controls.Add(Me.TxtAccountCode)
        Me.GroupPanel3.Controls.Add(Me.Label8)
        Me.GroupPanel3.Controls.Add(Me.ChkStaut)
        Me.GroupPanel3.Controls.Add(Me.CmdFindCashBox)
        Me.GroupPanel3.Controls.Add(Me.TxtCashName)
        Me.GroupPanel3.Controls.Add(Me.Label2)
        Me.GroupPanel3.Controls.Add(Me.TxtCode)
        Me.GroupPanel3.Controls.Add(Me.Label5)
        Me.GroupPanel3.Controls.Add(Me.TxtParentAccount)
        Me.GroupPanel3.Controls.Add(Me.Label1)
        Me.GroupPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel3.Location = New System.Drawing.Point(0, 72)
        Me.GroupPanel3.Name = "GroupPanel3"
        Me.GroupPanel3.Size = New System.Drawing.Size(586, 237)
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
        Me.GroupPanel3.TabIndex = 868
        '
        'ChkUseSales
        '
        Me.ChkUseSales.AutoSize = True
        Me.ChkUseSales.BackColor = System.Drawing.Color.Transparent
        Me.ChkUseSales.Location = New System.Drawing.Point(393, 170)
        Me.ChkUseSales.Name = "ChkUseSales"
        Me.ChkUseSales.Size = New System.Drawing.Size(175, 23)
        Me.ChkUseSales.TabIndex = 1042
        Me.ChkUseSales.Text = "تستخدم فى فواتير المبيعات"
        Me.ChkUseSales.UseVisualStyleBackColor = False
        '
        'TxtCostCenterGuid
        '
        Me.TxtCostCenterGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtCostCenterGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCostCenterGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtCostCenterGuid.Location = New System.Drawing.Point(198, 80)
        Me.TxtCostCenterGuid.Name = "TxtCostCenterGuid"
        Me.TxtCostCenterGuid.Size = New System.Drawing.Size(43, 22)
        Me.TxtCostCenterGuid.TabIndex = 1041
        Me.TxtCostCenterGuid.Text = "00000000-0000-0000-0000-000000000000"
        Me.TxtCostCenterGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtCostCenterGuid.Visible = False
        '
        'CmdFindCostCenter
        '
        Me.CmdFindCostCenter.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindCostCenter.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindCostCenter.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindCostCenter.Location = New System.Drawing.Point(110, 79)
        Me.CmdFindCostCenter.Name = "CmdFindCostCenter"
        Me.CmdFindCostCenter.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindCostCenter.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindCostCenter.TabIndex = 1040
        '
        'TxtCostCenterName
        '
        Me.TxtCostCenterName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtCostCenterName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCostCenterName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCostCenterName.ForeColor = System.Drawing.Color.Black
        Me.TxtCostCenterName.Location = New System.Drawing.Point(152, 78)
        Me.TxtCostCenterName.Name = "TxtCostCenterName"
        Me.TxtCostCenterName.ReadOnly = True
        Me.TxtCostCenterName.Size = New System.Drawing.Size(314, 24)
        Me.TxtCostCenterName.TabIndex = 1039
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(496, 81)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(76, 19)
        Me.Label7.TabIndex = 1038
        Me.Label7.Text = "مركز التكلفة"
        '
        'TxtAccountGuid
        '
        Me.TxtAccountGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtAccountGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtAccountGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtAccountGuid.Location = New System.Drawing.Point(223, 137)
        Me.TxtAccountGuid.Name = "TxtAccountGuid"
        Me.TxtAccountGuid.Size = New System.Drawing.Size(54, 25)
        Me.TxtAccountGuid.TabIndex = 1027
        Me.TxtAccountGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtAccountGuid.Visible = False
        '
        'TxtEndAccount
        '
        Me.TxtEndAccount.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtEndAccount.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtEndAccount.ForeColor = System.Drawing.Color.Black
        Me.TxtEndAccount.Location = New System.Drawing.Point(94, 110)
        Me.TxtEndAccount.Name = "TxtEndAccount"
        Me.TxtEndAccount.Size = New System.Drawing.Size(54, 22)
        Me.TxtEndAccount.TabIndex = 1026
        Me.TxtEndAccount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtEndAccount.Visible = False
        '
        'TxtParentCode
        '
        Me.TxtParentCode.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtParentCode.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtParentCode.ForeColor = System.Drawing.Color.Black
        Me.TxtParentCode.Location = New System.Drawing.Point(154, 110)
        Me.TxtParentCode.Name = "TxtParentCode"
        Me.TxtParentCode.Size = New System.Drawing.Size(54, 24)
        Me.TxtParentCode.TabIndex = 1025
        Me.TxtParentCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtParentCode.Visible = False
        '
        'TxtParentGuid
        '
        Me.TxtParentGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtParentGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.TxtParentGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtParentGuid.Location = New System.Drawing.Point(214, 110)
        Me.TxtParentGuid.Name = "TxtParentGuid"
        Me.TxtParentGuid.Size = New System.Drawing.Size(54, 22)
        Me.TxtParentGuid.TabIndex = 1024
        Me.TxtParentGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtParentGuid.Visible = False
        '
        'TxtAccountCode
        '
        Me.TxtAccountCode.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtAccountCode.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtAccountCode.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtAccountCode.ForeColor = System.Drawing.Color.Red
        Me.TxtAccountCode.Location = New System.Drawing.Point(283, 140)
        Me.TxtAccountCode.Name = "TxtAccountCode"
        Me.TxtAccountCode.ReadOnly = True
        Me.TxtAccountCode.Size = New System.Drawing.Size(183, 24)
        Me.TxtAccountCode.TabIndex = 1019
        Me.TxtAccountCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(493, 143)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(79, 19)
        Me.Label8.TabIndex = 1018
        Me.Label8.Text = "رقم الحساب"
        '
        'ChkStaut
        '
        Me.ChkStaut.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ChkStaut.BorderRadius = 0
        Me.ChkStaut.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChkStaut.ForeColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ChkStaut.Location = New System.Drawing.Point(317, 19)
        Me.ChkStaut.Margin = New System.Windows.Forms.Padding(3, 5, 3, 5)
        Me.ChkStaut.Name = "ChkStaut"
        Me.ChkStaut.Oncolor = System.Drawing.Color.Chartreuse
        Me.ChkStaut.Onoffcolor = System.Drawing.Color.Red
        Me.ChkStaut.Size = New System.Drawing.Size(51, 19)
        Me.ChkStaut.TabIndex = 976
        Me.ChkStaut.Textcolor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ChkStaut.Value = True
        '
        'CmdFindCashBox
        '
        Me.CmdFindCashBox.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindCashBox.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindCashBox.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindCashBox.Location = New System.Drawing.Point(42, 46)
        Me.CmdFindCashBox.Name = "CmdFindCashBox"
        Me.CmdFindCashBox.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindCashBox.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindCashBox.TabIndex = 975
        '
        'TxtCashName
        '
        Me.TxtCashName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtCashName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCashName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtCashName.ForeColor = System.Drawing.Color.Black
        Me.TxtCashName.Location = New System.Drawing.Point(84, 48)
        Me.TxtCashName.Name = "TxtCashName"
        Me.TxtCashName.Size = New System.Drawing.Size(382, 24)
        Me.TxtCashName.TabIndex = 974
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(495, 53)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(77, 19)
        Me.Label2.TabIndex = 973
        Me.Label2.Text = "اسم الخزينة"
        '
        'TxtCode
        '
        Me.TxtCode.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtCode.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCode.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtCode.ForeColor = System.Drawing.Color.Black
        Me.TxtCode.Location = New System.Drawing.Point(374, 16)
        Me.TxtCode.Name = "TxtCode"
        Me.TxtCode.ReadOnly = True
        Me.TxtCode.Size = New System.Drawing.Size(92, 24)
        Me.TxtCode.TabIndex = 972
        Me.TxtCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(496, 21)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(76, 19)
        Me.Label5.TabIndex = 971
        Me.Label5.Text = "كود الخزينة"
        '
        'TxtParentAccount
        '
        Me.TxtParentAccount.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtParentAccount.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtParentAccount.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtParentAccount.ForeColor = System.Drawing.Color.Red
        Me.TxtParentAccount.Location = New System.Drawing.Point(283, 108)
        Me.TxtParentAccount.Name = "TxtParentAccount"
        Me.TxtParentAccount.ReadOnly = True
        Me.TxtParentAccount.Size = New System.Drawing.Size(183, 24)
        Me.TxtParentAccount.TabIndex = 969
        Me.TxtParentAccount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(472, 110)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(100, 19)
        Me.Label1.TabIndex = 968
        Me.Label1.Text = "الحساب الرئيسي"
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
        Me.GroupPanel1.Size = New System.Drawing.Size(586, 72)
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
        Me.GroupPanel1.TabIndex = 867
        '
        'TxtTypeGuid
        '
        Me.TxtTypeGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtTypeGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTypeGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtTypeGuid.Location = New System.Drawing.Point(42, 18)
        Me.TxtTypeGuid.Name = "TxtTypeGuid"
        Me.TxtTypeGuid.Size = New System.Drawing.Size(182, 31)
        Me.TxtTypeGuid.TabIndex = 1029
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
        Me.TxtGuid.TabIndex = 1028
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
        Me.CmdExit.Location = New System.Drawing.Point(343, 0)
        Me.CmdExit.Name = "CmdExit"
        Me.CmdExit.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdExit.Size = New System.Drawing.Size(81, 72)
        Me.CmdExit.TabIndex = 316
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
        Me.CmdSave.Location = New System.Drawing.Point(424, 0)
        Me.CmdSave.Name = "CmdSave"
        Me.CmdSave.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdSave.Size = New System.Drawing.Size(81, 72)
        Me.CmdSave.TabIndex = 314
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
        Me.CMDNew.Location = New System.Drawing.Point(505, 0)
        Me.CMDNew.Name = "CMDNew"
        Me.CMDNew.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CMDNew.Size = New System.Drawing.Size(81, 72)
        Me.CMDNew.TabIndex = 315
        Me.CMDNew.Text = "جديد F1"
        '
        'ChkIsPublic
        '
        Me.ChkIsPublic.AutoSize = True
        Me.ChkIsPublic.BackColor = System.Drawing.Color.Transparent
        Me.ChkIsPublic.Location = New System.Drawing.Point(412, 199)
        Me.ChkIsPublic.Name = "ChkIsPublic"
        Me.ChkIsPublic.Size = New System.Drawing.Size(156, 23)
        Me.ChkIsPublic.TabIndex = 1043
        Me.ChkIsPublic.Text = "تستخدم فى يومية الفرع"
        Me.ChkIsPublic.UseVisualStyleBackColor = False
        '
        'EncodingCashBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(586, 309)
        Me.Controls.Add(Me.GroupPanel2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EncodingCashBox"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "خزينة جديدة"
        Me.GroupPanel2.ResumeLayout(False)
        Me.GroupPanel3.ResumeLayout(False)
        Me.GroupPanel3.PerformLayout()
        Me.GroupPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel3 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CMDNew As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents ChkStaut As Bunifu.Framework.UI.BunifuSwitch
    Friend WithEvents CmdFindCashBox As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtCashName As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TxtCode As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents TxtParentAccount As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents TxtAccountCode As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents TxtEndAccount As Label
    Friend WithEvents TxtParentCode As Label
    Friend WithEvents TxtParentGuid As Label
    Friend WithEvents TxtAccountGuid As Label
    Friend WithEvents TxtGuid As Label
    Friend WithEvents TxtTypeGuid As Label
    Friend WithEvents TxtCostCenterGuid As Label
    Friend WithEvents CmdFindCostCenter As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtCostCenterName As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents ChkUseSales As CheckBox
    Friend WithEvents ChkIsPublic As CheckBox
End Class
