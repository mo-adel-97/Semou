Imports System.Data.SqlClient

Public Class EncodingPriceSale
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Public haj As String
    Public mel As String
    Sub LOADMAINDIPLOM()
        MainDiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadDiplom", connection)
        If TxtFindDiploam.Text <> Nothing Then
            comm.Parameters.AddWithValue("@DiplomName", TxtFindDiploam.Text)
        End If
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadDiplom")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MainDiplomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MainDiplomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MainDiplomGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                MainDiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(3).ToString()
                MainDiplomGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(4).ToString()
                MainDiplomGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(5).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub LOADBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadBranch", connection)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadBranch")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MainBranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MainBranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MainBranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                MainBranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(3).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub TxtFindDiploam_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub EncodingPriceSale_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADMAINDIPLOM()
            LOADBRANCH()
            haj = Now.ToString("dd/MM/yyyy", New Globalization.CultureInfo("ar-sa"))
            mel = Now.ToString("dd/MM/yyyy", New Globalization.CultureInfo("ar-eg"))
            haj = DateConvert(mel)
            TxtFromDate.Text = haj
            TxtToDate.Text = haj
            RegDocPosted.WHATFORMTYPEGUID("pricesale")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFindDiploam_TextChanged_1(sender As Object, e As EventArgs) Handles TxtFindDiploam.TextChanged
        Try
            LOADMAINDIPLOM()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub MainDiplomGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainDiplomGrid.CellContentClick

    End Sub
    Sub FINDITEMEDIT_EDIT()
        Dim rowindex As String
        Dim found As Boolean = False
        For Each row As DataGridViewRow In DiplomGrid.Rows
            If row.Cells.Item("ItemCode").Value = Val(TxtItemCode.Text) Then
                rowindex = row.Index.ToString()
                found = True
                DiplomGrid.FirstDisplayedScrollingRowIndex = row.Index
                DiplomGrid.Rows(row.Index).Selected = True
                Exit For
            Else
                found = False
            End If
        Next
    End Sub
    Private Sub MainDiplomGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainDiplomGrid.CellDoubleClick
        Try
            Dim NOVALYE As Double = 0
            Dim row As DataGridViewRow
            Dim found As Boolean = False
            Dim ii As Integer = MainDiplomGrid.CurrentRow.Index
            For Each row In DiplomGrid.Rows
                For i As Integer = 0 To DiplomGrid.Rows.Count - 1
                    If row.Cells(9).Value = MainDiplomGrid.Rows(ii).Cells(2).Value.ToString Then
                        found = True
                        TxtItemCode.Text = MainDiplomGrid.Rows(ii).Cells(0).Value.ToString()
                        FINDITEMEDIT_EDIT()
                        MessageBox.Show("تم اضافة الدبلوم - الدورة مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit For
                    End If
                Next
            Next
            If Not found Then
                For Each row In MainDiplomGrid.SelectedRows
                    DiplomGrid.Rows.Add()
                    Dim i As Integer = DiplomGrid.Rows.Count() - 1
                    DiplomGrid.Rows(i).Cells(0).Value = MainDiplomGrid.Rows(ii).Cells(0).Value.ToString()
                    DiplomGrid.Rows(i).Cells(1).Value = MainDiplomGrid.Rows(ii).Cells(1).Value.ToString()
                    DiplomGrid.Rows(i).Cells(2).Value = NOVALYE
                    DiplomGrid.Rows(i).Cells(3).Value = NOVALYE
                    DiplomGrid.Rows(i).Cells(4).Value = NOVALYE
                    DiplomGrid.Rows(i).Cells(5).Value = NOVALYE
                    DiplomGrid.Rows(i).Cells(6).Value = MainDiplomGrid.Rows(ii).Cells(4).Value.ToString()
                    DiplomGrid.Rows(i).Cells(7).Value = NOVALYE
                    DiplomGrid.Rows(i).Cells(8).Value = NOVALYE
                    DiplomGrid.Rows(i).Cells(9).Value = MainDiplomGrid.Rows(ii).Cells(2).Value.ToString()
                    DiplomGrid.Rows(i).Cells(10).Value = MainDiplomGrid.Rows(ii).Cells(3).Value.ToString()
                Next
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub DiplomGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DiplomGrid.CellContentClick

    End Sub

    Private Sub DiplomGrid_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DiplomGrid.CellEndEdit
        Try
            Dim i As Integer = DiplomGrid.CurrentCell.RowIndex
            Dim j As Integer = DiplomGrid.CurrentCell.ColumnIndex
            If j = 2 Or j = 3 Then
                If Val(DiplomGrid.Rows(i).Cells(6).Value) = 0 Then
                    If Val(DiplomGrid.Rows(i).Cells(3).Value) <> 0 Then
                        DiplomGrid.Rows(i).Cells(4).Value = Math.Round(Val(DiplomGrid.Rows(i).Cells(2).Value) * (Val(DiplomGrid.Rows(i).Cells(3).Value) / 100), 3)
                        DiplomGrid.Rows(i).Cells(5).Value = Val(DiplomGrid.Rows(i).Cells(2).Value) - Val(DiplomGrid.Rows(i).Cells(4).Value)
                        DiplomGrid.Rows(i).Cells(8).Value = DiplomGrid.Rows(i).Cells(5).Value
                    Else
                        DiplomGrid.Rows(i).Cells(5).Value = DiplomGrid.Rows(i).Cells(2).Value
                        DiplomGrid.Rows(i).Cells(8).Value = DiplomGrid.Rows(i).Cells(5).Value
                    End If
                End If
            ElseIf j = 8 Then
                If Val(DiplomGrid.Rows(i).Cells(6).Value) <> 0 Then
                    Dim VAT As Double = (1 + Val(DiplomGrid.Rows(i).Cells(6).Value) / 100)
                    DiplomGrid.Rows(i).Cells(5).Value = Math.Round(Val(DiplomGrid.Rows(i).Cells(8).Value) / Val(VAT), 3)
                    DiplomGrid.Rows(i).Cells(7).Value = Math.Round(Val(DiplomGrid.Rows(i).Cells(5).Value) * (Val(DiplomGrid.Rows(i).Cells(6).Value) / 100), 3)
                    DiplomGrid.Rows(i).Cells(2).Value = DiplomGrid.Rows(i).Cells(5).Value
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub MainBranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainBranchGrid.CellContentClick

    End Sub

    Private Sub MainBranchGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainBranchGrid.CellDoubleClick
        Try
            Dim row As DataGridViewRow
            Dim found As Boolean = False
            Dim ii As Integer = MainBranchGrid.CurrentRow.Index
            For Each row In BranchGrid.Rows
                For i As Integer = 0 To BranchGrid.Rows.Count - 1
                    If row.Cells(2).Value = MainBranchGrid.Rows(ii).Cells(2).Value.ToString Then
                        found = True
                        MessageBox.Show("تم اختيار الفرع مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit For
                    End If
                Next
            Next
            If Not found Then
                For Each row In MainBranchGrid.SelectedRows
                    BranchGrid.Rows.Add()
                    Dim i As Integer = BranchGrid.Rows.Count() - 1
                    BranchGrid.Rows(i).Cells(0).Value = MainBranchGrid.Rows(ii).Cells(0).Value.ToString()
                    BranchGrid.Rows(i).Cells(1).Value = MainBranchGrid.Rows(ii).Cells(1).Value.ToString()
                    BranchGrid.Rows(i).Cells(2).Value = MainBranchGrid.Rows(ii).Cells(2).Value.ToString()
                Next
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.PriceSales.OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New PriceSale
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
    Sub ADDPRICESALE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddpriceSale", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@CreateDate", Convert.ToDateTime(TxtDayDate.Value))
        comm.Parameters.AddWithValue("@StartIn", TxtFromDate.Text)
        comm.Parameters.AddWithValue("@EndIn", TxtToDate.Text)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEPRICESALE()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdatePriceSale", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@CreateDate", Convert.ToDateTime(TxtDayDate.Value))
        comm.Parameters.AddWithValue("@StartIn", TxtFromDate.Text)
        comm.Parameters.AddWithValue("@EndIn", TxtToDate.Text)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWPRICESALEGUID()
        Dim REGDOC_GUID_ = dbcontext.PriceSales.FirstOrDefault(Function(m) m.Code = TxtCode.Text)
        TxtGuid.Text = REGDOC_GUID_.Guid.ToString()
    End Sub
    Sub ADDDPRICESALEDETAIL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddPriceSaleDetails", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(0).Value = Nothing Then
                Exit For
            Else
                comm.CommandType = CommandType.StoredProcedure
                comm.Parameters.AddWithValue("@PriceSaleGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@DiplomGuid", DiplomGrid.Rows(i).Cells(9).Value.ToString())
                comm.Parameters.AddWithValue("@DiplomCost", DiplomGrid.Rows(i).Cells(2).Value.ToString())
                comm.Parameters.AddWithValue("@DiplomDisVat", DiplomGrid.Rows(i).Cells(3).Value.ToString())
                comm.Parameters.AddWithValue("@DiplomDisValue", DiplomGrid.Rows(i).Cells(4).Value.ToString())
                comm.Parameters.AddWithValue("@DiplomTotal", DiplomGrid.Rows(i).Cells(5).Value.ToString())
                comm.Parameters.AddWithValue("@DiplomTaxVat", DiplomGrid.Rows(i).Cells(6).Value.ToString())
                comm.Parameters.AddWithValue("@DiplomTaxValue", DiplomGrid.Rows(i).Cells(7).Value.ToString())
                comm.Parameters.AddWithValue("@DiplomSubTotal", DiplomGrid.Rows(i).Cells(8).Value.ToString())
                comm.Parameters.AddWithValue("@DiplomType", DiplomGrid.Rows(i).Cells(10).Value.ToString())
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
    Sub ADDDPRICESALEBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddPriceSaleBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To BranchGrid.Rows.Count - 1
            If BranchGrid.Rows(i).Cells(0).Value = Nothing Then
                Exit For
            Else
                comm.CommandType = CommandType.StoredProcedure
                comm.Parameters.AddWithValue("@PriceSaleGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@BranchGuid", BranchGrid.Rows(i).Cells(2).Value.ToString())
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
    Sub SHOWPRICESALEINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowPriceSaleInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowPriceSaleInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDayDate.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                TxtFromDate.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtToDate.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(3).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(4)
                TxtGuid.Text = ds.Tables(0).Rows(i)(5).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWPRICESALEDETAIL()
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowPriceSaleDetail", connection)
        comm.Parameters.AddWithValue("@PriceSaleGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowPriceSaleDetail")
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
                DiplomGrid.Rows(COUNT__).Cells(10).Value = ds.Tables(0).Rows(i)(10).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWPRICESALEBRANCH()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowPriceSaleBranch", connection)
        comm.Parameters.AddWithValue("@PriceSaleGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowPriceSaleBranch")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub DELETEPRICESALEDETAIL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeletePriceSaleDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@PriceSaleGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETEPRICESALEBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeletePriceSaleBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@PriceSaleGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("pricesale") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDPRICESALE()
            SHOWPRICESALEGUID()
            ADDDPRICESALEDETAIL()
            ADDDPRICESALEBRANCH()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات العرض بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("pricesale") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEPRICESALE()
            DELETEPRICESALEDETAIL()
            ADDDPRICESALEDETAIL()
            DELETEPRICESALEBRANCH()
            ADDDPRICESALEBRANCH()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات العرض بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtFromDate.Text = Nothing Then
            MessageBox.Show("برجاء ادخال تاريخ البداية اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtToDate.Text = Nothing Then
            MessageBox.Show("برجاء ادخال تاريخ النهاية اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf BranchGrid.Rows.Count = 0 Then
            MessageBox.Show("برجاء اختيار الفروع المتاح بها العرض اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf DiplomGrid.Rows.Count = 0 Then
            MessageBox.Show("برجاء اختيار الخدمات المتاحه بالعرض اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Public Function NOCOST_() As Boolean
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(9).Value <> Nothing Then
                If DiplomGrid.Rows(i).Cells(8).Value = 0 Then
                    Return False
                End If
            End If
        Next
        Return True
    End Function
    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If NOCOST_() = False Then
                    MessageBox.Show("برجاء مراجعه تكاليف الخدمات", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            Dim w As New PriceSaleList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
            End If
            If TxtCode.Text <> Nothing Then
                SHOWPRICESALEINFO()
                If TxtGuid.Text <> Nothing Then
                    SHOWPRICESALEDETAIL()
                    SHOWPRICESALEBRANCH()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW__()
        TxtCode.Text = Nothing
        TxtNotes.Text = Nothing
        TxtGuid.Text = Nothing
        BranchGrid.Rows.Clear()
        DiplomGrid.Rows.Clear()
        ChkStaut.Value = True
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW__()
            EncodingPriceSale_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class