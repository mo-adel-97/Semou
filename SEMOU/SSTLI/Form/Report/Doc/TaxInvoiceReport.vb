Imports System.Drawing
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraPrinting.BarCode
Imports DevExpress.XtraPrinting.Drawing
Imports DevExpress.XtraReports.UI

''' <summary>بيانات رأس الفاتورة (من بيانات الفرع) + محتوى كود QR</summary>
Public Class TaxInvoiceHeader
    Public Property CompanyName As String
    Public Property Address As String
    Public Property VatNo As String
    Public Property CrNo As String
    Public Property Phone As String
    Public Property Notes As String
    Public Property QrText As String
End Class

''' <summary>
''' الفاتورة الضريبية المبسطة - تصميم موحد لكل الشاشات (بدل StudentInvoice).
''' الأعمدة المطلوبة في الجدول: PrintBill + TaxNo + InvDate
''' </summary>
Public Class TaxInvoiceReport
    Inherits XtraReport

    Private Const W As Single = 767.0F
    Private Const FONTNAME As String = "Segoe UI"
    Private ReadOnly BRAND As Color = Color.FromArgb(52, 61, 46)    ' لون لوجو السمو
    Private ReadOnly INK As Color = Color.FromArgb(25, 25, 25)
    Private ReadOnly MUTED As Color = Color.FromArgb(95, 95, 95)
    Private ReadOnly LINE As Color = Color.FromArgb(200, 203, 198)
    Private ReadOnly SOFT As Color = Color.FromArgb(244, 245, 242)

    Public Sub New(dt As DataTable, h As TaxInvoiceHeader)
        PaperKind = Printing.PaperKind.A4
        Margins = New Printing.Margins(30, 30, 40, 30)
        Font = New Font(FONTNAME, 10.0F)
        RightToLeft = DevExpress.XtraReports.UI.RightToLeft.Yes
        DataSource = dt

        Dim header As New ReportHeaderBand()
        Dim detail As New DetailBand()
        Dim footer As New ReportFooterBand()
        Bands.AddRange(New Band() {New TopMarginBand() With {.HeightF = 40}, header, detail, footer, New BottomMarginBand() With {.HeightF = 30}})

        '''''''''''''''''''''''''''''''''''' رأس الفاتورة: اسم المعهد والبيانات يمين - اللوجو شمال
        Dim logo As New XRPictureBox()
        logo.ImageSource = New ImageSource(My.Resources.semou)
        logo.Sizing = ImageSizeMode.ZoomImage
        logo.LocationFloat = New DevExpress.Utils.PointFloat(0, 0)
        logo.SizeF = New SizeF(140, 124)
        header.Controls.Add(logo)

        header.Controls.Add(TXT(h.CompanyName, 150, 0, W - 150, 40, 18, True, TextAlignment.MiddleRight, BRAND))
        Dim y As Single = 44
        For Each line As String In {h.Address,
                                    "الرقم الضريبي:  " & h.VatNo,
                                    "السجل التجاري:  " & h.CrNo,
                                    If(String.IsNullOrWhiteSpace(h.Phone), Nothing, "الهاتف:  " & h.Phone)}
            If line IsNot Nothing Then
                header.Controls.Add(TXT(line, 150, y, W - 150, 21, 10.5F, False, TextAlignment.MiddleRight, MUTED))
                y += 21
            End If
        Next

        Dim rule As New XRLine()
        rule.LineWidth = 2
        rule.ForeColor = BRAND
        rule.LocationFloat = New DevExpress.Utils.PointFloat(0, 134)
        rule.SizeF = New SizeF(W, 6)
        header.Controls.Add(rule)

        header.Controls.Add(TXT("فاتورة ضريبية مبسطة", 0, 146, W, 34, 17, True, TextAlignment.MiddleCenter, INK))
        header.Controls.Add(TXT("Simplified Tax Invoice", 0, 178, W, 18, 9, False, TextAlignment.MiddleCenter, MUTED))

        '''''''''''''''''''''''''''''''''''' بيانات الفاتورة والعميل - مربعين
        Dim boxW As Single = 372
        header.Controls.Add(INFOBOX(W - boxW, 208, boxW, {
            New String() {"رقم الفاتورة", "[TaxNo]", "NO"},
            New String() {"اسم العميل", "[StudentName]", "RTL"},
            New String() {"رقم الجوال", "[StudentTel]", "LTR"}}))
        header.Controls.Add(INFOBOX(0, 208, boxW, {
            New String() {"التاريخ", "[InvDate]", "LTR"},
            New String() {"رقم الهوية", "[NationalId]", "LTR"},
            New String() {"البائع", "[FullName]", "RTL"}}))

        '''''''''''''''''''''''''''''''''''' جدول البنود
        ' من الشمال لليمين: الإجمالي | الضريبة | نسبة الضريبة | السعر | الكمية | البيان
        Dim cols = New String() {"الإجمالي", "الضريبة", "نسبة الضريبة", "السعر", "الكمية", "البيان"}
        Dim exprs = New String() {"FormatString('{0:#,0.00}', [SubTotal])", "FormatString('{0:#,0.00}', [Tax])",
                                  "FormatString('{0:0.##}%', [TaxValue])", "FormatString('{0:#,0.00}', [Cost])",
                                  "FormatString('{0:0.##}', [Qty])", "[DiplomName]"}
        Dim widths = New Single() {105, 95, 95, 105, 70, W - 470}

        Dim headTable = TABLE(0, 324)
        headTable.BorderColor = BRAND
        Dim headRow As New XRTableRow()
        headRow.HeightF = 32
        For i As Integer = 0 To cols.Length - 1
            Dim c = CELL(cols(i), widths(i), True)
            c.BackColor = BRAND
            c.ForeColor = Color.White
            c.BorderColor = BRAND
            headRow.Cells.Add(c)
        Next
        headTable.Rows.Add(headRow)
        headTable.SizeF = New SizeF(W, 32)
        header.Controls.Add(headTable)
        header.HeightF = 356

        Dim lineTable = TABLE(0, 0)
        Dim lineRow As New XRTableRow()
        lineRow.HeightF = 32
        For i As Integer = 0 To cols.Length - 1
            Dim c = CELL(Nothing, widths(i), False)
            c.ExpressionBindings.Add(New ExpressionBinding("BeforePrint", "Text", exprs(i)))
            lineRow.Cells.Add(c)
        Next
        lineTable.Rows.Add(lineRow)
        lineTable.Borders = BorderSide.Left Or BorderSide.Right Or BorderSide.Bottom
        lineTable.SizeF = New SizeF(W, 32)
        detail.Controls.Add(lineTable)
        detail.HeightF = 32

        '''''''''''''''''''''''''''''''''''' الإجماليات شمال + كود QR يمين
        Dim ty As Single = 16
        Dim n As Integer = 0
        For Each t In {New String() {"الإجمالي قبل الضريبة", "FormatString('{0:#,0.00}', [BillTotal])"},
                       New String() {"ضريبة القيمة المضافة", "FormatString('{0:#,0.00}', [BillTax])"},
                       New String() {"الإجمالي شامل الضريبة (ر.س)", "FormatString('{0:#,0.00}', [BillSubTotal])"}}
            Dim isLast As Boolean = (n = 2)
            Dim totals = TABLE(0, ty)
            If n > 0 Then totals.Borders = BorderSide.Left Or BorderSide.Right Or BorderSide.Bottom
            Dim tr As New XRTableRow()
            tr.HeightF = 32
            Dim v = CELL(Nothing, 130, True)
            v.ExpressionBindings.Add(New ExpressionBinding("BeforePrint", "Text", t(1)))
            tr.Cells.Add(v)
            Dim lbl = CELL(t(0), 250, isLast)
            If isLast Then
                For Each c As XRTableCell In {v, lbl}
                    c.BackColor = BRAND
                    c.ForeColor = Color.White
                    c.BorderColor = BRAND
                Next
                totals.BorderColor = BRAND
            Else
                lbl.BackColor = SOFT
            End If
            tr.Cells.Add(lbl)
            totals.Rows.Add(tr)
            totals.SizeF = New SizeF(380, 32)
            footer.Controls.Add(totals)
            ty += 32
            n += 1
        Next

        Dim qr As New XRBarCode()
        Dim gen As New QRCodeGenerator()
        gen.CompactionMode = QRCodeCompactionMode.Byte
        gen.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.M
        qr.Symbology = gen
        qr.ShowText = False
        qr.AutoModule = True
        qr.Padding = New PaddingInfo(0, 0, 0, 0)
        qr.RightToLeft = DevExpress.XtraReports.UI.RightToLeft.No
        qr.Text = h.QrText
        qr.LocationFloat = New DevExpress.Utils.PointFloat(W - 140, 12)
        qr.SizeF = New SizeF(140, 140)
        footer.Controls.Add(qr)

        Dim fy As Single = 168
        If Not String.IsNullOrWhiteSpace(h.Notes) Then
            Dim note = TXT("ملاحظات: " & h.Notes, 0, fy, W, 24, 10, False, TextAlignment.TopRight, MUTED)
            note.CanGrow = True
            note.WordWrap = True
            footer.Controls.Add(note)
            fy += 32
        End If

        Dim sep As New XRLine()
        sep.LineWidth = 1
        sep.ForeColor = LINE
        sep.LocationFloat = New DevExpress.Utils.PointFloat(0, fy)
        sep.SizeF = New SizeF(W, 4)
        footer.Controls.Add(sep)
        footer.Controls.Add(TXT("المبالغ المدفوعة رسوم دراسية غير مستردة", 0, fy + 8, W, 26, 11, True, TextAlignment.MiddleCenter, INK))
        footer.HeightF = fy + 40
    End Sub

    ' مربع فيه 3 صفوف: العنوان يمين والقيمة شمال
    ' كل صف: {العنوان, التعبير, NO=رقم الفاتورة | RTL=نص عربي | LTR=أرقام وتواريخ}
    Private Function INFOBOX(x As Single, y As Single, boxW As Single, rows As String()()) As XRPanel
        Dim p As New XRPanel()
        p.LocationFloat = New DevExpress.Utils.PointFloat(x, y)
        p.SizeF = New SizeF(boxW, rows.Length * 33 + 2)
        p.Borders = BorderSide.All
        p.BorderColor = LINE
        p.BorderWidth = 1
        For r As Integer = 0 To rows.Length - 1
            Dim ry As Single = 1 + r * 33
            If r < rows.Length - 1 Then
                Dim strip = TXT(Nothing, 8, ry, boxW - 16, 33, 10, False, TextAlignment.MiddleCenter)
                strip.Borders = BorderSide.Bottom
                strip.BorderColor = Color.FromArgb(225, 227, 222)
                p.Controls.Add(strip)
            End If
            p.Controls.Add(TXT(rows(r)(0), boxW - 120, ry, 112, 33, 10, False, TextAlignment.MiddleRight, MUTED))
            If rows(r)(2) = "NO" Then
                Dim no = BOUND(rows(r)(1), 10, ry + 6, 70, 21, 12, True, TextAlignment.MiddleCenter, False)
                no.BackColor = BRAND
                no.ForeColor = Color.White
                p.Controls.Add(no)
            Else
                p.Controls.Add(BOUND(rows(r)(1), 8, ry, boxW - 130, 33, 11, True, TextAlignment.MiddleLeft, rows(r)(2) = "RTL"))
            End If
        Next
        Return p
    End Function

    ' align = المكان اللي النص يظهر فيه فعلاً على الورقة (يمين / شمال)
    Private Function TXT(text As String, x As Single, y As Single, w As Single, h As Single, size As Single, bold As Boolean,
                         align As TextAlignment, Optional color As Color = Nothing, Optional rtl As Boolean = True) As XRLabel
        Dim l As New XRLabel()
        l.RightToLeft = If(rtl, DevExpress.XtraReports.UI.RightToLeft.Yes, DevExpress.XtraReports.UI.RightToLeft.No)
        If rtl Then align = MIRROR(align)
        l.Text = text
        l.LocationFloat = New DevExpress.Utils.PointFloat(x, y)
        l.SizeF = New SizeF(w, h)
        l.Font = New Font(FONTNAME, size, If(bold, FontStyle.Bold, FontStyle.Regular))
        l.ForeColor = If(color.IsEmpty, INK, color)
        l.TextAlignment = align
        l.Padding = New PaddingInfo(4, 4, 0, 0)
        l.WordWrap = False
        l.Borders = BorderSide.None ' متورثش إطار المربع
        Return l
    End Function

    ' مع RightToLeft المحاذاة يمين/شمال بتتعكس في التقرير
    Private Shared Function MIRROR(a As TextAlignment) As TextAlignment
        Select Case a
            Case TextAlignment.MiddleRight : Return TextAlignment.MiddleLeft
            Case TextAlignment.MiddleLeft : Return TextAlignment.MiddleRight
            Case TextAlignment.TopRight : Return TextAlignment.TopLeft
            Case TextAlignment.TopLeft : Return TextAlignment.TopRight
            Case Else : Return a
        End Select
    End Function

    Private Function BOUND(expr As String, x As Single, y As Single, w As Single, h As Single, size As Single, bold As Boolean, align As TextAlignment,
                           Optional rtl As Boolean = True) As XRLabel
        Dim l = TXT(Nothing, x, y, w, h, size, bold, align, Nothing, rtl)
        l.ExpressionBindings.Add(New ExpressionBinding("BeforePrint", "Text", expr))
        Return l
    End Function

    Private Function TABLE(x As Single, y As Single) As XRTable
        Dim t As New XRTable()
        t.LocationFloat = New DevExpress.Utils.PointFloat(x, y)
        t.Borders = BorderSide.All
        t.BorderColor = LINE
        t.BorderWidth = 1
        t.RightToLeft = DevExpress.XtraReports.UI.RightToLeft.No
        Return t
    End Function

    Private Function CELL(text As String, w As Single, bold As Boolean) As XRTableCell
        Dim c As New XRTableCell()
        c.Text = text
        c.WidthF = w
        c.Font = New Font(FONTNAME, 10, If(bold, FontStyle.Bold, FontStyle.Regular))
        c.ForeColor = INK
        c.TextAlignment = TextAlignment.MiddleCenter
        c.Padding = New PaddingInfo(6, 6, 0, 0)
        c.RightToLeft = DevExpress.XtraReports.UI.RightToLeft.Yes
        c.Borders = BorderSide.All
        c.BorderColor = LINE
        Return c
    End Function
End Class
