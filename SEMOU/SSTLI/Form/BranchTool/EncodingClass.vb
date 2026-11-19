Imports System.Data.SqlClient

Public Class EncodingClass
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub
    Sub LOADSTUDYDETAILS()
        StudentGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("CollectStudentForClass", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "CollectStudentForClass")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            StudentGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                StudentGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                StudentGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                StudentGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                StudentGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                StudentGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                COUNT__ = COUNT__ + 1
            Next
            TxtCount.Text = StudentGrid.Rows.Count()
        Else
            TxtCount.Text = 0
        End If
    End Sub
    Private Sub CmdCalc_Click(sender As Object, e As EventArgs) Handles CmdCalc.Click
        Try
            TxtSelectedCount.Text = 0
            If Val(TxtClassCount.Text) <> 0 And TxtClassCount.Text <> Nothing Then
                LOADSTUDYDETAILS()
            Else
                MessageBox.Show("برجاء تحديد عدد طلاب الشعبة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
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
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع الاول", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
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
            Else
                MessageBox.Show("برجاء اختيار التخصص اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindBatch_Click(sender As Object, e As EventArgs) Handles CmdFindBatch.Click
        Try
            If TxtLevelGuid.Text <> Nothing Then
                Dim w As New BatchList
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtBatchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtBatchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار المستوي اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub ChkAll_CheckedChanged(sender As Object, e As EventArgs) Handles ChkAll.CheckedChanged
        Try
            For Each row As DataGridViewRow In StudentGrid.Rows
                If row.Cells(4).Value <> Nothing Then
                    If ChkAll.Checked = True Then
                        row.Cells(5).Value = True
                    Else
                        row.Cells(5).Value = False
                    End If
                End If
            Next
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub StudentGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles StudentGrid.CellContentClick

    End Sub

    Private Sub StudentGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles StudentGrid.CellClick
        Try
            Dim i As Integer = StudentGrid.CurrentCell.RowIndex
            Dim j As Integer = StudentGrid.CurrentCell.ColumnIndex
            If j = 5 Then
                If StudentGrid.Rows(i).Cells(5).Value = False Then
                    StudentGrid.Rows(i).Cells(5).Value = True
                Else
                    StudentGrid.Rows(i).Cells(5).Value = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Sub UpdateSelectedCount()
        Dim selectedCount As Integer = 0
        ' عدّ الصفوف المحددة
        For Each row As DataGridViewRow In StudentGrid.Rows
            If Not row.IsNewRow AndAlso Convert.ToBoolean(row.Cells("Column2").Value) = True Then
                selectedCount += 1
            End If
        Next
        ' عرض النتيجة
        TxtSelectedCount.Text = selectedCount
    End Sub

    Private Sub StudentGrid_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles StudentGrid.CurrentCellDirtyStateChanged
        ' تحديث القيمة فور تغيير المستخدم للحالة
        If StudentGrid.CurrentCell IsNot Nothing AndAlso TypeOf StudentGrid.CurrentCell Is DataGridViewCheckBoxCell Then
            StudentGrid.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub
    Private Function GetSelectedRowCount() As Integer
        Dim count As Integer = 0
        For Each row As DataGridViewRow In StudentGrid.Rows
            If Not row.IsNewRow AndAlso Convert.ToBoolean(row.Cells("Column2").Value) = True Then
                count += 1
            End If
        Next
        Return count
    End Function
    Private Sub StudentGrid_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles StudentGrid.CellValueChanged
        If StudentGrid.Rows.Count = 0 OrElse e.RowIndex < 0 Then Exit Sub

        If e.ColumnIndex = StudentGrid.Columns("Column2").Index Then
            ' اختبار العدد المحدد
            Dim currentSelectedCount As Integer = GetSelectedRowCount()

            Dim allowedCount As Integer
            If Not Integer.TryParse(TxtClassCount.Text, allowedCount) OrElse allowedCount <= 0 Then
                allowedCount = 0
            End If

            'If currentSelectedCount > allowedCount Then
            '    MessageBox.Show("لا يمكن اختيار طلاب اكثر من المسموح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    ' إلغاء التحديد الحالي لمنع الزيادة
            '    StudentGrid.Rows(e.RowIndex).Cells("Column2").Value = False
            'End If

            UpdateSelectedCount()
        End If
    End Sub

    Private Sub EncodingClass_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("addclass")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdClass_Click(sender As Object, e As EventArgs) Handles CmdClass.Click
        Try
            ' قراءة عدد الصفوف المراد تحديدها
            Dim rowCount As Integer

            If Integer.TryParse(TxtClassCount.Text, rowCount) AndAlso rowCount > 0 Then
                ' إلغاء تحديد جميع الصفوف أولاً
                For Each row As DataGridViewRow In StudentGrid.Rows
                    row.Cells("Column2").Value = False
                Next

                ' تحديد الصفوف المتاحة فقط
                Dim maxSelectableRows As Integer = Math.Min(rowCount, StudentGrid.Rows.Count)

                For i As Integer = 0 To maxSelectableRows - 1
                    StudentGrid.Rows(i).Cells("Column2").Value = True
                Next

                UpdateSelectedCount()
            Else
                MessageBox.Show("يرجى إدخال رقم صحيح أكبر من صفر.")
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub ADDCLASS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddClass", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        ' إضافة معامل إخراج لاستلام @Code
        Dim outputParam As New SqlParameter("@CodeOut", SqlDbType.Float)
        outputParam.Direction = ParameterDirection.Output
        comm.Parameters.Add(outputParam)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        ' استرجاع القيمة الفعلية لمعامل الإخراج
        Dim RETURNCODE As Double = Convert.ToDouble(outputParam.Value)
        TxtCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWCLASSINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowClassInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowClassInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtGuid.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtCalssCode.Text = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub LINKSTUDENTWITHCLASS()
        Try
            Dim connection = dbcontext.Database.Connection
            Using comm = New SqlCommand("LinkStudentWithClass", connection)
                comm.CommandType = CommandType.StoredProcedure

                ' التأكد من أن الاتصال مغلق قبل الفتح
                If connection.State = ConnectionState.Closed Then
                    connection.Open()
                End If

                For i As Integer = 0 To StudentGrid.Rows.Count - 1
                    ' التحقق من أن الخلية تحتوي على قيمة
                    If StudentGrid.Rows(i).Cells(4).Value Is Nothing Then
                        Exit For
                    End If

                    ' التحقق مما إذا كان الصف محددًا للتحديث
                    If Convert.ToBoolean(StudentGrid.Rows(i).Cells(5).Value) = True Then
                        ' إضافة المعاملات مع تنظيفها قبل كل تنفيذ
                        comm.Parameters.Clear()
                        comm.Parameters.AddWithValue("@ClassGuid", TxtGuid.Text)
                        comm.Parameters.AddWithValue("@Guid", StudentGrid.Rows(i).Cells(4).Value.ToString())
                        comm.Parameters.AddWithValue("@IsReg", True)

                        ' تنفيذ الاستعلام
                        comm.ExecuteNonQuery()
                    End If
                Next
            End Using
        Catch ex As Exception
            MessageBox.Show("حدث خطأ: " & ex.Message)
        End Try
    End Sub
    Sub ADD_USERACTION_()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("Add_UserAction", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ActionGuid", TxtGuid.Text)
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
    Function VALIDATE_DATA() As Boolean
        If TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtDiplomGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار التخصص اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtLevelGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار المستوي اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtBatchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الدفعه اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtSelectedCount.Text = 0 Then
            MessageBox.Show("لا يمكن الحفظ بدون تحديد طلاب تابعين للشعبة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub FORSAVE()
        If AllFiles.FORSAVE("addclass") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDCLASS()
            SHOWCLASSINFO()
            LINKSTUDENTWITHCLASS()
            ADD_USERACTION_()
            MessageBox.Show("تم أضافة الشعبة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub CmdSaveClass_Click(sender As Object, e As EventArgs) Handles CmdSaveClass.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtGuid.Text = Nothing Then
                    FORSAVE()
                    Dim result As DialogResult = MessageBox.Show("هل تريد انشاء شعبة جديدة لنفس البيانات؟", "تنبيه", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If result = DialogResult.Yes Then
                        TxtCode.Text = Nothing
                        TxtCalssCode.Text = Nothing
                        TxtGuid.Text = Nothing
                        TxtSelectedCount.Text = 0
                        LOADSTUDYDETAILS()
                    Else
                        NEW__()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW__()
        TxtBranchGuid.Text = Nothing
        TxtBranchName.Text = Nothing
        TxtDiplomName.Text = Nothing
        TxtDiplomGuid.Text = Nothing
        TxtLevelName.Text = Nothing
        TxtLevelGuid.Text = Nothing
        TxtBatchName.Text = Nothing
        TxtBatchGuid.Text = Nothing
        StudentGrid.Rows.Clear()
        TxtSelectedCount.Text = 0
        TxtCount.Text = 0
        TxtCode.Text = Nothing
        TxtCalssCode.Text = Nothing
        TxtGuid.Text = Nothing
    End Sub
    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW__()
            EncodingClass_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class