# Development Guidelines - Pulsar Admin UI

## Overview

This project is a Vue 3-based web application for managing Apache Pulsar messaging systems. For comprehensive project information, architecture, and usage details, **refer to [GEMINI.md](../GEMINI.md)** which contains the primary project documentation.

This document focuses on advanced development practices and project-specific technical details.

## Prerequisites & Environment

- **Node.js**: Version ^20.19.0 || >=22.12.0 (as specified in package.json engines)
- **Package Manager**: npm (project uses npm, not yarn or pnpm)
- **IDE**: VSCode with Volar extension recommended (disable Vetur)

## Build System & Configuration

### Key Technologies Stack
- **Build Tool**: Vite with custom `rolldown-vite` (experimental Rolldown bundler)
- **Framework**: Vue 3 with Composition API
- **State Management**: Pinia with reactive stores
- **Routing**: Vue Router 4
- **Styling**: Tailwind CSS 4 + DaisyUI components
- **Linting**: Dual setup with ESLint + Oxlint for performance
- **Formatting**: Prettier with Oxc plugin

### Development Configuration Notes

1. **Custom Vite Build**: Uses `npm:rolldown-vite@latest` instead of standard Vite
2. **Proxy Setup**: Development server proxies `/admin` and `/ws` to `localhost:8080` for Pulsar backend
3. **Path Aliases**: `@/` maps to `src/` directory (configured in both jsconfig.json and vite.config.js)
4. **ESLint Flat Config**: Uses modern flat configuration format with Vue essential rules + Oxlint integration

## Testing Setup & Procedures

### Framework: Vitest with Vue Test Utils

The project uses Vitest (Vite's native testing framework) configured with:
- **Test Environment**: jsdom for DOM simulation
- **Vue Testing**: @vue/test-utils for component testing
- **Store Testing**: Full Pinia integration support

### Running Tests

```bash
# Run tests in watch mode (development)
npm test

# Run tests once (CI/production)
npm run test:run

# Run tests with UI interface
npm run test:ui
```

### Writing Tests

Tests should be placed alongside components with `.test.js` extension:

```javascript
// Example: src/components/MyComponent.test.js
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { describe, it, expect, beforeEach } from 'vitest'
import MyComponent from './MyComponent.vue'

describe('MyComponent', () => {
  let wrapper
  let pinia

  beforeEach(() => {
    pinia = createPinia()
    setActivePinia(pinia)
    
    wrapper = mount(MyComponent, {
      props: { /* required props */ },
      global: { plugins: [pinia] }
    })
  })

  it('should render correctly', () => {
    expect(wrapper.exists()).toBe(true)
  })
})
```

### Testing Pinia Stores

When testing components that use Pinia stores:

1. **Create fresh Pinia instance** for each test to avoid state pollution
2. **Mock store data** by manipulating the store's reactive properties directly
3. **Use setActivePinia()** before mounting components
4. **Access store methods** through the composable: `const store = useMyStore()`

Example for message-monitor store:
```javascript
const store = useMessageMonitorStore()
// Mock monitor data by setting the monitors Map
const mockMonitor = { key: 'test-key', status: 'open', messages: [] }
store.monitors.set(mockMonitor.key, mockMonitor)
```

## Code Style & Development Practices

### Linting & Formatting Workflow

```bash
# Run all linting (ESLint + Oxlint)
npm run lint

# Format code with Prettier
npm run format

# Individual linting tools
npm run lint:eslint   # ESLint with auto-fix
npm run lint:oxlint   # Oxlint with correctness rules
```

### Project Structure Conventions

```
src/
├── components/     # Reusable UI components
├── views/         # Top-level page components (routes)
├── stores/        # Pinia state management stores
├── router/        # Vue Router configuration
└── style.css      # Global Tailwind CSS styles
```

### State Management Guidelines

- **Centralized API calls**: All Pulsar Admin API interactions go through `pulsar-admin.js` store
- **Reactive data**: Use `reactive()` for complex objects, `ref()` for primitives
- **Store naming**: Use descriptive names following pattern `use[Domain]Store`
- **Store organization**: Separate stores by domain/feature (admin, messages, preferences)

### API Integration Patterns

The project uses a centralized approach for API calls:

```javascript
// In stores/pulsar-admin.js
const fetchAdmin = async (path, options = {}) => {
  // Centralized error handling, loading states, etc.
}

// Usage in other stores
const response = await admin.fetchAdmin('path/to/endpoint', { method: 'POST' })
```

### Component Development Guidelines

1. **Use Composition API** with `<script setup>` syntax
2. **Props validation**: Always define prop types and required fields
3. **Reactive forms**: Use `reactive()` for form data objects
4. **Event handling**: Use descriptive method names for clarity
5. **Template organization**: Keep templates readable with proper indentation and spacing

### WebSocket Integration

The project includes WebSocket support for real-time message monitoring:
- **Connection management**: Handled in `message-monitor.js` store
- **Proxy configuration**: Development server proxies WebSocket connections
- **Error handling**: Includes timeout and reconnection logic
- **Message buffering**: Configurable buffer size to prevent memory issues

## Build & Deployment

### Development
```bash
npm install
npm run dev  # Starts development server with hot-reload
```

### Production
```bash
npm run build    # Creates optimized production build in dist/
npm run preview  # Preview production build locally
```

### Dependencies Management

The project uses npm with specific version constraints. Key dependencies:
- Vue 3.5.18+ with latest composition API features
- Pinia 3.0.3+ for state management
- Tailwind CSS 4.x (latest major version)
- Custom Vite build with Rolldown bundler

## Debugging & Development Tools

1. **Vue DevTools**: Enabled in development via `vite-plugin-vue-devtools`
2. **Browser DevTools**: Standard debugging with source maps
3. **Pinia DevTools**: Full state inspection and time-travel debugging
4. **Network Proxy**: All `/admin` and `/ws` requests proxied to backend during development

## Common Development Tasks

### Adding New Components
1. Create component in appropriate directory (`components/` or `views/`)
2. Write accompanying test file with `.test.js` extension
3. Import and use with `@/` path alias
4. Follow Vue 3 Composition API patterns

### Adding New Store
1. Create store file in `src/stores/`
2. Use `defineStore` with composition function pattern
3. Export composable with `use[Name]Store` convention
4. Add to appropriate components/views

### Modifying API Endpoints
1. Update `pulsar-admin.js` store with new endpoint methods
2. Use centralized `fetchAdmin` utility for consistency
3. Add proper error handling and loading states

This guidelines document should be updated as the project evolves and new patterns emerge.