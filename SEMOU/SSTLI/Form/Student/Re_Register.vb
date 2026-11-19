Imports System.Data.SqlClient

Public Class Re_Register
    Dim dbcontext As New SemouEntities
    Dim OPENGUID As Guid
    Dim REASON As String = ""
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub
    Sub SHOWREGDOCDIPLOM()
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowRegDocDiplomForRe", connection)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        comm.Parameters.AddWithValue("@Type_", 0)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegDocDiplomForRe")
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
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWREREGDOCDIPLOM()
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowRegDocDiplomForRe", connection)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@Type_", 0)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegDocDiplomForRe")
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
                DIPLOMTOTAL = Val(DIPLOMTOTAL) + (Val(DiplomGrid.Rows(i).Cells(2).Value))
                DIPLOMTAX = Val(DIPLOMTAX) + (Val(DiplomGrid.Rows(i).Cells(3).Value))
                DIPLOMSUBTOTAL = Val(DIPLOMSUBTOTAL) + (Val(DiplomGrid.Rows(i).Cells(4).Value))
            End If
        Next
        TxtFTotal1.Text = Math.Round(Val(DIPLOMTOTAL), 2)
        TxtFTotal2.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtFTotal3.Text = Math.Round(DIPLOMSUBTOTAL, 2)
    End Sub
    Sub CHKACCOUNT()
        RegDocPosted.SHOWREFSETTINGINFO(TxtDocCode.Text, Guid.Parse(TxtTypeGuid.Text))
        TxtBranchName.Text = RegDocPosted.BRANCHNAME.ToString()
        TxtBranchGuid.Text = RegDocPosted.BRANCHGUID.ToString()
        TxtPriceListGuid.Text = RegDocPosted.PRICELISTGUID.ToString()
        TxtVatGuid.Text = RegDocPosted.VATACCOUNTGUID.ToString()
    End Sub
    Private Sub CmdFindDoc_Click(sender As Object, e As EventArgs) Handles CmdFindDoc.Click
        Try
            Dim w As New DocRegSettingList
            w.TxtTypeGuid.Text = TxtTypeGuid.Text
            w.TxtBranchGuid.Text = TxtRegDocBranchGuid.Text
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

    Private Sub Re_Register_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("reregister")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("opendaily")
            TxtOpenDailyType.Text = RegDocPosted.TYPEGUID
            OPENGUID = Guid.Parse(TxtOpenDailyType.Text)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindSalesMan_Click(sender As Object, e As EventArgs) Handles CmdFindSalesMan.Click
        Try
            Dim w As New SalesManForReg
            w.TxtBranchGuid.Text = TxtBranchGuid.Text
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtSalesManName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtSalesManGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.ReRegDocs.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New ReRegDoc
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
    Sub ADDREGDOC()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddReRegDoc", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@ReDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtDocGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@SellerGuid", TxtSalesManGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@ReTotal", Val(TxtFTotal1.Text))
        comm.Parameters.AddWithValue("@ReTax", Val(TxtFTotal2.Text))
        comm.Parameters.AddWithValue("@ReSubTotal", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@ReNotes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWREREGDOCGUID()
        Dim REGDOC_GUID_ = dbcontext.ReRegDocs.FirstOrDefault(Function(m) m.Code = TxtCode.Text)
        TxtGuid.Text = REGDOC_GUID_.Guid.ToString()
    End Sub
    Sub ADDREGDOCDIPLOM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRegDocDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(5).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@DiplomGuid", DiplomGrid.Rows(i).Cells(5).Value.ToString())
                comm.Parameters.AddWithValue("@Cost", Val(DiplomGrid.Rows(i).Cells(2).Value))
                comm.Parameters.AddWithValue("@Tax", Val(DiplomGrid.Rows(i).Cells(3).Value))
                comm.Parameters.AddWithValue("@SubTotal", Val(DiplomGrid.Rows(i).Cells(4).Value))
                comm.Parameters.AddWithValue("@DiplomType", DiplomGrid.Rows(i).Cells(6).Value.ToString())
                comm.Parameters.AddWithValue("@Type_", 0)
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
    Public Function GETDAY1__NO() As Integer
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
    Sub ADDDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", OPENGUID)
        'comm.Parameters.AddWithValue("@Id", GETDAY1__NO)
        'comm.Parameters.AddWithValue("@Code", TxtDay1Code.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
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
                TxtDay1Code.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDay1Guid.Text = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDDAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        If RegDocPosted.CHKMADEN = True Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        Else
            comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.MADENGUID)
        End If
        comm.Parameters.AddWithValue("@Maden", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
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
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        If RegDocPosted.CHKDAEN = True Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        Else
            comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.DAENGUID)
        End If
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@CostGuid", RegDocPosted.COSTCENTERGUID)
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
    Sub FORSAVE()
        If AllFiles.FORSAVE("reregister") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDREGDOC()
            SHOWREREGDOCGUID()
            ADDREGDOCDIPLOM()
            ADDDAY1()
            SHOWDAY1INFO()
            ADDDAY2_MADEN()
            ADDDAY2_DAEN()
            UPDATEOLDREGDOC()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ مرتجع التسجيل بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtDocGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار نوع المستند", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtSalesManGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار مندوب البيع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtNotes.Text = Nothing Then
            MessageBox.Show("لا يمكن حفظ مرتجع التسجيل بدون ملاحظات", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub SHOWREREGDOCINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowReRegDocInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowReRegDocInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDateReg.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0))
                TxtDocName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtDocGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtDocCode.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtSalesManName.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtSalesManGuid.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtStudentName.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtNationalId.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtAccountGuid.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtFTotal1.Text = ds.Tables(0).Rows(i)(12).ToString()
                TxtFTotal2.Text = ds.Tables(0).Rows(i)(13).ToString()
                TxtFTotal3.Text = ds.Tables(0).Rows(i)(14).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(15).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(16)
                TxtRegDocGuid.Text = ds.Tables(0).Rows(i)(17).ToString()
                TxtDay1Code.Text = ds.Tables(0).Rows(i)(18).ToString()
                TxtDay1Guid.Text = ds.Tables(0).Rows(i)(19).ToString()
                TxtGuid.Text = ds.Tables(0).Rows(i)(20).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub UPDATEREGDOC()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateReRegDoc", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@ReDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtDocGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@SellerGuid", TxtSalesManGuid.Text)
        comm.Parameters.AddWithValue("@ReTotal", Val(TxtFTotal1.Text))
        comm.Parameters.AddWithValue("@ReTax", Val(TxtFTotal2.Text))
        comm.Parameters.AddWithValue("@ReSubTotal", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@ReNotes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
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
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
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
    Sub DELETEREGDOCDIPLOM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteReRegDocDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("reregister") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEREGDOC()
            DELETEREGDOCDIPLOM()
            ADDREGDOCDIPLOM()
            UPDATEDAY1()
            DELETEDAY2()
            ADDDAY2_MADEN()
            ADDDAY2_DAEN()
            UPDATEOLDREGDOC()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل مرتجع التسجيل بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub SHOWREGDOCINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("WhatIsSalesMan", connection)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "WhatIsSalesMan")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtSalesManName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtSalesManGuid.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtRegDocBranchGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
            Next
        End If
    End Sub
    Sub UPDATEOLDREGDOC()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ReReDoc", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        comm.Parameters.AddWithValue("@ReReg", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
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