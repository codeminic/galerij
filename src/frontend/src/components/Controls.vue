<template>
  <div class="text-white p-6 flex items-center justify-between w-full">
    <!-- Empty spacer (left) -->
    <div class="w-8"></div>

    <!-- Playback Controls (center) -->
    <div class="flex items-center gap-8">
      <!-- Previous Button -->
      <button
        @click="() => emit('previous')"
        :disabled="!hasImages"
        class="text-white hover:text-gray-400 disabled:text-gray-600 disabled:cursor-not-allowed transition"
        :class="{ 'opacity-50': !hasImages }"
      >
        <svg class="w-8 h-8" fill="currentColor" viewBox="0 0 24 24">
          <path d="M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z" />
        </svg>
      </button>

      <!-- Play/Pause Button -->
      <button
        @click="() => emit('toggle-play-pause')"
        :disabled="!hasImages"
        class="text-white hover:text-gray-400 disabled:text-gray-600 disabled:cursor-not-allowed transition"
        :class="{ 'opacity-50': !hasImages }"
      >
        <svg v-if="isPlaying" class="w-8 h-8" fill="currentColor" viewBox="0 0 24 24">
          <path d="M6 4h4v16H6V4zm8 0h4v16h-4V4z" />
        </svg>
        <svg v-else class="w-8 h-8" fill="currentColor" viewBox="0 0 24 24">
          <path d="M8 5v14l11-7z" />
        </svg>
      </button>

      <!-- Next Button -->
      <button
        @click="() => emit('next')"
        :disabled="!hasImages"
        class="text-white hover:text-gray-400 disabled:text-gray-600 disabled:cursor-not-allowed transition"
        :class="{ 'opacity-50': !hasImages }"
      >
        <svg class="w-8 h-8" fill="currentColor" viewBox="0 0 24 24">
          <path d="M10 6l-1.41 1.41L12.17 12l-3.58 4.59L10 18l6-6z" />
        </svg>
      </button>
    </div>

    <!-- Settings Button (right) -->
    <button
      @click="() => emit('open-settings')"
      class="text-white hover:text-gray-400 transition"
    >
      <svg class="w-8 h-8" fill="currentColor" viewBox="0 0 24 24">
        <circle cx="12" cy="6" r="2" />
        <circle cx="12" cy="12" r="2" />
        <circle cx="12" cy="18" r="2" />
      </svg>
    </button>
  </div>
</template>

<script setup lang="ts">
interface Props {
  isPlaying: boolean
  hasImages: boolean
}

defineProps<Props>()

const emit = defineEmits<{
  'toggle-play-pause': []
  'next': []
  'previous': []
  'open-settings': []
}>()
</script>
