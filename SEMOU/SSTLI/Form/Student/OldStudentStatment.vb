Imports System.Data.SqlClient

Public Class OldStudentStatment
    Dim Olddbcontext As New SSTLI2023Entities
    Sub LOADOLDSTATMENT(ACCCODE As String, BRNID As Integer)
        CashGrid.Rows.Clear()
        Dim connection = Olddbcontext.Database.Connection
        Dim comm As New SqlCommand("OldStudentStatment", connection)
        comm.Parameters.AddWithValue("@CustAcctCode", ACCCODE)
        comm.Parameters.AddWithValue("@BranId", BRNID)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "OldStudentStatment")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            CashGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                CashGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                CashGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                CashGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                CashGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                CashGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                CashGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                CashGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                CashGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                COUNT__ = COUNT__ + 1
            Next
            PAYTOTAL_()
        End If
    End Sub
    Sub PAYTOTAL_()
        Dim MADEN__ As Double = 0
        Dim DAEN__ As Double = 0

        For i As Integer = 0 To CashGrid.Rows.Count - 1
            If CashGrid.Rows(i).Cells(0).Value <> Nothing Then
                MADEN__ = Val(MADEN__) + Val(CashGrid.Rows(i).Cells(2).Value)
                DAEN__ = Val(DAEN__) + Val(CashGrid.Rows(i).Cells(3).Value)
            End If
        Next
        TxtPayMaden.Text = Math.Round(MADEN__, 2)
        TxtPayDaen.Text = Math.Round(DAEN__, 2)
    End Sub
    Sub LOADOLDREG(ACCCODE As String, BRNID As Integer)
        RegGrid.Rows.Clear()
        Dim connection = Olddbcontext.Database.Connection
        Dim comm As New SqlCommand("OldStudentReg", connection)
        comm.Parameters.AddWithValue("@CustAcctCode", ACCCODE)
        comm.Parameters.AddWithValue("@BrnId", BRNID)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "OldStudentReg")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            RegGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                RegGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                RegGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                RegGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                RegGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                RegGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                RegGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                RegGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                COUNT__ = COUNT__ + 1
            Next
            REGTOTAL_()
        End If
    End Sub
    Sub REGTOTAL_()
        Dim MADEN__ As Double = 0
        Dim DAEN__ As Double = 0

        For i As Integer = 0 To RegGrid.Rows.Count - 1
            If RegGrid.Rows(i).Cells(0).Value <> Nothing Then
                MADEN__ = Val(MADEN__) + Val(RegGrid.Rows(i).Cells(3).Value)
                DAEN__ = Val(DAEN__) + Val(RegGrid.Rows(i).Cells(4).Value)
            End If
        Next
        TxtRegMaden.Text = Math.Round(MADEN__, 2)
        TxtRegDaen.Text = Math.Round(DAEN__, 2)
    End Sub
    Sub LOADOPENBALANCE(ACCCODE As String, BRNID As String)
        Dim connection = Olddbcontext.Database.Connection
        Dim comm As New SqlCommand("GetOpenBalance", connection)
        comm.Parameters.AddWithValue("@CustAcctCode", ACCCODE)
        comm.Parameters.AddWithValue("@BranId", BRNID)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "GetOpenBalance")
        Dim COUNT__ As Integer = 0
        For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
            If ds.Tables(0).Rows.Count > 0 Then
                TxtOpenMaden.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtOpenDaen.Text = ds.Tables(0).Rows(i)(1).ToString()
            End If
        Next
    End Sub
    Sub LASTTOTAL()
        TxtMaden.Text = Val(TxtRegMaden.Text) + Val(TxtPayMaden.Text) + Val(TxtOpenMaden.Text)
        TxtDaen.Text = Val(TxtRegDaen.Text) + Val(TxtPayDaen.Text) + Val(TxtOpenDaen.Text)
        TxtBalance.Text = Val(TxtMaden.Text) - Val(TxtDaen.Text)
    End Sub
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub OldStudentStatment_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class