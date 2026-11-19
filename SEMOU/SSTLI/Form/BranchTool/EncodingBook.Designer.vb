<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class EncodingBook
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EncodingBook))
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.GroupPanel3 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.CboType = New System.Windows.Forms.ComboBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.TxtBookCode = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ChkStaut = New Bunifu.Framework.UI.BunifuSwitch()
        Me.CmdFind = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtBookName = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtCode = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtTypeGuid = New System.Windows.Forms.Label()
        Me.TxtGuid = New System.Windows.Forms.Label()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdSave = New DevExpress.XtraEditors.SimpleButton()
        Me.CMDNew = New DevExpress.XtraEditors.SimpleButton()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.CboBookType = New System.Windows.Forms.ComboBox()
        Me.GroupPanel1.SuspendLayout()
        Me.GroupPanel3.SuspendLayout()
        Me.GroupPanel2.SuspendLayout()
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
        Me.GroupPanel1.Size = New System.Drawing.Size(706, 276)
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
        Me.GroupPanel1.TabIndex = 872
        '
        'GroupPanel3
        '
        Me.GroupPanel3.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel3.Controls.Add(Me.Label11)
        Me.GroupPanel3.Controls.Add(Me.CboBookType)
        Me.GroupPanel3.Controls.Add(Me.CboType)
        Me.GroupPanel3.Controls.Add(Me.Label15)
        Me.GroupPanel3.Controls.Add(Me.TxtBookCode)
        Me.GroupPanel3.Controls.Add(Me.Label1)
        Me.GroupPanel3.Controls.Add(Me.ChkStaut)
        Me.GroupPanel3.Controls.Add(Me.CmdFind)
        Me.GroupPanel3.Controls.Add(Me.TxtBookName)
        Me.GroupPanel3.Controls.Add(Me.Label2)
        Me.GroupPanel3.Controls.Add(Me.TxtCode)
        Me.GroupPanel3.Controls.Add(Me.Label5)
        Me.GroupPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel3.Location = New System.Drawing.Point(0, 72)
        Me.GroupPanel3.Name = "GroupPanel3"
        Me.GroupPanel3.Size = New System.Drawing.Size(706, 204)
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
        'CboType
        '
        Me.CboType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.CboType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.CboType.BackColor = System.Drawing.Color.WhiteSmoke
        Me.CboType.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.CboType.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.CboType.ForeColor = System.Drawing.Color.Black
        Me.CboType.FormattingEnabled = True
        Me.CboType.Items.AddRange(New Object() {"نظري", "عملي", "نظري - عملي"})
        Me.CboType.Location = New System.Drawing.Point(375, 158)
        Me.CboType.Name = "CboType"
        Me.CboType.Size = New System.Drawing.Size(215, 31)
        Me.CboType.TabIndex = 1044
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.BackColor = System.Drawing.Color.Transparent
        Me.Label15.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label15.ForeColor = System.Drawing.Color.Black
        Me.Label15.Location = New System.Drawing.Point(608, 164)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(83, 19)
        Me.Label15.TabIndex = 1043
        Me.Label15.Text = "طبيعة المقرر"
        '
        'TxtBookCode
        '
        Me.TxtBookCode.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBookCode.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBookCode.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBookCode.ForeColor = System.Drawing.Color.Black
        Me.TxtBookCode.Location = New System.Drawing.Point(396, 91)
        Me.TxtBookCode.Name = "TxtBookCode"
        Me.TxtBookCode.Size = New System.Drawing.Size(194, 24)
        Me.TxtBookCode.TabIndex = 996
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(624, 90)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(67, 19)
        Me.Label1.TabIndex = 995
        Me.Label1.Text = "رمز المقرر"
        '
        'ChkStaut
        '
        Me.ChkStaut.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ChkStaut.BorderRadius = 0
        Me.ChkStaut.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChkStaut.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ChkStaut.ForeColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ChkStaut.Location = New System.Drawing.Point(415, 28)
        Me.ChkStaut.Margin = New System.Windows.Forms.Padding(3, 5, 3, 5)
        Me.ChkStaut.Name = "ChkStaut"
        Me.ChkStaut.Oncolor = System.Drawing.Color.Chartreuse
        Me.ChkStaut.Onoffcolor = System.Drawing.Color.Red
        Me.ChkStaut.Size = New System.Drawing.Size(51, 19)
        Me.ChkStaut.TabIndex = 994
        Me.ChkStaut.Textcolor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ChkStaut.Value = True
        '
        'CmdFind
        '
        Me.CmdFind.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdFind.ImageOptions.SvgImage = CType(resources.GetObject("CmdFind.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdFind.Location = New System.Drawing.Point(65, 60)
        Me.CmdFind.Name = "CmdFind"
        Me.CmdFind.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdFind.Size = New System.Drawing.Size(36, 26)
        Me.CmdFind.TabIndex = 993
        '
        'TxtBookName
        '
        Me.TxtBookName.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtBookName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtBookName.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtBookName.ForeColor = System.Drawing.Color.Black
        Me.TxtBookName.Location = New System.Drawing.Point(107, 60)
        Me.TxtBookName.Name = "TxtBookName"
        Me.TxtBookName.Size = New System.Drawing.Size(483, 24)
        Me.TxtBookName.TabIndex = 992
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(620, 62)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(71, 19)
        Me.Label2.TabIndex = 991
        Me.Label2.Text = "اسم المقرر"
        '
        'TxtCode
        '
        Me.TxtCode.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtCode.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCode.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCode.ForeColor = System.Drawing.Color.Black
        Me.TxtCode.Location = New System.Drawing.Point(503, 28)
        Me.TxtCode.Name = "TxtCode"
        Me.TxtCode.ReadOnly = True
        Me.TxtCode.Size = New System.Drawing.Size(87, 24)
        Me.TxtCode.TabIndex = 990
        Me.TxtCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(621, 30)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(70, 19)
        Me.Label5.TabIndex = 989
        Me.Label5.Text = "كود المقرر"
        '
        'GroupPanel2
        '
        Me.GroupPanel2.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel2.Controls.Add(Me.TxtTypeGuid)
        Me.GroupPanel2.Controls.Add(Me.TxtGuid)
        Me.GroupPanel2.Controls.Add(Me.CmdExit)
        Me.GroupPanel2.Controls.Add(Me.CmdSave)
        Me.GroupPanel2.Controls.Add(Me.CMDNew)
        Me.GroupPanel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel2.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel2.Name = "GroupPanel2"
        Me.GroupPanel2.Size = New System.Drawing.Size(706, 72)
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
        Me.CmdExit.Location = New System.Drawing.Point(463, 0)
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
        Me.CmdSave.Location = New System.Drawing.Point(544, 0)
        Me.CmdSave.Name = "CmdSave"
        Me.CmdSave.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdSave.Size = New System.Drawing.Size(81, 72)
        Me.CmdSave.TabIndex = 323
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
        Me.CMDNew.Location = New System.Drawing.Point(625, 0)
        Me.CMDNew.Name = "CMDNew"
        Me.CMDNew.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CMDNew.Size = New System.Drawing.Size(81, 72)
        Me.CMDNew.TabIndex = 324
        Me.CMDNew.Text = "جديد F1"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.BackColor = System.Drawing.Color.White
        Me.Label11.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label11.ForeColor = System.Drawing.Color.Black
        Me.Label11.Location = New System.Drawing.Point(623, 127)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(66, 19)
        Me.Label11.TabIndex = 1046
        Me.Label11.Text = "نوع المقرر"
        '
        'CboBookType
        '
        Me.CboBookType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.CboBookType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.CboBookType.BackColor = System.Drawing.Color.WhiteSmoke
        Me.CboBookType.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.CboBookType.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.CboBookType.ForeColor = System.Drawing.Color.Black
        Me.CboBookType.FormattingEnabled = True
        Me.CboBookType.Items.AddRange(New Object() {"مواد تخصص", "مواد عامة", "مواد عامة تخصصية"})
        Me.CboBookType.Location = New System.Drawing.Point(375, 121)
        Me.CboBookType.Name = "CboBookType"
        Me.CboBookType.Size = New System.Drawing.Size(215, 31)
        Me.CboBookType.TabIndex = 1045
        '
        'EncodingBook
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(706, 276)
        Me.Controls.Add(Me.GroupPanel1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EncodingBook"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "المقررات الدراسية"
        Me.GroupPanel1.ResumeLayout(False)
        Me.GroupPanel3.ResumeLayout(False)
        Me.GroupPanel3.PerformLayout()
        Me.GroupPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel3 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CMDNew As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents ChkStaut As Bunifu.Framework.UI.BunifuSwitch
    Friend WithEvents CmdFind As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtBookName As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TxtCode As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents TxtGuid As Label
    Friend WithEvents TxtTypeGuid As Label
    Friend WithEvents TxtBookCode As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents CboType As ComboBox
    Friend WithEvents Label15 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents CboBookType As ComboBox
End Class
