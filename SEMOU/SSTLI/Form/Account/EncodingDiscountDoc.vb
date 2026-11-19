Imports System.Data.SqlClient

Public Class EncodingDiscountDoc
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Dim OPENGUID As Guid
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub
    Sub SHOWBRANCHINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowBarnchInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtBranchCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowBarnchInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(12).ToString()
            Next
        End If
    End Sub
    Private Sub CmdFindBranch_Click(sender As Object, e As EventArgs) Handles CmdFindBranch.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtBranchGuid.Text <> Nothing Then
                SHOWBRANCHINFO()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub SHOWDISCOUNTTYPEINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("WhatDiscountType", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtDiscountTypeGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "WhatDiscountType")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                ChkDiscountPrf.Checked = ds.Tables(0).Rows(i)(0)
                TxtDicountType.Text = ds.Tables(0).Rows(i)(1).ToString()
            Next
        End If
    End Sub
    Sub HOWEDISCOUNT()
        SHOWDISCOUNTTYPEINFO()
        If TxtDicountType.Text = 1 Then ''''''''تسجيل
            CmdFindRegDoc.Enabled = True
            TxtRegDocAmount.ReadOnly = True
            If ChkDiscountPrf.Checked = True Then
                Label11.Visible = True
                Label8.Visible = True
                TxtDiscountTypeVat.Visible = True
                TxtAmount.ReadOnly = True
                TxtRegDocAmount.Visible = True
                Label6.Visible = True
            Else
                Label11.Visible = False
                Label8.Visible = False
                TxtDiscountTypeVat.Visible = False
                TxtAmount.ReadOnly = False
            End If
        Else
            CmdFindRegDoc.Enabled = False
            TxtRegDocGuid.Text = "00000000-0000-0000-0000-000000000000"
            'TxtAmount.Text = TxtDiscountTypeVat.Text
            Label11.Visible = False
            Label8.Visible = False
            TxtDiscountTypeVat.Visible = False
            TxtAmount.ReadOnly = False
            TxtRegDocAmount.Visible = False
            Label6.Visible = False
        End If
    End Sub
    Private Sub CmdFindDiscountType_Click(sender As Object, e As EventArgs) Handles CmdFindDiscountType.Click
        Try
            Dim w As New DiscountTypeList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDiscountTypeName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDiscountTypeVat.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                TxtDiscountAccountGuid.Text = w.BranchGrid.Rows(ii).Cells(4).Value.ToString()
                TxtDiscountTypeGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
            End If
            If TxtDiscountTypeGuid.Text <> Nothing Then
                HOWEDISCOUNT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindStudent_Click(sender As Object, e As EventArgs) Handles CmdFindStudent.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New StudentList
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtStudentName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtStudentGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindRegDoc_Click(sender As Object, e As EventArgs) Handles CmdFindRegDoc.Click
        Try
            If TxtStudentGuid.Text <> Nothing Then
                Dim w As New StudentRegDocList
                w.TxtStudentGuid.Text = TxtStudentGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtRegDocCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    TxtRegDocGuid.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                    TxtRegDocAmount.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtRegDocGuid.Text <> Nothing Then
                    If ChkDiscountPrf.Checked = True Then
                        TxtAmount.Text = Val(TxtRegDocAmount.Text) * (Val(TxtDiscountTypeVat.Text) / 100)
                    End If
                End If
            Else
                MessageBox.Show("برجاء اختيار الطالب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub EncodingDiscountDoc_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("discount")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("opendaily")
            TxtOpenDailyType.Text = RegDocPosted.TYPEGUID
            OPENGUID = Guid.Parse(TxtOpenDailyType.Text)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Public Function GET_DISCOUNT__NO() As Integer
        Dim MaxId = dbcontext.StudentDiscounts.OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New StudentDiscount
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.Id = MaxId
            TxtCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDDISCOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddStudentDiscount", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET_DISCOUNT__NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@ActionDate", Convert.ToDateTime(TxtDayDate.Value))
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtStudentGuid.Text)
        comm.Parameters.AddWithValue("@DiscountTypeGuid", TxtDiscountTypeGuid.Text)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        comm.Parameters.AddWithValue("@Amount", Val(TxtAmount.Text))
        comm.Parameters.AddWithValue("@ActionNotes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEDISCOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateStudentDiscount", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@ActionDate", Convert.ToDateTime(TxtDayDate.Value))
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtStudentGuid.Text)
        comm.Parameters.AddWithValue("@DiscountTypeGuid", TxtDiscountTypeGuid.Text)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtRegDocGuid.Text)
        comm.Parameters.AddWithValue("@Amount", Val(TxtAmount.Text))
        comm.Parameters.AddWithValue("@ActionNotes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Public Function GETDAY1_DISCOUNT_NO() As Integer
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
    Sub ADDDISCOUNTDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GETDAY1_DISCOUNT_NO())
        comm.Parameters.AddWithValue("@Code", TxtDay1Code.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDayDate.Value))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
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
    Sub SHOWDISCOUNTDAY1INFO()
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
    Sub ADDDISCOUNTDAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtDiscountAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", Val(TxtAmount.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDDISCOUNTDAY2_DAEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtStudentGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtAmount.Text))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
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
    Sub SHOWDISCOUNTGUID()
        Dim REGDOC_GUID_ = dbcontext.StudentDiscounts.FirstOrDefault(Function(m) m.Code = TxtCode.Text)
        TxtGuid.Text = REGDOC_GUID_.Guid.ToString()
    End Sub
    Sub UPDATEDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtDay1Guid.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDayDate.Value))
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
    Sub SHOWDISCOUNTINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowStudentDiscountInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowStudentDiscountInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDayDate.Text = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtStudentName.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtStudentGuid.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtDiscountTypeName.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtDiscountTypeGuid.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtDiscountTypeVat.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtRegDocCode.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtRegDocAmount.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtRegDocGuid.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtAmount.Text = ds.Tables(0).Rows(i)(12).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(13).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(14)
                TxtGuid.Text = ds.Tables(0).Rows(i)(15).ToString()
                TxtDay1Code.Text = ds.Tables(0).Rows(i)(16).ToString()
                TxtDay1Guid.Text = ds.Tables(0).Rows(i)(17).ToString()
                TxtDiscountAccountGuid.Text = ds.Tables(0).Rows(i)(18).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("discount") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDDISCOUNT()
            SHOWDISCOUNTGUID()
            ADDDISCOUNTDAY1()
            SHOWDISCOUNTDAY1INFO()
            ADDDISCOUNTDAY2_MADEN()
            ADDDISCOUNTDAY2_DAEN()
            ADD_USERACTION_()
            SHOWDISCOUNTINFO()
            MessageBox.Show("تم حفظ سند الخصم بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("discount") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEDISCOUNT()
            UPDATEDAY1()
            DELETEDAY2()
            ADDDISCOUNTDAY2_MADEN()
            ADDDISCOUNTDAY2_DAEN()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل سند الخصم بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtStudentGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الطالب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtDiscountTypeGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار نوع الخصم", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtDicountType.Text = 1 Then
            If TxtRegDocGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار استمارة التسجيل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
        ElseIf TxtNotes.Text = Nothing Then
                MessageBox.Show("برجاء ادخال البيان الخاص بعملية الخصم", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function

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
    Sub NEW__()
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        TxtCostCenterGuid.Text = Nothing
        TxtStudentName.Text = Nothing
        TxtStudentGuid.Text = Nothing
        TxtDiscountTypeName.Text = Nothing
        TxtDiscountTypeGuid.Text = Nothing
        TxtDiscountTypeVat.Text = 0
        TxtRegDocCode.Text = Nothing
        TxtRegDocAmount.Text = 0
        TxtRegDocGuid.Text = Nothing
        TxtAmount.Text = 0
        TxtNotes.Text = Nothing
        ChkStaut.Value = Nothing
        TxtGuid.Text = Nothing
        TxtDay1Code.Text = Nothing
        TxtDay1Guid.Text = Nothing
        TxtCode.Text = Nothing
        Label11.Visible = True
        Label8.Visible = True
        TxtDiscountTypeVat.Visible = True
        TxtAmount.ReadOnly = True
        TxtRegDocAmount.Visible = True
        Label6.Visible = True
        ChkDiscountPrf.Checked = False
        TxtDicountType.Text = Nothing
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW__()
            EncodingDiscountDoc_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            Dim w As New StudentDiscountList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
            End If
            If TxtCode.Text <> Nothing Then
                SHOWDISCOUNTINFO()
                HOWEDISCOUNT()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class