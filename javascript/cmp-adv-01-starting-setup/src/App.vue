<template>
  <div>
    <the-header></the-header>
    <base-card>
      <badge-list></badge-list>
    </base-card>
    <base-card>
      <user-info
          :full-name="activeUser.name"
          :info-text="activeUser.description"
          :role="activeUser.role"
      ></user-info>
    </base-card>
  </div>
</template>

<script>
import TheHeader from "@/components/TheHeader.vue";
import UserInfo from "@/components/UserInfo.vue";
import BadgeList from "@/components/BadgeList.vue";

export default {
  components: {
    TheHeader,
    UserInfo,
    BadgeList,
  },
  data() {
    return {
      activeUser: {
        name: 'Maximilian Schwarzmüller',
        description: 'Site owner and admin',
        role: 'admin',
      },
    };
  },
};
</script>

<style>
html {
  font-family: sans-serif;
}

body {
  margin: 0;
}
</style>