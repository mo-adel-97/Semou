Imports System.Data.SqlClient
Imports System.IO

Public Class StudyInfo_Student
    Dim dbcontext As New SemouEntities
    Sub LOADINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowStudyInfo_Student", connection)
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
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
        adp.Fill(ds, "ShowStudyInfo_Student")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtBatchName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtLevelName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtStart.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtEnd.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtStautName.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtClassName.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtClassGuid.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtLevelGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtGuid.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtRegType.Text = ds.Tables(0).Rows(i)(10).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub StudyInfo_Student_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub CmdViewAttach_Click(sender As Object, e As EventArgs) Handles CmdViewAttach.Click
        Try
            AllFiles.SHOWSYSTEMINFO()
            Dim baseDirectory As String = AllFiles.UPDATEURL
            Dim folderPath As String = baseDirectory & TxtNationalId.Text
            If TxtNationalId.Text = Nothing Then
                MessageBox.Show("برجاء اختيار الطالب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If Not Directory.Exists(folderPath) Then
                    MessageBox.Show("لا يتوفر ملف مرفقات للطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    Process.Start(folderPath)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub ADDSTUDENTTOCLASS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddStudentToClass", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@ClassGuid", TxtClassGuid.Text)
        comm.Parameters.AddWithValue("@IsReg", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub CmdUpdateClass_Click(sender As Object, e As EventArgs) Handles CmdUpdateClass.Click
        Try
            If TxtRegType.Text = 0 Then
                Dim w As New ClassList
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.TxtDiplomGuid.Text = TxtDiplomGuid.Text
                w.TxtLevelGuid.Text = TxtLevelGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtClassName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtClassGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                End If
                If TxtClassGuid.Text <> Nothing Then
                    ADDSTUDENTTOCLASS()
                    MessageBox.Show("تم ربط الطالب بالشعبة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                MessageBox.Show("الملف التدريبي للطالب غير مسموح له ربط بشعبة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class