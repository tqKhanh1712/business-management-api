# KIẾN TRÚC HỆ THỐNG QTKD (PHASE 2 - TUẦN 05)

## 1. Sơ đồ các tầng kiến trúc (Solution Architecture)

```mermaid
graph TD
    Client["Client Layer<br/>(Swagger UI / Postman / Browser)"] <-->|"Request (HTTP GET)<br/>Response (JSON)"| Controllers["API Layer (Controllers)<br/>CustomersController: GET /api/customers<br/>ContractsController: GET /api/contracts/{id}"]
    
    Controllers <-->|"Gọi Service (DI)<br/>Trả về DTO"| Services["Service Layer (Nghiệp vụ)<br/>ICustomerService / CustomerService<br/>IContractService / ContractService"]
    
    Services <-->|"Truy vấn LINQ / EF Core<br/>Trả về Entity"| DbContext["Data Access Layer<br/>QtkdDbContext"]
    
    DbContext <-->|"Npgsql Provider (Cổng 5432)"| Postgres[("PostgreSQL Database<br/>Database: qtkd_dev")]

    subgraph "Mô hình thực thể & Quan hệ (Domain Entities)"
        Customer["Customer<br/>• Id (PK)<br/>• Name<br/>• Status"]
        Contract["Contract<br/>• Id (PK)<br/>• ContractNo (Unique)<br/>• CustomerId (FK)<br/>• StartDate, EndDate (nullable)<br/>• Status"]
        PriceList["PriceList<br/>• Id (PK)<br/>• Name<br/>• EffectiveDate"]
        PriceItem["PriceItem<br/>• PK kép: (PriceListId, ServiceCode)<br/>• PriceListId (FK)<br/>• ServiceCode<br/>• UnitPrice"]

        Customer ---|"1 ────────── N<br/>(FK: CustomerId)"| Contract
        PriceList ---|"1 ────────── N<br/>(FK: PriceListId)"| PriceItem
    end
```

## 2. Mô tả chi tiết các thành phần chính
1. **Client Layer**:
   * **Swagger UI** (`https://localhost:<port>/swagger`): Công cụ trực quan kiểm thử trực tiếp API.
   * **Postman / Browser**: Các HTTP client gửi request đọc dữ liệu.
2. **API Layer (`src/QTKD.Api/Controllers`)**:
   * `CustomersController`: Cung cấp endpoint `GET /api/customers`, tiêm phụ thuộc `ICustomerService`, trả về `List<CustomerDto>`.
   * `ContractsController`: Cung cấp endpoint `GET /api/contracts/{id}`, tiêm phụ thuộc `IContractService`, trả về `ContractDto` (hoặc 404 NotFound).
3. **Service Layer (`src/QTKD.Api/Services`)**:
   * `ICustomerService / CustomerService`: Tách biệt logic đọc khách hàng và chuyển đổi sang DTO.
   * `IContractService / ContractService`: Tách biệt logic đọc chi tiết hợp đồng kèm thông tin khách hàng liên kết.
   * *Mục tiêu:* Đảm bảo phân tầng chuẩn theo đề bài, che giấu cấu trúc DB, tránh lỗi vòng lặp JSON và chuẩn bị sẵn sàng cho các rule CTR-01, CTR-02, CTR-03 ở Tuần 06.
4. **Data Access Layer (`src/QTKD.Api/Data`)**:
   * `QtkdDbContext`: Thiết lập cấu hình Fluent API, chuyển đổi Enum sang chuỗi (`HasConversion<string>()`), ràng buộc `Unique Index`, cấu hình hành vi xóa (`Restrict` cho Contract, `Cascade` cho PriceItem).
   * Seed Data (`HasData`): Khởi tạo 2 Customer (`Công ty Minh Phát`, `Công ty Hải Long`), 1 Contract Draft (`HD-2026-018` thiếu ngày), 1 PriceList và 2 PriceItem.
   * Migration đã sinh: `20261124131958_InitialCreate.cs` chứa đầy đủ cấu trúc bảng và seed data.
5. **Domain Entities (`src/QTKD.Api/Entities`)**:
   * Các thực thể: `Customer`, `Contract`, `PriceList`, `PriceItem`.
   * Các Enum: `CustomerStatus` (`Active`, `Inactive`), `ContractStatus` (`Draft`, `PendingApproval`, `RevisionRequested`, `Approved`, `Rejected`).
6. **PostgreSQL Database**:
   * Database: `qtkd_dev`
   * Cổng: `5432`
   * Bảng vật lý: `Customers`, `Contracts`, `PriceLists`, `PriceItems`, `__EFMigrationsHistory`.
