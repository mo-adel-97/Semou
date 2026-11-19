Imports System.Data.SqlClient
Imports DevExpress.XtraReports.UI

Public Class StusdentStatment
    Dim dbcontext As New SemouEntities
    Private Sub CmdExit_Click(sender As Object, e As EventArgs)

    End Sub
    Sub LOADSTUEDENT()
        BranchGrid.Rows.Clear()
        Dim BALANCE__ As Double = 0
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("StudentStatmentAllTime", connection)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        If TxtFind.Text <> Nothing Then
            comm.Parameters.AddWithValue("@Notes", TxtFind.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "StudentStatmentAllTime")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                BranchGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString
                BranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                If ds.Tables(0).Rows(i)(8).ToString() <> "salesinvoice" And ds.Tables(0).Rows(i)(8).ToString() <> "repay" Then
                    BALANCE__ = Val(BALANCE__) + (Val(BranchGrid.Rows(COUNT__).Cells(3).Value) - Val(BranchGrid.Rows(COUNT__).Cells(4).Value))
                    BranchGrid.Rows(COUNT__).Cells(5).Value = Math.Round(BALANCE__, 2)
                Else
                    BranchGrid.Rows(COUNT__).Cells(5).Value = Math.Round(BALANCE__, 2)
                End If
                BranchGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(5).ToString()
                BranchGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(6).ToString()
                BranchGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                BranchGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                If ds.Tables(0).Rows(i)(10).ToString() <> "00000000-0000-0000-0000-000000000000" Then
                    BranchGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(10).ToString()
                Else
                    BranchGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(11).ToString()
                End If
                COUNT__ = COUNT__ + 1
            Next
            DAILYTOTAL_()
        Else
            TxtBalance.Text = 0
            TxtMaden.Text = 0
            TxtDaen.Text = 0
        End If
    End Sub
    Sub DAILYTOTAL_()
        Dim MADEN__ As Double = 0
        Dim DAEN__ As Double = 0

        For i As Integer = 0 To BranchGrid.Rows.Count - 1
            If BranchGrid.Rows(i).Cells(0).Value <> Nothing Then
                If BranchGrid.Rows(i).Cells(8).Value <> "salesinvoice" Then
                    MADEN__ = Val(MADEN__) + Val(BranchGrid.Rows(i).Cells(3).Value)
                End If
                If BranchGrid.Rows(i).Cells(8).Value <> "repay" Then
                    DAEN__ = Val(DAEN__) + Val(BranchGrid.Rows(i).Cells(4).Value)
                End If
            End If
        Next
        TxtBalance.Text = Math.Round(Val(MADEN__) - Val(DAEN__), 2)
        TxtMaden.Text = Math.Round(MADEN__, 2)
        TxtDaen.Text = Math.Round(DAEN__, 2)
    End Sub
    Sub LOADBILLSTUEDENT()
        BillGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("BillForStudent", connection)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "BillForStudent")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BillGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BillGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString
                BillGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BillGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BillGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BillGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub LOADREBILLSTUEDENT()
        ReBillGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowReBillStudent", connection)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowReBillStudent")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            ReBillGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                ReBillGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString
                ReBillGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                ReBillGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                ReBillGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                ReBillGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdClose_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Sub StusdentStatment_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADSTUEDENT()
            LOADBILLSTUEDENT()
            LOADREBILLSTUEDENT()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFind_TextChanged(sender As Object, e As EventArgs) Handles TxtFind.TextChanged
        Try
            LOADSTUEDENT()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellContentClick

    End Sub

    Private Sub BranchGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellDoubleClick
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            Select Case BranchGrid.Rows(i).Cells(8).Value.ToString
                Case "addregdoc"
                    Dim w As New RegisterForm
                    w.TxtCode.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                    w.TxtTypeGuid.Text = BranchGrid.Rows(i).Cells(9).Value.ToString()
                    w.SHOWREGDOCINFO()

                    If w.TxtRegType.Text = 1 Then
                        w.SHOWCOURSEINFO()
                    End If
                    w.TxtStudentCount.Visible = False
                    w.TxtAvaCount.Visible = False
                    w.Label26.Visible = False
                    w.Label30.Visible = False
                    w.CHKWHATREGTYPE()
                    If w.ChkOtherFees.Checked = True Then
                        w.SHOWLISTFEES()
                        w.SHOWREGDOCFEES()
                    End If
                    w.SHOWREGDOCDIPLOM()
                    w.SHOWBILLDIPLOM()
                    w.PAYTOTAL()
                    w.SHOWDAY1INFO()
                    w.DIPLOMTOTAL()
                    w.FESSTOTAL()
                    w.ShowDialog()
                Case "salesinvoice"
                    Dim w As New EncodingNewPayInvoice
                    w.TxtCode.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                    w.SHOWBILLINFO()
                    w.SHOWBILLDIPLOM()
                    w.PAYTOTAL()
                    w.ShowDialog()
                Case "regfess"
                    Dim w As New RegFessForm
                    w.TxtCode.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                    w.SHOWREGOTERFESSINFO()
                    w.SHOWFEESDETAIL()
                    w.SHOWLISTFEES()
                    w.ShowDialog()
                Case "reregister"
                    Dim w As New Re_Register
                    w.TxtCode.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                    w.SHOWREREGDOCINFO()
                    w.SHOWREGDOCINFO()
                    w.SHOWREREGDOCDIPLOM()
                    w.ShowDialog()
                Case "repay"
                    Dim w As New RePay
                    w.TxtCode.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                    w.SHOWREPAYINFO()
                    w.SHOWBILLDIPLOM(Guid.Parse(w.TxtGuid.Text))
                    w.ShowDialog()
                Case "daily"
                    Dim w As New EncodingDaily
                    RegDocPosted.WHATFORMTYPEGUID("opendaily")
                    w.TxtOpenGuid.Text = RegDocPosted.TYPEGUID
                    w.TxtCode.Text = BranchGrid.Rows(i).Cells(2).Value.ToString
                    w.TxtTypeGuid.Text = w.TxtOpenGuid.Text
                    w.SHOWDAY1INFO()
                    If w.TxtGuid.Text <> Nothing Then
                        w.SHOWDAILYDETAILS()
                        w.DAILYTOTAL()
                    End If
                    w.ShowDialog()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub PRINT()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("StudentStatmentAllTime", connection)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        If TxtFind.Text <> Nothing Then
            comm.Parameters.AddWithValue("@Notes", TxtFind.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "StudentStatmentAllTime")
        Dim dt = ds.Tables(0)
        Dim report As New StudentStatment
        report.DataSource = dt
        report.DataAdapter = adp
        report.DataMember = "DataTable1"
        report.RequestParameters = False
        report.Parameters(0).Value = TxtStudentName.Text
        report.Parameters(1).Value = TxtMaden.Text
        report.Parameters(2).Value = TxtDaen.Text
        report.Parameters(3).Value = TxtBalance.Text
        report.Parameters(4).Value = AllFiles.ONLINE_USER_NAME____
        Dim Tool_Report As ReportPrintTool = New ReportPrintTool(report)
        report.CreateDocument()
        Dim frm As New ViewReport
        frm.DocumentViewer1.DocumentSource = report
        frm.ShowDialog()
    End Sub
    Private Sub CmdPrint_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub CmdPrint_Click_1(sender As Object, e As EventArgs) Handles CmdPrint.Click
        Try
            PRINT()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdClose_Click_1(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub CmdOldData_Click(sender As Object, e As EventArgs) Handles CmdOldData.Click
        Try
            Dim w As New FindOldStudentData
            w.TxtNationalId.Text = TxtNationalId.Text
            w.TxtAccountGuid.Text = TxtAccountGuid.Text
            w.LOADOLDSTUEDENT()
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BranchGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellClick
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex
            If j = 11 Then
                Dim w As New DocHistory
                w.TxtActionGuid.Text = BranchGrid.Rows(i).Cells(10).Value.ToString()
                w.ShowDialog()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelFind_Click(sender As Object, e As EventArgs) Handles CmdDelFind.Click
        TxtFind.Text = Nothing
    End Sub

    Private Sub BillGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BillGrid.CellContentClick

    End Sub

    Private Sub BillGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles BillGrid.CellClick
        Try
            Dim i As Integer = BillGrid.CurrentCell.RowIndex
            Dim j As Integer = BillGrid.CurrentCell.ColumnIndex
            If j = 5 Then
                Dim w As New EncodingNewPayInvoice
                w.TxtCode.Text = BillGrid.Rows(i).Cells(1).Value.ToString
                w.SHOWBILLINFO()
                w.SHOWBILLDIPLOM()
                w.PAYTOTAL()
                w.ShowDialog()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ReBillGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles ReBillGrid.CellContentClick
        Try
            Dim i As Integer = ReBillGrid.CurrentCell.RowIndex
            Dim j As Integer = ReBillGrid.CurrentCell.ColumnIndex
            If j = 5 Then
                Dim w As New RePay
                w.TxtCode.Text = ReBillGrid.Rows(i).Cells(1).Value.ToString
                w.SHOWREPAYINFO()
                w.SHOWBILLDIPLOM(Guid.Parse(w.TxtGuid.Text))
                w.ShowDialog()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class