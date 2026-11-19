<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PaySelect
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PaySelect))
        Me.GroupPanel9 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtPayType = New System.Windows.Forms.Label()
        Me.CmdPay4 = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdPay3 = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdPay2 = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdPay1 = New DevExpress.XtraEditors.SimpleButton()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupPanel9.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel9
        '
        Me.GroupPanel9.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel9.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel9.Controls.Add(Me.TxtPayType)
        Me.GroupPanel9.Controls.Add(Me.CmdPay4)
        Me.GroupPanel9.Controls.Add(Me.CmdPay3)
        Me.GroupPanel9.Controls.Add(Me.CmdPay2)
        Me.GroupPanel9.Controls.Add(Me.CmdPay1)
        Me.GroupPanel9.Controls.Add(Me.Label1)
        Me.GroupPanel9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel9.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel9.Name = "GroupPanel9"
        Me.GroupPanel9.Size = New System.Drawing.Size(270, 254)
        '
        '
        '
        Me.GroupPanel9.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel9.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel9.Style.BackColorGradientAngle = 90
        Me.GroupPanel9.Style.BorderBottomWidth = 1
        Me.GroupPanel9.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel9.Style.BorderLeftWidth = 1
        Me.GroupPanel9.Style.BorderRightWidth = 1
        Me.GroupPanel9.Style.BorderTopWidth = 1
        Me.GroupPanel9.Style.CornerDiameter = 4
        Me.GroupPanel9.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel9.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel9.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel9.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel9.TabIndex = 877
        '
        'TxtPayType
        '
        Me.TxtPayType.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtPayType.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtPayType.ForeColor = System.Drawing.Color.Black
        Me.TxtPayType.Location = New System.Drawing.Point(218, 6)
        Me.TxtPayType.Name = "TxtPayType"
        Me.TxtPayType.Size = New System.Drawing.Size(40, 31)
        Me.TxtPayType.TabIndex = 996
        Me.TxtPayType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CmdPay4
        '
        Me.CmdPay4.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdPay4.Appearance.Options.UseFont = True
        Me.CmdPay4.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdPay4.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdPay4.ImageOptions.SvgImage = CType(resources.GetObject("CmdPay4.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdPay4.Location = New System.Drawing.Point(49, 196)
        Me.CmdPay4.Name = "CmdPay4"
        Me.CmdPay4.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdPay4.Size = New System.Drawing.Size(161, 36)
        Me.CmdPay4.TabIndex = 995
        Me.CmdPay4.Text = "سداد رسوم الاختبار    "
        '
        'CmdPay3
        '
        Me.CmdPay3.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdPay3.Appearance.Options.UseFont = True
        Me.CmdPay3.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdPay3.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdPay3.ImageOptions.SvgImage = CType(resources.GetObject("CmdPay3.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdPay3.Location = New System.Drawing.Point(69, 154)
        Me.CmdPay3.Name = "CmdPay3"
        Me.CmdPay3.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdPay3.Size = New System.Drawing.Size(141, 36)
        Me.CmdPay3.TabIndex = 994
        Me.CmdPay3.Text = "سداد كامل المبلغ   "
        '
        'CmdPay2
        '
        Me.CmdPay2.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdPay2.Appearance.Options.UseFont = True
        Me.CmdPay2.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdPay2.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdPay2.ImageOptions.SvgImage = CType(resources.GetObject("CmdPay2.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdPay2.Location = New System.Drawing.Point(69, 112)
        Me.CmdPay2.Name = "CmdPay2"
        Me.CmdPay2.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdPay2.Size = New System.Drawing.Size(141, 36)
        Me.CmdPay2.TabIndex = 993
        Me.CmdPay2.Text = "سداد قسط شهري"
        '
        'CmdPay1
        '
        Me.CmdPay1.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdPay1.Appearance.Options.UseFont = True
        Me.CmdPay1.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdPay1.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdPay1.ImageOptions.SvgImage = CType(resources.GetObject("CmdPay1.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdPay1.Location = New System.Drawing.Point(58, 70)
        Me.CmdPay1.Name = "CmdPay1"
        Me.CmdPay1.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdPay1.Size = New System.Drawing.Size(161, 36)
        Me.CmdPay1.TabIndex = 992
        Me.CmdPay1.Text = "سداد دفعه مقدمة   "
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(49, 37)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(174, 19)
        Me.Label1.TabIndex = 991
        Me.Label1.Text = "برجاء اختيار نوع عملية السداد"
        '
        'PaySelect
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(270, 254)
        Me.Controls.Add(Me.GroupPanel9)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "PaySelect"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "اختيار نوع عملية السداد"
        Me.GroupPanel9.ResumeLayout(False)
        Me.GroupPanel9.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel9 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents CmdPay4 As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdPay3 As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdPay2 As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdPay1 As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtPayType As Label
End Class
