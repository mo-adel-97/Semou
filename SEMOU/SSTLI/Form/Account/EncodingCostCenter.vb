Imports System.Data.SqlClient

Public Class EncodingCostCenter
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub CmdFindParent_Click(sender As Object, e As EventArgs) Handles CmdFindParent.Click
        Try
            Dim w As New CostCenterList
            w.TxtAccountType2.Text = 1
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtParentCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtParentName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtParentGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkStaut_Click(sender As Object, e As EventArgs) Handles ChkStaut.Click

    End Sub

    Private Sub CboAccountType_KeyPress(sender As Object, e As KeyPressEventArgs) Handles CboAccountType.KeyPress
        e.Handled = True
    End Sub
    Sub ADDCOSTCENTER()
        AllFiles.GETCOSTCENTERCODE(Guid.Parse(TxtParentGuid.Text), TxtParentCode.Text)
        TxtCode.Text = AllFiles.COSTCENTERCODE.ToString()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCostCenter", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BasicCode", TxtCode.Text)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@CenterName", TxtCenterName.Text)
        comm.Parameters.AddWithValue("@CenterType", CboAccountType.SelectedIndex())
        comm.Parameters.AddWithValue("@ParentGuid", TxtParentGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATECENTER()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateCostCenter", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@CenterName", TxtCenterName.Text)
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

    Sub SHOWCENTERINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowCostCenter", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowCostCenter")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtCenterName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtParentName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtParentCode.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtParentGuid.Text = ds.Tables(0).Rows(i)(3).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(4)
                CboAccountType.SelectedIndex = ds.Tables(0).Rows(i)(5)
                TxtGuid.Text = ds.Tables(0).Rows(i)(6).ToString()
            Next
        End If
    End Sub
    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            Dim w As New CostCenterList
            w.TxtAccountType2.Text = 2
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtGuid.Text <> Nothing Then
                SHOWCENTERINFO()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If CboAccountType.Text = Nothing Then
            MessageBox.Show("برجاء اختيار نوع مركز التكلفة ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtParentGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار مركز التكلفة الرئيسي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCenterName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم مركز التكلفة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub FORSAVE()
        If AllFiles.FORSAVE("addcostcenter") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDCOSTCENTER()
            SHOWCENTERINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات مركز التكلفة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addcostcenter") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATECENTER()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات مركز التكلفة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
    Sub NEW___()
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        TxtCenterName.Text = Nothing
        TxtParentName.Text = Nothing
        TxtParentGuid.Text = Nothing
        TxtParentCode.Text = Nothing
        ChkStaut.Value = True
        CboAccountType.Text = Nothing
    End Sub

    Private Sub EncodingCostCenter_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("addcostcenter")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingCostCenter_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class