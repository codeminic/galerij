<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Slideshow from './components/Slideshow.vue'
import Controls from './components/Controls.vue'
import Settings from './components/Settings.vue'
import { useGallery } from './composables/useGallery'
import { useSettings } from './composables/useSettings'
import { useSlideshow } from './composables/useSlideshow'

const settingsComponent = ref<InstanceType<typeof Settings>>()
const showControls = ref(false)
let hideControlsTimeout: ReturnType<typeof setTimeout> | null = null

// Composables
const { currentImage, error, isLoading, fetchRandomImage, getImageUrl } = useGallery()
const { settings, updateInterval, toggleAutoPlay, fetchSettings } = useSettings()
const { isPlaying, togglePlayPause, next, previous } = useSlideshow(fetchRandomImage, settings)

onMounted(async () => {
  await Promise.all([fetchRandomImage(), fetchSettings()])
  // Start with play if autoPlay is enabled
  if (settings.value.autoPlay) {
    // The watch in useSlideshow will handle this
  }
})

function openSettings() {
  settingsComponent.value?.open()
}

function closeSettings() {
  settingsComponent.value?.close()
}

function handleMouseMove() {
  showControls.value = true

  // Clear existing timeout
  if (hideControlsTimeout) {
    clearTimeout(hideControlsTimeout)
  }

  // Hide controls after 3 seconds of no movement
  hideControlsTimeout = setTimeout(() => {
    showControls.value = false
  }, 3000)
}

function handleKeyDown(event: KeyboardEvent) {
  // Ignore keyboard input if Settings modal is open
  if (settingsComponent.value?.isOpen) {
    return
  }

  switch (event.key) {
    case 'ArrowRight':
      event.preventDefault()
      next()
      break
    case 'ArrowLeft':
      event.preventDefault()
      previous()
      break
    case ' ':
      event.preventDefault()
      togglePlayPause()
      break
  }
}
</script>

<template>
  <div class="w-screen h-screen relative bg-black" @mousemove="handleMouseMove" @keydown="handleKeyDown" :style="{ cursor: showControls ? 'auto' : 'none' }">
    <!-- Main Slideshow Area -->
    <div class="w-full h-full overflow-hidden">
      <Slideshow
        :current-image="currentImage"
        :is-loading="isLoading"
        :error="error"
        :get-image-url="getImageUrl"
      />
    </div>

    <!-- Control Bar (Overlaid) -->
    <Transition
      enter-active-class="transition-opacity duration-200"
      leave-active-class="transition-opacity duration-200"
      enter-from-class="opacity-0"
      leave-to-class="opacity-0"
    >
      <div v-if="showControls" class="absolute bottom-0 left-0 right-0 z-20">
        <Controls
          :is-playing="isPlaying"
          :has-images="!!currentImage"
          @toggle-play-pause="togglePlayPause"
          @next="next"
          @previous="previous"
          @open-settings="openSettings"
        />
      </div>
    </Transition>

    <!-- Settings Modal -->
    <Settings
      ref="settingsComponent"
      :interval="settings.interval"
      :auto-play="settings.autoPlay"
      :error="error"
      @update-interval="updateInterval"
      @toggle-auto-play="toggleAutoPlay"
      @close="closeSettings"
    />
  </div>
</template>

<style scoped></style>
