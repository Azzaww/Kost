# Localization Support Guide

## Current State
The Kost application has partial bilingual support (Indonesian/English) implemented in several components:

### Areas with Bilingual Support ✅
- **AddKamar.cs** - Room status dropdown: "Tersedia / Available", "Penuh / Full", "Perbaikan / Maintenance"
- **EditKamar.cs** - Same bilingual status support
- **DataKamar.cs** - Status filters with bilingual support
- **ApiClient.cs** - Helper methods for bilingual status display:
  - `GetBilingualStatus()` - Payment status
  - `GetBilingualCategory()` - Payment category  
  - `GetBilingualMethod()` - Payment method

### Areas WITHOUT Bilingual Support ❌
- **Form1.cs** - Login form (hardcoded Indonesian/English messages)
- **BerandaPage.cs** - Dashboard labels and messages
- **DataPenyewa.cs** - Tenant management UI
- **PembayaranForm.cs** - Payment form
- **GalleryForm.cs** - Gallery management
- All error messages and validation messages

---

## Recommended Approach for Full Localization

### Option 1: .NET Resource Files (ResX) - RECOMMENDED
**Best for: WinForms, structured approach, compiler-safe**

#### Steps:
1. Create `Resources` folder in project
2. Create `Strings.resx` (default/English)
3. Create `Strings.id.resx` (Indonesian)
4. Add string resources to both files

Example structure:
```
Strings.resx
├─ LoginTitle = "Login"
├─ LoginFailure = "Login Failed"
└─ PasswordRequired = "Password is required"

Strings.id.resx
├─ LoginTitle = "Masuk"
├─ LoginFailure = "Login Gagal"
└─ PasswordRequired = "Password wajib diisi"
```

#### Usage in Code:
```csharp
using Kost_SiguraGura.Properties;

// Automatic language detection
string message = Resources.LoginFailure; // Gets language based on CultureInfo
```

#### Advantages:
- ✅ Compile-time checking
- ✅ Visual Studio ResX editor
- ✅ Automatic fallback to default language
- ✅ Supports all .NET languages

---

### Option 2: JSON Configuration Files
**Best for: Web-like apps, easy editing, version control friendly**

#### Structure:
```
Resources/
├─ strings.en.json
└─ strings.id.json
```

Example `strings.en.json`:
```json
{
  "Login": {
	"Title": "Login",
	"Username": "Username",
	"Password": "Password",
	"InvalidCredentials": "Invalid username or password"
  },
  "Rooms": {
	"Available": "Available",
	"Full": "Full",
	"Maintenance": "Maintenance"
  }
}
```

#### Usage:
```csharp
var strings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(
	File.ReadAllText($"Resources/strings.{currentCulture}.json")
);

string title = strings["Login"]["Title"];
```

---

### Option 3: Database-Driven
**Best for: Enterprise apps, frequent translation updates**

Store all strings in database with language_id foreign key.

---

## Implementation Priority

### Immediate (Recommended for Phase 1):
1. Wrap all user-facing strings in centralized resource access
2. Keep hardcoded strings for now
3. Document string keys for future localization

### Phase 2:
1. Create ResX files for most common forms
2. Implement language selector in settings
3. Make UI responsive to language changes

### Phase 3:
1. Complete localization of all forms
2. Add language preference persistence
3. Support RTL languages if needed

---

## Files Needing Localization

**High Priority:**
- [ ] Form1.cs (Login) - 15+ strings
- [ ] BerandaPage.cs (Dashboard) - 10+ strings
- [ ] DataPenyewa.cs (Tenant management) - 12+ strings
- [ ] AddKamar.cs/EditKamar.cs (Room management) - 8+ strings

**Medium Priority:**
- [ ] PembayaranForm.cs - 8+ strings
- [ ] DataKamar.cs - 6+ strings
- [ ] GalleryForm.cs - 5+ strings

**Low Priority:**
- [ ] Sidebar.cs - 4+ strings
- [ ] Report.cs - 3+ strings

---

## String Categories to Extract

1. **UI Labels:** Button text, field labels, titles
2. **Error Messages:** Validation errors, HTTP errors, exceptions
3. **Success Messages:** Operation completed messages
4. **Status Values:** Room status, payment status, user roles
5. **Tooltips & Help Text:** Placeholder text, hints

---

## Example: Converting One Form to Localized

### Before (Hardcoded):
```csharp
MessageBox.Show("Gagal mengambil data tenant: " + ex.Message);
```

### After (Localized):
```csharp
MessageBox.Show(Resources.TenantLoadFailed + ex.Message);
// Or with JSON:
string message = strings["Messages"]["TenantLoadFailed"];
MessageBox.Show(message + ex.Message);
```

---

## Testing Localization

```csharp
// Test switching languages at runtime
System.Globalization.CultureInfo.CurrentUICulture = 
	new System.Globalization.CultureInfo("id-ID"); // Indonesian

System.Globalization.CultureInfo.CurrentUICulture = 
	new System.Globalization.CultureInfo("en-US"); // English
```

---

## Next Steps

1. **Decide approach:** ResX vs JSON vs Database
2. **Create resource files** for chosen approach
3. **Implement ResourceManager** wrapper for centralized access
4. **Convert Form1.cs** as pilot (high-impact, small scope)
5. **Test** language switching
6. **Roll out** to other forms incrementally

---

## References

- [Microsoft: Creating Resource Files](https://learn.microsoft.com/en-us/dotnet/framework/resources/creating-resource-files-for-desktop-apps)
- [CultureInfo Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo)
- [WinForms Localization Best Practices](https://learn.microsoft.com/en-us/dotnet/framework/winforms/localization-in-windows-forms)
