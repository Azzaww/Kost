# 📂 REPOSITORY STRUCTURE & FILE ORGANIZATION

## Project Overview
**Kost Application** - Desktop application untuk manajemen Kost (boarding house) menggunakan C# Windows Forms dan .NET Framework 4.8.

---

## 📁 Main Directory Structure

```
Kost/
├── .github/                          # GitHub workflows and templates
│   ├── workflows/
│   │   ├── ci.yml                   # CI/CD pipeline
│   │   └── release.yml              # Release workflow
│   └── copilot-instructions.md      # Copilot guidelines (bilingual support)
│
├── .gitignore                        # Git ignore rules ✅ UPDATED
├── Kost_SiguraGura/                 # Main C# Project
│   ├── Properties/
│   │   ├── AssemblyInfo.cs
│   │   └── Resources.Designer.cs
│   │
│   ├── MODELS - Data Models         # Data model classes
│   │   ├── Kamar.cs                 # Room/Kamar model
│   │   ├── Penyewa.cs               # Tenant model + Response models
│   │   ├── PaymentResponse.cs       # Payment & Booking models
│   │   └── LoginRequest.cs          # Login request model
│   │
│   ├── FORMS - UI Forms             # Windows Forms
│   │   ├── Form1.cs                 # Login form
│   │   ├── Sidebar.cs               # Main navigation sidebar
│   │   ├── BerandaPage.cs           # Dashboard
│   │   ├── DataKamar.cs             # Room management
│   │   ├── DataPenyewa.cs           # Tenant management
│   │   ├── AddKamar.cs              # Add new room
│   │   ├── EditKamar.cs             # Edit room
│   │   ├── GalleryForm.cs           # Gallery management
│   │   ├── AddGallery.cs            # Add gallery images
│   │   ├── PembayaranForm.cs        # Payment form
│   │   ├── PembayaranDetail.cs      # Payment details
│   │   ├── Report.cs                # Reports page
│   │   ├── TenantDetailForm.cs      # Tenant details
│   │   └── PenyewaDetail.cs         # Penyewa details page
│   │
│   ├── SERVICES & UTILITIES         # Business logic & helpers
│   │   ├── ApiClient.cs             # HTTP API client (centralized)
│   │   ├── Session.cs               # Session management (thread-safe)
│   │   ├── DataSyncManager.cs       # Data synchronization
│   │   ├── SyncConfiguration.cs     # Sync configuration
│   │   └── PaymentCardControl.cs    # Custom payment card control
│   │
│   ├── Program.cs                   # Application entry point
│   ├── Kost_SiguraGura.csproj       # Project file
│   └── app.config                   # Application configuration
│
├── docs/                             # Official documentation (TO CREATE)
│   ├── README.md                    # Project overview
│   ├── INSTALLATION.md              # Installation guide
│   ├── API.md                       # API documentation
│   ├── ARCHITECTURE.md              # System architecture
│   ├── USER_GUIDE.md                # User guide
│   └── DEVELOPER.md                 # Developer guide
│
├── .github-resources/               # GitHub specific files
│   ├── CONTRIBUTING.md              # Contribution guidelines
│   ├── CODE_OF_CONDUCT.md          # Code of conduct
│   └── ISSUE_TEMPLATES/            # Issue templates
│
├── README.md                         # Main project README ✅ TO COMMIT
├── LICENSE                          # Project license ✅ TO COMMIT
├── CHANGELOG.md                     # Official changelog ✅ TO COMMIT (final version only)
├── GIT_PUSH_GUIDELINES.md          # This file - guidelines for git push 🆕
└── .gitignore                       # Git ignore file ✅ TO COMMIT

```

---

## 📋 FILE CATEGORIES

### **Core Application Files** (MUST COMMIT ✅)

These files are essential to the application:

| File/Folder | Purpose | Commit? |
|-------------|---------|---------|
| `*.cs` | C# source code | ✅ Yes |
| `*.csproj` | Project configuration | ✅ Yes |
| `*.sln` | Solution file | ✅ Yes |
| `*.resx` | Resource files | ✅ Yes |
| `app.config` | App configuration (no secrets) | ✅ Yes |
| `Program.cs` | Application entry point | ✅ Yes |

---

### **Official Documentation** (MUST COMMIT ✅)

These documents are part of official project documentation:

| File | Purpose | Commit? |
|------|---------|---------|
| `README.md` | Project overview & quick start | ✅ Yes |
| `LICENSE` | Project license | ✅ Yes |
| `CHANGELOG.md` | Official changelog (final version) | ✅ Yes |
| `docs/API.md` | API documentation | ✅ Yes |
| `docs/INSTALLATION.md` | Installation instructions | ✅ Yes |
| `CONTRIBUTING.md` | Contribution guidelines | ✅ Yes |
| `.github/workflows/` | CI/CD workflows | ✅ Yes |

---

### **Development Documentation** (DO NOT COMMIT ❌)

These are internal development files:

| File | Purpose | Commit? | Location |
|------|---------|---------|----------|
| `AUDIT_FINDINGS_DETAILED.md` | Code audit report | ❌ No | Local |
| `LOCALIZATION_IMPLEMENTATION_GUIDE.md` | Implementation guide | ❌ No | Local |
| `*_NOTES.md` | Developer notes | ❌ No | Local |
| `DEBUGGING_NOTES.md` | Debugging documentation | ❌ No | Local |
| `*_TODO.md` | Task lists | ❌ No | Local |
| `DEVELOPER_NOTES.md` | Personal notes | ❌ No | Local |

**These are excluded in `.gitignore` ✅**

---

### **Build & IDE Files** (DO NOT COMMIT ❌)

Generated and machine-specific files:

