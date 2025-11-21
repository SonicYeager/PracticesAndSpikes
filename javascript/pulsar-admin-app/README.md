# Pulsar Admin UI

## Project Overview

This project is a web application designed to serve as an administrative user interface for Apache Pulsar. It is built using Vue.js 3 and the surrounding modern frontend ecosystem.

The application allows users to connect to a Pulsar instance, view and manage various resources like tenants, namespaces, and topics. It provides features for real-time message monitoring, topic creation/deletion, and managing topic-level policies like retention and permissions.

## Core Technologies

*   **Framework:** Vue.js 3
*   **Build Tool:** Vite (using Rolldown)
*   **Routing:** Vue Router
*   **State Management:** Pinia
*   **Styling:** Tailwind CSS with the DaisyUI component library
*   **Linting:** ESLint and Oxlint
*   **Formatting:** Prettier

## Architecture

The application is a single-page application (SPA). It communicates with a backend Pulsar Admin API. The `vite.config.js` is set up to proxy requests from `/admin` and `/ws` on the development server to a Pulsar broker running on `http://localhost:8080` to simplify local development and avoid CORS issues.

State is managed centrally using Pinia stores, with separate modules for different domains of the Pulsar Admin API (`pulsar-admin.js`), message monitoring (`message-monitor.js`), and user preferences (`preferences.js`).

## Building and Running

The project uses `npm` for dependency management and running scripts.

### Installation

Install the necessary Node.js dependencies.

```sh
npm install
```

### Development Server

To compile and run the application with hot-reloading for development:

```sh
npm run dev
```

The application will be accessible at a local URL provided by Vite (typically `http://localhost:5173`). This requires a Pulsar broker to be running and accessible at `http://localhost:8080`.

### Production Build

To compile and minify the application for production:

```sh
npm run build
```

The output will be generated in the `dist` directory.

### Preview Production Build

To serve the production build locally for previewing:

```sh
npm run preview
```

## Development Conventions

*   **Code Style & Linting:** The project enforces a consistent code style using ESLint, Oxlint, and Prettier. Before committing, it's recommended to run the linting and formatting scripts:
    ```sh
    # Run both ESLint and the faster Oxlint
    npm run lint

    # Format all files in the src directory
    npm run format
    ```
*   **Path Aliases:** The project is configured with a path alias in `jsconfig.json`. Use `@/` to refer to the `src` directory for cleaner import paths (e.g., `import HomeView from '@/views/HomeView.vue'`).
*   **State Management:** All application state should be managed through Pinia. New stateful logic should be added to existing stores or encapsulated in new ones within the `src/stores` directory.
*   **API Interaction:** All communication with the Pulsar Admin REST API is centralized in the `pulsar-admin.js` store. This provides a single place to manage API calls, loading states, and error handling. The `fetchAdmin` utility function should be used for making requests.
*   **Component Structure:** The application is organized into `views` (top-level page components) and `components` (reusable UI elements).
