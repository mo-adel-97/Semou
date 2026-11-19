Imports System.Data.SqlClient

Public Class EncodingBranch
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtBranchName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الفرع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtVatName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال الاسم الضريبي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtVatId.Text = Nothing Then
            MessageBox.Show("برجاء ادخال الرقم الضريبي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCostCenterGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار مركز التكلفة الأفتراضي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.Branches.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Branch
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@BrName", TxtBranchName.Text)
        comm.Parameters.AddWithValue("@BrEName", TxtBranchEName.Text)
        comm.Parameters.AddWithValue("@BrAddress", TxtBranchAddress.Text)
        comm.Parameters.AddWithValue("@BrWebSite", TxtWebSite.Text)
        comm.Parameters.AddWithValue("@BrEmail", TxtEmail.Text)
        comm.Parameters.AddWithValue("@BrTel", TxtTel.Text)
        comm.Parameters.AddWithValue("@BrFax", TxtFax.Text)
        comm.Parameters.AddWithValue("@BrVatId", TxtVatId.Text)
        comm.Parameters.AddWithValue("@BrVatSMain", TxtSameVat.Checked)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@BranchQCode", TxtOtherCode.Text)
        comm.Parameters.AddWithValue("@VatName", TxtVatName.Text)
        comm.Parameters.AddWithValue("@IsRegOn", ChkReg.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@BrName", TxtBranchName.Text)
        comm.Parameters.AddWithValue("@BrEName", TxtBranchEName.Text)
        comm.Parameters.AddWithValue("@BrAddress", TxtBranchAddress.Text)
        comm.Parameters.AddWithValue("@BrWebSite", TxtWebSite.Text)
        comm.Parameters.AddWithValue("@BrEmail", TxtEmail.Text)
        comm.Parameters.AddWithValue("@BrTel", TxtTel.Text)
        comm.Parameters.AddWithValue("@BrFax", TxtFax.Text)
        comm.Parameters.AddWithValue("@BrVatId", TxtVatId.Text)
        comm.Parameters.AddWithValue("@BrVatSMain", TxtSameVat.Checked)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@BranchQCode", TxtOtherCode.Text)
        comm.Parameters.AddWithValue("@VatName", TxtVatName.Text)
        comm.Parameters.AddWithValue("@IsRegOn", ChkReg.Value)
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
    Sub SHOWBRANCHINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowBarnchInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowBarnchInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtBranchName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtBranchEName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtBranchAddress.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtWebSite.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtEmail.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtFax.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtVatId.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtSameVat.Checked = ds.Tables(0).Rows(i)(8)
                ChkStaut.Value = ds.Tables(0).Rows(i)(9)
                TxtGuid.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtCostCenterName.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(12).ToString()
                TxtOtherCode.Text = ds.Tables(0).Rows(i)(13).ToString()
                TxtVatName.Text = ds.Tables(0).Rows(i)(14).ToString()
                ChkReg.Value = ds.Tables(0).Rows(i)(15)
            Next
        End If
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("addbranch") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDBRANCH()
            SHOWBRANCHINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ البيانات بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addbranch") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEBRANCH()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل البيانات بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
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

    Private Sub Cmd_Find_Click(sender As Object, e As EventArgs) Handles Cmd_Find.Click
        Try
            If AllFiles.FORFIND("addbranch") = False Then
                Return
            Else
                Dim w As New NewBranchList
                w.TxtType.Text = 0
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    TxtGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtGuid.Text <> Nothing Then
                    SHOWBRANCHINFO()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW__()
        TxtBranchName.Text = Nothing
        TxtBranchEName.Text = Nothing
        TxtBranchAddress.Text = Nothing
        TxtWebSite.Text = Nothing
        TxtEmail.Text = Nothing
        TxtTel.Text = Nothing
        TxtFax.Text = Nothing
        TxtVatId.Text = Nothing
        TxtSameVat.Checked = False
        TxtGuid.Text = Nothing
        TxtCode.Text = Nothing
        TxtCostCenterGuid.Text = Nothing
        TxtCostCenterName.Text = Nothing
        TxtOtherCode.Text = Nothing
        TxtVatName.Text = Nothing
        ChkStaut.Value = True
        ChkReg.Value = True
    End Sub
    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW__()
            EncodingBranch_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub EncodingBranch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("addbranch")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
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