Imports System.Data.SqlClient

Public Class BatchAvailableSeats
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Sub LOADBATCH()
        BranchGrid.Rows.Clear()
        Dim NAME As String = Nothing
        Dim ISUSE As String = Nothing
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowSeatsOnBatch", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
            If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowSeatsOnBatch")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                BranchGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub BatchAvailableSeats_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADBATCH()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellContentClick

    End Sub

    Private Sub BranchGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellDoubleClick
        Try
            ' تحقق أن الصف المحدد صالح
            If e.RowIndex >= 0 Then
                ' الحصول على القيمة من العمود الرابع (العمود رقم 4 بالترتيب الصفري)
                Dim availableSeats As Integer
                If Integer.TryParse(BranchGrid.Rows(e.RowIndex).Cells(4).Value.ToString(), availableSeats) Then
                    ' تحقق إذا كانت القيمة <= 0
                    If availableSeats <= 0 Then
                        MessageBox.Show("لا توجد مقاعد متاحة للتسجيل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Else
                        ' متابعة العملية إذا كان هناك مقاعد متاحة
                        'MessageBox.Show("يمكنك متابعة التسجيل.", "معلومات", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        IS_Yes = True
                        Me.Close()
                    End If
                Else
                    MessageBox.Show("القيمة غير صالحة.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class