<script>
import StoredResources from '@/components/learning-resource/StoredResource.vue';
import AddResource from '@/components/learning-resource/AddResource.vue';
import LearningResource from '@/objects/LearningResource';
import addResource from '@/components/learning-resource/AddResource.vue';

export default {
  components: {
    StoredResources,
    AddResource
  },
  data() {
    return {
      selectedTab: 'stored-resources',
      storedResources: [
        new LearningResource(1, 'official guide', 'vvvdfsdf', 'https://vuejs.org/v2/guide/'),
        new LearningResource(2, 'google', 'Google', 'https://google.org')
      ]
    };
  },
  provide() {
    return {
      resources: this.storedResources,
      addResource: this.addResource,
      deleteResource: this.removeResource,
    };
  },
  methods: {
    selectTab(tab) {
      this.selectedTab = tab;
    },
    addResource(title, description, url) {
      const newResource = {
        id: new Date().toISOString(),
        title: title,
        description: description,
        link: url
      };

      this.storedResources.unshift(newResource);
      this.selectedTab = 'stored-resources';
    },
    removeResource(resourceId) {
      const redIndex = this.storedResources.findIndex(res => res.id === resourceId);
      this.storedResources.splice(redIndex, 1);
    }
  }
};
</script>

<template>
  <base-card>
    <base-button @click="selectTab('stored-resources')" :mode="selectedTab === 'stored-resources' ? null : 'flat'">Stored Resources
    </base-button>
    <base-button @click="selectTab('add-resource')" :mode="selectedTab === 'add-resource' ? null : 'flat'">Add Resource</base-button>
  </base-card>
  <keep-alive>
    <component :is="selectedTab"></component>
  </keep-alive>
</template>

<style scoped>

</style>