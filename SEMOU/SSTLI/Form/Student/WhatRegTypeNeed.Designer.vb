<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class WhatRegTypeNeed
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(WhatRegTypeNeed))
        Me.GroupPanel7 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.CmdFindDepart = New DevExpress.XtraEditors.SimpleButton()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtAccountGuid = New System.Windows.Forms.Label()
        Me.TxtDocCode = New System.Windows.Forms.Label()
        Me.TxtDocGuid = New System.Windows.Forms.Label()
        Me.TxtDocName = New System.Windows.Forms.TextBox()
        Me.TxtTypeGuid = New System.Windows.Forms.Label()
        Me.TxtBranchGuid = New System.Windows.Forms.Label()
        Me.CmdClose = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdOk = New DevExpress.XtraEditors.SimpleButton()
        Me.CboType = New System.Windows.Forms.ComboBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.TxtBranchName = New System.Windows.Forms.TextBox()
        Me.ChkContract = New System.Windows.Forms.CheckBox()
        Me.GroupPanel7.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel7
        '
        Me.GroupPanel7.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel7.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel7.Controls.Add(Me.ChkContract)
        Me.GroupPanel7.Controls.Add(Me.Label1)
        Me.GroupPanel7.Controls.Add(Me.TextBox1)
        Me.GroupPanel7.Controls.Add(Me.CmdFindDepart)
        Me.GroupPanel7.Controls.Add(Me.Label4)
        Me.GroupPanel7.Controls.Add(Me.TxtAccountGuid)
        Me.GroupPanel7.Controls.Add(Me.TxtDocCode)
        Me.GroupPanel7.Controls.Add(Me.TxtDocGuid)
        Me.GroupPanel7.Controls.Add(Me.TxtDocName)
        Me.GroupPanel7.Controls.Add(Me.TxtTypeGuid)
        Me.GroupPanel7.Controls.Add(Me.TxtBranchGuid)
        Me.GroupPanel7.Controls.Add(Me.CmdClose)
        Me.GroupPanel7.Controls.Add(Me.CmdOk)
        Me.GroupPanel7.Controls.Add(Me.CboType)
        Me.GroupPanel7.Controls.Add(Me.Label10)
        Me.GroupPanel7.Controls.Add(Me.TxtBranchName)
        Me.GroupPanel7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel7.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel7.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel7.Name = "GroupPanel7"
        Me.GroupPanel7.Size = New System.Drawing.Size(567, 683)
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
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Red
        Me.Label1.Location = New System.Drawing.Point(457, 95)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(96, 19)
        Me.Label1.TabIndex = 1018
        Me.Label1.Text = "شروط التسجيل"
        '
        'TextBox1
        '
        Me.TextBox1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox1.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox1.ForeColor = System.Drawing.Color.Black
        Me.TextBox1.Location = New System.Drawing.Point(15, 117)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox1.Size = New System.Drawing.Size(540, 480)
        Me.TextBox1.TabIndex = 1017
        Me.TextBox1.Text = resources.GetString("TextBox1.Text")
        '
        'CmdFindDepart
        '
        Me.CmdFindDepart.Enabled = False
        Me.CmdFindDepart.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindDepart.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindDepart.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindDepart.Location = New System.Drawing.Point(45, 17)
        Me.CmdFindDepart.Name = "CmdFindDepart"
        Me.CmdFindDepart.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindDepart.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindDepart.TabIndex = 1016
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(516, 22)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(37, 19)
        Me.Label4.TabIndex = 1014
        Me.Label4.Text = "الفرع"
        '
        'TxtAccountGuid
        '
        Me.TxtAccountGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtAccountGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtAccountGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtAccountGuid.Location = New System.Drawing.Point(153, 55)
        Me.TxtAccountGuid.Name = "TxtAccountGuid"
        Me.TxtAccountGuid.Size = New System.Drawing.Size(36, 31)
        Me.TxtAccountGuid.TabIndex = 1013
        Me.TxtAccountGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtAccountGuid.Visible = False
        '
        'TxtDocCode
        '
        Me.TxtDocCode.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtDocCode.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDocCode.ForeColor = System.Drawing.Color.Black
        Me.TxtDocCode.Location = New System.Drawing.Point(61, 54)
        Me.TxtDocCode.Name = "TxtDocCode"
        Me.TxtDocCode.Size = New System.Drawing.Size(40, 31)
        Me.TxtDocCode.TabIndex = 1012
        Me.TxtDocCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtDocCode.Visible = False
        '
        'TxtDocGuid
        '
        Me.TxtDocGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtDocGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDocGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtDocGuid.Location = New System.Drawing.Point(107, 54)
        Me.TxtDocGuid.Name = "TxtDocGuid"
        Me.TxtDocGuid.Size = New System.Drawing.Size(40, 31)
        Me.TxtDocGuid.TabIndex = 1011
        Me.TxtDocGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtDocGuid.Visible = False
        '
        'TxtDocName
        '
        Me.TxtDocName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtDocName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtDocName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtDocName.Location = New System.Drawing.Point(195, 57)
        Me.TxtDocName.Name = "TxtDocName"
        Me.TxtDocName.ReadOnly = True
        Me.TxtDocName.Size = New System.Drawing.Size(52, 24)
        Me.TxtDocName.TabIndex = 1010
        Me.TxtDocName.Visible = False
        '
        'TxtTypeGuid
        '
        Me.TxtTypeGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtTypeGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtTypeGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtTypeGuid.Location = New System.Drawing.Point(12, 58)
        Me.TxtTypeGuid.Name = "TxtTypeGuid"
        Me.TxtTypeGuid.Size = New System.Drawing.Size(43, 22)
        Me.TxtTypeGuid.TabIndex = 1009
        Me.TxtTypeGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtTypeGuid.Visible = False
        '
        'TxtBranchGuid
        '
        Me.TxtBranchGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBranchGuid.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBranchGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtBranchGuid.Location = New System.Drawing.Point(211, 21)
        Me.TxtBranchGuid.Name = "TxtBranchGuid"
        Me.TxtBranchGuid.Size = New System.Drawing.Size(49, 22)
        Me.TxtBranchGuid.TabIndex = 1008
        Me.TxtBranchGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBranchGuid.Visible = False
        '
        'CmdClose
        '
        Me.CmdClose.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdClose.Appearance.Options.UseFont = True
        Me.CmdClose.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter
        Me.CmdClose.ImageOptions.SvgImage = CType(resources.GetObject("CmdClose.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdClose.Location = New System.Drawing.Point(348, 603)
        Me.CmdClose.Name = "CmdClose"
        Me.CmdClose.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdClose.Size = New System.Drawing.Size(98, 68)
        Me.CmdClose.TabIndex = 1007
        Me.CmdClose.Text = "إلغاء"
        '
        'CmdOk
        '
        Me.CmdOk.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdOk.Appearance.Options.UseFont = True
        Me.CmdOk.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter
        Me.CmdOk.ImageOptions.SvgImage = CType(resources.GetObject("CmdOk.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdOk.Location = New System.Drawing.Point(452, 603)
        Me.CmdOk.Name = "CmdOk"
        Me.CmdOk.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdOk.Size = New System.Drawing.Size(98, 68)
        Me.CmdOk.TabIndex = 1006
        Me.CmdOk.Text = "تأكيد"
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
        Me.CboType.Items.AddRange(New Object() {"دبلوم", "دورة"})
        Me.CboType.Location = New System.Drawing.Point(291, 49)
        Me.CboType.Name = "CboType"
        Me.CboType.Size = New System.Drawing.Size(168, 31)
        Me.CboType.TabIndex = 1005
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.BackColor = System.Drawing.Color.Transparent
        Me.Label10.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label10.ForeColor = System.Drawing.Color.Black
        Me.Label10.Location = New System.Drawing.Point(474, 55)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(79, 19)
        Me.Label10.TabIndex = 1004
        Me.Label10.Text = "نوع التسجيل"
        '
        'TxtBranchName
        '
        Me.TxtBranchName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBranchName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBranchName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBranchName.ForeColor = System.Drawing.Color.Black
        Me.TxtBranchName.Location = New System.Drawing.Point(87, 19)
        Me.TxtBranchName.Name = "TxtBranchName"
        Me.TxtBranchName.ReadOnly = True
        Me.TxtBranchName.Size = New System.Drawing.Size(372, 24)
        Me.TxtBranchName.TabIndex = 1015
        '
        'ChkContract
        '
        Me.ChkContract.AutoSize = True
        Me.ChkContract.Location = New System.Drawing.Point(116, 614)
        Me.ChkContract.Name = "ChkContract"
        Me.ChkContract.Size = New System.Drawing.Size(112, 23)
        Me.ChkContract.TabIndex = 1019
        Me.ChkContract.Text = "ChkContract"
        Me.ChkContract.UseVisualStyleBackColor = True
        Me.ChkContract.Visible = False
        '
        'WhatRegTypeNeed
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(567, 683)
        Me.ControlBox = False
        Me.Controls.Add(Me.GroupPanel7)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "WhatRegTypeNeed"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "تحديد نوع التسجيل"
        Me.GroupPanel7.ResumeLayout(False)
        Me.GroupPanel7.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel7 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CboType As ComboBox
    Friend WithEvents Label10 As Label
    Friend WithEvents CmdClose As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdOk As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtBranchGuid As Label
    Friend WithEvents TxtTypeGuid As Label
    Friend WithEvents TxtDocCode As Label
    Friend WithEvents TxtDocGuid As Label
    Friend WithEvents TxtDocName As TextBox
    Friend WithEvents TxtAccountGuid As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents TxtBranchName As TextBox
    Friend WithEvents CmdFindDepart As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents ChkContract As CheckBox
End Class
