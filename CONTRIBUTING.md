# Contributing

Thank you for contributing! These guidelines promote clarity, consistency, and maintainability.

## 1. Core Principles
- Clarity over cleverness.
- Consistency over personal style.
- Reduce duplication (prefer helpers/extensions).
- Use existing .NET / XrmToolBox APIs first.
- Improve shared helper libraries when needed.
- Code should explain itself; comment only when intent is non‑obvious.

## 2. Pull Request Checklist
Before opening a PR:
- [ ] Builds succeed.
- [ ] __Edit > Advanced > Format Document__ applied.
- [ ] No unused usings / debug remnants.
- [ ] No commented‑out dead code.
- [ ] Exception handling follows section 3.
- [ ] New public APIs have XML summaries.
- [ ] No compressed multi‑statement one‑liners.
- [ ] Repeated logic extracted to helpers.

## 3. Exception Handling
Always use braces for `try` / `catch`.

Bad (compressed, hard to scan):

```csharp
try { DoWork(); } catch (IOException ex) { LogWarn("Optional file load failed", ex); }
```

Good:

```csharp
try
{
    DoWork();
}
catch (IOException ex)
{
    LogWarn("Optional file load failed", ex);
}
```

Scope `try` blocks narrowly (only code that may throw).

Empty catch is allowed ONLY when:
1. Failure is truly non‑critical.
2. A comment explains why it is ignored.
3. Logging is added if it aids diagnosis.

Recommend:

A simple `try` with only one statement and empty `catch` (no statement and no comment) should be on one line:

```csharp
try { LoadOptionalMetadata(); } catch { }
```

Empty `catch` should be in one line:
```csharp
try
{
    DoWork();
    DoMoreStuff();
}
catch { }
```


Forbidden:
- `catch;` (invalid)
- Logic-heavy one-line: `try { if (x) A(); else B(); } catch { /* ignored */ }`
- Swallowing recoverable exceptions that users should know about

Prefer specific exceptions (e.g. `IOException`, `SqlException`) over broad `Exception` unless scope demands it.

## 4. Control Flow & Braces
Always use braces for `if`, `else`, `for`, `foreach`, `while`, `do`, `using`, `lock`, `switch` case blocks.

Bad:

```csharp
if (ready) DoThing(); 
if (condition) DoSomething(); else DoSomethingElse();
```

Good:

```csharp
if (ready)
{
    DoThing();
}

if (condition)
{
    DoSomething();
}
else
{
    DoSomethingElse();
}
```

No multiple statements on one line.

## 5. Formatting & Layout
- Auto-format before commit.
- Avoid excess blank lines; use them to separate logical sections only.
- Prefer early returns to reduce nesting.
- Extract helpers when a method exceeds ~30–40 lines or mixes concerns.
- Keep one public class per file (except small related types).
- Private helpers follow public API (group with regions only if it helps navigation).

## 6. Naming
- Methods: PascalCase verbs (`LoadEntities`, `GetState`).
- Booleans: clear condition/state (`IsLoaded`, `HasChanges`).
- Fields: `_camelCase` (private), avoid meaningless abbreviations.
- Common abbreviations ok (`Xml`, `Uri`, `CRM`).
- Extension classes named `<Domain>Extensions` or domain-specific helpers.

## 7. Logging
Use existing logging utilities (`LogUse`, `LogError`, `LogInfo`):
- LogError: unexpected failures.
- LogWarn (if available): recoverable fallbacks.
- LogInfo: significant operations (load, execute, export).
- Avoid logging per-item in large loops unless diagnostic mode.

## 8. Performance & Allocation
- Do not optimize prematurely.
- Use `StringBuilder` only for repeated concatenation in loops.
- LINQ is fine unless profiling shows a hotspot.
- Cache expensive lookups if repeated in tight loops.

## 9. Helpers & Extensions
Create a helper/extension when:
- Same logic appears 3+ times, OR
- A block obscures the caller’s intent.
Avoid micro‑helpers that wrap a single simple property/method without added meaning.

## 10. Null & Defensive Code
- Validate inputs at public boundaries.
- Use early returns instead of nested null checks.
- Avoid deep chains of null-conditionals—validate once.

## 11. Async / Background Work
- Marshal to UI with `InvokeRequired` guards.
- Keep UI updates minimal.
- Avoid capturing large objects in long‑lived closures.

## 12. File Organization
- Regions optional; avoid nesting deeply.
- Keep related private helper methods grouped.
- One logical responsibility per file.

## 13. Comments
Use comments for:
- Non-obvious intent
- Workarounds (reference issue or link)
Avoid:
- Restating obvious logic
- Historical notes (use Git history)

## 14. Deprecated Patterns
Do NOT:
- Use one-line `try/catch` with logic.
- Silently swallow exceptions (no comment/log).
- Rely on brittle `.Parent.Parent` chains (use ancestor search helpers).
- Overuse nested ternaries—prefer readable branching.
- Mix unrelated concerns in one method.

## 15. Example Patterns

Early return:
```csharp
public bool TryInitialize(Config config) 
{
    if (config == null)
    {
        return false;
    }
    if (!config.Enabled)
    { 
        return false;
    }
    InitializeCore(config);
    return true;
}
```

Guarded extension:
```csharp
public static IEnumerable<T> SafeSelect<T>(this IEnumerable<T> source, Func<T, bool> predicate)
{
    if (source == null) throw new ArgumentNullException(nameof(source));
    if (predicate == null) throw new ArgumentNullException(nameof(predicate));

    foreach (var item in source)
    {
        try
        {
            if (predicate(item)) yield return item;
        }
        catch (Exception ex)
        {
            LogError($"Error processing item in {nameof(SafeSelect)}: {ex.Message}", ex);
        }
    }
}

public static bool BringToolToFront(this PluginControlBase control, Control focus = null) { if (control == null) { return false; } // Logic ... return true; }
```

Safe invoke:
```csharp
public static TResult SafeInvoke<T, TResult>(this T obj, Func<T, TResult> func, TResult defaultValue = default)
{
    try
    {
        return obj != null ? func(obj) : defaultValue;
    }
    catch (Exception ex)
    {
        LogError($"Error in {nameof(SafeInvoke)}: {ex.Message}", ex);
        return defaultValue;
    }
}

public static void SafeInvoke(this Control control, Action action) { if (control.InvokeRequired) { control.Invoke(action); } else { action(); } }
```

Expression-bodied property:
```csharp
public class Person
{
    private string _firstName;
    private string _lastName;

    public Person(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
    }

    public string FullName => $"{_firstName} {_lastName}";
}

private readonly string _name; public string Name => _name ?? "<unknown>";
```

Numeric check extension:
```csharp
public static class NumericExtensions
{
    public static bool IsNumeric(this string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return false;
        foreach (char c in str)
        {
            if (!char.IsDigit(c)) return false;
        }
        return true;
    }
}

public static bool IsNumeric(this string value) => int.TryParse(value, out _);
```

Empty catch with rationale:
```csharp
try
{
    // Some optional initialization
}
catch
{
    // Ignored: optional feature not critical to operation
}

try { PreloadCache(); } catch { // Ignored: cache warm-up is optional }
```

## 16. Updating These Guidelines
Propose changes via PR with reasoning (e.g. new async pattern, analyzer adoption). Keep this concise and enforceable.

---
Consistent adherence improves readability and lowers maintenance cost. Thank you!

