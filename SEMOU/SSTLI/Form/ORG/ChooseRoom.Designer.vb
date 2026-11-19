<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ChooseRoom
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ChooseRoom))
        Me.GroupPanel3 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.CboRoom = New System.Windows.Forms.ComboBox()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.CmdOk = New DevExpress.XtraEditors.SimpleButton()
        Me.GroupPanel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel3
        '
        Me.GroupPanel3.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel3.Controls.Add(Me.CmdOk)
        Me.GroupPanel3.Controls.Add(Me.CboRoom)
        Me.GroupPanel3.Controls.Add(Me.Label30)
        Me.GroupPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.GroupPanel3.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel3.Name = "GroupPanel3"
        Me.GroupPanel3.Size = New System.Drawing.Size(340, 126)
        '
        '
        '
        Me.GroupPanel3.Style.BackColor = System.Drawing.Color.White
        Me.GroupPanel3.Style.BackColor2 = System.Drawing.Color.White
        Me.GroupPanel3.Style.BackColorGradientAngle = 90
        Me.GroupPanel3.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel3.Style.BorderBottomWidth = 1
        Me.GroupPanel3.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel3.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel3.Style.BorderLeftWidth = 1
        Me.GroupPanel3.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel3.Style.BorderRightWidth = 1
        Me.GroupPanel3.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel3.Style.BorderTopWidth = 1
        Me.GroupPanel3.Style.CornerDiameter = 4
        Me.GroupPanel3.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel3.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel3.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel3.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        Me.GroupPanel3.TabIndex = 614
        '
        'CboRoom
        '
        Me.CboRoom.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.CboRoom.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.CboRoom.BackColor = System.Drawing.Color.WhiteSmoke
        Me.CboRoom.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.CboRoom.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold)
        Me.CboRoom.ForeColor = System.Drawing.Color.Black
        Me.CboRoom.FormattingEnabled = True
        Me.CboRoom.Items.AddRange(New Object() {"قاعه رقم 1", "قاعه رقم 2", "قاعة رقم 3", "قاعة رقم 4", "قاعة رقم 5", "قاعه رقم 6", "قاعة رقم 7", "قاعة رقم 8", "قاعة رقم 9", "قاعة رقم 10", "معمل رقم 1", "معمل رقم 2", "معمل رقم 3"})
        Me.CboRoom.Location = New System.Drawing.Point(74, 22)
        Me.CboRoom.Name = "CboRoom"
        Me.CboRoom.Size = New System.Drawing.Size(137, 31)
        Me.CboRoom.TabIndex = 1112
        '
        'Label30
        '
        Me.Label30.AutoSize = True
        Me.Label30.BackColor = System.Drawing.Color.Transparent
        Me.Label30.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label30.ForeColor = System.Drawing.Color.Black
        Me.Label30.Location = New System.Drawing.Point(232, 28)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(43, 19)
        Me.Label30.TabIndex = 1111
        Me.Label30.Text = "القاعة"
        '
        'CmdOk
        '
        Me.CmdOk.Appearance.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold)
        Me.CmdOk.Appearance.Options.UseFont = True
        Me.CmdOk.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.RightCenter
        Me.CmdOk.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.CmdOk.ImageOptions.SvgImage = CType(resources.GetObject("CmdFindDiplom.ImageOptions.SvgImage"), DevExpress.Utils.Svg.SvgImage)
        Me.CmdOk.Location = New System.Drawing.Point(80, 66)
        Me.CmdOk.Name = "CmdOk"
        Me.CmdOk.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light
        Me.CmdOk.Size = New System.Drawing.Size(146, 45)
        Me.CmdOk.TabIndex = 1113
        Me.CmdOk.Text = "تأكيد"
        '
        'ChooseRoom
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(340, 126)
        Me.Controls.Add(Me.GroupPanel3)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ChooseRoom"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "اختيار القاعه"
        Me.GroupPanel3.ResumeLayout(False)
        Me.GroupPanel3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupPanel3 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents CboRoom As ComboBox
    Friend WithEvents Label30 As Label
    Friend WithEvents CmdOk As DevExpress.XtraEditors.SimpleButton
End Class
