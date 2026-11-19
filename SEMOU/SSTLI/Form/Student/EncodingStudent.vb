Imports System.Data.SqlClient
Imports System.Text
Public Class EncodingStudent
    Dim dbcontext As New SemouEntities
    Dim Olddbcontext As New SSTLI2023Entities
    Public IS_Yes As Boolean = False
    Dim REASON As String = ""
    Dim NOTES As String
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Sub CREATEOTHERID()
        Dim rnd As New Random
        Dim x As Integer
        x = rnd.Next
        TxtMainCode.Text = "S" + "" + x.ToString()
    End Sub
    Private Sub EncodingStudent_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            AllFiles.GETPARENTINFOFROMSETTING("10")
            TxtParentAccount.Text = AllFiles.PARENTNAMEFORNEW.ToString()
            TxtParentGuid.Text = AllFiles.PARENTGUIDFORNEW.ToString()
            TxtParentCode.Text = AllFiles.PARENTCODEFORNEW.ToString()
            TxtOpenAccountGuid.Text = AllFiles.OPENACCOUNTGUID.ToString()
            RegDocPosted.WHATFORMTYPEGUID("opendaily")
            TxtOpenTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("addstudent")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            'If TxtCourse.Text = 1 Then
            '    Dim w As New CourseStudentInfo
            '    w.TxtNationalId.Text = TxtNationalId.Text
            '    w.TxtStudentName.Text = TxtStudentName.Text
            '    w.ShowDialog()
            '    If w.IS_YES = False Then
            '        MessageBox.Show("لم يتم تحديد اى بيانات", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        Me.Close()
            '    ElseIf w.IS_YES = True Then
            '        TxtBranchId.Text = w.TxtBranchId.Text
            '        SHOWBRANCHINFO()
            '        CmdFindDepart.Enabled = False
            '        TxtCaseName.Text = w.TxtCaseName.Text
            '        TxtCaseGuid.Text = w.TxtCaseGuid.Text
            '        CmdFindCase.Enabled = False
            '        TxtCourseGuid.Text = w.TxtCourseGuid.Text
            '        SHOWBALANCE(TxtBranchId.Text)
            '    End If
            'End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.TxtIsRegOn.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                If TxtCourse.Text = 1 Then
                    TxtBranchId.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                End If
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
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
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
        ElseIf TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الفرع القائم بتسجيل الطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCaseGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار حالة تسجيل الطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtEmail.Text = Nothing Then
            MessageBox.Show("برجاء ادخال الايميل الخاص بالطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf CboType.Text = Nothing Then
            MessageBox.Show("برجاء تحديد نوع الطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf CboNational.Text = Nothing Then
            MessageBox.Show("برجاء تحديد جنسية الطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf ChkNoDate.Checked = True Then
            If TxtNotes.Text = Nothing Then
                MessageBox.Show("برجاء ادخال بيانات المعهد الآخر المسجل به الطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
        End If
        Return True
    End Function
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
        comm.Parameters.AddWithValue("@Maden", TxtMaden.Text)
        comm.Parameters.AddWithValue("@Daen", TxtDaen.Text)
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
                TxtMaden.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtDaen.Text = ds.Tables(0).Rows(i)(9).ToString()
            Next
        End If
    End Sub
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
        comm.Parameters.AddWithValue("@RegCaseGuid", TxtCaseGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@BarnchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@NoData", ChkNoDate.Checked)
        comm.Parameters.AddWithValue("@Email", TxtEmail.Text)
        comm.Parameters.AddWithValue("@StudentType", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@StudentNational", CboNational.SelectedIndex)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWSTUDENTINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowStudentInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowStudentInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtMainCode.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtStudentName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtNationalId.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtCaseName.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtCaseGuid.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtAccountCode.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtAccountGuid.Text = ds.Tables(0).Rows(i)(9).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(10)
                TxtGuid.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(12).ToString()
                ChkNoDate.Checked = ds.Tables(0).Rows(i)(13)
                TxtEmail.Text = ds.Tables(0).Rows(i)(14).ToString()
                CboType.SelectedIndex = ds.Tables(0).Rows(i)(15)
                CboNational.SelectedIndex = ds.Tables(0).Rows(i)(16)
            Next
        End If
    End Sub
    Sub UPDATESTUDENT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateStudent", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@StudentName", TxtStudentName.Text)
        comm.Parameters.AddWithValue("@StudentTel", TxtTel.Text)
        comm.Parameters.AddWithValue("@NationalId", TxtNationalId.Text)
        comm.Parameters.AddWithValue("@RegCaseGuid", TxtCaseGuid.Text)
        comm.Parameters.AddWithValue("@BarnchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@NoData", ChkNoDate.Checked)
        comm.Parameters.AddWithValue("@Email", TxtEmail.Text)
        comm.Parameters.AddWithValue("@StudentType", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@StudentNational", CboNational.SelectedIndex)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEACOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateAccount", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@AccountName", TxtStudentName.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
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
    Public Function GETDAY1__NO() As Integer
        Dim TYPEGUID As Guid = Guid.Parse(TxtOpenTypeGuid.Text)
        Dim MaxId = dbcontext.Day1.Where(Function(m) m.TypeGuid = TYPEGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Day1
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtDay1Code.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtDay1Code.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddOpenDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        'comm.Parameters.AddWithValue("@Id", GETDAY1__NO)
        'comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@DayType", False)
        comm.Parameters.AddWithValue("@DocGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@TypeGuid", TxtOpenTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionCode", TxtCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
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
    Sub SHOWDAY1INFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDay1Info", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@TypeGuid", TxtOpenTypeGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowDay1Info")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDay1Guid.Text = ds.Tables(0).Rows(i)(3).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDPAYDAY2_MADEN_STUDENT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", Val(TxtMaden.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDPAYDAY2_DAEN_OPENACCOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtOpenAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtMaden.Text))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDPAYDAY2_DAEN_STUDENT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtDaen.Text))
        comm.Parameters.AddWithValue("@Notes", "الرصيد الأفتتاحي للطالب")
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDPAYDAY2_MADEN_OPENACCOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtOpenAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", Val(TxtDaen.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", "الرصيد الأفتتاحي للطالب")
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDSTUDENTLEVEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddStudentLevel", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@RegDocGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@LevelMark", 0)
        comm.Parameters.AddWithValue("@LevelStaut", True)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@CourseGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@CaseGuid", TxtCaseGuid.Text)
        comm.Parameters.AddWithValue("@RegType", 0)

        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDSTUDENTCOURSE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddStudentLevel", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@RegDocGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@LevelGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@LevelMark", 0)
        comm.Parameters.AddWithValue("@LevelStaut", True)
        comm.Parameters.AddWithValue("@BatchGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@CourseGuid", TxtCourseGuid.Text)
        comm.Parameters.AddWithValue("@CaseGuid", TxtCaseGuid.Text)
        comm.Parameters.AddWithValue("@RegType", 1)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("addstudent") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDACCOUNT()
            SHOWACCOUNTINFO()
            CREATEOTHERID()
            ADDSTUDENT()
            SHOWSTUDENTINFO()
            If TxtMaden.Text <> 0 Or TxtDaen.Text <> 0 Then
                NOTES = "رصيد أفتتاحي للطالب" & " " & TxtNotes.Text
                ADDDAY1()
                SHOWDAY1INFO()
                If TxtMaden.Text <> 0 Then
                    ADDPAYDAY2_MADEN_STUDENT()
                    ADDPAYDAY2_DAEN_OPENACCOUNT()
                ElseIf TxtDaen.Text <> 0 Then
                    ADDPAYDAY2_MADEN_OPENACCOUNT()
                    ADDPAYDAY2_DAEN_STUDENT()
                End If
            End If
            If TxtIsFound.Text = 1 Then
                ADDSTUDENTLEVEL()
            End If
            If TxtCourse.Text = 1 Then
                ADDSTUDENTCOURSE()
            End If
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات الطالب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addstudent") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATESTUDENT()
            UPDATEACOUNT()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات الطالب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Public Function StudentExists(NATIONALID As String, STUDENTCODE As String) As Boolean
        Dim query = dbcontext.Students.FirstOrDefault(Function(m) m.NationalId = NATIONALID And m.Code <> STUDENTCODE)
        If (IsNothing(query)) Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Function StudentMobileExists(MOBILENUM As String, STUDENTCODE As String) As Boolean
        Dim query = dbcontext.Students.FirstOrDefault(Function(m) m.StudentTel = MOBILENUM And m.Code <> STUDENTCODE)
        If (IsNothing(query)) Then
            Return False
        Else
            Return True
        End If
    End Function
    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If StudentExists(TxtNationalId.Text, TxtCode.Text) = True Then
                    MessageBox.Show("رقم الهوية مستخدم مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If StudentMobileExists(TxtTel.Text, TxtCode.Text) = True Then
                        MessageBox.Show("رقم الجوال مستخدم مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        If TxtGuid.Text = Nothing Then
                            FORSAVE()
                            IS_Yes = True
                            Me.Close()
                        Else
                            Dim w As New UpdateResaon
                            w.ShowDialog()
                            If w.IS_Yes = True Then
                                REASON = w.TxtNotes.Text
                                FORUPDATE()
                                IS_Yes = True
                                Me.Close()
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub SHOWBRANCHINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowBarnchInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtBranchId.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowBarnchInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtBranchName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(12).ToString()
            Next
        End If
    End Sub
    Sub SHOWBALANCE(BRANCHID As String)
        Dim connection = Olddbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("CalcCustBAlance", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CustNo", TxtOldCustNo.Text)
        comm.Parameters.AddWithValue("@BrnId", BRANCHID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "CalcCustBAlance")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If Val(ds.Tables(0).Rows(i)(3)) > 0 Then
                    TxtMaden.Text = ds.Tables(0).Rows(i)(3).ToString
                Else
                    TxtDaen.Text = Math.Abs(ds.Tables(0).Rows(i)(3))
                End If
            Next
        End If
    End Sub
    Private Sub TxtCode_TextChanged(sender As Object, e As EventArgs) Handles TxtCode.TextChanged
    End Sub

    Private Sub CboType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboType.SelectedIndexChanged

    End Sub

    Private Sub CboType_KeyPress(sender As Object, e As KeyPressEventArgs) Handles CboType.KeyPress
        e.Handled = True
    End Sub

    Private Sub CboNational_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboNational.SelectedIndexChanged

    End Sub

    Private Sub CboNational_KeyPress(sender As Object, e As KeyPressEventArgs) Handles CboNational.KeyPress
        e.Handled = True
    End Sub

    Private Sub TxtNationalId_TextChanged(sender As Object, e As EventArgs) Handles TxtNationalId.TextChanged
        Try
            Dim inputNumber As String = TxtNationalId.Text
            If inputNumber.StartsWith("0") Then
                TxtIdCheck.Text = "رقم الهوية غير صحيح"
                TxtIdCheck.Visible = True
                TxtNationalId.Text = Nothing
                TxtIdCheck.ForeColor = Color.Red
            ElseIf String.IsNullOrWhiteSpace(inputNumber) Then
                TxtIdCheck.Text = "ادخل رقم الهوية"
                TxtIdCheck.Visible = True
                TxtIdCheck.ForeColor = Color.Red
            ElseIf inputNumber.Length <> 10 Then
                TxtIdCheck.Text = "رقم الهوية غير صحيح"
                TxtIdCheck.Visible = True
                TxtIdCheck.ForeColor = Color.Red
            Else
                ' إذا كان الرقم صحيحًا
                TxtIdCheck.Text = "رقم الهوية صحيح"
                TxtIdCheck.ForeColor = Color.Green
            End If

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtNationalId_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtNationalId.KeyPress
        Try
            ' التحقق من أن الإدخال عبارة عن أرقام فقط
            If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
                e.Handled = True
            End If

            ' منع الإدخال إذا كان عدد الأرقام 10 بالفعل
            If TxtNationalId.Text.Length >= 10 AndAlso Not Char.IsControl(e.KeyChar) Then
                e.Handled = True
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtTel_TextChanged(sender As Object, e As EventArgs) Handles TxtTel.TextChanged
        Try
            Dim inputNumber As String = TxtTel.Text
            If Not inputNumber.StartsWith("0") Then
                TxtTelCheck.Text = "رقم الجوال غير صحيح"
                TxtTelCheck.Visible = True
                TxtTel.Text = Nothing
                TxtTelCheck.ForeColor = Color.Red
            ElseIf String.IsNullOrWhiteSpace(inputNumber) Then
                TxtTelCheck.Text = "ادخل رقم الجوال"
                TxtTelCheck.Visible = True
                TxtTelCheck.ForeColor = Color.Red
            ElseIf inputNumber.Length <> 10 Then
                TxtTelCheck.Text = "رقم الجوال غير صحيح"
                TxtTelCheck.Visible = True
                TxtTelCheck.ForeColor = Color.Red
            Else
                ' إذا كان الرقم صحيحًا
                TxtTelCheck.Text = "رقم الجوال صحيح"
                TxtTelCheck.ForeColor = Color.Green
            End If

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtTel_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtTel.KeyPress
        Try
            ' التحقق من أن الإدخال عبارة عن أرقام فقط
            If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
                e.Handled = True
            End If

            ' منع الإدخال إذا كان عدد الأرقام 10 بالفعل
            If TxtTel.Text.Length >= 10 AndAlso Not Char.IsControl(e.KeyChar) Then
                e.Handled = True
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtParentAccount_TextChanged(sender As Object, e As EventArgs) Handles TxtParentAccount.TextChanged

    End Sub
End Class