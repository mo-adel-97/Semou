Imports System.Data.SqlClient

Public Class EncodingTestSetting
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub Label7_Click(sender As Object, e As EventArgs) Handles Label7.Click

    End Sub

    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TxtExamTime.TextChanged

    End Sub

    Private Sub Label8_Click(sender As Object, e As EventArgs) Handles Label8.Click

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

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        Try
            Select Case TabControl1.SelectedIndex
                Case 1
                    If TxtBookGuid.Text = Nothing Then
                        MessageBox.Show("برجاء اختيار المقرر الدراسي اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        TabControl1.SelectedTab = TabPage1
                    Else
                        If CboType.Text = Nothing Then
                            MessageBox.Show("برجاء تحديد طبيعة السؤال الأول", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            TabControl1.SelectedTab = TabPage1
                        End If
                    End If
                Case 2
                    If TxtBookGuid.Text = Nothing Then
                        MessageBox.Show("برجاء اختيار المقرر الدراسي اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        TabControl1.SelectedTab = TabPage1
                    Else
                        If CboType2.Text = Nothing Then
                            MessageBox.Show("برجاء تحديد طبيعة السؤال الثاني", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            TabControl1.SelectedTab = TabPage1
                        End If
                    End If
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub LOADQUESTION1(MYGRID1 As DataGridView)
        MYGRID1.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadQuestion", connection)
        If TxtFind.Text <> Nothing Then
            comm.Parameters.AddWithValue("@QuestionName", TxtFind.Text)
        End If
        comm.Parameters.AddWithValue("@BookGuid", TxtBookGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@Type", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadQuestion")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MYGRID1.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MYGRID1.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MYGRID1.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                MYGRID1.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(3).ToString()
                MYGRID1.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(4).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub LOADQUESTION2(MYGRID2 As DataGridView)
        MYGRID2.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadQuestion", connection)
        If TxtFind.Text <> Nothing Then
            comm.Parameters.AddWithValue("@QuestionName", TxtFind.Text)
        End If
        comm.Parameters.AddWithValue("@BookGuid", TxtBookGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@Type", CboType2.SelectedIndex)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadQuestion")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MYGRID2.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MYGRID2.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MYGRID2.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                MYGRID2.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(3).ToString()
                MYGRID2.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(4).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub EncodingTestSetting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("examsetting")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CboType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboType.SelectedIndexChanged

    End Sub

    Private Sub CboType_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles CboType.SelectionChangeCommitted
        Try
            If TxtBookGuid.Text <> Nothing Then
                LOADQUESTION1(MainQuestion1Grid)
            Else
                MessageBox.Show("برجاء اختيار المقرر الدراسي اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                CboType.Text = Nothing
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Sub CboType2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboType2.SelectedIndexChanged

    End Sub

    Private Sub CboType2_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles CboType2.SelectionChangeCommitted
        Try
            If TxtBookGuid.Text <> Nothing Then
                LOADQUESTION2(MainQuestion2Grid)
            Else
                MessageBox.Show("برجاء اختيار المقرر الدراسي اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                CboType2.Text = Nothing
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdAll1_Click(sender As Object, e As EventArgs) Handles CmdAll1.Click
        Try
            LOADQUESTION1(Question1Grid)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdAll2_Click(sender As Object, e As EventArgs) Handles CmdAll2.Click
        Try
            LOADQUESTION2(Question2Grid)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub MainQuestion1Grid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainQuestion1Grid.CellContentClick

    End Sub
    Sub FINDITEM1EDIT_EDIT()
        Dim rowindex As String
        Dim found As Boolean = False
        For Each row As DataGridViewRow In Question1Grid.Rows
            If row.Cells.Item("ItemCode").Value = Val(TxtItemCode.Text) Then
                rowindex = row.Index.ToString()
                found = True
                Question1Grid.FirstDisplayedScrollingRowIndex = row.Index
                Question1Grid.Rows(row.Index).Selected = True
                Exit For
            Else
                found = False
            End If
        Next
    End Sub
    Private Sub MainQuestion1Grid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainQuestion1Grid.CellDoubleClick
        Try
            Dim row As DataGridViewRow
            Dim found As Boolean = False
            Dim ii As Integer = MainQuestion1Grid.CurrentRow.Index
            For Each row In Question1Grid.Rows
                For i As Integer = 0 To Question1Grid.Rows.Count - 1
                    If row.Cells(2).Value = MainQuestion1Grid.Rows(ii).Cells(2).Value.ToString Then
                        found = True
                        TxtItemCode.Text = MainQuestion1Grid.Rows(ii).Cells(0).Value.ToString()
                        MessageBox.Show("تم اضافة السؤال مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        FINDITEM1EDIT_EDIT()
                        Exit For
                    End If
                Next
            Next
            If Not found Then
                For Each row In MainQuestion1Grid.SelectedRows
                    Question1Grid.Rows.Add()
                    Dim i As Integer = Question1Grid.Rows.Count() - 1
                    Question1Grid.Rows(i).Cells(0).Value = MainQuestion1Grid.Rows(ii).Cells(0).Value.ToString()
                    Question1Grid.Rows(i).Cells(1).Value = MainQuestion1Grid.Rows(ii).Cells(1).Value.ToString()
                    Question1Grid.Rows(i).Cells(2).Value = MainQuestion1Grid.Rows(ii).Cells(2).Value.ToString()
                    Question1Grid.Rows(i).Cells(3).Value = MainQuestion1Grid.Rows(ii).Cells(3).Value.ToString()
                Next
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub FINDITEM2EDIT_EDIT()
        Dim rowindex As String
        Dim found As Boolean = False
        For Each row As DataGridViewRow In Question2Grid.Rows
            If row.Cells.Item("ItemCode2").Value = Val(TxtItemCode2.Text) Then
                rowindex = row.Index.ToString()
                found = True
                Question2Grid.FirstDisplayedScrollingRowIndex = row.Index
                Question2Grid.Rows(row.Index).Selected = True
                Exit For
            Else
                found = False
            End If
        Next
    End Sub

    Private Sub MainQuestion2Grid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainQuestion2Grid.CellContentClick

    End Sub

    Private Sub MainQuestion2Grid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainQuestion2Grid.CellDoubleClick
        Try
            Dim row As DataGridViewRow
            Dim found As Boolean = False
            Dim ii As Integer = MainQuestion2Grid.CurrentRow.Index
            For Each row In Question2Grid.Rows
                For i As Integer = 0 To Question2Grid.Rows.Count - 1
                    If row.Cells(2).Value = MainQuestion2Grid.Rows(ii).Cells(2).Value.ToString Then
                        found = True
                        TxtItemCode2.Text = MainQuestion2Grid.Rows(ii).Cells(0).Value.ToString()
                        MessageBox.Show("تم اضافة السؤال مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        FINDITEM2EDIT_EDIT()
                        Exit For
                    End If
                Next
            Next
            If Not found Then
                For Each row In MainQuestion2Grid.SelectedRows
                    Question2Grid.Rows.Add()
                    Dim i As Integer = Question2Grid.Rows.Count() - 1
                    Question2Grid.Rows(i).Cells(0).Value = MainQuestion2Grid.Rows(ii).Cells(0).Value.ToString()
                    Question2Grid.Rows(i).Cells(1).Value = MainQuestion2Grid.Rows(ii).Cells(1).Value.ToString()
                    Question2Grid.Rows(i).Cells(2).Value = MainQuestion2Grid.Rows(ii).Cells(2).Value.ToString()
                    Question2Grid.Rows(i).Cells(3).Value = MainQuestion2Grid.Rows(ii).Cells(3).Value.ToString()
                Next
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtDiplomGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الدبلوم - الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtLevelGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار المستوي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtBookGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار المقرر الدراسي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtQuestion1Name.Text = Nothing Then
            MessageBox.Show("برجاء ادخال عنوان السؤال الأول", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf CboType.Text = Nothing Then
            MessageBox.Show("برجاء اختيار طبيعة السؤال الأول", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtQuestion2Name.Text = Nothing Then
            MessageBox.Show("برجاء ادخال عنوان السؤال الثاني", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf CboType2.Text = Nothing Then
            MessageBox.Show("برجاء اختيار طبيعة السؤال الثاني", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtExamMark.Text = Nothing Or TxtExamMark.Text = 0 Then
            MessageBox.Show("برجاء ادخال درجة الأختبار", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtExamTime.Text = Nothing Or TxtExamTime.Text = 0 Then
            MessageBox.Show("برجاء ادخال وقت الأختبار", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf Question1Grid.Rows.Count = 0 Then
            MessageBox.Show("برجاء اختيار أسئلة السؤال الأول", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf Question2Grid.Rows.Count = 0 Then
            MessageBox.Show("برجاء اختيار أسئلة السؤال الثاني", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.Exams.OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New Exam
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
    Sub ADDEXAM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddExam", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@BookGuid", TxtBookGuid.Text)
        comm.Parameters.AddWithValue("@Question1Name", TxtQuestion1Name.Text)
        comm.Parameters.AddWithValue("@Question2name", TxtQuestion2Name.Text)
        comm.Parameters.AddWithValue("@ExamMark", TxtExamMark.Text)
        comm.Parameters.AddWithValue("@ExamTime", TxtExamTime.Text)
        comm.Parameters.AddWithValue("@Question1Type", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@Questison2Type", CboType2.SelectedIndex)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEEXAM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateExam", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@Question1Name", TxtQuestion1Name.Text)
        comm.Parameters.AddWithValue("@Question2name", TxtQuestion2Name.Text)
        comm.Parameters.AddWithValue("@ExamMark", TxtExamMark.Text)
        comm.Parameters.AddWithValue("@ExamTime", TxtExamTime.Text)
        comm.Parameters.AddWithValue("@Question1Type", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@Questison2Type", CboType2.SelectedIndex)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWEXAMINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowExamInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowExamInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDiplomName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDiplomGuid.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtLevelName.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtLevelGuid.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtBookName.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtBookGuid.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtQuestion1Name.Text = ds.Tables(0).Rows(i)(6).ToString()
                CboType.SelectedIndex = ds.Tables(0).Rows(i)(7)
                TxtQuestion2Name.Text = ds.Tables(0).Rows(i)(8).ToString()
                CboType2.SelectedIndex = ds.Tables(0).Rows(i)(9)
                TxtExamMark.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtExamTime.Text = ds.Tables(0).Rows(i)(11).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(12)
                TxtGuid.Text = ds.Tables(0).Rows(i)(13).ToString()
            Next
        End If
    End Sub
    Sub ADDLISTDIPLOM(MYGRID As DataGridView)
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddExamDetails", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To MYGRID.Rows.Count - 1
            If MYGRID.Rows(i).Cells(2).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@ExamGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@QuestionGuid", MYGRID.Rows(i).Cells(2).Value)
                comm.Parameters.AddWithValue("@QuestionType", MYGRID.Rows(i).Cells(3).Value)
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
    Sub DELETEEXAMDETAILS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteExamDetails", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ExamGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("examsetting") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDEXAM()
            SHOWEXAMINFO()
            ADDLISTDIPLOM(Question1Grid)
            ADDLISTDIPLOM(Question2Grid)
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات الأختبار بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("examsetting") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEEXAM()
            DELETEEXAMDETAILS()
            ADDLISTDIPLOM(Question1Grid)
            ADDLISTDIPLOM(Question2Grid)
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات الأختبار بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
    Sub LOADEXAMDETAILS(MYGRIDshow As DataGridView, TYPE As Integer)
        MYGRIDshow.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadExamDetails", connection)
        comm.Parameters.AddWithValue("@ExamGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@Type", TYPE)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadExamDetails")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MYGRIDshow.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MYGRIDshow.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MYGRIDshow.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                MYGRIDshow.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                MYGRIDshow.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdFindExam_Click(sender As Object, e As EventArgs) Handles CmdFindExam.Click
        Try
            Dim w As New ExamList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
            End If
            If TxtCode.Text <> Nothing Then
                SHOWEXAMINFO()
                If TxtGuid.Text <> Nothing Then
                    LOADQUESTION1(MainQuestion1Grid)
                    LOADQUESTION2(MainQuestion2Grid)
                    LOADEXAMDETAILS(Question1Grid, CboType.SelectedIndex)
                    LOADEXAMDETAILS(Question2Grid, CboType2.SelectedIndex)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click

    End Sub
End Class