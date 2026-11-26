namespace Semou.Api.Security;

/// <summary>
/// Screens that exist only on the web. Their permissions live in dbo.WebFormPermission (not User_Premision,
/// which the desktop user screen wipes and rewrites on every save). The meaning of each right is listed so the
/// users screen can explain it.
/// </summary>
public static class WebForms
{
    public const string RegOrders = "webregorder";
    public const string PayOrders = "webpayorder";
    public const string Settings = "websettings";
    public const string StudyRequests = "webstudyrequest";
    public const string MyOrders = "webmyorders";
    public const string JobTitles = "webjobtitle";
    public const string EmploymentTypes = "webemptype";
    public const string Specializations = "webspecial";
    public const string Banks = "webbank";

    public sealed record Def(string Key, string Name, string Group, IReadOnlyDictionary<string, string> Rights);

    public static readonly IReadOnlyList<Def> All =
    [
        new(RegOrders, "طلبات الالتحاق", "المبيعات", new Dictionary<string, string>
        {
            ["view"] = "فتح الشاشة ومراجعة الطلبات",
            ["add"] = "قبول الطلب وإنشاء استمارة التسجيل",
            ["delete"] = "رفض الطلب",
            ["print"] = "طباعة الاستمارة والفاتورة بعد القبول",
        }),
        new(PayOrders, "طلبات السداد", "الحسابات العامة", new Dictionary<string, string>
        {
            ["view"] = "فتح الشاشة ومراجعة الطلبات",
            ["add"] = "تأكيد الطلب وإصدار الفاتورة الضريبية وسند القبض",
            ["edit"] = "تصحيح بيانات الحوالة قبل التأكيد",
            ["delete"] = "إلغاء الطلب أو تعليمه مكرراً",
            ["print"] = "طباعة الفاتورة بعد التأكيد",
        }),
        new(MyOrders, "طلباتي", "ملف", new Dictionary<string, string>
        {
            ["view"] = "فتح الشاشة ومتابعة طلبات الالتحاق والسداد التي أرسلها المستخدم وحالة كل طلب",
        }),
        new(StudyRequests, "طلبات الدراسة (الموقع)", "المبيعات", new Dictionary<string, string>
        {
            ["view"] = "فتح الشاشة ومتابعة طلبات الدراسة الواردة من الموقع (حضوري / عن بعد)",
            ["edit"] = "تغيير حالة الطلب وكتابة ملاحظات المتابعة",
        }),
        new(JobTitles, "المسميات الوظيفية", "ملف", new Dictionary<string, string>
        {
            ["view"] = "فتح الشاشة",
            ["find"] = "استعراض المسميات الوظيفية",
            ["add"] = "إضافة مسمى وظيفي جديد (من الشاشة أو من بطاقة المستخدم)",
            ["edit"] = "تعديل الاسم أو إيقاف العنصر",
        }),
        new(EmploymentTypes, "الحالات الوظيفية", "ملف", new Dictionary<string, string>
        {
            ["view"] = "فتح الشاشة",
            ["find"] = "استعراض الحالات الوظيفية",
            ["add"] = "إضافة حالة وظيفية جديدة (من الشاشة أو من بطاقة المستخدم)",
            ["edit"] = "تعديل الاسم أو إيقاف العنصر",
        }),
        new(Specializations, "التخصصات والمؤهلات", "ملف", new Dictionary<string, string>
        {
            ["view"] = "فتح الشاشة",
            ["find"] = "استعراض التخصصات",
            ["add"] = "إضافة تخصص جديد (من الشاشة أو من بطاقة المستخدم)",
            ["edit"] = "تعديل الاسم أو إيقاف العنصر",
        }),
        new(Banks, "البنوك", "ملف", new Dictionary<string, string>
        {
            ["view"] = "فتح الشاشة",
            ["find"] = "استعراض البنوك",
            ["add"] = "إضافة بنك جديد (من الشاشة أو من بطاقة المستخدم)",
            ["edit"] = "تعديل الاسم أو إيقاف العنصر",
        }),
        new(Settings, "الإعدادات العامة", "ملف", new Dictionary<string, string>
        {
            ["view"] = "فتح الشاشة والاطلاع على الإعدادات وسجل التعديلات",
            ["edit"] = "تعديل هيكلة القائمة الجانبية والنصوص وحفظها واسترجاع النسخ السابقة",
        }),
    ];

    /// <summary>A fixed id per web screen so the users screen can treat it like a Form_Name row.</summary>
    public static Guid GuidOf(string key) =>
        new(System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes("semou-web-form:" + key.ToLowerInvariant())));

    public static Def? ByGuid(Guid g) => All.FirstOrDefault(f => GuidOf(f.Key) == g);

    public static bool IsWebForm(string key) => All.Any(f => string.Equals(f.Key, key, StringComparison.OrdinalIgnoreCase));
}
