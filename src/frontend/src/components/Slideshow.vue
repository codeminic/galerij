<template>
  <div class="relative w-full h-full bg-black flex items-center justify-center overflow-hidden">
    <!-- Blurred Background with Transition -->
    <Transition
      mode="out-in"
      enter-active-class="transition-opacity duration-300"
      leave-active-class="transition-opacity duration-300"
      enter-from-class="opacity-0"
      leave-to-class="opacity-0"
    >
      <div
        v-if="currentImage"
        :key="`bg-${currentImage.id}`"
        class="absolute inset-0 w-full h-full"
        :style="{
          backgroundImage: `url('${getImageUrl(currentImage.id)}')`,
          backgroundSize: 'cover',
          backgroundPosition: 'center',
          filter: 'blur(20px) brightness(0.6)',
          zIndex: 0
        }"
      />
    </Transition>

    <!-- Content Container with Transition -->
    <Transition
      mode="out-in"
      enter-active-class="transition-opacity duration-300"
      leave-active-class="transition-opacity duration-300"
      enter-from-class="opacity-0"
      leave-to-class="opacity-0"
    >
      <div
        v-if="currentImage"
        :key="currentImage.id"
        class="relative w-full h-full flex items-center justify-center z-10"
      >
        <!-- Main Image -->
        <img
          :src="getImageUrl(currentImage.id)"
          :alt="currentImage.filename"
          class="w-full h-full object-contain"
        />
      </div>
    </Transition>

    <!-- No Images State -->
    <div v-if="!currentImage" class="relative text-center text-white z-10">
      <p v-if="isLoading" class="text-lg">Loading images...</p>
      <p v-else-if="error" class="text-lg text-red-400">{{ error }}</p>
      <p v-else class="text-lg">No images available</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { watch, onMounted } from 'vue'
import { useImagePreloader } from '../composables/useImagePreloader'

interface Props {
  currentImage: any
  isLoading: boolean
  error: string | null
  getImageUrl: (id: string) => string
}

const props = defineProps<Props>()
const { preloadImage } = useImagePreloader()

// Preload current image immediately when component mounts
onMounted(() => {
  if (props.currentImage) {
    preloadImage(props.getImageUrl(props.currentImage.id))
  }
})

// Preload image whenever current image changes
watch(
  () => props.currentImage,
  (newImage) => {
    if (newImage) {
      preloadImage(props.getImageUrl(newImage.id))
    }
  }
)
</script>
