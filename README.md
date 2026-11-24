# Hệ thống Quản trị Kinh doanh (QTKD) - Phase 2

Dự án thực hành môn **Phát triển Ứng dụng Hệ thống Thông tin Hiện đại**.
* **Công nghệ:** C# · ASP.NET Core Web API (.NET 10) · Entity Framework Core 10 · PostgreSQL · Swashbuckle (Swagger UI) · xUnit (Tuần 07).
* **MSSV:** 23127389
* **Giai đoạn hiện tại:** **Tuần 05 — Tạo nền tảng cho QTKD**

---

## 1. Cấu trúc Solution & Thư mục

Repository được bố trí theo tiêu chuẩn xuyên suốt cả 3 tuần (Tuần 05, 06, 07):

```text
23127389/
├── src/
│   └── QTKD.Api/
│       ├── Controllers/          # Tầng API: CustomersController, ContractsController
│       ├── Services/             # Tầng Nghiệp vụ: ICustomerService, IContractService
│       ├── Entities/             # Thực thể: Customer, Contract, PriceList, PriceItem, Enums
│       ├── Dtos/                 # Data Transfer Objects: CustomerDto, ContractDto
│       ├── Data/                 # Truy cập CSDL: QtkdDbContext, QtkdDbContextFactory
│       ├── Migrations/           # EF Core Migrations (InitialCreate có SeedData)
│       ├── Program.cs            # Cấu hình DI, Swagger UI, JSON Serializer
│       └── appsettings.json      # File cấu hình chuỗi kết nối mẫu (không lộ mật khẩu)
├── tests/                        # Kiểm thử tự động xUnit (dành cho Tuần 07)
├── docs/                         # Tài liệu thiết kế & sơ đồ kiến trúc
│   ├── architecture.md           # Mô tả kiến trúc phân tầng & ERD (Mermaid)
│   └── architecture.svg          # Sơ đồ kiến trúc dạng đồ họa SVG
├── QTKD.slnx                     # Solution file
├── .gitignore                    # Bỏ qua bin/, obj/, user secrets, appsettings.Development.json
└── README.md                     # Hướng dẫn chi tiết thiết lập và chạy dự án
```

---

## 2. Yêu cầu môi trường

* **.NET SDK:** 10.0.x (hoặc .NET 8/9/10). Kiểm tra bằng lệnh: `dotnet --version`
* **PostgreSQL:** Bản 14+ (đang chạy service trên cổng 5432).
* **Công cụ EF Core CLI:** `dotnet-ef` (cài đặt: `dotnet tool install --global dotnet-ef`).
* **Git & IDE:** Visual Studio 2022 / VS Code / JetBrains Rider.

---

## 3. Hướng dẫn thiết lập & Khởi chạy từng bước

### Bước 1: Tạo Database rỗng trên PostgreSQL
Mở `psql` hoặc **pgAdmin** và chạy lệnh SQL:
```sql
CREATE DATABASE qtkd_dev;
```

### Bước 2: Cấu hình Chuỗi kết nối (Bảo mật)
Theo yêu cầu đề bài **"không đưa mật khẩu lên Git"**, bạn nên dùng **.NET User Secrets** hoặc biến môi trường:

**Cách 1: Sử dụng .NET User Secrets (Khuyên dùng)**
Tại thư mục gốc dự án, di chuyển vào project API và thiết lập:
```powershell
cd src/QTKD.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=qtkd_dev;Username=postgres;Password=MAT_KHAU_POSTGRES_CUA_BAN"
cd ../..
```

**Cách 2: Sử dụng Biến môi trường**
* Trên PowerShell:
  ```powershell
  $env:ConnectionStrings__Default="Host=localhost;Port=5432;Database=qtkd_dev;Username=postgres;Password=MAT_KHAU_POSTGRES_CUA_BAN"
  ```
* Trên Linux / macOS / Bash:
  ```bash
  export ConnectionStrings__Default="Host=localhost;Port=5432;Database=qtkd_dev;Username=postgres;Password=MAT_KHAU_POSTGRES_CUA_BAN"
  ```

### Bước 3: Chạy Migration để tạo bảng & nạp dữ liệu mẫu (Seed Data)
Tại thư mục gốc solution, thực thi lệnh sau:
```powershell
dotnet ef database update --project src/QTKD.Api
```

