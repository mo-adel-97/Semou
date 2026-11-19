Imports System.Data.SqlClient

Public Class EncodingAcceptOrder
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Public haj As String
    Public mel As String
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub EncodingAcceptOrder_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("addcourse")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            haj = Now.ToString("dd/MM/yyyy", New Globalization.CultureInfo("ar-sa"))
            mel = Now.ToString("dd/MM/yyyy", New Globalization.CultureInfo("ar-eg"))
            haj = DateConvert(mel)
            TxtFromDate.Text = haj
            TxtToDate.Text = haj
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindBranch_Click(sender As Object, e As EventArgs) Handles CmdFindBranch.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.TxtFind.ReadOnly = False
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindBatch_Click(sender As Object, e As EventArgs) Handles CmdFindBatch.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New BatchAvailableSeats
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtBatchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtBatchGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindCompany_Click(sender As Object, e As EventArgs) Handles CmdFindCompany.Click
        Try
            Dim w As New SectorLIst
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCompanyName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtCompanyGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                TxtDate.Text = w.BranchGrid.Rows(ii).Cells(4).Value.ToString()
            End If
            If TxtDate.Text = True Then
                TxtFromDate.Enabled = True
                TxtToDate.Enabled = True
                TxtStartDate.Enabled = False
                TxtEndDate.Enabled = False
            Else
                TxtFromDate.Enabled = False
                TxtToDate.Enabled = False
                TxtStartDate.Enabled = True
                TxtEndDate.Enabled = True
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDiplom_Click(sender As Object, e As EventArgs) Handles CmdFindDiplom.Click
        Try
            Dim w As New DiplomList
            w.TxtIsUse.Text = True
            w.TxtType.Text = 0
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDiplomName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDiplomGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار فرع الدراسة المراد التسجيل يه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCompanyGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار القطاع التابع له المتدرب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtDiplomGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار البرنامج المراد التسحيل به", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtBatchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الدفعه المراد التسجيل بها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub ADDORDER()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddSectorOrder", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OrderDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@SectorGuid", TxtCompanyGuid.Text)
        comm.Parameters.AddWithValue("@DiplomGuid", TxtDiplomGuid.Text)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        If TxtDate.Text = True Then
            comm.Parameters.AddWithValue("@StudyStartDate", TxtFromDate.Text)
            comm.Parameters.AddWithValue("@StudyEndDate", TxtToDate.Text)
        ElseIf TxtDate.Text = False Then
            comm.Parameters.AddWithValue("@StudyStartDate", Convert.ToDateTime(TxtStartDate.Value).ToShortDateString())
            comm.Parameters.AddWithValue("@StudyEndDate", Convert.ToDateTime(TxtEndDate.Value).ToShortDateString())
        End If
        comm.Parameters.AddWithValue("@OrderStaut", ChkStaut.Value)
        comm.Parameters.AddWithValue("@OrderAcceptDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@SalesManGuid", TxtSallesManGuid.Text)
        comm.Parameters.AddWithValue("@OrderNotes", TxtNotes.Text)
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
    Sub SHOWORDERINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowSectorOrderInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowSectorOrderInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtGuid.Text = ds.Tables(0).Rows(i)(0).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtGuid.Text = Nothing Then
                    AllFiles.WHATONLINEUSERINFO()
                    TxtSallesManName.Text = AllFiles.ONLINE_USER_NAME____
                    TxtSallesManGuid.Text = AllFiles.USER_GUID____.ToString()
                    ADDORDER()
                    SHOWORDERINFO()
                    MessageBox.Show("تم تسجيل الطلب  بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class