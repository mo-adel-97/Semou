<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UpdateStudentLevel
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UpdateStudentLevel))
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtLevelGuid = New System.Windows.Forms.Label()
        Me.CmdFindLevel = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtLevelName = New System.Windows.Forms.TextBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdAccept = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtBatchGuid = New System.Windows.Forms.Label()
        Me.TxtDiplomGuid = New System.Windows.Forms.Label()
        Me.TxtBranchGuid = New System.Windows.Forms.Label()
        Me.TxtAccountGuid = New System.Windows.Forms.Label()
        Me.GroupPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel2
        '
        Me.GroupPanel2.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel2.Controls.Add(Me.TxtAccountGuid)
        Me.GroupPanel2.Controls.Add(Me.TxtBranchGuid)
        Me.GroupPanel2.Controls.Add(Me.TxtDiplomGuid)
        Me.GroupPanel2.Controls.Add(Me.TxtBatchGuid)
        Me.GroupPanel2.Controls.Add(Me.CmdExit)
        Me.GroupPanel2.Controls.Add(Me.CmdAccept)
        Me.GroupPanel2.Controls.Add(Me.TxtLevelGuid)
        Me.GroupPanel2.Controls.Add(Me.CmdFindLevel)
        Me.GroupPanel2.Controls.Add(Me.TxtLevelName)
        Me.GroupPanel2.Controls.Add(Me.Label21)
        Me.GroupPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel2.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel2.Name = "GroupPanel2"
        Me.GroupPanel2.Size = New System.Drawing.Size(446, 169)
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
        'TxtLevelGuid
        '
        Me.TxtLevelGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtLevelGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtLevelGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtLevelGuid.Location = New System.Drawing.Point(115, 31)
        Me.TxtLevelGuid.Name = "TxtLevelGuid"
        Me.TxtLevelGuid.Size = New System.Drawing.Size(40, 27)
        Me.TxtLevelGuid.TabIndex = 1062
        Me.TxtLevelGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtLevelGuid.Visible = False
        '
        'CmdFindLevel
        '
        Me.CmdFindLevel.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindLevel.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindLevel.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindLevel.Location = New System.Drawing.Point(18, 30)
        Me.CmdFindLevel.Name = "CmdFindLevel"
        Me.CmdFindLevel.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindLevel.Size = New System.Drawing.Size(32, 25)
        Me.CmdFindLevel.TabIndex = 1061
        '
        'TxtLevelName
        '
        Me.TxtLevelName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtLevelName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtLevelName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtLevelName.Location = New System.Drawing.Point(56, 31)
        Me.TxtLevelName.Name = "TxtLevelName"
        Me.TxtLevelName.ReadOnly = True
        Me.TxtLevelName.Size = New System.Drawing.Size(275, 24)
        Me.TxtLevelName.TabIndex = 1060
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.BackColor = System.Drawing.Color.Transparent
        Me.Label21.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label21.ForeColor = System.Drawing.Color.Black
        Me.Label21.Location = New System.Drawing.Point(337, 34)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(99, 19)
        Me.Label21.TabIndex = 1059
        Me.Label21.Text = "المستوي الجديد"
        '
        'CmdExit
        '
        Me.CmdExit.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdExit.Appearance.Options.UseFont = True
        Me.CmdExit.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdExit.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdExit.ImageOptions.SvgImage = CType(resources.GetObject("CmdExit.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdExit.Location = New System.Drawing.Point(64, 84)
        Me.CmdExit.Name = "CmdExit"
        Me.CmdExit.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdExit.Size = New System.Drawing.Size(152, 51)
        Me.CmdExit.TabIndex = 1064
        Me.CmdExit.Text = "إلغاء"
        '
        'CmdAccept
        '
        Me.CmdAccept.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdAccept.Appearance.Options.UseFont = True
        Me.CmdAccept.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdAccept.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdAccept.ImageOptions.SvgImage = CType(resources.GetObject("CmdAccept.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdAccept.Location = New System.Drawing.Point(222, 84)
        Me.CmdAccept.Name = "CmdAccept"
        Me.CmdAccept.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdAccept.Size = New System.Drawing.Size(152, 51)
        Me.CmdAccept.TabIndex = 1063
        Me.CmdAccept.Text = "تأكيد"
        '
        'TxtBatchGuid
        '
        Me.TxtBatchGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBatchGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBatchGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtBatchGuid.Location = New System.Drawing.Point(78, 1)
        Me.TxtBatchGuid.Name = "TxtBatchGuid"
        Me.TxtBatchGuid.Size = New System.Drawing.Size(40, 27)
        Me.TxtBatchGuid.TabIndex = 1065
        Me.TxtBatchGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBatchGuid.Visible = False
        '
        'TxtDiplomGuid
        '
        Me.TxtDiplomGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtDiplomGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDiplomGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtDiplomGuid.Location = New System.Drawing.Point(124, 0)
        Me.TxtDiplomGuid.Name = "TxtDiplomGuid"
        Me.TxtDiplomGuid.Size = New System.Drawing.Size(40, 27)
        Me.TxtDiplomGuid.TabIndex = 1066
        Me.TxtDiplomGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtDiplomGuid.Visible = False
        '
        'TxtBranchGuid
        '
        Me.TxtBranchGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtBranchGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBranchGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtBranchGuid.Location = New System.Drawing.Point(170, 0)
        Me.TxtBranchGuid.Name = "TxtBranchGuid"
        Me.TxtBranchGuid.Size = New System.Drawing.Size(40, 27)
        Me.TxtBranchGuid.TabIndex = 1067
        Me.TxtBranchGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtBranchGuid.Visible = False
        '
        'TxtAccountGuid
        '
        Me.TxtAccountGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtAccountGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtAccountGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtAccountGuid.Location = New System.Drawing.Point(216, 1)
        Me.TxtAccountGuid.Name = "TxtAccountGuid"
        Me.TxtAccountGuid.Size = New System.Drawing.Size(40, 27)
        Me.TxtAccountGuid.TabIndex = 1068
        Me.TxtAccountGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtAccountGuid.Visible = False
        '
        'UpdateStudentLevel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(446, 169)
        Me.Controls.Add(Me.GroupPanel2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UpdateStudentLevel"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ترحيل الطالب لمستوي جديد"
        Me.GroupPanel2.ResumeLayout(False)
        Me.GroupPanel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtLevelGuid As Label
    Friend WithEvents CmdFindLevel As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtLevelName As TextBox
    Friend WithEvents Label21 As Label
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdAccept As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtAccountGuid As Label
    Friend WithEvents TxtBranchGuid As Label
    Friend WithEvents TxtDiplomGuid As Label
    Friend WithEvents TxtBatchGuid As Label
End Class
