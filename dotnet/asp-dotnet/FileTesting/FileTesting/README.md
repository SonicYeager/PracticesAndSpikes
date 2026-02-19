# FileTesting

A .NET 10 ASP.NET Core Web API for image processing, storage, and repair. Features dual image processing backends (SkiaSharp + Magick.NET) with automatic fallback, MinIO/S3 storage integration, and corrupt image detection/repair capabilities.

## Features

- **Image Upload & Scaling** - Automatic thumbnail generation with SkiaSharp (primary) or Magick.NET (fallback)
- **Image Diagnostics** - Detect corrupt patterns, invalid ICC profiles, and structural issues
- **Image Repair** - Fix common image corruption (invalid ICC chunks, malformed profiles)
- **S3/MinIO Storage** - Store original and scaled images in S3-compatible storage
- **Metadata Tracking** - MySQL database for image metadata (dimensions, scaling method, URLs)
- **Load Test Client** - Console application for stress testing the API

## Technology Stack

| Component | Technology |
|-----------|------------|
| Framework | ASP.NET Core (.NET 10) |
| Image Processing | SkiaSharp 3.x, Magick.NET Q8 |
| Storage | MinIO / AWS S3 |
| Database | MySQL (via Pomelo EF Core) |
| API Client | Refit |

## Architecture

```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚                     FileTesting API                         â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚  Controllers                                                â”‚
â”‚  â””â”€â”€ ImagesController                                       â”‚
â”‚      â”œâ”€â”€ POST /Images          (Upload & Scale)             â”‚
â”‚      â”œâ”€â”€ GET  /Images          (List All)                   â”‚
â”‚      â”œâ”€â”€ GET  /Images/{id}     (Get Metadata)               â”‚
â”‚      â”œâ”€â”€ POST /Images/diagnose (Diagnose Image)             â”‚
â”‚      â”œâ”€â”€ POST /Images/repair   (Repair & Download)          â”‚
â”‚      â””â”€â”€ POST /Images/repair/details (Repair Info)          â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚  Services                                                   â”‚
â”‚  â”œâ”€â”€ ScalingService     (SkiaSharp + Magick.NET)            â”‚
â”‚  â”œâ”€â”€ ImageRepairService (Magick.NET diagnostics/repair)     â”‚
â”‚  â””â”€â”€ MinioService       (S3 storage operations)             â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚  Data                                                       â”‚
â”‚  â””â”€â”€ AppDbContext       (EF Core MySQL)                     â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- MySQL 8.0+ or MariaDB 10.6+
- MinIO or S3-compatible storage

### Configuration

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=filetesting;User=root;Password=yourpassword;"
  },
  "AWS": {
    "ServiceURL": "http://localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin"
  }
}
```

### Running the API

```bash
cd FileTesting/FileTesting
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

### Running the Load Test Client

```bash
cd FileTesting/FileTesting.Client
dotnet run
```

The client will automatically generate test images using both SkiaSharp (JPEG) and Magick.NET (PNG).

## API Reference

### Upload Image
```http
POST /Images
Content-Type: multipart/form-data

file: <image file>
```

**Response:**
```json
{
  "success": true,
  "id": "guid",
  "originalUrl": "presigned-url",
  "scaledUrl": "presigned-url"
}
```

### Get Image Metadata
```http
GET /Images/{id}
```

### List All Images
```http
GET /Images
```

### Diagnose Image
```http
POST /Images/diagnose
Content-Type: multipart/form-data

file: <image file>
```

**Response:**
```json
{
  "isCorrupt": false,
  "hasIccIssues": true,
  "isRepairable": true,
  "issues": [],
  "warnings": ["ICC Issue: iCCP: known incorrect sRGB profile"],
  "detectedFormat": "Png",
  "width": 1920,
  "height": 1080
}
```

### Repair Image
```http
POST /Images/repair?stripProfiles=true&stripMetadata=false
Content-Type: multipart/form-data

