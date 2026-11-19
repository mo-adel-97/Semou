Imports System.Data.SqlClient

Public Class MainFindStudent
    Dim dbcontext As New SemouEntities
    Dim Olddbcontext As New SSTLI2023Entities
    Dim FOUND As Boolean = False
    Dim INVOICETYPEGUID As String
    Dim PAYTYPE As String
    Dim DocCode As String
    Dim DocName As String
    Dim DocGuid As String
    Private Sub CmdNewStudent_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub CmdDelBranch_Click(sender As Object, e As EventArgs) Handles CmdDelBranch.Click
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        LOADSTUEDENT()
    End Sub

    Private Sub CmdDelStaut_Click(sender As Object, e As EventArgs) Handles CmdDelStaut.Click
        TxtCaseName.Text = Nothing
        TxtCaseGuid.Text = Nothing
        LOADSTUEDENT()
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            Dim w As New NewBranchList
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtBranchGuid.Text <> Nothing Then
                LOADSTUEDENT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindCase_Click(sender As Object, e As EventArgs) Handles CmdFindCase.Click
        Try
            Dim w As New StautList
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCaseName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtCaseGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtCaseGuid.Text <> Nothing Then
                LOADSTUEDENT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub LOADSTUEDENT()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("MainFindStudent", connection)
        If TxtFind.Text <> Nothing Then
            If FindByNaId.Checked = True Then
                comm.Parameters.AddWithValue("@NationalId", TxtFind.Text)
            ElseIf FindByTel.Checked = True Then
                comm.Parameters.AddWithValue("@StudentTel", TxtFind.Text)
            ElseIf FindByName.Checked = True Then
                comm.Parameters.AddWithValue("@StudentName", TxtFind.Text)
            End If
        Else
            BranchGrid.Rows.Clear()
        End If
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BarnchGuid", TxtBranchGuid.Text)
        End If
        If TxtCaseGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@RegCaseGuid", TxtCaseGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "MainFindStudent")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            TxtCount.Text = BranchGrid.Rows.Count()
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                BranchGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                BranchGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                BranchGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                BranchGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                BranchGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                BranchGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(10).ToString()
                BranchGrid.Rows(COUNT__).Cells(11).Value = ds.Tables(0).Rows(i)(11).ToString()
                BranchGrid.Rows(COUNT__).Cells(12).Value = ds.Tables(0).Rows(i)(19).ToString()
                BranchGrid.Rows(COUNT__).Cells(14).Value = ds.Tables(0).Rows(i)(17).ToString()
                BranchGrid.Rows(COUNT__).Cells(16).Value = ds.Tables(0).Rows(i)(18).ToString()
                BranchGrid.Rows(COUNT__).Cells(17).Value = ds.Tables(0).Rows(i)(12).ToString()
                BranchGrid.Rows(COUNT__).Cells(18).Value = ds.Tables(0).Rows(i)(13).ToString()
                BranchGrid.Rows(COUNT__).Cells(21).Value = ds.Tables(0).Rows(i)(14).ToString()
                BranchGrid.Rows(COUNT__).Cells(22).Value = ds.Tables(0).Rows(i)(15).ToString()
                BranchGrid.Rows(COUNT__).Cells(23).Value = ds.Tables(0).Rows(i)(16).ToString()
                BranchGrid.Rows(COUNT__).Cells(26).Value = ds.Tables(0).Rows(i)(20).ToString()
                If (IsDBNull(ds.Tables(0).Rows(i)(21))) Then
                    BranchGrid.Rows(COUNT__).Cells(27).Value = True
                Else
                    BranchGrid.Rows(COUNT__).Cells(27).Value = ds.Tables(0).Rows(i)(21).ToString()
                End If
                BranchGrid.Rows(COUNT__).Cells(28).Value = ds.Tables(0).Rows(i)(22).ToString()
                BranchGrid.Rows(COUNT__).Cells(29).Value = ds.Tables(0).Rows(i)(23).ToString()
                COUNT__ = COUNT__ + 1

            Next
        Else
            TxtCount.Text = 0
            FOUND = False
            TxtCheck.Text = 0
            If TxtCheck.Text = 0 And TxtFind.TextLength = 10 Then
                BranchGrid.Visible = False
                OldDataGrid.Visible = True
                OldDataGrid.Dock = DockStyle.Fill
                LOADOLDSTUEDENT()
            End If
        End If
    End Sub
    Sub LOADOLDSTUEDENT()
        OldDataGrid.Rows.Clear()
        Dim connection = Olddbcontext.Database.Connection
        Dim comm As New SqlCommand("FindStudentData", connection)
        If TxtFind.Text <> Nothing Then
            If FindByNaId.Checked = True Then
                comm.Parameters.AddWithValue("@PROOFNO", TxtFind.Text)
            ElseIf FindByTel.Checked = True Then
                comm.Parameters.AddWithValue("@CUSTMOBILE", TxtFind.Text)
            ElseIf FindByName.Checked = True Then
                comm.Parameters.AddWithValue("@StudentName", TxtFind.Text)
            End If
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "FindStudentData")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            OldDataGrid.RowCount = ds.Tables(0).Rows.Count
            TxtCount.Text = OldDataGrid.Rows.Count()
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                OldDataGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                OldDataGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                OldDataGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                OldDataGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                OldDataGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                OldDataGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                OldDataGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                OldDataGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(7).ToString()
                COUNT__ = COUNT__ + 1
            Next
        Else
            TxtCount.Text = 0
            FOUND = False
        End If
    End Sub
    Private Sub MainFindStudent_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'LOADSTUEDENT()
    End Sub

    Private Sub TxtFind_TextChanged(sender As Object, e As EventArgs) Handles TxtFind.TextChanged
        Try
            'If FindByNaId.Checked = True Or FindByTel.Checked = True Then
            If TxtFind.TextLength = 10 Then
                LOADSTUEDENT()
            End If
            'ElseIf FindByName.Checked = True Then
            'If TxtFind.TextLength = 5 Then
            'LOADSTUEDENT()
            'End If
            'End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            If AllFiles.CHEKPARENTINFO("10") = False Then
                Return
            Else
                Dim w As New EncodingStudent
                w.ShowDialog()
                If w.IS_Yes = True Then
                    FindByNaId.Checked = True
                    TxtFind.Text = w.TxtNationalId.Text
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub BranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellContentClick

    End Sub

    Private Sub BranchGrid_CancelRowEdit(sender As Object, e As QuestionEventArgs) Handles BranchGrid.CancelRowEdit

    End Sub
    Sub GETAUTODOCINFO(TYPEGUID As String, BRANCHGUID As String, TYPE As String)
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("AutoGetInvoiceType", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@DocTypeGuid", TYPEGUID)
        comm.Parameters.AddWithValue("@BranchGuid", BRANCHGUID)
        comm.Parameters.AddWithValue("@InvoiceType", TYPE)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "AutoGetInvoiceType")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DocCode = ds.Tables(0).Rows(i)(0).ToString()
                DocName = ds.Tables(0).Rows(i)(1).ToString()
                DocGuid = ds.Tables(0).Rows(i)(2).ToString()
            Next
        Else
            MessageBox.Show("برجاء مراجعه مدير النظام لا يتوفر دفتر للسداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
    End Sub
    Private Sub BranchGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellClick
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            If j = 12 Then
                If AllFiles.FORFIND("addstudent") = False Then
                    Return
                Else
                    Dim w As New EncodingStudent
                    w.TxtCode.Text = BranchGrid.Rows(i).Cells(9).Value.ToString
                    w.SHOWSTUDENTINFO()
                    w.SHOWACCOUNTINFO()
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        FindByNaId.Checked = True
                        TxtFind.Text = w.TxtNationalId.Text
                    End If
                End If
            ElseIf j = 14 Then
                If AllFiles.FORUPDATE("addstudent") = False Then
                    Return
                Else
                    Dim w As New ChangeStaut
                    w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                    w.TxtRegType.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                    w.TxtGuid.Text = BranchGrid.Rows(i).Cells(8).Value.ToString
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        FindByNaId.Checked = True
                        TxtFind.Text = w.TxtRegType.Text
                    End If
                End If
            ElseIf j = 13 Then
                If BranchGrid.Rows(i).Cells(23).Value = False Then
                    MessageBox.Show("الطالب موقوف عن التعامل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If BranchGrid.Rows(i).Cells(28).Value = True Then
                        MessageBox.Show("ملف الطالب مغلق برجاء التواصل مع الادارة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        If AllFiles.FORSAVE("addregdoc") = False Then
                            Return
                        Else
                            Dim what As New WhatRegTypeNeed
                            If BranchGrid.Rows(i).Cells(17).Value.ToString = Nothing Then
                                Dim result As DialogResult = MessageBox.Show("لا يتوفر فرع دراسة مسجل فيه الطالب" & vbCrLf &
                                                                         "هل تريد تسجيل الطالب بنفس فرع التسجيل" & vbCrLf &
                                                                         "OK -- تسجيل دراسة الطالب بنفس فرع التسجيل " & vbCrLf &
                                                                          BranchGrid.Rows(i).Cells(14).Value.ToString & vbCrLf &
                                                                         "Cancel -- يجب اختيار فرع لكي تتمكن من التسجيل",
                                                                         "تنبيه", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                                If result = DialogResult.OK Then
                                    what.CmdFindDepart.Enabled = False
                                    what.TxtBranchName.Text = BranchGrid.Rows(i).Cells(14).Value.ToString
                                    what.TxtBranchGuid.Text = BranchGrid.Rows(i).Cells(16).Value.ToString
                                Else
                                    what.CmdFindDepart.Enabled = True
                                End If
                            Else
                                what.CmdFindDepart.Enabled = True
                                what.TxtBranchName.Text = BranchGrid.Rows(i).Cells(4).Value.ToString
                                what.TxtBranchGuid.Text = BranchGrid.Rows(i).Cells(17).Value.ToString
                            End If
                            what.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                            what.ShowDialog()
                            If what.IS_Yes = True Then
                                Dim w As New RegisterForm
                                w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                                w.TxtTel.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                                w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                                w.TxtStudentGuid.Text = BranchGrid.Rows(i).Cells(8).Value.ToString
                                w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                                w.TxtDocName.Text = what.TxtDocName.Text
                                w.TxtDocGuid.Text = what.TxtDocGuid.Text
                                w.TxtDocCode.Text = what.TxtDocCode.Text
                                w.TxtstudentNational.Text = BranchGrid.Rows(i).Cells(22).Value.ToString
                                w.TxtEmail.Text = BranchGrid.Rows(i).Cells(26).Value.ToString
                                w.TxtRegType.Text = what.CboType.SelectedIndex
                                w.TxtTypeGuid.Text = what.TxtTypeGuid.Text
                                w.ChkContract.Checked = what.ChkContract.Checked
                                w.CHKWHATREGTYPE()
                                w.ShowDialog()
                                If w.IS_Yes = True Then
                                    FindByNaId.Checked = True
                                    TxtFind.Text = w.TxtNationalId.Text
                                End If
                            End If
                        End If
                    End If
                End If
            ElseIf j = 15 Then
                If BranchGrid.Rows(i).Cells(23).Value = False Then
                    MessageBox.Show("الطالب موقوف عن التعامل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If BranchGrid.Rows(i).Cells(28).Value = True Then
                        MessageBox.Show("ملف الطالب مغلق برجاء التواصل مع الادارة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else   '''''''''''''''''''''''''''''''''''''''''''''''''''''''' الجزء الخاص اذا كان الطالب خريج واستلم الشهادة
                        If BranchGrid.Rows(i).Cells(27).Value = False Then
                            MessageBox.Show("الطالب انهي الدراسة فى هذا الملف التدريبي " & vbCrLf &
                                        "ولذلك لن تتمكن من اجراء اي عملية سداد لهذا الملف ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Else
                            If AllFiles.FORSAVE("salesinvoice") = False Then
                                Return
                            Else
                                If BranchGrid.Rows(i).Cells(11).Value.ToString <> Nothing Then
                                    If AllFiles.ASKLASTBILLINFO(BranchGrid.Rows(i).Cells(10).Value) = True Then
                                        MessageBox.Show("اخر عميلة سداد لحساب الطالب" & vbCrLf &
                                                    "التاريخ" & " " & AllFiles.BILLDATE & vbCrLf &
                                                    "رقم المرجع" & " " & AllFiles.BILLREF & vbCrLf &
                                                    "الخزينة/البنك" & " " & AllFiles.CASHNAME & vbCrLf &
                                                    "المحصل" & " " & AllFiles.PAYMAN,
                                  "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                    End If
                                    RegDocPosted.WHATFORMTYPEGUID("salesinvoice")
                                    INVOICETYPEGUID = RegDocPosted.TYPEGUID
                                    PAYTYPE = 1
                                    GETAUTODOCINFO(INVOICETYPEGUID, BranchGrid.Rows(i).Cells(17).Value.ToString, PAYTYPE)
                                    Dim w As New EncodingNewPayInvoice
                                    w.TxtPayDocName.Text = DocName.ToString()
                                    w.TxtPayDocGuid.Text = DocGuid.ToString()
                                    w.TxtPayDocCode.Text = DocCode.ToString()
                                    w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                                    w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                                    w.TxtTel.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                                    w.TxtStudentNational.Text = BranchGrid.Rows(i).Cells(22).Value.ToString
                                    w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                                    w.TxtDiplomName.Text = BranchGrid.Rows(i).Cells(7).Value.ToString
                                    'w.TxtRegDocGuid.Text = BranchGrid.Rows(i).Cells(18).Value.ToString
                                    w.SHOWDIPLOMFROMSTUDENTLEVEL(BranchGrid.Rows(i).Cells(11).Value.ToString)
                                    w.TxtTypeGuid.Text = INVOICETYPEGUID.ToString()
                                    w.CHKACCOUNT()
                                    'w.SHOWREGDOCDIPLOM(0)
                                    w.ShowDialog()
                                Else

                                    MessageBox.Show("لن تتمكن من اتمام عملية سداد للطالب " & vbCrLf &
                                                " فى حالة ان طالب له استمارة تسجيل" & vbCrLf &
                                                 "برجاء الطلب من مشرف الفرع قبول التسجيل" & vbCrLf &
                                                 "في حالة ان الطالب لا يوجد له اى استمارة تسجيل" & vbCrLf &
                                                 "برجاء تسجيل استمارة للطالب اولا" & vbCrLf &
                                                 "-----------------------------" & vbCrLf &
                                                 "لكي تتمكن من عرض الاستمارت المسجلة للطالب من خلال كشف الحساب",
                                             "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Return
                                End If
                            End If
                        End If
                    End If
                End If
            ElseIf j = 16 Then
                If AllFiles.FORFIND("studentstatment") = False Then
                    Return
                Else
                    Dim w As New StusdentStatment
                    w.TxtId.Text = BranchGrid.Rows(i).Cells(0).Value.ToString
                    w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                    w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                    w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                    w.ShowDialog()
                End If
            ElseIf j = 19 Then
                If BranchGrid.Rows(i).Cells(23).Value = False Then
                    MessageBox.Show("الطالب موقوف عن التعامل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If BranchGrid.Rows(i).Cells(28).Value = True Then
                        MessageBox.Show("ملف الطالب مغلق برجاء التواصل مع الادارة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        If BranchGrid.Rows(i).Cells(27).Value = False Then
                            MessageBox.Show("الطالب انهي الدراسة فى هذا الملف التدريبي " & vbCrLf &
                                            "ولذلك لن تتمكن من أضافة استمارة رسوم لهذا الملف ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Else
                            If BranchGrid.Rows(i).Cells(11).Value.ToString <> Nothing Then
                                Dim w As New RegFessForm
                                w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                                w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                                w.TxtTel.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                                w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                                w.ShowDialog()
                            Else
                                AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
                                If AllFiles.NOFILEADDFESS = True Then
                                    Dim w As New RegFessForm
                                    w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                                    w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                                    w.TxtTel.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                                    w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                                    w.ShowDialog()
                                Else
                                    MessageBox.Show("لا تتوفر اى بيانات تدريبية للطالب " & vbCrLf &
                                                     " فى حالة ان طالب له استمارة تسجيل" & vbCrLf &
                                                     "برجاء الطلب من مشرف الفرع قبول التسجيل" & vbCrLf &
                                                     "في حالة ان الطالب لا يوجد له اى استمارة تسجيل" & vbCrLf &
                                                     "برجاء تسجيل استمارة للطالب اولا" & vbCrLf &
                                                     "-----------------------------" & vbCrLf &
                                                     "لكي تتمكن من عرض الاستمارت المسجلة للطالب من خلال كشف الحساب",
                                                     "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Return
                                End If
                            End If
                        End If
                    End If
                End If
            ElseIf j = 20 Then
                If BranchGrid.Rows(i).Cells(23).Value = False Then
                    MessageBox.Show("الطالب موقوف عن التعامل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If BranchGrid.Rows(i).Cells(28).Value = True Then
                        MessageBox.Show("ملف الطالب مغلق برجاء التواصل مع الادارة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        If BranchGrid.Rows(i).Cells(27).Value = False Then
                            MessageBox.Show("الطالب انهي الدراسة فى هذا الملف التدريبي " & vbCrLf &
                                        "ولذلك لن تتمكن من اجراء اي عملية سداد رسوم لهذا الملف ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Else
                            If BranchGrid.Rows(i).Cells(11).Value.ToString <> Nothing Then
                                RegDocPosted.WHATFORMTYPEGUID("salesinvoice")
                                INVOICETYPEGUID = RegDocPosted.TYPEGUID
                                PAYTYPE = 2
                                GETAUTODOCINFO(INVOICETYPEGUID, BranchGrid.Rows(i).Cells(17).Value.ToString, PAYTYPE)
                                Dim w As New EncodingNewPayInvoice
                                w.TxtPayDocName.Text = DocName.ToString()
                                w.TxtPayDocGuid.Text = DocGuid.ToString()
                                w.TxtPayDocCode.Text = DocCode.ToString()
                                w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                                w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                                w.TxtTel.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                                w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                                If BranchGrid.Rows(i).Cells(11).Value.ToString <> Nothing Then
                                    w.TxtDiplomName.Text = BranchGrid.Rows(i).Cells(7).Value.ToString
                                    w.TxtRegDocGuid.Text = BranchGrid.Rows(i).Cells(18).Value.ToString
                                End If
                                w.SHOWREGDOCDIPLOM(1)
                                w.TxtTypeGuid.Text = INVOICETYPEGUID.ToString()
                                w.CHKACCOUNT()
                                w.ShowDialog()
                            Else
                                AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
                                If AllFiles.NOFILEADDFESS = False Then
                                    MessageBox.Show("لا تتوفر اى بيانات تدريبية للطالب " & vbCrLf &
                                            " فى حالة ان طالب له استمارة تسجيل" & vbCrLf &
                                            "برجاء الطلب من مشرف الفرع قبول التسجيل" & vbCrLf &
                                            "في حالة ان الطالب لا يوجد له اى استمارة تسجيل" & vbCrLf &
                                            "برجاء تسجيل استمارة للطالب اولا" & vbCrLf &
                                            "-----------------------------" & vbCrLf &
                                            "لكي تتمكن من عرض الاستمارت المسجلة للطالب من خلال كشف الحساب",
                                            "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Return
                                Else
                                    Dim w As New EncodingNewPayInvoice
                                    w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                                    w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                                    w.TxtTel.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                                    w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                                    If BranchGrid.Rows(i).Cells(11).Value.ToString <> Nothing Then
                                        w.TxtDiplomName.Text = BranchGrid.Rows(i).Cells(7).Value.ToString
                                        w.TxtRegDocGuid.Text = BranchGrid.Rows(i).Cells(18).Value.ToString
                                    End If
                                    w.SHOWREGDOCDIPLOM(1)
                                    w.ShowDialog()
                                End If
                            End If
                        End If
                    End If
                End If
            ElseIf j = 24 Then
                If BranchGrid.Rows(i).Cells(28).Value = True Then
                    MessageBox.Show("ملف الطالب مغلق برجاء التواصل مع الادارة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If BranchGrid.Rows(i).Cells(11).Value.ToString <> Nothing Then
                        Dim w As New StudyInfo_Student
                        w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                        w.TxtTel.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                        w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                        w.TxtBranchName.Text = BranchGrid.Rows(i).Cells(4).Value.ToString
                        w.TxtDiplomName.Text = BranchGrid.Rows(i).Cells(7).Value.ToString
                        w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                        w.TxtDiplomGuid.Text = BranchGrid.Rows(i).Cells(11).Value.ToString
                        w.TxtBranchGuid.Text = BranchGrid.Rows(i).Cells(17).Value.ToString
                        w.LOADINFO()
                        w.ShowDialog()
                    Else
                        MessageBox.Show("لا تتوفر اى بيانات تدريبية للطالب " & vbCrLf &
                                    " فى حالة ان طالب له استمارة تسجيل" & vbCrLf &
                                    "برجاء الطلب من مشرف الفرع قبول التسجيل" & vbCrLf &
                                    "في حالة ان الطالب لا يوجد له اى استمارة تسجيل" & vbCrLf &
                                    "برجاء تسجيل استمارة للطالب اولا" & vbCrLf &
                                    "-----------------------------" & vbCrLf &
                                    "لكي تتمكن من عرض الاستمارت المسجلة للطالب من خلال كشف الحساب",
                                    "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                End If
            ElseIf j = 25 Then
                Dim w As New DocHistory
                w.TxtActionGuid.Text = BranchGrid.Rows(i).Cells(8).Value.ToString()
                w.ShowDialog()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub OldDataGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles OldDataGrid.CellContentClick

    End Sub
    Sub UPDATESTUEDNT2(NATIONALID As String)
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdatStudent2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@NationalId", NATIONALID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATESTUEDNT3(NATIONALID As String, BRANCHCODE As Integer)
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateStudent3", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@NationalId", NATIONALID)
        comm.Parameters.AddWithValue("@BranchCode", BRANCHCODE)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub OldDataGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles OldDataGrid.CellClick
        Try
            Dim i As Integer = OldDataGrid.CurrentCell.RowIndex
            Dim j As Integer = OldDataGrid.CurrentCell.ColumnIndex
            If j = 7 Then
                'If Val(OldDataGrid.Rows(i).Cells(8).Value) <> 0 Then
                Dim w As New EncodingStudent
                w.TxtStudentName.Text = OldDataGrid.Rows(i).Cells(0).Value.ToString
                w.TxtTel.Text = OldDataGrid.Rows(i).Cells(1).Value.ToString
                w.TxtNationalId.Text = OldDataGrid.Rows(i).Cells(2).Value.ToString
                w.TxtNotes.Text = OldDataGrid.Rows(i).Cells(4).Value.ToString
                w.TxtOldCustNo.Text = OldDataGrid.Rows(i).Cells(6).Value.ToString
                '''''' student2  الدبلومات
                If AllFiles.GETREALSTUDENTINFO(OldDataGrid.Rows(i).Cells(2).Value.ToString) = True Then
                    w.TxtBranchId.Text = AllFiles.REALSTUDENTBRANCHCODE.ToString()
                    If AllFiles.REALSTUDENTBRANCHCODE <> OldDataGrid.Rows(i).Cells(5).Value Then
                        w.SHOWBALANCE(OldDataGrid.Rows(i).Cells(5).Value.ToString())

                    Else
                        w.SHOWBALANCE(AllFiles.REALSTUDENTBRANCHCODE.ToString())
                    End If
                    w.TxtIsFound.Text = 1
                    w.TxtLevelGuid.Text = AllFiles.REALSTUDENTLEVELGUID.ToString()
                    w.TxtBatchGuid.Text = AllFiles.REALSTUDENTBATCHGUID.ToString()
                    w.TxtDiplomGuid.Text = AllFiles.REALSTUDENTDIPLOMGUID.ToString()
                    w.TxtCaseName.Text = AllFiles.REALSTUDENTCASENAME.ToString()
                    w.TxtCaseGuid.Text = AllFiles.REALSTUDENTCASEGUID.ToString()
                    w.CmdFindCase.Enabled = False
                    w.CmdFindDepart.Enabled = False
                    w.SHOWBRANCHINFO()
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        UPDATESTUEDNT2(w.TxtNationalId.Text)
                        BranchGrid.Visible = True
                        OldDataGrid.Visible = False
                        FindByNaId.Checked = True
                        TxtFind.Text = w.TxtNationalId.Text
                        LOADSTUEDENT()
                    End If
                    ''''''' student3 الدورات
                ElseIf AllFiles.GETROTHEREALSTUDENTINFO(OldDataGrid.Rows(i).Cells(2).Value.ToString, OldDataGrid.Rows(i).Cells(5).Value.ToString) = True Then
                    w.TxtCourse.Text = 1
                    Dim ww As New CourseStudentInfo
                    ww.TxtNationalId.Text = OldDataGrid.Rows(i).Cells(2).Value.ToString
                    ww.TxtStudentName.Text = OldDataGrid.Rows(i).Cells(0).Value.ToString
                    ww.TxtBranchCode.Text = OldDataGrid.Rows(i).Cells(5).Value.ToString
                    ww.ShowDialog()
                    If ww.IS_YES = True Then
                        w.TxtBranchId.Text = ww.TxtBranchCode.Text
                        w.TxtCaseGuid.Text = ww.TxtCaseGuid.Text
                        w.TxtCourseGuid.Text = ww.TxtCourseGuid.Text
                        w.TxtCaseName.Text = ww.TxtCaseName.Text
                        w.TxtDiplomGuid.Text = ww.TxtDiplomGuid.Text
                        w.CmdFindCase.Enabled = False
                        w.CmdFindDepart.Enabled = False
                        w.SHOWBRANCHINFO()
                        w.SHOWBALANCE(w.TxtBranchId.Text)
                        w.ShowDialog()
                        If w.IS_Yes = True Then
                            UPDATESTUEDNT3(w.TxtNationalId.Text, w.TxtBranchId.Text)
                            BranchGrid.Visible = True
                            OldDataGrid.Visible = False
                            FindByNaId.Checked = True
                            TxtFind.Text = w.TxtNationalId.Text
                            LOADSTUEDENT()
                        End If
                    End If
                    '''''''''غير موجود في برنامج الطلبه
                ElseIf AllFiles.GETREALSTUDENTINFO(OldDataGrid.Rows(i).Cells(2).Value.ToString) = False And
                        AllFiles.GETROTHEREALSTUDENTINFO(OldDataGrid.Rows(i).Cells(2).Value.ToString, OldDataGrid.Rows(i).Cells(5).Value.ToString) = False Then
                    Dim xx As New IfStudentNotRegister
                    xx.TxtNotes.Text = OldDataGrid.Rows(i).Cells(4).Value.ToString
                    xx.ShowDialog()
                    If xx.IS_Yes = True Then
                        If xx.CboType.SelectedIndex = 0 Then
                            w.TxtIsFound.Text = 1
                            w.TxtBranchId.Text = xx.TxtBranchId.Text
                            w.TxtLevelGuid.Text = xx.TxtLevelGuid.Text
                            w.TxtBatchGuid.Text = xx.TxtBatchGuid.Text
                            w.TxtDiplomGuid.Text = xx.TxtDiplomGuid.Text
                            w.TxtCaseName.Text = xx.TxtCaseName.Text
                            w.TxtCaseGuid.Text = xx.TxtCaseGuid.Text
                            w.CmdFindCase.Enabled = False
                            w.CmdFindDepart.Enabled = False
                            w.SHOWBALANCE(OldDataGrid.Rows(i).Cells(5).Value.ToString())
                        ElseIf xx.CboType.SelectedIndex = 1 Then
                            w.TxtCourse.Text = 1
                            w.TxtBranchId.Text = xx.TxtBranchId.Text
                            w.TxtCaseGuid.Text = xx.TxtCaseGuid.Text
                            w.TxtCourseGuid.Text = xx.TxtCourseGuid.Text
                            w.TxtCaseName.Text = xx.TxtCaseName.Text
                            w.TxtDiplomGuid.Text = xx.TxtCourseDiplomGuid.Text
                            w.CmdFindCase.Enabled = False
                            w.CmdFindDepart.Enabled = False
                            w.SHOWBALANCE(w.TxtBranchId.Text)
                        End If
                        w.SHOWBRANCHINFO()
                        w.ShowDialog()
                        If w.IS_Yes = True Then
                            FindByNaId.Checked = True
                            TxtFind.Text = w.TxtNationalId.Text
                            LOADSTUEDENT()
                        End If
                    End If
                End If
                'Else
                '    MessageBox.Show("لا توجد اي مبالغ مستحقة على الطالب ولذلك سوف يتم الاحتفاظ بالبيانات فى الارشيف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                '    Return
                'End If
            ElseIf j = 9 Then
                Dim w As New OldStudentStatment
                w.TxtStudentName.Text = OldDataGrid.Rows(i).Cells(0).Value.ToString
                w.LOADOLDSTATMENT(OldDataGrid.Rows(i).Cells(6).Value.ToString, OldDataGrid.Rows(i).Cells(5).Value.ToString)
                w.LOADOLDREG(OldDataGrid.Rows(i).Cells(6).Value.ToString, OldDataGrid.Rows(i).Cells(5).Value.ToString)
                w.LOADOPENBALANCE(OldDataGrid.Rows(i).Cells(6).Value.ToString, OldDataGrid.Rows(i).Cells(5).Value.ToString)
                w.LASTTOTAL()
                w.ShowDialog()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdRefresh_Click(sender As Object, e As EventArgs) Handles CmdRefresh.Click
        Try
            TxtBranchGuid.Text = Nothing
            TxtBranchName.Text = Nothing
            TxtCaseGuid.Text = Nothing
            TxtCaseName.Text = Nothing
            TxtFind.Text = Nothing
            TxtCount.Text = 0
            TxtCheck.Text = Nothing
            BranchGrid.Visible = True
            OldDataGrid.Visible = False
            BranchGrid.Rows.Clear()
            OldDataGrid.Rows.Clear()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening

    End Sub

    Private Sub BranchGrid_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles BranchGrid.CellMouseClick
        Try
            If e.Button = System.Windows.Forms.MouseButtons.Right Then
                '  ContextMenuStrip1.Items(3).Visible = False
                ContextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChangeStaut_Click(sender As Object, e As EventArgs) Handles ChangeStaut.Click
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            If BranchGrid.Rows(i).Cells(28).Value = True Then
                MessageBox.Show("ملف الطالب مغلق برجاء التواصل مع الادارة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                Dim w As New ChangeStudentStaut
                w.TxtGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                w.ChkStaut.Value = BranchGrid.Rows(i).Cells(23).Value
                w.ShowDialog()
                If w.IS_Yes = True Then
                    LOADSTUEDENT()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ReRegister_Click(sender As Object, e As EventArgs) Handles ReRegister.Click
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            If BranchGrid.Rows(i).Cells(18).Value.ToString = Nothing Or
                    BranchGrid.Rows(i).Cells(18).Value.ToString = "00000000-0000-0000-0000-000000000000" Then
                MessageBox.Show("لا تتوفر استمارة للطالب لعمل مرتجع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If BranchGrid.Rows(i).Cells(28).Value = True Then
                    MessageBox.Show("ملف الطالب مغلق برجاء التواصل مع الادارة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    Dim REGDOC_GUID_ As Guid = Guid.Parse(BranchGrid.Rows(i).Cells(18).Value.ToString)
                    Dim dbcontext As New SemouEntities
                    Dim ASKIFREREG = dbcontext.RegDocs.FirstOrDefault(Function(m) m.Guid = REGDOC_GUID_)
                    If ASKIFREREG.IsReReg = True Then
                        MessageBox.Show("تم عمل استرجاع للاستمارة بالفعل" & vbCrLf &
                           "لعرض مرتجع الاستمارة بالرجاء الدخول على كشف الحساب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        Dim w As New Re_Register
                        w.TxtRegDocGuid.Text = BranchGrid.Rows(i).Cells(18).Value.ToString
                        w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                        w.TxtTel.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                        w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                        w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                        w.SHOWREGDOCINFO()
                        w.SHOWREGDOCDIPLOM()
                        w.DIPLOMTOTAL()
                        w.ShowDialog()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub RePay_Click(sender As Object, e As EventArgs) Handles RePay.Click
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            If AllFiles.ASKFORSTUDENTBILL(BranchGrid.Rows(i).Cells(10).Value.ToString) = False Then
                MessageBox.Show("لا تتوفر اى فواتير خاصة بالطالب لعمل ارتجاع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If BranchGrid.Rows(i).Cells(28).Value = True Then
                    MessageBox.Show("ملف الطالب مغلق برجاء التواصل مع الادارة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    Dim w As New StudentBillList
                    w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                        Dim BILLGUID As Guid = Guid.Parse(w.BranchGrid.Rows(ii).Cells(3).Value.ToString())
                        Dim dbcontext As New SemouEntities
                        Dim ASKIFREPAY = dbcontext.Bills.FirstOrDefault(Function(m) m.Guid = BILLGUID)
                        If ASKIFREPAY.IsRePay = True Then
                            MessageBox.Show("تم عمل استرجاع للفاتورة بالفعل" & vbCrLf &
                               "لعرض مرتجع الفاتورة بالرجاء الدخول على كشف الحساب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        Else
                            Dim wx As New RePay
                            wx.TxtRegDocCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                            wx.TxtRegDocGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                            wx.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                            wx.TxtTel.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                            wx.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                            wx.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                            wx.SHOWOLDBILLINFO()
                            wx.SHOWBILLDIPLOM(Guid.Parse(wx.TxtRegDocGuid.Text))
                            wx.PAYTOTAL()
                            wx.ShowDialog()
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ViewData_Click(sender As Object, e As EventArgs) Handles ViewData.Click
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            If BranchGrid.Rows(i).Cells(28).Value = True Then
                MessageBox.Show("ملف الطالب مغلق برجاء التواصل مع الادارة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If AllFiles.FORFIND("addstudent") = False Then
                    Return
                Else
                    Dim w As New EncodingStudent
                    w.TxtCode.Text = BranchGrid.Rows(i).Cells(9).Value.ToString
                    w.SHOWSTUDENTINFO()
                    w.SHOWACCOUNTINFO()
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        FindByNaId.Checked = True
                        TxtFind.Text = w.TxtNationalId.Text
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub Statment_Click(sender As Object, e As EventArgs) Handles Statment.Click
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            If BranchGrid.Rows(i).Cells(28).Value = True Then
                MessageBox.Show("ملف الطالب مغلق برجاء التواصل مع الادارة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If AllFiles.FORFIND("studentstatment") = False Then
                    Return
                Else
                    Dim w As New StusdentStatment
                    w.TxtId.Text = BranchGrid.Rows(i).Cells(0).Value.ToString
                    w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
                    w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
                    w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                    'w.TxtFind.Text = BranchGrid.Rows(i).Cells(7).Value.ToString
                    w.ShowDialog()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub STUDENTPROFILE(STAUT As Boolean)
        Dim i As Integer = BranchGrid.CurrentCell.RowIndex
        Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("LockStudentProfile", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@AccountGuid", BranchGrid.Rows(i).Cells(10).Value.ToString)
        comm.Parameters.AddWithValue("@LockProfile", STAUT)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub CloseFile_Click(sender As Object, e As EventArgs) Handles CloseFile.Click
        Try
            If AllFiles.FORSAVE("closeprofile") = False Then
                Return
            Else
                STUDENTPROFILE(True)
                MessageBox.Show("تم اغلاق ملف الطالب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub Reopenprofile_Click(sender As Object, e As EventArgs) Handles Reopenprofile.Click
        Try
            If AllFiles.FORUPDATE("closeprofile") = False Then
                Return
            Else
                STUDENTPROFILE(False)
                MessageBox.Show("تم اعادة فتح ملف الطالب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub OrderDis_Click(sender As Object, e As EventArgs) Handles OrderDis.Click
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            Dim w As New EncodingOrderDis
            w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
            w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
            w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
            w.TxtTel.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub AcceptOrder_Click(sender As Object, e As EventArgs) Handles AcceptOrder.Click
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            Dim w As New EncodingAcceptOrder
            w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(1).Value.ToString
            w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString
            w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
            w.TxtTel.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class