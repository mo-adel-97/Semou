Imports System.Data.SqlClient

Public Class RegFessForm
    Dim dbcontext As New SemouEntities
    Dim OPENGUID As Guid
    Dim NOTES As String
    Dim REASON As String = ""
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub RegFessForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("regfess")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("receipt")
            TxtRecepitTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("opendaily")
            TxtOpenDailyType.Text = RegDocPosted.TYPEGUID
            OPENGUID = Guid.Parse(TxtOpenDailyType.Text)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub CHKACCOUNT()
        RegDocPosted.SHOWREFSETTINGINFO(TxtPayDocCode.Text, Guid.Parse(TxtTypeGuid.Text))
        TxtBranchName.Text = RegDocPosted.BRANCHNAME.ToString()
        TxtBranchGuid.Text = RegDocPosted.BRANCHGUID.ToString()
        TxtVatGuid.Text = RegDocPosted.VATACCOUNTGUID.ToString()
        ChVat.Checked = RegDocPosted.CHKVAT
        TxtPriceListGuid.Text = RegDocPosted.PRICELISTGUID.ToString()
    End Sub
    Private Sub CmdFindDoc_Click(sender As Object, e As EventArgs) Handles CmdFindDoc.Click
        Try
            Dim w As New DocRegSettingList
            w.TxtTypeGuid.Text = TxtTypeGuid.Text
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtPayDocCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtPayDocName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtPayDocGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                TxtFessType.Text = w.BranchGrid.Rows(ii).Cells(4).Value.ToString()
            End If
            If TxtPayDocGuid.Text <> Nothing Then
                CHKACCOUNT()
                SHOWLISTFEES()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub SHOWLISTFEES()
        MainFeesGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowPriceListDiplom", connection)
        comm.Parameters.AddWithValue("@PriceListGuid", TxtPriceListGuid.Text)
        comm.Parameters.AddWithValue("@Type", TxtFessType.Text)
        If TxtFindFees.Text <> Nothing Then
            comm.Parameters.AddWithValue("@DiplomName", TxtFindFees.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowPriceListDiplom")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MainFeesGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MainFeesGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MainFeesGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                MainFeesGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                MainFeesGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(4).ToString()
                MainFeesGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(5).ToString()
                MainFeesGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(6).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub

    Private Sub MainFeesGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainFeesGrid.CellContentClick

    End Sub
    Sub FESSTOTAL()
        Dim FESSTAX As Double = 0
        Dim FESSTOTAL As Double = 0
        Dim FESSSUBTOTAL As Double = 0
        For i As Integer = 0 To FeesGrid.Rows.Count - 1
            If FeesGrid.Rows(i).Cells(2).Value <> Nothing Then
                FESSTOTAL = Val(FESSTOTAL) + (Val(FeesGrid.Rows(i).Cells(2).Value))
                FESSTAX = Val(FESSTAX) + (Val(FeesGrid.Rows(i).Cells(3).Value))
                FESSSUBTOTAL = Val(FESSSUBTOTAL) + (Val(FeesGrid.Rows(i).Cells(4).Value))
            End If
        Next
        TxtTotal.Text = Math.Round(Val(FESSTOTAL), 2)
        TxtTax.Text = Math.Round(Val(FESSTAX), 2)
        TxtSubTotal.Text = Math.Round(FESSSUBTOTAL, 2)
    End Sub
    Private Sub MainFeesGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainFeesGrid.CellDoubleClick
        Try
            Dim row As DataGridViewRow
            Dim found As Boolean = False
            Dim ii As Integer = MainFeesGrid.CurrentRow.Index
            For Each row In FeesGrid.Rows
                For i As Integer = 0 To FeesGrid.Rows.Count - 1
                    If row.Cells(5).Value = MainFeesGrid.Rows(ii).Cells(5).Value.ToString Then
                        found = True
                        MessageBox.Show("تم ادراج الرسوم مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit For
                    End If
                Next
            Next
            If Not found Then
                For Each row In MainFeesGrid.SelectedRows
                    FeesGrid.Rows.Add()
                    Dim i As Integer = FeesGrid.Rows.Count() - 1
                    FeesGrid.Rows(i).Cells(0).Value = MainFeesGrid.Rows(ii).Cells(0).Value.ToString()
                    FeesGrid.Rows(i).Cells(1).Value = MainFeesGrid.Rows(ii).Cells(1).Value.ToString()
                    If ChVat.Checked = True Then
                        FeesGrid.Rows(i).Cells(2).Value = Math.Round((Val(MainFeesGrid.Rows(ii).Cells(2).Value) / 1.15), 2)
                        FeesGrid.Rows(i).Cells(3).Value = Math.Round((Val(MainFeesGrid.Rows(ii).Cells(2).Value) - Val(FeesGrid.Rows(i).Cells(2).Value)), 2)
                        FeesGrid.Rows(i).Cells(4).Value = MainFeesGrid.Rows(ii).Cells(4).Value.ToString()
                    Else
                        FeesGrid.Rows(i).Cells(2).Value = MainFeesGrid.Rows(ii).Cells(2).Value.ToString()
                        FeesGrid.Rows(i).Cells(3).Value = MainFeesGrid.Rows(ii).Cells(3).Value.ToString()
                        FeesGrid.Rows(i).Cells(4).Value = MainFeesGrid.Rows(ii).Cells(4).Value.ToString()
                    End If
                    FeesGrid.Rows(i).Cells(5).Value = MainFeesGrid.Rows(ii).Cells(5).Value.ToString()
                Next
                FESSTOTAL()
                'FINALTOTAL()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub FeesGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles FeesGrid.CellContentClick

    End Sub

    Private Sub FeesGrid_UserDeletedRow(sender As Object, e As DataGridViewRowEventArgs) Handles FeesGrid.UserDeletedRow
        FESSTOTAL()
    End Sub

    Private Sub CmdFindSalesMan_Click(sender As Object, e As EventArgs) Handles CmdFindSalesMan.Click
        Try
            If TxtPayDocGuid.Text <> Nothing Then
                Dim w As New SalesManForReg
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtSalesManName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtSalesManGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار نوع المستند اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtPayDocGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار نوع المستند", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtSalesManGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار مندوب البيع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtsubTotal.Text = 0 Then
            MessageBox.Show("لا يمكن حفظ الاستمارة بدون اختيار الرسوم", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Public Function GET_REG__NO() As Integer
        Dim MaxId = dbcontext.RegOtherFesses.OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New RegOtherFess
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
    Sub ADDREGOTHERFESS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRegOtherFess", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET_REG__NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@RegFessDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtPayDocGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@SalesManGuid", TxtSalesManGuid.Text)
        comm.Parameters.AddWithValue("@ActionTotal", Val(TxtTotal.Text))
        comm.Parameters.AddWithValue("@ActionTax", Val(TxtTax.Text))
        comm.Parameters.AddWithValue("@ActionSubTotal", Val(TxtSubTotal.Text))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWREGOTHERFESSGUID()
        Dim REGDOC_GUID_ = dbcontext.RegOtherFesses.FirstOrDefault(Function(m) m.Code = TxtCode.Text)
        TxtGuid.Text = REGDOC_GUID_.Guid.ToString()
    End Sub
    Sub ADDDETAIL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRegOtherFessDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To FeesGrid.Rows.Count - 1
            If FeesGrid.Rows(i).Cells(5).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@RegOtherFessGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@DiplomGuid", FeesGrid.Rows(i).Cells(5).Value.ToString())
                comm.Parameters.AddWithValue("@DiplomCost", Val(FeesGrid.Rows(i).Cells(2).Value))
                comm.Parameters.AddWithValue("@DiplomTax", Val(FeesGrid.Rows(i).Cells(3).Value))
                comm.Parameters.AddWithValue("@DiplomSubTotal", Val(FeesGrid.Rows(i).Cells(4).Value))
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
    Public Function GETDAY1_REG_NO() As Integer
        Dim MaxId = dbcontext.Day1.Where(Function(m) m.TypeGuid <> OPENGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Day1
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtDay1Code.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtDay1Code.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDPAYDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", OPENGUID)
        'comm.Parameters.AddWithValue("@Id", GETDAY1_REG_NO())
        'comm.Parameters.AddWithValue("@Code", TxtDay1Code.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@DayType", True)
        comm.Parameters.AddWithValue("@DocGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@TypeGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionCode", TxtCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
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
        TxtDay1Code.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWREGDAY1INFO()
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
                TxtDay1Code.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDay1Guid.Text = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDPAYDAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        If RegDocPosted.CHKMADEN = True Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        Else
            comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.MADENGUID)
        End If
        comm.Parameters.AddWithValue("@Maden", Val(TxtSubTotal.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDPAYDAY2_DAEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        If RegDocPosted.CHKDAEN = True Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        Else
            comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.DAENGUID)
        End If
        comm.Parameters.AddWithValue("@Maden", 0)
        If ChVat.Checked = True Then
            comm.Parameters.AddWithValue("@Daen", Val(TxtTotal.Text))
        Else
            comm.Parameters.AddWithValue("@Daen", Val(TxtSubTotal.Text))
        End If
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
    Sub ADDPAYDAY2_TAXDAEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.VATACCOUNTGUID)
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtTax.Text))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", "00000000-0000-0000-0000-000000000000")
        comm.Parameters.AddWithValue("@IsUse", True)
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
    Sub SHOWREGOTERFESSINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowRegOtherFessInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegOtherFessInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDateReg.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0))
                TxtPayDocName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtPayDocGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtPayDocCode.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtSalesManName.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtSalesManGuid.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtStudentName.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtNationalId.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtAccountGuid.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtTotal.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtTax.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtSubTotal.Text = ds.Tables(0).Rows(i)(12).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(13).ToString()
                TxtGuid.Text = ds.Tables(0).Rows(i)(14).ToString()
                TxtDay1Code.Text = ds.Tables(0).Rows(i)(15).ToString()
                TxtDay1Guid.Text = ds.Tables(0).Rows(i)(16).ToString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(17).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(18).ToString()
                TxtPriceListGuid.Text = ds.Tables(0).Rows(i)(19).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(20)
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWFEESDETAIL()
        FeesGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowRegOtherFessDetail", connection)
        comm.Parameters.AddWithValue("@RegOtherFessGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegOtherFessDetail")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            FeesGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                FeesGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                FeesGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                FeesGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                FeesGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                FeesGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                FeesGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub UPDATEREGOTHERFESS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateRegOtherFess", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@RegFessDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtPayDocGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@SalesManGuid", TxtSalesManGuid.Text)
        comm.Parameters.AddWithValue("@ActionTotal", Val(TxtTotal.Text))
        comm.Parameters.AddWithValue("@ActionTax", Val(TxtTax.Text))
        comm.Parameters.AddWithValue("@ActionSubTotal", Val(TxtSubTotal.Text))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETEREGOTHERFESSDETAIL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteRegOtherFessDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@RegOtherFessGuid", TxtGuid.Text)
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
        comm.Parameters.AddWithValue("@Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
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
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("regfess") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDREGOTHERFESS()
            SHOWREGOTHERFESSGUID()
            ADDDETAIL()
            NOTES = TxtPayDocName.Text & vbCrLf & " - " & TxtStudentName.Text & vbCrLf
            ADDPAYDAY1()
            SHOWREGDAY1INFO()
            ADDPAYDAY2_MADEN()
            ADDPAYDAY2_DAEN()
            If ChVat.Checked = True Then
                ADDPAYDAY2_TAXDAEN()
            End If
            SHOWREGOTERFESSINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ استمارة الرسوم بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("regfess") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEREGOTHERFESS()
            DELETEREGOTHERFESSDETAIL()
            ADDDETAIL()
            NOTES = TxtPayDocName.Text & vbCrLf & " - " & TxtStudentName.Text & vbCrLf
            UPDATEDAY1()
            DELETEDAY2()
            ADDPAYDAY2_MADEN()
            ADDPAYDAY2_DAEN()
            If ChVat.Checked = True Then
                ADDPAYDAY2_TAXDAEN()
            End If
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل استمارة الرسوم بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
End Class