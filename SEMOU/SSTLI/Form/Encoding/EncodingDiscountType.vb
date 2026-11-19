Imports System.Data.SqlClient

Public Class EncodingDiscountType
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub TxtNotes_TextChanged(sender As Object, e As EventArgs) Handles TxtNotes.TextChanged

    End Sub

    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub EncodingDiscountType_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("distype")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindAccount_Click(sender As Object, e As EventArgs)
    End Sub
    Sub ADDDISCOUNTTYPE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddMainDis", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@DisName", TxtDiscountName.Text)
        comm.Parameters.AddWithValue("@DisValue", TxtDiscountVat.Text)
        comm.Parameters.AddWithValue("@ChkAttach", ChkAttach.Checked)
        comm.Parameters.AddWithValue("@DisNotes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        ' إضافة معامل إخراج لاستلام @Code
        Dim outputParam As New SqlParameter("@CodeOut", SqlDbType.Float)
        outputParam.Direction = ParameterDirection.Output
        comm.Parameters.Add(outputParam)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        ' استرجاع القيمة الفعلية لمعامل الإخراج
        Dim RETURNCODE As Double = Convert.ToDouble(outputParam.Value)
        TxtCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEDISCOUNTTYPE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateMainDis", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@DisName", TxtDiscountName.Text)
        comm.Parameters.AddWithValue("@DisValue", TxtDiscountVat.Text)
        comm.Parameters.AddWithValue("@ChkAttach", ChkAttach.Checked)
        comm.Parameters.AddWithValue("@DisNotes", TxtNotes.Text)
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
    Sub SHOWDISCOUNTTYPEINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowMainDisInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowMainDisInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDiscountName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDiscountVat.Text = ds.Tables(0).Rows(i)(1).ToString()
                ChkAttach.Checked = ds.Tables(0).Rows(i)(2)
                TxtNotes.Text = ds.Tables(0).Rows(i)(3).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(4)
                TxtGuid.Text = ds.Tables(0).Rows(i)(5).ToString()
            Next
        End If
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("distype") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDDISCOUNTTYPE()
            SHOWDISCOUNTTYPEINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات الخصم بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("distype") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEDISCOUNTTYPE()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات الخصم بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtDiscountName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الخصم", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtDiscountVat.Text = Nothing Then
            MessageBox.Show("برجاء تحديد نسبه/قيمة الخصم اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function

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
        TxtDiscountName.Text = Nothing
        TxtDiscountVat.Text = 0
        TxtNotes.Text = Nothing
        TxtGuid.Text = Nothing
        TxtCode.Text = Nothing
        ChkAttach.Checked = False
    End Sub
    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingDiscountType_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            Dim w As New DiscountTypeList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
            End If
            If TxtCode.Text <> Nothing Then
                SHOWDISCOUNTTYPEINFO()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkDisPref_CheckedChanged(sender As Object, e As EventArgs) Handles ChkAttach.CheckedChanged
    End Sub

    Private Sub TxtCode_TextChanged(sender As Object, e As EventArgs) Handles TxtCode.TextChanged

    End Sub
End Class