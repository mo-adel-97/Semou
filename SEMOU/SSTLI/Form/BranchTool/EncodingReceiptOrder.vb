Imports System.Data.SqlClient
Imports DevExpress.XtraReports.UI

Public Class EncodingReceiptOrder
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub Cmd_Close_Click(sender As Object, e As EventArgs) Handles Cmd_Close.Click
        Me.Close()
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            Dim w As New NewBranchList
            If AllFiles.DEFBRANCH = True Then
                w.TxtType.Text = 1
                w.TxtUserGuid.Text = AllFiles.USER_GUID____.ToString
                w.TxtFind.ReadOnly = True
            Else
                w.TxtType.Text = 0
                w.TxtIsUse.Text = True
                w.TxtFind.ReadOnly = False
            End If
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
    Sub SHOWTAFKEET()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("Tafkeet", connection)
        comm.Parameters.AddWithValue("@Total", Val(TxtAmount.Text))
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "Tafkeet")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtTafkeet.Text = ds.Tables(0).Rows(i)(0).ToString()
            Next
        End If
    End Sub

    Private Sub TxtAmount_TextChanged(sender As Object, e As EventArgs) Handles TxtAmount.TextChanged
        Try
            If TxtAmount.Text <> 0 Or TxtAmount.Text <> Nothing Then
                SHOWTAFKEET()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub EncodingReceiptOrder_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("receiptorder")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW___()
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        TxtOrderTo.Text = Nothing
        TxtAmount.Text = 0
        TxtOrderFor.Text = Nothing
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        TxtUserName.Text = Nothing
        TxtUserGuid.Text = Nothing
        TxtId.Text = Nothing
        TxtTel.Text = Nothing
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingReceiptOrder_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtOrderTo.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم المستفيد اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtId.Text = Nothing Then
            MessageBox.Show("برجاء ادخال هوية المستفيد اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtTel.Text = Nothing Then
            MessageBox.Show("برجاء ادخال جوال المستفيد اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtAmount.Text = 0 Or TxtAmount.Text = Nothing Then
            MessageBox.Show("برجاء ادخال القيمة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtOrderFor.Text = Nothing Then
            MessageBox.Show("برجاء تحديد الغرض من الصرف اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub ADDRECEIPTORDER()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddReceiptOrder", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OrderDate", Convert.ToDateTime(TxtDayDate.Value))
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", TxtUserGuid.Text)
        comm.Parameters.AddWithValue("@ReceiptMan", TxtOrderTo.Text)
        comm.Parameters.AddWithValue("@NationalId", TxtId.Text)
        comm.Parameters.AddWithValue("@ReceiptTel", TxtTel.Text)
        comm.Parameters.AddWithValue("@OrderAmount", Val(TxtAmount.Text))
        comm.Parameters.AddWithValue("@OrderAmountTx", TxtTafkeet.Text)
        comm.Parameters.AddWithValue("@OrderFor", TxtOrderFor.Text)
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
    Sub SHOWRECEIPTINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowReceiptOrder", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowReceiptOrder")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDayDate.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0))
                TxtBranchName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtUserName.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtUserGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtOrderTo.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtId.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtAmount.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtTafkeet.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtOrderFor.Text = ds.Tables(0).Rows(i)(10).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(11)
                TxtGuid.Text = ds.Tables(0).Rows(i)(12).ToString()
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
        If AllFiles.FORSAVE("receiptorder") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDRECEIPTORDER()
            SHOWRECEIPTINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات اقرار استلام النقدية بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtGuid.Text = Nothing Then
                    AllFiles.WHATONLINEUSERINFO()
                    TxtUserName.Text = AllFiles.ONLINE_USER_NAME____
                    TxtUserGuid.Text = AllFiles.USER_GUID____.ToString()
                    FORSAVE()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindOrder_Click(sender As Object, e As EventArgs) Handles CmdFindOrder.Click
        Try
            Dim w As New ReceiptOrderList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
            End If
            If TxtCode.Text <> Nothing Then
                SHOWRECEIPTINFO()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdPrint_Click(sender As Object, e As EventArgs) Handles CmdPrint.Click
        Try
            If TxtGuid.Text <> Nothing Then
                Dim connection = dbcontext.Database.Connection
                Dim comm As New SqlCommand
                comm = New SqlCommand("PrintReceiptOrder", connection)
                comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
                comm.CommandType = CommandType.StoredProcedure
                If connection.State = ConnectionState.Open Then
                    connection.Close()
                End If
                connection.Open()
                comm.ExecuteNonQuery()
                Dim adp As New SqlDataAdapter(comm)
                Dim ds As DataSet = New DataSet()
                adp.Fill(ds, "PrintReceiptOrder")
                Dim dt = ds.Tables(0)
                Dim report As New ReceiptOrderDoc
                report.DataSource = dt
                report.DataAdapter = adp
                report.DataMember = "DataTable1"
                report.RequestParameters = False
                AllFiles.GETBRANCHINFO(Guid.Parse(TxtBranchGuid.Text))
                report.Parameters(0).Value = AllFiles.BRANCHNAME
                report.Parameters(1).Value = AllFiles.BRANCHADDRESS
                Dim Tool_Report As ReportPrintTool = New ReportPrintTool(report)
                report.CreateDocument()
                Dim frm As New ViewReport
                frm.DocumentViewer1.DocumentSource = report
                frm.ShowDialog()
            Else
                MessageBox.Show("برجاء اختيار امر صرف او حفظ امر الصرف الحالي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class