Imports System.Data.SqlClient

Public Class EncodinAccountSetting
    Dim dbcontext As New SemouEntities
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Sub FILLMAINGRID()
        Dim dataTable As New DataTable()
        ' Add columns to the DataTable
        dataTable.Columns.Add("القسم المحاسبي", GetType(String))
        dataTable.Columns.Add("guid", GetType(Integer))
        ' Add rows to the DataTable with data
        dataTable.Rows.Add("الطلاب", 10)
        dataTable.Rows.Add("المدربين", 20)
        dataTable.Rows.Add("الخزائن", 30)
        dataTable.Rows.Add("البنوك", 40)
        dataTable.Rows.Add("الموردين", 50)
        MainGrid.DataSource = dataTable
        MainGrid.Columns(1).Visible = False
    End Sub
    Private Sub EncodinAccountSetting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            FILLMAINGRID()
            SHOWACCOUNTSEETINGINFO()
            RegDocPosted.WHATFORMTYPEGUID("accountsetting")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub MainGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainGrid.CellContentClick

    End Sub

    Private Sub MainGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainGrid.CellDoubleClick
        Try
            Dim row As DataGridViewRow
            Dim found As Boolean = False
            Dim ii As Integer = MainGrid.CurrentRow.Index
            For Each row In AccountGrid.Rows
                For i As Integer = 0 To AccountGrid.Rows.Count - 1
                    If row.Cells(2).Value = MainGrid.Rows(ii).Cells(1).Value.ToString Then
                        found = True
                        MessageBox.Show("تم اضافة القسم المحاسبي مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit For
                    End If
                Next
            Next
            If Not found Then
                For Each row In MainGrid.SelectedRows
                    AccountGrid.Rows.Add()
                    Dim i As Integer = AccountGrid.Rows.Count() - 1
                    AccountGrid.Rows(i).Cells(0).Value = MainGrid.Rows(ii).Cells(0).Value.ToString()
                    AccountGrid.Rows(i).Cells(2).Value = MainGrid.Rows(ii).Cells(1).Value.ToString()
                Next
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub AccountGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles AccountGrid.CellContentClick

    End Sub

    Private Sub AccountGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles AccountGrid.CellClick
        Try
            Dim i As Integer = AccountGrid.CurrentCell.RowIndex
            Dim j As Integer = AccountGrid.CurrentCell.ColumnIndex
            If j = 1 Then
                If AccountGrid.Rows(i).Cells(3).Value <> Nothing Then
                    If AllFiles.IFACCOUNTISPARENT(Guid.Parse(AccountGrid.Rows(i).Cells(3).Value)) = False Then
                        MessageBox.Show("توجد حسابات مرتبطة بالفعل لا يمكن تغيير الحساب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                Else
                    Dim w As New AccountList
                    w.TxtAccountType.Text = 0
                    w.TxtIsUse.Text = True
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                        AccountGrid.Rows(i).Cells(1).Value = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                        AccountGrid.Rows(i).Cells(3).Value = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                        AccountGrid.Rows(i).Cells(4).Value = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    End If
                End If
            ElseIf j = 5 Then
                Dim w As New AccountList
                w.TxtAccountType.Text = 0
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    AccountGrid.Rows(i).Cells(5).Value = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    AccountGrid.Rows(i).Cells(6).Value = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
            ElseIf j = 7 Then
                If AccountGrid.Rows(i).Cells(3).Value <> Nothing Then
                    If AllFiles.IFACCOUNTISPARENT(Guid.Parse(AccountGrid.Rows(i).Cells(3).Value)) = False Then
                        MessageBox.Show("توجد حسابات مرتبطة بالفعل لا يمكن حذف الحساب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        AccountGrid.Rows.RemoveAt(i)
                    End If
                Else
                    AccountGrid.Rows.RemoveAt(i)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub ADDACCOUNTSETTING()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddAccountSeting", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To AccountGrid.Rows.Count - 1
            If AccountGrid.Rows(i).Cells(4).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@DepName", AccountGrid.Rows(i).Cells(0).Value.ToString())
                comm.Parameters.AddWithValue("@DepGuid", AccountGrid.Rows(i).Cells(2).Value.ToString())
                comm.Parameters.AddWithValue("@AccountGuid", AccountGrid.Rows(i).Cells(3).Value.ToString())
                If AccountGrid.Rows(i).Cells(5).Value = Nothing Then
                    comm.Parameters.AddWithValue("@AccountOpenGuid", "00000000-0000-0000-0000-000000000000")
                Else
                    comm.Parameters.AddWithValue("@AccountOpenGuid", AccountGrid.Rows(i).Cells(6).Value.ToString())
                End If
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
    Sub DELETEACCOUNTSETTING()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteAccountSetting", connection)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        Dim rs As Integer = comm.ExecuteNonQuery()
        If rs > 0 Then
            comm.Parameters.Clear()
        End If
    End Sub
    Sub SHOWACCOUNTSEETINGINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowAccountSetting", connection)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowAccountSetting")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            TxtStaut.Text = True
            AccountGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                AccountGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                AccountGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                AccountGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                AccountGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                AccountGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                AccountGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(6).ToString()
                AccountGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(7).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If AccountGrid.Rows.Count = 0 Then
            MessageBox.Show("برجاء ادخال الأقسام المحاسبية اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Public Function NOACCOUNT_() As Boolean
        For i As Integer = 0 To AccountGrid.Rows.Count - 1
            If AccountGrid.Rows(i).Cells(3).Value = Nothing Then
                Return False
            End If
        Next
        Return True
    End Function
    Sub FORSAVE()
        If AllFiles.FORSAVE("accountsetting") = False Then
            Return
        Else
            ADDACCOUNTSETTING()
            TxtStaut.Text = True
            MessageBox.Show("تم ضبط إعدادات الحسابات بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("accountsetting") = False Then
            Return
        Else
            DELETEACCOUNTSETTING()
            ADDACCOUNTSETTING()
            MessageBox.Show("تم إعادة ضبط إعدادات الحسابات بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If NOACCOUNT_() = False Then
                    MessageBox.Show("لا يمكن الحفظ بدون اختيار الحسابات", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If TxtStaut.Text = Nothing Then
                        FORSAVE()
                    Else
                        FORUPDATE()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class