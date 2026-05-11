# 01 - TUTORIAL MODUL AUTENTIKASI & LOGIN

**File Terkait:** `Form1.cs`, `Session.cs`, `ApiClient.cs`, `LoginRequest.cs`

---

## 📖 Pendahuluan

Modul autentikasi dan login merupakan gerbang utama aplikasi desktop admin. Modul ini bertanggung jawab untuk memverifikasi identitas pengguna (menggunakan kredensial username & password) sebelum memberikan akses ke dalam sistem. Di tutorial ini, kita akan membangun modul login step-by-step.

### Fungsi Utama
1. **Autentikasi Pengguna** - Verifikasi kredensial ke server backend.
2. **Manajemen Sesi** - Menyimpan data user yang sedang login di memori aplikasi.
3. **Keamanan & Kontrol Akses** - Memastikan hanya role valid yang bisa masuk menggunakan *HTTP Cookies* dan pengecekan otorisasi.

---

## Langkah 1: Memahami Alur (Flow Diagram)

Sebelum mulai coding, penting untuk memahami alur kerja (workflow) login:

```text
┌─────────────────────────────────────────────────────────┐
│              LOGIN SCREEN (Form1)                       │
│  User input Username & Password -> Klik "Masuk"         │
└─────────────────────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────────────────────┐
│     DATA PREPARATION & API CALL                         │
│  Convert to JSON -> POST ke /auth/login                 │
└─────────────────────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────────────────────┐
│     RESPONSE VALIDATION                                 │
│  Parse JSON -> Extract userData -> Cek Role Valid       │
└─────────────────────────────────────────────────────────┘
        /                 \
    Berhasil             Gagal
      /                     \
┌──────────┐           ┌──────────────┐
│ SESSION  │           │ TAMPIL ERROR │
│ TERSIMPAN│           │ (MessageBox) │
└──────────┘           └──────────────┘
      ↓
Buka MainForm, Tutup Form1
```

---

## Langkah 2: Menyiapkan Struktur Data (Model)

Buat class `LoginRequest` untuk merepresentasikan payload JSON yang akan dikirim ke API backend saat login.

```csharp
public class LoginRequest
{
    [JsonProperty("username")]
    public string username { get; set; }

    [JsonProperty("password")]
    public string password { get; set; }
}
```
> [!NOTE]
> Anotasi `[JsonProperty]` berguna agar proses serialisasi/deserialisasi dari library `Newtonsoft.Json` otomatis memetakan variabel di C# ke keys di dalam JSON.

---

## Langkah 3: Menyiapkan Session Management & API Client

Buat static class `Session` untuk menyimpan status pengguna saat aplikasi berjalan.

```csharp
public static class Session
{
    public static long UserId { get; set; }
    public static string UserRole { get; set; }  // "admin", "manager", "staff"
    public static string UserName { get; set; }
    
    public static bool IsLoggedIn() => UserId > 0 && !string.IsNullOrEmpty(UserRole);
    public static void Clear()
    {
        UserId = 0; UserRole = null; UserName = null;
    }
}
```

Buat juga singleton HTTP Client agar `CookieContainer` (Sesi Token dari Server) tetap tersimpan untuk setiap request selanjutnya.

```csharp
public static class ApiClient
{
    private static readonly CookieContainer cookieContainer = new CookieContainer();
    private static readonly HttpClientHandler handler = new HttpClientHandler() 
    { 
        CookieContainer = cookieContainer 
    };

    // Singleton HttpClient
    public static readonly HttpClient Client = new HttpClient(handler)
    {
        Timeout = TimeSpan.FromSeconds(30)
    };
}
```

---

## Langkah 4: Pembuatan UI Form Login

1. Buat **Form1** (atau `LoginForm.cs`).
2. Tambahkan dua `TextBox`: `txtUsername` dan `txtPassword`.
3. Tambahkan tombol `Button`: `btnAuth`.

---

## Langkah 5: Menulis Logika Autentikasi di UI

Tambahkan event click handler pada tombol `btnAuth`. Logika di bawah ini menunjukkan cara membungkus request, memanggil API, dan validasi keamanan secara berurutan.

```csharp
private async void btnAuth_Click_2(object sender, EventArgs e)
{
    // 1. Persiapan Data
    var dataLogin = new LoginRequest 
    { 
        username = txtUsername.Text, 
        password = txtPassword.Text 
    };
    string jsonString = JsonConvert.SerializeObject(dataLogin);
    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

    try
    {
        // 2. Kirim POST Request
        string url = "https://rahmatzaw.elarisnoir.my.id/api/auth/login";
        HttpResponseMessage response = await ApiClient.Client.PostAsync(url, content);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            // 3. Parse Response JSON
            var result = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
            Newtonsoft.Json.Linq.JToken userData = result["users"]?[0] ?? result["user"] ?? result["data"] ?? result;

            if (userData != null && userData.HasValues)
            {
                // 4. Extract dan Validasi Keamanan (Role)
                Session.UserId = (long)(userData["id"] ?? 0);
                string userRole = userData["role"]?.ToString();
                
                var validRoles = new[] { "admin", "manager", "staff" };
                if (string.IsNullOrEmpty(userRole) || !validRoles.Contains(userRole.ToLower()))
                {
                    MessageBox.Show("❌ Akses ditolak: Role tidak valid.", "Security Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 5. Simpan Session & Buka Aplikasi Utama
                Session.UserRole = userRole.ToLower();
                Session.UserName = userData["name"]?.ToString() ?? "Unknown";
                
                MainForm mainForm = new MainForm();
                mainForm.Show();
                this.Hide();
            }
        }
        else
        {
            MessageBox.Show("❌ Username atau password salah.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

---

## Langkah 6: Skenario Pengujian (Testing)

Sebelum pindah ke modul selanjutnya, pastikan melakukan pengujian berikut:

- [ ] **Test Login Sukses**: Input kredensial valid, pastikan `MainForm` terbuka dan Session berisi data yang benar.
- [ ] **Test Invalid Kredensial**: Input password salah, pastikan muncul peringatan error dan aplikasi tidak pindah halaman.
- [ ] **Test Server Mati**: Matikan koneksi internet, tekan login, pastikan `catch` exception menampilkan pesan Network Error dengan elegan.

---

## 📚 Navigasi Dokumentasi

**← Sebelumnya:** [00_INDEX](00_INDEX) - Daftar Isi Modul  
**Berikutnya →** [02_Navigasi_Sidebar](02_Navigasi_Sidebar) - Modul Navigasi & Sidebar

**Daftar Lengkap:** [README_DOKUMENTASI](README_DOKUMENTASI)