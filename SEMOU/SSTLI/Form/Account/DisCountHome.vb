Imports System.Data.SqlClient
Imports System.IO

Public Class DisCountHome
    Dim dbcontext As New SemouEntities
    Sub LOADDORDER()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDiscountOrderForHome", connection)
        If CboType.Text <> Nothing Then
            comm.Parameters.AddWithValue("@OrderStaut", CboType.SelectedIndex)
        End If
        If TxtFind2.Text <> Nothing Then
            comm.Parameters.AddWithValue("@NationalId", TxtFind2.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowDiscountOrderForHome")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                BranchGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                BranchGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                BranchGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                BranchGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                BranchGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub DisCountHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADDORDER()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CboType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboType.SelectedIndexChanged
        Try
            LOADDORDER()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CboType_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles CboType.SelectionChangeCommitted

    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            TxtFind2.Text = Nothing
            CboType.Text = Nothing
            LOADDORDER()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFind2_TextChanged(sender As Object, e As EventArgs) Handles TxtFind2.TextChanged
        Try
            LOADDORDER()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellContentClick

    End Sub

    Private Sub BranchGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellClick
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            AllFiles.SHOWSYSTEMINFO()
            Dim baseDirectory As String = AllFiles.UPDATEURL
            If j = 10 Then
                Dim folderPath As String = baseDirectory & BranchGrid.Rows(i).Cells(3).Value.ToString
                If BranchGrid.Rows(i).Cells(3).Value.ToString = Nothing Then
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
            ElseIf j = 11 Then
                If AllFiles.FORFIND("studentstatment") = False Then
                    Return
                Else
                    Dim w As New StusdentStatment
                    'w.TxtId.Text = BranchGrid.Rows(i).Cells(0).Value.ToString
                    w.TxtStudentName.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                    w.TxtAccountGuid.Text = BranchGrid.Rows(i).Cells(9).Value.ToString
                    w.TxtNationalId.Text = BranchGrid.Rows(i).Cells(3).Value.ToString
                    w.ShowDialog()
                End If
            ElseIf j = 12 Then
                Dim w As New ChangeDiscountStaut
                w.TxtGuid.Text = BranchGrid.Rows(i).Cells(8).Value.ToString
                w.TxtNotes.Text = BranchGrid.Rows(i).Cells(6).Value.ToString
                w.ShowDialog()
                If w.IS_Yes = True Then
                    LOADDORDER()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class