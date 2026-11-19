<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EncodingTransMoney
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EncodingTransMoney))
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtDay1Code = New System.Windows.Forms.Label()
        Me.TxtDay1Guid = New System.Windows.Forms.Label()
        Me.CmdClose = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtGuid = New System.Windows.Forms.Label()
        Me.TxtTypeGuid = New System.Windows.Forms.Label()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdSave = New DevExpress.XtraEditors.SimpleButton()
        Me.CMDNew = New DevExpress.XtraEditors.SimpleButton()
        Me.GroupPanel3 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel5 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtAmount = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TxtNotes = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtBranchGuid = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtDayDate = New System.Windows.Forms.DateTimePicker()
        Me.CmdFindBranch = New DevExpress.XtraEditors.SimpleButton()
        Me.ChkStaut = New Bunifu.Framework.UI.BunifuSwitch()
        Me.TxtCode = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.CmdFind = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtBranchName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtRef = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TxtFromCashBoxGuid = New System.Windows.Forms.Label()
        Me.TxtFromCashBoxName = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.CmdFindFromCashBox = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtToCashBoxGuid = New System.Windows.Forms.Label()
        Me.TxtToCashBoxName = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.CmdFindToCashBox = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtCostCenterGuid = New System.Windows.Forms.Label()
        Me.CmdFindCostCenter = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtCostCenterName = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtOpenDailyType = New System.Windows.Forms.Label()
        Me.GroupPanel1.SuspendLayout()
        Me.GroupPanel2.SuspendLayout()
        Me.GroupPanel3.SuspendLayout()
        Me.GroupPanel5.SuspendLayout()
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
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(992, 566)
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
        Me.GroupPanel1.TabIndex = 873
        '
        'GroupPanel2
        '
        Me.GroupPanel2.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel2.Controls.Add(Me.TxtOpenDailyType)
        Me.GroupPanel2.Controls.Add(Me.TxtDay1Code)
        Me.GroupPanel2.Controls.Add(Me.TxtDay1Guid)
        Me.GroupPanel2.Controls.Add(Me.CmdClose)
        Me.GroupPanel2.Controls.Add(Me.TxtGuid)
        Me.GroupPanel2.Controls.Add(Me.TxtTypeGuid)
        Me.GroupPanel2.Controls.Add(Me.CmdExit)
        Me.GroupPanel2.Controls.Add(Me.CmdSave)
        Me.GroupPanel2.Controls.Add(Me.CMDNew)
        Me.GroupPanel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel2.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel2.Name = "GroupPanel2"
        Me.GroupPanel2.Size = New System.Drawing.Size(992, 72)
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
        Me.GroupPanel2.TabIndex = 873
        '
        'TxtDay1Code
        '
        Me.TxtDay1Code.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtDay1Code.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDay1Code.ForeColor = System.Drawing.Color.Black
        Me.TxtDay1Code.Location = New System.Drawing.Point(442, 20)
        Me.TxtDay1Code.Name = "TxtDay1Code"
        Me.TxtDay1Code.Size = New System.Drawing.Size(91, 22)
        Me.TxtDay1Code.TabIndex = 1044
        Me.TxtDay1Code.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtDay1Code.Visible = False
        '
        'TxtDay1Guid
        '
        Me.TxtDay1Guid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtDay1Guid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDay1Guid.ForeColor = System.Drawing.Color.Black
        Me.TxtDay1Guid.Location = New System.Drawing.Point(539, 20)
        Me.TxtDay1Guid.Name = "TxtDay1Guid"
        Me.TxtDay1Guid.Size = New System.Drawing.Size(105, 22)
        Me.TxtDay1Guid.TabIndex = 1043
        Me.TxtDay1Guid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtDay1Guid.Visible = False
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
        Me.CmdClose.Location = New System.Drawing.Point(668, 0)
        Me.CmdClose.Name = "CmdClose"
        Me.CmdClose.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdClose.Size = New System.Drawing.Size(81, 72)
        Me.CmdClose.TabIndex = 1042
        Me.CmdClose.Text = "خروج F4"
        '
        'TxtGuid
        '
        Me.TxtGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtGuid.Location = New System.Drawing.Point(345, 20)
        Me.TxtGuid.Name = "TxtGuid"
        Me.TxtGuid.Size = New System.Drawing.Size(91, 22)
        Me.TxtGuid.TabIndex = 1041
        Me.TxtGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtGuid.Visible = False
        '
        'TxtTypeGuid
        '
        Me.TxtTypeGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtTypeGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTypeGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtTypeGuid.Location = New System.Drawing.Point(224, 20)
        Me.TxtTypeGuid.Name = "TxtTypeGuid"
        Me.TxtTypeGuid.Size = New System.Drawing.Size(115, 22)
        Me.TxtTypeGuid.TabIndex = 1040
        Me.TxtTypeGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtTypeGuid.Visible = False
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
        Me.CmdExit.Location = New System.Drawing.Point(749, 0)
        Me.CmdExit.Name = "CmdExit"
        Me.CmdExit.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdExit.Size = New System.Drawing.Size(81, 72)
        Me.CmdExit.TabIndex = 319
        Me.CmdExit.Text = "طباعه F3"
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
        Me.CmdSave.Location = New System.Drawing.Point(830, 0)
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
        Me.CMDNew.Location = New System.Drawing.Point(911, 0)
        Me.CMDNew.Name = "CMDNew"
        Me.CMDNew.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CMDNew.Size = New System.Drawing.Size(81, 72)
        Me.CMDNew.TabIndex = 318
        Me.CMDNew.Text = "جديد F1"
        '
        'GroupPanel3
        '
        Me.GroupPanel3.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel3.Controls.Add(Me.GroupPanel5)
        Me.GroupPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel3.Location = New System.Drawing.Point(0, 72)
        Me.GroupPanel3.Name = "GroupPanel3"
        Me.GroupPanel3.Size = New System.Drawing.Size(992, 494)
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
        Me.GroupPanel3.TabIndex = 874
        '
        'GroupPanel5
        '
        Me.GroupPanel5.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel5.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel5.Controls.Add(Me.TxtCostCenterGuid)
        Me.GroupPanel5.Controls.Add(Me.CmdFindCostCenter)
        Me.GroupPanel5.Controls.Add(Me.TxtCostCenterName)
        Me.GroupPanel5.Controls.Add(Me.Label4)
        Me.GroupPanel5.Controls.Add(Me.TxtToCashBoxGuid)
        Me.GroupPanel5.Controls.Add(Me.TxtToCashBoxName)
        Me.GroupPanel5.Controls.Add(Me.Label10)
        Me.GroupPanel5.Controls.Add(Me.CmdFindToCashBox)
        Me.GroupPanel5.Controls.Add(Me.TxtFromCashBoxGuid)
        Me.GroupPanel5.Controls.Add(Me.TxtFromCashBoxName)
        Me.GroupPanel5.Controls.Add(Me.Label7)
        Me.GroupPanel5.Controls.Add(Me.CmdFindFromCashBox)
        Me.GroupPanel5.Controls.Add(Me.TxtRef)
        Me.GroupPanel5.Controls.Add(Me.Label6)
        Me.GroupPanel5.Controls.Add(Me.TxtAmount)
        Me.GroupPanel5.Controls.Add(Me.Label8)
        Me.GroupPanel5.Controls.Add(Me.TxtNotes)
        Me.GroupPanel5.Controls.Add(Me.Label3)
        Me.GroupPanel5.Controls.Add(Me.TxtBranchGuid)
        Me.GroupPanel5.Controls.Add(Me.Label2)
        Me.GroupPanel5.Controls.Add(Me.TxtDayDate)
        Me.GroupPanel5.Controls.Add(Me.CmdFindBranch)
        Me.GroupPanel5.Controls.Add(Me.ChkStaut)
        Me.GroupPanel5.Controls.Add(Me.TxtCode)
        Me.GroupPanel5.Controls.Add(Me.Label5)
        Me.GroupPanel5.Controls.Add(Me.CmdFind)
        Me.GroupPanel5.Controls.Add(Me.TxtBranchName)
        Me.GroupPanel5.Controls.Add(Me.Label1)
        Me.GroupPanel5.Dock = System.Windows.Forms.DockStyle.Right
        Me.GroupPanel5.Location = New System.Drawing.Point(13, 0)
        Me.GroupPanel5.Name = "GroupPanel5"
        Me.GroupPanel5.Size = New System.Drawing.Size(979, 494)
        '
        '
        '
        Me.GroupPanel5.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel5.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel5.Style.BackColorGradientAngle = 90
        Me.GroupPanel5.Style.BorderBottomWidth = 1
        Me.GroupPanel5.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel5.Style.BorderLeftWidth = 1
        Me.GroupPanel5.Style.BorderRightWidth = 1
        Me.GroupPanel5.Style.BorderTopWidth = 1
        Me.GroupPanel5.Style.CornerDiameter = 4
        Me.GroupPanel5.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel5.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel5.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel5.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel5.TabIndex = 870
        '
        'TxtAmount
        '
        Me.TxtAmount.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtAmount.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtAmount.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtAmount.ForeColor = System.Drawing.Color.Red
        Me.TxtAmount.Location = New System.Drawing.Point(154, 25)
        Me.TxtAmount.Name = "TxtAmount"
        Me.TxtAmount.Size = New System.Drawing.Size(135, 24)
        Me.TxtAmount.TabIndex = 1046
        Me.TxtAmount.Text = "0"
        Me.TxtAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(319, 30)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(40, 19)
        Me.Label8.TabIndex = 1045
        Me.Label8.Text = "المبلغ"
        '
        'TxtNotes
        '
        Me.TxtNotes.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtNotes.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtNotes.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtNotes.ForeColor = System.Drawing.Color.Black
        Me.TxtNotes.Location = New System.Drawing.Point(19, 240)
        Me.TxtNotes.Multiline = True
        Me.TxtNotes.Name = "TxtNotes"
        Me.TxtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TxtNotes.Size = New System.Drawing.Size(830, 217)
        Me.TxtNotes.TabIndex = 1041
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(921, 243)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(39, 19)
        Me.Label3.TabIndex = 1040
        Me.Label3.Text = "البيان"
        '
        'TxtBranchGuid
        '
        Me.TxtBranchGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBranchGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBranchGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtBranchGuid.Location = New System.Drawing.Point(558, 120)
        Me.TxtBranchGuid.Name = "TxtBranchGuid"
        Me.TxtBranchGuid.Size = New System.Drawing.Size(40, 22)
        Me.TxtBranchGuid.TabIndex = 1039
        Me.TxtBranchGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBranchGuid.Visible = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(912, 60)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(48, 19)
        Me.Label2.TabIndex = 1038
        Me.Label2.Text = "التاريخ"
        '
        'TxtDayDate
        '
        Me.TxtDayDate.CustomFormat = ""
        Me.TxtDayDate.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.TxtDayDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.TxtDayDate.Location = New System.Drawing.Point(731, 54)
        Me.TxtDayDate.Name = "TxtDayDate"
        Me.TxtDayDate.Size = New System.Drawing.Size(118, 27)
        Me.TxtDayDate.TabIndex = 1037
        '
        'CmdFindBranch
        '
        Me.CmdFindBranch.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindBranch.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindBranch.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindBranch.Location = New System.Drawing.Point(501, 118)
        Me.CmdFindBranch.Name = "CmdFindBranch"
        Me.CmdFindBranch.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindBranch.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindBranch.TabIndex = 992
        '
        'ChkStaut
        '
        Me.ChkStaut.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ChkStaut.BorderRadius = 0
        Me.ChkStaut.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChkStaut.ForeColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ChkStaut.Location = New System.Drawing.Point(636, 25)
        Me.ChkStaut.Margin = New System.Windows.Forms.Padding(3, 5, 3, 5)
        Me.ChkStaut.Name = "ChkStaut"
        Me.ChkStaut.Oncolor = System.Drawing.Color.Chartreuse
        Me.ChkStaut.Onoffcolor = System.Drawing.Color.Red
        Me.ChkStaut.Size = New System.Drawing.Size(51, 19)
        Me.ChkStaut.TabIndex = 991
        Me.ChkStaut.Textcolor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ChkStaut.Value = True
        '
        'TxtCode
        '
        Me.TxtCode.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtCode.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCode.Font = New System.Drawing.Font("Calibri", 15.75!, System.Drawing.FontStyle.Bold)
        Me.TxtCode.ForeColor = System.Drawing.Color.Black
        Me.TxtCode.Location = New System.Drawing.Point(768, 22)
        Me.TxtCode.Name = "TxtCode"
        Me.TxtCode.ReadOnly = True
        Me.TxtCode.Size = New System.Drawing.Size(81, 26)
        Me.TxtCode.TabIndex = 990
        Me.TxtCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(896, 22)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(64, 19)
        Me.Label5.TabIndex = 989
        Me.Label5.Text = "رقم السند"
        '
        'CmdFind
        '
        Me.CmdFind.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFind.ImageOptions.SvgImage = CType(resources.GetObject("CmdFind.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFind.Location = New System.Drawing.Point(726, 20)
        Me.CmdFind.Name = "CmdFind"
        Me.CmdFind.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFind.Size = New System.Drawing.Size(36, 26)
        Me.CmdFind.TabIndex = 988
        '
        'TxtBranchName
        '
        Me.TxtBranchName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBranchName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBranchName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtBranchName.ForeColor = System.Drawing.Color.Black
        Me.TxtBranchName.Location = New System.Drawing.Point(543, 117)
        Me.TxtBranchName.Name = "TxtBranchName"
        Me.TxtBranchName.ReadOnly = True
        Me.TxtBranchName.Size = New System.Drawing.Size(306, 24)
        Me.TxtBranchName.TabIndex = 987
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(923, 120)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(37, 19)
        Me.Label1.TabIndex = 986
        Me.Label1.Text = "الفرع"
        '
        'TxtRef
        '
        Me.TxtRef.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtRef.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtRef.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtRef.ForeColor = System.Drawing.Color.Red
        Me.TxtRef.Location = New System.Drawing.Point(712, 87)
        Me.TxtRef.Name = "TxtRef"
        Me.TxtRef.Size = New System.Drawing.Size(137, 24)
        Me.TxtRef.TabIndex = 1058
        Me.TxtRef.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(913, 87)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(47, 19)
        Me.Label6.TabIndex = 1057
        Me.Label6.Text = "المرجع"
        '
        'TxtFromCashBoxGuid
        '
        Me.TxtFromCashBoxGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtFromCashBoxGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtFromCashBoxGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtFromCashBoxGuid.Location = New System.Drawing.Point(647, 149)
        Me.TxtFromCashBoxGuid.Name = "TxtFromCashBoxGuid"
        Me.TxtFromCashBoxGuid.Size = New System.Drawing.Size(40, 22)
        Me.TxtFromCashBoxGuid.TabIndex = 1062
        Me.TxtFromCashBoxGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtFromCashBoxGuid.Visible = False
        '
        'TxtFromCashBoxName
        '
        Me.TxtFromCashBoxName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtFromCashBoxName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtFromCashBoxName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtFromCashBoxName.ForeColor = System.Drawing.Color.Black
        Me.TxtFromCashBoxName.Location = New System.Drawing.Point(543, 150)
        Me.TxtFromCashBoxName.Name = "TxtFromCashBoxName"
        Me.TxtFromCashBoxName.ReadOnly = True
        Me.TxtFromCashBoxName.Size = New System.Drawing.Size(306, 24)
        Me.TxtFromCashBoxName.TabIndex = 1061
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(855, 153)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(111, 19)
        Me.Label7.TabIndex = 1060
        Me.Label7.Text = " من الخزينة/البنك"
        '
        'CmdFindFromCashBox
        '
        Me.CmdFindFromCashBox.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindFromCashBox.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindAccount.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindFromCashBox.Location = New System.Drawing.Point(501, 150)
        Me.CmdFindFromCashBox.Name = "CmdFindFromCashBox"
        Me.CmdFindFromCashBox.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindFromCashBox.Size = New System.Drawing.Size(36, 24)
        Me.CmdFindFromCashBox.TabIndex = 1059
        '
        'TxtToCashBoxGuid
        '
        Me.TxtToCashBoxGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtToCashBoxGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtToCashBoxGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtToCashBoxGuid.Location = New System.Drawing.Point(647, 179)
        Me.TxtToCashBoxGuid.Name = "TxtToCashBoxGuid"
        Me.TxtToCashBoxGuid.Size = New System.Drawing.Size(40, 22)
        Me.TxtToCashBoxGuid.TabIndex = 1066
        Me.TxtToCashBoxGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtToCashBoxGuid.Visible = False
        '
        'TxtToCashBoxName
        '
        Me.TxtToCashBoxName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtToCashBoxName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtToCashBoxName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtToCashBoxName.ForeColor = System.Drawing.Color.Black
        Me.TxtToCashBoxName.Location = New System.Drawing.Point(543, 180)
        Me.TxtToCashBoxName.Name = "TxtToCashBoxName"
        Me.TxtToCashBoxName.ReadOnly = True
        Me.TxtToCashBoxName.Size = New System.Drawing.Size(306, 24)
        Me.TxtToCashBoxName.TabIndex = 1065
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.BackColor = System.Drawing.Color.Transparent
        Me.Label10.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label10.ForeColor = System.Drawing.Color.Black
        Me.Label10.Location = New System.Drawing.Point(855, 183)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(109, 19)
        Me.Label10.TabIndex = 1064
        Me.Label10.Text = " إلي الخزينة/البنك"
        '
        'CmdFindToCashBox
        '
        Me.CmdFindToCashBox.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindToCashBox.ImageOptions.SvgImage = CType(resources.GetObject("SimpleButton1.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindToCashBox.Location = New System.Drawing.Point(501, 180)
        Me.CmdFindToCashBox.Name = "CmdFindToCashBox"
        Me.CmdFindToCashBox.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindToCashBox.Size = New System.Drawing.Size(36, 24)
        Me.CmdFindToCashBox.TabIndex = 1063
        '
        'TxtCostCenterGuid
        '
        Me.TxtCostCenterGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtCostCenterGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCostCenterGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtCostCenterGuid.Location = New System.Drawing.Point(742, 211)
        Me.TxtCostCenterGuid.Name = "TxtCostCenterGuid"
        Me.TxtCostCenterGuid.Size = New System.Drawing.Size(43, 22)
        Me.TxtCostCenterGuid.TabIndex = 1070
        Me.TxtCostCenterGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtCostCenterGuid.Visible = False
        '
        'CmdFindCostCenter
        '
        Me.CmdFindCostCenter.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindCostCenter.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindCostCenter.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindCostCenter.Location = New System.Drawing.Point(501, 210)
        Me.CmdFindCostCenter.Name = "CmdFindCostCenter"
        Me.CmdFindCostCenter.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindCostCenter.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindCostCenter.TabIndex = 1069
        '
        'TxtCostCenterName
        '
        Me.TxtCostCenterName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtCostCenterName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCostCenterName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCostCenterName.ForeColor = System.Drawing.Color.Black
        Me.TxtCostCenterName.Location = New System.Drawing.Point(543, 210)
        Me.TxtCostCenterName.Name = "TxtCostCenterName"
        Me.TxtCostCenterName.ReadOnly = True
        Me.TxtCostCenterName.Size = New System.Drawing.Size(306, 24)
        Me.TxtCostCenterName.TabIndex = 1068
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(884, 213)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(76, 19)
        Me.Label4.TabIndex = 1067
        Me.Label4.Text = "مركز التكلفة"
        '
        'TxtOpenDailyType
        '
        Me.TxtOpenDailyType.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtOpenDailyType.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtOpenDailyType.ForeColor = System.Drawing.Color.Black
        Me.TxtOpenDailyType.Location = New System.Drawing.Point(127, 20)
        Me.TxtOpenDailyType.Name = "TxtOpenDailyType"
        Me.TxtOpenDailyType.Size = New System.Drawing.Size(91, 22)
        Me.TxtOpenDailyType.TabIndex = 1045
        Me.TxtOpenDailyType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtOpenDailyType.Visible = False
        '
        'EncodingTransMoney
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(992, 566)
        Me.Controls.Add(Me.GroupPanel1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EncodingTransMoney"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "تحويل نقدية"
        Me.GroupPanel1.ResumeLayout(False)
        Me.GroupPanel2.ResumeLayout(False)
        Me.GroupPanel3.ResumeLayout(False)
        Me.GroupPanel5.ResumeLayout(False)
        Me.GroupPanel5.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtDay1Code As Label
    Friend WithEvents TxtDay1Guid As Label
    Friend WithEvents CmdClose As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtGuid As Label
    Friend WithEvents TxtTypeGuid As Label
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CMDNew As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents GroupPanel3 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel5 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtRef As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents TxtAmount As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents TxtNotes As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents TxtBranchGuid As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents TxtDayDate As DateTimePicker
    Friend WithEvents CmdFindBranch As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents ChkStaut As Bunifu.Framework.UI.BunifuSwitch
    Friend WithEvents TxtCode As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents CmdFind As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtBranchName As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents TxtToCashBoxGuid As Label
    Friend WithEvents TxtToCashBoxName As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents CmdFindToCashBox As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtFromCashBoxGuid As Label
    Friend WithEvents TxtFromCashBoxName As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents CmdFindFromCashBox As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtCostCenterGuid As Label
    Friend WithEvents CmdFindCostCenter As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtCostCenterName As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents TxtOpenDailyType As Label
End Class
