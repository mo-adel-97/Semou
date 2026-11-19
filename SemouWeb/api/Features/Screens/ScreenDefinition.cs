namespace Semou.Api.Features.Screens;

/*
 * Metadata-driven screens.
 * Most desktop "Encoding*" forms follow one pattern: an Add proc, an Update proc, a table, a
 * Code/Guid pair, VALIDATE_DATA checks, sometimes a branch sub-grid. Each one is described here
 * once, and ScreenService + the React CrudScreen run it with the same rules as the desktop:
 * permissions, numbering, validation messages, audit log, and the edit reason.
 *
 * All SQL identifiers (tables, columns, procs) come only from these definitions, never from the request.
 */

public enum FieldType { Text, TextArea, Int, Decimal, Bool, Select, Combo, Lookup }

/// <summary>How a new record gets its number.</summary>
public enum IdMode
{
    /// <summary>The proc takes @ID/@Id and @Code; the engine sends MAX(ID)+1 (per scope if any), like GET___NO.</summary>
    MaxPlusOne,
    /// <summary>The proc computes the number itself and returns it in @CodeOut.</summary>
    ProcOutput,
}

public sealed record FieldDef(string Name, string Label, FieldType Type)
{
    /// <summary>Message from the desktop VALIDATE_DATA. Null = optional.</summary>
    public string? Required { get; init; }
    /// <summary>Numbers: must be greater than zero (desktop "Text = 0" checks). Uses the Required message.</summary>
    public bool NonZero { get; init; }
    public int? MaxLength { get; init; }
    /// <summary>Select: the desktop ComboBox items; the stored value is the item index (SelectedIndex).</summary>
    public string[]? Options { get; init; }
    /// <summary>Combo: free text with suggestions (the stored value is the text).</summary>
    public string[]? Suggestions { get; init; }
    /// <summary>Lookup source name served by /api/lookups (e.g. "branches").</summary>
    public string? Lookup { get; init; }
    public string? Section { get; init; }
    /// <summary>1 = half width, 2 = full width.</summary>
    public int Span { get; init; } = 1;
    public string? Suffix { get; init; }
    public string? Hint { get; init; }
    /// <summary>Can't change after the first save (the update proc doesn't take it).</summary>
    public bool LockOnEdit { get; init; }
    public bool Ltr { get; init; }
    public object? Default { get; init; }
}

public enum ColumnKind { Text, Code, Status, Option, Number, Lookup }

public sealed record ColumnDef(string Name, string Label, ColumnKind Kind = ColumnKind.Text)
{
    /// <summary>For Option columns: the field whose Options translate the index.</summary>
    public string? OptionsFrom { get; init; }
    /// <summary>For Lookup columns: SQL expression producing the display text (joined in the list query).</summary>
    public string? Sql { get; init; }
}

public enum ChildKind
{
    /// <summary>A set of branches (e.g. SalesManBranch).</summary>
    BranchSet,
    /// <summary>A number per branch (e.g. BatchStudentCount).</summary>
    BranchNumbers,
}

/// <summary>A sub-grid saved as "delete all, then add each row", the same as the desktop.</summary>
public sealed record ChildDef(string Name, string Label, ChildKind Kind)
{
    public required string Table { get; init; }
    /// <summary>Column/param holding the parent Guid, e.g. SalesManGuid.</summary>
    public required string ParentParam { get; init; }
    public required string DeleteProc { get; init; }
    public required string AddProc { get; init; }
    /// <summary>For BranchNumbers: the number column/param, e.g. StudentCount.</summary>
    public string? ValueParam { get; init; }
    public string? ValueLabel { get; init; }
    /// <summary>Desktop message when the list must not be empty.</summary>
    public string? Required { get; init; }
}

public sealed record ScreenDef
{
    public required string Key { get; init; }
    /// <summary>Form_Name.UnqName used for permissions and the audit log.</summary>
    public required string Form { get; init; }
    public required string Title { get; init; }
    /// <summary>"مدرب" → "مدرب جديد", "بطاقة مدرب".</summary>
    public required string Singular { get; init; }
    public string? Subtitle { get; init; }
    public required string Table { get; init; }
    /// <summary>Main display column (shown as the drawer title).</summary>
    public required string NameField { get; init; }
    public required string AddProc { get; init; }
    public required string UpdateProc { get; init; }
    public IdMode IdMode { get; init; } = IdMode.MaxPlusOne;
    /// <summary>Numbering is per value of this field (e.g. BranchGuid for rooms/trainers). Also offered as a list filter.</summary>
    public string? ScopeField { get; init; }
    public string IsUseColumn { get; init; } = "IsUse";
    public required FieldDef[] Fields { get; init; }
    public required ColumnDef[] Columns { get; init; }
    public ChildDef[] Children { get; init; } = [];
}
