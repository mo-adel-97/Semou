<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PrintExam
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PrintExam))
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtCode = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.CmdFindExam = New DevExpress.XtraEditors.SimpleButton()
        Me.ChkAll = New System.Windows.Forms.RadioButton()
        Me.ChkOne = New System.Windows.Forms.RadioButton()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.SimpleButton1 = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdOK = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtExamGuid = New System.Windows.Forms.Label()
        Me.TxtDiplomGuid = New System.Windows.Forms.Label()
        Me.TxtLevelGuid = New System.Windows.Forms.Label()
        Me.GroupPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.TxtExamGuid)
        Me.GroupPanel1.Controls.Add(Me.TxtDiplomGuid)
        Me.GroupPanel1.Controls.Add(Me.TxtLevelGuid)
        Me.GroupPanel1.Controls.Add(Me.CmdOK)
        Me.GroupPanel1.Controls.Add(Me.SimpleButton1)
        Me.GroupPanel1.Controls.Add(Me.TextBox1)
        Me.GroupPanel1.Controls.Add(Me.ChkOne)
        Me.GroupPanel1.Controls.Add(Me.ChkAll)
        Me.GroupPanel1.Controls.Add(Me.TxtCode)
        Me.GroupPanel1.Controls.Add(Me.Label10)
        Me.GroupPanel1.Controls.Add(Me.CmdFindExam)
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(490, 190)
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
        'TxtCode
        '
        Me.TxtCode.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtCode.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCode.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCode.ForeColor = System.Drawing.Color.Black
        Me.TxtCode.Location = New System.Drawing.Point(105, 35)
        Me.TxtCode.Name = "TxtCode"
        Me.TxtCode.ReadOnly = True
        Me.TxtCode.Size = New System.Drawing.Size(315, 24)
        Me.TxtCode.TabIndex = 1107
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.BackColor = System.Drawing.Color.Transparent
        Me.Label10.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label10.ForeColor = System.Drawing.Color.Black
        Me.Label10.Location = New System.Drawing.Point(426, 38)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(51, 19)
        Me.Label10.TabIndex = 1108
        Me.Label10.Text = "الأختبار"
        '
        'CmdFindExam
        '
        Me.CmdFindExam.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFindExam.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindExam.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFindExam.Location = New System.Drawing.Point(63, 35)
        Me.CmdFindExam.Name = "CmdFindExam"
        Me.CmdFindExam.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFindExam.Size = New System.Drawing.Size(36, 26)
        Me.CmdFindExam.TabIndex = 1109
        '
        'ChkAll
        '
        Me.ChkAll.AutoSize = True
        Me.ChkAll.BackColor = System.Drawing.Color.Transparent
        Me.ChkAll.Checked = True
        Me.ChkAll.Location = New System.Drawing.Point(315, 75)
        Me.ChkAll.Name = "ChkAll"
        Me.ChkAll.Size = New System.Drawing.Size(158, 23)
        Me.ChkAll.TabIndex = 1110
        Me.ChkAll.TabStop = True
        Me.ChkAll.Text = "جميع الطلاب بالمستوي"
        Me.ChkAll.UseVisualStyleBackColor = False
        '
        'ChkOne
        '
        Me.ChkOne.AutoSize = True
        Me.ChkOne.BackColor = System.Drawing.Color.Transparent
        Me.ChkOne.Location = New System.Drawing.Point(377, 104)
        Me.ChkOne.Name = "ChkOne"
        Me.ChkOne.Size = New System.Drawing.Size(96, 23)
        Me.ChkOne.TabIndex = 1111
        Me.ChkOne.Text = "طالب محدد"
        Me.ChkOne.UseVisualStyleBackColor = False
        '
        'TextBox1
        '
        Me.TextBox1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox1.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox1.ForeColor = System.Drawing.Color.Black
        Me.TextBox1.Location = New System.Drawing.Point(56, 104)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.Size = New System.Drawing.Size(315, 24)
        Me.TextBox1.TabIndex = 1112
        Me.TextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'SimpleButton1
        '
        Me.SimpleButton1.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.SimpleButton1.ImageOptions.SvgImage = CType(resources.GetObject("SimpleButton1.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.SimpleButton1.Location = New System.Drawing.Point(14, 101)
        Me.SimpleButton1.Name = "SimpleButton1"
        Me.SimpleButton1.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.SimpleButton1.Size = New System.Drawing.Size(36, 26)
        Me.SimpleButton1.TabIndex = 1113
        '
        'CmdOK
        '
        Me.CmdOK.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdOK.Appearance.Options.UseFont = True
        Me.CmdOK.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdOK.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdOK.ImageOptions.SvgImage = CType(resources.GetObject("SimpleButton2.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdOK.Location = New System.Drawing.Point(179, 138)
        Me.CmdOK.Name = "CmdOK"
        Me.CmdOK.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdOK.Size = New System.Drawing.Size(126, 40)
        Me.CmdOK.TabIndex = 1114
        Me.CmdOK.Text = "تأكيد"
        '
        'TxtExamGuid
        '
        Me.TxtExamGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtExamGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtExamGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtExamGuid.Location = New System.Drawing.Point(179, 9)
        Me.TxtExamGuid.Name = "TxtExamGuid"
        Me.TxtExamGuid.Size = New System.Drawing.Size(40, 22)
        Me.TxtExamGuid.TabIndex = 1117
        Me.TxtExamGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtExamGuid.Visible = False
        '
        'TxtDiplomGuid
        '
        Me.TxtDiplomGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtDiplomGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtDiplomGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtDiplomGuid.Location = New System.Drawing.Point(271, 11)
        Me.TxtDiplomGuid.Name = "TxtDiplomGuid"
        Me.TxtDiplomGuid.Size = New System.Drawing.Size(40, 21)
        Me.TxtDiplomGuid.TabIndex = 1115
        Me.TxtDiplomGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtDiplomGuid.Visible = False
        '
        'TxtLevelGuid
        '
        Me.TxtLevelGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtLevelGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtLevelGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtLevelGuid.Location = New System.Drawing.Point(225, 9)
        Me.TxtLevelGuid.Name = "TxtLevelGuid"
        Me.TxtLevelGuid.Size = New System.Drawing.Size(40, 22)
        Me.TxtLevelGuid.TabIndex = 1116
        Me.TxtLevelGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtLevelGuid.Visible = False
        '
        'PrintExam
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(490, 190)
        Me.Controls.Add(Me.GroupPanel1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "PrintExam"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "طباعة أختبار مستوي"
        Me.GroupPanel1.ResumeLayout(False)
        Me.GroupPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CmdOK As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents SimpleButton1 As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents ChkOne As RadioButton
    Friend WithEvents ChkAll As RadioButton
    Friend WithEvents TxtCode As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents CmdFindExam As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtExamGuid As Label
    Friend WithEvents TxtDiplomGuid As Label
    Friend WithEvents TxtLevelGuid As Label
End Class
