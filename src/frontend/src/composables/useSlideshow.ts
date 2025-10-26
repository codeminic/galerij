import { ref, watch } from 'vue'

export function useSlideshow(fetchRandomImage: () => Promise<void>, settings: any) {
  const isPlaying = ref(false)
  let intervalId: ReturnType<typeof setInterval> | null = null

  function play() {
    if (isPlaying.value) return
    isPlaying.value = true
    startAutoAdvance()
  }

  function pause() {
    isPlaying.value = false
    stopAutoAdvance()
  }

  function togglePlayPause() {
    if (isPlaying.value) {
      pause()
    } else {
      play()
    }
  }

  function next() {
    fetchRandomImage()
  }

  function previous() {
    fetchRandomImage()
  }

  function startAutoAdvance() {
    stopAutoAdvance()
    intervalId = setInterval(() => {
      fetchRandomImage()
    }, settings.value.interval)
  }

  function stopAutoAdvance() {
    if (intervalId) {
      clearInterval(intervalId)
      intervalId = null
    }
  }

  // Auto-play when settings change
  watch(
    () => [settings.value.autoPlay, settings.value.interval],
    ([autoPlay]) => {
      if (autoPlay && !isPlaying.value) {
        play()
      } else if (!autoPlay && isPlaying.value) {
        pause()
      }
    }
  )

  // Update interval if settings change
  watch(
    () => settings.value.interval,
    () => {
      if (isPlaying.value) {
        stopAutoAdvance()
        startAutoAdvance()
      }
    }
  )

  return {
    isPlaying,
    togglePlayPause,
    next,
    previous,
    play,
    pause
  }
}
