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
┌─────────────────────────────────────────────────────────────┐
│                     FileTesting API                         │
├─────────────────────────────────────────────────────────────┤
│  Controllers                                                │
│  └── ImagesController                                       │
│      ├── POST /Images          (Upload & Scale)             │
│      ├── GET  /Images          (List All)                   │
│      ├── GET  /Images/{id}     (Get Metadata)               │
│      ├── POST /Images/diagnose (Diagnose Image)             │
│      ├── POST /Images/repair   (Repair & Download)          │
│      └── POST /Images/repair/details (Repair Info)          │
├─────────────────────────────────────────────────────────────┤
│  Services                                                   │
│  ├── ScalingService     (SkiaSharp + Magick.NET)            │
│  ├── ImageRepairService (Magick.NET diagnostics/repair)     │
│  └── MinioService       (S3 storage operations)             │
├─────────────────────────────────────────────────────────────┤
│  Data                                                       │
│  └── AppDbContext       (EF Core MySQL)                     │
└─────────────────────────────────────────────────────────────┘
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
├── FileTesting/                 # ASP.NET Core API
│   ├── Controllers/
│   │   └── ImagesController.cs
│   ├── Models/
│   │   ├── ImageMetadata.cs
│   │   ├── ImageDiagnosticResult.cs
│   │   ├── ImageRepairResult.cs
│   │   └── RepairOptions.cs
│   ├── Services/
│   │   ├── IScalingService.cs
│   │   ├── ScalingService.cs
│   │   ├── IImageRepairService.cs
│   │   ├── ImageRepairService.cs
│   │   ├── IMinioClient.cs
│   │   └── MinioService.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   └── Program.cs
│
├── FileTesting.Client/          # Load Test Console App
│   ├── Program.cs
│   ├── IFileTestingApi.cs
│   ├── DiagnosticModels.cs
│   └── TestImages/
│
└── FileTesting.sln
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
