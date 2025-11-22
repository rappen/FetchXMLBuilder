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

```
try { DoWork(); } catch (IOException ex) { LogWarn("Optional file load failed", ex); }
```

Good:

```
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

Recommend (single harmless statement):

```
try { LoadOptionalMetadata(); } catch { }
```

Empty `catch` guarding a multi‑statement block should be one line:

```
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
- Swallowing recoverable exceptions users should know about

Prefer specific exceptions (e.g. `IOException`, `SqlException`) over broad `Exception` unless scope demands it.

## 4. Control Flow & Braces
Always use braces for `if`, `else`, `for`, `foreach`, `while`, `do`, `using`, `lock`, `switch` case blocks.

Bad:

```
if (ready) DoThing();
if (condition) DoSomething(); else DoSomethingElse();
```

Good:

```
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

But for 'if invoke needed' statements, it is totally okay with a one-line:
```
MethodInvoker mi = () => { fxb.dockControlBuilder.Init(fetch, null, false, "Query from AI", true); };
if (InvokeRequired) Invoke(mi); else mi();
```

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
- Abuse nested ternaries—avoid more than one level.
- Mix unrelated concerns in one method.

## 15. Ternary (Conditional) Operator Usage
Ternaries are allowed and encouraged when they:
- Are a single, simple condition yielding clear alternative values.
- Improve readability compared to a short `if/else`.
- Have simple expressions on both sides (values or non-side-effect calls).
- Are not nested more than once.

Good:

```
var status   = isActive ? "Active" : "Inactive";
var logLevel = isDebug ? LogLevel.Debug : LogLevel.Info;
var result   = flag ? 42 : 0;
var message  = isError ? "Error" : "Success";
var display  = name != null ? name : "<unknown>";
var label    = customLabel ?? (isDefault ? "Default" : "Custom");
```

Avoid (rewrite with if/switch):

```
var value = a ? b ? c : d : e;                      // nested ternary
var role  = isA ? "A" : isB ? "B" : isC ? "C" : "X"; // long chain
var y     = isDebug ? LogDebug(message) : LogInfo(message); // side effects
```

Prefer conditional expression for return/assignment when both arms are simple values or pure calls.
Do not use ternaries for side-effect heavy logic—use `if/else`.

## 16. Example Patterns

Early return:

```
public bool Save()
{
    if (!Validate())
    {
        return false;
    }

    // Save logic...
    return true;
}
```

Guarded extension:

```
public static class EntityExtensions
{
    public static void SetName(this Entity entity, string name)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (name == null) throw new ArgumentNullException(nameof(name));
        entity["name"] = name;
    }
}
```

Safe invocation with logging:

```
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
```

UI invocation:

```
public static void SafeInvoke(this Control control, Action action)
{
    if (control.InvokeRequired)
    {
        control.Invoke(action);
    }
    else
    {
        action();
    }
}
```

Expression-bodied property:

```
public class Person
{
    private string _firstName;
    private string _lastName;

    public Person(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName  = lastName;
    }

    public string FullName => $"{_firstName} {_lastName}";
}

private readonly string _name;
public string Name => _name ?? "<unknown>";
```

Numeric check (manual and TryParse):

```
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

    public static bool IsNumericFast(this string value) => int.TryParse(value, out _);
}
```

Empty catch with rationale:

```
try
{
    // Optional initialization
}
catch
{
    // Ignored: optional feature not critical to operation
}

try { PreloadCache(); } catch { /* Ignored: cache warm-up is optional */ }
```

## 17. Updating These Guidelines
Propose changes via PR including:
- The rationale (e.g. new async pattern, analyzer adoption, readability improvement).
- Any required .editorconfig amendments.
Keep additions concise and enforceable. Large stylistic shifts should be discussed first in an issue.

---
Consistent adherence improves readability and lowers maintenance cost. Thank you!

