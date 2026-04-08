# 🔐 Modul 3 — Autentikasi (Login)

Modul ini menangani proses **verifikasi identitas user** ke server sebelum bisa mengakses fitur aplikasi.

---

## 📁 File yang Terlibat

| File | Tipe | Fungsi |
|------|------|--------|
| `Form1.cs` | Form (Entry Point) | UI halaman login + logika autentikasi |
| `Form1.Designer.cs` | Auto-generated | Definisi komponen UI |
| `LoginRequest.cs` | Model | Payload data yang dikirim ke API login |
| `Session.cs` | State Storage | Menyimpan data user setelah login berhasil |

---

## 3.1 — Alur Autentikasi

```
User input Username + Password
        │
        ▼
Klik Tombol Login (btnAuth_Click_2)
        │
        ▼
Buat objek LoginRequest { username, password }
        │
        ▼
Serialize ke JSON → kirim via POST /api/auth/login
        │
        ▼
Terima Response dari Server
        │
   ┌────┴────┐
   │         │
 200 OK    Gagal (4xx/5xx)
   │         │
   │         └─► MessageBox.Show("Login Gagal!")
   │
   ▼
Parse JSON response → cari data user
        │
        ▼
Simpan ke Session (UserId, UserRole, Username)
        │
        ▼
Buka Sidebar (Main Shell) → Sembunyikan Form1
```

---

## 3.2 — Kode Lengkap Form1.cs

```csharp
// File: Form1.cs
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Event handler tombol Login
        private async void btnAuth_Click_2(object sender, EventArgs e)
        {
            // ── STEP 1: Siapkan data login dari input user ──────────────────
            var dataLogin = new LoginRequest
            {
                username = txtUsername.Text,
                password = txtPassword.Text
            };

            // ── STEP 2: Serialize ke JSON ───────────────────────────────────
            string jsonString = JsonConvert.SerializeObject(dataLogin);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            try
            {
                string url = "https://rahmatzaw.elarisnoir.my.id/api/auth/login";

                // ── STEP 3: Kirim POST request ke server ────────────────────
                // Pakai ApiClient.Client agar cookie sesi tersimpan
                HttpResponseMessage response = await ApiClient.Client.PostAsync(url, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // ── STEP 4: Parse JSON response ─────────────────────────
                    Newtonsoft.Json.Linq.JObject result =
                        Newtonsoft.Json.Linq.JObject.Parse(responseBody);

                    Newtonsoft.Json.Linq.JToken userData = null;

                    // Deteksi otomatis struktur JSON response:
                    // Format 1: { "users": [{ ... }] }      → ambil elemen pertama
                    // Format 2: { "user": { ... } }         → langsung ambil
                    // Format 3: { "data": { ... } }         → ambil dari "data"
                    // Format 4: { "id": ..., "role": ... }  → langsung dari root
                    if (result["users"] != null) userData = result["users"][0];
                    else if (result["user"] != null) userData = result["user"];
                    else if (result["data"] != null) userData = result["data"];
                    else userData = result;  // Ambil root jika tidak ada pembungkus

                    if (userData != null && userData.HasValues)
                    {
                        // ── STEP 5: Simpan ke Session ───────────────────────
                        Session.UserId   = (long)(userData["id"] ?? 0);
                        Session.UserRole = userData["role"]?.ToString() ?? "admin";
                        Session.Username = userData["username"]?.ToString() ?? txtUsername.Text;

                        // ── STEP 6: Buka Main Shell ─────────────────────────
                        Sidebar main = new Sidebar();
                        main.Show();
                        this.Hide();  // Sembunyikan form login
                    }
                    else
                    {
                        // JSON valid tapi struktur tidak dikenali
                        MessageBox.Show("Struktur JSON tidak dikenali. Isi Respon:\n" + responseBody);
                    }
                }
                else
                {
                    MessageBox.Show("Login Gagal! Username/Password salah atau Server Error.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi Kesalahan: " + ex.Message);
            }
        }
    }
}
```

---

## 3.3 — Kode LoginRequest.cs

```csharp
// File: LoginRequest.cs
namespace Kost_SiguraGura
{
    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
```

---

## 3.4 — Contoh Response Login dari Server

### ✅ Login Berhasil (200 OK)

```json
{
  "message": "Login berhasil",
  "user": {
    "id": 1,
    "username": "admin_kost",
    "role": "admin",
    "email": "admin@kostsiguragura.id"
  }
}
```

### ❌ Login Gagal (401 Unauthorized)

```json
{
  "message": "Username atau password salah"
}
```

---

## 3.5 — State yang Tersimpan Setelah Login

```csharp
// Setelah login berhasil, nilai ini tersedia di seluruh aplikasi:
Session.UserId   // → 1           (long)
Session.UserRole // → "admin"     (string)
Session.Username // → "admin_kost" (string)
```

---

## 3.6 — Penggunaan Role di Modul Lain

```csharp
// Di DataPenyewa.cs — contoh role-based access control:
private void DataPenyewa_Load(object sender, EventArgs e)
{
    if (Session.UserRole?.ToLower() == "admin")
    {
        LoadDataPenyewa();  // ✅ Admin boleh akses
    }
    else
    {
        MessageBox.Show("Akses Ditolak! Anda bukan Admin.\nRole Anda: " + Session.UserRole);
    }
}
```

---

## 3.7 — Proses Logout

Logout dilakukan di `Sidebar.cs` dengan menutup window dan membuka ulang Form1:

```csharp
// Di Sidebar.cs — tombol logout (guna2Button7)
private void guna2Button7_Click(object sender, EventArgs e)
{
    // Tutup main shell
    this.Close();

    // Buka kembali halaman login
    new Form1().Show();

    // Catatan: Session tidak di-clear secara eksplisit.
    // Rekomendasi: tambahkan Session.Clear() sebelum this.Close()
}

// Rekomendasi tambahan — method untuk clear session:
// (Tambahkan di Session.cs)
public static void Clear()
{
    UserId   = 0;
    UserRole = null;
    Username = null;
    Token    = null;
}
```

---

## ⏭️ Langkah Berikutnya
Setelah user berhasil login, pelajari bagaimana data ringkasan ditampilkan di halaman utama di **[Modul 4 — Dashboard (Beranda)](./04_beranda.md)**.
