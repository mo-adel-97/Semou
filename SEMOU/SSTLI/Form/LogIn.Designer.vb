<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LogIn
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LogIn))
        Me.CompanyLogo = New System.Windows.Forms.PictureBox()
        Me.SimpleButton3 = New DevExpress.XtraEditors.SimpleButton()
        Me.SimpleButton2 = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdOk = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtPassword = New System.Windows.Forms.TextBox()
        Me.TxtUserName = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtCopyRight = New System.Windows.Forms.Label()
        Me.TxtGuid = New System.Windows.Forms.Label()
        Me.TxtSystemVirsion = New System.Windows.Forms.TextBox()
        Me.TxtUpdateUrl = New System.Windows.Forms.TextBox()
        Me.CmdUpdate = New DevExpress.XtraEditors.SimpleButton()
        CType(Me.CompanyLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CompanyLogo
        '
        Me.CompanyLogo.BackColor = System.Drawing.Color.Transparent
        Me.CompanyLogo.Image = Global.SEMOU.My.Resources.Resources.semou
        Me.CompanyLogo.Location = New System.Drawing.Point(294, 51)
        Me.CompanyLogo.Name = "CompanyLogo"
        Me.CompanyLogo.Size = New System.Drawing.Size(153, 139)
        Me.CompanyLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.CompanyLogo.TabIndex = 746
        Me.CompanyLogo.TabStop = False
        '
        'SimpleButton3
        '
        Me.SimpleButton3.Appearance.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SimpleButton3.Appearance.Options.UseFont = True
        Me.SimpleButton3.Enabled = False
        Me.SimpleButton3.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter
        Me.SimpleButton3.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.SimpleButton3.ImageOptions.SvgImage = CType(resources.GetObject("SimpleButton3.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.SimpleButton3.Location = New System.Drawing.Point(489, 240)
        Me.SimpleButton3.Name = "SimpleButton3"
        Me.SimpleButton3.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.SimpleButton3.Size = New System.Drawing.Size(34, 36)
        Me.SimpleButton3.TabIndex = 841
        '
        'SimpleButton2
        '
        Me.SimpleButton2.Appearance.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SimpleButton2.Appearance.Options.UseFont = True
        Me.SimpleButton2.Enabled = False
        Me.SimpleButton2.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter
        Me.SimpleButton2.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.SimpleButton2.ImageOptions.SvgImage = CType(resources.GetObject("SimpleButton2.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.SimpleButton2.Location = New System.Drawing.Point(489, 196)
        Me.SimpleButton2.Name = "SimpleButton2"
        Me.SimpleButton2.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.SimpleButton2.Size = New System.Drawing.Size(34, 36)
        Me.SimpleButton2.TabIndex = 840
        '
        'CmdExit
        '
        Me.CmdExit.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmdExit.Appearance.Options.UseFont = True
        Me.CmdExit.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdExit.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdExit.ImageOptions.SvgImage = CType(resources.GetObject("CmdExit.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdExit.Location = New System.Drawing.Point(263, 282)
        Me.CmdExit.Name = "CmdExit"
        Me.CmdExit.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdExit.Size = New System.Drawing.Size(99, 36)
        Me.CmdExit.TabIndex = 839
        Me.CmdExit.Text = "خروج"
        '
        'CmdOk
        '
        Me.CmdOk.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmdOk.Appearance.Options.UseFont = True
        Me.CmdOk.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdOk.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdOk.ImageOptions.SvgImage = CType(resources.GetObject("CmdOk.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdOk.Location = New System.Drawing.Point(368, 282)
        Me.CmdOk.Name = "CmdOk"
        Me.CmdOk.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdOk.Size = New System.Drawing.Size(99, 36)
        Me.CmdOk.TabIndex = 838
        Me.CmdOk.Text = "دخول"
        '
        'TxtPassword
        '
        Me.TxtPassword.BackColor = System.Drawing.Color.GhostWhite
        Me.TxtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtPassword.Font = New System.Drawing.Font("Calibri", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtPassword.ForeColor = System.Drawing.Color.Black
        Me.TxtPassword.Location = New System.Drawing.Point(239, 238)
        Me.TxtPassword.Name = "TxtPassword"
        Me.TxtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TxtPassword.Size = New System.Drawing.Size(244, 30)
        Me.TxtPassword.TabIndex = 837
        Me.TxtPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TxtUserName
        '
        Me.TxtUserName.BackColor = System.Drawing.Color.GhostWhite
        Me.TxtUserName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtUserName.Font = New System.Drawing.Font("Calibri", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtUserName.ForeColor = System.Drawing.Color.Black
        Me.TxtUserName.Location = New System.Drawing.Point(239, 196)
        Me.TxtUserName.Name = "TxtUserName"
        Me.TxtUserName.Size = New System.Drawing.Size(244, 30)
        Me.TxtUserName.TabIndex = 836
        Me.TxtUserName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Label2.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(140, 357)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(222, 23)
        Me.Label2.TabIndex = 844
        Me.Label2.Text = "SSTLI Management System"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtCopyRight
        '
        Me.TxtCopyRight.AutoSize = True
        Me.TxtCopyRight.BackColor = System.Drawing.Color.Transparent
        Me.TxtCopyRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.TxtCopyRight.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCopyRight.ForeColor = System.Drawing.Color.Black
        Me.TxtCopyRight.Location = New System.Drawing.Point(138, 412)
        Me.TxtCopyRight.Name = "TxtCopyRight"
        Me.TxtCopyRight.Size = New System.Drawing.Size(294, 23)
        Me.TxtCopyRight.TabIndex = 843
        Me.TxtCopyRight.Text = "Powerd By SAUDI INSTITUTE @2024"
        Me.TxtCopyRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtGuid
        '
        Me.TxtGuid.AutoSize = True
        Me.TxtGuid.BackColor = System.Drawing.Color.Transparent
        Me.TxtGuid.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.TxtGuid.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtGuid.Location = New System.Drawing.Point(141, 386)
        Me.TxtGuid.Name = "TxtGuid"
        Me.TxtGuid.Size = New System.Drawing.Size(164, 23)
        Me.TxtGuid.TabIndex = 842
        Me.TxtGuid.Text = "Version Pro 2.0.5+1"
        Me.TxtGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtSystemVirsion
        '
        Me.TxtSystemVirsion.BackColor = System.Drawing.Color.GhostWhite
        Me.TxtSystemVirsion.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtSystemVirsion.Font = New System.Drawing.Font("Calibri", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtSystemVirsion.ForeColor = System.Drawing.Color.Black
        Me.TxtSystemVirsion.Location = New System.Drawing.Point(90, 51)
        Me.TxtSystemVirsion.Name = "TxtSystemVirsion"
        Me.TxtSystemVirsion.Size = New System.Drawing.Size(132, 30)
        Me.TxtSystemVirsion.TabIndex = 845
        Me.TxtSystemVirsion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.TxtSystemVirsion.Visible = False
        '
        'TxtUpdateUrl
        '
        Me.TxtUpdateUrl.BackColor = System.Drawing.Color.GhostWhite
        Me.TxtUpdateUrl.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtUpdateUrl.Font = New System.Drawing.Font("Calibri", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtUpdateUrl.ForeColor = System.Drawing.Color.Black
        Me.TxtUpdateUrl.Location = New System.Drawing.Point(90, 87)
        Me.TxtUpdateUrl.Name = "TxtUpdateUrl"
        Me.TxtUpdateUrl.Size = New System.Drawing.Size(132, 30)
        Me.TxtUpdateUrl.TabIndex = 846
        Me.TxtUpdateUrl.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.TxtUpdateUrl.Visible = False
        '
        'CmdUpdate
        '
        Me.CmdUpdate.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CmdUpdate.Appearance.Options.UseFont = True
        Me.CmdUpdate.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdUpdate.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdUpdate.ImageOptions.SvgImage = CType(resources.GetObject("CmdUpdate.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdUpdate.Location = New System.Drawing.Point(473, 282)
        Me.CmdUpdate.Name = "CmdUpdate"
        Me.CmdUpdate.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdUpdate.Size = New System.Drawing.Size(99, 36)
        Me.CmdUpdate.TabIndex = 847
        Me.CmdUpdate.Text = "تحديث"
        Me.CmdUpdate.Visible = False
        '
        'LogIn
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.SEMOU.My.Resources.Resources.semouback
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.CmdUpdate)
        Me.Controls.Add(Me.TxtUpdateUrl)
        Me.Controls.Add(Me.TxtSystemVirsion)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TxtCopyRight)
        Me.Controls.Add(Me.TxtGuid)
        Me.Controls.Add(Me.SimpleButton3)
        Me.Controls.Add(Me.SimpleButton2)
        Me.Controls.Add(Me.CmdExit)
        Me.Controls.Add(Me.CmdOk)
        Me.Controls.Add(Me.TxtPassword)
        Me.Controls.Add(Me.TxtUserName)
        Me.Controls.Add(Me.CompanyLogo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "LogIn"
        Me.RightToLeftLayout = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        CType(Me.CompanyLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents CompanyLogo As PictureBox
    Friend WithEvents SimpleButton3 As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents SimpleButton2 As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdOk As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtPassword As TextBox
    Friend WithEvents TxtUserName As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TxtCopyRight As Label
    Friend WithEvents TxtGuid As Label
    Friend WithEvents TxtSystemVirsion As TextBox
    Friend WithEvents TxtUpdateUrl As TextBox
    Friend WithEvents CmdUpdate As DevExpress.XtraEditors.SimpleButton
End Class
