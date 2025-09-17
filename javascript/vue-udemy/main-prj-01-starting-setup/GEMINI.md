# Project Overview

This project is a Vue.js application for finding and hiring coaches. It allows users to browse a list of coaches, view their details, and contact them. Coaches can also register and manage their profiles. The application uses Vue Router for navigation and Vuex for state management.

## Technologies

*   **Vue.js:** The core framework for building the user interface.
*   **Vue Router:** For handling client-side routing and navigation.
*   **Vuex:** For managing the application's state.
*   **Webpack:** For bundling and building the project (via vue-cli-service).

# Building and Running

*   **Install dependencies:**
    ```bash
    npm install
    ```
*   **Run the development server:**
    ```bash
    npm run serve
    ```
*   **Build for production:**
    ```bash
    npm run build
    ```
*   **Lint files:**
    ```bash
    npm run lint
    ```

# Development Conventions

*   **Component-Based Architecture:** The application is structured into reusable Vue components, located in `src/components`.
*   **State Management:** Vuex is used for centralized state management. The store is organized into modules for `coaches`, `requests`, and `auth`.
*   **Routing:** Vue Router is used for all navigation. Routes are defined in `src/router.js`.
*   **UI Components:** The project includes a set of base UI components in `src/components/ui` that are globally registered for consistency.
*   **Authentication:** Route guards are used to protect routes that require authentication. The authentication state is managed in the `auth` Vuex module.
