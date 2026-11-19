<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ChangeUserPassword
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ChangeUserPassword))
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtCurrentPassword = New System.Windows.Forms.Label()
        Me.CmdShowNewPassword = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdShowOldPassWord = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdClose = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdoK = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtNewPassword = New System.Windows.Forms.TextBox()
        Me.TxtOldPassWord = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.Label2)
        Me.GroupPanel1.Controls.Add(Me.TxtCurrentPassword)
        Me.GroupPanel1.Controls.Add(Me.CmdShowNewPassword)
        Me.GroupPanel1.Controls.Add(Me.CmdShowOldPassWord)
        Me.GroupPanel1.Controls.Add(Me.CmdClose)
        Me.GroupPanel1.Controls.Add(Me.CmdoK)
        Me.GroupPanel1.Controls.Add(Me.TxtNewPassword)
        Me.GroupPanel1.Controls.Add(Me.TxtOldPassWord)
        Me.GroupPanel1.Controls.Add(Me.Label1)
        Me.GroupPanel1.Controls.Add(Me.Label5)
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel1.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(426, 177)
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
        Me.GroupPanel1.TabIndex = 759
        '
        'TxtCurrentPassword
        '
        Me.TxtCurrentPassword.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtCurrentPassword.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCurrentPassword.ForeColor = System.Drawing.Color.Black
        Me.TxtCurrentPassword.Location = New System.Drawing.Point(327, 108)
        Me.TxtCurrentPassword.Name = "TxtCurrentPassword"
        Me.TxtCurrentPassword.Size = New System.Drawing.Size(59, 31)
        Me.TxtCurrentPassword.TabIndex = 877
        Me.TxtCurrentPassword.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtCurrentPassword.Visible = False
        '
        'CmdShowNewPassword
        '
        Me.CmdShowNewPassword.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdShowNewPassword.ImageOptions.SvgImage = CType(resources.GetObject("CmdShowNewPassword.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdShowNewPassword.Location = New System.Drawing.Point(27, 72)
        Me.CmdShowNewPassword.Name = "CmdShowNewPassword"
        Me.CmdShowNewPassword.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdShowNewPassword.Size = New System.Drawing.Size(42, 27)
        Me.CmdShowNewPassword.TabIndex = 764
        '
        'CmdShowOldPassWord
        '
        Me.CmdShowOldPassWord.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdShowOldPassWord.ImageOptions.SvgImage = CType(resources.GetObject("CmdShowOldPassWord.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdShowOldPassWord.Location = New System.Drawing.Point(27, 37)
        Me.CmdShowOldPassWord.Name = "CmdShowOldPassWord"
        Me.CmdShowOldPassWord.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdShowOldPassWord.Size = New System.Drawing.Size(42, 27)
        Me.CmdShowOldPassWord.TabIndex = 763
        '
        'CmdClose
        '
        Me.CmdClose.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdClose.Appearance.ForeColor = System.Drawing.Color.Black
        Me.CmdClose.Appearance.Options.UseFont = True
        Me.CmdClose.Appearance.Options.UseForeColor = True
        Me.CmdClose.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdClose.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdClose.ImageOptions.SvgImage = CType(resources.GetObject("CmdClose.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdClose.Location = New System.Drawing.Point(103, 121)
        Me.CmdClose.Name = "CmdClose"
        Me.CmdClose.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdClose.Size = New System.Drawing.Size(97, 42)
        Me.CmdClose.TabIndex = 762
        Me.CmdClose.Text = "خروج"
        '
        'CmdoK
        '
        Me.CmdoK.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdoK.Appearance.ForeColor = System.Drawing.Color.Black
        Me.CmdoK.Appearance.Options.UseFont = True
        Me.CmdoK.Appearance.Options.UseForeColor = True
        Me.CmdoK.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdoK.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdoK.ImageOptions.SvgImage = CType(resources.GetObject("CmdoK.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdoK.Location = New System.Drawing.Point(206, 121)
        Me.CmdoK.Name = "CmdoK"
        Me.CmdoK.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdoK.Size = New System.Drawing.Size(97, 42)
        Me.CmdoK.TabIndex = 761
        Me.CmdoK.Text = "تأكيد"
        '
        'TxtNewPassword
        '
        Me.TxtNewPassword.BackColor = System.Drawing.Color.GhostWhite
        Me.TxtNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtNewPassword.Enabled = False
        Me.TxtNewPassword.Font = New System.Drawing.Font("justagain DIN", 14.25!)
        Me.TxtNewPassword.Location = New System.Drawing.Point(75, 72)
        Me.TxtNewPassword.Name = "TxtNewPassword"
        Me.TxtNewPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TxtNewPassword.Size = New System.Drawing.Size(221, 28)
        Me.TxtNewPassword.TabIndex = 523
        Me.TxtNewPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TxtOldPassWord
        '
        Me.TxtOldPassWord.BackColor = System.Drawing.Color.GhostWhite
        Me.TxtOldPassWord.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtOldPassWord.Font = New System.Drawing.Font("justagain DIN", 14.25!)
        Me.TxtOldPassWord.Location = New System.Drawing.Point(75, 38)
        Me.TxtOldPassWord.Name = "TxtOldPassWord"
        Me.TxtOldPassWord.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TxtOldPassWord.Size = New System.Drawing.Size(221, 28)
        Me.TxtOldPassWord.TabIndex = 522
        Me.TxtOldPassWord.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(304, 76)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(111, 18)
        Me.Label1.TabIndex = 521
        Me.Label1.Text = "كلمة المرور الجديدة"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(310, 41)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(105, 18)
        Me.Label5.TabIndex = 520
        Me.Label5.Text = "كلمة المرور الحالية"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Red
        Me.Label2.Location = New System.Drawing.Point(23, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(378, 14)
        Me.Label2.TabIndex = 878
        Me.Label2.Text = "لتتمكن من تغيير كلمة المرور برجاء ادخال كلمة المرور الحالية والضغط على مفتاح ENTE" &
    "R"
        '
        'ChangeUserPassword
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(426, 177)
        Me.ControlBox = False
        Me.Controls.Add(Me.GroupPanel1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ChangeUserPassword"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "تغيير كلمة المرور"
        Me.GroupPanel1.ResumeLayout(False)
        Me.GroupPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtCurrentPassword As Label
    Friend WithEvents CmdShowNewPassword As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdShowOldPassWord As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdClose As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdoK As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtNewPassword As TextBox
    Friend WithEvents TxtOldPassWord As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label2 As Label
End Class