file: <image file>
```

**Response:** Repaired image file (PNG)

### Repair with Details
```http
POST /Images/repair/details?stripProfiles=true&stripMetadata=false
Content-Type: multipart/form-data

file: <image file>
```

**Response:**
```json
{
  "wasRepaired": true,
  "appliedFixes": ["Removed 1 color profile(s)", "Re-encoded as Png"],
  "originalDiagnostics": { ... },
  "postRepairDiagnostics": { ... },
  "repairedSizeBytes": 245678
}
```

## Known Image Corruption Patterns & Fixes

### ICC Profile Issues

**Problem:** PNG files with incorrectly named ICC chunks (`ICC_` instead of `iCCP`) or invalid sRGB profiles.

**Symptoms:**
- libpng warning: "iCCP: known incorrect sRGB profile"
- SkiaSharp fails to decode the image
- Colors appear incorrect in some viewers

**Fix:** The `ImageRepairService` strips invalid color profiles and re-encodes the image.

```csharp
// Automatic fix via API
POST /Images/repair?stripProfiles=true

// Or use RepairOptions programmatically
var options = new RepairOptions
{
    StripColorProfiles = true,
    ReEncodeImage = true
};
var result = repairService.Repair(imageBytes, options);
```

### CRC Errors

**Problem:** Corrupted PNG chunks with invalid CRC checksums.

**Symptoms:**
- Image fails to load in strict decoders
- Partial image rendering

**Fix:** Magick.NET can often recover these images by re-encoding from the valid pixel data.

### Malformed Metadata

**Problem:** EXIF, XMP, or other metadata blocks contain invalid data.

**Fix:**

```csharp
var options = new RepairOptions
{
    StripMetadata = true  // Removes all metadata
};
```

## Project Structure

```
FileTesting/
â”œâ”€â”€ FileTesting/                 # ASP.NET Core API
â”‚   â”œâ”€â”€ Controllers/
â”‚   â”‚   â””â”€â”€ ImagesController.cs
â”‚   â”œâ”€â”€ Models/
â”‚   â”‚   â”œâ”€â”€ ImageMetadata.cs
â”‚   â”‚   â”œâ”€â”€ ImageDiagnosticResult.cs
â”‚   â”‚   â”œâ”€â”€ ImageRepairResult.cs
â”‚   â”‚   â””â”€â”€ RepairOptions.cs
â”‚   â”œâ”€â”€ Services/
â”‚   â”‚   â”œâ”€â”€ IScalingService.cs
â”‚   â”‚   â”œâ”€â”€ ScalingService.cs
â”‚   â”‚   â”œâ”€â”€ IImageRepairService.cs
â”‚   â”‚   â”œâ”€â”€ ImageRepairService.cs
â”‚   â”‚   â”œâ”€â”€ IMinioClient.cs
â”‚   â”‚   â””â”€â”€ MinioService.cs
â”‚   â”œâ”€â”€ Data/
â”‚   â”‚   â””â”€â”€ AppDbContext.cs
â”‚   â””â”€â”€ Program.cs
â”‚
â”œâ”€â”€ FileTesting.Client/          # Load Test Console App
â”‚   â”œâ”€â”€ Program.cs
â”‚   â”œâ”€â”€ IFileTestingApi.cs
â”‚   â”œâ”€â”€ DiagnosticModels.cs
â”‚   â””â”€â”€ TestImages/
â”‚
â””â”€â”€ FileTesting.sln
```

## Development

### Adding Database Migrations

```bash
cd FileTesting/FileTesting
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

### Building

```bash
dotnet build FileTesting.sln
```

## License

MIT

<!-- DIRECTORY_NAVIGATION:START -->
## Directory Navigation

- Directory: `dotnet/asp-dotnet/FileTesting/FileTesting`
- Parent: [`..`](..) | [Parent README](../README.md)
- Children: [FileTesting](FileTesting/) | [README](FileTesting/README.md), [FileTesting.Client](FileTesting.Client/) | [README](FileTesting.Client/README.md)
<!-- DIRECTORY_NAVIGATION:END -->
