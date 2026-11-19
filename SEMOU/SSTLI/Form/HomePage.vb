Imports System.Globalization

Public Class HomePage
    Public Sub viewchildform(form As Form)
        form.MdiParent = Me
        form.Show()
    End Sub
    Private Function GetHijriDate(gregorianDate As DateTime) As String
        ' إنشاء كائن للتقويم الهجري
        Dim hijriCalendar As New HijriCalendar()
        Dim hijriCulture As New CultureInfo("ar-SA")
        hijriCulture.DateTimeFormat.Calendar = hijriCalendar

        ' تحويل التاريخ الميلادي إلى هجري بصيغة مخصصة
        Dim hijriDate As String = gregorianDate.ToString("yyyy/MM/dd", hijriCulture)
        Return hijriDate
    End Function
    Private Sub HomePage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim culture As New Globalization.CultureInfo("ar-SA")
        Dim hijriDate As String = GetHijriDate(DateTime.Now)
        TxtDate2.Caption = hijriDate
        TxtDate1.Caption = Convert.ToDateTime(Now.ToShortDateString).ToString("yyyy/MM/dd")
        Dim frm As New About
        viewchildform(frm)
        SHOWPROFILE()
        AllFiles.BEGINVIEWCACHE()
        Try
            SHOWCONTANENT_FILE()
            SHOWCONTANENT_ACCOUNT()
            SHOWCONTANENT_CONTROL()
            SHOWCONTANENT_BRANCHTOOL()
            SHOWCONTANENT_REPORT()
            SHOWCONTANENT_CUSTSERVICES()
        Finally
            AllFiles.ENDVIEWCACHE()
        End Try
        '''''''''''''''''''''''''''''''''''''' جزئية عرض الاستمارات الغير موقع لها اتفاقية تدريب
        If AllFiles.ChkIfFoundRegDocNotContract(AllFiles.SELLERMAN_GUID____.ToString) = True Then
            Dim msgfrm As New MssgHome
            viewchildform(msgfrm)
        End If
    End Sub

    Private Sub CompanyInfo_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles CompanyInfo.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingCompanyInfo)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingCompanyInfo
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingCompanyInfo
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewBranch_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewBranch.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingBranch)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingBranch
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingBranch
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewUser_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewUser.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingUser)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingUser
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingUser
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewTrainer_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewTrainer.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingTrainer)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingTrainer
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingTrainer
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewDeploma_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewDeploma.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingDiplom)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingDiplom
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingDiplom
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewBatch_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewBatch.ItemClick
        Try
            Dim frm As New EncodingBatch
            frm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewLevel_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewLevel.ItemClick
        Try
            Dim frm As New EncodingLevel
            frm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewAccount_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewAccount.ItemClick
        Try
            Dim frm As New EncodingAccount
            frm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewCostCenter_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewCostCenter.ItemClick
        Try
            Dim frm As New EncodingCostCenter
            frm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewCashBox_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewCashBox.ItemClick
        Try
            Dim frm As New EncodingCashBox
            frm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewBank_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewBank.ItemClick
        Try
            Dim frm As New EncodingBank
            frm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub RegCases_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles RegCases.ItemClick
        Try
            Dim frm As New EncodingRegCases
            frm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub DisCar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles DisCar.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of DisCountHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New DisCountHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New DisCountHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewBook_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewBook.ItemClick
        Try
            Dim frm As New EncodingBook
            frm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub StudyPlan_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles StudyPlan.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingStudyPlan)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingStudyPlan
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingStudyPlan
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub Company_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles Company.ItemClick
        Try
            Dim frm As New EncodingCompany
            frm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub PriceList_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles PriceList.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingPriceList)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingPriceList
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingPriceList
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub OldSellerReport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles OldSellerReport.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of OldSellerReport)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New OldSellerReport
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New OldSellerReport
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub RegDoc_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles RegDocView.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of RegReview)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New RegReview
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New RegReview
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub OtherFees_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles OtherFees.ItemClick
        Try
            Dim w As New EncodingOtherFees
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewAccountChart_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewAccountChart.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of AccountHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New AccountHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New AccountHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewCostCenterChart_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewCostCenterChart.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of CostCenterHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New CostCenterHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New CostCenterHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub AccountSeting_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles AccountSeting.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodinAccountSetting)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodinAccountSetting
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodinAccountSetting
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ReceptionDesk_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles ReceptionDesk.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of MainFindStudent)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New MainFindStudent
                    frm.TxtFind.Select()
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New MainFindStudent
                frm.TxtFind.Select()
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub RegSetting_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles RegSetting.ItemClick
        Try
            Dim w As New EncodingRegSetting
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewSalesMan_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewSalesMan.ItemClick
        Try
            Dim w As New EncodingSalesMan
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub SHOWPROFILE()
        If AllFiles.MENU_VIEW__("file") = False Then
            File.Visible = False
        End If
        If AllFiles.MENU_VIEW__("account") = False Then
            Account.Visible = False
        End If
        If AllFiles.MENU_VIEW__("control") = False Then
            Control.Visible = False
        End If
        If AllFiles.MENU_VIEW__("student") = False Then
            BranchTool.Visible = False
        End If
        If AllFiles.MENU_VIEW__("custservices") = False Then
            CustServices.Visible = False
        End If
        If AllFiles.MENU_VIEW__("report") = False Then
            Report.Visible = False
        End If
        If AllFiles.MENU_VIEW__("toolbranch") = False Then
            BranchTool.Visible = False
        End If
    End Sub
    Sub SHOWCONTANENT_FILE()
        If AllFiles.FORVIEW("addbranch") = False Then
            NewBranch.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("adduser") = False Then
            NewUser.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("addtrainer") = False Then
            NewTrainer.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("addsalesman") = False Then
            NewSalesMan.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("addpricelist") = False Then
            PriceList.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("regsetting") = False Then
            RegSetting.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("resdesk") = False Then
            ReceptionDesk.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("distype") = False Then
            AddDisCount.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("accountsetting") = False Then
            AccountSeting.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("pricesale") = False Then
            PriceSale.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("adddiplom") = False Then
            NewDeploma.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("offerprice") = False Then
            OfferPrice.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("discountcar") = False Then
            DisCar.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("sector") = False Then
            Company.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
    End Sub
    Sub SHOWCONTANENT_ACCOUNT()
        If AllFiles.FORVIEW("addaccount") = False Then
            NewAccount.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("addcostcenter") = False Then
            NewCostCenter.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("accountchart") = False Then
            NewAccountChart.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("centertree") = False Then
            NewCostCenterChart.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("addcashbox") = False Then
            NewCashBox.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("addbank") = False Then
            NewBank.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("opendaily") = False Then
            OpenDaily.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("daily") = False Then
            Daily.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("receipt") = False Then
            NewReceipt.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("pay") = False Then
            NewPay.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("dailyhome") = False Then
            NewDaily.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("statment") = False Then
            NewAccountStatment.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("transmoney") = False Then
            TransMoney.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("cashhistory") = False Then
            NewCashHistory.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("mezan") = False Then
            TrailView.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("discount") = False Then
            DiscountDoc.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("openbalance") = False Then
            OpenBalance.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("instatment") = False Then
            InstatmentHome.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
    End Sub
    Sub SHOWCONTANENT_CONTROL()
        If AllFiles.FORVIEW("addbatch") = False Then
            NewBatch.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("addlevel") = False Then
            NewLevel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If

        If AllFiles.FORVIEW("addstaut") = False Then
            RegCases.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If

        If AllFiles.FORVIEW("addcourse") = False Then
            OpenCourse.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If

        If AllFiles.FORVIEW("question") = False Then
            AddQuestion.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("examsetting") = False Then
            TestSetting.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("allcourse") = False Then
            AllCourseReport.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("newstudenthome") = False Then
            NewStudent.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("diplomstudent") = False Then
            LevelReport.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("coursestudent") = False Then
            StudentCourse.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("batchcount") = False Then
            BatchCount.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("addbatchcount") = False Then
            BatchCountHome.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
    End Sub
    Sub SHOWCONTANENT_BRANCHTOOL()
        If AllFiles.FORVIEW("worktime") = False Then
            BranchTime.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("classroom") = False Then
            BranchClassRoom.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("addbook") = False Then
            NewBook.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("addplan") = False Then
            StudyPlan.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("addclass") = False Then
            AddClass.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("studytable") = False Then
            ClassTable.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("classtabel") = False Then
            ClassTabelHome.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("paymentorder") = False Then
            PayOrder.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("receiptorder") = False Then
            ReceiptOrder.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("branchdaliy") = False Then
            BranchDaly.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("contract") = False Then
            AskContract.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("contractfollow") = False Then
            ContractFollow.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("traniertabel") = False Then
            TabelOfTranier.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("roomtabel") = False Then
            TabelOfRoom.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
    End Sub
    Sub SHOWCONTANENT_REPORT()
        If AllFiles.FORVIEW("salesreport") = False Then
            AllSalesInvoice.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("reviewdesk") = False Then
            RegDocView.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("custbalance") = False Then
            CustomerList.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("salesmanreport") = False Then
            SalesManHome.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("taxreport") = False Then
            TaxReport.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("customerbalance") = False Then
            CustomerBalance.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("expenseanalysis") = False Then
            ExpenesAnalysis.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
    End Sub
    Sub SHOWCONTANENT_CUSTSERVICES()
        If AllFiles.FORVIEW("crm") = False Then
            CustOffice.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
        If AllFiles.FORVIEW("crmreport") = False Then
            CrmDaily.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
        End If
    End Sub
    Private Sub NewDaily_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewDaily.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of DailyHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New DailyHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New DailyHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub LevelReport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles LevelReport.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of LevelReportForm)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New LevelReportForm
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New LevelReportForm
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub AddQuestion_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles AddQuestion.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingQuestion)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingQuestion
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingQuestion
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TestSetting_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TestSetting.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingTestSetting)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingTestSetting
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingTestSetting
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub OpenDaily_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles OpenDaily.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingOpenDaily)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingOpenDaily
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingOpenDaily
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TrailView_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TrailView.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of TrailHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New TrailHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New TrailHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewReceipt_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewReceipt.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingCashDayReceipt)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingCashDayReceipt
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingCashDayReceipt
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewPay_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewPay.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingCashDayPay)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingCashDayPay
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingCashDayPay
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewAccountStatment_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewAccountStatment.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of GeneralAccountStatment)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New GeneralAccountStatment
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New GeneralAccountStatment
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CustomerList_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles CustomerList.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of CustomerHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New CustomerHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New CustomerHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub StudentDashBoard_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles StudentDashBoard.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of StudentDashBoard)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New StudentDashBoard
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New StudentDashBoard
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewStudent_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewStudent.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of NewStudentHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New NewStudentHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New NewStudentHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub OpenCourse_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles OpenCourse.ItemClick
        Try
            Dim frm As New EncodingCourse
            frm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub StudentCourse_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles StudentCourse.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of StudentCourseList)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New StudentCourseList
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New StudentCourseList
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub Daily_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles Daily.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingDaily)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingDaily
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingDaily
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewCashHistory_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewCashHistory.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of CashBoxHistory)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New CashBoxHistory
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New CashBoxHistory
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub AllSalesInvoice_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles AllSalesInvoice.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of SalesReport)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New SalesReport
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New SalesReport
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub AllCourseReport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles AllCourseReport.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of CourseHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New CourseHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New CourseHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TransMoney_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TransMoney.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingTransMoney)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingTransMoney
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingTransMoney
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub AddDisCount_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles AddDisCount.ItemClick
        Try
            Dim w As New EncodingDiscountType
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub DiscountDoc_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles DiscountDoc.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingDiscountDoc)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingDiscountDoc
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingDiscountDoc
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub PriceSale_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles PriceSale.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingPriceSale)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingPriceSale
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingPriceSale
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub NewChangePassword_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewChangePassword.ItemClick
        Dim w As New ChangeUserPassword
        w.ShowDialog()
    End Sub

    Private Sub NewLogOut_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles NewLogOut.ItemClick
        Try
            Dim frm As New LogIn
            frm.Show()
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub SalesManHome_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles SalesManHome.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of SalesManReport)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New SalesManReport
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New SalesManReport
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub OfferPrice_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles OfferPrice.ItemClick
        Try
            Dim frm As New EncodingOfferPrice
            frm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub PublicSetting_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles PublicSetting.ItemClick

    End Sub

    Private Sub TaxReport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TaxReport.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of TaxHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New TaxHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New TaxHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ReTaxReport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles ReTaxReport.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of ReTaxReport)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New ReTaxReport
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New ReTaxReport
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdTech_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles CmdTech.ItemClick
        Try
            Dim startInfo = New ProcessStartInfo()
            startInfo.FileName = "\\192.168.8.200\Update\AnyDesk.exe"
            Process.Start(startInfo)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BatchCount_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BatchCount.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of BatchSeatsHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New BatchSeatsHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New BatchSeatsHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BranchTime_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BranchTime.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of BranchTimeWork)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New BranchTimeWork
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New BranchTimeWork
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BranchClassRoom_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BranchClassRoom.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingCalssRoom)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingCalssRoom
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingCalssRoom
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ClassTable_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles ClassTable.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingTabel)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingTabel
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingTabel
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ClassTabelHome_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles ClassTabelHome.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of ClassTabel)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New ClassTabel
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New ClassTabel
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub AddClass_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles AddClass.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of EncodingClass)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New EncodingClass
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New EncodingClass
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub OpenBalance_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles OpenBalance.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of AccountOpenBalnceList)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New AccountOpenBalnceList
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New AccountOpenBalnceList
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub PayOrder_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles PayOrder.ItemClick
        Try
            Dim w As New EncodingPaymentOrder
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ReceiptOrder_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles ReceiptOrder.ItemClick
        Try
            Dim w As New EncodingReceiptOrder
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BranchDaly_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BranchDaly.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of BranchDailyHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New BranchDailyHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New BranchDailyHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub AskContract_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles AskContract.ItemClick
        Try
            Dim w As New ContractHome
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub InstatmentHome_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles InstatmentHome.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of IncomeStatment)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New IncomeStatment
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New IncomeStatment
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CustomerBalance_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles CustomerBalance.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of CustomerBalanceHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New CustomerBalanceHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New CustomerBalanceHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ContractFollow_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles ContractFollow.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of ContractReport)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New ContractReport
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New ContractReport
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CustOffice_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles CustOffice.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of MainCallHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New MainCallHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New MainCallHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BatchCountHome_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BatchCountHome.ItemClick
        Try
            Dim w As New EncodingBatchCount
            w.ShowDialog()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ExpenesAnalysis_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles ExpenesAnalysis.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of ExpenseAnalysisHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New ExpenseAnalysisHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New ExpenseAnalysisHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CrmDaily_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles CrmDaily.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of CrmDailyHome)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New CrmDailyHome
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New CrmDailyHome
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TabelOfTranier_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TabelOfTranier.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of TranierTabel)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New TranierTabel
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New TranierTabel
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TabelOfRoom_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TabelOfRoom.ItemClick
        Try
            Dim xopen = Application.OpenForms().OfType(Of RoomTabel)
            If xopen.Any Then
                Dim result As DialogResult = MessageBox.Show("الشاشة مفتوحة من قبل .. هل تريد فتح شاشة آخري", "Medico", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then
                    Dim frm As New RoomTabel
                    viewchildform(frm)
                Else
                    xopen.Last().Select()
                End If
            Else
                Dim frm As New RoomTabel
                viewchildform(frm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class
