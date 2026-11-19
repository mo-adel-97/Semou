Imports System.Data.SqlClient
Imports System.IO
Imports DevExpress.XtraReports.UI
Imports System.Globalization
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Formatting = Newtonsoft.Json.Formatting

Public Class RegisterForm
    Dim dbcontext As New SemouEntities
    Dim NOTES As String
    Dim REASON As String = ""
    Dim OPENGUID As Guid
    Public IS_Yes As Boolean = False
    Public IS_Ok As Boolean = False
    Dim NOGUID As String = "00000000-0000-0000-0000-000000000000"
    Private client As HttpClient
    Dim jsonResult As String

    Private Async Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdNewSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                CHKACCOUNT()
                If TxtGuid.Text = Nothing Then
                    AllFiles.WHATONLINEUSERINFO()
                    TxtUserName.Text = AllFiles.ONLINE_USER_NAME____
                    TxtUserGuid.Text = AllFiles.USER_GUID____.ToString()
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''  الجزء الخاص بمراجعه اذا كان هناك مقاعد متاحة للدبلوم ام لا
                    If TxtRegType.Text = 0 Then
                        If AllFiles.ASKDIPLOMSEAT(TxtBranchGuid.Text, TxtBatchGuid.Text, TxtDiplomGuid.Text) = False Then
                            MessageBox.Show("عذرا التخصص الذي قمت باختيار لا تتوفر اى مقاعد للتسجيل فية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        Else
                            FORSAVE()
                        End If
                    Else
                        FORSAVE()
                    End If
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

    Private Sub CmdFindStudent_Click(sender As Object, e As EventArgs) Handles CmdFindStudent.Click
        Try
            Dim w As New MainFindStudent
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindBatch_Click(sender As Object, e As EventArgs) Handles CmdFindBatch.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New BatchAvailableSeats
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtBatchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtBatchGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار نوع المستند اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub CHKACCOUNT()
        RegDocPosted.SHOWREFSETTINGINFO(TxtDocCode.Text, Guid.Parse(TxtTypeGuid.Text))
        TxtBranchName.Text = RegDocPosted.BRANCHNAME.ToString()
        TxtBranchGuid.Text = RegDocPosted.BRANCHGUID.ToString()
        TxtPriceListGuid.Text = RegDocPosted.PRICELISTGUID.ToString()
        ChkOtherFees.Checked = RegDocPosted.CHKOTHERFEES
        ChkIfNeedPay.Checked = RegDocPosted.CHKSTARTPAY
        TxtVatGuid.Text = RegDocPosted.VATACCOUNTGUID.ToString()
        ChkVat.Checked = RegDocPosted.CHKVAT
        If ChkIfNeedPay.Checked = True Then
            TxtPayDocName.Text = RegDocPosted.DOCPAYNAME.ToString()
            TxtPayDocGuid.Text = RegDocPosted.DOCPAYGUID.ToString()
            TxtPayBranchName.Text = RegDocPosted.DOCBRANCHNAME.ToString()
            TxtPayBranchGuid.Text = RegDocPosted.DOCBRANCHGUID.ToString()
            ChkPayAmount.Checked = RegDocPosted.CHKPAYAMOUNT
            TxtPayAmount.Text = RegDocPosted.PAYAMOUNT.ToString()
            TxtPayDocCode.Text = RegDocPosted.PAYDOCCODE.ToString()
        End If
    End Sub
    Sub CHKWHATREGTYPE()
        CHKACCOUNT()
        If TxtRegType.Text = 0 Then
            TxtCourseName.Text = Nothing
            CmdFindCourse.Enabled = False
            CmdFindBatch.Enabled = True
            CmdFindLevel.Enabled = True
            TxtCourseGuid.Text = "00000000-0000-0000-0000-000000000000"
            TxtBatchGuid.Text = Nothing
            SHOWDEFLEVEL()
            If AllFiles.WhatPriceSaleGuidByBranch(TxtBranchGuid.Text) = True Then
                TxtPriceSaleGuid.Text = AllFiles.PRICESALEGUID.ToString()
                SHOWPRICESALEDIPLOM(0)
            Else
                SHOWLISTDIPLOM(0)
            End If
        Else
            TxtBatchName.Text = Nothing
            TxtLevelName.Text = Nothing
            CmdFindBatch.Enabled = False
            CmdFindLevel.Enabled = False
            CmdFindCourse.Enabled = True
            TxtCourseGuid.Text = Nothing
            TxtBatchGuid.Text = "00000000-0000-0000-0000-000000000000"
            TxtLevelGuid.Text = "00000000-0000-0000-0000-000000000000"
            If AllFiles.WhatPriceSaleGuidByBranch(TxtBranchGuid.Text) = True Then
                'TxtPriceSaleGuid.Text = AllFiles.PRICESALEGUID.ToString()
                'SHOWPRICESALEDIPLOM(1)
                'If TxtCourseGuid.Text <> Nothing Then
                '    SHOWPRICESALEONLYCOURSE(1, TxtCourseDiplomGuid.Text)
                'End If
                TxtFindDiploam.Enabled = False
            Else
                SHOWLISTDIPLOM(1)
            End If
        End If
        If ChkOtherFees.Checked = True Then
            SHOWLISTFEES()
        End If
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
                TxtRegType.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtDocGuid.Text <> Nothing Then
                CHKWHATREGTYPE()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub SHOWLISTDIPLOM(STUDYTYPE As Integer)
        MainDiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowPriceListDiplom", connection)
        comm.Parameters.AddWithValue("@PriceListGuid", TxtPriceListGuid.Text)
        comm.Parameters.AddWithValue("@Type", 0)
        If TxtFindDiploam.Text <> Nothing Then
            comm.Parameters.AddWithValue("@DiplomName", TxtFindDiploam.Text)
        End If
        comm.Parameters.AddWithValue("@StudyType", STUDYTYPE)
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
            MainDiplomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MainDiplomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MainDiplomGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                If TxtstudentNational.Text = 0 Then     ''''''' مواطن
                    MainDiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                    MainDiplomGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(4).ToString()
                    MainDiplomGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(5).ToString()
                Else '''''''' أجنبي
                    Dim VAT As Double = (1 + 15 / 100)
                    MainDiplomGrid.Rows(COUNT__).Cells(2).Value = Math.Round(Val(ds.Tables(0).Rows(i)(2)) / Val(VAT), 3)
                    MainDiplomGrid.Rows(COUNT__).Cells(3).Value = Math.Round(Val(MainDiplomGrid.Rows(i).Cells(2).Value) * (15 / 100), 3)
                    MainDiplomGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(5).ToString()
                End If
                MainDiplomGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(6).ToString()
                MainDiplomGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(3).ToString()
                MainDiplomGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWPRICESALEDIPLOM(STUDYTYPE As String)
        MainDiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowPriceSaleDiplom", connection)
        comm.Parameters.AddWithValue("@PriceSaleGuid", TxtPriceSaleGuid.Text)
        comm.Parameters.AddWithValue("@Type", 0)
        If TxtFindDiploam.Text <> Nothing Then
            comm.Parameters.AddWithValue("@DiplomName", TxtFindDiploam.Text)
        End If
        comm.Parameters.AddWithValue("@StudyType", STUDYTYPE)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowPriceSaleDiplom")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MainDiplomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MainDiplomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MainDiplomGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                If TxtstudentNational.Text = 0 Then     ''''''' مواطن
                    MainDiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                    MainDiplomGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(4).ToString()
                    MainDiplomGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(5).ToString()
                Else '''''''' أجنبي
                    Dim VAT As Double = (1 + 15 / 100)
                    MainDiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                    MainDiplomGrid.Rows(COUNT__).Cells(3).Value = Math.Round(Val(MainDiplomGrid.Rows(i).Cells(2).Value) * (15 / 100), 3)
                    MainDiplomGrid.Rows(COUNT__).Cells(4).Value = Math.Round(Val(MainDiplomGrid.Rows(COUNT__).Cells(2).Value) + Val(MainDiplomGrid.Rows(COUNT__).Cells(3).Value), 3)
                End If
                MainDiplomGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(6).ToString()
                MainDiplomGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(3).ToString()
                MainDiplomGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWPRICESALEONLYCOURSE(STUDYTYPE As Integer, DIPLOMGUID As String)
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowPriceSaleDiplom", connection)
        comm.Parameters.AddWithValue("@PriceSaleGuid", TxtPriceSaleGuid.Text)
        comm.Parameters.AddWithValue("@Type", 0)
        If TxtFindDiploam.Text <> Nothing Then
            comm.Parameters.AddWithValue("@DiplomName", TxtFindDiploam.Text)
        End If
        comm.Parameters.AddWithValue("@StudyType", STUDYTYPE)
        comm.Parameters.AddWithValue("@DiplomGuid", DIPLOMGUID)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowPriceSaleDiplom")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            DiplomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DiplomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                DiplomGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                DiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                DiplomGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(4).ToString()
                DiplomGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(5).ToString()
                DiplomGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(6).ToString()
                'MainDiplomGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(3).ToString()
                'MainDiplomGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWLISTFEES()
        MainFeesGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowPriceListDiplom", connection)
        comm.Parameters.AddWithValue("@PriceListGuid", TxtPriceListGuid.Text)
        comm.Parameters.AddWithValue("@Type", 1)
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

    Private Sub TxtFindDiploam_TextChanged(sender As Object, e As EventArgs) Handles TxtFindDiploam.TextChanged
        Try
            If TxtPriceListGuid.Text <> Nothing Then
                If TxtRegType.Text = 0 Then
                    If AllFiles.WhatPriceSaleGuidByBranch(TxtBranchGuid.Text) = True Then
                        TxtPriceSaleGuid.Text = AllFiles.PRICESALEGUID.ToString()
                        SHOWPRICESALEDIPLOM(0)
                    Else
                        SHOWLISTDIPLOM(0)
                    End If
                Else
                    If AllFiles.WhatPriceSaleGuidByBranch(TxtBranchGuid.Text) = True Then
                        TxtPriceSaleGuid.Text = AllFiles.PRICESALEGUID.ToString()
                        SHOWPRICESALEDIPLOM(1)
                    Else
                        SHOWLISTDIPLOM(1)
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFindFees_TextChanged(sender As Object, e As EventArgs) Handles TxtFindFees.TextChanged
        Try
            If TxtPriceListGuid.Text <> Nothing Then
                SHOWLISTFEES()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub MainDiplomGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainDiplomGrid.CellContentClick

    End Sub

    Private Sub MainDiplomGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainDiplomGrid.CellDoubleClick
        Try
            Dim row As DataGridViewRow
            Dim found As Boolean = False
            Dim ii As Integer = MainDiplomGrid.CurrentRow.Index
            For Each row In DiplomGrid.Rows
                For i As Integer = 0 To DiplomGrid.Rows.Count - 1
                    If row.Cells(5).Value = MainDiplomGrid.Rows(ii).Cells(5).Value.ToString Then
                        found = True
                        MessageBox.Show("تم اختيار الدبلوم - الدورة مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
                    DiplomGrid.Rows(i).Cells(2).Value = MainDiplomGrid.Rows(ii).Cells(2).Value.ToString()
                    DiplomGrid.Rows(i).Cells(3).Value = MainDiplomGrid.Rows(ii).Cells(3).Value.ToString()
                    DiplomGrid.Rows(i).Cells(4).Value = MainDiplomGrid.Rows(ii).Cells(4).Value.ToString()
                    DiplomGrid.Rows(i).Cells(5).Value = MainDiplomGrid.Rows(ii).Cells(5).Value.ToString()
                    TxtDiplomName.Text = MainDiplomGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtDiplomGuid.Text = MainDiplomGrid.Rows(ii).Cells(5).Value.ToString()
                Next
                If ChkIfNeedPay.Checked = True Then
                    PayGrid.Rows.Clear()
                    For Each row In MainDiplomGrid.SelectedRows
                        PayGrid.Rows.Add()
                        Dim i As Integer = PayGrid.Rows.Count() - 1
                        PayGrid.Rows(i).Cells(0).Value = MainDiplomGrid.Rows(ii).Cells(0).Value.ToString()
                        PayGrid.Rows(i).Cells(1).Value = MainDiplomGrid.Rows(ii).Cells(1).Value.ToString()
                        PayGrid.Rows(i).Cells(2).Value = "PCS"
                        PayGrid.Rows(i).Cells(3).Value = 1
                        PayGrid.Rows(i).Cells(4).Value = 0
                        If TxtRegType.Text = 0 Then
                            If TxtstudentNational.Text = 0 Then
                                PayGrid.Rows(i).Cells(5).Value = MainDiplomGrid.Rows(ii).Cells(6).Value.ToString()
                            Else
                                PayGrid.Rows(i).Cells(5).Value = 15
                            End If
                        Else
                            PayGrid.Rows(i).Cells(5).Value = MainDiplomGrid.Rows(ii).Cells(6).Value.ToString()
                        End If
                        PayGrid.Rows(i).Cells(6).Value = 0
                        PayGrid.Rows(i).Cells(7).Value = 0
                        PayGrid.Rows(i).Cells(8).Value = MainDiplomGrid.Rows(ii).Cells(5).Value.ToString()
                        PayGrid.Rows(i).Cells(9).Value = MainDiplomGrid.Rows(ii).Cells(7).Value.ToString()
                    Next
                End If
                DIPLOMTOTAL()
                FINALTOTAL()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub MainFeesGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainFeesGrid.CellContentClick

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
                    FeesGrid.Rows(i).Cells(2).Value = MainFeesGrid.Rows(ii).Cells(2).Value.ToString()
                    FeesGrid.Rows(i).Cells(3).Value = MainFeesGrid.Rows(ii).Cells(3).Value.ToString()
                    FeesGrid.Rows(i).Cells(4).Value = MainFeesGrid.Rows(ii).Cells(4).Value.ToString()
                    FeesGrid.Rows(i).Cells(5).Value = MainFeesGrid.Rows(ii).Cells(5).Value.ToString()
                Next
                FESSTOTAL()
                FINALTOTAL()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
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
        TxtDTotal1.Text = Math.Round(Val(DIPLOMTOTAL), 2)
        TxtDTotal2.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtDTotal3.Text = Math.Round(DIPLOMSUBTOTAL, 2)
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
        TxtFeesTotal1.Text = Math.Round(Val(FESSTOTAL), 2)
        TxtFeesTotal2.Text = Math.Round(Val(FESSTAX), 2)
        TxtFeesTotal3.Text = Math.Round(FESSSUBTOTAL, 2)
    End Sub

    Private Sub DiplomGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DiplomGrid.CellContentClick

    End Sub
    Sub FINALTOTAL()
        TxtFTotal1.Text = Val(TxtDTotal1.Text) + Val(TxtFeesTotal1.Text)
        TxtFTotal2.Text = Val(TxtDTotal2.Text) + Val(TxtFeesTotal2.Text)
        TxtFTotal3.Text = Val(TxtDTotal3.Text) + Val(TxtFeesTotal3.Text)
    End Sub
    Private Sub DiplomGrid_UserDeletedRow(sender As Object, e As DataGridViewRowEventArgs) Handles DiplomGrid.UserDeletedRow
        Try
            DIPLOMTOTAL()
            FINALTOTAL()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub FeesGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles FeesGrid.CellContentClick

    End Sub

    Private Sub FeesGrid_UserDeletedRow(sender As Object, e As DataGridViewRowEventArgs) Handles FeesGrid.UserDeletedRow
        Try
            FESSTOTAL()
            FINALTOTAL()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.RegDocs.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New RegDoc
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
    Public Function GET_PAY__NO() As Integer
        Dim MaxId = dbcontext.Bills.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Bill
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtPayCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtPayCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Public Function GET_RECEPIT__NO() As Integer
        Dim MaxId = dbcontext.CashDays.Where(Function(m) m.CashDayType = 0).OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New CashDay
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtRecepitCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.Id = MaxId
            TxtRecepitCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Public Function GET_RECEPIT2__NO() As Integer
        Dim MaxId = dbcontext.CashDays.Where(Function(m) m.CashDayType = 0).OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New CashDay
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtRecepitCode2.Text = MaxId
        Else
            MaxId += 1
            MaxDay.Id = MaxId
            TxtRecepitCode2.Text = MaxId
        End If
        Return MaxId
    End Function
    Public Function GETDAY1__NO() As Integer
        Dim MaxId = dbcontext.Day1.Where(Function(m) m.TypeGuid <> OPENGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
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
    Public Function GETDAY1_PAY_NO() As Integer
        Dim MaxId = dbcontext.Day1.Where(Function(m) m.TypeGuid <> OPENGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Day1
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtDay1PayCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtDay1PayCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Public Function GETDAY1_RECEPIT_NO() As Integer
        Dim MaxId = dbcontext.Day1.Where(Function(m) m.TypeGuid <> OPENGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Day1
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtDay1RecepitCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtDay1RecepitCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Public Function GETDAY1_RECEPIT2_NO() As Integer
        Dim MaxId = dbcontext.Day1.Where(Function(m) m.TypeGuid <> OPENGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Day1
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtDay1RecepitCode2.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtDay1RecepitCode2.Text = MaxId
        End If
        Return MaxId
    End Function
    Function VALIDATE_DATA() As Boolean
        If TxtDocGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار نوع المستند", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtBatchGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيارالدفعة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء مراجعة إعدادات التسجيل لعدم وجود فرع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtSalesManGuid.Text = Nothing Then
            MessageBox.Show("برجاءاختيار مندوب البيع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtLevelGuid.Text = Nothing Then
            MessageBox.Show("برجاءاختيار المستوي المراد تسجيل الطالب عليه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf DiplomGrid.Rows.Count = 0 Then
            MessageBox.Show("لا يمكن التسجيل بدون اختيار الدبلوم - الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf FeesGrid.Rows.Count = 0 And ChkOtherFees.Checked = True Then
            MessageBox.Show("لا يمكن التسجيل بدون اختيار رسوم الاختبار - الشهادة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCourseGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf ChkAttach.Checked = False Then
            If TxtGuid.Text = Nothing Then
                MessageBox.Show("لا يمكن التسجيل بدون ادخال المرفقات الخاصة بالتسجيل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
        ElseIf ChkIfNeedPay.Checked = True Then
            If TxtPayDocGuid.Text = Nothing Then
                MessageBox.Show("برجاء التواصل مع مدير النظام لانه لا يتوفر نوع مستند السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf TxtCostCenterGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار مركز تكلفة الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf TxtRef.Text = Nothing Then
                MessageBox.Show("برجاء ادخال رقم المرجع الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf TxtPayTotal.Text = 0 And TxtZeroAmount.Text = 0 Then
                MessageBox.Show("برجاء ادخال المبلغ الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf ChkPayDate.Checked = False Then
                MessageBox.Show("برجاء تاريخ الحوالة البنكية / السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf ChlOtherBank.Checked = True Then
                If TxtCostCenterGuid2.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار مركز تكلفة 2 الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                ElseIf TxtRef2.Text = Nothing Then
                    MessageBox.Show("برجاء ادخال رقم المرجع 2 الخاص بعملية السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                ElseIf ChkPayDate2.Checked = False Then
                    MessageBox.Show("برجاء تاريخ الحوالة 2 البنكية / السداد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                ElseIf TxtAmount1.Text = 0 Then
                    MessageBox.Show("برجاء ادخال قيمة الحوالة الاولي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                ElseIf TxtAmount2.Text = 0 Then
                    MessageBox.Show("برجاء ادخال قيمة الحوالة الثانية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                ElseIf (Val(TxtAmount1.Text) + Val(TxtAmount2.Text)) <> Val(TxtPaySubTotal.Text) Then
                    MessageBox.Show("المبالغ الخاصة بالحولات غير متساوية مع اجمالي الفاتورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If
            End If
        End If
        Return True
    End Function
    Sub CHKATTACHSTAUT()

    End Sub
    Sub ADDREGDOC()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRegDoc", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@RegDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtDocGuid.Text)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@SellerGuid", TxtSalesManGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@DocTotal", Val(TxtFTotal1.Text))
        comm.Parameters.AddWithValue("@DocTax", Val(TxtFTotal2.Text))
        comm.Parameters.AddWithValue("@DocSubTotal", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsPay", ChkPay.Checked)
        comm.Parameters.AddWithValue("@PayGuid", TxtPayGuid.Text)
        comm.Parameters.AddWithValue("@PayCode", TxtPayCode.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@RegType", TxtRegType.Text)
        comm.Parameters.AddWithValue("@CourseGuid", TxtCourseGuid.Text)
        comm.Parameters.AddWithValue("@ChkContract", ChkContract.Checked)
        If ChkContract.Checked = True Then
            comm.Parameters.AddWithValue("@ContractDate", Convert.ToDateTime(TxtDateReg.Value))
        Else
            comm.Parameters.AddWithValue("@RemainingDays", 30)
        End If
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
        TxtCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
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
                comm.Parameters.AddWithValue("@DiplomType", 0)
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
    Sub ADDREGDOCFEES()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRegDocDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To FeesGrid.Rows.Count - 1
            If FeesGrid.Rows(i).Cells(5).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@DiplomGuid", FeesGrid.Rows(i).Cells(5).Value.ToString())
                comm.Parameters.AddWithValue("@Cost", Val(FeesGrid.Rows(i).Cells(2).Value))
                comm.Parameters.AddWithValue("@Tax", Val(FeesGrid.Rows(i).Cells(3).Value))
                comm.Parameters.AddWithValue("@SubTotal", Val(FeesGrid.Rows(i).Cells(4).Value))
                comm.Parameters.AddWithValue("@DiplomType", 1)
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
    Sub ADDDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", OPENGUID)
        'comm.Parameters.AddWithValue("@Id", GETDAY1__NO)
        'comm.Parameters.AddWithValue("@Code", TxtDayCode.Text)
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
        TxtDayCode.Text = RETURNCODE.ToString()
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
    Sub SHOWPAYDAY1INFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDay1InfoForDoc", connection)
        comm.Parameters.AddWithValue("@DocGuid", TxtPayGuid.Text)
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
                TxtDay1PayCode.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDay1PayGuid.Text = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWREGDOCINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowRegDocInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegDocInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDateReg.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0))
                TxtDocName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtDocGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtDocCode.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtBatchName.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtBatchGuid.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtSalesManGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtSalesManName.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtStudentName.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(11).ToString()
                TxtNationalId.Text = ds.Tables(0).Rows(i)(12).ToString()
                TxtAccountGuid.Text = ds.Tables(0).Rows(i)(13).ToString()
                TxtFTotal1.Text = ds.Tables(0).Rows(i)(14).ToString()
                TxtFTotal2.Text = ds.Tables(0).Rows(i)(15).ToString()
                TxtFTotal3.Text = ds.Tables(0).Rows(i)(16).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(17).ToString()
                ChkPay.Checked = ds.Tables(0).Rows(i)(18)
                TxtPayGuid.Text = ds.Tables(0).Rows(i)(19).ToString()
                TxtGuid.Text = ds.Tables(0).Rows(i)(20).ToString()
                TxtStaut.Text = ds.Tables(0).Rows(i)(21).ToString()
                TxtPayCode.Text = ds.Tables(0).Rows(i)(22).ToString()
                TxtLevelName.Text = ds.Tables(0).Rows(i)(23).ToString()
                TxtLevelGuid.Text = ds.Tables(0).Rows(i)(24).ToString()
                TxtCashBoxName.Text = ds.Tables(0).Rows(i)(25).ToString()
                TxtCashBoxGuid.Text = ds.Tables(0).Rows(i)(26).ToString()
                TxtDayCode.Text = ds.Tables(0).Rows(i)(27).ToString()
                TxtDayGuid.Text = ds.Tables(0).Rows(i)(28).ToString()
                TxtDay1PayCode.Text = ds.Tables(0).Rows(i)(29).ToString()
                TxtDay1PayGuid.Text = ds.Tables(0).Rows(i)(30).ToString()
                TxtDay1RecepitCode.Text = ds.Tables(0).Rows(i)(31).ToString()
                TxtDay1RecpitGuid.Text = ds.Tables(0).Rows(i)(32).ToString()
                TxtCostCenterName.Text = ds.Tables(0).Rows(i)(33).ToString()
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(34).ToString()
                TxtRef.Text = ds.Tables(0).Rows(i)(35).ToString()
                TxtPayNotes.Text = ds.Tables(0).Rows(i)(36).ToString()
                TxtUserGuid.Text = ds.Tables(0).Rows(i)(37).ToString()
                TxtUserName.Text = ds.Tables(0).Rows(i)(38).ToString()
                TxtRecepitCode.Text = ds.Tables(0).Rows(i)(39).ToString()
                TxtRecepitGuid.Text = ds.Tables(0).Rows(i)(40).ToString()
                TxtPayBranchName.Text = ds.Tables(0).Rows(i)(41).ToString()
                TxtPayBranchGuid.Text = ds.Tables(0).Rows(i)(42).ToString()
                TxtPayDocName.Text = ds.Tables(0).Rows(i)(43).ToString()
                TxtPayDocGuid.Text = ds.Tables(0).Rows(i)(44).ToString()
                TxtPayDocCode.Text = ds.Tables(0).Rows(i)(45).ToString()
                TxtRegType.Text = ds.Tables(0).Rows(i)(46).ToString()
                TxtCourseName.Text = ds.Tables(0).Rows(i)(47).ToString()
                TxtCourseGuid.Text = ds.Tables(0).Rows(i)(48).ToString()
                TxtCourseCode.Text = ds.Tables(0).Rows(i)(49).ToString()
                TxtstudentNational.Text = ds.Tables(0).Rows(i)(50).ToString()
                If (IsDBNull(ds.Tables(0).Rows(i)(51))) Then
                    ChkPayDate.Checked = False
                Else
                    ChkPayDate.Checked = ds.Tables(0).Rows(i)(51)
                End If
                If (IsDBNull(ds.Tables(0).Rows(i)(52))) Then
                    TxtPayDate.Value = Convert.ToDateTime(DateTime.Now).ToShortDateString()
                Else
                    TxtPayDate.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(52)).ToShortDateString()
                End If
                TxtEmail.Text = ds.Tables(0).Rows(i)(63).ToString()
                SHOWRECEIPT1INFO(TxtPayCode.Text, TxtCashBoxGuid.Text)
                ChlOtherBank.Checked = ds.Tables(0).Rows(i)(54)
                If ds.Tables(0).Rows(i)(54) = True Then
                    'TxtAmount1.Text = ds.Tables(0).Rows(i)(55).ToString()
                    TxtCashBoxName2.Text = ds.Tables(0).Rows(i)(56).ToString()
                    TxtCashBoxGuid2.Text = ds.Tables(0).Rows(i)(57).ToString()
                    TxtCostCenterName2.Text = ds.Tables(0).Rows(i)(58).ToString()
                    TxtCostCenterGuid2.Text = ds.Tables(0).Rows(i)(59).ToString()
                    ChkPayDate2.Checked = ds.Tables(0).Rows(i)(60)
                    TxtPayDate2.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(61)).ToShortDateString()
                    SHOWRECEIPT2INFO(TxtPayCode.Text, TxtCashBoxGuid2.Text, TxtRecepitCode.Text)
                End If
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWRECEIPT1INFO(BILLCODE As String, CASHGUID As String)
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowReceipt2Info", connection)
        comm.Parameters.AddWithValue("@BillCode", BILLCODE)
        comm.Parameters.AddWithValue("@CashBoxGuid", CASHGUID)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowReceipt2Info")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtRecepitCode.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtRecepitGuid.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtDay1RecepitCode.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtDay1RecpitGuid.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtAmount1.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtRef.Text = ds.Tables(0).Rows(i)(5).ToString()
            Next
        End If
    End Sub
    Sub SHOWRECEIPT2INFO(BILLCODE As String, CASHGUID As String, RECEIPT1 As String)
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowReceipt2Info", connection)
        comm.Parameters.AddWithValue("@BillCode", BILLCODE)
        comm.Parameters.AddWithValue("@CashBoxGuid", CASHGUID)
        comm.Parameters.AddWithValue("@ReceiptCode", RECEIPT1)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowReceipt2Info")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtRecepitCode2.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtRecepitGuid2.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtDay1RecepitCode2.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtDay1RecpitGuid2.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtAmount2.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtRef2.Text = ds.Tables(0).Rows(i)(5).ToString()
            Next
        End If
    End Sub
    Private Sub RegisterForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            client = New HttpClient()
            RegDocPosted.WHATFORMTYPEGUID("addregdoc")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("salesinvoice")
            TxtPayTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("receipt")
            TxtRecepitTypeGuid.Text = RegDocPosted.TYPEGUID
            RegDocPosted.WHATFORMTYPEGUID("opendaily")
            TxtOpenDailyType.Text = RegDocPosted.TYPEGUID
            OPENGUID = Guid.Parse(TxtOpenDailyType.Text)
            AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            If AllFiles.ZEROAMOUNT = True Then
                TxtZeroAmount.Text = 1
                DiplomGrid.Columns(4).ReadOnly = False
                FeesGrid.Columns(4).ReadOnly = False
            Else
                TxtZeroAmount.Text = 0
                DiplomGrid.Columns(4).ReadOnly = True
                FeesGrid.Columns(4).ReadOnly = True
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub UPDATEREGDOC()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateRegDoc", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@RegDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtDocGuid.Text)
        comm.Parameters.AddWithValue("@BatchGuid", TxtBatchGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@DocTotal", Val(TxtFTotal1.Text))
        comm.Parameters.AddWithValue("@DocTax", Val(TxtFTotal2.Text))
        comm.Parameters.AddWithValue("@DocSubTotal", Val(TxtFTotal3.Text))
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@RegType", TxtRegType.Text)
        comm.Parameters.AddWithValue("@CourseGuid", TxtCourseGuid.Text)
        'comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@IsPay", ChkPay.Checked)
        comm.Parameters.AddWithValue("@PayGuid", TxtPayGuid.Text)
        comm.Parameters.AddWithValue("@PayCode", TxtPayCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEPAYSTAUT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdatePayStaut", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@IsPay", ChkPay.Checked)
        comm.Parameters.AddWithValue("@PayGuid", TxtPayGuid.Text)
        comm.Parameters.AddWithValue("@PayCode", TxtPayCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETEREGDOCDIPLOM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteRegDocDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@DiplomType", 0)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETEREGDOCOTHERFESS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteRegDocDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@DiplomType", 1)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEDAY1(DAY1GUID As String)
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", DAY1GUID)
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
    Sub DELETEDAY2(DAY1GUID As String)
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", DAY1GUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADD_USERACTION_(TYPEGUID As String, DOCCODE As String, GUID As String)
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("Add_UserAction", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ActionGuid", GUID)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
        comm.Parameters.AddWithValue("@Date_", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@Time_", Convert.ToDateTime(DateTime.Now.ToShortTimeString))
        comm.Parameters.AddWithValue("@Notes_", AllFiles.ActionType___)
        comm.Parameters.AddWithValue("@ActionTypeGuid", TYPEGUID)
        comm.Parameters.AddWithValue("@ActionReason", REASON)
        comm.Parameters.AddWithValue("@ActionCode", DOCCODE)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDBILL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddBill", connection)
        comm.CommandType = CommandType.StoredProcedure
        'comm.Parameters.AddWithValue("@Id", TxtPayCode.Text)
        'comm.Parameters.AddWithValue("@Code", TxtPayCode.Text)
        comm.Parameters.AddWithValue("@BillDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtPayDocGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", TxtUserGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@BillTotal", Val(TxtPayTotal.Text))
        comm.Parameters.AddWithValue("@BillTax", Val(TxtPayTax.Text))
        comm.Parameters.AddWithValue("@BillSubTotal", Val(TxtPaySubTotal.Text))
        comm.Parameters.AddWithValue("@Notes", TxtPayNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Refrance", TxtRef.Text)
        comm.Parameters.AddWithValue("@CashBoxGuid", TxtCashBoxGuid.Text)
        comm.Parameters.AddWithValue("@ChkPayDate", ChkPayDate.Checked)
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtPayDate.Value))
        comm.Parameters.AddWithValue("@ChkOtherBank", ChlOtherBank.Checked)
        If ChlOtherBank.Checked = True Then
            comm.Parameters.AddWithValue("@CashBoxguid2", TxtCashBoxGuid2.Text)
            comm.Parameters.AddWithValue("@CostCentGuid2", TxtCostCenterGuid2.Text)
        Else
            comm.Parameters.AddWithValue("@CashBoxguid2", NOGUID)
            comm.Parameters.AddWithValue("@CostCentGuid2", NOGUID)
        End If
        comm.Parameters.AddWithValue("@ChkPayDate2", ChkPayDate2.Checked)
        comm.Parameters.AddWithValue("@PayDate2", Convert.ToDateTime(TxtPayDate2.Value))
        comm.Parameters.AddWithValue("@Ref2", TxtRef2.Text)
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
        TxtPayCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEBILL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateBill", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtPayGuid.Text)
        comm.Parameters.AddWithValue("@BillDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@DocGuid", TxtPayDocGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        comm.Parameters.AddWithValue("@CostCenterGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@BillTotal", Val(TxtPayTotal.Text))
        comm.Parameters.AddWithValue("@BillTax", Val(TxtPayTax.Text))
        comm.Parameters.AddWithValue("@BillSubTotal", Val(TxtPaySubTotal.Text))
        comm.Parameters.AddWithValue("@Notes", TxtPayNotes.Text)
        comm.Parameters.AddWithValue("@Refrance", TxtRef.Text)
        comm.Parameters.AddWithValue("@CashBoxGuid", TxtCashBoxGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@ChkPayDate", ChkPayDate.Checked)
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtPayDate.Value))
        comm.Parameters.AddWithValue("@ChkOtherBank", ChlOtherBank.Checked)
        If ChlOtherBank.Checked = True Then
            comm.Parameters.AddWithValue("@CashBoxguid2", TxtCashBoxGuid2.Text)
            comm.Parameters.AddWithValue("@CostCentGuid2", TxtCostCenterGuid2.Text)
        Else
            comm.Parameters.AddWithValue("@CashBoxguid2", NOGUID)
            comm.Parameters.AddWithValue("@CostCentGuid2", NOGUID)
        End If
        comm.Parameters.AddWithValue("@ChkPayDate2", ChkPayDate2.Checked)
        comm.Parameters.AddWithValue("@PayDate2", Convert.ToDateTime(TxtPayDate2.Value))
        comm.Parameters.AddWithValue("@Ref2", TxtRef2.Text)
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
        comm.Parameters.AddWithValue("@RegDocGuid", TxtPayGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWBILLINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowBillInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtPayCode.Text)
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
                TxtPayDocName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtPayDocGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtPayDocCode.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtPayBranchGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtPayBranchName.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtCostCenterGuid.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtCostCenterName.Text = ds.Tables(0).Rows(i)(7).ToString()
                TxtUserGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
                TxtUserName.Text = ds.Tables(0).Rows(i)(9).ToString()
                TxtPayNotes.Text = ds.Tables(0).Rows(i)(17).ToString()
                TxtPayGuid.Text = ds.Tables(0).Rows(i)(19).ToString()
                TxtRef.Text = ds.Tables(0).Rows(i)(20).ToString()
                TxtCashBoxGuid.Text = ds.Tables(0).Rows(i)(21).ToString()
                TxtCashBoxName.Text = ds.Tables(0).Rows(i)(22).ToString()
                'ChlOtherBank.Checked = ds.Tables(0).Rows(i)(34)
                'If ds.Tables(0).Rows(i)(34) = True Then
                '    TxtAmount1.Text = ds.Tables(0).Rows(i)(42).ToString()
                'End If
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDBILLITEM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRegDocDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To PayGrid.Rows.Count - 1
            If PayGrid.Rows(i).Cells(5).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@RegDocGuid", TxtPayGuid.Text)
                comm.Parameters.AddWithValue("@DiplomGuid", PayGrid.Rows(i).Cells(8).Value.ToString())
                comm.Parameters.AddWithValue("@Cost", Val(PayGrid.Rows(i).Cells(4).Value))
                comm.Parameters.AddWithValue("@Tax", Val(PayGrid.Rows(i).Cells(6).Value))
                comm.Parameters.AddWithValue("@SubTotal", Val(PayGrid.Rows(i).Cells(7).Value))
                comm.Parameters.AddWithValue("@DiplomType", PayGrid.Rows(i).Cells(9).Value.ToString())
                comm.Parameters.AddWithValue("@Type_", 1)
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
    Sub ADDPAYDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", OPENGUID)
        'comm.Parameters.AddWithValue("@Id", TxtDay1PayCode.Text)
        'comm.Parameters.AddWithValue("@Code", TxtDay1PayCode.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@DayType", True)
        comm.Parameters.AddWithValue("@DocGuid", TxtPayGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@TypeGuid", TxtPayTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionCode", TxtPayCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
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
        TxtDay1PayCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub ADDPAYDAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1PayGuid.Text)
        If RegDocPosted.CHKMADEN = True Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        Else
            comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.MADENGUID)
        End If
        If ChkVat.Checked = True Then
            comm.Parameters.AddWithValue("@Maden", Val(TxtPaySubTotal.Text))
        Else
            comm.Parameters.AddWithValue("@Maden", Val(TxtPayTotal.Text))
        End If
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
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
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1PayGuid.Text)
        If RegDocPosted.CHKDAEN = True Then
            comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        Else
            comm.Parameters.AddWithValue("@AccountGuid", RegDocPosted.DAENGUID)
        End If
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtPayTotal.Text))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDPAYTAXDAY2_DAEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1PayGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtVatGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtPayTax.Text))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDSTUDENTLEVEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddStudentLevel", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StudentGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@LevelGuid", TxtLevelGuid.Text)
        comm.Parameters.AddWithValue("@LevelMark", 0)
        comm.Parameters.AddWithValue("@LevelStaut", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDRECEPIT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCashDay", connection)
        comm.CommandType = CommandType.StoredProcedure
        'comm.Parameters.AddWithValue("@Id", TxtRecepitCode.Text)
        'comm.Parameters.AddWithValue("@Code", TxtRecepitCode.Text)
        comm.Parameters.AddWithValue("@CashDayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@FAccountGuid", TxtCashBoxGuid.Text)
        If ChlOtherBank.Checked = True Then
            comm.Parameters.AddWithValue("@Amount", Val(TxtAmount1.Text))
        Else
            comm.Parameters.AddWithValue("@Amount", Val(TxtPaySubTotal.Text))
        End If
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@LinkGuid", TxtPayTypeGuid.Text)
        comm.Parameters.AddWithValue("@LinkRef", TxtPayCode.Text)
        comm.Parameters.AddWithValue("@CashDayType", 0)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Ref", TxtRef.Text)
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtPayDate.Value))
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
        TxtRecepitCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub ADDRECEPIT2()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCashDay", connection)
        comm.CommandType = CommandType.StoredProcedure
        'comm.Parameters.AddWithValue("@Id", TxtRecepitCode2.Text)
        'comm.Parameters.AddWithValue("@Code", TxtRecepitCode2.Text)
        comm.Parameters.AddWithValue("@CashDayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@FAccountGuid", TxtCashBoxGuid2.Text)
        comm.Parameters.AddWithValue("@Amount", Val(TxtAmount2.Text))
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@LinkGuid", TxtPayTypeGuid.Text)
        comm.Parameters.AddWithValue("@LinkRef", TxtPayCode.Text)
        comm.Parameters.AddWithValue("@CashDayType", 0)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Ref", TxtRef2.Text)
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtPayDate2.Value))
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
        TxtRecepitCode2.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATERECEPIT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateCashDay", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtRecepitGuid.Text)
        comm.Parameters.AddWithValue("@CashDayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@FAccountGuid", TxtCashBoxGuid.Text)
        If ChlOtherBank.Checked = False Then
            comm.Parameters.AddWithValue("@Amount", Val(TxtPaySubTotal.Text))
        Else
            comm.Parameters.AddWithValue("@Amount", Val(TxtAmount1.Text))
        End If
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Ref", TxtRef.Text)
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtPayDate.Value))
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATERECEPIT2()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateCashDay", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtRecepitGuid2.Text)
        comm.Parameters.AddWithValue("@CashDayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@FAccountGuid", TxtCashBoxGuid2.Text)
        comm.Parameters.AddWithValue("@Amount", Val(TxtAmount2.Text))
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@Ref", TxtRef2.Text)
        comm.Parameters.AddWithValue("@PayDate", Convert.ToDateTime(TxtPayDate2.Value))
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETECASHDAYDETAIL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteCashDayDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CashDayGuid", TxtRecepitGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub DELETECASHDAYDETAIL2()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteCashDayDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CashDayGuid", TxtRecepitGuid2.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWRECEPITINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowCashdayInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtRecepitCode.Text)
        comm.Parameters.AddWithValue("@CashType", 0)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowCashdayInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtRecepitGuid.Text = ds.Tables(0).Rows(i)(10).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWRECEPITINFO2()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowCashdayInfo", connection)
        comm.Parameters.AddWithValue("@Code", TxtRecepitCode2.Text)
        comm.Parameters.AddWithValue("@CashType", 0)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowCashdayInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtRecepitGuid2.Text = ds.Tables(0).Rows(i)(10).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDRECEPITDETAILS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCashDayDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CashDayGuid", TxtRecepitGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        If ChlOtherBank.Checked = False Then
            comm.Parameters.AddWithValue("@Daen", Val(TxtPaySubTotal.Text))
        Else
            comm.Parameters.AddWithValue("@Daen", Val(TxtAmount1.Text))
        End If
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDRECEPITDETAILS2()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddCashDayDetail", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CashDayGuid", TxtRecepitGuid2.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Daen", Val(TxtAmount2.Text))
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid2.Text)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDRECEPITDAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", OPENGUID)
        'comm.Parameters.AddWithValue("@Id", TxtDay1RecepitCode.Text)
        'comm.Parameters.AddWithValue("@Code", TxtDay1RecepitCode.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@DayType", True)
        comm.Parameters.AddWithValue("@DocGuid", TxtRecepitGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@TypeGuid", TxtRecepitTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionCode", TxtRecepitCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
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
        TxtDay1RecepitCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub ADDRECEPIT2DAY1()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay1", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@OpenGuid", OPENGUID)
        'comm.Parameters.AddWithValue("@Id", TxtDay1RecepitCode2.Text)
        'comm.Parameters.AddWithValue("@Code", TxtDay1RecepitCode2.Text)
        comm.Parameters.AddWithValue("@DayDate", Convert.ToDateTime(TxtDateReg.Value))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@DayType", True)
        comm.Parameters.AddWithValue("@DocGuid", TxtRecepitGuid2.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.Parameters.AddWithValue("@TypeGuid", TxtRecepitTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionCode", TxtRecepitCode2.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtPayBranchGuid.Text)
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
        TxtDay1RecepitCode2.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWRECEPITDAY1INFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDay1InfoForDoc", connection)
        comm.Parameters.AddWithValue("@DocGuid", TxtRecepitGuid.Text)
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
                TxtDay1RecepitCode.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDay1RecpitGuid.Text = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWRECEPIT2DAY1INFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDay1InfoForDoc", connection)
        comm.Parameters.AddWithValue("@DocGuid", TxtRecepitGuid2.Text)
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
                TxtDay1RecepitCode2.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDay1RecpitGuid2.Text = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDRECEPITDAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1RecpitGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtCashBoxGuid.Text)
        If ChlOtherBank.Checked = False Then
            comm.Parameters.AddWithValue("@Maden", Val(TxtPaySubTotal.Text))
        Else
            comm.Parameters.AddWithValue("@Maden", Val(TxtAmount1.Text))
        End If
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDRECEPIT2DAY2_MADEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1RecpitGuid2.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtCashBoxGuid2.Text)
        comm.Parameters.AddWithValue("@Maden", Val(TxtAmount2.Text))
        comm.Parameters.AddWithValue("@Daen", 0)
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid2.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDRECEPITDAY2_DAEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1RecpitGuid.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        If ChlOtherBank.Checked = False Then
            comm.Parameters.AddWithValue("@Daen", Val(TxtPaySubTotal.Text))
        Else
            comm.Parameters.AddWithValue("@Daen", Val(TxtAmount1.Text))
        End If
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDRECEPIT2DAY2_DAEN()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDay2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Day1Guid", TxtDay1RecpitGuid2.Text)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@Maden", 0)
        comm.Parameters.AddWithValue("@Daen", Val(TxtAmount2.Text))
        comm.Parameters.AddWithValue("@Notes", NOTES)
        comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid2.Text)
        comm.Parameters.AddWithValue("@IsUse", True)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWBILLDIPLOM()
        PayGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowBillDetail", connection)
        comm.Parameters.AddWithValue("@BillGuid", TxtPayGuid.Text)
        comm.Parameters.AddWithValue("@Type_", 1)
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
            PayGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                PayGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                PayGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                PayGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                PayGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                PayGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                PayGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                PayGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                PayGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                PayGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                PayGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWREGDOCGUID()
        Dim REGDOC_GUID_ = dbcontext.RegDocs.FirstOrDefault(Function(m) m.Code = TxtCode.Text)
        TxtGuid.Text = REGDOC_GUID_.Guid.ToString()
    End Sub
    Sub SHOWBILLGUID()
        Dim REGDOC_GUID_ = dbcontext.Bills.FirstOrDefault(Function(m) m.Code = TxtPayCode.Text)
        TxtPayGuid.Text = REGDOC_GUID_.Guid.ToString()
    End Sub
    Sub SHOWDEFLEVEL()
        Dim REGDOC_GUID_ = dbcontext.Levels.FirstOrDefault(Function(m) m.IsDef = True)
        TxtLevelName.Text = REGDOC_GUID_.LevelName.ToString()
        TxtLevelGuid.Text = REGDOC_GUID_.Guid.ToString()
    End Sub
    Public Function PAYNOEXISTS(PAYCODE As Integer) As Boolean
        Dim QUERY = dbcontext.Bills.FirstOrDefault(Function(m) m.ID = PAYCODE)
        If (IsNothing(QUERY)) Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Function RECEIPTNOEXISTS(RECEIPTCODE As Integer) As Boolean
        Dim QUERY = dbcontext.CashDays.FirstOrDefault(Function(m) m.Id = RECEIPTCODE And m.CashDayType = 0)
        If (IsNothing(QUERY)) Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Function DAY1PAYNOEXISTS(DAY1CODE As Integer) As Boolean
        Dim QUERY = dbcontext.Day1.FirstOrDefault(Function(m) m.ID = DAY1CODE And m.TypeGuid <> OPENGUID)
        If (IsNothing(QUERY)) Then
            Return False
        Else
            Return True
        End If
    End Function
    'Private Async Function SendRequestAsync(userName As String, password As String) As Task(Of String)
    '    Dim request As New HttpRequestMessage(HttpMethod.Post, "http://localhost:2020/Auth/login")
    '    request.Headers.Add("accept", "*/*")

    '    ' إنشاء محتوى JSON بناءً على المدخلات
    '    Dim jsonContent As String = $"{{""userName"": ""{userName}"", ""password"": ""{password}""}}"
    '    request.Content = New StringContent(jsonContent, Encoding.UTF8, "application/json")

    '    Try
    '        Dim response As HttpResponseMessage = Await client.SendAsync(request)
    '        response.EnsureSuccessStatusCode()
    '        Dim responseBody As String = Await response.Content.ReadAsStringAsync()

    '        ' استخراج التوكن فقط
    '        Dim json As JObject = JObject.Parse(responseBody)
    '        Dim token As String = json("token")?.ToString()

    '        Return If(String.IsNullOrEmpty(token), "Token not found!", token)
    '    Catch ex As Exception
    '        Return "Error: " & ex.Message
    '    End Try
    'End Function
    Private Async Function GetInvoiceDataAsync(invoiceID As String) As Task(Of String)
        Dim invoiceData As New Dictionary(Of String, Object)
        Dim dbcontext As New SemouEntities
        Using connection = dbcontext.Database.Connection
            If connection.State = ConnectionState.Closed Then
                connection.Open()
            End If
            Try
                Using comm = New SqlCommand("ShowBillInfo", CType(connection, SqlConnection))
                    comm.CommandType = CommandType.StoredProcedure
                    comm.Parameters.AddWithValue("@Code", TxtPayCode.Text)

                    Using reader As SqlDataReader = Await comm.ExecuteReaderAsync()
                        If reader.HasRows Then
                            While reader.Read()
                                invoiceData("id") = AllFiles.GetTaxInvoiceNo(reader("Guid").ToString()) ' رقم الفاتورة الضريبية الجديد
                                invoiceData("issueDate") = Convert.ToDateTime(reader("BillDate")).ToString("yyyy-MM-dd")
                                invoiceData("issueTime") = Convert.ToDateTime(reader("BillDate")).ToString("HH:mm:ss")
                                invoiceData("customer") = New Dictionary(Of String, Object) From {
                                              {"registrationName", reader("StudentName").ToString()},
                                              {"taxId", Nothing},
                                              {"address", Nothing}
                                                                   }
                                invoiceData("invoiceTotals") = New Dictionary(Of String, Object) From {
                                             {"lineExtensionAmount", Convert.ToDecimal(reader("BillTotal")).ToString("F2", CultureInfo.InvariantCulture)},
                                             {"taxExclusiveAmount", Convert.ToDecimal(reader("BillTotal")).ToString("F2", CultureInfo.InvariantCulture)},
                                             {"taxInclusiveAmount", Convert.ToDecimal(reader("BillSubTotal")).ToString("F2", CultureInfo.InvariantCulture)},
                                             {"prepaidAmount", 0.00},
                                             {"payableAmount", Convert.ToDecimal(reader("BillSubTotal")).ToString("F2", CultureInfo.InvariantCulture)}
                                                                                                          }
                            End While
                        Else
                            MessageBox.Show("لم يتم العثور على بيانات الفاتورة!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return Nothing
                        End If
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.ToString)
            End Try

            Dim invoiceLines As New List(Of Dictionary(Of String, Object))

            Using comm = New SqlCommand("ShowBillDetail", CType(connection, SqlConnection))
                comm.CommandType = CommandType.StoredProcedure
                comm.Parameters.AddWithValue("@BillGuid", TxtPayGuid.Text)
                comm.Parameters.AddWithValue("@Type_", 1)
                Using reader As SqlDataReader = Await comm.ExecuteReaderAsync()
                    While reader.Read()
                        invoiceLines.Add(New Dictionary(Of String, Object) From {
                            {"id", reader("Code").ToString()},
                            {"quantity", Convert.ToDecimal(reader("Qty")).ToString("F2", CultureInfo.InvariantCulture)},
                            {"itemName", reader("DiplomName").ToString()},
                            {"price", Convert.ToDecimal(reader("Cost")).ToString("F2", CultureInfo.InvariantCulture)},
                            {"taxRate", Convert.ToDecimal(reader("TaxValue")).ToString},
                            {"taxCategoryCode", reader("TAXCATOGREY").ToString()},
                            {"lineExtensionAmount", Convert.ToDecimal(reader("SubTotal")).ToString("F2", CultureInfo.InvariantCulture)},
                            {"taxAmount", Convert.ToDecimal(reader("Tax")).ToString("F2", CultureInfo.InvariantCulture)},
                            {"roundingAmount", Convert.ToDecimal(reader("SubTotal")).ToString("F2", CultureInfo.InvariantCulture)}
                        })
                    End While
                End Using
            End Using

            invoiceData("invoiceLines") = invoiceLines
        End Using

        'Return JsonConvert.SerializeObject(invoiceData, Formatting.Indented)
        jsonResult = JsonConvert.SerializeObject(invoiceData, Formatting.Indented)
    End Function
    'Private Async Function SendInvoiceAsync(jsonContent As String) As Task(Of String)
    '    'MessageBox.Show(jsonContent, "JSON Content")
    '    Dim client As New HttpClient()
    '    Dim request As New HttpRequestMessage(HttpMethod.Post, "http://localhost:2020/SendInvoice")

    '    request.Headers.Add("accept", "*/*")
    '    request.Headers.Authorization = New AuthenticationHeaderValue("Bearer", OtpSender.Token__)

    '    request.Content = New StringContent(jsonContent)
    '    request.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")

    '    Dim response As HttpResponseMessage = Await client.SendAsync(request)
    '    response.EnsureSuccessStatusCode()
    '    Return Await response.Content.ReadAsStringAsync()
    'End Function
    'Private Async Function SENDTOZATCK() As Task
    '    AllFiles.SHOWSYSTEMINFO()
    '    Dim userName As String = AllFiles.ZUSERNAME
    '    Dim password As String = AllFiles.ZPASSWORD

    '    If String.IsNullOrWhiteSpace(userName) OrElse String.IsNullOrWhiteSpace(password) Then
    '        MessageBox.Show("الرجاء إدخال اسم المستخدم وكلمة المرور.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    '        Exit Function
    '    End If

    '    Dim result As String = Await SendRequestAsync(userName, password)

    '    ' حفظ التوكن
    '    OtpSender.Token__ = result

    '    ''''''''''''''''''''''''''''''''' OTP
    '    Dim otp As String = AllFiles.ZOTP
    '    Dim sender2 As New OtpSender()
    '    Dim result2 As String = Await sender2.SendOtpAsync(otp)
    '    'MessageBox.Show(result2, "Response", MessageBoxButtons.OK, MessageBoxIcon.Information)

    '    '''''''''''''''''''''''''''''''''' Send Invoice
    '    Dim invoiceID As String = TxtCode.Text
    '    If String.IsNullOrEmpty(invoiceID) Then
    '        MessageBox.Show("يرجى إدخال كود الفاتورة!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        Return
    '    End If

    '    Dim jsonContent As String = Await GetInvoiceDataAsync(invoiceID)
    '    If String.IsNullOrEmpty(jsonContent) Then
    '        MessageBox.Show("لم يتم العثور على بيانات الفاتورة!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        Return
    '    End If
    '    Dim response As String = Await SendInvoiceAsync(jsonContent)
    'End Function
    Sub ADDBILLBYJSON()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddBillByJson", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BillCode", TxtPayCode.Text)
        comm.Parameters.AddWithValue("@BillGuid", TxtPayGuid.Text)
        comm.Parameters.AddWithValue("@JsonCintent", jsonResult.ToString())
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Async Sub FORSAVE()
        If AllFiles.FORSAVE("addregdoc") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDREGDOC()
            SHOWREGDOCGUID()
            ADDREGDOCDIPLOM()
            If ChkOtherFees.Checked = True Then
                ADDREGDOCFEES()
            End If
            NOTES = "تسجيل الطالب" & " " & TxtStudentName.Text & vbCrLf & " " & TxtDocName.Text _
                    & vbCrLf & " " & TxtBatchName.Text & vbCrLf & " " & TxtDiplomName.Text
            ADDDAY1()
            SHOWDAY1INFO()
            ADDDAY2_MADEN()
            ADDDAY2_DAEN()
            If ChkIfNeedPay.Checked = True Then
                ''''''''''''''''''''''''''''''''''''''' فاتورة المبيعات
                ADDBILL()
                SHOWBILLGUID()
                ADDBILLITEM()
                NOTES = "فاتورة الدفعة المقدمة لحساب الطالب" & " " & TxtStudentName.Text & vbCrLf & " " & TxtDocName.Text & vbCrLf & " " & TxtDiplomName.Text
                ADDPAYDAY1()
                SHOWPAYDAY1INFO()
                RegDocPosted.SHOWREFSETTINGINFO(TxtPayDocCode.Text, Guid.Parse(TxtPayTypeGuid.Text))
                ADDPAYDAY2_MADEN()
                ADDPAYDAY2_DAEN()
                If ChkVat.Checked = True And TxtFTotal2.Text <> 0 Then
                    ADDPAYTAXDAY2_DAEN()
                End If
                'End If
                ChkPay.Checked = True
                UPDATEPAYSTAUT()
                'End If
                ADD_USERACTION_(TxtPayTypeGuid.Text, TxtPayCode.Text, TxtPayGuid.Text)
                ''''''''''''''''''''''''''''''''''''''''''''' سند القبض
                NOTES = "سداد دفعة مقدمة لحساب الطالب" & " " & TxtStudentName.Text & vbCrLf & " " & TxtDocName.Text & vbCrLf & " " & TxtDiplomName.Text
                ADDRECEPIT()
                SHOWRECEPITINFO()
                ADDRECEPITDETAILS()
                ADDRECEPITDAY1()
                SHOWRECEPITDAY1INFO()
                ADDRECEPITDAY2_MADEN()
                ADDRECEPITDAY2_DAEN()
                ADD_USERACTION_(TxtRecepitTypeGuid.Text, TxtRecepitCode.Text, TxtRecepitGuid.Text)
                '''''''''''''''''''''''''''''''' سند القبض الخاص بالحوالة الثانية ان وجدت
                If ChlOtherBank.Checked = True Then
                    ADDRECEPIT2()
                    SHOWRECEPITINFO2()
                    ADDRECEPITDETAILS2()
                    ADDRECEPIT2DAY1()
                    SHOWRECEPIT2DAY1INFO()
                    ADDRECEPIT2DAY2_MADEN()
                    ADDRECEPIT2DAY2_DAEN()
                    ADD_USERACTION_(TxtRecepitTypeGuid.Text, TxtRecepitCode2.Text, TxtRecepitGuid2.Text)
                End If
                SHOWBILLINFO()
            End If
            SHOWREGDOCINFO()
            ADD_USERACTION_(TxtTypeGuid.Text, TxtCode.Text, TxtGuid.Text)
            ' استدعاء إرسال الفاتورة للزكاة والدخل
            Await GetInvoiceDataAsync(TxtPayCode.Text)
            ADDBILLBYJSON()
            MessageBox.Show("تم التسجيل بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addregdoc") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEREGDOC()
            DELETEREGDOCDIPLOM()
            ADDREGDOCDIPLOM()
            If ChkOtherFees.Checked = True Then
                DELETEREGDOCOTHERFESS()
                ADDREGDOCFEES()
            End If
            NOTES = "تسجيل الطالب" & " " & TxtStudentName.Text & vbCrLf & " " & TxtDocName.Text _
                    & vbCrLf & " " & TxtBatchName.Text & vbCrLf & " " & TxtDiplomName.Text
            UPDATEDAY1(TxtDayGuid.Text)
            DELETEDAY2(TxtDayGuid.Text)
            ADDDAY2_MADEN()
            ADDDAY2_DAEN()
            If ChkIfNeedPay.Checked = True Then
                ''''''''''''''''''''''''''''''''''''''' فاتورة المبيعات
                If TxtPayGuid.Text <> Nothing And TxtPayGuid.Text <> "00000000-0000-0000-0000-000000000000" Then
                    UPDATEBILL()
                    DELETEBILLITEM()
                Else
                    AllFiles.WHATONLINEUSERINFO()
                    TxtUserName.Text = AllFiles.ONLINE_USER_NAME____
                    TxtUserGuid.Text = AllFiles.USER_GUID____.ToString()
                    'Do
                    '    GET_PAY__NO()
                    'Loop While PAYNOEXISTS(TxtPayCode.Text)
                    'If PAYNOEXISTS(TxtPayCode.Text) = False Then
                    ADDBILL()
                    SHOWBILLGUID()
                    UPDATEREGDOC()
                    'End If
                End If
                ADDBILLITEM()
                NOTES = "فاتورة الدفعة المقدمة لحساب الطالب" & " " & TxtStudentName.Text & vbCrLf & " " & TxtDocName.Text & vbCrLf & " " & TxtDiplomName.Text
                If TxtDay1PayGuid.Text <> Nothing Then
                    UPDATEDAY1(TxtDay1PayGuid.Text)
                    DELETEDAY2(TxtDay1PayGuid.Text)
                Else
                    'Do
                    '    GETDAY1_PAY_NO()
                    'Loop While DAY1PAYNOEXISTS(TxtDay1PayCode.Text)
                    'If DAY1PAYNOEXISTS(TxtDay1PayCode.Text) = False Then
                    ADDPAYDAY1()
                        SHOWPAYDAY1INFO()
                    'End If
                End If
                RegDocPosted.SHOWREFSETTINGINFO(TxtPayDocCode.Text, Guid.Parse(TxtPayTypeGuid.Text))
                ADDPAYDAY2_MADEN()
                ADDPAYDAY2_DAEN()
                If ChkVat.Checked = True Then
                    ADDPAYTAXDAY2_DAEN()
                End If
                ADD_USERACTION_(TxtPayTypeGuid.Text, TxtPayCode.Text, TxtPayGuid.Text)
            End If
            ''''''''''''''''''''''''''''''''''''''''''''' سند القبض
            NOTES = "سداد دفعة مقدمة لحساب الطالب" & " " & TxtStudentName.Text & vbCrLf & " " & TxtDocName.Text & vbCrLf & " " & TxtDiplomName.Text
            If TxtRecepitGuid.Text <> Nothing Then
                UPDATERECEPIT()
                DELETECASHDAYDETAIL()
            Else
                'Do
                '    GET_RECEPIT__NO()
                'Loop While RECEIPTNOEXISTS(TxtRecepitCode.Text)
                'If RECEIPTNOEXISTS(TxtRecepitCode.Text) = False Then
                ADDRECEPIT()
                SHOWRECEPITINFO()
                'End If
            End If
            ADDRECEPITDETAILS()
            If TxtDay1RecpitGuid.Text <> Nothing Then
                UPDATEDAY1(TxtDay1RecpitGuid.Text)
                DELETEDAY2(TxtDay1RecpitGuid.Text)
            Else
                'Do
                '    GETDAY1_RECEPIT_NO()
                'Loop While DAY1PAYNOEXISTS(TxtDay1RecepitCode.Text)
                'If DAY1PAYNOEXISTS(TxtDay1RecepitCode.Text) = False Then
                ADDRECEPITDAY1()
                    SHOWRECEPITDAY1INFO()
                'End If
            End If
            ADDRECEPITDAY2_MADEN()
            ADDRECEPITDAY2_DAEN()
            ADD_USERACTION_(TxtRecepitTypeGuid.Text, TxtRecepitCode.Text, TxtRecepitGuid.Text)
            '''''''''''''''''''''''''''''''''''''''''' سند القبض الخاص بالحوالة الثانية ان وجدت
            If ChlOtherBank.Checked = True Then
                If TxtRecepitGuid2.Text <> Nothing Then
                    UPDATERECEPIT2()
                    DELETECASHDAYDETAIL2()
                Else
                    'Do
                    '    GET_RECEPIT2__NO()
                    'Loop While RECEIPTNOEXISTS(TxtRecepitCode2.Text)
                    'If RECEIPTNOEXISTS(TxtRecepitCode2.Text) = False Then
                    ADDRECEPIT2()
                    SHOWRECEPITINFO2()
                    'End If
                End If
                ADDRECEPITDETAILS2()
                If TxtDay1RecpitGuid2.Text <> Nothing Then
                    UPDATEDAY1(TxtDay1RecpitGuid2.Text)
                    DELETEDAY2(TxtDay1RecpitGuid2.Text)
                Else
                    'Do
                    '    GETDAY1_RECEPIT2_NO()
                    'Loop While DAY1PAYNOEXISTS(TxtDay1RecepitCode2.Text)
                    'If DAY1PAYNOEXISTS(TxtDay1RecepitCode2.Text) = False Then
                    ADDRECEPIT2DAY1()
                    SHOWRECEPIT2DAY1INFO()
                    'End If
                End If
                ADDRECEPIT2DAY2_MADEN()
                ADDRECEPIT2DAY2_DAEN()
                ADD_USERACTION_(TxtRecepitTypeGuid.Text, TxtRecepitCode2.Text, TxtRecepitGuid2.Text)
            End If
            ADD_USERACTION_(TxtTypeGuid.Text, TxtCode.Text, TxtGuid.Text)
            MessageBox.Show("تم تعديل التسجيل بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdNewNext.Click
        Try
            If TxtGuid.Text = Nothing Then
                If TabControl1.SelectedIndex = 0 Then
                    If VALIDATE_DATAFOR_STEP1() = False Then
                        Return
                    Else
                        IS_Ok = True
                        TabControl1.SelectedTab = TabPage2
                    End If
                ElseIf TabControl1.SelectedIndex = 1 Then
                    If ChkIfNeedPay.Checked = True Then
                        If VALIDATE_DATAFOR_STEP2() = False Then
                            Return
                        Else
                            IS_Ok = True
                            TabControl1.SelectedTab = TabPage3
                        End If
                    Else
                        CmdNewSave.Enabled = True
                    End If

                End If
            Else
            If TabControl1.SelectedIndex = 0 Then
                IS_Ok = True
                TabControl1.SelectedTab = TabPage2
            ElseIf TabControl1.SelectedIndex = 1 Then
                IS_Ok = True
                TabControl1.SelectedTab = TabPage3
            End If
            CmdNewSave.Enabled = True
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdPay_Click(sender As Object, e As EventArgs) Handles CmdPay.Click
        Try
            If TxtGuid.Text <> Nothing Then
                If ChkPay.Checked = False Then
                    Dim w As New PaySelect
                    w.CmdPay2.Enabled = False
                    w.CmdPay4.Enabled = False
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        TxtPayType.Text = w.TxtPayType.Text
                    Else
                        TxtPayType.Text = Nothing
                    End If
                    If TxtPayType.Text <> Nothing Then
                        Dim N As New EncodingPayInvoice
                        N.TxtStudentName.Text = TxtStudentName.Text
                        N.TxtTel.Text = TxtTel.Text
                        N.TxtNationalId.Text = TxtNationalId.Text
                        N.TxtAccountGuid.Text = TxtAccountGuid.Text
                        N.TxtStudentGuid.Text = TxtStudentGuid.Text
                        N.TxtRegDocGuid.Text = TxtGuid.Text
                        N.TxtPayType.Text = TxtPayType.Text
                        N.WHATDATAINGRID()
                        N.DIPLOMTOTAL()
                        N.ShowDialog()
                        If N.IS_Yes = True Then
                            TxtPayGuid.Text = N.TxtGuid.Text
                            TxtPayCode.Text = N.TxtCode.Text
                            ChkPay.Checked = True
                        End If
                    End If
                Else
                    Dim ASKFOREDIT As DialogResult = MessageBox.Show("تم سداد الدفعه المقدمه من قبل هل تريد عرض الفاتورة",
                             "سداد الدفعة المقدمة", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If ASKFOREDIT = DialogResult.Yes Then
                        Dim w As New EncodingPayInvoice
                        w.TxtCode.Text = TxtPayCode.Text
                        w.TxtGuid.Text = TxtPayGuid.Text
                        w.SHOWBILLINFO()
                        w.SHOWBILLDIPLOM()
                        w.DIPLOMTOTAL()
                        w.SHOWDAY1INFO()
                        w.ShowDialog()
                    End If
                End If
            Else
                MessageBox.Show("برجاء اختيار حفظ الاستمارة او اختيار استمارة تم حفظها مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindSalesMan_Click(sender As Object, e As EventArgs) Handles CmdFindSalesMan.Click
        Try
            If TxtDocGuid.Text <> Nothing Then
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
    Sub SHOWREGDOCDIPLOM()
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowRegDocDiplom", connection)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@DiplomType", 0)
        comm.Parameters.AddWithValue("@Type_", 0)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegDocDiplom")
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
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWREGDOCFEES()
        FeesGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowRegDocDiplom", connection)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@DiplomType", 1)
        comm.Parameters.AddWithValue("@Type_", 0)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRegDocDiplom")
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

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        Try
            Select Case TabControl1.SelectedIndex
                Case 1
                    If IS_Ok = False Then
                        MessageBox.Show("برجاء استكمال الخطوات عن طريق الضغط على التالي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        TabControl1.SelectedIndex = 0
                    End If
                Case 2
                    If IS_Ok = False Then
                        MessageBox.Show("برجاء استكمال الخطوات عن طريق الضغط على التالي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        TabControl1.SelectedIndex = 0
                    End If
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATAFOR_STEP1() As Boolean
        If TxtDocGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار نوع المستند", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtRegType.Text = 0 Then
            If TxtBatchGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيارالدفعة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf TxtBranchGuid.Text = Nothing Then
                MessageBox.Show("برجاء مراجعة إعدادات التسجيل لعدم وجود فرع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf TxtLevelGuid.Text = Nothing Then
                MessageBox.Show("برجاءاختيار المستوي المراد تسجيل الطالب عليه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
        ElseIf TxtRegType.Text = 1 Then
            If TxtCourseGuid.Text = Nothing Then
                MessageBox.Show(" برجاء اختيار الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            ElseIf TxtAvaCount.Text = 0 Then
                MessageBox.Show("لا توجد اى مقاعد متاحة للتسجيل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
        ElseIf TxtSalesManGuid.Text = Nothing Then
            MessageBox.Show("برجاءاختيار مندوب البيع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Function VALIDATE_DATAFOR_STEP2() As Boolean
        If DiplomGrid.Rows.Count = 0 Then
            MessageBox.Show("لا يمكن التسجيل بدون اختيار الدبلوم - الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf FeesGrid.Rows.Count = 0 And ChkOtherFees.Checked = True Then
            MessageBox.Show("لا يمكن التسجيل بدون اختيار رسوم الاختبار - الشهادة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Private Sub CmdNext_Click(sender As Object, e As EventArgs) Handles CmdNext.Click
        Try
            If TabControl1.SelectedIndex = 1 Then
                TabControl1.SelectedTab = TabPage1
            ElseIf TabControl1.SelectedIndex = 2 Then
                TabControl1.SelectedTab = TabPage2
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdNewExit_Click(sender As Object, e As EventArgs) Handles CmdNewExit.Click
        If TxtGuid.Text <> Nothing Then
            IS_Yes = True
        End If
        Me.Close()
    End Sub

    Private Sub PayGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles PayGrid.CellContentClick

    End Sub
    Sub PAYTOTAL()
        Dim DIPLOMTAX As Double = 0
        Dim DIPLOMTOTAL As Double = 0
        Dim DIPLOMSUBTOTAL As Double = 0
        For i As Integer = 0 To PayGrid.Rows.Count - 1
            If PayGrid.Rows(i).Cells(2).Value <> Nothing Then
                DIPLOMTOTAL = Val(DIPLOMTOTAL) + (Val(PayGrid.Rows(i).Cells(4).Value))
                DIPLOMTAX = Val(DIPLOMTAX) + (Val(PayGrid.Rows(i).Cells(6).Value))
                DIPLOMSUBTOTAL = Val(DIPLOMSUBTOTAL) + (Val(PayGrid.Rows(i).Cells(7).Value))
            End If
        Next
        TxtPayTotal.Text = Math.Round(Val(DIPLOMTOTAL), 2)
        TxtPayTax.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtPaySubTotal.Text = Math.Round(DIPLOMSUBTOTAL, 2)
    End Sub
    Private Sub PayGrid_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles PayGrid.CellEndEdit
        Try
            Dim i As Integer = PayGrid.CurrentCell.RowIndex
            Dim j As Integer = PayGrid.CurrentCell.ColumnIndex
            If j = 4 Then
                If Val(PayGrid.Rows(i).Cells(4).Value) >= Val(TxtPayAmount.Text) Then
                    If Val(PayGrid.Rows(i).Cells(5).Value) <> 0 Then
                        PayGrid.Rows(i).Cells(6).Value = Math.Round(Val(PayGrid.Rows(i).Cells(4).Value) * (Val(PayGrid.Rows(i).Cells(5).Value) / 100), 3)
                        PayGrid.Rows(i).Cells(7).Value = Val(PayGrid.Rows(i).Cells(4).Value) + Val(PayGrid.Rows(i).Cells(6).Value)
                    Else
                        PayGrid.Rows(i).Cells(7).Value = PayGrid.Rows(i).Cells(4).Value
                    End If
                Else
                    MessageBox.Show("لا يمكن ادخال قيمة اقل من القيمة المحددة للدفعة المقدمة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    PayGrid.Rows(i).Cells(4).Value = TxtPayAmount.Text
                    If Val(PayGrid.Rows(i).Cells(5).Value) <> 0 Then
                        PayGrid.Rows(i).Cells(6).Value = Math.Round(Val(PayGrid.Rows(i).Cells(4).Value) * (Val(PayGrid.Rows(i).Cells(5).Value) / 100), 3)
                        PayGrid.Rows(i).Cells(7).Value = Val(PayGrid.Rows(i).Cells(4).Value) + Val(PayGrid.Rows(i).Cells(6).Value)
                    Else
                        PayGrid.Rows(i).Cells(7).Value = PayGrid.Rows(i).Cells(4).Value
                    End If
                End If
            ElseIf j = 7 Then
                If Val(PayGrid.Rows(i).Cells(7).Value) >= Val(TxtPayAmount.Text) Then
                    If Val(PayGrid.Rows(i).Cells(7).Value) > Val(TxtDTotal3.Text) Then
                        MessageBox.Show("لا يمكن ادخال قيمة اكبر من قيمة الدبلوم / الدورة بدون الرسوم", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        PayGrid.Rows(i).Cells(7).Value = TxtDTotal3.Text
                        If Val(PayGrid.Rows(i).Cells(5).Value) <> 0 Then
                            Dim VAT As Double = (1 + Val(PayGrid.Rows(i).Cells(5).Value) / 100)
                            PayGrid.Rows(i).Cells(4).Value = Math.Round(Val(PayGrid.Rows(i).Cells(7).Value) / Val(VAT), 3)
                            PayGrid.Rows(i).Cells(6).Value = Math.Round(Val(PayGrid.Rows(i).Cells(4).Value) * (Val(PayGrid.Rows(i).Cells(5).Value) / 100), 3)
                        Else
                            PayGrid.Rows(i).Cells(4).Value = PayGrid.Rows(i).Cells(7).Value
                        End If
                    Else
                        If Val(PayGrid.Rows(i).Cells(5).Value) <> 0 Then
                            Dim VAT As Double = (1 + Val(PayGrid.Rows(i).Cells(5).Value) / 100)
                            PayGrid.Rows(i).Cells(4).Value = Math.Round(Val(PayGrid.Rows(i).Cells(7).Value) / Val(VAT), 3)
                            PayGrid.Rows(i).Cells(6).Value = Math.Round(Val(PayGrid.Rows(i).Cells(4).Value) * (Val(PayGrid.Rows(i).Cells(5).Value) / 100), 3)
                        Else
                            PayGrid.Rows(i).Cells(4).Value = PayGrid.Rows(i).Cells(7).Value
                        End If
                    End If
                Else
                    MessageBox.Show("لا يمكن ادخال قيمة اقل من القيمة المحددة للدفعة المقدمة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    PayGrid.Rows(i).Cells(7).Value = TxtPayAmount.Text
                    If Val(PayGrid.Rows(i).Cells(5).Value) <> 0 Then
                        Dim VAT As Double = (1 + Val(PayGrid.Rows(i).Cells(5).Value) / 100)
                        PayGrid.Rows(i).Cells(4).Value = Math.Round(Val(PayGrid.Rows(i).Cells(7).Value) / Val(VAT), 3)
                        PayGrid.Rows(i).Cells(6).Value = Math.Round(Val(PayGrid.Rows(i).Cells(4).Value) * (Val(PayGrid.Rows(i).Cells(5).Value) / 100), 3)
                    Else
                        PayGrid.Rows(i).Cells(4).Value = PayGrid.Rows(i).Cells(7).Value
                    End If
                End If
            End If
            PAYTOTAL()
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
            If TxtCostCenterGuid.Text <> Nothing Then
                CmdNewSave.Enabled = True
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindLevel_Click(sender As Object, e As EventArgs) Handles CmdFindLevel.Click
        Try
            If TxtDocGuid.Text <> Nothing Then
                Dim w As New LevelList
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtLevelName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtLevelGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
            Else
                MessageBox.Show("برجاء اختيار نوع المستند اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkStaut_Click(sender As Object, e As EventArgs) Handles ChkStaut.Click

    End Sub

    Private Sub CmdFindCashBox_Click(sender As Object, e As EventArgs) Handles CmdFindCashBox.Click
        Try
            If ChkAttach.Checked = False Then
                Dim w As New CashBoxList
                w.TxtUseInSales.Text = True
                w.Text = "قائمة البنوك /الخزائن"
                'w.TxtType.Text = 1
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCashBoxName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtCashBoxGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                    TxtCostCenterName.Text = w.BranchGrid.Rows(ii).Cells(4).Value.ToString()
                    TxtCostCenterGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
                    CmdNewSave.Enabled = True
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub SHOWCOURSEINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowCourseInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCourseCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowCourseInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtFromDate.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtToDate.Text = ds.Tables(0).Rows(i)(1).ToString()
                CboTime.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtCourseDiplomGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtDiplomName.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtStudentCount.Text = ds.Tables(0).Rows(i)(14).ToString()
                'TxtCourseGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
            Next
        End If
    End Sub
    Sub CHKCOURSECOUNT()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ChkCourseCount", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@CourseGuid", TxtCourseGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ChkCourseCount")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If ds.Tables(0).Rows(i)(0) = 0 Then
                    TxtAvaCount.Text = TxtStudentCount.Text
                Else
                    TxtAvaCount.Text = Val(TxtStudentCount.Text) - Val(ds.Tables(0).Rows(i)(0))
                End If
            Next
        End If
    End Sub
    Sub ADDAUTOGRIDPAY(STUDYTYPE As Integer, DIPLOMGUID As String)
        PayGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowPriceSaleDiplom", connection)
        comm.Parameters.AddWithValue("@PriceSaleGuid", TxtPriceSaleGuid.Text)
        comm.Parameters.AddWithValue("@Type", 0)
        If TxtFindDiploam.Text <> Nothing Then
            comm.Parameters.AddWithValue("@DiplomName", TxtFindDiploam.Text)
        End If
        comm.Parameters.AddWithValue("@StudyType", STUDYTYPE)
        comm.Parameters.AddWithValue("@DiplomGuid", DIPLOMGUID)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowPriceSaleDiplom")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            PayGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                PayGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                PayGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                PayGrid.Rows(COUNT__).Cells(2).Value = "PCS"
                PayGrid.Rows(COUNT__).Cells(3).Value = 1
                PayGrid.Rows(COUNT__).Cells(4).Value = 0
                PayGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(3).ToString()
                PayGrid.Rows(COUNT__).Cells(6).Value = 0
                PayGrid.Rows(COUNT__).Cells(7).Value = 0
                PayGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(6).ToString()
                PayGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(7).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdFindCourse_Click(sender As Object, e As EventArgs) Handles CmdFindCourse.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New CourseList
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCourseName.Text = w.BranchGrid.Rows(ii).Cells(6).Value.ToString()
                    TxtCourseGuid.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
                    TxtCourseCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                End If
                If TxtCourseGuid.Text <> Nothing Then
                    SHOWCOURSEINFO()
                    CHKCOURSECOUNT()
                    If AllFiles.WhatPriceSaleGuidByBranch(TxtBranchGuid.Text) = True Then
                        TxtPriceSaleGuid.Text = AllFiles.PRICESALEGUID.ToString()
                        SHOWPRICESALEONLYCOURSE(1, TxtCourseDiplomGuid.Text)
                        If ChkIfNeedPay.Checked = True Then
                            PayGrid.Rows.Clear()
                            ADDAUTOGRIDPAY(1, TxtCourseDiplomGuid.Text)
                            DIPLOMTOTAL()
                            FINALTOTAL()
                        End If
                    End If
                End If
            Else
                MessageBox.Show("برجاء مراجعة إعدادات التسجيل لعدم وجود فرع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub PRINTREGDOC()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("PrintRegDoc", connection)
        comm.Parameters.AddWithValue("@RegDocGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "PrintRegDoc")
        Dim dt = ds.Tables(0)
        Dim report As New StudentRegDoc
        report.DataSource = dt
        report.DataAdapter = adp
        report.DataMember = "DataTable1"
        report.RequestParameters = False
        Dim indate As String = Convert.ToString(TxtDateReg.Value)
        AllFiles.GETBRANCHINFO(Guid.Parse(TxtBranchGuid.Text))
        Dim barcodeinfo As String = AllFiles.GetQrCodeEncryptionDetails(AllFiles.BRANCHVATNAME, AllFiles.BRANCHVAT, indate, Convert.ToDouble(TxtPaySubTotal.Text).ToString("0.00"), Convert.ToDouble(TxtPayTax.Text).ToString("0.00"))
        report.XrBarCode1.Text = barcodeinfo
        report.Parameters(0).Value = AllFiles.BRANCHNAME
        report.Parameters(1).Value = AllFiles.BRANCHADDRESS
        report.Parameters(2).Value = "الرقم الضريبي - " & " " & AllFiles.BRANCHVAT
        report.Parameters(3).Value = " الهاتف - " & " " & AllFiles.BRANCHTEL
        report.Parameters(4).Value = TxtNotes.Text
        report.Parameters(5).Value = " السجل التجاري - " & " " & AllFiles.BRANCHNFAX
        If TxtRegType.Text = 1 Then
            report.XrLabel51.Visible = True
            report.XrLabel52.Visible = True
            report.XrLabel53.Visible = True
            report.Parameters(6).Value = TxtEmail.Text
            report.Parameters(7).Value = TxtFromDate.Text
            report.Parameters(8).Value = TxtToDate.Text
        End If
        Dim Tool_Report As ReportPrintTool = New ReportPrintTool(report)
        report.CreateDocument()
        Dim frm As New ViewReport
        frm.DocumentViewer1.DocumentSource = report
        frm.ShowDialog()
    End Sub
    Private Sub CmdPrintDoc_Click(sender As Object, e As EventArgs) Handles CmdPrintDoc.Click
        Try
            If TxtGuid.Text = Nothing Then
                MessageBox.Show("برجاء حفظ المستند الحالي لتتمكن من الطباعه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                PRINTREGDOC()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub PRINTINVOICE()
        AllFiles.PRINTTAXINVOICE(TxtPayGuid.Text, TxtBranchGuid.Text, TxtPayNotes.Text)
    End Sub
    Private Sub CmdPrintInvoice_Click(sender As Object, e As EventArgs) Handles CmdPrintInvoice.Click
        Try
            If TxtGuid.Text = Nothing Then
                MessageBox.Show("برجاء حفظ المستند الحالي لتتمكن من الطباعه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                PRINTINVOICE()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub GroupPanel4_Click(sender As Object, e As EventArgs) Handles GroupPanel4.Click

    End Sub

    Private Sub DiplomGrid_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DiplomGrid.CellEndEdit
        Try
            Dim i As Integer = DiplomGrid.CurrentCell.RowIndex
            Dim j As Integer = DiplomGrid.CurrentCell.ColumnIndex
            If j = 4 Then
                DiplomGrid.Rows(i).Cells(2).Value = DiplomGrid.Rows(i).Cells(4).Value
                'DiplomGrid.Rows(i).Cells(3).Value = DiplomGrid.Rows(i).Cells(4).Value
            End If
            DIPLOMTOTAL()
            FINALTOTAL()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub FeesGrid_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles FeesGrid.CellEndEdit
        Try
            Dim i As Integer = FeesGrid.CurrentCell.RowIndex
            Dim j As Integer = FeesGrid.CurrentCell.ColumnIndex
            If j = 4 Then
                FeesGrid.Rows(i).Cells(2).Value = FeesGrid.Rows(i).Cells(4).Value
                FeesGrid.Rows(i).Cells(3).Value = FeesGrid.Rows(i).Cells(4).Value
            End If
            FESSTOTAL()
            FINALTOTAL()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkPayDate_CheckedChanged(sender As Object, e As EventArgs) Handles ChkPayDate.CheckedChanged
        Try
            If ChkPayDate.Checked = True Then
                TxtPayDate.Enabled = True
            Else
                TxtPayDate.Enabled = False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdAttach_Click(sender As Object, e As EventArgs) Handles CmdAttach.Click
        Try
            If ChkAttach.Checked = False Then
                If TxtCashBoxGuid.Text <> Nothing Then
                    Dim w As New AttachForm
                    w.TxtType.Text = TxtRegType.Text
                    w.TxtNationalid.Text = TxtNationalId.Text
                    w.TxtPassword.Text = TxtCashBoxName.Text
                    w.TxtPasseord2.Text = TxtCashBoxName2.Text
                    w.ChkContract.Checked = ChkContract.Checked
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        ChkAttach.Checked = True
                    Else
                        ChkAttach.Checked = False
                    End If
                Else
                    MessageBox.Show("برجاء تحديد الخزينة / البنك اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            Else
                MessageBox.Show("تم ادخال المرفقات بالفعل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFinCashBox2_Click(sender As Object, e As EventArgs) Handles CmdFinCashBox2.Click
        Try
            If ChkAttach.Checked = False Then
                Dim w As New CashBoxList
                w.Text = "قائمة البنوك /الخزائن"
                'w.TxtType.Text = 1
                w.TxtIsUse.Text = True
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCashBoxName2.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                    TxtCashBoxGuid2.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                    TxtCostCenterName2.Text = w.BranchGrid.Rows(ii).Cells(4).Value.ToString()
                    TxtCostCenterGuid2.Text = w.BranchGrid.Rows(ii).Cells(5).Value.ToString()
                    'CmdNewSave.Enabled = True
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChlOtherBank_CheckedChanged(sender As Object, e As EventArgs) Handles ChlOtherBank.CheckedChanged
        Try
            If ChlOtherBank.Checked = True Then
                CmdFinCashBox2.Enabled = True
                TxtRef2.Enabled = True
                ChkPayDate2.Enabled = True
                TxtCashBoxGuid2.Text = Nothing
                TxtCostCenterGuid2.Text = Nothing
                TxtAmount1.Enabled = True
                TxtAmount2.Enabled = True
            Else
                CmdFinCashBox2.Enabled = False
                TxtCashBoxName2.Text = Nothing
                TxtCashBoxGuid2.Text = "00000000-0000-0000-0000-000000000000"
                TxtCostCenterName2.Text = Nothing
                TxtCostCenterGuid2.Text = "00000000-0000-0000-0000-000000000000"
                TxtRef2.Text = Nothing
                TxtRef2.Enabled = False
                ChkPayDate2.Enabled = False
                ChkPayDate2.Checked = False
                TxtAmount1.Enabled = False
                TxtAmount2.Enabled = False
                TxtAmount1.Text = 0
                TxtAmount2.Text = 0
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkPayDate2_CheckedChanged(sender As Object, e As EventArgs) Handles ChkPayDate2.CheckedChanged
        Try
            If ChkPayDate2.Checked = True Then
                TxtPayDate2.Enabled = True
            Else
                TxtPayDate2.Enabled = False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtRef_TextChanged(sender As Object, e As EventArgs) Handles TxtRef.TextChanged

    End Sub

    Private Sub TxtRef_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtRef.KeyDown
        If e.Control And e.KeyCode = Keys.C Then ' منع النسخ (Ctrl+C)
            e.SuppressKeyPress = True
        ElseIf e.Control And e.KeyCode = Keys.V Then ' منع اللصق (Ctrl+V)
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TxtRef2_TextChanged(sender As Object, e As EventArgs) Handles TxtRef2.TextChanged

    End Sub

    Private Sub TxtRef2_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtRef2.KeyDown
        If e.Control And e.KeyCode = Keys.C Then ' منع النسخ (Ctrl+C)
            e.SuppressKeyPress = True
        ElseIf e.Control And e.KeyCode = Keys.V Then ' منع اللصق (Ctrl+V)
            e.SuppressKeyPress = True
        End If
    End Sub
End Class