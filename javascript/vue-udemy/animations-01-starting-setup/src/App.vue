<template>
  <div class="container">
    <div class="block" :class="{ animate: animatedBlock }"></div>
    <button @click="animateBlock">Animate</button>
  </div>
  <div class="container">
    <transition
      :css="false"
      @before-enter="beforeEnter"
      @enter="enter"
      @after-enter="afterEnter"
      @before-leave="beforeLeave"
      @leave="leave"
      @after-leave="afterLeave"
      @enter-cancelled="enterCancelled"
      @leave-cancelled="leaveCancelled"
    >
      <p v-if="animatedParagraph">Hide And Seek??</p>
    </transition>
    <button @click="animateParagraph">Toggle Paragraph</button>
  </div>
  <div class="container">
    <transition name="fade-button" mode="out-in">
      <button @click="showUser" v-if="!showUsers">Show Users</button>
      <button @click="hideUser" v-else>Hide Users</button>
    </transition>
  </div>
  <base-modal @close="hideDialog" :open="dialogIsVisible">
    <p>This is a test dialog!</p>
    <button @click="hideDialog">Close it!</button>
  </base-modal>
  <div class="container">
    <button @click="showDialog">Show Dialog</button>
  </div>
</template>

<script>
export default {
  data() {
    return {
      dialogIsVisible: false,
      animatedBlock: false,
      animatedParagraph: false,
      showUsers: false,
      enterInterval: null,
      leaveInterval: null,
    };
  },
  methods: {
    showDialog() {
      this.dialogIsVisible = true;
    },
    hideDialog() {
      this.dialogIsVisible = false;
    },
    animateBlock() {
      this.animatedBlock = !this.animatedBlock;
    },
    animateParagraph() {
      this.animatedParagraph = !this.animatedParagraph;
    },
    showUser() {
      this.showUsers = true;
    },
    hideUser() {
      this.showUsers = false;
    },
    beforeEnter(el) {
      el.style.opacity = 0;
    },
    enter(el, done) {
      let round = 1;
      this.enterInterval = setInterval(() => {
        el.style.opacity = round * 0.1;
        round++;
        if (round > 10) {
          clearInterval(this.enterInterval);
          done();
        }
      }, 20);
    },
    afterEnter(el) {
      el.style.opacity = 1;
    },
    beforeLeave(el) {
      el.style.opacity = 1;
    },
    leave(el, done) {
      let round = 10;
      this.leaveInterval = setInterval(() => {
        el.style.opacity = round * 0.1;
        round--;
        if (round < 0) {
          clearInterval(this.leaveInterval);
          done();
        }
      }, 20);
    },
    afterLeave(el) {
      el.style.opacity = 0;
    },
    enterCancelled() {
      clearInterval(this.enterInterval);
    },
    leaveCancelled() {
      clearInterval(this.leaveInterval);
    },
  },
};
</script>

<style>
* {
  box-sizing: border-box;
}

html {
  font-family: sans-serif;
}

body {
  margin: 0;
}

button {
  font: inherit;
  padding: 0.5rem 2rem;
  border: 1px solid #810032;
  border-radius: 30px;
  background-color: #810032;
  color: white;
  cursor: pointer;
}

button:hover,
button:active {
  background-color: #a80b48;
  border-color: #a80b48;
}

.block {
  width: 8rem;
  height: 8rem;
  background-color: #290033;
  margin-bottom: 2rem;
  /* transition: transform 0.5s ease-out; */
}

.container {
  max-width: 40rem;
  margin: 2rem auto;
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  padding: 2rem;
  border: 2px solid #ccc;
  border-radius: 12px;
}

.animate {
  /* transform: translateX(-150px); */
  animation: slide-fade 1s ease-out forwards;
}

/*
.v-enter-from {
  opacity: 0;
  transform: translateY(-30px);
}

.v-enter-to {
  opacity: 1;
  transform: translateY(0);
}

.v-leave-to {
  opacity: 0;
  transform: translateY(-30px);
}

.v-leave-from {
  opacity: 1;
  transform: translateY(0);
}
*/

/* .para-enter-active {
  transition: all 0.3s ease-out;
  animation: slide-scale 0.5s ease-out;
}

.para-leave-active {
  transition: all 0.3s ease-in;
  animation: slide-scale 0.5s ease-in;
} 
*/

.fade-buton-enter-from,
.fade-buton-leave-to {
  opacity: 0;
}

.fade-buton-enter-active,
.fade-buton-leave-active {
  transition: opacity 0.5s ease-out;
}

.fade-buton-enter-to,
.fade-buton-leave-from {
  opacity: 1;
}

@keyframes slide-scale {
  0% {
    transform: translateX(0) scale(1);
  }

  70% {
    transform: translateX(-120px) scale(1.1);
  }

  100% {
    transform: translateX(-150px) scale(1);
  }
}

@keyframes modal {
  from {
    opacity: 0;
    transform: translateY(-3rem);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
