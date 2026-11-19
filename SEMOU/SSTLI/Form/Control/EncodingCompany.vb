Imports System.Data.SqlClient

Public Class EncodingCompany
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub EncodingCompany_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("sector")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW___()
        TxtCompanyName.Text = Nothing
        TxtCompanyText.Text = Nothing
        TxtCompanyMan.Text = Nothing
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        ChkChangeDate.Checked = False
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingCompany_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtCompanyName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم القطاع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCompanyText.Text = Nothing Then
            MessageBox.Show("برجاء ادخال نص الموافقة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCompanyMan.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم المسئول", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub ADDSECTOR()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddSector", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CompanyName", TxtCompanyName.Text)
        comm.Parameters.AddWithValue("@CompanyText", TxtCompanyText.Text)
        comm.Parameters.AddWithValue("@CompanyMan", TxtCompanyMan.Text)
        comm.Parameters.AddWithValue("@ChangeDate", ChkChangeDate.Checked)
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
    Sub UPDATESECTOR()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateSector", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@CompanyName", TxtCompanyName.Text)
        comm.Parameters.AddWithValue("@CompanyText", TxtCompanyText.Text)
        comm.Parameters.AddWithValue("@CompanyMan", TxtCompanyMan.Text)
        comm.Parameters.AddWithValue("@ChangeDate", ChkChangeDate.Checked)
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
    Sub SHOWSECTORINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowSectorInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowSectorInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtCompanyName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtCompanyText.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtCompanyMan.Text = ds.Tables(0).Rows(i)(2).ToString()
                ChkChangeDate.Checked = ds.Tables(0).Rows(i)(3)
                ChkStaut.Value = ds.Tables(0).Rows(i)(4)
                TxtGuid.Text = ds.Tables(0).Rows(i)(5).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("sector") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDSECTOR()
            SHOWSECTORINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات القطاع بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("sector") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATESECTOR()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات القطاع بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
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

    Private Sub CmdFindSector_Click(sender As Object, e As EventArgs) Handles CmdFindSector.Click
        Try
            Dim w As New SectorLIst
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
            End If
            If TxtCode.Text <> Nothing Then
                SHOWSECTORINFO()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class