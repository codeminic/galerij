import { ref } from 'vue'
import axios from 'axios'
import { useSession } from './useSession'

export interface GallerySettings {
  interval: number
  autoPlay: boolean
}

const API_BASE = 'http://localhost:5001/api/gallery'

export function useSettings() {
  const { getSessionHeader } = useSession()
  const settings = ref<GallerySettings>({
    interval: 5000,
    autoPlay: true
  })
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  async function fetchSettings() {
    isLoading.value = true
    error.value = null
    try {
      const response = await axios.get<GallerySettings>(`${API_BASE}/settings`, {
        headers: getSessionHeader()
      })
      settings.value = response.data
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Unknown error'
    } finally {
      isLoading.value = false
    }
  }

  async function saveSettings(newSettings: GallerySettings) {
    isLoading.value = true
    error.value = null
    try {
      const response = await axios.post<GallerySettings>(`${API_BASE}/settings`, newSettings, {
        headers: getSessionHeader()
      })
      settings.value = response.data
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Unknown error'
    } finally {
      isLoading.value = false
    }
  }

  function updateInterval(interval: number) {
    settings.value.interval = interval
    saveSettings(settings.value)
  }

  function toggleAutoPlay() {
    settings.value.autoPlay = !settings.value.autoPlay
    saveSettings(settings.value)
  }

  return {
    settings,
    isLoading,
    error,
    fetchSettings,
    saveSettings,
    updateInterval,
    toggleAutoPlay
  }
}
