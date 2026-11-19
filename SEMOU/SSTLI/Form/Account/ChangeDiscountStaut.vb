Imports System.Data.SqlClient

Public Class ChangeDiscountStaut
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Sub CHANGESTAUT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangeDiscountOrderStaut", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@OrderStaut", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub CmdOk_Click(sender As Object, e As EventArgs) Handles CmdOk.Click
        Try
            If CboType.Text <> Nothing Then
                If TxtNotes.Text = Nothing Then
                    If CboType.SelectedIndex = 2 Then
                        MessageBox.Show("برجاء ادخال سبب الرفض", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                Else
                    CHANGESTAUT()
                    IS_Yes = True
                    Me.Close()
                    MessageBox.Show("تم تعديل حالة طلب الخصم بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                MessageBox.Show("برجاء اختيار حالة التنفيذ اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class