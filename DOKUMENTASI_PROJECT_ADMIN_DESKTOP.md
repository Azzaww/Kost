# 📋 Dokumentasi Project Admin Desktop - Kos Kosan Rahmat ZAW

**Versi:** 1.0  
**Tanggal:** Januari 2026  
**Platform:** Windows Desktop (.NET Framework 4.8)  
**Bahasa:** C# dengan Windows Forms  
**Database:** Remote API (REST)  

---

## 📑 Daftar Isi
1. [Overview Sistem](#overview-sistem)
2. [Arsitektur Teknis](#arsitektur-teknis)
3. [Struktur Project](#struktur-project)
4. [Modul-Modul Utama](#modul-modul-utama)
5. [Data Model](#data-model)
6. [Alur Aplikasi](#alur-aplikasi)
7. [Fitur-Fitur Detail](#fitur-fitur-detail)
8. [Integrasi API](#integrasi-api)
9. [Stack Teknologi](#stack-teknologi)
10. [Keamanan & Autentikasi](#keamanan--autentikasi)

---

## 🎯 Overview Sistem

### Definisi
Aplikasi Desktop Admin untuk Kos Kosan Rahmat ZAW adalah sistem informasi berbasis desktop yang membantu admin mengelola:
- Data kamar kos
- Data penghuni/penyewa
- Transaksi pembayaran
- Laporan bisnis
- Galeri/dokumentasi kos

### Target User
- **Admin Kos**: Mengelola seluruh aspek operasional kos

### Tujuan Utama
1. Mempermudah pengelolaan data kos secara terpusat
2. Mengurangi kesalahan pencatatan manual
3. Menyediakan data real-time yang akurat
4. Membantu monitoring kondisi dan finansial kos
5. Menghasilkan laporan bisnis yang akurat

### Karakteristik Sistem
- ✅ **Offline-First**: Aplikasi berjalan di desktop, namun sinkronisasi dengan server
- ✅ **Cloud-Connected**: Terhubung ke API REST untuk data persistence
- ✅ **Role-Based**: Kontrol akses berbasis role pengguna
- ✅ **Real-Time Updates**: Data selalu tersinkronisasi dengan server
- ✅ **User-Friendly**: Interface intuitif dengan visual feedback

---

## 🏗️ Arsitektur Teknis

### Pola Arsitektur
```
┌─────────────────────────────────────────────────────────┐
│                    Desktop Application                  │
│                  (Windows Forms - C#)                   │
├─────────────────────────────────────────────────────────┤
│  UI Layer (Forms & UserControls)                        │
│  ├─ Login Form (Form1)                                 │
│  ├─ Main Navigation (Sidebar)                          │
│  ├─ Dashboard (BerandaPage)                            │
│  ├─ Room Management (DataKamar)                        │
│  ├─ Tenant Management (DataPenyewa)                    │
│  ├─ Payment Management (PembayaranForm)                │
│  ├─ Reports (Report)                                   │
│  └─ Gallery (GalleryForm)                              │
├─────────────────────────────────────────────────────────┤
│  Business Logic Layer (Services & Managers)             │
│  ├─ Session Management (Session)                        │
│  ├─ Data Sync (DataSyncManager)                         │
│  ├─ API Client (ApiClient)                             │
│  └─ Dashboard Stats (DashboardStats)                   │
├─────────────────────────────────────────────────────────┤
│  Data Model Layer (DTOs & Entities)                     │
│  ├─ Kamar (Room Model)                                 │
│  ├─ Penyewa (Tenant Model)                             │
│  ├─ Pembayaran (Payment Model)                         │
│  ├─ LoginRequest                                        │
│  ├─ PaymentResponse                                     │
│  └─ DashboardStats                                      │
├─────────────────────────────────────────────────────────┤
│  Infrastructure Layer                                   │
│  └─ HTTP Client (HttpClient with Cookie Container)     │
└─────────────────────────────────────────────────────────┘
			  ↓ (HTTPS Communication)
┌─────────────────────────────────────────────────────────┐
│              Backend API Server                         │
│         (https://rahmatzaw.elarisnoir.my.id/api)       │
│                                                         │
│  Endpoints:                                             │
│  ├─ /auth/login                                        │
│  ├─ /kamar (GET, POST, PUT, DELETE)                   │
│  ├─ /penyewa (GET, POST, PUT, DELETE)                 │
│  ├─ /payments (GET, PUT)                              │
│  ├─ /laporan (GET)                                    │
│  ├─ /gallery (GET, POST, DELETE)                      │
│  └─ /dashboard-stats (GET)                            │
└─────────────────────────────────────────────────────────┘
```

### Flow Komunikasi
```
Desktop App
	↓
ApiClient (HttpClient)
	↓ (Request)
Backend API Server
	↓ (Response JSON)
DTO Deserialization (Newtonsoft.Json)
	↓
Business Logic Processing
	↓
UI Rendering (Windows Forms)
```

---

## 📂 Struktur Project

### Organisasi File
```
Kost_SiguraGura/
├── 📄 CORE APPLICATION
│   ├── Program.cs                    → Entry point aplikasi
│   ├── Form1.cs                      → Login Form
│   ├── Sidebar.cs                    → Main Navigation Container
│   └── Session.cs                    → Session Management
│
├── 📄 UI FORMS & CONTROLS
│   ├── BerandaPage.cs                → Dashboard/Home Page (UserControl)
│   ├── DataKamar.cs                  → Room Management Page (UserControl)
│   ├── DataPenyewa.cs                → Tenant Management Page (UserControl)
│   ├── PembayaranForm.cs             → Payment Management Form
│   ├── Report.cs                     → Reports Page (UserControl)
│   ├── GalleryForm.cs                → Gallery Management Form
│   ├── PaymentCardControl.cs         → Reusable Payment Card Control
│   ├── TenantDetailForm.cs           → Tenant Detail Viewer
│   ├── PenyewaDetail.cs              → Tenant Detail Control
│   ├── PembayaranDetail.cs           → Payment Detail Control
│   ├── AddKamar.cs                   → Add/Edit Room Dialog
│   ├── EditKamar.cs                  → Edit Room Dialog
│   ├── AddGallery.cs                 → Add Gallery Image Dialog
│   └── [*.Designer.cs & *.resx]      → Auto-generated Designer files
│
├── 📄 DATA MODELS & DTOs
│   ├── Kamar.cs                      → Room Entity Model
│   ├── Penyewa.cs                    → Tenant Entity Model
│   ├── LoginRequest.cs               → Login Request DTO
│   ├── PaymentResponse.cs            → Payment Response DTO
│   ├── DashboardStats.cs             → Dashboard Statistics DTO
│   └── User.cs (implied)             → User Entity Model
│
├── 📄 SERVICES & MANAGERS
│   ├── ApiClient.cs                  → HTTP Client & API Endpoints
│   ├── DataSyncManager.cs            → Data Synchronization Logic
│   ├── SyncConfiguration.cs          → Sync Configuration Settings
│   └── Session.cs                    → Session State Management
│
├── 📄 CONFIGURATION
│   ├── App.config                    → Application Configuration
│   ├── packages.config               → NuGet Dependencies
│   └── Properties/
│       ├── AssemblyInfo.cs
│       ├── Settings.settings
│       └── Resources.resx
│
└── 📁 Resources/
	├── icon.png                      → App Icon
	├── icon_orange.png               → Alt Icon
	├── home_*.png                    → Home Button Icons
	├── room_*.png                    → Room Button Icons
	├── tenant_*.png                  → Tenant Button Icons
	├── payment_*.png                 → Payment Button Icons
	├── report_*.png                  → Report Button Icons
	├── gallery_*.png                 → Gallery Button Icons
	├── calendar_*.png                → Calendar Icons
	├── search.png                    → Search Icon
	├── add.png                       → Add Button Icon
	└── export_*.png                  → Export Icons
```

### File Dependencies
```
Program.cs
	↓ → Form1.cs (Entry Form)
		 ├─ ApiClient.cs (Authentication)
		 ├─ Session.cs (Session Storage)
		 └─ LoginRequest.cs (DTO)

Form1.cs → Sidebar.cs (Main Container)
	├─ BerandaPage.cs (Home/Dashboard)
	│   ├─ ApiClient.cs
	│   └─ DashboardStats.cs
	├─ DataKamar.cs (Room Management)
	│   ├─ ApiClient.cs
	│   ├─ Kamar.cs
	│   └─ AddKamar.cs / EditKamar.cs
	├─ DataPenyewa.cs (Tenant Management)
	│   ├─ ApiClient.cs
	│   ├─ Penyewa.cs
	│   └─ PenyewaDetail.cs
	├─ PembayaranForm.cs (Payment Management)
	│   ├─ ApiClient.cs
	│   ├─ PaymentResponse.cs
	│   ├─ PaymentCardControl.cs
	│   └─ PembayaranDetail.cs
	├─ Report.cs (Reports)
	│   ├─ ApiClient.cs
	│   └─ DashboardStats.cs
	└─ GalleryForm.cs (Gallery)
		├─ ApiClient.cs
		└─ AddGallery.cs
```

---

## 🔧 Modul-Modul Utama

### 1. **Modul Autentikasi & Session**

#### File Utama
- `Form1.cs` - Login UI
- `Session.cs` - Session Management
- `LoginRequest.cs` - Login DTO
- `ApiClient.cs` - Authentication Logic

#### Alur Proses
```
1. User input username & password di Form1
2. Click tombol Login
3. Kirim POST request ke /api/auth/login
4. Server return User Data + Role
5. Validasi Role (admin/tenant/guest/non_active)
6. Simpan di Session object:
   - Session.UserId
   - Session.UserRole
   - Session.Username
7. Open Sidebar (Main Application)
```

#### Fitur Keamanan
- ✅ Cookie-based session management
- ✅ HTTPS encryption untuk semua request
- ✅ Role validation sebelum login
- ✅ Session timeout (implicit dari server)
- ✅ Request timeout (30 detik)

---

### 2. **Modul Dashboard / Beranda**

#### File Utama
- `BerandaPage.cs` - UI Dashboard
- `DashboardStats.cs` - Data Model
- `Sidebar.cs` - Navigation

#### Fitur Utama
| Fitur | Deskripsi |
|-------|-----------|
| **Welcome Message** | Menampilkan greeting kepada admin |
| **Quick Stats** | Total kamar, penghuni aktif, pembayaran pending |
| **Recent Activity** | Daftar aktivitas terbaru |
| **Navigation Buttons** | Link cepat ke modul lain |
| **Real-time Updates** | Data refresh otomatis setiap interval tertentu |

#### Data Yang Ditampilkan
```json
{
  "totalRooms": 10,
  "occupiedRooms": 7,
  "availableRooms": 3,
  "maintenanceRooms": 0,
  "activeResidents": 7,
  "inactiveResidents": 0,
  "totalIncome": 2100000,
  "pendingPayments": 500000,
  "totalPayments": 1600000
}
```

---

### 3. **Modul Manajemen Kamar**

#### File Utama
- `DataKamar.cs` - Room List UI
- `Kamar.cs` - Room Model
- `AddKamar.cs` - Add Room Dialog
- `EditKamar.cs` - Edit Room Dialog
- `ApiClient.cs` - API calls

#### Fitur Utama
| Fitur | Deskripsi |
|-------|-----------|
| **View Rooms** | Tampilkan daftar semua kamar dengan thumbnail |
| **Add Room** | Tambah kamar baru (dialog popup) |
| **Edit Room** | Edit detail kamar (dialog popup) |
| **Delete Room** | Hapus kamar dari sistem |
| **Search Filter** | Cari kamar berdasarkan nomor |
| **Status Filter** | Filter berdasarkan status kamar |
| **Bilingual Status** | Mendukung "Tersedia/Available", "Penuh/Full", "Perbaikan/Maintenance" |

#### Data Model - Kamar
```csharp
public class Kamar
{
	public int NO { get; set; }              // id dari API
	public string ROOM { get; set; }         // nomor_kamar
	public string TYPE { get; set; }         // tipe_kamar
	public decimal PRICE { get; set; }       // harga_per_bulan
	public int FLOOR { get; set; }           // floor
	public string STATUS { get; set; }       // status (Available/Full/Maintenance)
	public int KAPASITAS { get; set; }       // capacity
	public string SIZE { get; set; }         // ukuran (m²)
	public int BEDROOMS { get; set; }        // jumlah kamar tidur
	public int BATHROOMS { get; set; }       // jumlah kamar mandi
	public string ThumbnailUrl { get; set; } // image_url
	public Image THUMBNAIL { get; set; }     // cached image
	// Fasilitas & deskripsi lainnya...
}
```

#### API Endpoints
```
GET    /api/kamar              → Get all rooms
GET    /api/kamar/{id}         → Get single room detail
POST   /api/kamar              → Create new room
PUT    /api/kamar/{id}         → Update room
DELETE /api/kamar/{id}         → Delete room
```

---

### 4. **Modul Manajemen Penghuni/Penyewa**

#### File Utama
- `DataPenyewa.cs` - Tenant List UI
- `Penyewa.cs` - Tenant Model
- `PenyewaDetail.cs` - Tenant Detail Control
- `TenantDetailForm.cs` - Tenant Detail Form
- `ApiClient.cs` - API calls

#### Fitur Utama
| Fitur | Deskripsi |
|-------|-----------|
| **View Tenants** | Tampilkan daftar penghuni aktif |
| **Add Tenant** | Tambah penghuni baru |
| **View Details** | Lihat detail lengkap penghuni |
| **Edit Tenant** | Edit data penghuni |
| **Delete Tenant** | Hapus penghuni dari sistem |
| **Search Filter** | Cari berdasarkan nama/NIK |
| **Status Filter** | Filter penghuni aktif/non-aktif |

#### Data Model - Penyewa
```csharp
public class Penyewa
{
	public int ID { get; set; }                  // id
	public int? USER_ID { get; set; }            // user_id (FK)
	public User USER { get; set; }               // nested user object
	public string NAMA_LENGKAP { get; set; }     // nama_lengkap
	public string NIK { get; set; }              // NIK/ID number
	public string KONTAK { get; set; }           // email
	public string NOMOR_HP { get; set; }         // nomor_hp
	public DateTime? TANGGAL_LAHIR { get; set; } // tanggal_lahir
	public string ALAMAT_ASAL { get; set; }      // alamat_asal
	public string JENIS_KELAMIN { get; set; }    // jenis_kelamin
	public string FOTO_PROFIL { get; set; }      // foto_profil (URL)
	public string PERAN { get; set; }            // role (tenant/guest/etc)
	public DateTime? CREATED_AT { get; set; }    // created_at
	// Related data
	public List<Pemesanan> PEMESANAN { get; set; }
}
```

#### API Endpoints
```
GET    /api/penyewa              → Get all tenants
GET    /api/penyewa/{id}         → Get single tenant detail
POST   /api/penyewa              → Create new tenant
PUT    /api/penyewa/{id}         → Update tenant
DELETE /api/penyewa/{id}         → Delete tenant
```

---

### 5. **Modul Manajemen Pembayaran**

#### File Utama
- `PembayaranForm.cs` - Payment List UI
- `PaymentResponse.cs` - Payment Model
- `PaymentCardControl.cs` - Reusable Payment Card
- `PembayaranDetail.cs` - Payment Detail Control
- `ApiClient.cs` - API calls

#### Fitur Utama
| Fitur | Deskripsi |
|-------|-----------|
| **View Payments** | Tampilkan daftar pembayaran (grid/card view) |
| **Payment Details** | Lihat detail pembayaran lengkap |
| **Confirm Payment** | Konfirmasi pembayaran dari penyewa |
| **Filter by Status** | Filter pembayaran pending/confirmed/rejected |
| **Filter by Date** | Filter berdasarkan range tanggal |
| **Search** | Cari berdasarkan nama penyewa/kamar |

#### Data Model - Pembayaran
```csharp
public class Pembayaran
{
	public int ID { get; set; }                    // id
	public int PEMESANAN_ID { get; set; }          // pemesanan_id (FK)
	public Pemesanan PEMESANAN { get; set; }       // nested booking object
		// Nested: Penyewa & Kamar
	public decimal JUMLAH { get; set; }            // jumlah
	public DateTime TANGGAL_PEMBAYARAN { get; set; } // tanggal_pembayaran
	public string METODE { get; set; }             // metode (transfer/cash/etc)
	public string STATUS { get; set; }             // status (pending/confirmed/rejected)
	public string BUKTI_PEMBAYARAN { get; set; }   // bukti_pembayaran (URL)
	public DateTime? TANGGAL_KONFIRMASI { get; set; } // tanggal_konfirmasi
	public string CATATAN { get; set; }            // catatan
	public DateTime CREATED_AT { get; set; }       // created_at
}
```

#### Status Pembayaran
- **Pending** - Menunggu konfirmasi dari admin
- **Confirmed** - Sudah dikonfirmasi & diterima
- **Rejected** - Ditolak karena ada masalah

#### API Endpoints
```
GET    /api/payments              → Get all payments
GET    /api/payments/{id}         → Get single payment detail
PUT    /api/payments/{id}/confirm → Confirm payment
PUT    /api/payments/{id}         → Update payment
DELETE /api/payments/{id}         → Delete payment
```

---

### 6. **Modul Laporan**

#### File Utama
- `Report.cs` - Report UI
- `DashboardStats.cs` - Report Data

#### Fitur Utama
| Fitur | Deskripsi |
|-------|-----------|
| **Income Report** | Laporan pemasukan per periode |
| **Occupancy Report** | Laporan okupansi kamar |
| **Tenant Report** | Laporan data penghuni |
| **Payment Report** | Laporan pembayaran |
| **Export to PDF** | Export laporan ke format PDF |
| **Export to Excel** | Export laporan ke format Excel |
| **Date Range Filter** | Filter laporan berdasarkan tanggal |

#### Data Model - Laporan
```csharp
public class DashboardStats
{
	public int TotalRooms { get; set; }
	public int OccupiedRooms { get; set; }
	public int AvailableRooms { get; set; }
	public int MaintenanceRooms { get; set; }
	public int ActiveResidents { get; set; }
	public int InactiveResidents { get; set; }
	public decimal TotalIncome { get; set; }
	public decimal PendingPayments { get; set; }
	public decimal TotalPayments { get; set; }
}
```

#### API Endpoints
```
GET    /api/laporan              → Get all reports
GET    /api/laporan/{id}         → Get specific report
GET    /api/dashboard-stats      → Get dashboard statistics
```

---

### 7. **Modul Galeri**

#### File Utama
- `GalleryForm.cs` - Gallery UI
- `AddGallery.cs` - Add Image Dialog
- `ApiClient.cs` - API calls

#### Fitur Utama
| Fitur | Deskripsi |
|-------|-----------|
| **View Images** | Tampilkan galeri kos |
| **Add Image** | Tambah foto baru |
| **Delete Image** | Hapus foto dari galeri |
| **Categorize** | Kategorisasi foto (interior/exterior/etc) |
| **Lazy Loading** | Load image on-demand untuk performance |

#### Data Model - Gallery
```csharp
public class Gallery
{
	public int ID { get; set; }              // id
	public string TITLE { get; set; }        // title
	public string CATEGORY { get; set; }     // kategori
	public string IMAGE_URL { get; set; }    // image_url
	public DateTime CREATED_AT { get; set; } // created_at
}
```

#### API Endpoints
```
GET    /api/gallery              → Get all gallery images
POST   /api/gallery              → Upload new image
DELETE /api/gallery/{id}         → Delete image
```

---

## 📊 Data Model

### Entity Relationship Diagram (ERD)

```
┌─────────────┐
│    User     │
├─────────────┤
│ id (PK)     │
│ username    │
│ password    │
│ role        │
│ created_at  │
└────────┬────┘
		 │ (1:N)
		 │
		 ├─────────────────┐
		 │                 │
	┌────▼────────┐   ┌────▼──────────┐
	│  Penyewa    │   │  Admin Users  │
	├─────────────┤   └───────────────┘
	│ id (PK)     │
	│ user_id (FK)│──→ User
	│ nama        │
	│ email       │
	│ nik         │
	│ alamat      │
	│ created_at  │
	└────┬────────┘
		 │ (1:N)
		 │
	┌────▼──────────┐
	│  Pemesanan    │
	├───────────────┤
	│ id (PK)       │
	│ penyewa_id(FK)│──→ Penyewa
	│ kamar_id (FK) │──→ Kamar
	│ tgl_mulai     │
	│ durasi        │
	│ status        │
	└────┬──────────┘
		 │ (1:N)
		 │
	┌────▼─────────────┐
	│  Pembayaran      │
	├──────────────────┤
	│ id (PK)          │
	│ pemesanan_id(FK) │──→ Pemesanan
	│ jumlah           │
	│ tgl_pembayaran   │
	│ metode           │
	│ status           │
	│ bukti_url        │
	└──────────────────┘

┌──────────────┐
│    Kamar     │
├──────────────┤
│ id (PK)      │
│ nomor_kamar  │
│ tipe         │
│ harga        │
│ floor        │
│ status       │
│ capacity     │
│ size         │
│ bedrooms     │
│ bathrooms    │
│ image_url    │
│ created_at   │
└──────────────┘

┌────────────────┐
│    Gallery     │
├────────────────┤
│ id (PK)        │
│ title          │
│ kategori       │
│ image_url      │
│ created_at     │
└────────────────┘

┌──────────────────────┐
│  PaymentReminder     │
├──────────────────────┤
│ id (PK)              │
│ pembayaran_id (FK)   │──→ Pembayaran
│ tgl_reminder         │
│ status               │
└──────────────────────┘

┌──────────────────┐
│    Review        │
├──────────────────┤
│ id (PK)          │
│ user_id (FK)     │──→ User
│ kamar_id (FK)    │──→ Kamar
│ rating           │
│ komentar         │
│ created_at       │
└──────────────────┘

┌────────────────┐
│    Laporan     │
├────────────────┤
│ id (PK)        │
│ periode        │
│ total_income   │
│ jml_penghuni   │
│ generated_at   │
└────────────────┘
```

### Relasi Antar Entity
| Relasi | Tipe | Deskripsi |
|--------|------|-----------|
| User → Penyewa | 1:N | 1 user bisa punya banyak data penyewa |
| Penyewa → Pemesanan | 1:N | 1 penyewa bisa punya banyak pemesanan |
| Kamar → Pemesanan | 1:N | 1 kamar bisa punya banyak pemesanan |
| Pemesanan → Pembayaran | 1:N | 1 pemesanan bisa punya banyak pembayaran |
| Pembayaran → PaymentReminder | 1:N | 1 pembayaran bisa punya banyak reminder |
| User → Review | 1:N | 1 user bisa membuat banyak review |
| Kamar → Review | 1:N | 1 kamar bisa punya banyak review |

---

## 🔄 Alur Aplikasi

### 1. Alur Login
```
START
  ↓
[Display Login Form]
  ↓
User input username & password
  ↓
[Click Login Button]
  ↓
POST /api/auth/login
{
  "username": "admin",
  "password": "****"
}
  ↓
[Server validate credentials]
  ↓
Return User Data:
{
  "id": 1,
  "username": "admin",
  "role": "admin",
  ...
}
  ↓
[Validate role]
  ↓
[Store in Session]
Session.UserId = 1
Session.UserRole = "admin"
Session.Username = "admin"
  ↓
[Display Sidebar - Main App]
  ↓
[Enable navigation to other modules]
  ↓
END
```

### 2. Alur Navigasi Utama
```
START (Sidebar)
  ├─→ [Home] → BerandaPage (Dashboard)
  │
  ├─→ [Kamar] → DataKamar
  │    ├─→ [Add] → AddKamar (Dialog)
  │    ├─→ [Edit] → EditKamar (Dialog)
  │    └─→ [Delete] → Confirm Dialog
  │
  ├─→ [Penghuni] → DataPenyewa
  │    ├─→ [View Detail] → PenyewaDetail/TenantDetailForm
  │    ├─→ [Add] → Dialog
  │    └─→ [Edit/Delete] → Dialog
  │
  ├─→ [Pembayaran] → PembayaranForm
  │    ├─→ [View Detail] → PembayaranDetail
  │    └─→ [Confirm] → API Call + Confirmation
  │
  ├─→ [Laporan] → Report
  │    ├─→ [Export PDF] → Generate PDF
  │    └─→ [Export Excel] → Generate Excel
  │
  └─→ [Galeri] → GalleryForm
	   ├─→ [Add Image] → AddGallery (Dialog)
	   └─→ [Delete] → Confirm Dialog
```

### 3. Alur CRUD Kamar
```
READ (View All Rooms)
  ├─ GET /api/kamar
  ├─ Parse JSON Response
  ├─ Display in DataGridView
  └─ Show thumbnail images

CREATE (Add New Room)
  ├─ [Click Add Button]
  ├─ [Open AddKamar Dialog]
  ├─ [Input room details]
  ├─ [Click Save]
  ├─ POST /api/kamar { ...data }
  ├─ [Show success message]
  └─ [Refresh room list]

UPDATE (Edit Room)
  ├─ [Click Edit Button]
  ├─ [Open EditKamar Dialog]
  ├─ [Pre-fill current data]
  ├─ [Modify details]
  ├─ [Click Save]
  ├─ PUT /api/kamar/{id} { ...data }
  ├─ [Show success message]
  └─ [Refresh room list]

DELETE (Remove Room)
  ├─ [Click Delete Button]
  ├─ [Show confirmation dialog]
  ├─ [User confirms]
  ├─ DELETE /api/kamar/{id}
  ├─ [Show success message]
  └─ [Refresh room list]
```

### 4. Alur Manajemen Pembayaran
```
PAYMENT CONFIRMATION FLOW:

Admin Views Payments
  ↓
[Filter/Search payments if needed]
  ↓
[Click on pending payment]
  ↓
[Review payment details]
  ├─ Tenant name
  ├─ Room number
  ├─ Payment amount
  ├─ Payment date
  ├─ Payment method
  └─ Evidence URL (bukti pembayaran)
  ↓
[Click Confirm Payment Button]
  ↓
PUT /api/payments/{id}/confirm
  ↓
[Server updates payment status to "Confirmed"]
  ↓
[Show success notification]
  ↓
[Refresh payment list]
  ↓
Payment status changes from Pending → Confirmed
```

---

## ✨ Fitur-Fitur Detail

### A. Fitur Autentikasi & Keamanan

#### Login
- Username & password validation
- Role-based access control (RBAC)
- Session persistence
- HTTPS encrypted communication

#### Session Management
```csharp
public static class Session
{
	public static long UserId { get; set; }
	public static string UserRole { get; set; }  // "admin", "tenant", "guest"
	public static string Username { get; set; }
}
```

---

### B. Fitur UI/UX

#### Sidebar Navigation
- 6 main menu buttons
- Active state indicator (yellow highlight)
- Icon changes based on active state
- Dynamic user greeting (e.g., "Welcome, Admin!")

#### Status Indicators
- **Kamar Status**: Available (Tersedia), Full (Penuh), Maintenance (Perbaikan)
- **Pembayaran Status**: Pending, Confirmed, Rejected
- **Penyewa Status**: Active, Inactive

#### Icon System
- Home (orange, gray variant)
- Room (orange, gray variant)
- Tenant (orange, gray variant)
- Payment (orange, gray variant)
- Report (orange, gray variant)
- Gallery (orange, gray variant)
- Search icon
- Add icon
- Calendar icon
- Export icon

---

### C. Fitur Data Management

#### Search & Filter
- **Room**: By room number, type, status
- **Tenant**: By name, NIK, status
- **Payment**: By status, date range, tenant name
- **Gallery**: By category

#### Sorting
- Ascending/Descending order
- Multiple column sort

#### Pagination
- Configurable items per page
- Previous/Next navigation
- Jump to page

---

### D. Fitur Reporting

#### Report Types
1. **Pemasukan Bulanan** (Monthly Income Report)
2. **Okupansi Kamar** (Room Occupancy Report)
3. **Data Penghuni** (Tenant Report)
4. **Pembayaran Tertunggak** (Payment Arrears Report)

#### Export Options
- PDF format
- Excel format
- Print option

#### Date Filtering
- Custom date range
- Predefined ranges (This month, Last 3 months, etc)

---

### E. Fitur Real-Time Updates

#### Data Synchronization
- Auto-sync every interval (configurable)
- Manual refresh button
- Sync status indicator
- Error retry mechanism

#### Configuration
```csharp
public class SyncConfiguration
{
	public int IntervalSeconds { get; set; }
	public int MaxRetries { get; set; }
	public int TimeoutSeconds { get; set; }
	public bool AutoSyncEnabled { get; set; }
}
```

---

## 🌐 Integrasi API

### Base URL
```
https://rahmatzaw.elarisnoir.my.id/api
```

### Timeout Configuration
- Default: 30 seconds
- Configurable per request if needed

### Authentication
- Cookie-based (HttpClientHandler with CookieContainer)
- Credentials passed in Login POST request
- Cookies stored automatically by HttpClient

### Request/Response Format
- Format: JSON
- Encoding: UTF-8
- Library: Newtonsoft.Json (Json.NET)

### API Endpoints Summary

#### Authentication
```
POST   /auth/login                 → User login
```

#### Room Management
```
GET    /kamar                      → Get all rooms
GET    /kamar/{id}                 → Get room detail
POST   /kamar                      → Create room
PUT    /kamar/{id}                 → Update room
DELETE /kamar/{id}                 → Delete room
```

#### Tenant Management
```
GET    /penyewa                    → Get all tenants
GET    /penyewa/{id}               → Get tenant detail
POST   /penyewa                    → Create tenant
PUT    /penyewa/{id}               → Update tenant
DELETE /penyewa/{id}               → Delete tenant
```

#### Payment Management
```
GET    /payments                   → Get all payments
GET    /payments/{id}              → Get payment detail
PUT    /payments/{id}/confirm      → Confirm payment
PUT    /payments/{id}              → Update payment
DELETE /payments/{id}              → Delete payment
```

#### Reports
```
GET    /laporan                    → Get all reports
GET    /laporan/{id}               → Get specific report
GET    /dashboard-stats            → Get dashboard statistics
```

#### Gallery
```
GET    /gallery                    → Get all gallery images
POST   /gallery                    → Upload new image
DELETE /gallery/{id}               → Delete image
```

---

## ⚙️ Stack Teknologi

### Platform & Framework
| Komponen | Teknologi |
|----------|-----------|
| **OS Target** | Windows Desktop |
| **Framework** | .NET Framework 4.8 |
| **Language** | C# 7.0+ |
| **UI Framework** | Windows Forms + Guna.UI2 |

### Libraries & Dependencies
| Library | Versi | Fungsi |
|---------|-------|--------|
| **Newtonsoft.Json** | 13.0+ | JSON serialization/deserialization |
| **Guna.UI2** | Latest | Modern UI components |
| **System.Net.Http** | Built-in | HTTP client |
| **System.Windows.Forms** | Built-in | Windows Forms |

### Database
| Aspek | Teknologi |
|-------|-----------|
| **Type** | Relational |
| **Engine** | MySQL / SQLite (Backend) |
| **Access** | REST API (not direct DB) |
| **Sync** | HTTP/HTTPS |

### Development Environment
| Tool | Versi |
|------|-------|
| **IDE** | Visual Studio 2022 Community |
| **VCS** | Git (GitHub) |
| **Build Tool** | MSBuild (.NET Framework) |

---

## 🔐 Keamanan & Autentikasi

### Security Features

#### 1. Authentication
- ✅ Username & password validation
- ✅ HTTPS encrypted communication
- ✅ Cookie-based session management
- ✅ Server-side session validation

#### 2. Authorization
- ✅ Role-Based Access Control (RBAC)
- ✅ Valid roles: admin, tenant, guest, non_active
- ✅ Role validation on login (CRITICAL)
- ✅ Permission checks per feature

#### 3. Data Protection
- ✅ HTTPS for all API calls
- ✅ No plaintext password storage
- ✅ Secure cookie transmission
- ✅ Request timeout (30 sec) to prevent hanging

#### 4. Error Handling
- ✅ 401 Unauthorized → Session expired
- ✅ 403 Forbidden → Insufficient permissions
- ✅ 400 Bad Request → Invalid input
- ✅ 500 Server Error → Generic error message

### Known Security Fixes (Issue #3)
```
CRITICAL FIX: Remove default "admin" role assignment

BEFORE (VULNERABLE):
if (userData != null && userData.HasValues)
{
	Session.UserRole = userRole ?? "admin";  // ❌ DEFAULT ROLE
}

AFTER (SECURE):
if (userData != null && userData.HasValues)
{
	string userRole = userData["role"]?.ToString();
	if (string.IsNullOrEmpty(userRole))
	{
		MessageBox.Show("SECURITY ERROR: User role tidak ditemukan");
		return;
	}
	if (!(new[] {"admin", "tenant", "guest", "non_active"}.Contains(userRole)))
	{
		MessageBox.Show("SECURITY ERROR: Invalid user role");
		return;
	}
	Session.UserRole = userRole;  // ✅ VALIDATED
}
```

### Best Practices Implemented
1. **Principle of Least Privilege** - Only necessary permissions
2. **Input Validation** - All user inputs validated
3. **Error Messages** - Informative without exposing system details
4. **Session Timeout** - Implicit via server cookie expiration
5. **HTTPS Only** - All communication encrypted

---

## 📋 Summary Fitur per Halaman

### Halaman Login (Form1)
- Username input field
- Password input field
- Login button
- Validation messages
- Redirect to Sidebar on success

### Halaman Dashboard (BerandaPage)
- Welcome greeting
- Quick stats (rooms, tenants, income)
- Recent activity feed
- Navigation shortcuts
- Real-time refresh

### Halaman Kamar (DataKamar)
- Room list with thumbnails
- Add button → AddKamar dialog
- Edit button → EditKamar dialog
- Delete button + confirmation
- Search by room number
- Filter by status
- Bilingual status support

### Halaman Penghuni (DataPenyewa)
- Tenant list with avatar
- Add button → Add tenant dialog
- View details button → PenyewaDetail form
- Edit/Delete actions
- Search by name/NIK
- Filter by status

### Halaman Pembayaran (PembayaranForm)
- Payment list (card/grid view)
- PaymentCardControl for each payment
- View details button
- Confirm payment button
- Filter by status (Pending/Confirmed/Rejected)
- Filter by date range
- Search by tenant/room

### Halaman Laporan (Report)
- Report type selector
- Date range picker
- Export to PDF button
- Export to Excel button
- Preview pane
- Print button

### Halaman Galeri (GalleryForm)
- Image gallery grid
- Add button → AddGallery dialog
- Delete button + confirmation
- Category filter
- Lazy loading for images
- Image preview

---

## 🎯 Performance Considerations

### Optimization Strategies
1. **Image Lazy Loading** - Load images on demand, not all at once
2. **Request Timeout** - 30 second timeout prevents hanging
3. **Cookie Reuse** - Single HttpClient for all requests
4. **Async Operations** - All API calls are async to prevent UI freezing
5. **Pagination** - Show limited items per page
6. **Caching** - Cache thumbnail images to reduce network calls

### Scalability
- Designed for small-to-medium kos operations
- Typical: 10-50 rooms, 30-100 active tenants
- Performance acceptable for ~1000 payment records

---

## 📝 Notes & Remarks

### Known Issues & Fixes
1. **Issue #2**: Request timeout not set → FIXED (30 sec timeout added)
2. **Issue #3**: Default "admin" role on failed role parsing → FIXED (Strict validation)

### Future Enhancements
- [ ] Offline mode with local SQLite cache
- [ ] Advanced reporting with charts/graphs
- [ ] SMS/Email notifications for payment reminders
- [ ] Multi-language support (i18n)
- [ ] Dark mode UI option
- [ ] Mobile app companion
- [ ] QR code for room/tenant identification
- [ ] Maintenance scheduling system
- [ ] Lease agreement templates
- [ ] Document upload/storage

### Configuration Files
- `App.config` - Application settings
- `packages.config` - NuGet dependencies
- `Settings.settings` - User preferences
- `SyncConfiguration.cs` - Sync settings

---

## 📞 Support & Documentation

### API Documentation
- Full API docs: https://rahmatzaw.elarisnoir.my.id/docs
- Base URL: https://rahmatzaw.elarisnoir.my.id/api

### Developer Guide
- Project Structure: See Section 3
- Architecture: See Section 2
- Development Setup: Use Visual Studio 2022 with .NET Framework 4.8

### Contact
- Project Repository: https://github.com/Azzaww/Kost
- Maintainer: [Your Name/Team]

---

**Dokumen ini dibuat sebagai referensi rinci untuk Project Admin Desktop Kos Kosan Rahmat ZAW.**  
**Terakhir diperbarui: Januari 2026**  
**Status: Production Ready**
