# Implemented Pulsar Admin API Endpoints

This document lists the Apache Pulsar Admin API endpoints currently implemented and used within this application, along with a brief description of their functionality. The endpoints are based on the Pulsar Admin REST API (v2).

## Broker Operations

*   **GET `/admin/v2/brokers/health`**
    *   **Description:** Checks the health of the Pulsar broker. Returns 200 OK if the broker is healthy.
    *   **Used in:** `checkHealth` function in `pulsar-admin.js`

## Cluster Operations

*   **GET `/admin/v2/clusters`**
    *   **Description:** Retrieves a list of all configured Pulsar clusters.
    *   **Used in:** `getClusters` function in `pulsar-admin.js`

## Tenant Operations

*   **GET `/admin/v2/tenants`**
    *   **Description:** Retrieves a list of all tenants configured in the Pulsar instance.
    *   **Used in:** `getTenants` function in `pulsar-admin.js`

## Namespace Operations

*   **GET `/admin/v2/namespaces/{tenant}`**
    *   **Description:** Retrieves a list of all namespaces for a given tenant.
    *   **Used in:** `getNamespaces` function in `pulsar-admin.js`

## Topic Operations

*   **GET `/admin/v2/persistent/{tenant}/{namespace}`**
    *   **Description:** Retrieves a list of all persistent topics for a given tenant and namespace.
    *   **Used in:** `getTopicsForNamespace` function in `pulsar-admin.js`
*   **GET `/admin/v2/non-persistent/{tenant}/{namespace}`**
    *   **Description:** Retrieves a list of all non-persistent topics for a given tenant and namespace.
    *   **Used in:** `getTopicsForNamespace` function in `pulsar-admin.js`
*   **GET `/admin/v2/schemas/{tenant}/{namespace}/{topic}/schema`**
    *   **Description:** Retrieves the schema for a specific topic.
    *   **Used in:** `fetchTopicDetails` function in `pulsar-admin.js`
*   **GET `/admin/v2/persistent/{tenant}/{namespace}/{topic}/stats`**
    *   **Description:** Retrieves statistics for a persistent topic, including message rates, storage size, and backlog.
    *   **Used in:** `fetchTopicDetails` function in `pulsar-admin.js`
*   **PUT `/admin/v2/persistent/{tenant}/{namespace}/{topic}`**
    *   **Description:** Creates a new persistent topic.
    *   **Used in:** `createTopic` function in `pulsar-admin.js`
*   **DELETE `/admin/v2/persistent/{tenant}/{namespace}/{topic}`**
    *   **Description:** Deletes a persistent topic.
    *   **Used in:** `deleteTopic` function in `pulsar-admin.js`
*   **POST `/admin/v2/persistent/{tenant}/{namespace}/{topic}/retention`**
    *   **Description:** Updates the retention policy for a persistent topic.
    *   **Used in:** `updateRetention` function in `pulsar-admin.js`
*   **GET `/admin/v2/persistent/{tenant}/{namespace}/{topic}/permissions`**
    *   **Description:** Retrieves the permissions for a specific topic.
    *   **Used in:** `getPermissions` function in `pulsar-admin.js`
*   **POST `/admin/v2/persistent/{tenant}/{namespace}/{topic}/permissions/{role}`**
    *   **Description:** Grants permissions to a specific role for a topic.
    *   **Used in:** `grantPermissions` function in `pulsar-admin.js`
*   **DELETE `/admin/v2/persistent/{tenant}/{namespace}/{topic}/permissions/{role}`**
    *   **Description:** Revokes permissions from a specific role for a topic.
    *   **Used in:** `revokePermissions` function in `pulsar-admin.js`
