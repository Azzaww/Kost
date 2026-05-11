# 06 - TUTORIAL INTEGRASI API REST

**File Terkait:** `ApiClient.cs`, `App.config`, `packages.config`

---

## 📖 Pendahuluan

Modul Integrasi API adalah jembatan komunikasi antara aplikasi Desktop dan *Backend Server*. Di tutorial ini, kita akan membuat *Singleton HTTP Client* yang reusable. Pola (*Pattern*) *Singleton* sangat penting digunakan pada HTTP Client di .NET untuk mencegah kehabisan sumber daya jaringan (Socket Exhaustion) dan memudahkan manajemen *Cookie* sesi autentikasi.

### Fungsi Utama
1. **Singleton HttpClient** - Hanya ada 1 *instance* `HttpClient` yang mengurus seluruh pemanggilan internet.
2. **Cookie Management** - Menyimpan token sesi autentikasi (hasil login) di dalam memori dan dikirim otomatis setiap memanggil API.
3. **Async / Await** - Eksekusi API secara asynchronous tanpa memberhentikan aplikasi.

---

## Langkah 1: Memahami Arsitektur API

```text
┌─────────────────────────────────────────────────────────┐
│     DESKTOP APP (Forms, UserControls)                  │
│  Memanggil Method Static -> ApiClient.GetRooms()       │
└─────────────────────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────────────────────┐
│     ApiClient.cs (SINGLETON CLASS)                     │
│  Membungkus request dengan header, Cookie Session,     │
│  dan timeout 30 detik.                                 │
└─────────────────────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────────────────────┐
│     BACKEND SERVER (HTTPS)                             │
│  Endpoint: https://rahmatzaw.elarisnoir.my.id/api      │
└─────────────────────────────────────────────────────────┘
```

---

## Langkah 2: Setup Singleton HttpClient & Cookies

Buat sebuah class *static* `ApiClient.cs` di dalam folder core/services Anda.

```csharp
using System.Net;
using System.Net.Http;
using System;

public static class ApiClient
{
    // 1. Buat CookieContainer untuk menyimpan Session Cookie otomatis
    private static readonly CookieContainer cookieContainer = new CookieContainer();
    
    // 2. Pasang container ke dalam HTTP Handler
    private static readonly HttpClientHandler handler = new HttpClientHandler() 
    { 
        CookieContainer = cookieContainer 
    };

    // 3. HttpClient dilarang dideklarasikan berulang kali. Gunakan SATU saja (Singleton).
    public static readonly HttpClient Client = new HttpClient(handler);

    // 4. Base URL API
    public const string BaseUrl = "https://rahmatzaw.elarisnoir.my.id/api";

    // Static Constructor
    static ApiClient()
    {
        // Beri timeout 30 detik untuk menghindari aplikasi nge-hang jika server mati.
        Client.Timeout = TimeSpan.FromSeconds(30);
    }
}
```

---

## Langkah 3: Implementasi Request - GET Data

Metode GET dipakai untuk menarik list data dari server, misal: List Kamar atau List Pembayaran. Di sini, kita akan mengembalikan bentuk String JSON, lalu UI Layer yang akan men-deserialize-nya. Anda juga bisa membungkus proses deserialisasi di sini.

Contoh membungkus API spesifik `GetAllPayments`:
```csharp
public static async Task<List<Pembayaran>> GetAllPayments()
{
    string url = $"{BaseUrl}/payments";
    
    // Eksekusi GET
    HttpResponseMessage response = await Client.GetAsync(url);

    if (response.IsSuccessStatusCode)
    {
        string jsonResponse = await response.Content.ReadAsStringAsync();
        
        // PENTING: Response backend terkadang dibungkus key "data".
        var result = Newtonsoft.Json.Linq.JToken.Parse(jsonResponse);
        var dataArray = result is Newtonsoft.Json.Linq.JArray ? result : (result["pembayarans"] ?? result["data"] ?? result);
        
        return dataArray.ToObject<List<Pembayaran>>() ?? new List<Pembayaran>();
    }
    else if (response.StatusCode == HttpStatusCode.Unauthorized)
    {
        throw new Exception("Sesi login habis. Silakan login ulang.");
    }
    
    throw new Exception($"Error: {response.StatusCode}");
}
```

---

## Langkah 4: Implementasi Request - POST & PUT

Metode POST/PUT mengirimkan *Body* berformat JSON (Payload) ke backend.

```csharp
public static async Task<bool> UpdateRoom(int kamarId, Kamar kamar)
{
    // 1. Serialize C# Object ke format String JSON
    string jsonString = JsonConvert.SerializeObject(kamar);
    
    // 2. Set Content Type menjadi application/json
    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

    // 3. Eksekusi PUT
    string url = $"{BaseUrl}/kamar/{kamarId}";
    HttpResponseMessage response = await Client.PutAsync(url, content);

    if (response.IsSuccessStatusCode)
    {
        return true;
    }
    
    throw new Exception($"Failed to update. Status: {response.StatusCode}");
}
```

---

## Langkah 5: Implementasi Request - DELETE

Metode DELETE paling ringkas karena jarang membawa *Body Payload*. Cukup menyertakan ID di parameter rute URL.

```csharp
public static async Task<bool> DeleteRoom(int kamarId)
{
    string url = $"{BaseUrl}/kamar/{kamarId}";
    HttpResponseMessage response = await Client.DeleteAsync(url);

    return response.IsSuccessStatusCode;
}
```

---

## Langkah 6: Best Practices HTTP di .NET

Sebelum merilis ke fase produksi, selalu ingat hal-hal berikut:
1. **Jangan memanggil `.Result`** dari *Async Task* di lingkungan UI (Windows Forms) karena akan mengakibatkan UI Deadlock/Nge-hang. Gunakan selalu `await`.
2. **Jangan Instansiasi HttpClient di dalam Loop / Button Click**. Gunakan versi static (`ApiClient.Client`). Jika instansiasi dilakukan terus menerus, port PC Anda akan habis digunakan (*Socket Exhaustion*).
3. Bungkus selalu `await Client.GetAsync` di dalam blok `try...catch(HttpRequestException)` agar saat komputer tidak terhubung internet, aplikasi tidak *Crash*.

---

## 📚 Navigasi Dokumentasi

**← Sebelumnya:** [05_Edit_Kamar](05_Edit_Kamar) - Modul Edit Kamar  
**Berikutnya →** [07_Model_Data](07_Model_Data) - Model Data & Struktur

**Daftar Lengkap:** [README_DOKUMENTASI](README_DOKUMENTASI)
