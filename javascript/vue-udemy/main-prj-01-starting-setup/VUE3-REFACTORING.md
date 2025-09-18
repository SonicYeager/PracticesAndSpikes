# Vue 3 Refactoring Guide

This document outlines a plan to refactor the project to fully leverage the features of Vue 3. The current implementation uses Vue 3, but it's written with the legacy Options API and uses Vuex for state management. By migrating to the Composition API, Pinia, Vite, and TypeScript, we can significantly improve the project's performance, maintainability, and developer experience.

## Refactoring Areas

### 1. Component Refactoring: Options API to Composition API

The current components are written using the Options API. The Composition API, introduced in Vue 3, offers several advantages:

*   **Better Code Organization:** Logic can be grouped by feature, making components easier to read and maintain.
*   **Improved Reusability:** Logic can be extracted into reusable "composables."
*   **Better Type Inference:** The Composition API provides better type inference, which is especially beneficial when using TypeScript.

**TODO:**

*   [ ] Refactor `App.vue` to use the Composition API.
*   [ ] Refactor `CoachFilter.vue` to use the Composition API.
*   [ ] Refactor `CoachForm.vue` to use the Composition API.
*   [ ] Refactor `CoachItem.vue` to use the Composition API.
*   [ ] Refactor `TheHeader.vue` to use the Composition API.
*   [ ] Refactor `RequestItem.vue` to use the Composition API.
*   [ ] Refactor `UserAuth.vue` to use the Composition API.
*   [ ] Refactor `CoachDetails.vue` to use the Composition API.
*   [ ] Refactor `CoachesList.vue` to use the Composition API.
*   [ ] Refactor `CoachRegistration.vue` to use the Composition API.
*   [ ] Refactor `ContactCoach.vue` to use the Composition API.
*   [ ] Refactor `RequestsReveived.vue` to use the Composition API.

### 2. State Management: Vuex to Pinia

Pinia is the new official state management library for Vue. It's simpler, more lightweight, and more type-safe than Vuex.

**TODO:**

*   [ ] Install Pinia.
*   [ ] Create a new `stores` directory.
*   [ ] Migrate the `auth` Vuex module to a Pinia store.
*   [ ] Migrate the `coaches` Vuex module to a Pinia store.
*   [ ] Migrate the `requests` Vuex module to a Pinia store.
*   [ ] Replace Vuex with Pinia in the application's entry point (`main.js`).

### 3. Build Tooling: Vue CLI to Vite

Vite is a modern build tool that offers a significantly faster development experience than Vue CLI. It uses native ES modules in the browser, which means no bundling is required during development.

**TODO:**

*   [ ] Create a new Vite project.
*   [ ] Copy the source files to the new project.
*   [ ] Adjust the configuration to match the old project.
*   [ ] Install all dependencies.

### 4. Language: JavaScript to TypeScript

TypeScript is a superset of JavaScript that adds static typing. This can help to catch errors early, improve code quality, and make the codebase easier to refactor and maintain.

**TODO:**

*   [ ] Add TypeScript to the project.
*   [ ] Rename all `.js` files to `.ts`.
*   [ ] Add types to the components, stores, and other files.
*   [ ] Configure TypeScript to work with Vue and Pinia.
