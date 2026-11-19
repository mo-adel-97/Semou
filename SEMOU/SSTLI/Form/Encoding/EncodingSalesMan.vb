Imports System.Data.SqlClient

Public Class EncodingSalesMan
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
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
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub EncodingSalesMan_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADBRANCH()
            RegDocPosted.WHATFORMTYPEGUID("addsalesman")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
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
        Dim MaxId = dbcontext.SalesMen.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New SalesMan
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
        If TxtFullName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم المندوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtUseName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال الاسم المختصر للمندوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf BranchGrid.Rows.Count = 0 Then
            MessageBox.Show("برجاء اختيار الفروع المسموح للمندوب التسجيل فيها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub ADDSALESMAN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddSalesMan", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@ManFullName", TxtFullName.Text)
        comm.Parameters.AddWithValue("@ManUseName", TxtUseName.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATESALESMAN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateSalesMan", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@ManFullName", TxtFullName.Text)
        comm.Parameters.AddWithValue("@ManUseName", TxtUseName.Text)
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
    Sub SHOWSALESMANINF()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowSalesManInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowSalesManInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtFullName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtUseName.Text = ds.Tables(0).Rows(i)(1).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(2)
                TxtGuid.Text = ds.Tables(0).Rows(i)(3).ToString()
            Next
        End If
    End Sub
    Sub ADDSALESMANBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddSalesManBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To BranchGrid.Rows.Count - 1
            If BranchGrid.Rows(i).Cells(2).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@SalesManGuid", TxtGuid.Text)
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
    Sub DELETESALESMANBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteSalesManBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@SalesManGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        Dim rs As Integer = comm.ExecuteNonQuery()
        If rs > 0 Then
            comm.Parameters.Clear()
        End If
    End Sub
    Sub SHOWSALESMANBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowSalesManBranch", connection)
        comm.Parameters.AddWithValue("@SalesManGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowSalesManBranch")
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
    Sub FORSAVE()
        If AllFiles.FORSAVE("addsalesman") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDSALESMAN()
            SHOWSALESMANINF()
            ADDSALESMANBRANCH()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات مندوب البيع بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addsalesman") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATESALESMAN()
            DELETESALESMANBRANCH()
            ADDSALESMANBRANCH()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات مندوب البيع بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
    Sub NEW___()
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        TxtFullName.Text = Nothing
        TxtUseName.Text = Nothing
        ChkStaut.Value = True
        BranchGrid.Rows.Clear()
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingSalesMan_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            If AllFiles.FORFIND("addsalesman") = False Then
                Return
            Else
                Dim w As New SalesManList
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    TxtGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtGuid.Text <> Nothing Then
                    SHOWSALESMANINF()
                    SHOWSALESMANBRANCH()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class