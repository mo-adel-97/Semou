Imports System.Data.SqlClient

Public Class BranchTimeWork
    Dim dbcontext As New SemouEntities
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            Dim w As New NewBranchList
            If AllFiles.DEFBRANCH = True Then
                w.TxtType.Text = 1
                w.TxtUserGuid.Text = AllFiles.USER_GUID____.ToString
                w.TxtFind.ReadOnly = True
            Else
                w.TxtType.Text = 0
                w.TxtIsUse.Text = True
                w.TxtFind.ReadOnly = False
            End If
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtBranchGuid.Text <> Nothing Then
                SHOWDAILYTABEL()
                SHOWTIMECOURSE()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW____()
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        DayTimeGrid.Rows.Clear()
        TxtStaut.Text = False
        TimeGrid.Rows.Clear()
    End Sub
    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW____()
            BranchTimeWork_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub LOADDAY()
        DayGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadDay", connection)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadDay")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            DayGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DayGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                DayGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                DayGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub FORMATTIMEINGRID()
        ' ضبط تنسيق الأعمدة
        With DayTimeGrid
            ' عمود بداية الدوام
            If .Columns.Contains("StartColumn") Then
                .Columns("StartColumn").DefaultCellStyle.Format = "HH:mm" ' تنسيق 24 ساعة
            End If

            ' عمود نهاية الدوام
            If .Columns.Contains("EndColumn") Then
                .Columns("EndColumn").DefaultCellStyle.Format = "HH:mm" ' تنسيق 24 ساعة
            End If
        End With
    End Sub
    Sub FORMATTIMEINTIMEGRID()
        ' ضبط تنسيق الأعمدة
        With TimeGrid
            ' عمود بداية المحاضرة
            If .Columns.Contains("startin") Then
                .Columns("startin").DefaultCellStyle.Format = "HH:mm" ' تنسيق 24 ساعة
            End If

            ' عمود نهاية المحاضرة
            If .Columns.Contains("endin") Then
                .Columns("endin").DefaultCellStyle.Format = "HH:mm" ' تنسيق 24 ساعة
            End If
        End With
    End Sub
    Private Sub BranchTimeWork_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADDAY()
            FORMATTIMEINGRID()
            FORMATTIMEINTIMEGRID()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub DayGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DayGrid.CellContentClick

    End Sub

    Private Sub DayGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DayGrid.CellDoubleClick
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim row As DataGridViewRow
                Dim found As Boolean = False
                Dim ii As Integer = DayGrid.CurrentRow.Index
                For Each row In DayTimeGrid.Rows
                    For i As Integer = 0 To DayTimeGrid.Rows.Count - 1
                        If row.Cells(4).Value = DayGrid.Rows(ii).Cells(2).Value.ToString Then
                            found = True
                            MessageBox.Show("تم ادراج اليوم مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit For
                        End If
                    Next
                Next
                If Not found Then
                    For Each row In DayGrid.SelectedRows
                        DayTimeGrid.Rows.Add()
                        Dim i As Integer = DayTimeGrid.Rows.Count() - 1
                        DayTimeGrid.Rows(i).Cells(0).Value = DayGrid.Rows(ii).Cells(1).Value.ToString()
                        DayTimeGrid.Rows(i).Cells(4).Value = DayGrid.Rows(ii).Cells(2).Value.ToString()
                    Next
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub DayTimeGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DayTimeGrid.CellContentClick

    End Sub
    Private Function IsValid24HourTime(timeString As String) As Boolean
        Dim timeFormat As String = "HH:mm" ' صيغة الوقت 24 ساعة
        Dim parsedTime As DateTime
        Return DateTime.TryParseExact(timeString, timeFormat, Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, parsedTime)
    End Function
    Private Sub DayTimeGrid_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles DayTimeGrid.CellValidating
        '' تحديد الأعمدة التي تحتاج للتحقق
        'If e.ColumnIndex = DayTimeGrid.Columns("StartColumn").Index OrElse
        '   e.ColumnIndex = DayTimeGrid.Columns("EndColumn").Index Then
        '    ' التحقق من صيغة الوقت
        '    Dim inputTime As String = e.FormattedValue.ToString()
        '    If Not IsValid24HourTime(inputTime) Then
        '        MessageBox.Show("برجاء إدخال الوقت بصيغة 24 ساعة (HH:mm)", "خطأ في التنسيق", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '        e.Cancel = True ' منع الانتقال إلى الخلية التالية
        '    End If
        'End If
    End Sub
    Private Sub CalculateWorkHours()
        'For Each row As DataGridViewRow In DayTimeGrid.Rows
        '    Try
        '        Dim startTime As TimeSpan = TimeSpan.Parse(row.Cells("StartTime").ToString())
        '        Dim endTime As TimeSpan = TimeSpan.Parse(row.Cells("EndTime").ToString())

        '        ' حساب الفارق الزمني
        '        Dim duration As TimeSpan = endTime.Subtract(startTime)

        '        ' معالجة الحالة إذا كان الوقت يمتد عبر يومين
        '        If duration < TimeSpan.Zero Then
        '            duration = duration.Add(TimeSpan.FromHours(24))
        '        End If

        '        'Console.WriteLine($"مدة العمل: {duration.Hours} ساعات و {duration.Minutes} دقائق")

        '        '' جلب القيم من أعمدة البداية والنهاية
        '        'Dim startTime As DateTime = DateTime.Parse(row.Cells("StartColumn").Value.ToString())
        '        'Dim endTime As DateTime = DateTime.Parse(row.Cells("EndColumn").Value.ToString())

        '        '' حساب عدد الساعات
        '        Dim workHours As Double = (endTime - startTime).TotalHours

        '        '' عرض عدد الساعات في العمود الثالث
        '        row.Cells("WorkHoursColumn").Value = Math.Round(workHours, 2)
        '    Catch ex As Exception
        '        ' في حالة وجود خطأ، تجاهل
        '        row.Cells("WorkHoursColumn").Value = "خطأ"
        '    End Try
        'Next
    End Sub

    Private Sub DayTimeGrid_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DayTimeGrid.CellValueChanged
        'Try
        '    If e.ColumnIndex = 1 Or e.ColumnIndex = 2 Then
        '        'If TxtStaut.Text = False Then
        '        CalculateWorkHours()
        '        'End If
        '    End If
        'Catch ex As Exception
        '    MessageBox.Show(ex.ToString)
        'End Try
    End Sub

    Private Sub DayTimeGrid_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles DayTimeGrid.CellBeginEdit
    End Sub

    Private Sub DayTimeGrid_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles DayTimeGrid.EditingControlShowing
        Try
            ' التحقق من أن العمود هو عمود الوقت
            If DayTimeGrid.CurrentCell.ColumnIndex = DayTimeGrid.Columns("StartColumn").Index OrElse
               DayTimeGrid.CurrentCell.ColumnIndex = DayTimeGrid.Columns("EndColumn").Index Then

                ' إضافة حدث KeyPress لمعالجة الإدخال
                Dim editingControl As TextBox = TryCast(e.Control, TextBox)
                If editingControl IsNot Nothing Then
                    RemoveHandler editingControl.KeyPress, AddressOf TimeCell_KeyPress ' إزالة أي أحداث مكررة
                    AddHandler editingControl.KeyPress, AddressOf TimeCell_KeyPress
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Sub TimeCell_KeyPress(sender As Object, e As KeyPressEventArgs)
        Dim textBox As TextBox = CType(sender, TextBox)

        ' السماح فقط بالأرقام والرمز ":" والحذف
        If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> Convert.ToChar(Keys.Back) Then
            e.Handled = True
            Return
        End If

        ' إضافة ":" تلقائيًا بعد إدخال الساعات (حرفان)
        If Char.IsDigit(e.KeyChar) AndAlso textBox.Text.Length = 2 Then
            textBox.Text &= ":"
            textBox.SelectionStart = textBox.Text.Length ' وضع المؤشر في نهاية النص
        End If

        ' منع إدخال أكثر من 5 أحرف (HH:mm)
        If textBox.Text.Length >= 5 AndAlso e.KeyChar <> Convert.ToChar(Keys.Back) Then
            e.Handled = True
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf DayTimeGrid.Rows.Count = 0 Then
            MessageBox.Show("لا يمكن الحقظ بدوت ادخال اوقات الدوام", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub ADDTIMETABEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddBranchDailyWork", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To DayTimeGrid.Rows.Count - 1
            If DayTimeGrid.Rows(i).Cells(4).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
                comm.Parameters.AddWithValue("@FromTime", DayTimeGrid.Rows(i).Cells(1).Value.ToString())
                comm.Parameters.AddWithValue("@ToTime", DayTimeGrid.Rows(i).Cells(2).Value.ToString())
                comm.Parameters.AddWithValue("@CountTime", Val(DayTimeGrid.Rows(i).Cells(3).Value))
                comm.Parameters.AddWithValue("@DayGuid", DayTimeGrid.Rows(i).Cells(4).Value.ToString())
            End If
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
            connection.Open()
            Dim rs As Integer = comm.ExecuteNonQuery()
            If rs > 0 Then
                comm.Parameters.Clear()
            End If
        Next
    End Sub
    Sub ADDTIMECOURSE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddTimeWork", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To TimeGrid.Rows.Count - 1
            If TimeGrid.Rows(i).Cells(0).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
                comm.Parameters.AddWithValue("@StartIn", TimeGrid.Rows(i).Cells(0).Value.ToString())
            End If
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
            connection.Open()
            Dim rs As Integer = comm.ExecuteNonQuery()
            If rs > 0 Then
                comm.Parameters.Clear()
            End If
        Next
    End Sub
    Sub DELETETIMETABEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteBranchDailyWork", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETETIMECOURSE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteTimeWork", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWDAILYTABEL()
        DayTimeGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowBranchDailyWork", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowBranchDailyWork")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            DayTimeGrid.RowCount = ds.Tables(0).Rows.Count
            TxtStaut.Text = True
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DayTimeGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                DayTimeGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                DayTimeGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                DayTimeGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                DayTimeGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                COUNT__ = COUNT__ + 1
            Next
        Else
            TxtStaut.Text = False
        End If
    End Sub
    Sub SHOWTIMECOURSE()
        TimeGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowTimeWork", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowTimeWork")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            TimeGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TimeGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtStaut.Text = False Then
                    If AllFiles.FORSAVE("worktime") = False Then
                        Return
                    Else
                        ADDTIMETABEL()
                        ADDTIMECOURSE()
                        TxtStaut.Text = True
                        MessageBox.Show("تم حفظ جدول دوام الفرع بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                ElseIf TxtStaut.Text = True Then
                    If AllFiles.FORUPDATE("worktime") = False Then
                    Else
                        DELETETIMETABEL()
                        ADDTIMETABEL()
                        DELETETIMECOURSE()
                        ADDTIMECOURSE()
                        MessageBox.Show("تم تعديل جدول دوام الفرع بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub DayTimeGrid_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DayTimeGrid.CellEndEdit
        Try
            Dim i As Integer = DayTimeGrid.CurrentCell.RowIndex
            Dim j As Integer = DayTimeGrid.CurrentCell.ColumnIndex
            If j = 1 Or j = 2 Then
                If Val(DayTimeGrid.Rows(i).Cells(1).Value) <> Nothing And Val(DayTimeGrid.Rows(i).Cells(2).Value) <> Nothing Then
                    DayTimeGrid.Rows(i).Cells(3).Value = Math.Abs(Val(DayTimeGrid.Rows(i).Cells(1).Value) - Val(DayTimeGrid.Rows(i).Cells(2).Value))
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TimeGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles TimeGrid.CellContentClick

    End Sub

    Private Sub TimeGrid_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles TimeGrid.CellValidating
        'Try
        '    ' تحديد الأعمدة التي تحتاج للتحقق
        '    If e.ColumnIndex = TimeGrid.Columns("startin").Index OrElse
        '       e.ColumnIndex = TimeGrid.Columns("endin").Index Then
        '        ' التحقق من صيغة الوقت
        '        Dim inputTime As String = e.FormattedValue.ToString()
        '        If Not IsValid24HourTime(inputTime) Then
        '            MessageBox.Show("برجاء إدخال الوقت بصيغة 24 ساعة (HH:mm)", "خطأ في التنسيق", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '            e.Cancel = True ' منع الانتقال إلى الخلية التالية
        '        End If
        '    End If
        'Catch ex As Exception
        '    MessageBox.Show(ex.ToString)
        'End Try
    End Sub

    Private Sub TimeGrid_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles TimeGrid.EditingControlShowing
        Try
            ' التحقق من أن العمود هو عمود الوقت
            If TimeGrid.CurrentCell.ColumnIndex = TimeGrid.Columns("startin").Index OrElse
               TimeGrid.CurrentCell.ColumnIndex = TimeGrid.Columns("endin").Index Then

                ' إضافة حدث KeyPress لمعالجة الإدخال
                Dim editingControl As TextBox = TryCast(e.Control, TextBox)
                If editingControl IsNot Nothing Then
                    RemoveHandler editingControl.KeyPress, AddressOf Time1Cell_KeyPress ' إزالة أي أحداث مكررة
                    AddHandler editingControl.KeyPress, AddressOf Time1Cell_KeyPress
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Sub Time1Cell_KeyPress(sender As Object, e As KeyPressEventArgs)
        Dim textBox As TextBox = CType(sender, TextBox)

        ' السماح فقط بالأرقام والرمز ":" والحذف
        If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> Convert.ToChar(Keys.Back) Then
            e.Handled = True
            Return
        End If

        ' إضافة ":" تلقائيًا بعد إدخال الساعات (حرفان)
        If Char.IsDigit(e.KeyChar) AndAlso textBox.Text.Length = 2 Then
            textBox.Text &= ":"
            textBox.SelectionStart = textBox.Text.Length ' وضع المؤشر في نهاية النص
        End If

        ' منع إدخال أكثر من 5 أحرف (HH:mm)
        If textBox.Text.Length >= 5 AndAlso e.KeyChar <> Convert.ToChar(Keys.Back) Then
            e.Handled = True
        End If
    End Sub

    Private Sub CmdCalc_Click(sender As Object, e As EventArgs) Handles CmdCalc.Click
        Try
            ' طلب إدخال بداية الدوام
            Dim startWorkInput As String = InputBox("من فضلك، أدخل وقت بداية الدوام (بصيغة HH:mm)", "بداية الدوام")
            ' طلب إدخال نهاية الدوام
            Dim endWorkInput As String = InputBox("من فضلك، أدخل وقت نهاية الدوام (بصيغة HH:mm)", "نهاية الدوام")
            ' طلب إدخال مدة المحاضرة
            Dim lectureDurationInput As String = InputBox("من فضلك، أدخل مدة المحاضرة بالساعة (مثل: 2 لـ ساعتين)", "مدة المحاضرة")

            ' التحقق من إدخال القيم بشكل صحيح
            Dim startWork As DateTime
            Dim endWork As DateTime
            Dim lectureDuration As Double

            If DateTime.TryParse(startWorkInput, startWork) AndAlso DateTime.TryParse(endWorkInput, endWork) AndAlso Double.TryParse(lectureDurationInput, lectureDuration) Then
                ' إضافة مواعيد المحاضرات إلى الداتا جريد
                PopulateLectureTimes(startWork, endWork, lectureDuration)
            Else
                MessageBox.Show("المدخلات غير صحيحة. من فضلك أدخل القيم بشكل صحيح.", "خطأ في الإدخال", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Sub PopulateLectureTimes(startWork As DateTime, endWork As DateTime, lectureDuration As Double)
        ' مسح البيانات السابقة في الداتا جريد
        TimeGrid.Rows.Clear()

        ' حساب عدد المحاضرات بناءً على المدة المتاحة
        Dim lectureStart As DateTime = startWork
        Dim lectureEnd As DateTime
        Dim lectureCount As Integer = 0

        ' التكرار لإضافة المحاضرات
        While lectureStart.AddHours(lectureDuration) <= endWork
            lectureEnd = lectureStart.AddHours(lectureDuration)

            ' دمج وقت البداية والنهاية في عمود واحد
            Dim lectureTime As String = $"{lectureStart:HH:mm} - {lectureEnd:HH:mm}"

            ' إضافة بيانات المحاضرة إلى الداتا جريد
            TimeGrid.Rows.Add(lectureTime)

            ' تحديث بداية المحاضرة التالية
            lectureStart = lectureEnd
            lectureCount += 1
        End While

        ' إذا لم يكن هناك محاضرات بعد الحساب
        If lectureCount = 0 Then
            MessageBox.Show("لا يوجد وقت كافٍ لإضافة محاضرات بناءً على المدخلات.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

    End Sub
End Class