import { ref } from 'vue'
import axios from 'axios'
import { useSession } from './useSession'

export interface ImageMetadata {
  id: string
  filename: string
  sizeBytes: number
  lastModified: string
}

const API_BASE = 'http://localhost:5001/api/gallery'

export function useGallery() {
  const { getSessionHeader } = useSession()
  const currentImage = ref<ImageMetadata | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  async function fetchRandomImage() {
    isLoading.value = true
    error.value = null
    try {
      const response = await axios.get<ImageMetadata>(`${API_BASE}/random-image`, {
        headers: getSessionHeader()
      })
      currentImage.value = response.data
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Unknown error'
      currentImage.value = null
    } finally {
      isLoading.value = false
    }
  }

  function getImageUrl(imageId: string): string {
    return `${API_BASE}/image/${imageId}`
  }

  return {
    currentImage,
    isLoading,
    error,
    fetchRandomImage,
    getImageUrl
  }
}
