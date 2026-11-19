Imports System.Data.SqlClient

Public Class EncodingCall3
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Public IS_Yes As Boolean = False
    Private Sub EncodingCall3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("newcall")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        If TxtGuid.Text <> Nothing Then
            IS_Yes = True
        End If
        Me.Close()
    End Sub
    Function VALIDATE_DATA() As Boolean
        If CboQueryType.Text = Nothing Then
            MessageBox.Show("برجاء تحديد نوع الاستفسار", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtNotes.Text = Nothing Then
            MessageBox.Show("برجاء ادخال ملاحظات ما تم الاستفسار عنه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf CboStaut.Text = Nothing Then
            MessageBox.Show("برجاء تحديد حالة الاتصال", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub ADDCALL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddMainCall", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StartDate", Convert.ToDateTime(TxtStartDate.Value))
        comm.Parameters.AddWithValue("@CallType", CboTybe.SelectedIndex)
        comm.Parameters.AddWithValue("@QueryType", CboQueryType.SelectedIndex)
        comm.Parameters.AddWithValue("@QueryType2", 100)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@CallStaut", CboStaut.SelectedIndex)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@CallCity", CboCity.SelectedIndex)
        If CboStaut.SelectedIndex = 0 Then
            comm.Parameters.AddWithValue("@FinishDate", Convert.ToDateTime(Now))
            comm.Parameters.AddWithValue("@FinishUserGuid", AllFiles.USER_GUID____)
            comm.Parameters.AddWithValue("@FinshNotes", TxtNotes.Text)
        End If
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
    Sub SHOWCALLINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowMainCallGuid", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowMainCallGuid")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtGuid.Text = ds.Tables(0).Rows(i)(0).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
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
    Sub FORSAVE()
        If AllFiles.FORSAVE("newcall") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDCALL()
            SHOWCALLINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم تسجيل الاتصال بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtGuid.Text = Nothing Then
                    FORSAVE()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class