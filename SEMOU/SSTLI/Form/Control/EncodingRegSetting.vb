Imports System.Data.SqlClient

Public Class EncodingRegSetting
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub CmdFindPriceList_Click(sender As Object, e As EventArgs) Handles CmdFindPriceList.Click
        Try
            Dim w As New PriceList
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtPriceListName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtPriceListGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindBranch_Click(sender As Object, e As EventArgs) Handles CmdFindBranch.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
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

    Private Sub ChkDaily_CheckedChanged(sender As Object, e As EventArgs) Handles ChkDaily.CheckedChanged
        Try
            If ChkDaily.Checked = True Then
                ChkMaden.Enabled = True
                ChkDaen.Enabled = True
                ChVat.Enabled = True
                CmdFindMaden.Enabled = True
                CmdFindDaen.Enabled = True
                CmdFindCostCenter.Enabled = True
                TxtMadenGuid.Text = Nothing
                TxtDaenGuid.Text = Nothing
                TxtCostCenterGuid.Text = Nothing
                'CmdFindVat.Enabled = True
                'TxtVatGuid.Text = Nothing
            Else
                ChkMaden.Enabled = False
                ChkMaden.Checked = False
                ChkDaen.Enabled = False
                ChkDaen.Checked = False
                ChVat.Enabled = False
                ChVat.Checked = False
                CmdFindMaden.Enabled = False
                CmdFindDaen.Enabled = False
                CmdFindCostCenter.Enabled = False
                CmdFindVat.Enabled = False
                TxtMadenName.Text = Nothing
                TxtDaenName.Text = Nothing
                TxtCostCenterName.Text = Nothing
                TxtVatName.Text = Nothing
                TxtVatGuid.Text = "00000000-0000-0000-0000-000000000000"
                TxtMadenGuid.Text = "00000000-0000-0000-0000-000000000000"
                TxtDaenGuid.Text = "00000000-0000-0000-0000-000000000000"
                TxtCostCenterGuid.Text = "00000000-0000-0000-0000-000000000000"
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkMaden_CheckedChanged(sender As Object, e As EventArgs) Handles ChkMaden.CheckedChanged
        Try
            If ChkMaden.Checked = True Then
                CmdFindMaden.Enabled = False
                TxtMadenName.Text = Nothing
                TxtMadenGuid.Text = "00000000-0000-0000-0000-000000000000"
            Else
                CmdFindMaden.Enabled = True
                TxtMadenGuid.Text = Nothing
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindMaden_Click(sender As Object, e As EventArgs) Handles CmdFindMaden.Click
        Try
            Dim w As New AccountList
            w.TxtAccountType.Text = 0
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtMadenName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtMadenGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkDaen_CheckedChanged(sender As Object, e As EventArgs) Handles ChkDaen.CheckedChanged
        Try
            If ChkDaen.Checked = True Then
                CmdFindDaen.Enabled = False
                TxtDaenName.Text = Nothing
                TxtDaenGuid.Text = "00000000-0000-0000-0000-000000000000"
            Else
                CmdFindDaen.Enabled = True
                TxtDaenGuid.Text = Nothing
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDaen_Click(sender As Object, e As EventArgs) Handles CmdFindDaen.Click
        Try
            Dim w As New AccountList
            w.TxtAccountType2.Text = 2
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDaenName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDaenGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
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
    Public Function GET___NO() As Integer
        Dim TYPEGUID As Guid = Guid.Parse(TxtDocTypeGuid.Text)
        Dim MaxId = dbcontext.RegSettings.Where(Function(m) m.DocTypeGuid = TYPEGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New RegSetting
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
    Function VALIDATE_DATA() As Boolean
        If TxtDocName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الدفتر", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtPriceListGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار قائمة الأسعار", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الفرع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf ChkDaily.Checked = True Then
            If TxtMadenGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار الجانب المدين", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf TxtDaenGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار الجانب الدائن", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf TxtVatGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار حساب الضريبة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf TxtCostCenterGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار مركز التكلفة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
        End If
        Return True
    End Function
    Sub ADDREGSETTING()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRegSetting", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@DocName", TxtDocName.Text)
        comm.Parameters.AddWithValue("@PriceListGuid", TxtPriceListGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@ChkDaily", ChkDaily.Checked)
        comm.Parameters.AddWithValue("@ChkMaden", ChkMaden.Checked)
        comm.Parameters.AddWithValue("@MadenGuid", TxtMadenGuid.Text)
        comm.Parameters.AddWithValue("@ChkDaen", ChkDaen.Checked)
        comm.Parameters.AddWithValue("@DaenGuid", TxtDaenGuid.Text)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@DocTypeGuid", TxtDocTypeGuid.Text)
        comm.Parameters.AddWithValue("@chkotherfees", ChkOtherFees.Checked)
        comm.Parameters.AddWithValue("@ChkStartPay", ChkStartPay.Checked)
        comm.Parameters.AddWithValue("@ChkAmountPay", ChkAmountPay.Checked)
        comm.Parameters.AddWithValue("@AmountPay", Val(TxtAmountPay.Text))
        comm.Parameters.AddWithValue("@DocPayGuid", TxtDocPayGuid.Text)
        comm.Parameters.AddWithValue("@RegType", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@VatAccountGuid", TxtVatGuid.Text)
        comm.Parameters.AddWithValue("@ChkVat", ChVat.Checked)
        comm.Parameters.AddWithValue("@InvoiceType", CboInvoiceType.SelectedIndex)
        comm.Parameters.AddWithValue("@FessType", CboFessType.SelectedIndex)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEREGSETTING()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateRegSetting", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@DocName", TxtDocName.Text)
        comm.Parameters.AddWithValue("@PriceListGuid", TxtPriceListGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@ChkDaily", ChkDaily.Checked)
        comm.Parameters.AddWithValue("@ChkMaden", ChkMaden.Checked)
        comm.Parameters.AddWithValue("@MadenGuid", TxtMadenGuid.Text)
        comm.Parameters.AddWithValue("@ChkDaen", ChkDaen.Checked)
        comm.Parameters.AddWithValue("@DaenGuid", TxtDaenGuid.Text)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@chkotherfees", ChkOtherFees.Checked)
        comm.Parameters.AddWithValue("@ChkStartPay", ChkStartPay.Checked)
        comm.Parameters.AddWithValue("@ChkAmountPay", ChkAmountPay.Checked)
        comm.Parameters.AddWithValue("@AmountPay", Val(TxtAmountPay.Text))
        comm.Parameters.AddWithValue("@DocPayGuid", TxtDocPayGuid.Text)
        comm.Parameters.AddWithValue("@RegType", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@VatAccountGuid", TxtVatGuid.Text)
        comm.Parameters.AddWithValue("@ChkVat", ChVat.Checked)
        comm.Parameters.AddWithValue("@InvoiceType", CboInvoiceType.SelectedIndex)
        comm.Parameters.AddWithValue("@FessType", CboFessType.SelectedIndex)
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
    Sub DELETEUSER()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteRegSettingUser", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@regSettingGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDUSER()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRegSettingUser", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To UserGrid.Rows.Count - 1
            If UserGrid.Rows(i).Cells(2).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@RegSettingGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@UserGuid", UserGrid.Rows(i).Cells(2).Value.ToString())
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
    Sub SHOWREFSETTINGINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowRegSetting", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@DocTypeGuid", TxtDocTypeGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegSetting")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDocName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtPriceListName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtPriceListGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                ChkDaily.Checked = ds.Tables(0).Rows(i)(5)
                ChkMaden.Checked = ds.Tables(0).Rows(i)(6)
                TxtMadenName.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtMadenGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
                ChkDaen.Checked = ds.Tables(0).Rows(i)(9)
                TxtDaenName.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtDaenGuid.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtCostCenterName.Text = ds.Tables(0).Rows(i)(12).ToString()
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(13).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(14)
                TxtGuid.Text = ds.Tables(0).Rows(i)(15).ToString()
                TxtDocTypeName.Text = ds.Tables(0).Rows(i)(16).ToString()
                TxtDocTypeGuid.Text = ds.Tables(0).Rows(i)(17).ToString()
                ChkOtherFees.Checked = ds.Tables(0).Rows(i)(18)
                ChkStartPay.Checked = ds.Tables(0).Rows(i)(19)
                ChkAmountPay.Checked = ds.Tables(0).Rows(i)(20)
                TxtAmountPay.Text = ds.Tables(0).Rows(i)(21).ToString()
                TxtDocPay.Text = ds.Tables(0).Rows(i)(22).ToString()
                TxtDocPayGuid.Text = ds.Tables(0).Rows(i)(23).ToString()
                If IsDBNull(ds.Tables(0).Rows(i)(27)) = False Then
                    CboType.SelectedIndex = ds.Tables(0).Rows(i)(27)
                Else
                    CboType.Text = Nothing
                End If
                TxtVatName.Text = ds.Tables(0).Rows(i)(28).ToString()
                TxtVatGuid.Text = ds.Tables(0).Rows(i)(29).ToString()
                ChVat.Checked = ds.Tables(0).Rows(i)(30)
                CboInvoiceType.SelectedIndex = ds.Tables(0).Rows(i)(31)
                CboFessType.SelectedIndex = ds.Tables(0).Rows(i)(32)
            Next
        End If
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("regsetting") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDREGSETTING()
            SHOWREFSETTINGINFO()
            ADDUSER()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات الدفتر بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("regsetting") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEREGSETTING()
            DELETEUSER()
            ADDUSER()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات الدفتر بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
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

        End Try
    End Sub
    Sub LAODDOCUSER()
        UserGrid.Rows.Clear()
        Dim NAME As String = Nothing
        Dim ISUSE As String = Nothing
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadRegSttingUser", connection)
        comm.Parameters.AddWithValue("@regSettingGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadRegSttingUser")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            UserGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                UserGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                UserGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                UserGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            If AllFiles.FORFIND("regsetting") = False Then
                Return
            Else
                If TxtDocTypeGuid.Text <> Nothing Then
                    Dim w As New RegSettingList
                    w.TxtDocTypeGuid.Text = TxtDocTypeGuid.Text
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                        TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                        TxtGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                    End If
                    If TxtGuid.Text <> Nothing Then
                        SHOWREFSETTINGINFO()
                        LAODDOCUSER()
                    End If
                Else
                    MessageBox.Show("برجاء اختيار نوع الدفتر اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW___()
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        TxtDocName.Text = Nothing
        TxtPriceListName.Text = Nothing
        TxtPriceListGuid.Text = Nothing
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        ChkDaily.Checked = False
        ChkStaut.Value = True
        TxtDocTypeName.Text = Nothing
        TxtDocTypeGuid.Text = Nothing
        ChkOtherFees.Checked = False
        ChkStartPay.Checked = False
        CboType.Text = Nothing
        CboInvoiceType.Text = Nothing
        TxtUnqName.Text = Nothing
        CboType.Enabled = False
        CboInvoiceType.Enabled = False
        CboFessType.Text = Nothing
        CboFessType.Enabled = False
        UserGrid.Rows.Clear()
    End Sub
    Sub LOADUSER()
        MainUserGrid.Rows.Clear()
        Dim NAME As String = Nothing
        Dim ISUSE As String = Nothing
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadUser", connection)
        If TxtFind.Text <> Nothing Then
            comm.Parameters.AddWithValue("@UserName", TxtFind.Text)
        End If
        comm.Parameters.AddWithValue("@Staut", True)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadUser")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MainUserGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MainUserGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MainUserGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                MainUserGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(3).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub EncodingRegSetting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("regsetting")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            LOADUSER()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingRegSetting_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDocType_Click(sender As Object, e As EventArgs) Handles CmdFindDocType.Click
        Try
            Dim w As New DocTypeList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDocTypeName.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtDocTypeGuid.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtUnqName.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
            End If
            If TxtDocTypeGuid.Text <> Nothing Then
                If TxtUnqName.Text = "addregdoc" Then
                    CboType.Enabled = True
                    CboType.Text = Nothing
                Else
                    CboType.Enabled = False
                    CboType.SelectedIndex = 2
                End If
                If TxtUnqName.Text = "salesinvoice" Then
                    CboInvoiceType.Enabled = True
                    CboInvoiceType.Text = Nothing
                Else
                    CboInvoiceType.Enabled = False
                    CboInvoiceType.SelectedIndex = 3
                End If
                If TxtUnqName.Text = "regfess" Then
                    CboFessType.Text = Nothing
                    CboFessType.Enabled = True
                Else
                    CboFessType.Enabled = False
                    CboFessType.SelectedIndex = 0
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkStartPay_CheckedChanged(sender As Object, e As EventArgs) Handles ChkStartPay.CheckedChanged
        Try
            If ChkStartPay.Checked = True Then
                ChkAmountPay.Enabled = True
                TxtDocPay.Enabled = True
                CmdFindDocPay.Enabled = True
                TxtDocPayGuid.Text = Nothing
            Else
                ChkAmountPay.Enabled = False
                TxtDocPay.Enabled = False
                TxtDocPay.Text = Nothing
                CmdFindDocPay.Enabled = False
                ChkAmountPay.Checked = False
                TxtDocPayGuid.Text = "00000000-0000-0000-0000-000000000000"
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub SHOWPAYTYPEGUID()
        Dim ACTION_GUID_ = dbcontext.Form_Name.FirstOrDefault(Function(m) m.UnqName = "salesinvoice")
        TxtPayTypeGuid.Text = ACTION_GUID_.Guid.ToString
    End Sub
    Private Sub CmdFindDocPay_Click(sender As Object, e As EventArgs) Handles CmdFindDocPay.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                SHOWPAYTYPEGUID()
                Dim w As New RegSettingList
                w.TxtDocTypeGuid.Text = TxtPayTypeGuid.Text
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtDocPay.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtDocPayGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkAmountPay_CheckedChanged(sender As Object, e As EventArgs) Handles ChkAmountPay.CheckedChanged
        Try
            If ChkAmountPay.Checked = True Then
                TxtAmountPay.Enabled = True
            Else
                TxtAmountPay.Text = 0
                TxtAmountPay.Enabled = False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindVat_Click(sender As Object, e As EventArgs) Handles CmdFindVat.Click
        Try
            Dim w As New AccountList
            w.TxtAccountType.Text = 1
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtVatName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtVatGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChVat_CheckedChanged(sender As Object, e As EventArgs) Handles ChVat.CheckedChanged
        Try
            If ChVat.Checked = True Then
                CmdFindVat.Enabled = True
                'TxtVatName.Text = Nothing
                'TxtVatGuid.Text = Nothing
            Else
                CmdFindVat.Enabled = False
                TxtVatName.Text = Nothing
                TxtVatGuid.Text = "00000000-0000-0000-0000-000000000000"
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFind_TextChanged(sender As Object, e As EventArgs) Handles TxtFind.TextChanged
        LOADUSER()
    End Sub

    Private Sub MainUserGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainUserGrid.CellContentClick

    End Sub
    Sub FINDITEMEDIT_EDIT()
        Dim rowindex As String
        Dim found As Boolean = False
        For Each row As DataGridViewRow In UserGrid.Rows
            If row.Cells.Item("UserCode").Value = Val(TxtUserCode.Text) Then
                rowindex = row.Index.ToString()
                found = True
                UserGrid.FirstDisplayedScrollingRowIndex = row.Index
                UserGrid.Rows(row.Index).Selected = True
                Exit For
            Else
                found = False
            End If
        Next
    End Sub
    Private Sub MainUserGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainUserGrid.CellDoubleClick
        Try
            Dim NOVALYE As Double = 0
            Dim row As DataGridViewRow
            Dim found As Boolean = False
            Dim ii As Integer = MainUserGrid.CurrentRow.Index
            For Each row In UserGrid.Rows
                For i As Integer = 0 To UserGrid.Rows.Count - 1
                    If row.Cells(2).Value = MainUserGrid.Rows(ii).Cells(2).Value.ToString Then
                        found = True
                        TxtUserCode.Text = MainUserGrid.Rows(ii).Cells(0).Value.ToString()
                        FINDITEMEDIT_EDIT()
                        MessageBox.Show("تم اضافة المستخدم مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit For
                    End If
                Next
            Next
            If Not found Then
                For Each row In MainUserGrid.SelectedRows
                    UserGrid.Rows.Add()
                    Dim i As Integer = UserGrid.Rows.Count() - 1
                    UserGrid.Rows(i).Cells(0).Value = MainUserGrid.Rows(ii).Cells(0).Value.ToString()
                    UserGrid.Rows(i).Cells(1).Value = MainUserGrid.Rows(ii).Cells(1).Value.ToString()
                    UserGrid.Rows(i).Cells(2).Value = MainUserGrid.Rows(ii).Cells(2).Value.ToString()
                Next
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub


End Class