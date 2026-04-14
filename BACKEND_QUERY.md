# Backend API Query - Tenant Status Filter

## Pertanyaan tentang Endpoint: GET /api/tenants

Kami sedang mengimplementasikan fitur filtering di halaman tenant management. Perlu klarifikasi tentang parameter filter untuk status tenant.

### 1. Parameter Query yang Diterima
- Apakah endpoint `/api/tenants` menerima parameter **`status`** atau **`role`** untuk filtering?
- Atau apakah ada parameter lain yang digunakan?

### 2. Nilai/Value yang Diterima untuk Status
Untuk fitur ini, kami ingin bisa filter tenant berdasarkan status mereka:

**"Active Tenants"** (Tenant Aktif/Sedang Menghuni)
- Apa value/string yang seharusnya dikirim? 
- Contoh: `?status=active` atau `?role=active` atau yang lain?

**"Non Active"** (Tenant Tidak Aktif)
- Apa value/string yang seharusnya dikirim?
- Contoh: `?status=non_active` atau `?status=inactive` atau yang lain?

**"Guest Users"** (User Tamu)
- Apa value/string yang seharusnya dikirim?
- Contoh: `?status=guest` atau `?role=guest` atau yang lain?

### 3. Data Model Tenant
- Apakah di model Penyewa/Tenant ada field bernama `status` atau `is_active` atau sesuatu yang menunjukkan status aktif/tidak aktif?
- Atau status itu disimpan di field `role`?
- Mohon jelaskan struktur field yang berkaitan dengan status tenant

### 4. Contoh Response
Bisa berikan contoh response dari endpoint ini dengan filter status?
- `/api/tenants?status=active` → response dengan tenant aktif
- `/api/tenants?status=non_active` → response dengan tenant tidak aktif  
- `/api/tenants?status=guest` → response dengan guest users

### 5. Field yang Dikembalikan
Mohon confirm bahwa response endpoint `/api/tenants` sudah include field:
- `id` ✓
- `nama_lengkap` ✓
- `email` ✓
- `nomor_hp` ✓
- `nik` ✓
- `tanggal_lahir` ✓
- `alamat_asal` ✓
- `jenis_kelamin` ✓
- `foto_profil` ✓
- `role` (atau `status`?) - **perlu clarifikasi**
- `created_at` ✓
- `updated_at` ✓

---

## Current Implementation
Di desktop app, kami sudah implementasikan dropdown dengan opsi:
- All Status (tampilkan semua)
- Active (tenant aktif/menghuni)
- Non Active (tenant non-aktif)
- Guest Users (user tamu)

Tinggal menunggu klarifikasi tentang parameter & value yang tepat untuk backend.

---

## Contact
Silakan balas dengan informasi di atas untuk bisa menyelesaikan implementasi filtering ini.
