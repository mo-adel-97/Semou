namespace Semou.Api.Features.Screens;

/// <summary>
/// Every metadata-driven screen, ported from the desktop forms.
/// Fields are named exactly like the proc parameters / table columns; messages are copied from VALIDATE_DATA.
/// </summary>
public static class ScreenRegistry
{
    // Desktop ComboBox items (the stored value is the item index).
    private static readonly string[] Departs = ["التقنية الادارية", "القانون", "الحاسب", "إدارة السلامة", "بدون"];
    private static readonly string[] BookKinds = ["مواد تخصص", "مواد عامة", "مواد عامة تخصصية"];

    private static FieldDef IsUse(string label = "فعال") => new("IsUse", label, FieldType.Bool) { Default = true, Section = "الحالة" };
    private static FieldDef Notes(string name = "Notes", string label = "ملاحظات", int max = 250) =>
        new(name, label, FieldType.TextArea) { MaxLength = max, Span = 2 };
    private static ColumnDef Code => new("Code", "الكود", ColumnKind.Code);
    private static ColumnDef Status => new("IsUse", "الحالة", ColumnKind.Status);

    public static readonly ScreenDef[] All =
    [
        // ---------------- ملف ----------------
        new()
        {
            Key = "trainers", Form = "addtrainer", Title = "المدربين", Singular = "مدرب", Subtitle = "بيانات المدربين وساعات العمل والأقسام التدريبية",
            Table = "Trainer", NameField = "Name", AddProc = "AddTrainer", UpdateProc = "UpdateTrainer",
            IdMode = IdMode.ProcOutput, ScopeField = "BranchGuid",
            Fields =
            [
                new("Name", "اسم المدرب", FieldType.Text) { Required = "يرجى إدخال اسم المدرب", MaxLength = 150, Span = 2, Section = "البيانات الأساسية" },
                new("NationalId", "رقم الهوية", FieldType.Text) { Required = "يرجى إدخال رقم الهوية", MaxLength = 50, Ltr = true },
                new("NationalName", "الجنسية", FieldType.Combo) { Suggestions = ["سعودي", "مصري", "سوداني"], MaxLength = 50 },
                new("BranchGuid", "الفرع", FieldType.Lookup) { Lookup = "branches", Required = "يرجى اختيار الفرع التابع له المدرب أولاً", Span = 2 },
                new("Mob", "الجوال", FieldType.Text) { Required = "يرجى إدخال رقم الجوال", MaxLength = 50, Ltr = true, Section = "التواصل" },
                new("Tel", "الهاتف", FieldType.Text) { MaxLength = 50, Ltr = true },
                new("Email", "البريد الإلكتروني", FieldType.Text) { MaxLength = 70, Ltr = true },
                new("Address", "العنوان", FieldType.Text) { MaxLength = 150 },
                new("Eduction", "المؤهل العلمي", FieldType.Text) { MaxLength = 200, Span = 2, Section = "المؤهلات والتدريب" },
                new("OtherEduction", "مؤهلات أخرى", FieldType.Text) { MaxLength = 200, Span = 2 },
                new("TranierDepart", "القسم التدريبي", FieldType.Select) { Options = Departs, Required = "يرجى تحديد القسم التدريبي التابع له المدرب" },
                new("BookType", "المقررات", FieldType.Select) { Options = BookKinds, Required = "يرجى اختيار نوع المقررات التي يدرسها المدرب" },
                new("DailyTime", "ساعات العمل اليومية", FieldType.Decimal) { Required = "يرجى إدخال ساعات العمل اليومية", NonZero = true, Suffix = "ساعة" },
                new("WeekTime", "ساعات العمل الأسبوعية", FieldType.Decimal) { Required = "يرجى إدخال ساعات العمل الأسبوعية", NonZero = true, Suffix = "ساعة" },
                new("UseMainAllow", "يسمح للمدرب بتدريب أي مقرر من نوع مواد عامة", FieldType.Bool) { Default = false, Span = 2 },
                IsUse(),
            ],
            Columns = [Code, new("Name", "اسم المدرب"), new("Mob", "الجوال"), new("TranierDepart", "القسم", ColumnKind.Option) { OptionsFrom = "TranierDepart" },
                       new("BranchName", "الفرع", ColumnKind.Lookup) { Sql = "(SELECT BrName FROM dbo.Branch WHERE Branch.Guid = T.BranchGuid)" }, Status],
        },
        new()
        {
            Key = "salesmen", Form = "addsalesman", Title = "مناديب البيع", Singular = "مندوب", Subtitle = "المناديب والفروع المسموح لهم التسجيل فيها",
            Table = "SalesMan", NameField = "ManFullName", AddProc = "AddSalesMan", UpdateProc = "UpdateSalesMan",
            Fields =
            [
                new("ManFullName", "اسم المندوب", FieldType.Text) { Required = "يرجى إدخال اسم المندوب", MaxLength = 150, Span = 2, Section = "البيانات الأساسية" },
                new("ManUseName", "اسم مختصر", FieldType.Text) { Required = "يرجى إدخال الاسم المختصر للمندوب", MaxLength = 50 },
                IsUse(),
            ],
            Columns = [Code, new("ManFullName", "اسم المندوب"), new("ManUseName", "الاسم المختصر"), Status],
            Children =
            [
                new("branches", "الفروع المتاح التعامل معها", ChildKind.BranchSet)
                {
                    Table = "SalesManBranch", ParentParam = "SalesManGuid", DeleteProc = "DeleteSalesManBranch", AddProc = "AddSalesManBranch",
                    Required = "يرجى اختيار الفروع المسموح للمندوب التسجيل فيها",
                },
            ],
        },
        new()
        {
            Key = "discount-types", Form = "distype", Title = "أنواع الخصم", Singular = "نوع خصم",
            Table = "MainDis", NameField = "DisName", AddProc = "AddMainDis", UpdateProc = "UpdateMainDis", IdMode = IdMode.ProcOutput,
            Fields =
            [
                new("DisName", "اسم الخصم", FieldType.Text) { Required = "يرجى إدخال اسم الخصم", MaxLength = 150, Span = 2, Section = "البيانات الأساسية" },
                new("DisValue", "نسبة الخصم", FieldType.Decimal) { Required = "يرجى تحديد نسبة / قيمة الخصم أولاً", Suffix = "%" },
                new("ChkAttach", "يجب إدخال مرفقات", FieldType.Bool) { Default = false },
                Notes("DisNotes", max: 150),
                IsUse(),
            ],
            Columns = [Code, new("DisName", "اسم الخصم"), new("DisValue", "النسبة %", ColumnKind.Number), Status],
        },
        new()
        {
            Key = "sectors", Form = "sector", Title = "القطاعات", Singular = "قطاع", Subtitle = "نصوص الموافقة والمسئولين لكل قطاع",
            Table = "Sector", NameField = "CompanyName", AddProc = "AddSector", UpdateProc = "UpdateSector", IdMode = IdMode.ProcOutput,
            Fields =
            [
                new("CompanyName", "اسم القطاع", FieldType.Text) { Required = "يرجى إدخال اسم القطاع", MaxLength = 50, Section = "البيانات الأساسية" },
                new("CompanyMan", "عناية (اسم المسئول)", FieldType.Text) { Required = "يرجى إدخال اسم المسئول", MaxLength = 50 },
                new("CompanyText", "نص الموافقة", FieldType.TextArea) { Required = "يرجى إدخال نص الموافقة", Span = 2 },
                new("ChangeDate", "اعتماد التاريخ الهجري", FieldType.Bool) { Default = false },
                IsUse(),
            ],
            Columns = [Code, new("CompanyName", "اسم القطاع"), new("CompanyMan", "المسئول"), Status],
        },
        new()
        {
            Key = "services", Form = "adddiplom", Title = "الخدمات", Singular = "خدمة", Subtitle = "الدبلومات والدورات والرسوم والضرائب",
            Table = "Diplom", NameField = "DiplomName", AddProc = "AddDiplom", UpdateProc = "UpdateDiplom",
            Fields =
            [
                new("DiplomName", "اسم الخدمة", FieldType.Text) { Required = "يرجى إدخال اسم الدبلوم - الدورة", MaxLength = 150, Span = 2, Section = "البيانات الأساسية" },
                new("DiplomType", "نوع الخدمة", FieldType.Select) { Options = ["دراسة", "رسوم دراسية", "رسوم إدارية"], Default = 0 },
                new("StudyType", "نوع الدراسة", FieldType.Select) { Options = ["دبلوم", "دورة"], Default = 0 },
                new("Depart", "القسم التدريبي", FieldType.Select) { Options = Departs, Default = 4 },
                new("DiplomUnit", "وحدة الخدمة", FieldType.Text) { MaxLength = 50, Default = "Pcs" },
                new("CountNum", "مدة الخدمة", FieldType.Int) { Required = "يرجى إدخال مدة الدبلوم - الدورة", Section = "المدة" },
                new("CountText", "وصف المدة", FieldType.Combo) { Suggestions = ["شهر", "سنة", "بدون"], Required = "يرجى إدخال وصف مدة الدبلوم / الدورة", MaxLength = 50 },
                new("TaxType", "نوع الضريبة", FieldType.Select) { Options = ["ضريبة القيمة المضافة", "الضريبة الصفرية", "الضريبة المعفاة"], Required = "يرجى اختيار نوع الضريبة", Section = "الضريبة" },
                new("TaxValue", "نسبة الضريبة", FieldType.Decimal) { Suffix = "%" },
                Notes("DiplomDesc", "وصف الخدمة", int.MaxValue),
                IsUse(),
            ],
            Columns = [Code, new("DiplomName", "اسم الخدمة"), new("StudyType", "النوع", ColumnKind.Option) { OptionsFrom = "StudyType" },
                       new("CountNum", "المدة", ColumnKind.Number), new("TaxValue", "الضريبة %", ColumnKind.Number), Status],
        },

        // ---------------- شئون الطلاب ----------------
        new()
        {
            Key = "batches", Form = "addbatch", Title = "الدفعات", Singular = "دفعة", Subtitle = "الدفعات الدراسية وعدد الطلاب المتاح في كل فرع",
            Table = "Batch", NameField = "BatchName", AddProc = "AddBatch", UpdateProc = "UpdateBatch",
            Fields =
            [
                new("BatchName", "اسم الدفعة", FieldType.Text) { Required = "يرجى إدخال اسم الدفعة", MaxLength = 150, Span = 2, Section = "البيانات الأساسية" },
                Notes(),
                IsUse(),
            ],
            Columns = [Code, new("BatchName", "اسم الدفعة"), new("Notes", "ملاحظات"), Status],
            Children =
            [
                new("counts", "عدد الطلاب المتاح تسجيلهم في كل فرع", ChildKind.BranchNumbers)
                {
                    Table = "BatchStudentCount", ParentParam = "BatchGuid", DeleteProc = "DeleteBatchStudentCount", AddProc = "AddBatchStudentCount",
                    ValueParam = "StudentCount", ValueLabel = "عدد الطلاب",
                },
            ],
        },
        new()
        {
            Key = "levels", Form = "addlevel", Title = "المستويات", Singular = "مستوى", Table = "Levels", NameField = "LevelName",
            AddProc = "AddLevel", UpdateProc = "UpdateLevel",
            Fields = [new("LevelName", "اسم المستوى", FieldType.Text) { Required = "يرجى إدخال اسم المستوى", MaxLength = 150, Span = 2, Section = "البيانات الأساسية" }, Notes(), IsUse()],
            Columns = [Code, new("LevelName", "اسم المستوى"), new("Notes", "ملاحظات"), Status],
        },
        new()
        {
            Key = "reg-cases", Form = "addstaut", Title = "حالات التسجيل", Singular = "حالة", Table = "Stauts", NameField = "StautName",
            AddProc = "AddStaut", UpdateProc = "UpdateStaut",
            Fields = [new("StautName", "اسم الحالة", FieldType.Text) { Required = "يرجى إدخال اسم الحالة", MaxLength = 150, Span = 2, Section = "البيانات الأساسية" }, Notes(), IsUse()],
            Columns = [Code, new("StautName", "اسم الحالة"), new("Notes", "ملاحظات"), Status],
        },
        new()
        {
            Key = "other-fees", Form = "addfees", Title = "الرسوم الأخرى", Singular = "رسوم", Table = "OtherFees", NameField = "FessName",
            AddProc = "AddOtherFees", UpdateProc = "UpdateOtherFees",
            Fields = [new("FessName", "اسم الرسوم", FieldType.Text) { Required = "يرجى إدخال اسم الرسوم", MaxLength = 150, Span = 2, Section = "البيانات الأساسية" }, IsUse()],
            Columns = [Code, new("FessName", "اسم الرسوم"), Status],
        },

        // ---------------- إدارة الفرع ----------------
        new()
        {
            Key = "classrooms", Form = "classroom", Title = "القاعات الدراسية", Singular = "قاعة", Subtitle = "قاعات كل فرع ومساحتها وعدد المقاعد",
            Table = "RoomClass", NameField = "RoomName", AddProc = "AddRoomClass", UpdateProc = "UpdateRoomClass", ScopeField = "BranchGuid",
            Fields =
            [
                new("BranchGuid", "الفرع", FieldType.Lookup) { Lookup = "branches", Required = "يرجى اختيار الفرع أولاً", LockOnEdit = true, Span = 2, Section = "البيانات الأساسية" },
                new("RoomName", "اسم القاعة", FieldType.Text) { Required = "يرجى إدخال اسم القاعة", MaxLength = 50 },
                new("RoomType", "نوع القاعة", FieldType.Select) { Options = ["قاعة نظري", "مختبر"], Required = "يرجى تحديد نوع القاعة" },
                new("RoomH", "الطول", FieldType.Decimal) { Required = "يرجى إدخال طول القاعة", NonZero = true, Suffix = "م", Section = "المساحة" },
                new("RoomW", "العرض", FieldType.Decimal) { Required = "يرجى إدخال عرض القاعة", NonZero = true, Suffix = "م" },
                new("RoomCount", "عدد المقاعد", FieldType.Int) { Suffix = "مقعد" },
                Notes("RoomNotes", "الوصف", int.MaxValue),
                IsUse(),
            ],
            Columns = [Code, new("RoomName", "اسم القاعة"), new("RoomType", "النوع", ColumnKind.Option) { OptionsFrom = "RoomType" },
                       new("RoomCount", "المقاعد", ColumnKind.Number),
                       new("BranchName", "الفرع", ColumnKind.Lookup) { Sql = "(SELECT BrName FROM dbo.Branch WHERE Branch.Guid = T.BranchGuid)" }, Status],
        },
        new()
        {
            Key = "books", Form = "addbook", Title = "المقررات الدراسية", Singular = "مقرر", Table = "Book", NameField = "BookName",
            AddProc = "AddBook", UpdateProc = "UpdateBook",
            Fields =
            [
                new("BookName", "اسم المقرر", FieldType.Text) { Required = "يرجى إدخال اسم المقرر الدراسي", MaxLength = 150, Span = 2, Section = "البيانات الأساسية" },
                new("BookCode", "رمز المقرر", FieldType.Text) { Required = "يرجى إدخال رمز المقرر الدراسي", MaxLength = 50, Ltr = true },
                new("BookType", "طبيعة المقرر", FieldType.Select) { Options = ["نظري", "عملي", "نظري - عملي"], Required = "يرجى تحديد طبيعة المقرر الدراسي" },
                new("BookAdvnced", "نوع المقرر", FieldType.Select) { Options = BookKinds, Required = "يرجى تحديد نوع المقرر الدراسي" },
                IsUse(),
            ],
            Columns = [Code, new("BookCode", "الرمز"), new("BookName", "اسم المقرر"), new("BookType", "الطبيعة", ColumnKind.Option) { OptionsFrom = "BookType" },
                       new("BookAdvnced", "النوع", ColumnKind.Option) { OptionsFrom = "BookAdvnced" }, Status],
        },
    ];

    private static readonly Dictionary<string, ScreenDef> ByKey = All.ToDictionary(s => s.Key, StringComparer.OrdinalIgnoreCase);
    public static ScreenDef? Find(string key) => ByKey.GetValueOrDefault(key);
}
