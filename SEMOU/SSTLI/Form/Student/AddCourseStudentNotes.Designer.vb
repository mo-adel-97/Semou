<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AddCourseStudentNotes
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AddCourseStudentNotes))
        Me.GroupPanel2 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.TxtAccountGuid = New System.Windows.Forms.Label()
        Me.TxtBranchGuid = New System.Windows.Forms.Label()
        Me.TxtCourseGuid = New System.Windows.Forms.Label()
        Me.CmdExit = New DevExpress.XtraEditors.SimpleButton()
        Me.CmdAccept = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtNotes = New System.Windows.Forms.TextBox()
        Me.GroupPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel2
        '
        Me.GroupPanel2.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel2.Controls.Add(Me.TxtAccountGuid)
        Me.GroupPanel2.Controls.Add(Me.TxtBranchGuid)
        Me.GroupPanel2.Controls.Add(Me.TxtCourseGuid)
        Me.GroupPanel2.Controls.Add(Me.CmdExit)
        Me.GroupPanel2.Controls.Add(Me.CmdAccept)
        Me.GroupPanel2.Controls.Add(Me.TxtNotes)
        Me.GroupPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel2.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel2.Name = "GroupPanel2"
        Me.GroupPanel2.Size = New System.Drawing.Size(492, 232)
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
        'TxtCourseGuid
        '
        Me.TxtCourseGuid.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TxtCourseGuid.Font = New System.Drawing.Font("justagain DIN", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCourseGuid.ForeColor = System.Drawing.Color.Black
        Me.TxtCourseGuid.Location = New System.Drawing.Point(78, 1)
        Me.TxtCourseGuid.Name = "TxtCourseGuid"
        Me.TxtCourseGuid.Size = New System.Drawing.Size(40, 27)
        Me.TxtCourseGuid.TabIndex = 1065
        Me.TxtCourseGuid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TxtCourseGuid.Visible = False
        '
        'CmdExit
        '
        Me.CmdExit.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdExit.Appearance.Options.UseFont = True
        Me.CmdExit.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdExit.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdExit.ImageOptions.SvgImage = CType(resources.GetObject("CmdExit.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdExit.Location = New System.Drawing.Point(86, 161)
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
        Me.CmdAccept.Location = New System.Drawing.Point(244, 161)
        Me.CmdAccept.Name = "CmdAccept"
        Me.CmdAccept.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdAccept.Size = New System.Drawing.Size(152, 51)
        Me.CmdAccept.TabIndex = 1063
        Me.CmdAccept.Text = "تأكيد"
        '
        'TxtNotes
        '
        Me.TxtNotes.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TxtNotes.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtNotes.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.TxtNotes.Location = New System.Drawing.Point(12, 31)
        Me.TxtNotes.Multiline = True
        Me.TxtNotes.Name = "TxtNotes"
        Me.TxtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TxtNotes.Size = New System.Drawing.Size(468, 122)
        Me.TxtNotes.TabIndex = 1060
        '
        'AddCourseStudentNotes
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(492, 232)
        Me.Controls.Add(Me.GroupPanel2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AddCourseStudentNotes"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "اضافة ملاحظة للطالب"
        Me.GroupPanel2.ResumeLayout(False)
        Me.GroupPanel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel2 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents TxtAccountGuid As Label
    Friend WithEvents TxtBranchGuid As Label
    Friend WithEvents TxtCourseGuid As Label
    Friend WithEvents CmdExit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmdAccept As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtNotes As TextBox
End Class
