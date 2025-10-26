
export function useImagePreloader() {
  const preloadedImages = new Set<string>()

  function preloadImage(url: string): Promise<void> {
    return new Promise((resolve, reject) => {
      // If already preloaded, resolve immediately
      if (preloadedImages.has(url)) {
        resolve()
        return
      }

      const img = new Image()
      img.onload = () => {
        preloadedImages.add(url)
        resolve()
      }
      img.onerror = reject
      img.src = url
    })
  }

  function preloadImages(urls: string[]): Promise<void[]> {
    return Promise.all(urls.map(url => preloadImage(url).catch(() => {})))
  }

  return {
    preloadImage,
    preloadImages,
    preloadedImages
  }
}
