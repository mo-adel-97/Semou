<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TransAllDiplomStudent
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TransAllDiplomStudent))
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel4 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.CmdSave = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.ChkUpdate5 = New System.Windows.Forms.CheckBox()
        Me.TxtOldBatchGuid = New System.Windows.Forms.Label()
        Me.CmdFindBatch = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtBatchName = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.ChkUpdate4 = New System.Windows.Forms.CheckBox()
        Me.TxtOldDiplomGuid = New System.Windows.Forms.Label()
        Me.CmdFindDiplom = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtDiplomName = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ChkUpdate2 = New System.Windows.Forms.CheckBox()
        Me.ChkUpdate1 = New System.Windows.Forms.CheckBox()
        Me.TxtBranch2Guid = New System.Windows.Forms.Label()
        Me.CmdFindBranch2 = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtBranch2Name = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtBranch1Guid = New System.Windows.Forms.Label()
        Me.CmdFindBranch1 = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtBranch1Name = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.GroupPanel1.SuspendLayout()
        Me.GroupPanel4.SuspendLayout()
        Me.GroupPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.GroupPanel4)
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(639, 83)
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
        Me.GroupPanel1.TabIndex = 1035
        '
        'GroupPanel4
        '
        Me.GroupPanel4.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel4.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel4.Controls.Add(Me.CmdSave)
        Me.GroupPanel4.Controls.Add(Me.CmdExit)
        Me.GroupPanel4.Dock = System.Windows.Forms.DockStyle.Right
        Me.GroupPanel4.Location = New System.Drawing.Point(481, 0)
        Me.GroupPanel4.Name = "GroupPanel4"
        Me.GroupPanel4.Size = New System.Drawing.Size(158, 83)
        '
        '
        '
        Me.GroupPanel4.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel4.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel4.Style.BackColorGradientAngle = 90
        Me.GroupPanel4.Style.BorderBottomWidth = 1
        Me.GroupPanel4.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel4.Style.BorderLeftWidth = 1
        Me.GroupPanel4.Style.BorderRightWidth = 1
        Me.GroupPanel4.Style.BorderTopWidth = 1
        Me.GroupPanel4.Style.CornerDiameter = 4
        Me.GroupPanel4.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel4.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel4.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel4.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel4.TabIndex = 1034
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
        Me.CmdSave.Location = New System.Drawing.Point(85, 12)
        Me.CmdSave.Name = "CmdSave"
        Me.CmdSave.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdSave.Size = New System.Drawing.Size(64, 65)
        Me.CmdSave.TabIndex = 315
        Me.CmdSave.Text = "تأكيد"
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
        Me.CmdExit.Location = New System.Drawing.Point(15, 12)
        Me.CmdExit.Name = "CmdExit"
        Me.CmdExit.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdExit.Size = New System.Drawing.Size(64, 65)
        Me.CmdExit.TabIndex = 313
        Me.CmdExit.Text = "خروج"
        '
        'GroupPanel2
        '
        Me.GroupPanel2.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel2.Controls.Add(Me.ChkUpdate5)
        Me.GroupPanel2.Controls.Add(Me.TxtOldBatchGuid)
        Me.GroupPanel2.Controls.Add(Me.CmdFindBatch)
        Me.GroupPanel2.Controls.Add(Me.TxtBatchName)
        Me.GroupPanel2.Controls.Add(Me.Label7)
        Me.GroupPanel2.Controls.Add(Me.ChkUpdate4)
        Me.GroupPanel2.Controls.Add(Me.TxtOldDiplomGuid)
        Me.GroupPanel2.Controls.Add(Me.CmdFindDiplom)
        Me.GroupPanel2.Controls.Add(Me.TxtDiplomName)
        Me.GroupPanel2.Controls.Add(Me.Label5)
        Me.GroupPanel2.Controls.Add(Me.ChkUpdate2)
        Me.GroupPanel2.Controls.Add(Me.ChkUpdate1)
        Me.GroupPanel2.Controls.Add(Me.TxtBranch2Guid)
        Me.GroupPanel2.Controls.Add(Me.CmdFindBranch2)
        Me.GroupPanel2.Controls.Add(Me.TxtBranch2Name)
        Me.GroupPanel2.Controls.Add(Me.Label2)
        Me.GroupPanel2.Controls.Add(Me.TxtBranch1Guid)
        Me.GroupPanel2.Controls.Add(Me.CmdFindBranch1)
        Me.GroupPanel2.Controls.Add(Me.TxtBranch1Name)
        Me.GroupPanel2.Controls.Add(Me.Label4)
        Me.GroupPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel2.Location = New System.Drawing.Point(0, 83)
        Me.GroupPanel2.Name = "GroupPanel2"
        Me.GroupPanel2.Size = New System.Drawing.Size(639, 144)
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
        Me.GroupPanel2.TabIndex = 1036
        '
        'ChkUpdate5
        '
        Me.ChkUpdate5.AutoSize = True
        Me.ChkUpdate5.Location = New System.Drawing.Point(614, 106)
        Me.ChkUpdate5.Name = "ChkUpdate5"
        Me.ChkUpdate5.Size = New System.Drawing.Size(15, 14)
        Me.ChkUpdate5.TabIndex = 1103
        Me.ChkUpdate5.UseVisualStyleBackColor = True
        '
        'TxtOldBatchGuid
        '
        Me.TxtOldBatchGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtOldBatchGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtOldBatchGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtOldBatchGuid.Location = New System.Drawing.Point(348, 103)
        Me.TxtOldBatchGuid.Name = "TxtOldBatchGuid"
        Me.TxtOldBatchGuid.Size = New System.Drawing.Size(40, 21)
        Me.TxtOldBatchGuid.TabIndex = 1102
        Me.TxtOldBatchGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtOldBatchGuid.Visible = False
        '
        'CmdFindBatch
        '
        Me.CmdFindBatch.Enabled = False
        Me.CmdFindBatch.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindBatch.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindBatch.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindBatch.Location = New System.Drawing.Point(258, 103)
        Me.CmdFindBatch.Name = "CmdFindBatch"
        Me.CmdFindBatch.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindBatch.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindBatch.TabIndex = 1101
        '
        'TxtBatchName
        '
        Me.TxtBatchName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBatchName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBatchName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtBatchName.ForeColor = System.Drawing.Color.Black
        Me.TxtBatchName.Location = New System.Drawing.Point(300, 102)
        Me.TxtBatchName.Name = "TxtBatchName"
        Me.TxtBatchName.ReadOnly = True
        Me.TxtBatchName.Size = New System.Drawing.Size(201, 24)
        Me.TxtBatchName.TabIndex = 1100
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(563, 103)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(45, 19)
        Me.Label7.TabIndex = 1099
        Me.Label7.Text = "الدفعة"
        '
        'ChkUpdate4
        '
        Me.ChkUpdate4.AutoSize = True
        Me.ChkUpdate4.Location = New System.Drawing.Point(614, 76)
        Me.ChkUpdate4.Name = "ChkUpdate4"
        Me.ChkUpdate4.Size = New System.Drawing.Size(15, 14)
        Me.ChkUpdate4.TabIndex = 1098
        Me.ChkUpdate4.UseVisualStyleBackColor = True
        '
        'TxtOldDiplomGuid
        '
        Me.TxtOldDiplomGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtOldDiplomGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtOldDiplomGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtOldDiplomGuid.Location = New System.Drawing.Point(348, 73)
        Me.TxtOldDiplomGuid.Name = "TxtOldDiplomGuid"
        Me.TxtOldDiplomGuid.Size = New System.Drawing.Size(40, 21)
        Me.TxtOldDiplomGuid.TabIndex = 1097
        Me.TxtOldDiplomGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtOldDiplomGuid.Visible = False
        '
        'CmdFindDiplom
        '
        Me.CmdFindDiplom.Enabled = False
        Me.CmdFindDiplom.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindDiplom.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindDiplom.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindDiplom.Location = New System.Drawing.Point(98, 73)
        Me.CmdFindDiplom.Name = "CmdFindDiplom"
        Me.CmdFindDiplom.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindDiplom.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindDiplom.TabIndex = 1096
        '
        'TxtDiplomName
        '
        Me.TxtDiplomName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtDiplomName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtDiplomName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtDiplomName.ForeColor = System.Drawing.Color.Black
        Me.TxtDiplomName.Location = New System.Drawing.Point(140, 72)
        Me.TxtDiplomName.Name = "TxtDiplomName"
        Me.TxtDiplomName.ReadOnly = True
        Me.TxtDiplomName.Size = New System.Drawing.Size(361, 24)
        Me.TxtDiplomName.TabIndex = 1095
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(562, 73)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(46, 19)
        Me.Label5.TabIndex = 1094
        Me.Label5.Text = "الدبلوم"
        '
        'ChkUpdate2
        '
        Me.ChkUpdate2.AutoSize = True
        Me.ChkUpdate2.Location = New System.Drawing.Point(614, 47)
        Me.ChkUpdate2.Name = "ChkUpdate2"
        Me.ChkUpdate2.Size = New System.Drawing.Size(15, 14)
        Me.ChkUpdate2.TabIndex = 1093
        Me.ChkUpdate2.UseVisualStyleBackColor = True
        '
        'ChkUpdate1
        '
        Me.ChkUpdate1.AutoSize = True
        Me.ChkUpdate1.Location = New System.Drawing.Point(614, 18)
        Me.ChkUpdate1.Name = "ChkUpdate1"
        Me.ChkUpdate1.Size = New System.Drawing.Size(15, 14)
        Me.ChkUpdate1.TabIndex = 1092
        Me.ChkUpdate1.UseVisualStyleBackColor = True
        '
        'TxtBranch2Guid
        '
        Me.TxtBranch2Guid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBranch2Guid.Font = New System.Drawing.Font("justagain DIN", 14.25!)
        Me.TxtBranch2Guid.ForeColor = System.Drawing.Color.Black
        Me.TxtBranch2Guid.Location = New System.Drawing.Point(98, 44)
        Me.TxtBranch2Guid.Name = "TxtBranch2Guid"
        Me.TxtBranch2Guid.Size = New System.Drawing.Size(54, 19)
        Me.TxtBranch2Guid.TabIndex = 1091
        Me.TxtBranch2Guid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBranch2Guid.Visible = False
        '
        'CmdFindBranch2
        '
        Me.CmdFindBranch2.Enabled = False
        Me.CmdFindBranch2.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindBranch2.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindBranch2.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindBranch2.Location = New System.Drawing.Point(28, 42)
        Me.CmdFindBranch2.Name = "CmdFindBranch2"
        Me.CmdFindBranch2.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindBranch2.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindBranch2.TabIndex = 1090
        '
        'TxtBranch2Name
        '
        Me.TxtBranch2Name.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBranch2Name.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBranch2Name.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBranch2Name.ForeColor = System.Drawing.Color.Black
        Me.TxtBranch2Name.Location = New System.Drawing.Point(70, 41)
        Me.TxtBranch2Name.Name = "TxtBranch2Name"
        Me.TxtBranch2Name.ReadOnly = True
        Me.TxtBranch2Name.Size = New System.Drawing.Size(431, 24)
        Me.TxtBranch2Name.TabIndex = 1089
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(533, 44)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(75, 19)
        Me.Label2.TabIndex = 1088
        Me.Label2.Text = "فرع الدراسة"
        '
        'TxtBranch1Guid
        '
        Me.TxtBranch1Guid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBranch1Guid.Font = New System.Drawing.Font("justagain DIN", 14.25!)
        Me.TxtBranch1Guid.ForeColor = System.Drawing.Color.Black
        Me.TxtBranch1Guid.Location = New System.Drawing.Point(98, 14)
        Me.TxtBranch1Guid.Name = "TxtBranch1Guid"
        Me.TxtBranch1Guid.Size = New System.Drawing.Size(54, 19)
        Me.TxtBranch1Guid.TabIndex = 1087
        Me.TxtBranch1Guid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBranch1Guid.Visible = False
        '
        'CmdFindBranch1
        '
        Me.CmdFindBranch1.Enabled = False
        Me.CmdFindBranch1.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindBranch1.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindBranch1.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindBranch1.Location = New System.Drawing.Point(28, 12)
        Me.CmdFindBranch1.Name = "CmdFindBranch1"
        Me.CmdFindBranch1.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindBranch1.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindBranch1.TabIndex = 1086
        '
        'TxtBranch1Name
        '
        Me.TxtBranch1Name.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBranch1Name.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBranch1Name.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBranch1Name.ForeColor = System.Drawing.Color.Black
        Me.TxtBranch1Name.Location = New System.Drawing.Point(70, 11)
        Me.TxtBranch1Name.Name = "TxtBranch1Name"
        Me.TxtBranch1Name.ReadOnly = True
        Me.TxtBranch1Name.Size = New System.Drawing.Size(431, 24)
        Me.TxtBranch1Name.TabIndex = 1085
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(527, 14)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(81, 19)
        Me.Label4.TabIndex = 1084
        Me.Label4.Text = "فرع التسجيل"
        '
        'TransAllDiplomStudent
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(639, 227)
        Me.Controls.Add(Me.GroupPanel2)
        Me.Controls.Add(Me.GroupPanel1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "TransAllDiplomStudent"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "نقل كل الطلاب"
        Me.GroupPanel1.ResumeLayout(False)
        Me.GroupPanel4.ResumeLayout(False)
        Me.GroupPanel2.ResumeLayout(False)
        Me.GroupPanel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel4 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CmdSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents ChkUpdate5 As CheckBox
    Friend WithEvents TxtOldBatchGuid As Label
    Friend WithEvents CmdFindBatch As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtBatchName As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents ChkUpdate4 As CheckBox
    Friend WithEvents TxtOldDiplomGuid As Label
    Friend WithEvents CmdFindDiplom As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtDiplomName As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents ChkUpdate2 As CheckBox
    Friend WithEvents ChkUpdate1 As CheckBox
    Friend WithEvents TxtBranch2Guid As Label
    Friend WithEvents CmdFindBranch2 As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtBranch2Name As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TxtBranch1Guid As Label
    Friend WithEvents CmdFindBranch1 As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtBranch1Name As TextBox
    Friend WithEvents Label4 As Label
End Class
