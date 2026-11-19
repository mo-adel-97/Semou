Imports System.Data.SqlClient
Imports DevExpress.XtraReports.UI

Public Class EncodingPayInvoice
    Dim dbcontext As New SemouEntities
    Dim NOTES As String
    Public IS_Yes As Boolean = False
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub EncodingPayInvoice_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("salesinvoice")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            AllFiles.WHATONLINEUSERINFO()
            TxtUserName.Text = AllFiles.ONLINE_USER_NAME____
            TxtUserGuid.Text = AllFiles.USER_GUID____.ToString()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub CHKACCOUNT()
        RegDocPosted.SHOWREFSETTINGINFO(TxtDocCode.Text, Guid.Parse(TxtTypeGuid.Text))
        TxtBranchName.Text = RegDocPosted.BRANCHNAME.ToString()
        TxtBranchGuid.Text = RegDocPosted.BRANCHGUID.ToString()
    End Sub
    Private Sub CmdFindDoc_Click(sender As Object, e As EventArgs) Handles CmdFindDoc.Click
        Try
            Dim w As New DocRegSettingList
            w.TxtTypeGuid.Text = TxtTypeGuid.Text
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDocCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtDocName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDocGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
            End If
            If TxtDocGuid.Text <> Nothing Then
                CHKACCOUNT()
            End If
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
    Sub WHATDATAINGRID()
        Select Case TxtPayType.Text
            Case 1
                SHOWREGDOCDIPLOM_1()
            Case 2
            Case 3
                SHOWREGDOCDIPLOM_3()
            Case 4

        End Select
    End Sub
    Sub SHOWREGDOCDIPLOM_1()
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowRegInfoForPay", connection)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        comm.Parameters.AddWithValue("@Type", 0)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegInfoForPay")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            DiplomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DiplomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                DiplomGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                DiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                DiplomGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                DiplomGrid.Rows(COUNT__).Cells(4).Value = 0
                DiplomGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(4).ToString()
                DiplomGrid.Rows(COUNT__).Cells(6).Value = 0
                DiplomGrid.Rows(COUNT__).Cells(7).Value = 0
                DiplomGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(5).ToString()
                DiplomGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWREGDOCDIPLOM_3()
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowRegInfoForPay", connection)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegInfoForPay")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            DiplomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DiplomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                DiplomGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                DiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                DiplomGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                DiplomGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(6).ToString()
                DiplomGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(4).ToString()
                DiplomGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(7).ToString()
                DiplomGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(8).ToString()
                DiplomGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(5).ToString()
                DiplomGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWBILLDIPLOM()
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowBillDetail", connection)
        comm.Parameters.AddWithValue("@BillGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowBillDetail")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            DiplomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DiplomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                DiplomGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                DiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                DiplomGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                DiplomGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                DiplomGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                DiplomGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                DiplomGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                DiplomGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                DiplomGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub DIPLOMTOTAL()
        Dim DIPLOMTAX As Double = 0
        Dim DIPLOMTOTAL As Double = 0
        Dim DIPLOMSUBTOTAL As Double = 0
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(2).Value <> Nothing Then
                DIPLOMTOTAL = Val(DIPLOMTOTAL) + (Val(DiplomGrid.Rows(i).Cells(4).Value))
                DIPLOMTAX = Val(DIPLOMTAX) + (Val(DiplomGrid.Rows(i).Cells(6).Value))
                DIPLOMSUBTOTAL = Val(DIPLOMSUBTOTAL) + (Val(DiplomGrid.Rows(i).Cells(7).Value))
            End If
        Next
        TxtDTotal1.Text = Math.Round(Val(DIPLOMTOTAL), 2)
        TxtDTotal2.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtDTotal3.Text = Math.Round(DIPLOMSUBTOTAL, 2)
        TxtFTotal1.Text = TxtDTotal1.Text
        TxtFTotal2.Text = TxtDTotal2.Text
        TxtFTotal3.Text = TxtDTotal3.Text
    End Sub

    Private Sub DiplomGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DiplomGrid.CellContentClick

    End Sub

    Private Sub DiplomGrid_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DiplomGrid.CellEndEdit
        Try
            Dim i As Integer = DiplomGrid.CurrentCell.RowIndex
            Dim j As Integer = DiplomGrid.CurrentCell.ColumnIndex
            If j = 4 Then
                If Val(DiplomGrid.Rows(i).Cells(5).Value) <> 0 Then
                    DiplomGrid.Rows(i).Cells(6).Value = Math.Round(Val(DiplomGrid.Rows(i).Cells(4).Value) * (Val(DiplomGrid.Rows(i).Cells(5).Value) / 100), 3)
                    DiplomGrid.Rows(i).Cells(7).Value = Val(DiplomGrid.Rows(i).Cells(4).Value) + Val(DiplomGrid.Rows(i).Cells(6).Value)
                Else
                    DiplomGrid.Rows(i).Cells(7).Value = DiplomGrid.Rows(i).Cells(4).Value
                End If
                'DiplomGrid.Rows(i).Cells(4).Value = Val(DiplomGrid.Rows(i).Cells(2).Value) + Val(DiplomGrid.Rows(i).Cells(3).Value)
            End If
            DIPLOMTOTAL()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtDocGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار نوع المستند", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCostCenterGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار مركز التكلفة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء مراجعة إعدادات التسجيل لعدم وجود فرع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf DiplomGrid.Rows.Count = 0 Then
            MessageBox.Show("لا يمكن التسجيل بدون اختيار الدبلوم - الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.Bills.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Bill
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
    Public Function GETDAY1__NO() As Integer
        Dim MaxId = dbcontext.Day1.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Day1
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtDayCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtDayCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDBILL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddBill", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@BillDate", Convert.ToDateTime(TxtRegDate.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtDocGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", TxtUserGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@BillTotal", Val(TxtFTotal1.Text))
        comm.Parameters.AddWithValue("@BillTax", Val(TxtFTotal2.Text))
        comm.Parameters.AddWithValue("@BillSubTotal", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Refrance", TxtRef.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDBILLITEM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRegDocDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(5).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@DiplomGuid", DiplomGrid.Rows(i).Cells(8).Value.ToString())
                comm.Parameters.AddWithValue("@Cost", Val(DiplomGrid.Rows(i).Cells(4).Value))
                comm.Parameters.AddWithValue("@Tax", Val(DiplomGrid.Rows(i).Cells(6).Value))
                comm.Parameters.AddWithValue("@SubTotal", Val(DiplomGrid.Rows(i).Cells(7).Value))
                comm.Parameters.AddWithValue("@DiplomType", DiplomGrid.Rows(i).Cells(9).Value.ToString())
            End If
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
            connection.Open()
            Dim rs As Integer = comm.ExecuteNonQuery()
            If rs > 0 Then
                comm.Parameters.Clear()
            End If
        Next
    End Sub
    Sub ADDDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GETDAY1__NO)
        comm.Parameters.AddWithValue("@Code", TxtDayCode.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtRegDate.Value))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@DayType", True)
        comm.Parameters.AddWithValue("@DocGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@TypeGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionCode", TxtCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDDAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDayGuid.Text)
        If RegDocPosted.CHKMADEN = True Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        Else
            comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.MADENGUID)
        End If
        comm.Parameters.AddWithValue("@Maden", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", RegDocPosted.COSTCENTERGUID)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDDAY2_DAEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDayGuid.Text)
        If RegDocPosted.CHKDAEN = True Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        Else
            comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.DAENGUID)
        End If
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", RegDocPosted.COSTCENTERGUID)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWDAY1INFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDay1InfoForDoc", connection)
        comm.Parameters.AddWithValue("@DocGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowDay1InfoForDoc")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDayCode.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDayGuid.Text = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWBILLINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowBillInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowBillInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtRegDate.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                TxtDocName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtDocGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtDocCode.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtCostCenterName.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtUserGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtUserName.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtAccountGuid.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtStudentName.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(12).ToString()
                TxtNationalId.Text = ds.Tables(0).Rows(i)(13).ToString()
                TxtFTotal1.Text = ds.Tables(0).Rows(i)(14).ToString()
                TxtFTotal2.Text = ds.Tables(0).Rows(i)(15).ToString()
                TxtFTotal3.Text = ds.Tables(0).Rows(i)(16).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(17).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(18)
                TxtGuid.Text = ds.Tables(0).Rows(i)(19).ToString()
                TxtRef.Text = ds.Tables(0).Rows(i)(20).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub UPDATEBILL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateBill", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@BillDate", Convert.ToDateTime(TxtRegDate.Value))
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@BillTotal", Val(TxtFTotal1.Text))
        comm.Parameters.AddWithValue("@BillTax", Val(TxtFTotal2.Text))
        comm.Parameters.AddWithValue("@BillSubTotal", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Refrance", TxtRef.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETEBILLITEM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteBillItem", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtDayGuid.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtRegDate.Value))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETEDAY2()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDayGuid.Text)
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
    Sub FORSAVE()
        If AllFiles.FORSAVE("salesinvoice") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDBILL()
            SHOWBILLINFO()
            ADDBILLITEM()
            NOTES = "سداد الطالب" & " " & TxtStudentName.Text & vbCrLf & " " & TxtDocName.Text
            ADDDAY1()
            SHOWDAY1INFO()
            ADDDAY2_MADEN()
            ADDDAY2_DAEN()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ السداد بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("salesinvoice") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEBILL()
            DELETEBILLITEM()
            ADDBILLITEM()
            NOTES = "سداد الطالب" & " " & TxtStudentName.Text & vbCrLf & " " & TxtDocName.Text
            UPDATEDAY1()
            DELETEDAY2()
            ADDDAY2_MADEN()
            ADDDAY2_DAEN()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل السداد بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                CHKACCOUNT()
                If TxtGuid.Text = Nothing Then
                    FORSAVE()
                Else
                    FORUPDATE()
                End If
                IS_Yes = True
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        Try
            AllFiles.PRINTTAXINVOICE(TxtGuid.Text, TxtBranchGuid.Text, "")
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class