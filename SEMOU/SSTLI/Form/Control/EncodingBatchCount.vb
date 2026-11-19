Imports System.Data.SqlClient

Public Class EncodingBatchCount
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub


    Private Sub EncodingBatchCount_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("addbatchcount")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindBranch_Click(sender As Object, e As EventArgs) Handles CmdFindBranch.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.TxtIsRegOn.Text = True
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
    Sub LOADCOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("WhatBAtchCount", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "WhatBAtchCount")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtMainCount.Text = ds.Tables(0).Rows(i)(0).ToString()
            Next
        Else
            TxtMainCount.Text = 0
        End If
    End Sub
    Sub LOADDIPLOMCOUNT()
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadDiplomCountSeat", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadDiplomCountSeat")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            DiplomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DiplomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                DiplomGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                DiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                COUNT__ = COUNT__ + 1
            Next
        Else
            LOADDIPLOM()
        End If
    End Sub
    Sub COUNTTOTAL()
        Dim DIPLOMTAX As Double = 0
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(2).Value <> Nothing Then
                DIPLOMTAX = Val(DIPLOMTAX) + (Val(DiplomGrid.Rows(i).Cells(1).Value))
            End If
        Next
        TxtGridCount.Text = DIPLOMTAX.ToString()
        TxtNowCount.Text = Val(TxtMainCount.Text) - Val(TxtGridCount.Text)
    End Sub
    Sub LOADDIPLOM()
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadDiplom", connection)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@DiplomType", 0)
        comm.Parameters.AddWithValue("@StdyType", 0)
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
            DiplomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DiplomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(1).ToString()
                DiplomGrid.Rows(COUNT__).Cells(1).Value = 0
                DiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(3).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
        COUNTTOTAL()
    End Sub
    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New BatchList
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtBatchCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    TxtBatchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtBatchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtBatchGuid.Text <> Nothing Then
                    LOADCOUNT()
                    LOADDIPLOMCOUNT()
                    COUNTTOTAL()
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
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
            If j = 1 Then
                If DiplomGrid.Rows(i).Cells(1).Value > Val(TxtNowCount.Text) Then
                    MessageBox.Show("العدد الذي قمت بادخاله غير متاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    DiplomGrid.Rows(i).Cells(1).Value = TxtNowCount.Text
                    COUNTTOTAL()
                Else
                    COUNTTOTAL()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtBatchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الدفعه اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub ADDSTUDENTCOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDiplomCountSeat", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(2).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
                comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
                comm.Parameters.AddWithValue("@DiplomCount", DiplomGrid.Rows(i).Cells(1).Value.ToString())
                comm.Parameters.AddWithValue("@DiplomGuid", DiplomGrid.Rows(i).Cells(2).Value.ToString())
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
        comm.Parameters.AddWithValue("@ActionGuid", TxtBatchGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
        comm.Parameters.AddWithValue("@Date_", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@Time_", Convert.ToDateTime(DateTime.Now.ToShortTimeString))
        comm.Parameters.AddWithValue("@Notes_", AllFiles.ActionType___)
        comm.Parameters.AddWithValue("@ActionTypeGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionReason", REASON)
        comm.Parameters.AddWithValue("@ActionCode", TxtBatchCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETECOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteDiplomCountSeat", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addbatchcount") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            DELETECOUNT()
            ADDSTUDENTCOUNT()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل الدفعة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                Dim w As New UpdateResaon
                w.ShowDialog()
                If w.IS_Yes = True Then
                    REASON = w.TxtNotes.Text
                    FORUPDATE()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class