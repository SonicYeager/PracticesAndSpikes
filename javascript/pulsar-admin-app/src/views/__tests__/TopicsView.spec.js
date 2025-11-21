import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import TopicsView from '../TopicsView.vue'
import { usePulsarAdminStore } from '@/stores/pulsar-admin'
import { useNotificationStore } from '@/stores/notification'
import TopicCreateModal from '@/components/TopicCreateModal.vue'
import TopicListItem from '@/components/TopicListItem.vue'

// Mock components
const TopicMonitorPanel = { template: '<div>Monitor Panel</div>' }
const TopicPublisherPanel = { template: '<div>Publisher Panel</div>' }

describe('TopicsView', () => {
    let wrapper
    let pulsarStore
    let notificationStore

    beforeEach(() => {
        const pinia = createPinia()
        setActivePinia(pinia)

        pulsarStore = usePulsarAdminStore()
        notificationStore = useNotificationStore()

        // Mock fetch
        const fetchMock = vi.fn((url) => {
            if (url.includes('/tenants')) {
                return Promise.resolve({ ok: true, json: () => Promise.resolve(['tenant1']) })
            }
            if (url.includes('/namespaces')) {
                return Promise.resolve({ ok: true, json: () => Promise.resolve(['tenant1/ns1']) })
            }
            if (url.includes('/persistent/') && !url.includes('/stats') && !url.includes('/permissions') && !url.includes('/schema')) {
                // Topics list
                return Promise.resolve({ ok: true, json: () => Promise.resolve(['persistent://tenant1/ns1/topic1']) })
            }
            if (url.includes('/non-persistent/')) {
                // Topics list
                return Promise.resolve({ ok: true, json: () => Promise.resolve([]) })
            }
            if (url.includes('/stats')) {
                return Promise.resolve({ ok: true, json: () => Promise.resolve({}) })
            }
            if (url.includes('/permissions')) {
                return Promise.resolve({ ok: true, json: () => Promise.resolve({}) })
            }
            // Default fallback
            return Promise.resolve({ ok: true, json: () => Promise.resolve({}) })
        })
        vi.stubGlobal('fetch', fetchMock)

        wrapper = mount(TopicsView, {
            global: {
                plugins: [pinia],
                stubs: {
                    TopicMonitorPanel,
                    TopicPublisherPanel,
                    TopicCreateModal: true,  // Stub to simplify testing
                    TopicListItem: true       // Stub to simplify testing
                }
            }
        })
    })

    afterEach(() => {
        vi.restoreAllMocks()
    })

    it('renders properly', () => {
        expect(wrapper.text()).toContain('Topics')
    })

    it('fetches tenants on mount', async () => {
        await flushPromises()
        expect(global.fetch).toHaveBeenCalledWith(expect.stringContaining('/tenants'), expect.any(Object))
    })

    it('opens create topic modal', async () => {
        const createBtn = wrapper.find('button.btn-primary')
        expect(createBtn.exists()).toBe(true)
        await createBtn.trigger('click')

        const modal = wrapper.findComponent(TopicCreateModal)
        expect(modal.exists()).toBe(true)
    })

    it('creates a topic', async () => {
        // Setup state
        pulsarStore.selectedTenant = 'tenant1'
        pulsarStore.selectedNamespace = 'tenant1/ns1'

        // Open modal
        const createBtn = wrapper.find('button.btn-primary')
        await createBtn.trigger('click')

        // Find modal component
        const modal = wrapper.findComponent(TopicCreateModal)
        expect(modal.exists()).toBe(true)

        // Emit create event
        await modal.vm.$emit('create', {
            tenant: 'tenant1',
            namespace: 'tenant1/ns1',
            topic: 'new-topic',
            retentionTimeInMinutes: '',
            retentionSizeInMB: '',
            role: '',
            actions: []
        })

        // Wait for async handler
        await flushPromises()

        expect(global.fetch).toHaveBeenCalledWith(expect.stringContaining('persistent/tenant1/ns1/new-topic'), expect.objectContaining({ method: 'PUT' }))
        expect(notificationStore.toasts.length).toBeGreaterThan(0)
        expect(notificationStore.toasts[0].type).toBe('success')
    })

    it('deletes a topic', async () => {
        const topic = { fqdn: 'persistent://tenant1/ns1/topic1', tenant: 'tenant1', namespace: 'ns1', topic: 'topic1' }

        // Mock confirm
        window.confirm = vi.fn(() => true)

        await wrapper.vm.doDelete(topic)

        expect(global.fetch).toHaveBeenCalledWith(expect.stringContaining('persistent/tenant1/ns1/topic1'), expect.objectContaining({ method: 'DELETE' }))
    })
})
