# 📚 Gallery API Backend Specification

## Overview
Dokumentasi lengkap tentang sistem Gallery API backend dan cara kerjanya.

---

## 1. 🔗 Endpoint Structure

### GET /api/galleries
**Tujuan:** Mengambil semua data gallery
**Authentication:** PUBLIC (tidak perlu token)
**Method:** GET
**URL:** `https://rahmatzaw.elarisnoir.my.id/api/galleries`

### POST /api/galleries
**Tujuan:** Upload/Tambah gallery baru
**Authentication:** REQUIRED (Admin Token JWT)
**Method:** POST
**Request Body:** Form-Data dengan file upload

### DELETE /api/galleries/{id}
**Tujuan:** Hapus gallery berdasarkan ID
**Authentication:** REQUIRED (Admin Token JWT)
**Method:** DELETE

---

## 2. 📊 Response Format & Data Structure

### Response Type
- **Format:** Raw JSON Array (TIDAK wrapped dalam object)
- **Status Code:** 200 OK
- **Content-Type:** application/json

### Example Response
```json
[
  {
    "id": 1,
    "title": "Kamar Tidur Utama",
    "category": "Interior",
    "image_url": "https://res.cloudinary.com/...",
    "created_at": "2024-04-14T07:22:00Z",
    "updated_at": "2024-04-14T07:22:00Z"
  },
  {
    "id": 2,
    "title": "Ruang Tamu",
    "category": "Facilities",
    "image_url": "/gallery/67890.jpg",
    "created_at": "2024-04-15T10:15:30Z",
    "updated_at": "2024-04-15T10:15:30Z"
  }
]
```

### Field Descriptions
| Field | Type | Description |
|-------|------|-------------|
| id | int | Gallery unique identifier |
| title | string | Gallery title/name |
| category | string | Gallery category (e.g., "Interior", "Facilities") |
| image_url | string | Image URL (full CDN atau local path) |
| created_at | timestamp | Waktu pembuatan (ISO 8601 format) |
| updated_at | timestamp | Waktu last updated (ISO 8601 format) |

---

## 3. 🖼️ Image URL Handling

### Case 1: Full CDN URL (Cloudinary)
```
https://res.cloudinary.com/dso9w0b8u/image/upload/v1712...
```
✅ **Bisa langsung digunakan** di HttpClient.GetByteArrayAsync()

### Case 2: Local Path (Fallback)
```
/gallery/12345.jpg
```
⚠️ **Perlu di-prepend base URL:**
```csharp
if (imageUrl.StartsWith("/"))
    finalUrl = "https://rahmatzaw.elarisnoir.my.id" + imageUrl;
else
    finalUrl = imageUrl;
```

### Image CDN Source
- **Primary:** Cloudinary (Production)
- **Fallback:** Local file system (jika CLOUDINARY_URL tidak set)
- **Rate Limit:** TIDAK ADA (unlimited requests)

---

## 4. 📋 Pagination & Sorting

❌ **NOT SUPPORTED**

Backend endpoint GET /api/galleries:
- Return **ALL records** sekaligus (no limit/offset parameter)
- Default sort: **PRIMARY KEY (id) ASC** (terlama di atas)
- Tidak ada parameter custom sorting

### Workaround (di frontend C#)
Jika perlu pagination, filter di client-side:
```csharp
// Contoh: ambil 10 items per halaman
var page1 = allGalleries.Skip(0).Take(10).ToList();
var page2 = allGalleries.Skip(10).Take(10).ToList();
```

---

## 5. 🔐 Authentication & Authorization

### Public Endpoint
- ✅ GET /api/galleries - BISA tanpa login

### Protected Endpoint (Admin Only)
- 🔒 POST /api/galleries - PERLU Admin Token
- 🔒 DELETE /api/galleries/{id} - PERLU Admin Token

### Token Format
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 6. 📝 Data Initialization & Seeding

### Database Status Saat Startup
| Table | Status | Seeded Data |
|-------|--------|-------------|
| users | ✅ SEEDED | Admin default account |
| kamar | ✅ SEEDED | Sample rooms |
| gallery | ❌ EMPTY | Kosong (0 records) |

### Implikasi
- Aplikasi **TIDAK menampilkan apapun** saat pertama kali buka halaman Gallery
- User **HARUS** add gallery terlebih dahulu melalui form "Add New Image"
- Setelah add pertama kali, data akan langsung muncul (Real-time)

---

## 7. 🔄 Real-time Updates

### Caching Strategy
❌ **TIDAK ADA caching** (no Redis, no local cache layer)

### Update Flow
1. User click "Add New Image" → Form modal terbuka
2. User submit form → POST /api/galleries
3. Backend return 201 Created + data baru
4. Frontend trigger `LoadGalleries()` 
5. GET /api/galleries return full list (include data baru)
6. UI instantly update tanpa delay

### Latency
- Network dependent (~100-500ms)
- Tidak ada artificial delay/refresh interval

---

## 8. 🐛 Error Scenarios & Troubleshooting

### Scenario 1: No Data Displayed
```
Cause: Database gallery table kosong (seeder tidak populate)
Solution: Add gallery pertama kali via "Add New Image" button
Expected: After add, data will appear instantly
```

### Scenario 2: Image URL Not Loading (404)
```
Cause: Image path local fallback, tapi URL base tidak di-prepend
Solution: Check LoadImageAsync() di frontend - ensure local paths di-prefix dengan base URL
```

### Scenario 3: API Response Parse Error
```
Cause: Response format tidak match JSON array structure
Solution: 
  1. Check backend logs (stderr)
  2. Verify response di Postman: GET https://rahmatzaw.elarisnoir.my.id/api/galleries
  3. Check JSON validity di jsonlint.com
```

### Scenario 4: Network Timeout
```
Cause: HTTP timeout default terlalu pendek, atau network slow
Solution: Frontend already set 30s timeout untuk GET galleries
  - Jika masih timeout, check internet connection
```

---

## 9. 📊 Performance Considerations

### Current Limitations
- **No Pagination:** All records loaded at once (potential issue if >10k records)
- **No Sorting:** Fixed order by ID ASC
- **No Filtering:** Server-side filtering tidak ada (must filter client-side)
- **No Search:** Server-side search tidak ada (must search client-side)

### Recommendations for Scale
If data grows large (>1000 records):
1. Implement pagination di backend: `/api/galleries?page=1&limit=20`
2. Add sorting: `/api/galleries?sort=created_at&order=desc`
3. Add search: `/api/galleries?search=keyword`

---

## 10. 📌 Summary Checklist

- ✅ Endpoint: GET /api/galleries (PUBLIC, no auth needed)
- ✅ Response: Raw JSON Array dengan 6 fields
- ✅ Pagination: NO
- ✅ Sorting: NO (default by ID ASC)
- ✅ Image URL: Full CDN atau local path (needs prepending)
- ✅ Rate Limit: NO
- ✅ Database Init: Gallery table EMPTY (user must add first)
- ✅ Real-time: YES (no caching layer)
- ✅ Update Flow: POST gallery → instant appear on GET list

---

**Last Updated:** 2024-04-15
**Backend Language:** Go + PostgreSQL
**Frontend Language:** C# .NET Framework 4.8
