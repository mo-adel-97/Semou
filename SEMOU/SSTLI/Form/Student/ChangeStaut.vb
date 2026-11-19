Imports System.Data.SqlClient

Public Class ChangeStaut
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Private Sub ChangeStaut_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Private Sub CmdFindCase_Click(sender As Object, e As EventArgs) Handles CmdFindCase.Click
        Try
            Dim w As New StautList
            w.TxtIsUse.Text = True
            w.TxtAuto.Text = 0
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
        If TxtCaseGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار حالة التسجيل ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
    Sub CHANGELEVELSTAUT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangeLavelStaut", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        'If TxtRegType.Text = 0 Then
        '    comm.Parameters.AddWithValue("@CourseGuid", TxtDiplomGuid.Text)
        'ElseIf TxtRegType.Text = 1 Then
        comm.Parameters.AddWithValue("@CourseGuid", TxtCourseGuid.Text)
        'End If
        comm.Parameters.AddWithValue("@RegCaseGuid", TxtCaseGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub CHANGELEVELSTAUT_DIPLOM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangeLavelStaut_Diplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@CourseGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@RegCaseGuid", TxtCaseGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    'Sub ADD_USERACTION_()
    '    Dim connection = dbcontext.Database.Connection
    '    Dim comm = New SqlCommand("Add_UserAction", connection)
    '    comm.CommandType = CommandType.StoredProcedure
    '    comm.Parameters.AddWithValue("@ActionGuid", TxtGuid.Text)
    '    comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
    '    comm.Parameters.AddWithValue("@Date_", Convert.ToDateTime(DateTime.Now))
    '    comm.Parameters.AddWithValue("@Time_", Convert.ToDateTime(DateTime.Now.ToShortTimeString))
    '    comm.Parameters.AddWithValue("@Notes_", AllFiles.ActionType___)
    '    comm.Parameters.AddWithValue("@ActionTypeGuid", TxtTypeGuid.Text)
    '    comm.Parameters.AddWithValue("@ActionReason", REASON)
    '    comm.Parameters.AddWithValue("@ActionCode", TxtCode.Text)
    '    If connection.State = ConnectionState.Open Then
    '        connection.Close()
    '    End If
    '    connection.Open()
    '    comm.ExecuteNonQuery()
    '    comm.Parameters.Clear()
    'End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                CHANGESTAUT()
                If TxtRegType.Text = 0 Then
                    CHANGELEVELSTAUT_DIPLOM()
                ElseIf TxtRegType.Text = 1 Then
                    CHANGELEVELSTAUT()
                End If
                MessageBox.Show("تم تغيير حالة الطالب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    IS_Yes = True
                Me.Close()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class