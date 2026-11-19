Imports System.Data.SqlClient

Public Class EncodingTrainer
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub EncodingTrainer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("addtrainer")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    'Public Function GET___NO() As Integer
    '    Dim MaxId = dbcontext.Trainers.OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
    '    Dim MaxDay = New Trainer
    '    If (IsNothing(MaxId)) Then
    '        MaxId = 1
    '        TxtCode.Text = MaxId
    '    Else
    '        MaxId += 1
    '        MaxDay.Id = MaxId
    '        TxtCode.Text = MaxId
    '    End If
    '    Return MaxId
    'End Function
    Sub ADDTRAINER()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddTrainer", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Name", TxtName.Text)
        comm.Parameters.AddWithValue("@Address", TxtAddress.Text)
        comm.Parameters.AddWithValue("@NationalId", TxtID.Text)
        comm.Parameters.AddWithValue("@NationalName", CboNational.Text)
        comm.Parameters.AddWithValue("@Email", TxtEmail.Text)
        comm.Parameters.AddWithValue("@Tel", TxtTel.Text)
        comm.Parameters.AddWithValue("@Mob", TxtMob.Text)
        comm.Parameters.AddWithValue("@Eduction", TxtEduction.Text)
        comm.Parameters.AddWithValue("@OtherEduction", TxtOtherEduction.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@TranierDepart", CboTrainerType.SelectedIndex)
        comm.Parameters.AddWithValue("@BookType", CboBookType.SelectedIndex)
        comm.Parameters.AddWithValue("@DailyTime", TxtDailyTime.Text)
        comm.Parameters.AddWithValue("@WeekTime", TxtWeekTime.Text)
        comm.Parameters.AddWithValue("@UseMainAllow", ChkUseMain.Checked)
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
    Sub UPDATETRAINER()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateTrainer", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@Name", TxtName.Text)
        comm.Parameters.AddWithValue("@Address", TxtAddress.Text)
        comm.Parameters.AddWithValue("@NationalId", TxtID.Text)
        comm.Parameters.AddWithValue("@NationalName", CboNational.Text)
        comm.Parameters.AddWithValue("@Email", TxtEmail.Text)
        comm.Parameters.AddWithValue("@Tel", TxtTel.Text)
        comm.Parameters.AddWithValue("@Mob", TxtMob.Text)
        comm.Parameters.AddWithValue("@Eduction", TxtEduction.Text)
        comm.Parameters.AddWithValue("@OtherEduction", TxtOtherEduction.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@TranierDepart", CboTrainerType.SelectedIndex)
        comm.Parameters.AddWithValue("@BookType", CboBookType.SelectedIndex)
        comm.Parameters.AddWithValue("@DailyTime", TxtDailyTime.Text)
        comm.Parameters.AddWithValue("@WeekTime", TxtWeekTime.Text)
        comm.Parameters.AddWithValue("@UseMainAllow", ChkUseMain.Checked)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWTRAINERINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowTrainerInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowTrainerInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtAddress.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtID.Text = ds.Tables(0).Rows(i)(2).ToString()
                CboNational.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtEmail.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtMob.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtEduction.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtOtherEduction.Text = ds.Tables(0).Rows(i)(8).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(9)
                TxtGuid.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(12).ToString()
                CboTrainerType.SelectedIndex = ds.Tables(0).Rows(i)(13)
                CboBookType.SelectedIndex = ds.Tables(0).Rows(i)(14)
                TxtDailyTime.Text = ds.Tables(0).Rows(i)(15).ToString()
                TxtWeekTime.Text = ds.Tables(0).Rows(i)(16).ToString()
                ChkUseMain.Checked = ds.Tables(0).Rows(i)(17)
            Next
        End If
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
        If TxtName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم المدرب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtID.Text = Nothing Then
            MessageBox.Show("برجاء ادخال رقم الهوية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtMob.Text = Nothing Then
            MessageBox.Show("برجاء ادخال رقم الجوال", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الفرع التابع له المدرب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf CboTrainerType.Text = Nothing Then
            MessageBox.Show("برجاء تحديد القسم التدريبي التابع له المدرب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf CboBookType.Text = Nothing Then
            MessageBox.Show("برجاء اختيار نوع المقررات التي يدرسها المدرب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtDailyTime.Text = Nothing Or TxtDailyTime.Text = 0 Then
            MessageBox.Show("برجاء ادخال ساعات العمل اليومية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtWeekTime.Text = 0 Or TxtWeekTime.Text = Nothing Then
            MessageBox.Show("برجاء ادخال ساعات العمل الأسبوعية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub FORSAVE()
        If AllFiles.FORSAVE("addtrainer") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDTRAINER()
            SHOWTRAINERINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات المدرب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addtrainer") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATETRAINER()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات المدرب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtGuid.Text = Nothing Then
                    FORSAVE()
                Else
                    Dim w As New UpdateResaon
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        REASON = w.TxtNotes.Text
                        FORUPDATE()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW___()
        TxtName.Text = Nothing
        TxtAddress.Text = Nothing
        TxtID.Text = Nothing
        CboNational.Text = Nothing
        TxtEmail.Text = Nothing
        TxtTel.Text = Nothing
        TxtMob.Text = Nothing
        TxtEduction.Text = Nothing
        TxtOtherEduction.Text = Nothing
        ChkStaut.Value = True
        TxtGuid.Text = Nothing
        TxtCode.Text = Nothing
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        CboTrainerType.Text = Nothing
        CboBookType.Text = Nothing
        TxtDailyTime.Text = 0
        TxtWeekTime.Text = 0
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingTrainer_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New TrainerList
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                End If
                If TxtCode.Text <> Nothing Then
                    SHOWTRAINERINFO()
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
End Class