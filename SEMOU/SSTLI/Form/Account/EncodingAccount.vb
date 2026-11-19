Imports System.Data.SqlClient

Public Class EncodingAccount
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub CmdFindParent_Click(sender As Object, e As EventArgs) Handles CmdFindParent.Click
        Try
            Dim w As New AccountList
            w.TxtAccountType2.Text = 1
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtParentCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtParentAccountName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtParentGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                TxtEndAccount.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CboAccountType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboAccountType.SelectedIndexChanged

    End Sub

    Private Sub CboAccountType_KeyPress(sender As Object, e As KeyPressEventArgs) Handles CboAccountType.KeyPress
        e.Handled = True
    End Sub
    Sub ADDACOUNT()
        AllFiles.GETACCOUNTCODE(Guid.Parse(TxtParentGuid.Text), TxtParentCode.Text)
        TxtCode.Text = AllFiles.ACCOUNTCODE.ToString()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddBasicAccount", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BacicCode", TxtCode.Text)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@AccountName", TxtAccountName.Text)
        comm.Parameters.AddWithValue("@AccountKind", False)
        comm.Parameters.AddWithValue("@AccountType", CboAccountType.SelectedIndex())
        comm.Parameters.AddWithValue("@EndAccount", TxtEndAccount.Text)
        comm.Parameters.AddWithValue("@ParentGuid", TxtParentGuid.Text)
        comm.Parameters.AddWithValue("@IsShow", True)
        comm.Parameters.AddWithValue("@Type_", CboType.SelectedIndex)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEACOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateBasicAccount", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@AccountName", TxtAccountName.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Maden", TxtMaden.Text)
        comm.Parameters.AddWithValue("@Daen", TxtDaen.Text)
        comm.Parameters.AddWithValue("@Type_", CboType.SelectedIndex)
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
    Sub SHOWACCOUNTINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowAccount", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowAccount")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtAccountName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtParentAccountName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtParentCode.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtEndAccount.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtParentGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(5)
                CboAccountType.SelectedIndex = ds.Tables(0).Rows(i)(6)
                TxtGuid.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtMaden.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtDaen.Text = ds.Tables(0).Rows(i)(9).ToString()
                CboType.SelectedIndex = ds.Tables(0).Rows(i)(10)
            Next
        End If
    End Sub
    Sub LOADACCOUNTOPENBALANCE()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowAccountOpenBalance", connection)
        comm.Parameters.AddWithValue("@AccountGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowAccountOpenBalance")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MainBranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MainBranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MainBranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                MainBranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                MainBranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                MainBranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                MainBranchGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            If AllFiles.FORFIND("addaccount") = False Then
                Return
            Else
                Dim w As New AccountList
                w.TxtAccountType2.Text = 2
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    TxtGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtGuid.Text <> Nothing Then
                    SHOWACCOUNTINFO()
                    LOADACCOUNTOPENBALANCE()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If CboAccountType.Text = Nothing Then
            MessageBox.Show("برجاء اختيار نوع الحساب ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtParentGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الحساب الرئيسي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtAccountName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الحساب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub ADDACCOUNTOPENBALANCE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddAccountOpenBalance", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To MainBranchGrid.Rows.Count - 1
            If MainBranchGrid.Rows(i).Cells(2).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@AccountGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@BranchGuid", MainBranchGrid.Rows(i).Cells(2).Value.ToString())
                comm.Parameters.AddWithValue("@OpenMaden", MainBranchGrid.Rows(i).Cells(3).Value.ToString())
                comm.Parameters.AddWithValue("@OpenDaen", MainBranchGrid.Rows(i).Cells(4).Value.ToString())
                If MainBranchGrid.Rows(i).Cells(5).Value <> Nothing Then
                    comm.Parameters.AddWithValue("@OpenNotes", MainBranchGrid.Rows(i).Cells(5).Value.ToString())
                Else
                    comm.Parameters.AddWithValue("@OpenNotes", "لا توجد ملاحظات")
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
    Sub DELETEACCOUNTOPENBALANCE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteAccountOpenBalance", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@AccountGuid", TxtGuid.Text)
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
        If AllFiles.FORSAVE("addaccount") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDACOUNT()
            SHOWACCOUNTINFO()
            ADDACCOUNTOPENBALANCE()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات الحساب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addaccount") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEACOUNT()
            DELETEACCOUNTOPENBALANCE()
            ADDACCOUNTOPENBALANCE()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات الحساب بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
    Private Sub EncodingAccount_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADBRANCH()
            RegDocPosted.WHATFORMTYPEGUID("addaccount")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub LOADBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadBranch", connection)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadBranch")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MainBranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MainBranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MainBranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                MainBranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(3).ToString()
                MainBranchGrid.Rows(COUNT__).Cells(3).Value = 0
                MainBranchGrid.Rows(COUNT__).Cells(4).Value = 0
                MainBranchGrid.Rows(COUNT__).Cells(5).Value = ""
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub NEW___()
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        TxtAccountName.Text = Nothing
        TxtParentAccountName.Text = Nothing
        TxtParentGuid.Text = Nothing
        TxtParentCode.Text = Nothing
        TxtEndAccount.Text = Nothing
        ChkStaut.Value = True
        CboAccountType.Text = Nothing
        TxtMaden.Text = 0
        TxtDaen.Text = 0
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingAccount_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub MainBranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainBranchGrid.CellContentClick

    End Sub

    Private Sub MainBranchGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainBranchGrid.CellDoubleClick
        Try
            'Dim row As DataGridViewRow
            'Dim found As Boolean = False
            'Dim ii As Integer = MainBranchGrid.CurrentRow.Index
            'For Each row In BranchGrid.Rows
            '    For i As Integer = 0 To BranchGrid.Rows.Count - 1
            '        If row.Cells(2).Value = MainBranchGrid.Rows(ii).Cells(2).Value.ToString Then
            '            found = True
            '            MessageBox.Show("تم اختيار الفرع مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '            Exit For
            '        End If
            '    Next
            'Next
            'If Not found Then
            '    For Each row In MainBranchGrid.SelectedRows
            '        BranchGrid.Rows.Add()
            '        Dim i As Integer = BranchGrid.Rows.Count() - 1
            '        BranchGrid.Rows(i).Cells(0).Value = MainBranchGrid.Rows(ii).Cells(0).Value.ToString()
            '        BranchGrid.Rows(i).Cells(1).Value = MainBranchGrid.Rows(ii).Cells(1).Value.ToString()
            '        BranchGrid.Rows(i).Cells(2).Value = MainBranchGrid.Rows(ii).Cells(2).Value.ToString()
            '        BranchGrid.Rows(i).Cells(3).Value = 0
            '        BranchGrid.Rows(i).Cells(4).Value = 0
            '    Next
            'End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class