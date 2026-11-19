Imports System.Data.SqlClient

Public Class EncodingCashBox
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
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

    Private Sub EncodingCashBox_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            AllFiles.GETPARENTINFOFROMSETTING("30")
            TxtParentAccount.Text = AllFiles.PARENTNAMEFORNEW.ToString()
            TxtParentGuid.Text = AllFiles.PARENTGUIDFORNEW.ToString()
            TxtParentCode.Text = AllFiles.PARENTCODEFORNEW.ToString()
            RegDocPosted.WHATFORMTYPEGUID("addcashbox")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.CashBoxes.Where(Function(m) m.CashType = 0).OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New CashBox
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.Id = MaxId
            TxtCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDACCOUNT()
        AllFiles.GETACCOUNTCODE(Guid.Parse(TxtParentGuid.Text), TxtParentCode.Text)
        TxtAccountCode.Text = AllFiles.ACCOUNTCODE.ToString()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddBasicAccount", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BacicCode", TxtAccountCode.Text)
        comm.Parameters.AddWithValue("@Code", TxtAccountCode.Text)
        comm.Parameters.AddWithValue("@AccountName", TxtCashName.Text)
        comm.Parameters.AddWithValue("@AccountKind", True)
        comm.Parameters.AddWithValue("@AccountType", 1)
        comm.Parameters.AddWithValue("@EndAccount", TxtEndAccount.Text)
        comm.Parameters.AddWithValue("@ParentGuid", TxtParentGuid.Text)
        comm.Parameters.AddWithValue("@IsShow", True)
        comm.Parameters.AddWithValue("@Type_", 2)
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
        comm.Parameters.AddWithValue("@Code", TxtAccountCode.Text)
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
                TxtAccountGuid.Text = ds.Tables(0).Rows(i)(7).ToString()
            Next
        End If
    End Sub
    Sub ADDCASHBOX()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCashBox", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ID", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@CashName", TxtCashName.Text)
        comm.Parameters.AddWithValue("@AccountNumber", "")
        comm.Parameters.AddWithValue("@CashType", 0)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@UseInSales", ChkUseSales.Checked)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWCASHBOXINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowCashBoxInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@Type", 0)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowCashBoxInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtCashName.Text = ds.Tables(0).Rows(i)(0).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(2)
                TxtAccountCode.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtAccountGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtGuid.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtCostCenterName.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
                ChkUseSales.Checked = ds.Tables(0).Rows(i)(9)
                ChkIsPublic.Checked = ds.Tables(0).Rows(i)(10)
            Next
        End If
    End Sub
    Sub UPDATECASHBOX()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateCashBox", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@CashName", TxtCashName.Text)
        comm.Parameters.AddWithValue("@AccountNumber", "")
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@UseInSales", ChkUseSales.Checked)
        comm.Parameters.AddWithValue("@IsPublic", ChkIsPublic.Checked)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEACOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateAccount", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@AccountName", TxtCashName.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
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
    Function VALIDATE_DATA() As Boolean
        If TxtCashName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الخزينة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub FORSAVE()
        If AllFiles.FORSAVE("addcashbox") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDACCOUNT()
            SHOWACCOUNTINFO()
            ADDCASHBOX()
            SHOWCASHBOXINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات الخزينة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addcashbox") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATECASHBOX()
            UPDATEACOUNT()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات الخزينة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub CmdFindCashBox_Click(sender As Object, e As EventArgs) Handles CmdFindCashBox.Click
        Try
            Dim w As New CashBoxList
            w.Text = "قائمة الخزائن"
            w.TxtType.Text = 0
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                'TxtGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtCode.Text <> Nothing Then
                SHOWCASHBOXINFO()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW___()
        TxtCode.Text = Nothing
        TxtCashName.Text = Nothing
        TxtGuid.Text = Nothing
        TxtAccountCode.Text = Nothing
        TxtAccountGuid.Text = Nothing
        ChkIsPublic.Checked = False
        ChkUseSales.Checked = False
        ChkStaut.Value = True
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingCashBox_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindCostCenter_Click(sender As Object, e As EventArgs) Handles CmdFindCostCenter.Click
        Try
            Dim w As New CostCenterList
            w.TxtAccountType.Text = 1
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCostCenterName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtCostCenterGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class