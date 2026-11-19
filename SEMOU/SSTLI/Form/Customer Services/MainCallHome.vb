Imports System.Data.SqlClient

Public Class MainCallHome
    Dim dbcontext As New SemouEntities
    Sub SHOWSTUDENTINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("FindStudent_CRM", connection)
        If TxtFind.Text <> Nothing Then
            If FindById.Checked = True Then
                comm.Parameters.AddWithValue("@NationalId", TxtFind.Text)
            ElseIf FindByTel.Checked = True Then
                comm.Parameters.AddWithValue("@StudentTel", TxtFind.Text)
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
        adp.Fill(ds, "FindStudent_CRM")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            TxtMssg.Visible = False
            CmdStudyFile.Enabled = True
            CmdStatment.Enabled = True
            CmdSaveStudent.Enabled = False
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtStudentName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtNationalId.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtAccountGuid.Text = ds.Tables(0).Rows(i)(3).ToString()
                CboType.SelectedIndex = ds.Tables(0).Rows(i)(4)
                CboNational.SelectedIndex = ds.Tables(0).Rows(i)(5)
                COUNT__ = COUNT__ + 1
            Next
        Else
            BranchGrid.Rows.Clear()
            TxtStudentName.Text = Nothing
            TxtNationalId.Text = Nothing
            TxtTel.Text = Nothing
            TxtAccountGuid.Text = Nothing
            TxtMssg.Visible = True
            CmdStudyFile.Enabled = False
            CmdStatment.Enabled = False
            CmdSaveStudent.Enabled = True
            CboType.Text = Nothing
            CboNational.Text = Nothing
        End If
    End Sub
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub TxtFind_TextChanged(sender As Object, e As EventArgs) Handles TxtFind.TextChanged

    End Sub

    Private Sub TxtFind_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtFind.KeyPress
        Try
            If e.KeyChar = Chr(13) Then
                SHOWSTUDENTINFO()
                If TxtAccountGuid.Text <> Nothing Then
                    LOADCALL()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdStatment_Click(sender As Object, e As EventArgs) Handles CmdStatment.Click
        Try
            Dim w As New StusdentStatment
            w.TxtStudentName.Text = TxtStudentName.Text
            w.TxtAccountGuid.Text = TxtAccountGuid.Text
            w.TxtNationalId.Text = TxtNationalId.Text
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdStudyFile_Click(sender As Object, e As EventArgs) Handles CmdStudyFile.Click
        Try
            If TxtAccountGuid.Text <> Nothing Then
                Dim w As New StudyInfoCrm
                w.TxtAccountGuid.Text = TxtAccountGuid.Text
                w.ShowDialog()
            Else
                MessageBox.Show("برجاء اختيار طالب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub ADDACCOUNT()
        AllFiles.GETACCOUNTCODE(Guid.Parse(TxtParentGuid.Text), TxtParentCode.Text)
        TxtAccountCode.Text = AllFiles.ACCOUNTCODE.ToString()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddBasicAccount", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BacicCode", TxtAccountCode.Text)
        comm.Parameters.AddWithValue("@Code", TxtAccountCode.Text)
        comm.Parameters.AddWithValue("@AccountName", TxtStudentName.Text)
        comm.Parameters.AddWithValue("@AccountKind", True)
        comm.Parameters.AddWithValue("@AccountType", 1)
        comm.Parameters.AddWithValue("@EndAccount", TxtEndAccount.Text)
        comm.Parameters.AddWithValue("@ParentGuid", TxtParentGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@IsShow", False)
        comm.Parameters.AddWithValue("@Type_", 2)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWACCOUNTINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowAccount", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtAccountCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowAccount")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtAccountGuid.Text = ds.Tables(0).Rows(i)(7).ToString()
            Next
        End If
    End Sub
    Sub CREATEOTHERID()
        Dim rnd As New Random
        Dim x As Integer
        x = rnd.Next
        TxtMainCode.Text = "S" + "" + x.ToString()
    End Sub
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.Students.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Student
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDSTUDENT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddStudent", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ID", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@AcadmyId", TxtMainCode.Text)
        comm.Parameters.AddWithValue("@StudentName", TxtStudentName.Text)
        comm.Parameters.AddWithValue("@StudentTel", TxtTel.Text)
        comm.Parameters.AddWithValue("@NationalId", TxtNationalId.Text)
        comm.Parameters.AddWithValue("@RegCaseGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@BarnchGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@Notes", "")
        comm.Parameters.AddWithValue("@NoData", False)
        comm.Parameters.AddWithValue("@Email", "")
        comm.Parameters.AddWithValue("@StudentType", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@StudentNational", CboNational.SelectedIndex)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtStudentName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtTel.Text = Nothing Then
            MessageBox.Show("برجاء ادخال رقم الجوال", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtTel.TextLength <> 10 Then
            MessageBox.Show("برجاء ادخال رقم جوال صحيح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtNationalId.Text = Nothing Then
            MessageBox.Show("برجاء ادخال رقم الهوية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtNationalId.TextLength <> 10 Then
            MessageBox.Show("برجاء ادخال رقم هوية صحيح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf CboType.Text = Nothing Then
            MessageBox.Show("برجاء تحديد نوع الطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf CboNational.Text = Nothing Then
            MessageBox.Show("برجاء تحديد جنسية الطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Private Sub CmdSaveStudent_Click(sender As Object, e As EventArgs) Handles CmdSaveStudent.Click
        Try
            AllFiles.GETPARENTINFOFROMSETTING("10")
            TxtParentAccount.Text = AllFiles.PARENTNAMEFORNEW.ToString()
            TxtParentGuid.Text = AllFiles.PARENTGUIDFORNEW.ToString()
            TxtParentCode.Text = AllFiles.PARENTCODEFORNEW.ToString()
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtAccountGuid.Text = Nothing Then
                    ADDACCOUNT()
                    SHOWACCOUNTINFO()
                    CREATEOTHERID()
                    ADDSTUDENT()
                    MessageBox.Show("تم حفظ بيانات الطالب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub LOADCALL()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowStudentCall", connection)
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowStudentCall")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(1)).ToShortDateString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                BranchGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdNewCall_Click(sender As Object, e As EventArgs) Handles CmdNewCall.Click
        Try
            If AllFiles.FORSAVE("newcall") = False Then
                Return
            Else
                If TxtAccountGuid.Text <> Nothing Then
                    Dim w As New NewCallType
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        Select Case w.CboTybe.SelectedIndex
                            Case 0
                                Dim CALL1 As New EncodingCall1
                                CALL1.CboTybe.SelectedIndex = w.CboTybe.SelectedIndex
                                CALL1.TxtAccountGuid.Text = TxtAccountGuid.Text
                                CALL1.ShowDialog()
                                If CALL1.IS_Yes = True Then
                                    LOADCALL()
                                End If
                            Case 1
                                Dim CALL2 As New EncodingCall2
                                CALL2.CboTybe.SelectedIndex = w.CboTybe.SelectedIndex
                                CALL2.TxtAccountGuid.Text = TxtAccountGuid.Text
                                CALL2.ShowDialog()
                                If CALL2.IS_Yes = True Then
                                    LOADCALL()
                                End If
                            Case 2
                                Dim CALL3 As New EncodingCall3
                                CALL3.CboTybe.SelectedIndex = w.CboTybe.SelectedIndex
                                CALL3.TxtAccountGuid.Text = TxtAccountGuid.Text
                                CALL3.ShowDialog()
                                If CALL3.IS_Yes = True Then
                                    LOADCALL()
                                End If
                        End Select
                    End If
                Else
                    MessageBox.Show("برجاء اختيار طالب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class