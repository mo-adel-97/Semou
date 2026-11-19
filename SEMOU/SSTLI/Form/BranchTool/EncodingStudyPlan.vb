Imports System.Data.SqlClient

Public Class EncodingStudyPlan
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click

    End Sub

    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
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
                    TxtDiplomCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    TxtDiplomName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtDiplomGuid.Text <> Nothing Then
                    LOADSTUDYDETAILS()
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindLevel_Click(sender As Object, e As EventArgs) Handles CmdFindLevel.Click
        Try
            Dim w As New LevelList
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtLevelName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtLevelGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub LOADMAINBOOK()
        BookGrid.Rows.Clear()
        Dim NAME As String = Nothing
        Dim ISUSE As String = Nothing
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadBook", connection)
        If TxtFindLevel.Text <> Nothing Then
            NAME = TxtFindLevel.Text
            comm.Parameters.AddWithValue("@BookName", NAME)
        Else
            comm.Parameters.AddWithValue("@BookName", NAME)
        End If
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadBook")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BookGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BookGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BookGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BookGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(3).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub EncodingStudyPlan_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADMAINBOOK()
            RegDocPosted.WHATFORMTYPEGUID("addplan")
            TxtType2Guid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFindLevel_TextChanged(sender As Object, e As EventArgs) Handles TxtFindLevel.TextChanged
        Try
            LOADMAINBOOK()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BookGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BookGrid.CellContentClick

    End Sub

    Private Sub BookGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles BookGrid.CellDoubleClick
        Try
            If TxtDiplomGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار الدبلوم او الدورة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If TxtLevelGuid.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار المستوي اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    Dim NOVALYE As Double = 0
                    Dim row As DataGridViewRow
                    Dim found As Boolean = False
                    Dim ii As Integer = BookGrid.CurrentRow.Index
                    For Each row In PlanGrid.Rows
                        For i As Integer = 0 To PlanGrid.Rows.Count - 1
                            If row.Cells(6).Value = BookGrid.Rows(ii).Cells(2).Value.ToString _
                                And row.Cells(5).Value = TxtLevelGuid.Text Then
                                found = True
                                MessageBox.Show("تم اضافة المقرر مع نفس المستوي مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Exit For
                            End If
                        Next
                    Next
                    If Not found Then
                        For Each row In BookGrid.SelectedRows
                            PlanGrid.Rows.Add()
                            Dim i As Integer = PlanGrid.Rows.Count() - 1
                            PlanGrid.Rows(i).Cells(0).Value = TxtLevelName.Text
                            PlanGrid.Rows(i).Cells(1).Value = BookGrid.Rows(ii).Cells(0).Value.ToString()
                            PlanGrid.Rows(i).Cells(2).Value = BookGrid.Rows(ii).Cells(1).Value.ToString()
                            PlanGrid.Rows(i).Cells(3).Value = NOVALYE
                            PlanGrid.Rows(i).Cells(4).Value = TxtDiplomGuid.Text
                            PlanGrid.Rows(i).Cells(5).Value = TxtLevelGuid.Text
                            PlanGrid.Rows(i).Cells(6).Value = BookGrid.Rows(ii).Cells(2).Value.ToString()
                        Next
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub LOADSTUDYDETAILS()
        PlanGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowStudyPlan", connection)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowStudyPlan")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            PlanGrid.RowCount = ds.Tables(0).Rows.Count
            TxtStaut.Text = True
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                PlanGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                PlanGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                PlanGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                PlanGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                PlanGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                PlanGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                PlanGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                COUNT__ = COUNT__ + 1
            Next
        Else
            TxtStaut.Text = False
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtDiplomGuid.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الدبلوم - الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf PlanGrid.Rows.Count = 0 Then
            MessageBox.Show("برجاء ادخال بيانات المقررات الدراسية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub ADD_USERACTION_()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("Add_UserAction", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ActionGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
        comm.Parameters.AddWithValue("@Date_", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@Time_", Convert.ToDateTime(DateTime.Now.ToShortTimeString))
        comm.Parameters.AddWithValue("@Notes_", AllFiles.ActionType___)
        comm.Parameters.AddWithValue("@ActionTypeGuid", TxtType2Guid.Text)
        comm.Parameters.AddWithValue("@ActionReason", REASON)
        comm.Parameters.AddWithValue("@ActionCode", TxtDiplomCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDSTUDYDETAILS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddStudyPlan", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To PlanGrid.Rows.Count - 1
            If PlanGrid.Rows(i).Cells(4).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@HourStudy", Val(PlanGrid.Rows(i).Cells(3).Value))
                comm.Parameters.AddWithValue("@DiplomGuid", PlanGrid.Rows(i).Cells(4).Value.ToString())
                comm.Parameters.AddWithValue("@LevelGuid", PlanGrid.Rows(i).Cells(5).Value.ToString())
                comm.Parameters.AddWithValue("@BookGuid", PlanGrid.Rows(i).Cells(6).Value.ToString())
                comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
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
    Sub DELETEdETAILS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteStudyPlan", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        Dim rs As Integer = comm.ExecuteNonQuery()
        If rs > 0 Then
            comm.Parameters.Clear()
        End If
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("addplan") = False Then
            Return
        Else
            AllFiles.ActionType___ = "اضافة خطة تدريبية"
            ADDSTUDYDETAILS()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ الخطة التدريبية بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addplan") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل خطة تدريبية"
            DELETEdETAILS()
            ADDSTUDYDETAILS()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل الخطة التدريبية بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtStaut.Text = False Then
                    FORSAVE()
                    TxtStaut.Text = True
                ElseIf TxtStaut.Text = True Then
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
        TxtDiplomCode.Text = Nothing
        TxtDiplomGuid.Text = Nothing
        TxtDiplomName.Text = Nothing
        TxtDiplomGuid.Text = Nothing
        TxtLevelName.Text = Nothing
        TxtLevelGuid.Text = Nothing
        PlanGrid.Rows.Clear()
        BookGrid.Rows.Clear()
        TxtStaut.Text = False
        TxtBranchGuid.Text = Nothing
        TxtBranchName.Text = Nothing
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingStudyPlan_Load(e, e)
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