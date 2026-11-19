Imports System.Data.SqlClient

Public Class EncodingOtherFees
    Dim dbcontext As New SemouEntities
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.OtherFees.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New OtherFee
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
    Sub ADDFEES()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddOtherFees", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@FessName", TxtFessname.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEFEES()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateOtherFees", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@FessName", TxtFessname.Text)
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
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWFEESINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowOtherFeesInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowOtherFeesInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtFessname.Text = ds.Tables(0).Rows(i)(0).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(1)
                TxtGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
            Next
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtFessname.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الرسوم", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub FORSAVE()
        If AllFiles.FORSAVE("addfees") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDFEES()
            SHOWFEESINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم أضافة بيانات الرسوم بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addfees") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEFEES()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات الرسوم بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
                    FORUPDATE()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW____()
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        TxtFessname.Text = Nothing
        ChkStaut.Value = True
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW____()
            EncodingOtherFees_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub EncodingOtherFees_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            If AllFiles.FORFIND("addfees") = False Then
                Return
            Else
                Dim w As New OtherFeesList
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    TxtGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtGuid.Text <> Nothing Then
                    SHOWFEESINFO()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class