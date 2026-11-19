Imports System.Data.SqlClient

Public Class EncodingTabel
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub
    Sub SHOWTIMECOURSE()
        StudyTabelGrid.Rows.Clear()
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
        StudyTabelGrid.Columns.Clear()
        ' إضافة العمود الأول لعرض "اليوم"
        Dim dayColumn As New DataGridViewTextBoxColumn()
        dayColumn.Name = "DayColumn"
        dayColumn.HeaderText = "اليوم"
        StudyTabelGrid.Columns.Add(dayColumn)

        ' إضافة الفترات الزمنية كعناوين أعمدة بدءًا من العمود الثاني
        For Each row As DataRow In ds.Tables(0).Rows
            Dim timeSlotID As String = row("guid").ToString() ' GUID الخاص بالفترة
            Dim timeSlot As String = row("StartIn").ToString() ' اسم الفترة الزمنية

            ' إنشاء عمود جديد لكل فترة زمنية
            Dim column As New DataGridViewTextBoxColumn()
            column.Name = timeSlotID ' استخدام GUID كاسم فريد
            column.HeaderText = timeSlot ' عرض اسم الفترة
            StudyTabelGrid.Columns.Add(column)
        Next
        ' إضافة 3 أعمدة إضافية بعد آخر عمود للفترات
        Dim dayguid As New DataGridViewTextBoxColumn()
        dayguid.Name = "Dayguid"
        dayguid.HeaderText = "dayguid"
        StudyTabelGrid.Columns.Add(dayguid)
        dayguid.Visible = False
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
                TxtClassTime.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
            End If
            If TxtBranchGuid.Text <> Nothing Then
                SHOWDAILYTABEL()
                SHOWTIMECOURSE()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDiplom_Click(sender As Object, e As EventArgs) Handles CmdFindDiplom.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New DiplomList
                w.TxtIsUse.Text = True
                w.TxtType.Text = 0
                w.TxtStudyType.Text = 0
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtDiplomName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                    TxtDiplomDepart.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
                End If
                If TxtDiplomGuid.Text <> Nothing Then
                    LOADTRANIER()
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع الاول", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub LOADBOOK()
        BookGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadBookForTabel", connection)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadBookForTabel")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BookGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BookGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BookGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BookGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BookGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BookGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub LOADTRANIER()
        TranierGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadTranierForTable", connection)
        comm.Parameters.AddWithValue("@TranierDepart", TxtDiplomDepart.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If TxtBookAdvanced.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BookType", TxtBookAdvanced.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadTranierForTable")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            TranierGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TranierGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                TranierGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWDAILYTABEL()
        DayGrid.Rows.Clear()
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
            DayGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DayGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                DayGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(4).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdFindLevel_Click(sender As Object, e As EventArgs) Handles CmdFindLevel.Click
        Try
            If TxtDiplomGuid.Text <> Nothing Then
                Dim w As New LevelList
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtLevelName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtLevelGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtLevelGuid.Text <> Nothing Then
                    LOADBOOK()
                End If
            Else
                MessageBox.Show("برجاء اختيار التخصص اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub DayGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DayGrid.CellContentClick

    End Sub

    Private Sub DayGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DayGrid.CellDoubleClick
        Try
            If TxtTranierGuid.Text <> Nothing Then
                For Each row In DayGrid.SelectedRows
                    StudyTabelGrid.Rows.Add()
                    Dim i As Integer = StudyTabelGrid.Rows.Count() - 1
                    ' إضافة اسم اليوم في العمود المحدد باستخدام الاسم
                    StudyTabelGrid.Rows(i).Cells("DayColumn").Value = DayGrid.Rows(row.Index).Cells("MDayName").Value.ToString()

                    ' إضافة GUID في العمود المحدد باستخدام الاسم
                    StudyTabelGrid.Rows(i).Cells("Dayguid").Value = DayGrid.Rows(row.Index).Cells("MDayGuid").Value.ToString()
                Next
            Else
                MessageBox.Show("برجاء اختيار المدرب اولا ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub StudyTabelGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles StudyTabelGrid.CellContentClick

    End Sub

    Private Sub StudyTabelGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles StudyTabelGrid.CellClick
        If TxtBookGuid.Text <> Nothing Then
            If TxtTranierGuid.Text <> Nothing Then
                If TxtBookCount.Text <> 0 Then
                    If TxtTranierCount.Text <> 0 Then
                        ' التحقق من أن النقر تم على خلية داخل الجريد وليست على العنوان
                        If e.RowIndex >= 0 AndAlso e.ColumnIndex > 0 Then
                            ' الحصول على اسم العمود (اسم الفترة الزمنية)
                            Dim timeSlotName As String = StudyTabelGrid.Columns(e.ColumnIndex).Name
                            Dim timeName As String = StudyTabelGrid.Columns(e.ColumnIndex).HeaderText
                            ' الحصول على القيمة المخزنة في العمود DayGuid لنفس الصف
                            Dim dayGuidValue As String = StudyTabelGrid.Rows(e.RowIndex).Cells("DayGuid").Value.ToString()
                            Dim dayNameValue As String = StudyTabelGrid.Rows(e.RowIndex).Cells(0).Value.ToString()
                            If AllFiles.ASKFORAVALABILTRANIER(TxtBranchGuid.Text, dayGuidValue, timeSlotName, TxtTranierGuid.Text) = True Then
                                MessageBox.Show("عفوا المدرب الذي قمت باختياره غير متاح فى الوقت المحدد ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return
                            Else
                                Dim w As New ChooseData
                                w.TxtRowIndex.Text = e.RowIndex
                                w.TxtDayName.Text = dayNameValue
                                w.TxttimeGuid.Text = timeSlotName
                                w.TxtTimeName.Text = timeName
                                w.TxtDayGuid.Text = dayGuidValue
                                w.TxtTranierGuid.Text = TxtTranierGuid.Text
                                w.TxtTranierName.Text = TxtTranierName.Text
                                w.TxtBookGuid.Text = TxtBookGuid.Text
                                w.TxtBookName.Text = TxtBookName.Text
                                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                                w.ShowDialog()
                                StudyTabelGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True
                                StudyTabelGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
                                If w.IS_Yes = True Then
                                    StudyTabelGrid.Rows(e.RowIndex).Cells(timeSlotName).Value = TxtBookName.Text & vbCrLf & w.TxtRoomName.Text & vbCrLf & TxtTranierName.Text
                                    TxtBookCount.Text = Val(TxtBookCount.Text) - Val(TxtClassTime.Text)
                                    TempGrid.Rows.Add()
                                    Dim i As Integer = TempGrid.Rows.Count() - 1
                                    TempGrid.Rows(i).Cells(0).Value = w.TxtDayGuid.Text
                                    TempGrid.Rows(i).Cells(1).Value = w.TxttimeGuid.Text
                                    TempGrid.Rows(i).Cells(2).Value = w.TxtBookGuid.Text
                                    TempGrid.Rows(i).Cells(3).Value = w.TxtRoomGuid.Text
                                    TempGrid.Rows(i).Cells(4).Value = w.TxtTranierGuid.Text

                                    '' حساب الوقت المتبقي للمدرب
                                    Dim selectedTranierGuid As String = TxtTranierGuid.Text

                                    ' عد عدد مرات تكرار trainerguid في الجريد الثانية
                                    Dim count As Integer = 0
                                    For Each row As DataGridViewRow In TempGrid.Rows
                                        count = 0
                                        If row.Cells(4).Value IsNot Nothing AndAlso row.Cells(4).Value.ToString() = selectedTranierGuid Then
                                            count += 1
                                        End If
                                    Next
                                    TxtTranierCount.Text = Val(TxtTranierCount.Text) - (Val(count) * Val(TxtClassTime.Text))
                                End If
                            End If
                        End If
                    Else
                        MessageBox.Show("تم اكتمال عدد ساعات العمل المحددة للمدرب ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                Else
                    MessageBox.Show("تم اكتمال عدد ساعات الاتصال الخاصة بالمقرر ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            Else
                MessageBox.Show("برجاء اختيار المدرب اولا ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Else
                MessageBox.Show("برجاء اختيار المقرر اولا ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

    End Sub

    Private Sub BookGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BookGrid.CellContentClick

    End Sub

    Private Sub BookGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles BookGrid.CellClick
        Try
            Dim i As Integer = BookGrid.CurrentCell.RowIndex
            Dim j As Integer = BookGrid.CurrentCell.ColumnIndex
            TxtBookName.Text = BookGrid.Rows(i).Cells(0).Value.ToString()
            TxtBookGuid.Text = BookGrid.Rows(i).Cells(3).Value.ToString()
            TxtBookCount.Text = BookGrid.Rows(i).Cells(2).Value.ToString()
            TxtBookAdvanced.Text = BookGrid.Rows(i).Cells(4).Value.ToString()
            If TxtBookGuid.Text <> Nothing Then
                LOADTRANIER()
            End If
            ' تأكد من أن الصف والنقر صالحان
            If e.RowIndex >= 0 Then
                ' احصل على bookguid من الجريد الأولى
                Dim selectedBookGuid As String = BookGrid.Rows(e.RowIndex).Cells(3).Value.ToString()

                ' عد عدد مرات تكرار bookguid في الجريد الثانية
                Dim count As Integer = 0
                For Each row As DataGridViewRow In TempGrid.Rows
                    If row.Cells(2).Value IsNot Nothing AndAlso row.Cells(2).Value.ToString() = selectedBookGuid Then
                        count += 1
                    End If
                Next
                count = count * Val(TxtClassTime.Text)
                TxtBookCount.Text = Val(TxtBookCount.Text) - Val(count)
            End If
        Catch ex As Exception

        End Try
    End Sub
    Sub SHOWTRANIERTIME()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ChkTrainerAvailableTime", connection)
        comm.Parameters.AddWithValue("@TranierGuid", TxtTranierGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ChkTrainerAvailableTime")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtTranierCount.Text = ds.Tables(0).Rows(i)(0).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub TranierGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles TranierGrid.CellContentClick

    End Sub
    Private Sub TranierGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles TranierGrid.CellClick
        Try
            Dim i As Integer = TranierGrid.CurrentCell.RowIndex
            Dim j As Integer = TranierGrid.CurrentCell.ColumnIndex
            TxtTranierName.Text = TranierGrid.Rows(i).Cells(0).Value.ToString()
            TxtTranierGuid.Text = TranierGrid.Rows(i).Cells(1).Value.ToString()
            If TxtTranierGuid.Text <> Nothing Then
                SHOWTRANIERTIME()
            End If
            ' تأكد من أن الصف والنقر صالحان
            If e.RowIndex >= 0 Then
                ' احصل على bookguid من الجريد الأولى
                Dim selectedTranierGuid As String = TranierGrid.Rows(e.RowIndex).Cells(1).Value.ToString()

                ' عد عدد مرات تكرار trainerguid في الجريد الثانية
                Dim count As Integer = 0
                For Each row As DataGridViewRow In TempGrid.Rows
                    If row.Cells(4).Value IsNot Nothing AndAlso row.Cells(4).Value.ToString() = selectedTranierGuid Then
                        count += 1
                    End If
                Next
                count = count * 2
                TxtTranierCount.Text = Val(TxtTranierCount.Text) - Val(count)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub StudyTabelGrid_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles StudyTabelGrid.CellMouseClick
        Try
            ' تحقق من أن النقر كان بزر الماوس الأيمن وعلى خلية صالحة
            If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
                ' احصل على القيمة الحالية للخلية قبل الحذف
                'Dim cellValue As String = If(StudyTabelGrid.Rows(e.RowIndex).Cells(e.ColumnIndex).Value, "").ToString()
                ' الحصول على اسم العمود (اسم الفترة الزمنية)
                Dim timeSlotName As String = StudyTabelGrid.Columns(e.ColumnIndex).Name

                ' الحصول على القيمة المخزنة في العمود DayGuid لنفس الصف
                Dim dayGuidValue As String = StudyTabelGrid.Rows(e.RowIndex).Cells("DayGuid").Value.ToString()

                ' طلب تأكيد من المستخدم قبل مسح البيانات
                Dim result As DialogResult = MessageBox.Show("هل تريد مسح البيانات من هذه الخلية؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    ' مسح بيانات الخلية المحددة
                    StudyTabelGrid.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = Nothing

                    ' البحث عن القيمة في الجريد الثانية وحذف الصف إذا تم العثور عليه
                    For Each row As DataGridViewRow In TempGrid.Rows
                        ' التحقق من أن القيمتين موجودتان ومتطابقتان
                        If row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString() = dayGuidValue AndAlso
                           row.Cells(1).Value IsNot Nothing AndAlso row.Cells(1).Value.ToString() = timeSlotName Then
                            ' حذف الصف من DataGridView2
                            TempGrid.Rows.Remove(row)
                            TxtBookGuid.Text = Nothing
                            TxtBookName.Text = Nothing
                            TxtBookCount.Text = Nothing
                            TxtTranierGuid.Text = Nothing
                            TxtTranierName.Text = Nothing
                            TxtTranierCount.Text = Nothing
                            Exit For ' خروج من الحلقة بعد الحذف
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtCalssCode.Text = Nothing Then
            MessageBox.Show("برجاء ادخال رمز الشعبة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الفرع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TempGrid.Rows.Count = 0 Then
            MessageBox.Show("لا يمكن حفظ الشعبة بدون اي مواعيد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Function CHKBOOK() As Boolean
        For Each row1 As DataGridViewRow In BookGrid.Rows
            If Not row1.IsNewRow Then
                Dim bookGuid1 As String = row1.Cells("bookguid").Value.ToString()
                Dim bookTime As Integer = Convert.ToInt32(row1.Cells("booktime").Value)
                Dim textBoxValue As Integer = Convert.ToInt32(TxtClassTime.Text) ' القيمة المخزنة في TextBox

                ' البحث عن bookguid في الجريد الثانية
                Dim count As Integer = 0
                For Each row2 As DataGridViewRow In TempGrid.Rows
                    If Not row2.IsNewRow Then
                        Dim bookGuid2 As String = row2.Cells("tempbookguid").Value.ToString()
                        If bookGuid1 = bookGuid2 Then
                            count += 1
                        End If
                    End If
                Next

                ' إذا لم يتم العثور على bookguid في الجريد الثانية، عرض رسالة خطأ
                If count = 0 Then
                    ''  MessageBox.Show("لم يتم العثور على BookGUID: " & bookGuid1, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Else
                    ' حساب القيمة الجديدة
                    Dim newBookTime As Integer = bookTime - (count * textBoxValue)
                    If (count * textBoxValue) <> row1.Cells("booktime").Value Then
                        Return False
                    End If
                    'row1.Cells("booktime").Value = newBookTime
                End If
            End If
        Next

        Return True
    End Function
    Public Function GET___NO() As Integer
        'Dim BRANCHGUID As Guid = Guid.Parse(TxtBranchGuid.Text)
        'Dim MaxId = dbcontext.Classes.Where(Function(m) m.BranchGuid = BRANCHGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        'Dim MaxDay = New [Class]
        'If (IsNothing(MaxId)) Then
        '    MaxId = 1
        '    TxtCode.Text = MaxId
        'Else
        '    MaxId += 1
        '    MaxDay.Id = MaxId
        '    TxtCode.Text = MaxId
        'End If
        'Return MaxId
    End Function
    Sub ADDCLASS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddClass", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ID", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@ClassCode", TxtCalssCode.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADD_USERACTION_()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("Add_UserAction", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ActionGuid", TxtClassGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
        comm.Parameters.AddWithValue("@Date_", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@Time_", Convert.ToDateTime(DateTime.Now.ToShortTimeString))
        comm.Parameters.AddWithValue("@Notes_", AllFiles.ActionType___)
        comm.Parameters.AddWithValue("@ActionTypeGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionReason", REASON)
        comm.Parameters.AddWithValue("@ActionCode", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWCLASSGUID()
        'Dim BRANCHGUID As Guid = Guid.Parse(TxtBranchGuid.Text)
        'Dim REGDOC_GUID_ = dbcontext.Classes.FirstOrDefault(Function(m) m.Code = TxtCode.Text And m.BranchGuid = BRANCHGUID)
        'TxtGuid.Text = REGDOC_GUID_.Guid.ToString()
    End Sub
    Sub ADDTABEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddClassTimeTable", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To TempGrid.Rows.Count - 1
            If TempGrid.Rows(i).Cells(0).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@ClassGuid", TxtClassGuid.Text)
                comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
                comm.Parameters.AddWithValue("@DayGuid", TempGrid.Rows(i).Cells(0).Value.ToString())
                comm.Parameters.AddWithValue("@TimeGuid", TempGrid.Rows(i).Cells(1).Value.ToString())
                comm.Parameters.AddWithValue("@BookGuid", TempGrid.Rows(i).Cells(2).Value.ToString())
                comm.Parameters.AddWithValue("@RoomGuid", TempGrid.Rows(i).Cells(3).Value.ToString())
                comm.Parameters.AddWithValue("@TranierGuid", TempGrid.Rows(i).Cells(4).Value.ToString())
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
    Sub ADDCLASSBOOK()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddClassBook", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To BookGrid.Rows.Count - 1
            If BookGrid.Rows(i).Cells(3).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@CalssGuid", TxtClassGuid.Text)
                comm.Parameters.AddWithValue("@BookGuid", BookGrid.Rows(i).Cells(3).Value.ToString())
                comm.Parameters.AddWithValue("@BookTime", BookGrid.Rows(i).Cells(2).Value.ToString())
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
    Sub FORSAVE()
        If AllFiles.FORSAVE("studytable") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            'ADDCLASS()
            'SHOWCLASSGUID()
            ADDTABEL()
            ADDCLASSBOOK()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ جدول الشعبة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If CHKBOOK() = False Then
                    MessageBox.Show("بعض المقررات غير مكتملة او غير مدرجة بالجدول برجاء مراجعه المقررات علي الجدول ", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If TxtGuid.Text = Nothing Then
                        FORSAVE()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub EncodingTabel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("studytable")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindClass_Click(sender As Object, e As EventArgs) Handles CmdFindClass.Click
        Try
            If TxtLevelGuid.Text <> Nothing Then
                Dim w As New ClassList
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.TxtDiplomGuid.Text = TxtDiplomGuid.Text
                w.TxtLevelGuid.Text = TxtLevelGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    TxtCalssCode.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtClassGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار المستوي اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click

    End Sub

    Private Sub GroupPanel3_Click(sender As Object, e As EventArgs) Handles GroupPanel3.Click

    End Sub
End Class