Lệnh này sẽ tự động:
1. Tạo 4 bảng: `Customers`, `Contracts`, `PriceLists`, `PriceItems` cùng bảng lịch sử `__EFMigrationsHistory`.
2. Tạo các ràng buộc Khóa chính, Khóa ngoại (`Contracts.CustomerId -> Customers.Id`, `PriceItems.PriceListId -> PriceLists.Id`), Unique Index (`Contracts.ContractNo`).
3. Tự động nạp dữ liệu mẫu ban đầu:
   * **Customer (2):** `Công ty Minh Phát` (Id=1), `Công ty Hải Long` (Id=2) - Status: `Active`.
   * **Contract (1):** `HD-2026-018` (Id=1, CustomerId=1) - Status: `Draft`, `StartDate` & `EndDate` null.
   * **PriceList (1):** `Bảng giá 2026` (Id=1, EffectiveDate=2026-01-01).
   * **PriceItem (2):** `SV-001` (1,500,000 VNĐ), `SV-002` (2,300,000 VNĐ).

### Bước 4: Khởi động Web API
Chạy lệnh:
```powershell
dotnet run --project src/QTKD.Api
```

API sẽ khởi chạy tại các địa chỉ (xem log trên màn hình terminal, ví dụ: `https://localhost:7089` hoặc `http://localhost:5089`).

---

## 4. Thử nghiệm API & Đối chiếu dữ liệu

Mở trình duyệt truy cập Swagger UI tại:
```text
https://localhost:<port>/swagger
```

### 1. `GET /api/customers`
* **Mục đích:** Lấy toàn bộ danh sách khách hàng.
* **Kết quả trả về mong đợi (HTTP 200 OK):**
  ```json
  [
    {
      "id": 1,
      "name": "Công ty Minh Phát",
      "status": "Active"
    },
    {
      "id": 2,
      "name": "Công ty Hải Long",
      "status": "Active"
    }
  ]
  ```

### 2. `GET /api/contracts/1`
* **Mục đích:** Lấy chi tiết hợp đồng Id = 1 (kèm tên khách hàng).
* **Kết quả trả về mong đợi (HTTP 200 OK):**
  ```json
  {
    "id": 1,
    "contractNo": "HD-2026-018",
    "customerId": 1,
    "customerName": "Công ty Minh Phát",
    "startDate": null,
    "endDate": null,
    "status": "Draft"
  }
  ```

### 3. `GET /api/contracts/999`
* **Mục đích:** Kiểm tra xử lý trường hợp hợp đồng không tồn tại.
* **Kết quả trả về mong đợi (HTTP 404 Not Found):**
  ```json
  {
    "error": "Contract not found",
    "id": 999
  }
  ```

---

## 5. Đối chiếu dữ liệu với PostgreSQL

Dùng pgAdmin hoặc `psql -U postgres -d qtkd_dev` để chạy các câu truy vấn đối chiếu:

```sql
-- 1. Xem danh sách khách hàng
SELECT "Id", "Name", "Status" FROM "Customers";

-- 2. Xem hợp đồng và khách hàng liên kết
SELECT c."Id", c."ContractNo", c."Status", c."StartDate", c."EndDate", cust."Name" AS "CustomerName"
FROM "Contracts" c
JOIN "Customers" cust ON c."CustomerId" = cust."Id";

-- 3. Xem bảng giá và các dịch vụ đi kèm
SELECT pl."Name" AS "PriceListName", pi."ServiceCode", pi."UnitPrice"
FROM "PriceLists" pl
JOIN "PriceItems" pi ON pl."Id" = pi."PriceListId";
```

---

## 6. Minh chứng chuẩn bị nộp bài Tuần 05
1. **Mã nguồn:** Đã tổ chức đầy đủ các tầng (`Controllers`, `Services`, `Entities`, `Dtos`, `Data`, `Migrations`).
2. **Sơ đồ kiến trúc:** Xem tại [`docs/architecture.md`](file:///D:/nam4_ki1/23127389/docs/architecture.md) hoặc [`docs/architecture.svg`](file:///D:/nam4_ki1/23127389/docs/architecture.svg).
3. **Ảnh/Video quay màn hình minh chứng:**
   * Terminal khởi động API và thực hiện migrations.
   * Giao diện Swagger UI thực hiện 2 endpoint (`GET /api/customers`, `GET /api/contracts/1`).
   * Cửa sổ pgAdmin/psql hiển thị kết quả truy vấn trong database `qtkd_dev`.
