Imports System.Data.SqlClient

Public Class EncodingPriceList
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Sub LOADMAINDIPLOM()
        MainDiplomGrid.Rows.Clear()
        Dim NAME As String = Nothing
        Dim ISUSE As String = Nothing
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadDiplom", connection)
        If TxtFindDiploam.Text <> Nothing Then
            NAME = TxtFindDiploam.Text
            comm.Parameters.AddWithValue("@DiplomName", NAME)
        Else
            comm.Parameters.AddWithValue("@DiplomName", NAME)
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
    Private Sub CmdExit_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub CmdExit_Click_1(sender As Object, e As EventArgs) Handles CmdExit.Click

    End Sub

    Private Sub EncodingPriceList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADMAINDIPLOM()
            'LOADMAINFEES()
            RegDocPosted.WHATFORMTYPEGUID("addpricelist")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
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
                    If row.Cells(6).Value = MainDiplomGrid.Rows(ii).Cells(2).Value.ToString Then
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
                    DiplomGrid.Rows(i).Cells(3).Value = MainDiplomGrid.Rows(ii).Cells(4).Value.ToString()
                    DiplomGrid.Rows(i).Cells(4).Value = NOVALYE
                    DiplomGrid.Rows(i).Cells(5).Value = NOVALYE
                    DiplomGrid.Rows(i).Cells(6).Value = MainDiplomGrid.Rows(ii).Cells(2).Value.ToString()
                    DiplomGrid.Rows(i).Cells(7).Value = MainDiplomGrid.Rows(ii).Cells(3).Value.ToString()
                Next
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFindDiploam_TextChanged(sender As Object, e As EventArgs) Handles TxtFindDiploam.TextChanged
        Try
            LOADMAINDIPLOM()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFindFees_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub MainFeesGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub MainFeesGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
    End Sub

    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.PriceLists.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New PriceList
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
        If TxtListName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الدبلوم - الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf DiplomGrid.Rows.Count = 0 Then
            MessageBox.Show("برجاء ادخال الدبلوم - الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf ChkOfferPrice.Checked = True Then
            Dim dbcontext As New SemouEntities
            Dim ASKIFFONDDEF = dbcontext.PriceLists.FirstOrDefault(Function(m) m.ChkOfferPrice = True)
            If (IsNothing(ASKIFFONDDEF)) Then
            Else
                MessageBox.Show("توجد بالفعل قائمة مخصصة لعرض الاسعار", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
        End If
        Return True
    End Function
    Sub ADDPRICELIST()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddPriceList", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@ListName", TxtListName.Text)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@ChkOfferPrice", ChkOfferPrice.Checked)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEPRICELIST()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdatePriceList", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@ListName", TxtListName.Text)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@ChkOfferPrice", ChkOfferPrice.Checked)
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
    Sub SHOWLISTINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowPriceListInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowPriceListInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtListName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(1).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(2)
                TxtGuid.Text = ds.Tables(0).Rows(i)(3).ToString()
                ChkOfferPrice.Checked = ds.Tables(0).Rows(i)(4)
            Next
        End If
    End Sub
    Sub SHOWLISTDIPLOM()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowPriceListDiplom", connection)
        comm.Parameters.AddWithValue("@PriceListGuid", TxtGuid.Text)
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
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDLISTDIPLOM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddPriceListDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(6).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@PriceListGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@DiplomCost", Val(DiplomGrid.Rows(i).Cells(2).Value))
                comm.Parameters.AddWithValue("@DiplomTaxVat", Val(DiplomGrid.Rows(i).Cells(3).Value))
                comm.Parameters.AddWithValue("@DiplomTaxValue", Val(DiplomGrid.Rows(i).Cells(4).Value))
                comm.Parameters.AddWithValue("@DiplomSubTotal", Val(DiplomGrid.Rows(i).Cells(5).Value))
                comm.Parameters.AddWithValue("@DiplomGuid", DiplomGrid.Rows(i).Cells(6).Value.ToString())
                comm.Parameters.AddWithValue("@DiplomType", DiplomGrid.Rows(i).Cells(7).Value.ToString())
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
    Sub DELETELISTDIPLOM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeletePriceListDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@PriceListGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        Dim rs As Integer = comm.ExecuteNonQuery()
        If rs > 0 Then
            comm.Parameters.Clear()
        End If
    End Sub
    Sub DELETELISTFEES()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeletePriceListFees", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@PriceListGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        Dim rs As Integer = comm.ExecuteNonQuery()
        If rs > 0 Then
            comm.Parameters.Clear()
        End If
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("addpricelist") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDPRICELIST()
            SHOWLISTINFO()
            ADDLISTDIPLOM()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ قائمة الاسعار بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addpricelist") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEPRICELIST()
            DELETELISTDIPLOM()
            ADDLISTDIPLOM()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل قائمة الاسعار بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
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

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            If AllFiles.FORFIND("addpricelist") = False Then
                Return
            Else
                Dim w As New PriceList
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    TxtGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtGuid.Text <> Nothing Then
                    SHOWLISTINFO()
                    SHOWLISTDIPLOM()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW___()
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        TxtListName.Text = Nothing
        TxtNotes.Text = Nothing
        DiplomGrid.Rows.Clear()
        ChkStaut.Value = True
        ChkOfferPrice.Checked = False
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingPriceList_Load(e, e)
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
            If j = 2 Then
                If Val(DiplomGrid.Rows(i).Cells(3).Value) <> 0 Then
                    DiplomGrid.Rows(i).Cells(4).Value = Math.Round(Val(DiplomGrid.Rows(i).Cells(2).Value) * (Val(DiplomGrid.Rows(i).Cells(3).Value) / 100), 3)
                    DiplomGrid.Rows(i).Cells(5).Value = Val(DiplomGrid.Rows(i).Cells(2).Value) + Val(DiplomGrid.Rows(i).Cells(4).Value)
                Else
                    DiplomGrid.Rows(i).Cells(5).Value = DiplomGrid.Rows(i).Cells(2).Value
                End If
            ElseIf j = 5 Then
                If Val(DiplomGrid.Rows(i).Cells(3).Value) <> 0 Then
                    Dim VAT As Double = (1 + Val(DiplomGrid.Rows(i).Cells(3).Value) / 100)
                    DiplomGrid.Rows(i).Cells(2).Value = Math.Round(Val(DiplomGrid.Rows(i).Cells(5).Value) / Val(VAT), 3)
                    DiplomGrid.Rows(i).Cells(4).Value = Math.Round(Val(DiplomGrid.Rows(i).Cells(2).Value) * (Val(DiplomGrid.Rows(i).Cells(3).Value) / 100), 3)
                Else
                    DiplomGrid.Rows(i).Cells(2).Value = DiplomGrid.Rows(i).Cells(5).Value
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class