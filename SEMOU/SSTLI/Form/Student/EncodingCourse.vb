Imports System.Data.SqlClient

Public Class EncodingCourse
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Public haj As String
    Public mel As String
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub EncodingCourse_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("addcourse")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            haj = Now.ToString("dd/MM/yyyy", New Globalization.CultureInfo("ar-sa"))
            mel = Now.ToString("dd/MM/yyyy", New Globalization.CultureInfo("ar-eg"))
            haj = DateConvert(mel)
            TxtFromDate.Text = haj
            TxtToDate.Text = haj
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDiplom_Click(sender As Object, e As EventArgs) Handles CmdFindDiplom.Click
        Try
            Dim w As New DiplomList
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDiplomName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                TxtAmount.Text = w.BranchGrid.Rows(ii).Cells(4).Value.ToString()
            End If
            If TxtDiplomGuid.Text <> Nothing Then

            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            Dim w As New TrainerList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtTranierName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtTranierGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindBranch_Click(sender As Object, e As EventArgs) Handles CmdFindBranch.Click
        Try
            AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            Dim w As New NewBranchList
            If AllFiles.DEFBRANCH = True Then
                w.TxtType.Text = 1
                w.TxtUserGuid.Text = AllFiles.USER_GUID____.ToString
            Else
                w.TxtType.Text = 0
                w.TxtIsUse.Text = True
            End If
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                TxtBranchQCode.Text = w.BranchGrid.Rows(ii).Cells(4).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function GetMonthDifference(startDate As Date, endDate As Date) As Integer
        ' Ensure end date is after start date
        If endDate < startDate Then
            Return GetMonthDifference(endDate, startDate) * -1 ' Handle negative difference
        End If

        Dim yearDiff As Integer = endDate.Year - startDate.Year
        Dim monthDiff As Integer = endDate.Month - startDate.Month

        ' Account for months in the starting year
        If startDate.Day > endDate.Day Then
            monthDiff -= 1 ' Not a full month in the starting year
        End If

        ' Add full years
        monthDiff += yearDiff * 12

        Return monthDiff
    End Function

    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.Courses.OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New Course
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.Id = MaxId
            TxtCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Function VALIDATE_DATA() As Boolean
        If CboTime.Text = Nothing Then
            MessageBox.Show("برجاء تحديد الفترة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاءاختيار الفرع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtDiplomGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtTranierGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار المدرب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCount.Text = 0 Or TxtCount.Text = Nothing Then
            MessageBox.Show("برجاء تحديد عدد المقاعد المتاحة ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub ADDCOURSE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCourse", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ID", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@DateStart", TxtFromDate.Text)
        comm.Parameters.AddWithValue("@DateEnd", TxtToDate.Text)
        comm.Parameters.AddWithValue("@When_", CboTime.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@TrainerGuid", TxtTranierGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@CourseName", TxtCourseName.Text)
        comm.Parameters.AddWithValue("@StudentCount", TxtCount.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATECOURSE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateCourse", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@DateStart", TxtFromDate.Text)
        comm.Parameters.AddWithValue("@DateEnd", TxtToDate.Text)
        comm.Parameters.AddWithValue("@When_", CboTime.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@TrainerGuid", TxtTranierGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@CourseName", TxtCourseName.Text)
        comm.Parameters.AddWithValue("@StudentCount", TxtCount.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWCOURSEINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowCourseInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowCourseInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtFromDate.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtToDate.Text = ds.Tables(0).Rows(i)(1).ToString()
                CboTime.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtDiplomName.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtDiplomGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtTranierName.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtTranierGuid.Text = ds.Tables(0).Rows(i)(6).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(7)
                TxtGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtAmount.Text = ds.Tables(0).Rows(i)(12).ToString()
                If IsDBNull(ds.Tables(0).Rows(i)(13)) = False Then
                    TxtCourseName.Text = ds.Tables(0).Rows(i)(13).ToString()
                Else
                    TxtCourseName.Text = Nothing
                End If
                TxtCount.Text = ds.Tables(0).Rows(i)(14).ToString()
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
    Sub FORSAVE()
        If AllFiles.FORSAVE("addcourse") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDCOURSE()
            SHOWCOURSEINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات الدورة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addcourse") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATECOURSE()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات الدورة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
    Private Sub CmdCalc_Click(sender As Object, e As EventArgs) Handles CmdCalc.Click
    End Sub
    Private Sub CmdFindCourse_Click(sender As Object, e As EventArgs) Handles CmdFindCourse.Click
        Try
            Dim w As New CourseList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
            End If
            If TxtCode.Text <> Nothing Then
                SHOWCOURSEINFO()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW__()
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        TxtDiplomGuid.Text = Nothing
        TxtDiplomName.Text = Nothing
        TxtTranierGuid.Text = Nothing
        TxtTranierName.Text = Nothing
        TxtNotes.Text = Nothing
        TxtCount.Text = 0
        CboTime.Text = Nothing
        TxtCourseName.Text = Nothing
        TxtAmount.Text = Nothing
    End Sub
    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW__()
            EncodingCourse_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtCount_TextChanged(sender As Object, e As EventArgs) Handles TxtCount.TextChanged

    End Sub

    Private Sub TxtCount_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtCount.KeyPress
        If Not Char.IsNumber(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then e.KeyChar = ""
    End Sub
End Class