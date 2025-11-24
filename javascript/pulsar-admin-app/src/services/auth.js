/**
 * OAuth2 Client Credentials Flow Authentication Service
 * Handles token acquisition, storage, and refresh for Pulsar Admin API.
 */

class OAuth2Service {
    constructor() {
        // Token storage (in-memory only for security)
        this.accessToken = null
        this.tokenExpiry = null
        this.config = null
    }

    /**
     * Initialize the service with OAuth2 configuration.
     * @param {object} config - OAuth2 configuration
     * @param {string} config.clientId - Client ID
     * @param {string} config.clientSecret - Client Secret
     * @param {string} config.tokenEndpoint - Token endpoint URL
     * @param {string} [config.scope] - Optional scope
     */
    initialize(config) {
        this.config = config
        this.clearToken()
    }

    /**
     * Acquires a new access token from the OAuth2 token endpoint.
     * @returns {Promise<string>} The access token
     * @throws {Error} If token acquisition fails
     */
    async acquireToken() {
        if (!this.config) {
            throw new Error('OAuth2 service not initialized')
        }

        const { clientId, clientSecret, tokenEndpoint, scope } = this.config

        try {
            const body = new URLSearchParams()
            body.append('grant_type', 'client_credentials')
            body.append('client_id', clientId)
            body.append('client_secret', clientSecret)
            if (scope) {
                body.append('scope', scope)
            }

            const response = await fetch(tokenEndpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                body: body.toString()
            })

            if (!response.ok) {
                const errorText = await response.text()
                throw new Error(`Token acquisition failed: ${response.status} ${errorText}`)
            }

            const data = await response.json()

            if (!data.access_token) {
                throw new Error('No access token in response')
            }

            // Store token and calculate expiry
            this.accessToken = data.access_token

            // Calculate expiry time (expires_in is in seconds)
            // Subtract 60 seconds as a buffer to refresh before actual expiry
            const expiresIn = data.expires_in || 3600
            this.tokenExpiry = Date.now() + (expiresIn - 60) * 1000

            return this.accessToken
        } catch (error) {
            this.clearToken()
            throw new Error(`Failed to acquire OAuth2 token: ${error.message}`)
        }
    }

    /**
     * Gets a valid access token, refreshing if necessary.
     * @returns {Promise<string>} A valid access token
     */
    async getValidToken() {
        if (this.isTokenValid()) {
            return this.accessToken
        }

        return await this.acquireToken()
    }

    /**
     * Checks if the current token is valid.
     * @returns {boolean} True if token is valid
     */
    isTokenValid() {
        return this.accessToken !== null &&
            this.tokenExpiry !== null &&
            Date.now() < this.tokenExpiry
    }

    /**
     * Clears the stored token.
     */
    clearToken() {
        this.accessToken = null
        this.tokenExpiry = null
    }

    /**
     * Checks if the service is initialized.
     * @returns {boolean} True if initialized
     */
    isInitialized() {
        return this.config !== null
    }
}

// Export a singleton instance
export const oauth2Service = new OAuth2Service()
