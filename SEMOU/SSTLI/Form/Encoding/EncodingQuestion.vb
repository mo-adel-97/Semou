Imports System.Data.SqlClient

Public Class EncodingQuestion
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub CboType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboType.SelectedIndexChanged

    End Sub

    Private Sub CboType_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles CboType.SelectionChangeCommitted
        Try
            If CboType.SelectedIndex = 0 Then
                TxtChoose1.Enabled = True
                TxtChoose2.Enabled = True
                TxtChoose3.Enabled = True
                TxtChoose4.Enabled = True
            ElseIf CboType.SelectedIndex = 1 Then
                TxtChoose1.Enabled = False
                TxtChoose1.Text = Nothing
                TxtChoose2.Enabled = False
                TxtChoose2.Text = Nothing
                TxtChoose3.Enabled = False
                TxtChoose3.Text = Nothing
                TxtChoose4.Enabled = False
                TxtChoose4.Text = Nothing
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            If TxtLevelGuid.Text <> Nothing Then
                Dim w As New BookByLevelList
                w.TxtDiplomGuid.Text = TxtDiplomGuid.Text
                w.TxtLevelGuid.Text = TxtLevelGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtBookName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtBookGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار المستوي اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDiplom_Click(sender As Object, e As EventArgs) Handles CmdFindDiplom.Click
        Try
            Dim w As New DiplomList
            w.TxtIsUse.Text = True
            w.TxtType.Text = 0
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDiplomName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
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
                MessageBox.Show("برجاء اختيار الدبلوم - الدورة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Public Function GET___NO() As Integer
        Dim LEVELGUID As Guid = Guid.Parse(TxtLevelGuid.Text)
        Dim BOOKGUID As Guid = Guid.Parse(TxtBookGuid.Text)
        Dim MaxId = dbcontext.Questions.Where(Function(m) m.LevelGuid = LEVELGUID And m.BookGuid = BOOKGUID).OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New Question
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
        If TxtBookGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار المقرر الدراسي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtLevelGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار المستوي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtQuestionName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم السؤال", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf CboType.SelectedIndex = 0 Then
            If TxtChoose1.Text = Nothing Or TxtChoose2.Text = Nothing Or TxtChoose3.Text = Nothing Or TxtChoose4.Text = Nothing Then
                MessageBox.Show("برجاء ادخال الاختيارات المحددة للسؤال", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
        End If
        Return True
    End Function
    Sub ADDQUESTION()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddQuestion", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@QuestionName", TxtQuestionName.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@BookGuid", TxtBookGuid.Text)
        comm.Parameters.AddWithValue("@QuestionType", CboType.SelectedIndex)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDQUESTIONCHOOSE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddQuestionChoose", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Choose1", TxtChoose1.Text)
        comm.Parameters.AddWithValue("@Choose2", TxtChoose2.Text)
        comm.Parameters.AddWithValue("@Choose3", TxtChoose3.Text)
        comm.Parameters.AddWithValue("@Choose4", TxtChoose4.Text)
        comm.Parameters.AddWithValue("@QuestionGuid", TxtQuestionGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWQUESTIONINFO()
        Dim LEVELGUID As Guid = Guid.Parse(TxtLevelGuid.Text)
        Dim BOOKGUID As Guid = Guid.Parse(TxtBookGuid.Text)
        Dim QUESTION_GUID_ = dbcontext.Questions.FirstOrDefault(Function(m) m.LevelGuid = LEVELGUID And m.BookGuid = BOOKGUID And m.Code = TxtCode.Text)
        TxtQuestionGuid.Text = QUESTION_GUID_.Guid.ToString()
    End Sub
    Sub SHOWCHOOSE()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowQuestionChoose", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@QuestionGuid", TxtQuestionGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowQuestionChoose")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtChoose1.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtChoose2.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtChoose3.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtChoose4.Text = ds.Tables(0).Rows(i)(3).ToString()
            Next
        End If
    End Sub
    Sub UPDATEQUESTION()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateQuestion", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtQuestionGuid.Text)
        comm.Parameters.AddWithValue("@QuestionName", TxtQuestionName.Text)
        comm.Parameters.AddWithValue("@QuestionType", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETEQUESTIONCHOOSE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteQuestionChoose", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@QuestionGuid", TxtQuestionGuid.Text)
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
        comm.Parameters.AddWithValue("@ActionGuid", TxtQuestionGuid.Text)
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
        If AllFiles.FORSAVE("question") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDQUESTION()
            SHOWQUESTIONINFO()
            If CboType.SelectedIndex = 0 Then
                ADDQUESTIONCHOOSE()
            End If
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ البيانات بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("question") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEQUESTION()
            If CboType.SelectedIndex = 0 Then
                DELETEQUESTIONCHOOSE()
                ADDQUESTIONCHOOSE()
            End If
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل البيانات بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtQuestionGuid.Text = Nothing Then
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

    Private Sub CmdFindQuestion_Click(sender As Object, e As EventArgs) Handles CmdFindQuestion.Click
        Try
            If TxtLevelGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار المستوي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If TxtBookGuid.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار المقرر الدراسي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    Dim w As New LevelQuestionList
                    w.TxtBookGuid.Text = TxtBookGuid.Text
                    w.TxtLevelGuid.Text = TxtLevelGuid.Text
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                        TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                        TxtQuestionName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                        ChkStaut.Value = w.BranchGrid.Rows(ii).Cells(2).Value
                        TxtQuestionGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                        CboType.SelectedIndex = w.BranchGrid.Rows(ii).Cells(4).Value
                    End If
                    If TxtQuestionGuid.Text <> Nothing Then
                        If CboType.SelectedIndex = 0 Then
                            SHOWCHOOSE()
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW___()
        TxtCode.Text = Nothing
        TxtQuestionGuid.Text = Nothing
        TxtQuestionName.Text = Nothing
        ChkStaut.Value = True
        TxtChoose1.Text = Nothing
        TxtChoose2.Text = Nothing
        TxtChoose3.Text = Nothing
        TxtChoose4.Text = Nothing
        TxtDiplomGuid.Text = Nothing
        TxtDiplomName.Text = Nothing
        TxtLevelGuid.Text = Nothing
        TxtLevelName.Text = Nothing
        TxtBookGuid.Text = Nothing
        TxtBookName.Text = Nothing
        CboType.Text = Nothing
    End Sub
    Sub NEW_WITHBASIC()
        TxtCode.Text = Nothing
        TxtQuestionGuid.Text = Nothing
        TxtQuestionName.Text = Nothing
        ChkStaut.Value = True
        TxtChoose1.Text = Nothing
        TxtChoose2.Text = Nothing
        TxtChoose3.Text = Nothing
        TxtChoose4.Text = Nothing
        CboType.Text = Nothing
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            Dim ASKFOREDIT As DialogResult = MessageBox.Show("هل تريد الاستمرار بنفس البيانات الاساسية",
               "أضافة اسئلة الاختبارات", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If ASKFOREDIT = DialogResult.Yes Then
                NEW_WITHBASIC()
            Else
                NEW___()
                EncodingQuestion_Load(e, e)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub EncodingQuestion_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("question")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class