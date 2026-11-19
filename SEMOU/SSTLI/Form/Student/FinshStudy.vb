Imports System.Data.SqlClient

Public Class FinshStudy
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Private Sub CmdFindCase_Click(sender As Object, e As EventArgs) Handles CmdFindCase.Click
        Try
            Dim w As New StautList
            w.TxtIsUse.Text = True
            w.TxtAuto.Text = 1
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

    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Sub FINSHSTUDY()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("FinshStudy", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@CourseGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@RegCaseGuid", TxtCaseGuid.Text)
        comm.Parameters.AddWithValue("@LevelMark", TxtMark.Text)
        comm.Parameters.AddWithValue("@LevelNotes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtCaseGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار حالة التسجيل ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtMark.Text = Nothing Then
            MessageBox.Show("برجاء تحديد نسبة الاختبار الخاصة بالطالب ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub CHANGESTAUT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangeStaut", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@RegCaseGuid", TxtCaseGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                FINSHSTUDY()
                CHANGESTAUT()
                MessageBox.Show("تم انهاء دراسة الطالب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                IS_Yes = True
                Me.Close()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class