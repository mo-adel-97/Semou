Imports System.Data.SqlClient
Imports System.Text

Public Class AllFiles
    Public Shared USER_GUID____ As Guid
    Public Shared SELLERMAN_GUID____ As Guid
    Public Shared USER_NAME____ As String
    Public Shared DEFBRANCHGUID As Guid
    Public Shared DEFBRANCHNAME As String
    Public Shared DEFBRANCH As Boolean
    Public Shared ZEROAMOUNT As Boolean
    Public Shared NOFILEADDFESS As Boolean
    Public Shared ONLINE_USER_NAME____ As String
    Public Shared ActionType___ As String = Nothing
    Public Shared ACCOUNTCODE As Double
    Public Shared COSTCENTERCODE As Double
    Public Shared PARENTGUIDFORNEW As String
    Public Shared PARENTCODEFORNEW As String
    Public Shared PARENTNAMEFORNEW As String
    Public Shared BRANCHNAME As String
    Public Shared BRANCHNFAX As String
    Public Shared BRANCHADDRESS As String
    Public Shared BRANCHVAT As String
    Public Shared BRANCHVATNAME As String
    Public Shared BRANCHTEL As String
    Public Shared OPENACCOUNTGUID As String
    Public Shared REALSTUDENTLEVELGUID As String
    Public Shared REALSTUDENTBATCHGUID As String
    Public Shared REALSTUDENTDIPLOMGUID As String
    Public Shared REALSTUDENTBRANCHCODE As String
    Public Shared REALSTUDENTCASEGUID As String
    Public Shared REALSTUDENTCASENAME As String
    Public Shared COURSESTUDENT As String = 0
    Public Shared PRICESALEGUID As String
    Public Shared appPath As String = Application.StartupPath()
    Public Shared GROUPCOUNT As String
    Public Shared DIPLOMNAME As String
    Public Shared BILLDATE As String
    Public Shared BILLREF As String
    Public Shared CASHNAME As String
    Public Shared PAYMAN As String
    Public Shared SYSTEMVER As String
    Public Shared UPDATEURL As String
    Public Shared SETTINGPASS As String
    Public Shared UPDATEEXE As String
    Public Shared EXENAME As String
    Public Shared ZUSERNAME As String
    Public Shared ZPASSWORD As String
    Public Shared ZOTP As String
    Public Shared Function SHOWSYSTEMINFO()
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowSystemInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowSystemInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                SYSTEMVER = ds.Tables(0).Rows(i)(0).ToString()
                UPDATEURL = ds.Tables(0).Rows(i)(1).ToString()
                SETTINGPASS = ds.Tables(0).Rows(i)(2).ToString()
                UPDATEEXE = ds.Tables(0).Rows(i)(3).ToString()
                EXENAME = ds.Tables(0).Rows(i)(4).ToString()
                ZUSERNAME = ds.Tables(0).Rows(i)(6).ToString()
                ZPASSWORD = ds.Tables(0).Rows(i)(7).ToString()
                ZOTP = ds.Tables(0).Rows(i)(8).ToString()
            Next
        End If
        Return True
    End Function

    Public Shared Function MENU_VIEW__(FormName As String) As Boolean
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowUserProfile", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@UserGuid", USER_GUID____)
        comm.Parameters.AddWithValue("@UnqName", FormName)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowUserProfile")
        For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
            If ds.Tables(0).Rows(i)(0) = False Then
                Return False
            Else
                Return True
            End If
        Next
        Return True
    End Function

    Public Shared Function FORSAVE(FormName As String) As Boolean
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("DoUserHavePremision", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@UserGuid", USER_GUID____)
        comm.Parameters.AddWithValue("@FormName", FormName)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "DoUserHavePremision")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If ds.Tables(0).Rows(i)(0) = False Then
                    MessageBox.Show("لا تملك صلاحية اضافة جديد", "مدير النظام", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Else
                    Return True
                End If
            Next
        Else
            MessageBox.Show("لا تملك صلاحية اضافة جديد", "مدير النظام", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Public Shared Function FORUPDATE(FormName As String) As Boolean
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("DoUserHavePremision", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@UserGuid", USER_GUID____)
        comm.Parameters.AddWithValue("@FormName", FormName)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "DoUserHavePremision")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If ds.Tables(0).Rows(i)(1) = False Then
                    MessageBox.Show("لا تملك صلاحية تعديل البيانات", "مدير النظام", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Else
                    Return True
                End If
            Next
        Else
            MessageBox.Show("لا تملك صلاحية تعديل البيانات", "مدير النظام", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Public Shared Function FORFIND(FormName As String) As Boolean
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("DoUserHavePremision", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@UserGuid", USER_GUID____)
        comm.Parameters.AddWithValue("@FormName", FormName)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "DoUserHavePremision")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If ds.Tables(0).Rows(i)(4) = False Then
                    MessageBox.Show("لا تملك صلاحية استعراض البيانات", "مدير النظام", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Else
                    Return True
                End If
            Next
        Else
            MessageBox.Show("لا تملك صلاحية استعراض البيانات", "مدير النظام", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    ''''''''''''''''''''''''''''''''' كاش صلاحيات العرض - استعلام واحد بدل استعلام لكل شاشة وقت فتح الصفحة الرئيسية
    Private Shared VIEWCACHE As Dictionary(Of String, Boolean) = Nothing
    Public Shared Sub BEGINVIEWCACHE()
        Try
            Dim cache As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            Using connection As New SqlConnection(New SemouEntities().Database.Connection.ConnectionString)
                Dim comm As New SqlCommand("SELECT F.UnqName, P.View_ FROM dbo.Form_Name F INNER JOIN dbo.User_Premision P ON F.Guid = P.FormGuid WHERE P.UserGuid = @UserGuid", connection)
                comm.Parameters.AddWithValue("@UserGuid", USER_GUID____)
                connection.Open()
                Using reader = comm.ExecuteReader()
                    While reader.Read()
                        Dim key As String = reader(0).ToString().Trim()
                        If Not cache.ContainsKey(key) Then
                            cache(key) = Not IsDBNull(reader(1)) AndAlso CBool(reader(1))
                        End If
                    End While
                End Using
            End Using
            VIEWCACHE = cache
        Catch
            VIEWCACHE = Nothing ' لو حصل خطأ نرجع للطريقة القديمة
        End Try
    End Sub
    Public Shared Sub ENDVIEWCACHE()
        VIEWCACHE = Nothing
    End Sub
    ''''''''''''''''''''''''''''''''' رقم الفاتورة الضريبية (01 ، 02 ...) - الفواتير القديمة ترجع برقمها القديم
    Public Shared Function GetTaxInvoiceNo(BILLGUID As String) As String
        Using connection As New SqlConnection(New SemouEntities().Database.Connection.ConnectionString)
            connection.Open()
            Dim code As String = Nothing
            Using comm As New SqlCommand("SELECT Code FROM dbo.Bill WHERE Guid = @Guid", connection)
                comm.Parameters.AddWithValue("@Guid", Guid.Parse(BILLGUID))
                code = Convert.ToString(comm.ExecuteScalar())
            End Using
            Try
                Using comm As New SqlCommand("SELECT TaxNo FROM dbo.Bill WHERE Guid = @Guid", connection)
                    comm.Parameters.AddWithValue("@Guid", Guid.Parse(BILLGUID))
                    Dim taxNo = comm.ExecuteScalar()
                    If taxNo IsNot Nothing AndAlso Not IsDBNull(taxNo) Then
                        Return Convert.ToInt32(taxNo).ToString("00")
                    End If
                End Using
            Catch ex As SqlException
                ' عمود TaxNo مش موجود (سكريبت القاعدة لسه ماتشغلش) - نكمل بالرقم القديم
            End Try
            Return code
        End Using
    End Function
    ' للتقارير: الرقم الداخلي -> رقم الفاتورة الضريبية (للفواتير اللي ليها رقم ضريبي بس)
    Public Shared Function GETTAXNOMAP() As Dictionary(Of String, String)
        Dim map As New Dictionary(Of String, String)
        Try
            Using connection As New SqlConnection(New SemouEntities().Database.Connection.ConnectionString)
                Using comm As New SqlCommand("SELECT Code, TaxNo FROM dbo.Bill WHERE TaxNo IS NOT NULL", connection)
                    connection.Open()
                    Using reader = comm.ExecuteReader()
                        While reader.Read()
                            map(reader(0).ToString().Trim()) = Convert.ToInt32(reader(1)).ToString("00")
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            ' عمود TaxNo مش موجود - التقارير تفضل بالرقم القديم
        End Try
        Return map
    End Function
    ' البحث برقم الفاتورة: لو الرقم المكتوب رقم ضريبي نرجع الرقم الداخلي بتاعه
    Public Shared Function TAXNOTOCODE(TEXT As String) As String
        Dim n As Integer
        If Integer.TryParse(TEXT.Trim(), n) Then
            Try
                Using connection As New SqlConnection(New SemouEntities().Database.Connection.ConnectionString)
                    Using comm As New SqlCommand("SELECT Code FROM dbo.Bill WHERE TaxNo = @TaxNo", connection)
                        comm.Parameters.AddWithValue("@TaxNo", n)
                        connection.Open()
                        Dim code = comm.ExecuteScalar()
                        If code IsNot Nothing AndAlso Not IsDBNull(code) Then
                            Return code.ToString()
                        End If
                    End Using
                End Using
            Catch ex As SqlException
            End Try
        End If
        Return TEXT
    End Function
    Public Shared Sub SETTAXINVOICENO(dt As DataTable, BILLGUID As String)
        If Not dt.Columns.Contains("TaxNo") Then
            dt.Columns.Add("TaxNo", GetType(String))
        End If
        Dim taxNo As String = GetTaxInvoiceNo(BILLGUID)
        For Each row As DataRow In dt.Rows
            row("TaxNo") = taxNo
        Next
    End Sub
    ''''''''''''''''''''''''''''''''' طباعة الفاتورة الضريبية - شكل موحد من كل الشاشات
    Public Shared Sub PRINTTAXINVOICE(BILLGUID As String, BRANCHGUID As String, NOTES As String)
        Dim dt As New DataTable()
        Using connection As New SqlConnection(New SemouEntities().Database.Connection.ConnectionString)
            Using comm As New SqlCommand("PrintBill", connection)
                comm.CommandType = CommandType.StoredProcedure
                comm.Parameters.AddWithValue("@BillGuid", BILLGUID)
                Using adp As New SqlDataAdapter(comm)
                    adp.Fill(dt)
                End Using
            End Using
        End Using
        If dt.Rows.Count = 0 Then
            MessageBox.Show("لم يتم العثور على بيانات الفاتورة!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        SETTAXINVOICENO(dt, BILLGUID)
        GETBRANCHINFO(Guid.Parse(BRANCHGUID))

        Dim first As DataRow = dt.Rows(0)
        Dim billDate As DateTime = Convert.ToDateTime(first("BillDate"))
        If Not dt.Columns.Contains("InvDate") Then
            dt.Columns.Add("InvDate", GetType(String))
        End If
        Dim invDate As String = billDate.ToString("yyyy/MM/dd   hh:mm tt", Globalization.CultureInfo.InvariantCulture)
        For Each row As DataRow In dt.Rows
            row("InvDate") = invDate
        Next

        ' كود QR بصيغة الهيئة: التاريخ ISO 8601 والمبالغ بنقطة عشرية
        Dim inv = Globalization.CultureInfo.InvariantCulture
        Dim qrText As String = GetQrCodeEncryptionDetails(BRANCHVATNAME, BRANCHVAT,
                                    billDate.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", inv),
                                    Convert.ToDouble(first("BillSubTotal")).ToString("0.00", inv),
                                    Convert.ToDouble(first("BillTax")).ToString("0.00", inv))

        Dim report As New TaxInvoiceReport(dt, New TaxInvoiceHeader With {
            .CompanyName = BRANCHNAME,
            .Address = BRANCHADDRESS,
            .VatNo = BRANCHVAT,
            .CrNo = BRANCHNFAX,
            .Phone = BRANCHTEL,
            .Notes = NOTES,
            .QrText = qrText})
        report.CreateDocument()
        Dim frm As New ViewReport
        frm.DocumentViewer1.DocumentSource = report
        frm.ShowDialog()
    End Sub
    Public Shared Function FORVIEW(FormName As String) As Boolean
        If VIEWCACHE IsNot Nothing Then
            Dim allow As Boolean
            Return VIEWCACHE.TryGetValue(FormName.Trim(), allow) AndAlso allow
        End If
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("DoUserHavePremision", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@UserGuid", USER_GUID____)
        comm.Parameters.AddWithValue("@FormName", FormName)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "DoUserHavePremision")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If ds.Tables(0).Rows(i)(5) = False Then
                    Return False
                Else
                    Return True
                End If
            Next
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function GETACCOUNTCODE(PARENTGUID As Guid, PARENTCODE As String) As Integer
        Dim dbcontext As New SemouEntities
        Dim MaxId = dbcontext.Accounts.Where(Function(m) m.ParentGuid = PARENTGUID).OrderByDescending(Function(p) p.BacicCode).FirstOrDefault()?.BacicCode
        Dim MaxDay = New Account
        If (IsNothing(MaxId)) Then
            MaxId = 1 + Val(PARENTCODE) * 100
            ACCOUNTCODE = MaxId
        Else
            MaxId += 1
            ACCOUNTCODE = MaxId
        End If
        Return ACCOUNTCODE
    End Function
    Public Shared Function GETCOSTCENTERCODE(PARENTGUID As Guid, PARENTCODE As String) As Integer
        Dim dbcontext As New SemouEntities
        Dim MaxId = dbcontext.CostCenters.Where(Function(m) m.ParentGuid = PARENTGUID).OrderByDescending(Function(p) p.BasicCode).FirstOrDefault()?.BasicCode
        Dim MaxDay = New CostCenter
        If (IsNothing(MaxId)) Then
            MaxId = 1 + Val(PARENTCODE) * 100
            COSTCENTERCODE = MaxId
        Else
            MaxId += 1
            COSTCENTERCODE = MaxId
        End If
        Return COSTCENTERCODE
    End Function
    Public Shared Function IFACCOUNTISPARENT(FACCOUNTGUID As Guid) As Boolean
        Dim dbcontext As New SemouEntities
        Dim match = (From p In dbcontext.Accounts.Where(Function(m) m.Guid = FACCOUNTGUID) Select p.Guid, p.COUNT_ACCOUNT).FirstOrDefault()
        Dim MaxDay = New Account
        If match.COUNT_ACCOUNT <> 0 Then
            Return False
        Else
            Return True
        End If
        Return True
    End Function
    Public Shared Function CHEKPARENTINFO(DEPGUID As String) As Boolean
        Dim dbcontext As New SemouEntities
        Dim MaxId = dbcontext.AccountSettings.Where(Function(m) m.DepGuid = DEPGUID).OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New AccountSetting
        If (IsNothing(MaxId)) Then
            MessageBox.Show("لا يمكن اضافة طالب غير متاح الحساب الرئيسي" _
                           & vbCrLf & "برجاء التواصل مع مدير النظام" _
                            , "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Else
            Return True
        End If
        Return True
    End Function
    Public Shared Function GETPARENTINFOFROMSETTING(DEPGUID As String)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowAccountSetting", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@DepGuid", DEPGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowAccountSetting")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                PARENTNAMEFORNEW = ds.Tables(0).Rows(i)(1).ToString()
                PARENTGUIDFORNEW = ds.Tables(0).Rows(i)(3).ToString()
                PARENTCODEFORNEW = ds.Tables(0).Rows(i)(4).ToString()
                OPENACCOUNTGUID = ds.Tables(0).Rows(i)(7).ToString()
            Next
        End If
        Return True
    End Function
    Public Shared Function WhatPriceSaleGuidByBranch(BRANCHGUID As String)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("WhatPriceSaleGuidByBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BranchGuid", BRANCHGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "WhatPriceSaleGuidByBranch")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                PRICESALEGUID = ds.Tables(0).Rows(i)(0).ToString()
            Next
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function WHATONLINEUSERINFO()
        Dim dbcontext As New SemouEntities
        Dim match = (From p In dbcontext.User_Info.Where(Function(m) m.Guid = USER_GUID____) Select p.Guid, p.FullName).FirstOrDefault()
        Dim MaxDay = New User_Info
        If (IsNothing(match)) Then
        Else
            ONLINE_USER_NAME____ = match.FullName
        End If
        Return ONLINE_USER_NAME____
    End Function

    Public Shared Function GetQrCodeEncryptionDetails(SellerName As String, VatNo As String, DtTime As String, InvTotal As String, VatTotal As String) As String
        Dim r As String = ""
        Dim Tages = New Integer() {1, 2, 3, 4, 5}

        '' Fix Date Format and Make it 20 Len
        'Dim Dat1 = DateTime.ParseExact(DtTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")
        'Dim Tim1 = DateTime.ParseExact(DtTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).ToString("HH:mm:ss")
        'DtTime = Dat1 & "T" & Tim1 & "Z"

        Try
            Dim SelLen As Integer = Encoding.UTF8.GetByteCount(SellerName)
            Dim SelTag As Integer = Tages(0)
            Dim VatNoLen As Integer = Encoding.UTF8.GetByteCount(VatNo)
            Dim VatNoTag As Integer = Tages(1)
            Dim DtTimeLen As Integer = Encoding.UTF8.GetByteCount(DtTime)
            Dim DtTimeTag As Integer = Tages(2)
            Dim InvTotalLen As Integer = Encoding.UTF8.GetByteCount(InvTotal)
            Dim InvTotalTag As Integer = Tages(3)
            Dim VatTotalLen As Integer = Encoding.UTF8.GetByteCount(VatTotal)
            Dim VatTotalTag As Integer = Tages(4)

            ' Convert Seller First
            r &= BinaryStringToHexString(SelTag) & BinaryStringToHexString(SelLen)
            Dim Selbytes As Byte() = Encoding.UTF8.GetBytes(SellerName)
            Dim sb As New StringBuilder()
            For Each Value As Byte In Selbytes
                sb.Append(Value.ToString("x2"))
            Next
            Dim SelHex As String = sb.ToString()
            r &= SelHex

            ' Convert VatNo
            r &= BinaryStringToHexString(VatNoTag) & BinaryStringToHexString(VatNoLen)
            Dim VNObytes As Byte() = Encoding.Default.GetBytes(VatNo)
            Dim VNoHex As String = BitConverter.ToString(VNObytes).Replace("-", "")
            r &= VNoHex

            ' Convert DtTime
            r &= BinaryStringToHexString(DtTimeTag) & BinaryStringToHexString(DtTimeLen)
            Dim Datbytes As Byte() = Encoding.Default.GetBytes(DtTime)
            Dim dDatHex As String = BitConverter.ToString(Datbytes).Replace("-", "")
            r &= dDatHex

            ' Convert InvTotal
            r &= BinaryStringToHexString(InvTotalTag) & BinaryStringToHexString(InvTotalLen)
            Dim InvTotbytes As Byte() = Encoding.Default.GetBytes(InvTotal)
            Dim InvTotHex As String = BitConverter.ToString(InvTotbytes).Replace("-", "")
            r &= InvTotHex

            ' Convert VatTotal
            r &= BinaryStringToHexString(VatTotalTag) & BinaryStringToHexString(VatTotalLen)
            Dim VatTotByte As Byte() = Encoding.Default.GetBytes(VatTotal)
            Dim VatTotHex As String = BitConverter.ToString(VatTotByte).Replace("-", "")
            r &= VatTotHex

            ' Fix R and Return Base64
            Dim bytes() As Byte = New Byte(r.Length / 2 - 1) {}
            For i As Integer = 0 To bytes.Length - 1
                bytes(i) = Convert.ToByte(r.Substring(i * 2, 2), 16)
            Next
            r = Convert.ToBase64String(bytes)
        Catch ex As Exception
            ' Handle exception here
        End Try

        Return r
    End Function
    Public Shared Function BinaryStringToHexString(ByVal X As Integer) As String
        Dim hexValue As String = X.ToString("X") ' Use the old way (more efficient for integer to hex conversion)
        If hexValue.Length = 1 Then
            hexValue = "0" & hexValue ' Add leading zero if necessary
        End If
        Return hexValue
    End Function
    Public Shared Function GETBRANCHINFO(BRANCHGUID As Guid) As Integer
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("LoadBranchInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BranchGuid", BRANCHGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadBranchInfo")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BRANCHNAME = ds.Tables(0).Rows(i)(0).ToString()
                BRANCHADDRESS = ds.Tables(0).Rows(i)(1).ToString()
                BRANCHVAT = ds.Tables(0).Rows(i)(2).ToString()
                BRANCHTEL = ds.Tables(0).Rows(i)(3).ToString()
                BRANCHVATNAME = ds.Tables(0).Rows(i)(4).ToString()
                BRANCHNFAX = ds.Tables(0).Rows(i)(5).ToString()
            Next
        End If
        Return True
    End Function
    Public Shared Function GETREALSTUDENTINFO(NATIONALID As String)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("FoundStudentInOld", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@NationalCode", NATIONALID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "FoundStudentInOld")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                REALSTUDENTLEVELGUID = ds.Tables(0).Rows(i)(0).ToString()
                REALSTUDENTBATCHGUID = ds.Tables(0).Rows(i)(1).ToString()
                REALSTUDENTDIPLOMGUID = ds.Tables(0).Rows(i)(2).ToString()
                REALSTUDENTBRANCHCODE = ds.Tables(0).Rows(i)(3).ToString()
                REALSTUDENTCASEGUID = ds.Tables(0).Rows(i)(4).ToString()
                REALSTUDENTCASENAME = ds.Tables(0).Rows(i)(5).ToString()
            Next
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function GETROTHEREALSTUDENTINFO(NATIONALID As String, BRANCHCODE As Integer)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowCourseStudentInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@NationalId", NATIONALID)
        comm.Parameters.AddWithValue("@BranchCode", BRANCHCODE)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowCourseStudentInfo")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

            Next
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function CHEKDEFBRANCHINFO(USERGUID As String)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ChkDefBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", USERGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "@Guid")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DEFBRANCH = ds.Tables(0).Rows(i)(0)
                DEFBRANCHNAME = ds.Tables(0).Rows(i)(1).ToString()
                DEFBRANCHGUID = Guid.Parse(ds.Tables(0).Rows(i)(2).ToString())
                ZEROAMOUNT = ds.Tables(0).Rows(i)(3)
                NOFILEADDFESS = ds.Tables(0).Rows(i)(4)
            Next
        End If
        Return True
    End Function
    Public Shared Function ASKFORSTUDENTBILL(STUDENTGUID As String)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("AskIfFoundBill", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@AccountGuid", STUDENTGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "AskIfFoundBill")
        If ds.Tables(0).Rows.Count > 0 Then
            'For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
            '    PRICESALEGUID = ds.Tables(0).Rows(i)(0).ToString()
            'Next
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function ASKGROUPISUSED(GROUPID As Integer)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ChkIsGroupUsed", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@GroupId", GROUPID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ChkIsGroupUsed")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                GROUPCOUNT = ds.Tables(0).Rows(i)(0).ToString()
            Next
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function ASKROOMAVABLIEINTIME(DAYGUID As String, ROOMID As Integer)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ASKIFROOMAVALABEL", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@T1RoomId", ROOMID)
        comm.Parameters.AddWithValue("@DayGuid", DAYGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ASKIFROOMAVALABEL")
        If ds.Tables(0).Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function ASKROOMAVABLIEINTIME2(DAYGUID As String, ROOMID As Integer)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ASKIFROOMAVALABEL2", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@T2RoomId", ROOMID)
        comm.Parameters.AddWithValue("@DayGuid", DAYGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ASKIFROOMAVALABEL2")
        If ds.Tables(0).Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function ASKROOMAVABLIEINTIME3(DAYGUID As String, ROOMID As Integer)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ASKIFROOMAVALABEL3", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@T3RoomId", ROOMID)
        comm.Parameters.AddWithValue("@DayGuid", DAYGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ASKIFROOMAVALABEL3")
        If ds.Tables(0).Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function ASKROOMAVABLIEINTIME4(DAYGUID As String, ROOMID As Integer)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ASKIFROOMAVALABEL4", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@T4RoomId", ROOMID)
        comm.Parameters.AddWithValue("@DayGuid", DAYGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ASKIFROOMAVALABEL4")
        If ds.Tables(0).Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function ASKROOMAVABLIEINTIME5(DAYGUID As String, ROOMID As Integer)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ASKIFROOMAVALABEL5", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@T5RoomId", ROOMID)
        comm.Parameters.AddWithValue("@DayGuid", DAYGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ASKIFROOMAVALABEL5")
        If ds.Tables(0).Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function ASKSTUDENTDIPLOMINFO(STUDENTGUID As String)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ChkIfStudentRegDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StudentGuid", STUDENTGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ChkIfStudentRegDiplom")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DIPLOMNAME = ds.Tables(0).Rows(i)(0).ToString()
            Next
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function ASKLASTBILLINFO(STUDENTGUID As String)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("AskForLastBill", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@StudentGuid", STUDENTGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "AskForLastBill")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BILLDATE = ds.Tables(0).Rows(i)(0).ToString()
                BILLREF = ds.Tables(0).Rows(i)(1).ToString()
                CASHNAME = ds.Tables(0).Rows(i)(2).ToString()
                PAYMAN = ds.Tables(0).Rows(i)(3).ToString()
            Next
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function ASKDIPLOMSEAT(BRANCHGUD As String, BATCHGUID As String, DIPLOMGUID As String)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("AskForDiplomSeat", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BranchGuid", BRANCHGUD)
        comm.Parameters.AddWithValue("@BatchGuid", BATCHGUID)
        comm.Parameters.AddWithValue("@DiplomGuid", DIPLOMGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "AskForDiplomSeat")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                Dim DIPLOMSEAT As Double = Val(ds.Tables(0).Rows(i)(0)) - Val(ds.Tables(0).Rows(i)(1))
                If DIPLOMSEAT <= 0 Then
                    Return False
                Else
                    Return True
                End If
            Next
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function ASKFORAVALABILTRANIER(BRANCHGUID As String, DAYGUID As String, TIMEGUID As String, TRANIERGUID As String)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("AskForAvaibelData", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BranchGuid", BRANCHGUID)
        comm.Parameters.AddWithValue("@DayGuid", DAYGUID)
        comm.Parameters.AddWithValue("@TimeGuid", TIMEGUID)
        comm.Parameters.AddWithValue("@TranierGuid", TRANIERGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "AskForAvaibelData")
        If ds.Tables(0).Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
        Return True
    End Function
    Public Shared Function ChkIfFoundRegDocNotContract(SELLERGUID As String)
        Dim dbcontext As New SemouEntities
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ChkIfFoundRegDocNotContract", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@SellerGuid", SELLERGUID)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ChkIfFoundRegDocNotContract")
        If ds.Tables(0).Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
        Return True
    End Function
End Class
