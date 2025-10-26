<template>
  <Transition
    enter-active-class="transition-opacity duration-200"
    leave-active-class="transition-opacity duration-200"
    enter-from-class="opacity-0"
    leave-to-class="opacity-0"
  >
    <div v-if="isOpen" class="fixed inset-0 flex items-center justify-center z-50" @click="close">
      <div class="relative text-white rounded-lg p-8 max-w-sm w-full mx-4 backdrop-blur-xl" @click.stop>
        <!-- Dark semi-transparent overlay behind content -->
        <div class="absolute inset-0 bg-black opacity-40 rounded-lg"></div>
        <!-- Content wrapper -->
        <div class="relative z-10">
        <!-- Title -->
        <h2 class="text-3xl font-light mb-8 text-center">Settings</h2>

        <!-- Interval Control -->
        <div class="mb-10">
          <div class="flex items-center justify-between mb-4">
            <label class="text-base text-gray-300">Interval</label>
            <span class="text-base font-medium">{{ (interval / 1000).toFixed(1) }}s</span>
          </div>
          <input
            type="range"
            min="1000"
            max="30000"
            step="1000"
            :value="interval"
            @input="(e) => updateInterval(Number((e.target as HTMLInputElement).value))"
            class="w-full h-1 bg-gray-400 rounded-lg appearance-none cursor-pointer accent-white"
          />
        </div>

        <!-- Auto Play Toggle -->
        <div class="mb-8 flex items-center justify-between">
          <label class="text-base text-gray-300">Auto Play</label>
          <button
            @click="toggleAutoPlay"
            :class="autoPlay ? 'bg-white' : 'bg-gray-700'"
            class="relative w-12 h-6 rounded-full transition-colors"
          >
            <div
              :class="autoPlay ? 'translate-x-6' : 'translate-x-1'"
              class="absolute top-1 w-4 h-4 bg-black rounded-full transition-transform"
            ></div>
          </button>
        </div>

        <!-- Error Message -->
        <p v-if="error" class="text-red-400 text-sm mb-6 text-center">{{ error }}</p>

        <!-- Save Button (subtle) -->
        <button
          @click="close"
          class="w-full py-2 text-base text-gray-300 hover:text-white transition border border-gray-400 rounded-lg"
        >
          Save
        </button>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

interface Props {
  interval: number
  autoPlay: boolean
  error?: string | null
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'update-interval': [interval: number]
  'toggle-auto-play': []
  'close': []
}>()

const isOpen = ref(false)

const interval = computed(() => props.interval)
const autoPlay = computed(() => props.autoPlay)
const error = computed(() => props.error)

function open() {
  isOpen.value = true
}

function close() {
  isOpen.value = false
  emit('close')
}

function updateInterval(newInterval: number) {
  emit('update-interval', newInterval)
}

function toggleAutoPlay() {
  emit('toggle-auto-play')
}

defineExpose({
  open,
  close,
  isOpen
})
</script>
