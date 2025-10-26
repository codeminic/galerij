# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Galerij** is an image gallery application that displays a slideshow of images from a configured folder. The name is Dutch for "gallery".

The MVP consists of:
- **Backend**: ASP.NET Core 9.0 Minimal API serving images and managing session-specific settings
- **Frontend**: Vue3 + TypeScript with Composition API and Tailwind CSS for a responsive UI
- **Session Management**: Browser-based session tracking via UUID stored in localStorage, persisted settings on backend

## Project Structure

```
/src
├── backend/
│   └── Galerij.Api/
│       ├── Galerij.Api.sln
│       └── Galerij.Api/
│           ├── Program.cs
│           ├── Galerij.Api.csproj
│           ├── appsettings.json
│           ├── appsettings.Development.json
│           ├── Properties/launchSettings.json
│           ├── Options/
│           │   └── GalleryOptions.cs
│           ├── Services/
│           │   ├── ImageService.cs       # Scans folder and streams images
│           │   └── SessionService.cs     # Manages persistent session settings
│           ├── Models/
│           │   ├── ImageMetadata.cs
│           │   └── GallerySettings.cs
│           └── Endpoints/
│               └── GalleryEndpoints.cs
├── frontend/
│   ├── package.json
│   ├── tsconfig.json
│   ├── vite.config.ts
│   ├── tailwind.config.js
│   ├── postcss.config.js
│   ├── src/
│   │   ├── main.ts
│   │   ├── App.vue
│   │   ├── style.css
│   │   ├── components/
│   │   │   ├── Slideshow.vue      # Image display with counter
│   │   │   ├── Controls.vue        # Navigation and control buttons
│   │   │   └── Settings.vue        # Settings modal (interval, autoplay)
│   │   └── composables/
│   │       ├── useSession.ts       # Session ID management
│   │       ├── useGallery.ts       # Image list fetching
│   │       ├── useSettings.ts      # Settings CRUD operations
│   │       └── useSlideshow.ts     # Slideshow logic
│   └── dist/                        # Built output
└── infrastructure/                  # (placeholder)
```

## Build and Run Commands

### Backend (ASP.NET Core 9.0)

Working directory: `/src/backend/Galerij.Api`

**Build:**
```bash
dotnet build
```

**Run Development Server:**
```bash
dotnet run
```
API available at:
- HTTP: http://localhost:5001
- HTTPS: https://localhost:7064

**Configuration:**
- Update `appsettings.json` to configure the gallery folder path and defaults
- `Gallery:ImageFolderPath` - directory containing images (relative to app root)
- `Gallery:AllowedExtensions` - array of allowed file extensions
- `Gallery:DefaultInterval` - default slide interval in milliseconds
- `Gallery:DefaultAutoPlay` - default auto-play setting

### Frontend (Vue3 + Vite)

Working directory: `/src/frontend`

**Install Dependencies:**
```bash
npm install
```

**Development Server:**
```bash
npm run dev
```

**Build for Production:**
```bash
npm run build
```

**Preview Built Output:**
```bash
npm run preview
```

Frontend development server typically runs at `http://localhost:5173`

## Technology Stack

**Backend:**
- .NET 9.0 with Minimal APIs
- C# with nullable reference types and implicit usings
- Swashbuckle for OpenAPI/Swagger documentation

**Frontend:**
- Vue 3 with Composition API
- TypeScript for type safety
- Tailwind CSS for styling
- Vite as build tool
- Axios for HTTP requests

## Architecture Details

### Backend Architecture

**Configuration System (`Options/GalleryOptions.cs`):**
- Uses .NET options pattern for configuration binding
- Loads from `appsettings.json`
- Can be overridden by environment variables

**Services:**
- `ImageService` (scoped): Scans the configured folder and streams image files with directory traversal protection
- `SessionService` (singleton): Manages persistent session settings with in-memory storage backed by file (sessions.json)

**Session Management:**
- No authentication required (local network only)
- Clients identify via `X-Session-Id` header (UUID)
- Settings are persisted per session and survive server restarts

**API Endpoints:**
- `GET /api/gallery/images` - List available images with metadata
- `GET /api/gallery/image/{imageId}` - Stream image file with proper content-type
- `GET /api/gallery/settings` - Get current session settings
- `POST /api/gallery/settings` - Update session settings
- `GET /api/gallery/config` - Get gallery configuration

**Security Considerations:**
- Directory traversal prevention in image path validation
- File extension whitelisting for image types
- CORS policy allows local network access

### Frontend Architecture

**Composition API Pattern:**
- Composables (hooks) for reusable logic
- Reactive state management without additional libraries
- Component-based UI with Tailwind CSS

**Data Flow:**
1. `useSession`: Manages session ID (UUID) persistence
2. `useGallery`: Fetches image list from API
3. `useSettings`: Loads/saves user preferences to API
4. `useSlideshow`: Handles slideshow logic (auto-advance, shuffle, navigation)
5. Components: Display, controls, and settings UI

**Key Features:**
- Always-shuffled slideshow (images are automatically shuffled on load and whenever the image list changes)
- Auto-play with configurable interval
- Manual navigation (next/previous)
- Settings modal for real-time configuration (interval, auto-play)
- Responsive image display with aspect ratio preservation
- Axios for API communication with type-safe request/response handling

## CI/CD Pipeline

The GitHub Actions workflow (`.github/workflows/dotnet.yml`) runs on push/PR to main:
1. Sets up .NET 7.0
2. Restores backend dependencies (from `/src`)
3. Builds the solution
4. Runs tests

Note: Frontend is not currently included in CI/CD pipeline.