| File/Folder | Purpose | Commit? |
|-------------|---------|---------|
| `bin/` | Compiled binaries | ❌ No |
| `obj/` | Intermediate object files | ❌ No |
| `.vs/` | Visual Studio cache | ❌ No |
| `.vscode/` | VS Code settings | ❌ No |
| `*.user` | Project user options | ❌ No |
| `*.suo` | Solution user options | ❌ No |

**These are excluded in `.gitignore` ✅**

---

### **Sensitive Files** (NEVER COMMIT ⚠️)

Files containing secrets or sensitive data:

| File | Purpose | Commit? | Reason |
|------|---------|---------|--------|
| `.env` | Environment variables | ❌ Never | Contains secrets |
| `.env.local` | Local environment vars | ❌ Never | Contains secrets |
| `appsettings.Local.json` | Local config with secrets | ❌ Never | Contains credentials |
| `secrets/` | Secrets directory | ❌ Never | Contains API keys |
| `*.secrets` | Secret files | ❌ Never | Contains passwords |

**These are excluded in `.gitignore` ✅**

---

## 🔧 FILE FIXES IMPLEMENTED

The following files were modified during the code fix phase:

### Modified Files (MUST COMMIT ✅)

| File | Changes | Lines Changed |
|------|---------|----------------|
| `ApiClient.cs` | Added HttpClient timeout configuration | +8 |
| `Session.cs` | Implemented thread-safe session management | Complete rewrite |
| `Form1.cs` | Added security validation for login role | +30 |
| `DataPenyewa.cs` | Added CellDoubleClick event wiring | +1 |
| `AddKamar.cs` | Added file size validation for images | +21 |
| `BerandaPage.cs` | Fixed async patterns + added JSON validation | +80 |
| `DataKamar.cs` | Added parallel image loading + HTTP validation | +60 |

**These code fixes should be committed immediately to GitHub ✅**

---

## 📊 DOCUMENTATION STATUS

### ✅ Completed Documentation (Local Reference)
- `AUDIT_FINDINGS_DETAILED.md` - Detailed audit report
- `GIT_PUSH_GUIDELINES.md` - This file!
- `LOCALIZATION_IMPLEMENTATION_GUIDE.md` - Localization roadmap

**→ These are in `.gitignore` and will NOT be pushed**

### 📝 Documentation Needed for GitHub (TO CREATE)

Create these files in `docs/` folder:

```
docs/
├── README.md                    # Technical overview
├── INSTALLATION.md              # How to set up the project
├── API.md                       # Backend API documentation
├── ARCHITECTURE.md              # System design & architecture
├── USER_GUIDE.md                # How to use the application
├── DEVELOPER_GUIDE.md           # Developer setup & contribution
└── TROUBLESHOOTING.md           # Common issues & solutions
```

---

## 🚀 GIT WORKFLOW

### Before Making a Commit

```bash
# 1. Check status
git status

# 2. See what will be committed
git diff --cached

# 3. Verify .md files are NOT being committed
git ls-files | grep "\.md$"
```

### Safe Push Checklist

```bash
# Only code files and essential docs should be staged:
# ✓ *.cs files
# ✓ *.csproj, *.sln files
# ✓ app.config, packages.config
# ✓ README.md, LICENSE, CHANGELOG.md
# ✗ AUDIT_*.md (should NOT be included)
# ✗ bin/, obj/, .vs/ (should NOT be included)
# ✗ .env, secrets/ (should NEVER be included)
```

---

## 📌 IMPORTANT REMINDERS

### ✅ DO COMMIT:
- All C# source code changes
- Bug fixes and features
- Configuration changes (without secrets)
- Official documentation updates
- `.gitignore` file itself

### ❌ DO NOT COMMIT:
- Audit reports and analysis files
- Development notes and TODOs
- IDE-specific folders
- Build artifacts (bin/, obj/)
- Local configuration files
- Sensitive data (API keys, passwords)

### ⚠️ CRITICAL - NEVER COMMIT:
- `.env` files with secrets
- Database files
- API keys or credentials
- Personal SSH keys
- Configuration with database passwords

---

## 🔗 RELATED DOCUMENTS

- **`.gitignore`** - File exclusion rules
- **`GIT_PUSH_GUIDELINES.md`** - Detailed push guidelines
- **`README.md`** - Project overview
- **`CONTRIBUTING.md`** - How to contribute

---

## 📞 QUICK REFERENCE

| Question | Answer | File |
|----------|--------|------|
| What files should I commit? | Check `.gitignore` and this file | `.gitignore`, This file |
| What files must NOT be committed? | Analysis reports, IDE files, secrets | `GIT_PUSH_GUIDELINES.md` |
| How to structure commits? | One feature/fix per commit | See workflow above |
| Where should new docs go? | In `docs/` folder for official docs | `docs/` |
| Where should dev notes go? | Local machine only, not committed | Local files (in `.gitignore`) |

---

## 📋 DOCUMENT OWNERSHIP

| File | Owner | Updates |
|------|-------|---------|
| `.gitignore` | Team Lead | When new file types need excluding |
| `GIT_PUSH_GUIDELINES.md` | Documentation Lead | When guidelines change |
| `README.md` | Product Owner | Project status updates |
| `docs/*` | Technical Writer | Official documentation |
| `CHANGELOG.md` | Release Manager | For each release |

---

## ✨ FINAL NOTES

1. **Keep it clean** - Only commit necessary files
2. **Keep it safe** - Never commit secrets
3. **Keep it organized** - Use proper folder structure
4. **Keep it documented** - Update official docs in `docs/`
5. **Keep it local** - Audit/analysis files stay on your machine

When in doubt, check `.gitignore` or ask before committing!
